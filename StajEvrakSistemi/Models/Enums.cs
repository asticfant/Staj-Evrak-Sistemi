namespace StajEvrakSistemi.Models;

public enum UserRole
{
    Student,
    Admin
}

public enum DocumentDurum
{
    Yuklendi,
    Inceleniyor,
    HataliEksik,
    Onaylandi,
    Tamamlandi
}

/// <summary>Durumların kullanıcıya gösterilen adları (hata mesajlarında enum adı görünmesin diye).</summary>
public static class DocumentDurumAdlari
{
    public static string GorunenAd(this DocumentDurum durum) => durum switch
    {
        DocumentDurum.Yuklendi => "Yüklendi",
        DocumentDurum.Inceleniyor => "İnceleniyor",
        DocumentDurum.HataliEksik => "Hatalı / Eksik",
        DocumentDurum.Onaylandi => "Onaylandı",
        DocumentDurum.Tamamlandi => "Tamamlandı",
        _ => durum.ToString()
    };
}

public enum DosyaTuru
{
    Doc,
    Docx,
    Pdf,
    Jpeg,
    Png
}

public enum IssueTuru
{
    Uyusmazlik,
    Okunamadi,
    BosAlan,
    FormatHatasi,
    Uyari
}
