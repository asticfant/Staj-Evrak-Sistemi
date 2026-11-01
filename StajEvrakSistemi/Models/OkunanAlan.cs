namespace StajEvrakSistemi.Models;

/// <summary>Belgeden okunan bir alan (vizede Word/PDF, finalde OCR).</summary>
public class OkunanAlan
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string AlanAdi { get; set; } = string.Empty;
    public string? Deger { get; set; }
}
