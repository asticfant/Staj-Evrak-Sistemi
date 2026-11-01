using StajEvrakSistemi.Models;

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

/// <summary>Bilgisayarda doldurulmuş .docx Ek-16'yı okur (DocumentFormat.OpenXml).</summary>
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
/// Taranmış (resim) PDF'ler OCR gerektirir -> final.
/// </summary>
public class PdfReader : IDocumentReader
{
    public Dictionary<string, string> Oku(string dosyaYolu)
    {
        // TODO (Muhammed): NuGet'ten PdfPig ekle, sayfa metinlerinden alanları ayıkla.
        throw new NotImplementedException();
    }
}

/// <summary>El yazısı / taranmış belge okuyucu. (Final – planlanan)</summary>
public class OcrReader : IDocumentReader
{
    public Dictionary<string, string> Oku(string dosyaYolu)
    {
        throw new NotSupportedException("OCR okuma final kapsamında.");
    }
}

/// <summary>Dosya türüne göre doğru okuyucuyu seçer. Okunamayan türlerde null döner.</summary>
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
            // .doc (eski Word) OpenXml ile okunamaz; resimler OCR ister -> final
            _ => null
        };
    }
}
