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

    public Task<IslemSonucu> KayitOl(string kullaniciAdi, string email, string sifre, string sifreTekrar)
    {
        // TODO (Mevlüt):
        // 1) sifre != sifreTekrar ise IslemSonucu.Hata("Şifreler eşleşmiyor")
        // 2) KullaniciVarMi(kullaniciAdi, email) ise IslemSonucu.Hata("Bu kullanıcı adı veya e-posta kullanılıyor")
        // 3) var ogrenci = new Student(kullaniciAdi, email, _hasher.Hashle(sifre));
        // 4) _db.Students.Add(ogrenci); await _db.SaveChangesAsync();
        // 5) IslemSonucu.Ok("Kayıt başarılı")
        throw new NotImplementedException();
    }

    public Task<GirisSonucu> GirisYap(string kullaniciAdiVeyaEmail, string sifre)
    {
        // TODO (Mevlüt):
        // 1) var user = KullaniciBul(kullaniciAdiVeyaEmail);
        //    null ise -> LogKaydet(girdi, false), "Kullanıcı adı veya şifre hatalı"
        // 2) user.KilitliMi() ise -> LogKaydet(..., false), "Hesabınız 15 dakika kilitli"
        // 3) _hasher.Dogrula(sifre, user.SifreHash) yanlışsa
        //    -> user.HataliGirisKaydet(); LogKaydet(..., false); SaveChanges; hata mesajı
        // 4) Doğruysa -> user.SayaciSifirla(); LogKaydet(..., true); SaveChanges;
        //    await _http.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ClaimsOlustur(user));
        // 5) GirisSonucu { Basarili = true, Kullanici = user }
        throw new NotImplementedException();
    }

    public Task CikisYap()
    {
        // TODO (Mevlüt): await _http.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        throw new NotImplementedException();
    }

    /// <summary>Cookie'deki kullanıcı Id'sinden giriş yapmış kullanıcıyı getirir (controller'lar kullanır).</summary>
    public Task<User?> AktifKullaniciGetir()
    {
        // TODO (Mevlüt):
        // var idText = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        // int.TryParse ile çevir, başarısızsa null
        // return await _db.Users.FindAsync(id);
        throw new NotImplementedException();
    }

    private bool KullaniciVarMi(string kullaniciAdi, string email)
    {
        // TODO: _db.Users.Any(u => u.KullaniciAdi == kullaniciAdi || u.Email == email)
        throw new NotImplementedException();
    }

    private User? KullaniciBul(string girdi)
    {
        // TODO: girdi '@' içeriyorsa Email'e, içermiyorsa KullaniciAdi'na göre ara
        throw new NotImplementedException();
    }

    private void LogKaydet(string kullaniciAdi, bool basariliMi)
    {
        // TODO: _db.LoginLogs.Add(new LoginLog { KullaniciAdi = kullaniciAdi, BasariliMi = basariliMi });
        throw new NotImplementedException();
    }

    private ClaimsPrincipal ClaimsOlustur(User user)
    {
        // TODO: NameIdentifier = user.Id, Name = user.KullaniciAdi, Role = user.Rol.ToString()
        //       new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
        throw new NotImplementedException();
    }
}
