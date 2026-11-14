namespace StajEvrakSistemi.Models;

/// <summary>
/// Admin'in kendine özgü alanı yok. Kalıtımın amacı rolü tip düzeyinde ayırmak;
/// admin işlemleri servis katmanında. AnaSayfa() User'daki abstract metodu override eder.
/// </summary>
public class Admin : User
{
    // EF Core için
    private Admin() { }

    public Admin(string kullaniciAdi, string email, string sifreHash)
        : base(kullaniciAdi, email, sifreHash, UserRole.Admin)
    {
    }

    /// <summary>Admin girişten sonra belge listesine gider.</summary>
    public override string AnaSayfa() => "/Admin/Belgeler";
}
