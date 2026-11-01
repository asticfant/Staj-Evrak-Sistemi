namespace StajEvrakSistemi.Models;

/// <summary>
/// Admin'in kendine özgü alanı yok. Kalıtımın amacı rolü tip düzeyinde ayırmak;
/// admin işlemleri servis katmanında.
/// </summary>
public class Admin : User
{
    // EF Core için
    private Admin() { }

    public Admin(string kullaniciAdi, string email, string sifreHash)
        : base(kullaniciAdi, email, sifreHash, UserRole.Admin)
    {
    }
}
