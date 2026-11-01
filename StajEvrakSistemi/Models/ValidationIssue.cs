namespace StajEvrakSistemi.Models;

/// <summary>Raporda gösterilen bir sorun satırı.</summary>
public class ValidationIssue
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string AlanAdi { get; set; } = string.Empty;
    public string? GirilenDeger { get; set; }
    public string? OkunanDeger { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public IssueTuru Tur { get; set; }
}
