namespace StajEvrakSistemi.Models;

/// <summary>
/// Her giriş denemesinin kaydı. Kullanıcıya FK ile değil KullaniciAdi ile bağlı,
/// böylece var olmayan kullanıcı adıyla yapılan denemeler de loglanır.
/// </summary>
public class LoginLog
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public DateTime Tarih { get; set; } = DateTime.UtcNow;
    public bool BasariliMi { get; set; }
}
