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
