namespace StajEvrakSistemi.Models;

/// <summary>
/// Öğrencinin her belge yüklemesinde formda girdiği staja özgü bilgiler.
/// Her Document'a tam olarak bir StajBilgisi bağlıdır.
/// NOT: Alan listesi Ek-16 şablonuyla karşılaştırılıp kesinleştirilmeli.
/// </summary>
public class StajBilgisi
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public Document? Document { get; set; }

    public string Ek16TeslimYili { get; set; } = string.Empty;   // örn. "2025-2026"
    public DateTime StajBaslangic { get; set; }
    public DateTime StajBitis { get; set; }
    public List<DayOfWeek> CalisilanGunler { get; set; } = new();
    public string IsyeriAdi { get; set; } = string.Empty;
    public string IsyeriAdresi { get; set; } = string.Empty;
    public string IsyeriTelefon { get; set; } = string.Empty;
    public string IsyeriEmail { get; set; } = string.Empty;
    public int MuhendisSayisi { get; set; }
    public string MuhendisBolumu { get; set; } = string.Empty;
}
