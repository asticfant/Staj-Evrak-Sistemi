namespace StajEvrakSistemi.Models;

public class Student : User
{
    public Student()
    {
        Rol = UserRole.Student;
    }

    public string AdSoyad { get; set; } = string.Empty;
    public string TcKimlikNo { get; set; } = string.Empty;
    public string Uyruk { get; set; } = string.Empty;
    public string OgrenciNo { get; set; } = string.Empty;
    public DateTime DogumTarihi { get; set; }
    public string Adres { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Ek16TeslimYili { get; set; } = string.Empty;
    public int OncekiStajGunSayisi { get; set; }
    public string Telefon { get; set; } = string.Empty;
    public string SaglikYardimiStatusu { get; set; } = string.Empty;
    public string? GoogleFormUrl { get; set; }

    public List<Document> Belgeler { get; set; } = new();

    public bool YabanciMi()
    {
        return !string.Equals(Uyruk.Trim(), "T.C.", StringComparison.OrdinalIgnoreCase);
    }
}