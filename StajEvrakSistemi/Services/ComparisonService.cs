using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Readers;

namespace StajEvrakSistemi.Services;

/// <summary>
/// İki karşılaştırma:
///  - FormIleKarsilastir   : belgeden okunan  &lt;-&gt; öğrencinin girdiği (öğrenci tarafı, "teslime hazır mı?")
///  - VeriSetiIleKarsilastir: belgeden okunan &lt;-&gt; Excel veri seti (admin tarafı)
/// Alan adları Readers/Ek16Alanlari sınıfındaki sabitlerdir.
/// Belgede okunmayan alanlar (örn. test PDF'lerinde staj tarihleri, işyeri adı) karşılaştırılmaz.
/// </summary>
public class ComparisonService
{
    private static readonly CultureInfo Tr = new("tr-TR");

    private static readonly string[] TarihBicimleri =
    {
        "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd-MM-yyyy"
    };

    private readonly AppDbContext _db;

    public ComparisonService(AppDbContext db)
    {
        _db = db;
    }

    public List<ValidationIssue> FormIleKarsilastir(Student student, StajBilgisi staj, List<OkunanAlan> okunanAlanlar)
    {
        return Karsilastir(okunanAlanlar, alan => GirilenDeger(student, staj, alan), "girdiğiniz bilgi");
    }

    public List<ValidationIssue> VeriSetiIleKarsilastir(List<OkunanAlan> okunanAlanlar)
    {
        // 1) Belgeden okunan öğrenci numarası
        var okunanNo = okunanAlanlar.FirstOrDefault(a => a.AlanAdi == Ek16Alanlari.OgrenciNo)?.Deger;
        var no = SadeceRakam(okunanNo);

        if (no.Length == 0)
        {
            return new List<ValidationIssue>
            {
                new()
                {
                    AlanAdi = Ek16Alanlari.OgrenciNo,
                    Tur = IssueTuru.Okunamadi,
                    Mesaj = "Belgeden öğrenci numarası okunamadı, veri setiyle karşılaştırılamadı"
                }
            };
        }

        // 2) Veri setinde ara
        var kayit = _db.VeriSetiKayitlari.AsNoTracking().FirstOrDefault(k => k.OgrenciNo == no);
        if (kayit == null)
        {
            return new List<ValidationIssue>
            {
                new()
                {
                    AlanAdi = Ek16Alanlari.OgrenciNo,
                    OkunanDeger = okunanNo,
                    Tur = IssueTuru.Uyari,
                    Mesaj = "Veri setinde kayıt yok"
                }
            };
        }

        // 3) Alan alan karşılaştır
        return Karsilastir(okunanAlanlar, alan => VeriSetiDegeri(kayit, alan), "veri seti");
    }

    /// <summary>
    /// Ortak karşılaştırma döngüsü. beklenenGetir null dönerse o alan karşılaştırılmaz
    /// (örn. onay alanları ya da tarafta karşılığı olmayan alanlar).
    /// </summary>
    private List<ValidationIssue> Karsilastir(List<OkunanAlan> okunanAlanlar, Func<string, string?> beklenenGetir, string kaynakAdi)
    {
        var hatalar = new List<ValidationIssue>();

        foreach (var alan in okunanAlanlar)
        {
            var beklenen = beklenenGetir(alan.AlanAdi);
            if (beklenen == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(alan.Deger))
            {
                hatalar.Add(new ValidationIssue
                {
                    AlanAdi = alan.AlanAdi,
                    GirilenDeger = beklenen,
                    OkunanDeger = alan.Deger,
                    Tur = IssueTuru.BosAlan,
                    Mesaj = "Belgede bu alan boş"
                });
                continue;
            }

            if (Normalize(alan.AlanAdi, beklenen) != Normalize(alan.AlanAdi, alan.Deger))
            {
                hatalar.Add(new ValidationIssue
                {
                    AlanAdi = alan.AlanAdi,
                    GirilenDeger = beklenen,
                    OkunanDeger = alan.Deger,
                    Tur = IssueTuru.Uyusmazlik,
                    Mesaj = $"Belgedeki değer {kaynakAdi} ile uyuşmuyor"
                });
            }
        }

        return hatalar;
    }

