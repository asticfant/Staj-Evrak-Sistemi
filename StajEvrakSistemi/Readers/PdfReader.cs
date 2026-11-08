using System;
using System.Collections.Generic;
using StajEvrakSistemi.Models;
using UglyToad.PdfPig;

namespace StajEvrakSistemi.Readers;

/// <summary>
/// Belge okuma sözleşmesi (soyutlama + çok biçimlilik).
/// Her okuyucu belgeyi "alan adı -> değer" sözlüğüne çevirir.
/// Finalde OCR eklenince sadece yeni bir okuyucu sınıfı yazılır, başka kod değişmez.
/// </summary>
public interface IDocumentReader
{
    Dictionary<string, string> Oku(string dosyaYolu);
}

/// <summary>
/// Bilgisayarda doldurulmuş .docx Ek-16'yı okur (DocumentFormat.OpenXml).
/// </summary>
public class DocxReader : IDocumentReader
{
    public Dictionary<string, string> Oku(string dosyaYolu)
    {
        // TODO (Muhammed):
        // using var doc = WordprocessingDocument.Open(dosyaYolu, false);
        // Ek-16 tablolarındaki etiket hücresinin yanındaki değeri oku,
        // anahtar olarak Ek16Alanlari sabitlerini kullan.
        throw new NotImplementedException();
    }
}

/// <summary>
/// Metin tabanlı PDF'i okur (PdfPig). Test PDF'lerinde her satır "Etiket: değer" biçimindedir.
/// Taranmış (resim) PDF'ler OCR gerektirir -> final.
/// </summary>
public class PdfReader : IDocumentReader
{
    // PDF'teki etiket -> Ek16Alanlari anahtarı.
    // Etiketler karşılaştırılmadan önce EtiketiSadelestir'den geçer (Türkçe karakter / büyük-küçük harf farkı önemsiz).
    private static readonly Dictionary<string, string> EtiketHaritasi = new()
    {
        ["OGRENCI AD SOYAD"] = Ek16Alanlari.AdSoyad,
        ["AD SOYAD"] = Ek16Alanlari.AdSoyad,
        ["UYRUK"] = Ek16Alanlari.Uyruk,
        ["KIMLIK NO"] = Ek16Alanlari.TcKimlikNo,
        ["TC KIMLIK NO"] = Ek16Alanlari.TcKimlikNo,
        ["OGRENCI NO"] = Ek16Alanlari.OgrenciNo,
        ["DOGUM TARIHI"] = Ek16Alanlari.DogumTarihi,
        ["IKAMETGAH ADRESI"] = Ek16Alanlari.Adres,
        ["OKUDUGU BOLUM"] = Ek16Alanlari.Bolum,
        ["TESLIM YILI"] = Ek16Alanlari.Ek16TeslimYili,
        ["DAHA ONCE YAPILAN STAJ GUN SAYISI"] = Ek16Alanlari.OncekiStajGunSayisi,
        ["E POSTA"] = Ek16Alanlari.Email,
        ["TELEFON NO"] = Ek16Alanlari.Telefon,
        ["SAGLIK YARDIMI STATUSU"] = Ek16Alanlari.SaglikYardimiStatusu,
        ["OGRENCI ONAYI"] = Ek16Alanlari.OgrenciOnayi,
        ["STAJ DANISMANI ONAYI"] = Ek16Alanlari.DanismanOnayi,
        ["ISVEREN ONAYI"] = Ek16Alanlari.IsverenOnayi,
        ["KURUMDAKI MUHENDIS SAYISI"] = Ek16Alanlari.MuhendisSayisi,
        ["MUHENDIS MEZUNIYET BOLUMU"] = Ek16Alanlari.MuhendisBolumu,
        ["STAJ BASLANGIC TARIHI"] = Ek16Alanlari.StajBaslangic,
        ["STAJ BITIS TARIHI"] = Ek16Alanlari.StajBitis,
        ["ISYERI ADI"] = Ek16Alanlari.IsyeriAdi,
        ["ISYERI ADRESI"] = Ek16Alanlari.IsyeriAdresi,
        ["ISYERI TELEFON"] = Ek16Alanlari.IsyeriTelefon,
        ["ISYERI E POSTA"] = Ek16Alanlari.IsyeriEmail,
    };

