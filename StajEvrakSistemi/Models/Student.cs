namespace StajEvrakSistemi.Models;

/// <summary>
/// Öğrenci kendisi kayıt olur; kişisel bilgilerini kayıttan sonra profil formunda girer.
/// </summary>
public class Student : User
{
    public string? AdSoyad { get; set; }
    public string? TcKimlikNo { get; set; }
    public string? Uyruk { get; set; }
    public string? OgrenciNo { get; set; }
    public DateTime? DogumTarihi { get; set; }
    public string? Adres { get; set; }
    public string? Bolum { get; set; }
    public string? Telefon { get; set; }
    public string? SaglikYardimiStatusu { get; set; }
    public int OncekiStajGunSayisi { get; set; }
    public string? GoogleFormUrl { get; set; }

    public List<Document> Belgeler { get; set; } = new();

    // EF Core için
    private Student() { }

    public Student(string kullaniciAdi, string email, string sifreHash)
        : base(kullaniciAdi, email, sifreHash, UserRole.Student)
    {
    }

    public bool YabanciMi()
    {
        if (string.IsNullOrWhiteSpace(Uyruk))
        {
            return false;
        }

        var uyruk = Uyruk.Trim().ToUpper(new System.Globalization.CultureInfo("tr-TR"));
        return uyruk != "T.C." && uyruk != "TC" && uyruk != "TÜRKİYE";
    }

    public bool ProfilTamamMi()
    {
        return !string.IsNullOrWhiteSpace(AdSoyad)
            && !string.IsNullOrWhiteSpace(TcKimlikNo)
            && !string.IsNullOrWhiteSpace(Uyruk)
            && !string.IsNullOrWhiteSpace(OgrenciNo)
            && DogumTarihi.HasValue
            && !string.IsNullOrWhiteSpace(Adres)
            && !string.IsNullOrWhiteSpace(Bolum)
            && !string.IsNullOrWhiteSpace(Telefon);
    }
}
