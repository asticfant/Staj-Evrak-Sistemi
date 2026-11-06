namespace StajEvrakSistemi.Readers;

/// <summary>
/// Okuyucular ve ComparisonService arasındaki ortak alan adları.
/// Okuyucu sözlüğe bu anahtarlarla yazar, karşılaştırma bu anahtarlarla okur.
/// NOT: Liste Ek-16 şablonuna göre kesinleştirilmeli.
/// </summary>
public static class Ek16Alanlari
{
    // Öğrenci (profil) bilgileri
    public const string AdSoyad = "AdSoyad";
    public const string TcKimlikNo = "TcKimlikNo";
    public const string OgrenciNo = "OgrenciNo";
    public const string DogumTarihi = "DogumTarihi";
    public const string Adres = "Adres";
    public const string Bolum = "Bolum";
    public const string Telefon = "Telefon";
    public const string Email = "Email";
    public const string Uyruk = "Uyruk";
    public const string OncekiStajGunSayisi = "OncekiStajGunSayisi";
    public const string SaglikYardimiStatusu = "SaglikYardimiStatusu";

    // Staj bilgileri
    public const string Ek16TeslimYili = "Ek16TeslimYili";
    public const string StajBaslangic = "StajBaslangic";
    public const string StajBitis = "StajBitis";
    public const string IsyeriAdi = "IsyeriAdi";
    public const string IsyeriAdresi = "IsyeriAdresi";
    public const string IsyeriTelefon = "IsyeriTelefon";
    public const string IsyeriEmail = "IsyeriEmail";
    public const string MuhendisSayisi = "MuhendisSayisi";
    public const string MuhendisBolumu = "MuhendisBolumu";

    // Onay / imza alanları (okunur ve raporda gösterilir; karşılaştırılmaz, kural kontrolü final)
    public const string OgrenciOnayi = "OgrenciOnayi";
    public const string DanismanOnayi = "DanismanOnayi";
    public const string IsverenOnayi = "IsverenOnayi";
}
