namespace StajEvrakSistemi.Models;

/// <summary>
/// Öğrencinin yüklediği Ek-16 belgesi.
/// Durum dışarıdan atanamaz; yalnızca aşağıdaki metotlarla değişir (kapsülleme).
/// </summary>
public class Document
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public string DosyaYolu { get; set; } = string.Empty;
    public DosyaTuru DosyaTuru { get; set; }
    public DocumentDurum Durum { get; private set; } = DocumentDurum.Yuklendi;
    public DateTime YuklenmeTarihi { get; set; } = DateTime.UtcNow;

    public StajBilgisi? StajBilgisi { get; set; }
    public List<OkunanAlan> OkunanAlanlar { get; set; } = new();
    public List<ValidationIssue> Hatalar { get; set; } = new();

    // Bölüm 5: hangi durumdan hangisine geçilebilir?
    private static readonly Dictionary<DocumentDurum, DocumentDurum[]> IzinliGecisler = new()
    {
        [DocumentDurum.Yuklendi] = new[] { DocumentDurum.Inceleniyor },
        [DocumentDurum.Inceleniyor] = new[] { DocumentDurum.HataliEksik, DocumentDurum.Onaylandi },
        [DocumentDurum.HataliEksik] = new[] { DocumentDurum.Inceleniyor },
        [DocumentDurum.Onaylandi] = new[] { DocumentDurum.Tamamlandi },
        [DocumentDurum.Tamamlandi] = Array.Empty<DocumentDurum>()
    };

    /// <summary>Sistem: yüklenen belgeyi kontrole alır.</summary>
    public void IncelemeyeAl()
    {
        if (Durum != DocumentDurum.Yuklendi)
        {
            throw new InvalidOperationException("Sadece yeni yüklenen belge incelemeye alınabilir.");
        }
        DurumDegistir(DocumentDurum.Inceleniyor);
    }

    /// <summary>Sistem: uyuşmazlık bulundu.</summary>
    public void HataliOlarakIsaretle() => DurumDegistir(DocumentDurum.HataliEksik);

    /// <summary>Sistem: uyuşmazlık yok, belge otomatik onaylanır.</summary>
    public void Onayla() => DurumDegistir(DocumentDurum.Onaylandi);

    /// <summary>Admin: onaylanmış belgeyi tamamlar.</summary>
    public void Tamamla() => DurumDegistir(DocumentDurum.Tamamlandi);

    /// <summary>Öğrenci formu düzelttikten sonra belge tekrar kontrole girer.</summary>
    public void YenidenKontrolEt()
    {
        if (Durum != DocumentDurum.HataliEksik)
        {
            throw new InvalidOperationException("Sadece Hatalı/Eksik belge yeniden kontrol edilebilir.");
        }
        DurumDegistir(DocumentDurum.Inceleniyor);
    }

    /// <summary>Tamamlandı belgeyi yalnızca Admin değiştirebilir / silebilir.</summary>
    public bool DegistirilebilirMi(UserRole rol)
    {
        return Durum != DocumentDurum.Tamamlandi || rol == UserRole.Admin;
    }

    /// <summary>
    /// Admin'in elle durum düzeltmesi. Geçiş kurallarına bağlı değildir
    /// (hocanın cevabı: admin tamamlanmış belgeyi de güncelleyebilir).
    /// </summary>
    public void DurumAta(DocumentDurum yeni, UserRole yapanRol)
    {
        if (yapanRol != UserRole.Admin)
        {
            throw new UnauthorizedAccessException("Durumu elle sadece Admin değiştirebilir.");
        }
        Durum = yeni;
    }

    private void DurumDegistir(DocumentDurum yeni)
    {
        if (!IzinliGecisler[Durum].Contains(yeni))
        {
            throw new InvalidOperationException($"{Durum} durumundan {yeni} durumuna geçilemez.");
        }
        Durum = yeni;
    }
}
