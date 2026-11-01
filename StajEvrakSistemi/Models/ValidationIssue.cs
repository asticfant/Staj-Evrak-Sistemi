namespace StajEvrakSistemi.Models;

public class ValidationIssue
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public Document? Document { get; set; }
    public string AlanAdi { get; set; } = string.Empty;
    public string Mesaj { get; set; } = string.Empty;
    public IssueTuru Tur { get; set; }
}