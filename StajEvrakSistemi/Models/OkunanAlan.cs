namespace StajEvrakSistemi.Models;

public class OkunanAlan
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public Document? Document { get; set; }
    public string AlanAdi { get; set; } = string.Empty;
    public string Deger { get; set; } = string.Empty;
}