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

    /// <summary>Öğrenci girişten sonra kendi belgelerine gider.</summary>
    public override string AnaSayfa() => "/Student/Belgelerim";

    public bool YabanciMi() => YabanciUyrukMu(Uyruk);

    /// <summary>
    /// Uyruk metni T.C. değilse true. "T.C.", "TC", "Türkiye", "Turkiye", "Türk" T.C. sayılır.
    /// Static: profil formu ve Excel aktarımı da (henüz Student nesnesi yokken) kullanır.
    /// </summary>
    public static bool YabanciUyrukMu(string? uyruk)
    {
        if (string.IsNullOrWhiteSpace(uyruk))
        {
            return false;
        }

        var sade = uyruk.Trim().ToUpper(new System.Globalization.CultureInfo("tr-TR"))
            .Replace('İ', 'I').Replace('Ü', 'U')
            .Replace(".", "").Replace(" ", "");
        return sade != "TC" && sade != "TURKIYE" && sade != "TURK";
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
