namespace StajEvrakSistemi.Models;

public abstract class User
{
    public const int MaxHataliGiris = 5;
    public const int KilitSuresiDakika = 15;

    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SifreHash { get; set; } = string.Empty;
    public UserRole Rol { get; protected set; }
    public int HataliGirisSayisi { get; private set; }
    public DateTime? KilitBitisZamani { get; private set; }

    public bool KilitliMi()
    {
        return KilitBitisZamani.HasValue && KilitBitisZamani.Value > DateTime.Now;
    }

    public void HataliGirisKaydet()
    {
        HataliGirisSayisi++;
        if (HataliGirisSayisi >= MaxHataliGiris)
        {
            KilitBitisZamani = DateTime.Now.AddMinutes(KilitSuresiDakika);
            HataliGirisSayisi = 0;
        }
    }

    public void SayaciSifirla()
    {
        HataliGirisSayisi = 0;
        KilitBitisZamani = null;
    }
}