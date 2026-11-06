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
/// Metin tabanlı PDF'i okur. Vizede PdfPig paketi eklenerek yazılacak.
/// </summary>
public class PdfReader : IDocumentReader
{
    private readonly Dictionary<string, string> _etiketHaritasi = new Dictionary<string, string>
    {
        { "Ogrenci Ad Soyad", "AdSoyad" },
        { "Kimlik No", "TcKimlikNo" },
        { "Telefon No", "Telefon" }
    };

    public Dictionary<string, string> Oku(string dosyaYolu)
    {
        var sonuc = new Dictionary<string, string>();

        using (var pdf = PdfDocument.Open(dosyaYolu))
        {
            foreach (var sayfa in pdf.GetPages())
            {
                var metin = sayfa.Text;
                // Şimdilik hata vermemesi için boş okuma yapıyoruz, 
                // metin ayıklama mantığı proje ilerledikçe buraya eklenecek.
            }
        }
        return sonuc;
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
        return tur switch
        {
            DosyaTuru.Docx => _docx,
            DosyaTuru.Pdf => _pdf,
            _ => null
        };
    }
}