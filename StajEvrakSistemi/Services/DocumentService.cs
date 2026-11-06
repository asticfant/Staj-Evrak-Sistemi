using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

/// <summary>
/// Gereksinim 4.4 ve 5 – belge yükleme, düzeltme, durum işlemleri.
/// Vize kontrolleri (hocanın cevabı): sadece dosya formatı / uzantı (+ imza ve boyut).
/// </summary>
public class DocumentService
{
    public const long MaxDosyaBoyutu = 10 * 1024 * 1024; // 10 MB

    private static readonly string[] IzinliUzantilar = { ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png" };

    // Her dosya türünün ilk byte'ları ("sihirli sayı"). Uzantısı değiştirilmiş dosyaları yakalar.
    private static readonly Dictionary<string, byte[]> DosyaImzalari = new()
    {
        [".pdf"] = new byte[] { 0x25, 0x50, 0x44, 0x46 }, // %PDF
        [".docx"] = new byte[] { 0x50, 0x4B, 0x03, 0x04 }, // ZIP (docx aslında zip)
        [".doc"] = new byte[] { 0xD0, 0xCF, 0x11, 0xE0 }, // eski Word
        [".jpg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".jpeg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47 }
    };

    private readonly AppDbContext _db;
    private readonly BelgeIslemeService _isleme;
    private readonly IWebHostEnvironment _env;

    public DocumentService(AppDbContext db, BelgeIslemeService isleme, IWebHostEnvironment env)
    {
        _db = db;
        _isleme = isleme;
        _env = env;
    }

    /// <summary>
    /// Öğrencinin Ek-16 belgesini staj formuyla birlikte yükler.
    /// Profil eksikse, dosya boş/büyükse, uzantı veya içerik uygun değilse reddeder.
    /// </summary>
    public async Task<YuklemeSonucu> BelgeYukle(IFormFile dosya, StajBilgisi staj, Student student)
    {
        // 0) Profil dolu mu? (Karşılaştırma profil bilgilerine dayanıyor)
        if (!student.ProfilTamamMi())
        {
            return Hata("Belge yüklemeden önce profil bilgilerinizi doldurun");
        }

        // 1) Dosya kontrolleri
        if (dosya == null || !BoyutGecerliMi(dosya))
        {
            return Hata("Dosya boş ya da 10 MB'tan büyük");
        }

        if (!UzantiGecerliMi(dosya.FileName))
        {
            return Hata("Sadece Word (.doc, .docx), PDF, JPEG ve PNG dosyaları kabul edilir");
        }

        if (!ImzaGecerliMi(dosya))
        {
            return Hata("Dosyanın içeriği uzantısıyla uyuşmuyor");
        }

        // 2) Dosyayı uploads klasörüne benzersiz bir adla kaydet
        var uzanti = Path.GetExtension(dosya.FileName).ToLowerInvariant();
        var klasor = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(klasor); // klasör yoksa oluşturur, varsa bir şey yapmaz

        var tamYol = Path.Combine(klasor, $"{Guid.NewGuid()}{uzanti}");
        using (var akis = new FileStream(tamYol, FileMode.Create))
        {
            await dosya.CopyToAsync(akis);
        }

        // 3) Veritabanına belge kaydı (+ staj bilgisi aynı anda)
        var belge = new Document
        {
            StudentId = student.Id,
            DosyaYolu = tamYol,
            DosyaTuru = uzanti switch
            {
                ".doc" => DosyaTuru.Doc,
                ".docx" => DosyaTuru.Docx,
                ".pdf" => DosyaTuru.Pdf,
                ".png" => DosyaTuru.Png,
                _ => DosyaTuru.Jpeg // .jpg ve .jpeg
            },
            StajBilgisi = staj
        };

        _db.Documents.Add(belge);
        await _db.SaveChangesAsync();

        // 4) Belgeyi işle (oku → karşılaştır → durumu belirle)
        // TODO (B7 bitince yorumu kaldır):
         await _isleme.Isle(belge);

        return new YuklemeSonucu
        {
            Basarili = true,
            Mesaj = "Belge yüklendi",
            Belge = belge
        };
    }

    /// <summary>
    /// Hatalı/Eksik belgenin staj bilgilerini düzeltir ve belgeyi yeniden kontrole sokar.
    /// Öğrenci sadece kendi belgesini düzeltebilir.
    /// </summary>
    public async Task<YuklemeSonucu> StajBilgisiDuzelt(int documentId, StajBilgisi staj, Student student)
    {
        var belge = await _db.Documents
            .Include(d => d.StajBilgisi)
            .Include(d => d.Hatalar)
            .Include(d => d.OkunanAlanlar)
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (belge == null || belge.StudentId != student.Id)
        {
            return Hata("Belge bulunamadı");
        }

        if (belge.Durum != DocumentDurum.HataliEksik)
        {
            return Hata("Sadece Hatalı/Eksik durumundaki belgenin bilgileri düzeltilebilir");
        }

        // Yeni bilgileri mevcut kayda kopyala
        var mevcut = belge.StajBilgisi!;
        mevcut.Ek16TeslimYili = staj.Ek16TeslimYili;
        mevcut.StajBaslangic = staj.StajBaslangic;
        mevcut.StajBitis = staj.StajBitis;
        mevcut.CalisilanGunler = staj.CalisilanGunler;
        mevcut.IsyeriAdi = staj.IsyeriAdi;
        mevcut.IsyeriAdresi = staj.IsyeriAdresi;
        mevcut.IsyeriTelefon = staj.IsyeriTelefon;
        mevcut.IsyeriEmail = staj.IsyeriEmail;
        mevcut.MuhendisSayisi = staj.MuhendisSayisi;
        mevcut.MuhendisBolumu = staj.MuhendisBolumu;

        // Eski sonuçları temizle, belgeyi tekrar kontrole al
        _db.ValidationIssues.RemoveRange(belge.Hatalar);
        _db.OkunanAlanlar.RemoveRange(belge.OkunanAlanlar);
        belge.YenidenKontrolEt();
        await _db.SaveChangesAsync();

        
        await _isleme.Isle(belge);

        return new YuklemeSonucu
        {
            Basarili = true,
            Mesaj = "Bilgiler güncellendi, belge yeniden kontrol ediliyor",
            Belge = belge
        };
    }

    /// <summary>Admin onaylanmış belgeyi tamamlar.</summary>
    public async Task Tamamla(int documentId, Admin yapan)
    {
        var belge = await BelgeyiGetir(documentId);
        belge.Tamamla(); // Onaylandı değilse Document hata fırlatır
        await _db.SaveChangesAsync();

        // Final: Google Form'a gönderim (IGoogleFormService)
    }

    /// <summary>Admin'in elle durum değiştirmesi (Tamamlandı belgeler dahil).</summary>
    public async Task DurumGuncelle(int documentId, DocumentDurum yeniDurum, User yapan)
    {
        var belge = await BelgeyiGetir(documentId);
        belge.DurumAta(yeniDurum, yapan.Rol); // Admin değilse Document hata fırlatır
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Belgeyi ve dosyasını siler. Tamamlandı belgeyi sadece Admin silebilir;
    /// öğrenci sadece kendi belgesini silebilir.
    /// </summary>
    public async Task Sil(int documentId, User yapan)
    {
        var belge = await BelgeyiGetir(documentId);

        if (!belge.DegistirilebilirMi(yapan.Rol))
        {
            throw new UnauthorizedAccessException("Tamamlanmış belgeyi sadece Admin silebilir.");
        }

        if (yapan.Rol == UserRole.Student && belge.StudentId != yapan.Id)
        {
            throw new UnauthorizedAccessException("Başka bir öğrencinin belgesini silemezsiniz.");
        }

        if (File.Exists(belge.DosyaYolu))
        {
            File.Delete(belge.DosyaYolu);
        }

        // StajBilgisi, OkunanAlanlar ve Hatalar cascade ile birlikte silinir
        _db.Documents.Remove(belge);
        await _db.SaveChangesAsync();
    }

    // ---------------- Yardımcılar ----------------

    private bool UzantiGecerliMi(string dosyaAdi)
    {
        var uzanti = Path.GetExtension(dosyaAdi).ToLowerInvariant();
        return IzinliUzantilar.Contains(uzanti);
    }

    private bool ImzaGecerliMi(IFormFile dosya)
    {
        var uzanti = Path.GetExtension(dosya.FileName).ToLowerInvariant();
        if (!DosyaImzalari.TryGetValue(uzanti, out var beklenen))
        {
            return false;
        }

        var baslik = new byte[beklenen.Length];
        using var akis = dosya.OpenReadStream();
        var okunan = akis.Read(baslik, 0, baslik.Length);

        return okunan == beklenen.Length && baslik.SequenceEqual(beklenen);
    }

    private bool BoyutGecerliMi(IFormFile dosya)
    {
        return dosya.Length > 0 && dosya.Length <= MaxDosyaBoyutu;
    }

    private async Task<Document> BelgeyiGetir(int documentId)
    {
        return await _db.Documents.FindAsync(documentId)
            ?? throw new KeyNotFoundException("Belge bulunamadı.");
    }

    private static YuklemeSonucu Hata(string mesaj)
    {
        return new YuklemeSonucu { Basarili = false, Mesaj = mesaj };
    }
}