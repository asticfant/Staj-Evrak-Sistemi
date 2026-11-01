namespace StajEvrakSistemi.Models;

/// <summary>
/// Öğrenci ve Admin'in ortak üst sınıfı. Kendisinden nesne üretilemez (abstract).
/// Veritabanında Student ve Admin ile birlikte tek tabloda (TPH) tutulur.
/// </summary>
public abstract class User
{
    public const int MaxHataliGiris = 5;
    public const int KilitSuresiDakika = 15;

    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Kapsülleme: aşağıdaki alanlar dışarıdan doğrudan değiştirilemez.
    public string SifreHash { get; private set; } = string.Empty;
    public UserRole Rol { get; private set; }
    public int HataliGirisSayisi { get; private set; }
    public DateTime? KilitBitisZamani { get; private set; }

    // EF Core için
    protected User() { }

    protected User(string kullaniciAdi, string email, string sifreHash, UserRole rol)
    {
        KullaniciAdi = kullaniciAdi;
        Email = email;
        SifreHash = sifreHash;
        Rol = rol;
    }

    public bool KilitliMi()
    {
        return KilitBitisZamani.HasValue && KilitBitisZamani.Value > DateTime.UtcNow;
    }

    public void HataliGirisKaydet()
    {
        // Önceki kilidin süresi dolmuşsa sayaç sıfırdan başlar.
        // Yoksa kilit açıldıktan sonraki ilk yanlış denemede hesap yine kilitlenirdi.
        if (KilitBitisZamani.HasValue && KilitBitisZamani.Value <= DateTime.UtcNow)
        {
            SayaciSifirla();
        }

        HataliGirisSayisi++;

        if (HataliGirisSayisi >= MaxHataliGiris)
        {
            KilitBitisZamani = DateTime.UtcNow.AddMinutes(KilitSuresiDakika);
        }
    }

    public void SayaciSifirla()
    {
        HataliGirisSayisi = 0;
        KilitBitisZamani = null;
    }
}
