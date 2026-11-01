using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Dtos;

/// <summary>Kayıt, profil güncelleme gibi basit işlemlerin sonucu.</summary>
public class IslemSonucu
{
    public bool Basarili { get; set; }
    public string Mesaj { get; set; } = string.Empty;

    public static IslemSonucu Ok(string mesaj = "") => new() { Basarili = true, Mesaj = mesaj };
    public static IslemSonucu Hata(string mesaj) => new() { Basarili = false, Mesaj = mesaj };
}

/// <summary>"Şifre yanlış" ile "hesap kilitli" farklı mesajlar gösterebilmek için.</summary>
public class GirisSonucu
{
    public bool Basarili { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public User? Kullanici { get; set; }
}

public class YuklemeSonucu
{
    public bool Basarili { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public Document? Belge { get; set; }
}

public class ImportSonucu
{
    public int EklenenSayisi { get; set; }
    public int AtlananSayisi { get; set; }
    public List<string> Hatalar { get; set; } = new();
}

/// <summary>Admin belge listesindeki filtre alanları. Boş bırakılan alan filtrelenmez.</summary>
public class BelgeFiltresi
{
    public DocumentDurum? Durum { get; set; }
    public string? OgrenciNo { get; set; }
    public string? Bolum { get; set; }
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
}

/// <summary>Belge detay / rapor sayfasında gösterilecek her şey.</summary>
public class BelgeDetay
{
    public Document Belge { get; set; } = null!;
    public StajBilgisi? StajBilgisi { get; set; }
    public List<OkunanAlan> OkunanAlanlar { get; set; } = new();
    public List<ValidationIssue> Hatalar { get; set; } = new();
    public List<ValidationIssue> VeriSetiFarklari { get; set; } = new();
}

/// <summary>Öğrencinin profil formunda girdiği kişisel bilgiler.</summary>
public class ProfilFormu
{
    public string AdSoyad { get; set; } = string.Empty;
    public string TcKimlikNo { get; set; } = string.Empty;
    public string Uyruk { get; set; } = string.Empty;
    public string OgrenciNo { get; set; } = string.Empty;
    public DateTime DogumTarihi { get; set; }
    public string Adres { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string SaglikYardimiStatusu { get; set; } = string.Empty;
    public int OncekiStajGunSayisi { get; set; }
}
