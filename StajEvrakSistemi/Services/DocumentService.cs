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

    private readonly AppDbContext _db;
    private readonly BelgeIslemeService _isleme;
    private readonly IWebHostEnvironment _env;

    public DocumentService(AppDbContext db, BelgeIslemeService isleme, IWebHostEnvironment env)
    {
        _db = db;
        _isleme = isleme;
        _env = env;
    }

    public Task<YuklemeSonucu> BelgeYukle(IFormFile dosya, StajBilgisi staj, Student student)
    {
        // TODO (Mevlüt):
        // 0) student.ProfilTamamMi() değilse -> "Önce profil bilgilerinizi doldurun"
        // 1) UzantiGecerliMi / ImzaGecerliMi / BoyutGecerliMi -> geçersizse YuklemeSonucu hata + uyarı mesajı
        // 2) dosyayı Path.Combine(_env.ContentRootPath, "uploads", Guid.NewGuid() + uzanti) altına kaydet
        // 3) new Document { StudentId, DosyaYolu, DosyaTuru, StajBilgisi = staj } -> Add, SaveChanges
        // 4) await _isleme.Isle(belge);
        // 5) YuklemeSonucu { Basarili = true, Belge = belge }
        throw new NotImplementedException();
    }

    public Task<YuklemeSonucu> StajBilgisiDuzelt(int documentId, StajBilgisi staj, Student student)
    {
        // TODO (Mevlüt):
        // 1) belgeyi StajBilgisi ile birlikte getir, öğrencinin kendi belgesi mi kontrol et
        // 2) belge.Durum HataliEksik değilse hata
        // 3) StajBilgisi alanlarını güncelle
        // 4) belge.YenidenKontrolEt(); eski Hatalar'ı sil; SaveChanges
        // 5) await _isleme.Isle(belge);
        throw new NotImplementedException();
    }

    public Task Tamamla(int documentId, Admin yapan)
    {
        // TODO (Mevlüt): belge.Tamamla(); SaveChanges
        // Final: ardından IGoogleFormService.Gonder(...)
        throw new NotImplementedException();
    }

    public Task DurumGuncelle(int documentId, DocumentDurum yeniDurum, User yapan)
    {
        // TODO (Mevlüt): belge.DurumAta(yeniDurum, yapan.Rol); SaveChanges
        throw new NotImplementedException();
    }

    public Task Sil(int documentId, User yapan)
    {
        // TODO (Mevlüt):
        // 1) belge.DegistirilebilirMi(yapan.Rol) false ise UnauthorizedAccessException
        // 2) öğrenci siliyorsa belge kendisinin mi kontrol et
        // 3) dosyayı diskten sil, kaydı sil, SaveChanges
        throw new NotImplementedException();
    }

    private bool UzantiGecerliMi(string dosyaAdi)
    {
        // TODO: IzinliUzantilar.Contains(Path.GetExtension(dosyaAdi).ToLowerInvariant())
        throw new NotImplementedException();
    }

    private bool ImzaGecerliMi(IFormFile dosya)
    {
        // TODO: dosyanın ilk byte'larına bak (uzantısı .pdf olan dosya gerçekten PDF mi?)
        //  PDF  : 25 50 44 46 ("%PDF")
        //  DOCX : 50 4B 03 04 (zip)
        //  DOC  : D0 CF 11 E0
        //  JPEG : FF D8 FF
        //  PNG  : 89 50 4E 47
        throw new NotImplementedException();
    }

    private bool BoyutGecerliMi(IFormFile dosya)
    {
        // TODO: dosya.Length > 0 && dosya.Length <= MaxDosyaBoyutu
        throw new NotImplementedException();
    }
}
