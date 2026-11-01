namespace StajEvrakSistemi.Models;

public class LoginLog
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = string.Empty;
    public DateTime Tarih { get; set; } = DateTime.Now;
    public bool BasariliMi { get; set; }
}