using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Readers;

namespace StajEvrakSistemi.Services;

/// <summary>
/// Yüklenen belgeyi sırasıyla işler: oku -> kaydet -> karşılaştır -> durumu belirle.
/// </summary>
public class BelgeIslemeService
{
    private readonly AppDbContext _db;
    private readonly OkuyucuSecici _okuyucuSecici;
    private readonly ComparisonService _karsilastirma;

    public BelgeIslemeService(AppDbContext db, OkuyucuSecici okuyucuSecici, ComparisonService karsilastirma)
    {
        _db = db;
        _okuyucuSecici = okuyucuSecici;
        _karsilastirma = karsilastirma;
    }

    public Task Isle(Document document)
    {
        // TODO (Mevlüt / Muhammed):
        // 1) document.Durum Yuklendi ise document.IncelemeyeAl()
        // 2) var okuyucu = _okuyucuSecici.Sec(document.DosyaTuru);
        //    okuyucu null ise (vizede resim / .doc) -> belge Inceleniyor'da kalır,
        //    tek bir ValidationIssue { Tur = Okunamadi, Mesaj = "Bu belge türü finalde otomatik okunacak" } ekle, SaveChanges, çık
        // 3) var alanlar = okuyucu.Oku(document.DosyaYolu);
        //    document.OkunanAlanlar = alanlar.Select(a => new OkunanAlan { AlanAdi = a.Key, Deger = a.Value }).ToList();
        // 4) var hatalar = _karsilastirma.FormIleKarsilastir(document.Student!, document.StajBilgisi!, document.OkunanAlanlar);
        //    document.Hatalar = hatalar;
        // 5) Uyusmazlik türünde hata varsa document.HataliOlarakIsaretle(), yoksa document.Onayla()
        // 6) SaveChanges
        // Final: 4. adımdan sonra IFieldValidator ve IBelgeKurali listeleri de çalıştırılacak (Bölüm 6)
        throw new NotImplementedException();
    }
}
