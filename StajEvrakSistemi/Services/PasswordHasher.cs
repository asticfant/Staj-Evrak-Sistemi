namespace StajEvrakSistemi.Services;

/// <summary>
/// Şifre hash'leme sözleşmesi (soyutlama).
/// AuthService ve DbSeeder BCrypt'i doğrudan değil bu arayüz üzerinden kullanır.
/// </summary>
public interface IPasswordHasher
{
    string Hashle(string sifre);
    bool Dogrula(string sifre, string hash);
}

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hashle(string sifre) => BCrypt.Net.BCrypt.HashPassword(sifre);

    public bool Dogrula(string sifre, string hash) => BCrypt.Net.BCrypt.Verify(sifre, hash);
}