    private string? GirilenDeger(Student student, StajBilgisi staj, string alanAdi)
    {
        return alanAdi switch
        {
            Ek16Alanlari.AdSoyad => student.AdSoyad,
            Ek16Alanlari.TcKimlikNo => student.TcKimlikNo,
            Ek16Alanlari.Uyruk => student.Uyruk,
            Ek16Alanlari.OgrenciNo => student.OgrenciNo,
            Ek16Alanlari.DogumTarihi => student.DogumTarihi?.ToString("dd.MM.yyyy"),
            Ek16Alanlari.Adres => student.Adres,
            Ek16Alanlari.Bolum => student.Bolum,
            Ek16Alanlari.Telefon => student.Telefon,
            Ek16Alanlari.Email => student.Email,
            Ek16Alanlari.SaglikYardimiStatusu => student.SaglikYardimiStatusu,
            Ek16Alanlari.OncekiStajGunSayisi => student.OncekiStajGunSayisi.ToString(),

            Ek16Alanlari.Ek16TeslimYili => staj.Ek16TeslimYili,
            Ek16Alanlari.StajBaslangic => staj.StajBaslangic.ToString("dd.MM.yyyy"),
            Ek16Alanlari.StajBitis => staj.StajBitis.ToString("dd.MM.yyyy"),
            Ek16Alanlari.IsyeriAdi => staj.IsyeriAdi,
            Ek16Alanlari.IsyeriAdresi => staj.IsyeriAdresi,
            Ek16Alanlari.IsyeriTelefon => staj.IsyeriTelefon,
            Ek16Alanlari.IsyeriEmail => staj.IsyeriEmail,
            Ek16Alanlari.MuhendisSayisi => staj.MuhendisSayisi.ToString(),
            Ek16Alanlari.MuhendisBolumu => staj.MuhendisBolumu,

            _ => null // onay alanları vb. karşılaştırılmaz
        };
    }

    private static string? VeriSetiDegeri(VeriSetiKaydi k, string alanAdi)
    {
        return alanAdi switch
        {
            Ek16Alanlari.AdSoyad => k.AdSoyad,
            Ek16Alanlari.TcKimlikNo => k.TcKimlikNo,
            Ek16Alanlari.Uyruk => k.Uyruk,
            Ek16Alanlari.DogumTarihi => k.DogumTarihi.ToString("dd.MM.yyyy"),
            Ek16Alanlari.Adres => k.Adres,
            Ek16Alanlari.Bolum => k.Bolum,
            Ek16Alanlari.Ek16TeslimYili => k.Ek16TeslimYili,
            Ek16Alanlari.OncekiStajGunSayisi => k.OncekiStajGunSayisi.ToString(),
            Ek16Alanlari.Email => k.Email,
            Ek16Alanlari.Telefon => k.Telefon,
            Ek16Alanlari.SaglikYardimiStatusu => k.SaglikYardimiStatusu,
            _ => null // OgrenciNo ile zaten eşleştirildi; staj alanları veri setinde yok
        };
    }

    /// <summary>
    /// İki değeri karşılaştırılabilir hale getirir.
    /// PDF'ler Türkçe karakter kullanmıyor ("Yilmaz"), profilde "Yılmaz" yazılıyor;
    /// bu yüzden büyük harfe çevirdikten sonra Türkçe harfler ASCII karşılığına indirilir.
    /// </summary>
    private string Normalize(string alanAdi, string? deger)
    {
        if (string.IsNullOrWhiteSpace(deger))
        {
            return string.Empty;
        }

        switch (alanAdi)
        {
            case Ek16Alanlari.TcKimlikNo:
            case Ek16Alanlari.OgrenciNo:
                return deger.Trim();   // içinde harf/sembol varsa ("1037343[3243") fark olarak görünsün

            case Ek16Alanlari.Telefon:
            case Ek16Alanlari.IsyeriTelefon:
                // +90 532 ..., 0532 ..., 532 ... hepsi son 10 haneye indirgenir
                var rakam = SadeceRakam(deger);
                return rakam.Length >= 10 ? rakam[^10..] : rakam;

            case Ek16Alanlari.DogumTarihi:
            case Ek16Alanlari.StajBaslangic:
            case Ek16Alanlari.StajBitis:
                return DateTime.TryParseExact(deger.Trim(), TarihBicimleri, Tr, DateTimeStyles.None, out var t)
                    ? t.ToString("yyyy-MM-dd")
                    : deger.Trim();

            case Ek16Alanlari.OncekiStajGunSayisi:
            case Ek16Alanlari.MuhendisSayisi:
                return int.TryParse(deger.Trim(), out var sayi) ? sayi.ToString() : deger.Trim();

            case Ek16Alanlari.Email:
            case Ek16Alanlari.IsyeriEmail:
                return deger.Trim().ToLowerInvariant();

            case Ek16Alanlari.Uyruk:
                var uyruk = Metin(deger).Replace(".", "");
                return uyruk is "TC" or "TURKIYE" or "TURK" ? "TC" : uyruk;

            case Ek16Alanlari.Adres:
            case Ek16Alanlari.IsyeriAdresi:
                // "Mah.," / "Mah," ve "No:5" / "No: 5" gibi yazım farkları önemsiz
                return Regex.Replace(Metin(deger), @"[^\p{L}\p{N}]+", " ").Trim();

            default:
                return Metin(deger);
        }
    }

    /// <summary>Trim + fazla boşlukları teke indir + tr-TR büyük harf + Türkçe harfleri sadeleştir.</summary>
    private static string Metin(string deger)
    {
        var s = Regex.Replace(deger.Trim(), @"\s+", " ").ToUpper(Tr);
        return s.Replace('İ', 'I').Replace('Ş', 'S').Replace('Ğ', 'G')
                .Replace('Ü', 'U').Replace('Ö', 'O').Replace('Ç', 'C');
    }

    private static string SadeceRakam(string? deger)
    {
        return deger == null ? string.Empty : new string(deger.Where(char.IsDigit).ToArray());
    }
}
