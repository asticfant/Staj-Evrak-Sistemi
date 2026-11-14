using Microsoft.EntityFrameworkCore;
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

    public async Task Isle(Document document)
    {
        // Öğrenci ve staj bilgisi yüklü değilse getir (karşılaştırma ikisine de bakar)
        if (document.Student == null)
        {
            await _db.Entry(document).Reference(d => d.Student).LoadAsync();
        }
        if (document.StajBilgisi == null)
        {
            await _db.Entry(document).Reference(d => d.StajBilgisi).LoadAsync();
        }

        // 1) Yeni yüklenen belge kontrole alınır (yeniden kontrolde zaten Inceleniyor'dadır)
        if (document.Durum == DocumentDurum.Yuklendi)
        {
            document.IncelemeyeAl();
        }

        // 2) Dosya türüne uygun okuyucu
        var okuyucu = _okuyucuSecici.Sec(document.DosyaTuru);
        if (okuyucu == null)
        {
            // Vizede resim / .doc: belge Inceleniyor'da kalır, admin elle inceler
            document.Hatalar.Add(new ValidationIssue
            {
                AlanAdi = "Belge",
                Tur = IssueTuru.Okunamadi,
                Mesaj = "Bu belge türü finalde otomatik okunacak"
            });
            await _db.SaveChangesAsync();
            return;
        }

        // 3) Oku ve okunan alanları kaydet
        Dictionary<string, string> alanlar;
        try
        {
            alanlar = okuyucu.Oku(document.DosyaYolu);
        }
        catch (Exception ex) when (ex is not NotSupportedException)
        {
            // Bozuk / şifreli dosya: belge Inceleniyor'da kalır
            document.Hatalar.Add(new ValidationIssue
            {
                AlanAdi = "Belge",
                Tur = IssueTuru.Okunamadi,
                Mesaj = "Belge okunamadı: dosya bozuk olabilir"
            });
            await _db.SaveChangesAsync();
            return;
        }

        if (alanlar.Count == 0)
        {
            // Metin içermeyen (taranmış) PDF -> OCR gerekir, final
            document.Hatalar.Add(new ValidationIssue
            {
                AlanAdi = "Belge",
                Tur = IssueTuru.Okunamadi,
                Mesaj = "Belgede Ek-16 alanı bulunamadı (taranmış belge ya da farklı bir şablon olabilir)"
            });
            await _db.SaveChangesAsync();
            return;
        }

        document.OkunanAlanlar = alanlar
            .Select(a => new OkunanAlan { AlanAdi = a.Key, Deger = a.Value })
            .ToList();

        // 4) Öğrencinin girdiği bilgilerle karşılaştır
        var hatalar = _karsilastirma.FormIleKarsilastir(document.Student!, document.StajBilgisi!, document.OkunanAlanlar);
        document.Hatalar = hatalar;

        // 5) Uyuşmazlık ya da belgede boş alan varsa Hatalı/Eksik, yoksa Onaylandı
        if (hatalar.Any(h => h.Tur is IssueTuru.Uyusmazlik or IssueTuru.BosAlan))
        {
            document.HataliOlarakIsaretle();
        }
        else
        {
            document.Onayla();
        }

        // Final: burada IFieldValidator ve IBelgeKurali listeleri de çalıştırılacak (Bölüm 6)

        // 6) Kaydet
        await _db.SaveChangesAsync();
    }
}
