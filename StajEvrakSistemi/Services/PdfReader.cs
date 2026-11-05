using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UglyToad.PdfPig;

namespace StajEvrakSistemi.Services
{
    // NYP Prensibi: Soyutlama (Abstraction)
    public interface IDocumentReader
    {
        Dictionary<string, string> BelgeyiOku(Stream dosyaAkisi);
    }

    // NYP Prensibi: Kalıtım/Çok Biçimlilik (Inheritance/Polymorphism)
    public class PdfReader : IDocumentReader
    {
        // Yönergede istenen zorunlu etiket eşleştirmeleri
        private readonly Dictionary<string, string> _etiketHaritasi = new Dictionary<string, string>
        {
            { "Ogrenci Ad Soyad", "AdSoyad" },
            { "Kimlik No", "TcKimlikNo" },
            { "Telefon No", "Telefon" }
        };

        public Dictionary<string, string> BelgeyiOku(Stream dosyaAkisi)
        {
            var okunanVeriler = new Dictionary<string, string>();

            // PdfPig kullanılarak PDF belgesinin açılması
            using (PdfDocument pdf = PdfDocument.Open(dosyaAkisi))
            {
                foreach (var page in pdf.GetPages())
                {
                    // Sayfadaki kelimeleri alıp satır satır birleştiriyorus
                    var satirlar = page.GetWords()
                                       .GroupBy(w => Math.Round(w.BoundingBox.Bottom, 1))
                                       .Select(g => string.Join(" ", g.Select(w => w.Text)))
                                       .ToList();

                    foreach (var satir in satirlar)
                    {
                        // "Etiket: değer" biçimindeki satırları ayıklama
                        if (satir.Contains(":"))
                        {
                            var parcalar = satir.Split(new[] { ':' }, 2);
                            var etiket = parcalar[0].Trim();
                            var deger = parcalar[1].Trim();

                            // Eğer etiket haritamızda (Ogrenci Ad Soyad vb.) varsa, sistemdeki Ek16Alanlari anahtarına dönüştürerek kaydet
                            if (_etiketHaritasi.ContainsKey(etiket))
                            {
                                string sistemAnahtari = _etiketHaritasi[etiket];
                                okunanVeriler[sistemAnahtari] = deger;
                            }
                        }
                    }
                }
            }

            return okunanVeriler;
        }
    }
}