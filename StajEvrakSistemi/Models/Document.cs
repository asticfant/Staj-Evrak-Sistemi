namespace StajEvrakSistemi.Models;

public class Document
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public string DosyaYolu { get; set; } = string.Empty;
    public DosyaTuru DosyaTuru { get; set; }
    public DocumentDurum Durum { get; private set; } = DocumentDurum.Yuklendi;
    public DateTime YuklenmeTarihi { get; set; } = DateTime.Now;

    public List<OkunanAlan> OkunanAlanlar { get; set; } = new();
    public List<ValidationIssue> Hatalar { get; set; } = new();

    public void IncelemeyeAl() => DurumDegistir(DocumentDurum.Inceleniyor);
    public void HataliOlarakIsaretle() => DurumDegistir(DocumentDurum.HataliEksik);
    public void Onayla() => DurumDegistir(DocumentDurum.Onaylandi);
    public void Tamamla() => DurumDegistir(DocumentDurum.Tamamlandi);

    public bool DegistirilebilirMi(UserRole rol)
    {
        return Durum != DocumentDurum.Tamamlandi || rol == UserRole.Admin;
    }

    private void DurumDegistir(DocumentDurum yeni)
    {
        bool izinliMi = (Durum, yeni) switch
        {
            (DocumentDurum.Yuklendi, DocumentDurum.Inceleniyor) => true,
            (DocumentDurum.Inceleniyor, DocumentDurum.HataliEksik) => true,
            (DocumentDurum.Inceleniyor, DocumentDurum.Onaylandi) => true,
            (DocumentDurum.Onaylandi, DocumentDurum.Tamamlandi) => true,
            _ => false
        };

        if (!izinliMi)
            throw new InvalidOperationException($"{Durum} durumundan {yeni} durumuna geçilemez.");

        Durum = yeni;
    }
}