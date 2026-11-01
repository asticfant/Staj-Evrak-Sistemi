using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

// =====================================================================
//  FİNAL KAPSAMI (<<planlanan>>) – vizede DI'a kaydedilmez, kullanılmaz.
// =====================================================================

/// <summary>Gereksinim 4.7 – tamamlanmış belgeden Ek-17 üretir. (Final)</summary>
public class Ek17Generator
{
    public string Olustur(Document document)
    {
        throw new NotSupportedException("Ek-17 üretimi final kapsamında.");
    }
}

/// <summary>Gereksinim 4.6 – Tamamlandı olan belgenin bilgilerini Google Form'a gönderir. (Final)</summary>
public interface IGoogleFormService
{
    Task Gonder(string url, List<OkunanAlan> alanlar);
}
