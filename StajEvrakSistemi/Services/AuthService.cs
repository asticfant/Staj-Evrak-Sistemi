using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

/// <summary>Gereksinim 4.1 – kayıt, giriş, çıkış, loglama, kilitleme.</summary>
public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IHttpContextAccessor _http;

    public AuthService(AppDbContext db, IPasswordHasher hasher, IHttpContextAccessor http)
    {
        _db = db;
        _hasher = hasher;
        _http = http;
    }

    /// <summary>Öğrencinin kendi hesabını oluşturması. Şifre hash'lenerek saklanır.</summary>
    public async Task<IslemSonucu> KayitOl(string kullaniciAdi, string email, string sifre, string sifreTekrar)
    {
        // 1) Şifreler aynı mı?
        if (sifre != sifreTekrar)
        {
            return IslemSonucu.Hata("Şifreler eşleşmiyor");
        }

        // 2) Kullanıcı adı veya e-posta daha önce alınmış mı?
        if (KullaniciVarMi(kullaniciAdi, email))
        {
            return IslemSonucu.Hata("Bu kullanıcı adı veya e-posta kullanılıyor");
        }

        // 3) Yeni öğrenci (şifre düz değil, hash olarak)
        var ogrenci = new Student(kullaniciAdi, email, _hasher.Hashle(sifre));

        // 4) Veritabanına kaydet
        _db.Students.Add(ogrenci);
        await _db.SaveChangesAsync();

        // 5) Başarılı
        return IslemSonucu.Ok("Kayıt başarılı");
    }

    /// <summary>
    /// Kullanıcı adı veya e-posta + şifre ile giriş.
    /// Her deneme loglanır; 5 hatalı denemede hesap 15 dakika kilitlenir.
    /// </summary>
    public async Task<GirisSonucu> GirisYap(string kullaniciAdiVeyaEmail, string sifre)
    {
        // 1) Kullanıcı var mı?
        var user = KullaniciBul(kullaniciAdiVeyaEmail);
        if (user == null)
        {
            LogKaydet(kullaniciAdiVeyaEmail, false);
            await _db.SaveChangesAsync();
            return new GirisSonucu
            {
                Basarili = false,
                Mesaj = "Kullanıcı adı veya şifre hatalı"
            };
        }

        // 2) Hesap kilitli mi? (Kilitliyse şifreye hiç bakılmaz)
        if (user.KilitliMi())
        {
            LogKaydet(user.KullaniciAdi, false);
            await _db.SaveChangesAsync();
            return new GirisSonucu
            {
                Basarili = false,
                Mesaj = "Hesabınız 15 dakika kilitli"
            };
        }

        // 3) Şifre yanlış mı?
        if (!_hasher.Dogrula(sifre, user.SifreHash))
        {
            user.HataliGirisKaydet();
            LogKaydet(user.KullaniciAdi, false);
            await _db.SaveChangesAsync();
            return new GirisSonucu
            {
                Basarili = false,
                Mesaj = user.KilitliMi()
                    ? "5 hatalı deneme yaptınız, hesabınız 15 dakika kilitlendi"
                    : "Kullanıcı adı veya şifre hatalı"
            };
        }

        // 4) Şifre doğru: sayacı sıfırla, logla, kaydet, cookie ile giriş yap
        user.SayaciSifirla();
        LogKaydet(user.KullaniciAdi, true);
        await _db.SaveChangesAsync();

        await _http.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimsOlustur(user));

        // 5) Başarılı
        return new GirisSonucu
        {
            Basarili = true,
            Mesaj = "Giriş başarılı",
            Kullanici = user
        };
    }

    /// <summary>Cookie'yi siler; oturum sona erer.</summary>
    public async Task CikisYap()
    {
        await _http.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    /// <summary>Cookie'deki kullanıcı Id'sinden giriş yapmış kullanıcıyı getirir (controller'lar kullanır).</summary>
    public async Task<User?> AktifKullaniciGetir()
    {
        var idText = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idText, out var id))
        {
            return null;
        }

        return await _db.Users.FindAsync(id);
    }

    /// <summary>Kullanıcı adı veya e-posta daha önce kullanılmış mı?</summary>
    private bool KullaniciVarMi(string kullaniciAdi, string email)
    {
        return _db.Users.Any(u => u.KullaniciAdi == kullaniciAdi || u.Email == email);
    }

    /// <summary>Girdide '@' varsa e-postaya, yoksa kullanıcı adına göre arar.</summary>
    private User? KullaniciBul(string girdi)
    {
        if (girdi.Contains('@'))
        {
            return _db.Users.FirstOrDefault(u => u.Email == girdi);
        }
        else
        {
            return _db.Users.FirstOrDefault(u => u.KullaniciAdi == girdi);
        }
    }

    /// <summary>
    /// Her giriş denemesi için log satırı hazırlar.
    /// Kaydetmez; SaveChangesAsync'i çağıran metot kaydeder.
    /// </summary>
    private void LogKaydet(string kullaniciAdi, bool basariliMi)
    {
        _db.LoginLogs.Add(new LoginLog
        {
            KullaniciAdi = kullaniciAdi,
            BasariliMi = basariliMi,
            Tarih = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Cookie'ye yazılacak kimliği hazırlar (Id, kullanıcı adı, rol).
    /// Rol bilgisi [Authorize(Roles = ...)] kontrollerinde kullanılır.
    /// </summary>
    private ClaimsPrincipal ClaimsOlustur(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.KullaniciAdi),
            new Claim(ClaimTypes.Role, user.Rol.ToString())
        };

        var kimlik = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(kimlik);
    }
}