    public Dictionary<string, string> Oku(string dosyaYolu)
    {
        var sonuc = new Dictionary<string, string>();

        using var pdf = UglyToad.PdfPig.PdfDocument.Open(dosyaYolu);
        foreach (var sayfa in pdf.GetPages())
        {
            foreach (var satir in Satirlar(sayfa))
            {
                var ikiNokta = satir.IndexOf(':');
                if (ikiNokta <= 0)
                {
                    continue;
                }

                var etiket = EtiketiSadelestir(satir[..ikiNokta]);
                var deger = satir[(ikiNokta + 1)..].Trim();

                // Aynı alan iki kez geçerse ilk dolu değer kalır
                if (EtiketHaritasi.TryGetValue(etiket, out var anahtar)
                    && (!sonuc.TryGetValue(anahtar, out var eski) || string.IsNullOrWhiteSpace(eski)))
                {
                    sonuc[anahtar] = deger;
                }
            }
        }

        return sonuc;
    }

    /// <summary>Kelimeleri dikey konuma göre satırlara, satır içinde soldan sağa dizer.</summary>
    private static IEnumerable<string> Satirlar(UglyToad.PdfPig.Content.Page sayfa)
    {
        var kelimeler = sayfa.GetWords()
            .OrderByDescending(k => k.BoundingBox.Bottom)
            .ThenBy(k => k.BoundingBox.Left)
            .ToList();

        var satir = new List<UglyToad.PdfPig.Content.Word>();
        double? satirY = null;

        foreach (var kelime in kelimeler)
        {
            // Yüksekliğin yarısından az fark varsa aynı satır sayılır
            if (satirY.HasValue && Math.Abs(kelime.BoundingBox.Bottom - satirY.Value) > kelime.BoundingBox.Height / 2)
            {
                yield return string.Join(" ", satir.OrderBy(k => k.BoundingBox.Left).Select(k => k.Text));
                satir.Clear();
            }
            if (satir.Count == 0)
            {
                satirY = kelime.BoundingBox.Bottom;
            }
            satir.Add(kelime);
        }

        if (satir.Count > 0)
        {
            yield return string.Join(" ", satir.OrderBy(k => k.BoundingBox.Left).Select(k => k.Text));
        }
    }

    private static string EtiketiSadelestir(string etiket)
    {
        var buyuk = etiket.Trim().ToUpper(new System.Globalization.CultureInfo("tr-TR"))
            .Replace('İ', 'I').Replace('Ş', 'S').Replace('Ğ', 'G')
            .Replace('Ü', 'U').Replace('Ö', 'O').Replace('Ç', 'C')
            .Replace('-', ' ').Replace('.', ' ');
        return System.Text.RegularExpressions.Regex.Replace(buyuk, @"\s+", " ").Trim();
    }
}

/// <summary>
/// El yazısı / taranmış belge okuyucu. (Final – planlanan)
/// </summary>
public class OcrReader : IDocumentReader
{
    public Dictionary<string, string> Oku(string dosyaYolu)
    {
        throw new NotSupportedException("OCR okuma final kapsamında.");
    }
}

/// <summary>
/// Dosya türüne göre doğru okuyucuyu seçer. Okunamayan türlerde null döner.
/// </summary>
public class OkuyucuSecici
{
    private readonly DocxReader _docx;
    private readonly PdfReader _pdf;

    public OkuyucuSecici(DocxReader docx, PdfReader pdf)
    {
        _docx = docx;
        _pdf = pdf;
    }

    public IDocumentReader? Sec(DosyaTuru tur)
    {
        // DocxReader henüz yazılmadığı için .docx şimdilik okunmuyor; null dönünce belge
        // "finalde otomatik okunacak" notuyla İnceleniyor'da kalır ve admin elle inceler.
        // DocxReader bitince aşağıdaki satır açılır:
        //     DosyaTuru.Docx => _docx,
        return tur switch
        {
            DosyaTuru.Pdf => _pdf,
            _ => null
        };
    }
}