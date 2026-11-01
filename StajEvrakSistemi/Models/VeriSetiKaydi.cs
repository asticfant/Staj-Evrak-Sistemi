namespace StajEvrakSistemi.Models;

/// <summary>
/// Admin'in yüklediği Excel test veri setinin bir satırı.
/// Kullanıcı hesabı DEĞİLDİR; sadece admin tarafı karşılaştırma içindir.
/// Belgeyle OgrenciNo üzerinden eşleşir.
/// </summary>
public class VeriSetiKaydi
{
    public int Id { get; set; }
    public string OgrenciNo { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public string TcKimlikNo { get; set; } = string.Empty;
    public string Uyruk { get; set; } = string.Empty;
    public DateTime DogumTarihi { get; set; }
    public string Adres { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Ek16TeslimYili { get; set; } = string.Empty;
    public int OncekiStajGunSayisi { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string SaglikYardimiStatusu { get; set; } = string.Empty;
}
