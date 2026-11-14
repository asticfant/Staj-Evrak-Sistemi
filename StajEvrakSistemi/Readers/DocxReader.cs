using DocumentFormat.OpenXml.Packaging;
using W = DocumentFormat.OpenXml.Wordprocessing;   // "Document", "Table" adları Models ile çakışmasın diye takma ad

namespace StajEvrakSistemi.Readers;

/// <summary>
/// Bilgisayarda doldurulmuş .docx Ek-16'yı okur (DocumentFormat.OpenXml).
/// Ayrıştırma ve etiket eşleme üst sınıfta (EtiketliOkuyucu); burada sadece
/// Word belgesinden satırların nasıl çıkarılacağı yazılır.
///
/// İki yazım biçimini de destekler:
///  1) Paragraf:  "Öğrenci No: 230000001"
///  2) Tablo:     | Öğrenci No | 230000001 |   (etiket hücresi + değer hücresi)
///     Aynı satırda iki çift varsa (| Ad Soyad | Ali | Öğrenci No | 23.. |) ikisi de okunur.
/// </summary>
public class DocxReader : EtiketliOkuyucu
{
    protected override IEnumerable<string> SatirlariOku(string dosyaYolu)
    {
        var satirlar = new List<string>();

        using var belge = WordprocessingDocument.Open(dosyaYolu, false);
        var govde = belge.MainDocumentPart?.Document?.Body;
        if (govde == null)
        {
            return satirlar;
        }

        // Gövdedeki öğeleri belgedeki sırasıyla dolaş
        foreach (var oge in govde.ChildElements)
        {
            if (oge is W.Paragraph paragraf)
            {
                satirlar.Add(paragraf.InnerText);
            }
            else if (oge is W.Table tablo)
            {
                foreach (var tabloSatiri in tablo.Descendants<W.TableRow>())
                {
                    satirlar.AddRange(TabloSatiriniOku(tabloSatiri));
                }
            }
        }

        return satirlar;
    }

    /// <summary>
    /// Tablo satırını "Etiket: değer" metinlerine çevirir.
    /// İlk hücrede ':' yoksa hücreler (etiket, değer) çiftleri olarak okunur;
    /// varsa her hücre zaten "Etiket: değer" biçimindedir.
    /// </summary>
    private static IEnumerable<string> TabloSatiriniOku(W.TableRow tabloSatiri)
    {
        var hucreler = tabloSatiri.Elements<W.TableCell>()
            .Select(HucreMetni)
            .ToList();

        if (hucreler.Count >= 2 && !hucreler[0].Contains(':'))
        {
            for (var i = 0; i + 1 < hucreler.Count; i += 2)
            {
                var etiket = hucreler[i].TrimEnd(':', ' ');
                if (etiket.Length > 0)
                {
                    yield return $"{etiket}: {hucreler[i + 1]}";
                }
            }
        }
        else
        {
            foreach (var hucre in hucreler)
            {
                yield return hucre;
            }
        }
    }

    /// <summary>Hücredeki paragrafları tek satırda birleştirir.</summary>
    private static string HucreMetni(W.TableCell hucre)
    {
        var parcalar = hucre.Elements<W.Paragraph>()
            .Select(p => p.InnerText.Trim())
            .Where(t => t.Length > 0);
        return string.Join(" ", parcalar);
    }
}
