using System.ComponentModel.DataAnnotations;

namespace StajEvrakSistemi.Dtos;

/// <summary>Giriş sayfasındaki form.</summary>
public class GirisFormu
{
    [Required(ErrorMessage = "Kullanıcı adı veya e-posta zorunludur")]
    public string KullaniciAdiVeyaEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [DataType(DataType.Password)]
    public string Sifre { get; set; } = string.Empty;
}

/// <summary>Kayıt ol sayfasındaki form.</summary>
public class KayitFormu
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    public string KullaniciAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [DataType(DataType.Password)]
    public string Sifre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
    [DataType(DataType.Password)]
    public string SifreTekrar { get; set; } = string.Empty;
}
