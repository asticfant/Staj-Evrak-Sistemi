using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

/// <summary>
/// Gereksinim 4.3 – Admin'in yüklediği Excel test veri setini VeriSetiKayitlari tablosuna aktarır.
/// Kullanıcı hesabı OLUŞTURMAZ (hocanın cevabı: öğrenciler kendileri kayıt olur).
/// Hatalı satırlar eklenmez, satır no + sebep olarak sonuca yazılır.
/// Aynı öğrenci no zaten varsa satır atlanır.
/// </summary>
public class DatasetImportService
{
    private static readonly string[] ZorunluSutunlar =
    {
        "AdSoyad", "TcKimlikNo", "Uyruk", "OgrenciNo", "DogumTarihi", "Adres", "Bolum",
        "Ek16TeslimYili", "OncekiStajGunSayisi", "Email", "Telefon", "SaglikYardimiStatusu"
    };

    private static readonly string[] TarihBicimleri = { "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yyyy", "yyyy-MM-dd" };

    private readonly AppDbContext _db;

    public DatasetImportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ImportSonucu> ExceldenAktar(IFormFile dosya)
    {
        var sonuc = new ImportSonucu();

        XLWorkbook workbook;
        try
        {
            workbook = new XLWorkbook(dosya.OpenReadStream());
        }
        catch (Exception)
        {
            sonuc.Hatalar.Add("Dosya okunamadı. Geçerli bir .xlsx dosyası yükleyin.");
            return sonuc;
        }

        using (workbook)
        {
            var sayfa = workbook.Worksheets.First();
            var basliklar = BasliklariOku(sayfa.Row(1));

            var eksik = ZorunluSutunlar.Where(s => !basliklar.ContainsKey(s)).ToList();
            if (eksik.Count > 0)
            {
                sonuc.Hatalar.Add("Eksik sütun(lar): " + string.Join(", ", eksik));
                return sonuc;
            }

            // Aynı dosyada tekrar eden numaraları da yakalamak için
            var buDosyadakiler = new HashSet<string>();

            foreach (var satir in sayfa.RowsUsed().Where(r => r.RowNumber() > 1))
            {
                if (satir.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                {
                    continue;
                }

                VeriSetiKaydi kayit;
                try
                {
                    kayit = SatirdanKayitOlustur(satir, basliklar);
                }
                catch (FormatException ex)
                {
                    sonuc.Hatalar.Add($"Satır {satir.RowNumber()}: {ex.Message}");
                    continue;
                }

                if (KayitVarMi(kayit.OgrenciNo) || !buDosyadakiler.Add(kayit.OgrenciNo))
                {
                    sonuc.AtlananSayisi++;
                    continue;
                }

                _db.VeriSetiKayitlari.Add(kayit);
                sonuc.EklenenSayisi++;
            }
        }

        await _db.SaveChangesAsync();
        return sonuc;
    }

    private bool KayitVarMi(string ogrenciNo)
    {
        return _db.VeriSetiKayitlari.Any(k => k.OgrenciNo == ogrenciNo);
    }

    /// <summary>Başlık adı -> sütun numarası. Fazladan sütunlar (örn. TestNotu) haritada olur ama kullanılmaz.</summary>
    private static Dictionary<string, int> BasliklariOku(IXLRow baslikSatiri)
    {
        var harita = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var hucre in baslikSatiri.CellsUsed())
        {
            var ad = hucre.GetString().Trim();
            if (ad.Length > 0 && !harita.ContainsKey(ad))
            {
                harita[ad] = hucre.Address.ColumnNumber;
            }
        }
        return harita;
    }

    /// <summary>Sütunları sıraya göre değil, başlık satırındaki ADA göre bulur. Hatalıysa FormatException fırlatır.</summary>
    private static VeriSetiKaydi SatirdanKayitOlustur(IXLRow satir, Dictionary<string, int> basliklar)
    {
        string Al(string sutun) => Metin(satir.Cell(basliklar[sutun]));

        var hatalar = new List<string>();

        var ogrenciNo = Al("OgrenciNo");
        var adSoyad = Al("AdSoyad");
        var tc = Al("TcKimlikNo");
        var uyruk = Al("Uyruk");
        var adres = Al("Adres");
        var email = Al("Email");
        var telefon = Al("Telefon");

        if (!Regex.IsMatch(ogrenciNo, @"^\d+$")) hatalar.Add("öğrenci no boş veya rakam dışı karakter içeriyor");
        if (adSoyad.Length == 0) hatalar.Add("ad soyad boş");

        var yabanci = uyruk.Length > 0 && !Regex.IsMatch(uyruk.ToUpperInvariant(), @"^(T\.?C\.?|TÜRKİYE|TURKIYE)$");
        if (!Regex.IsMatch(tc, @"^\d{11}$")) hatalar.Add("kimlik no 11 haneli olmalı");
        else if (tc[0] == '0') hatalar.Add("kimlik no 0 ile başlayamaz");
        else if (yabanci && !tc.StartsWith("99")) hatalar.Add("yabancı uyruklu öğrencinin kimlik numarası 99 ile başlamalı");

        if (!Regex.IsMatch(telefon, @"^\+90 ?\d{3} ?\d{3} ?\d{2} ?\d{2}$")) hatalar.Add("telefon +90 5xx xxx xx xx biçiminde olmalı");
        if (email.Length == 0) hatalar.Add("e-posta boş");
        else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) hatalar.Add("e-posta geçersiz");

        var a = adres.ToUpper(new CultureInfo("tr-TR")).Replace('İ', 'I');
        var acikAdres = a.Contains("MAH")
            && (a.Contains("CAD") || a.Contains("SOK"))
            && a.Contains("BINA NO")
            && a.Contains("DAIRE NO");
        if (!acikAdres)
        {
            hatalar.Add("adres açık adres değil (Mahalle, Cadde/Sokak, Bina No, Daire No olmalı)");
        }

        DateTime dogum = default;
        if (!TarihOku(satir.Cell(basliklar["DogumTarihi"]), out dogum)) hatalar.Add("doğum tarihi okunamadı");

        int stajGun = 0;
        var stajMetin = Al("OncekiStajGunSayisi");
        if (stajMetin.Length > 0 && !int.TryParse(stajMetin, out stajGun)) hatalar.Add("staj gün sayısı sayı olmalı");

        if (hatalar.Count > 0)
        {
            throw new FormatException(string.Join("; ", hatalar));
        }

        return new VeriSetiKaydi
        {
            OgrenciNo = ogrenciNo,
            AdSoyad = adSoyad,
            TcKimlikNo = tc,
            Uyruk = uyruk,
            DogumTarihi = dogum,
            Adres = adres,
            Bolum = Al("Bolum"),
            Ek16TeslimYili = Al("Ek16TeslimYili"),
            OncekiStajGunSayisi = stajGun,
            Email = email,
            Telefon = telefon,
            SaglikYardimiStatusu = Al("SaglikYardimiStatusu")
        };
    }

    /// <summary>
    /// Hücreyi metin olarak okur. TC / öğrenci no / telefon sayı olarak girilmişse
    /// "1,23E+10" gibi görünmesin diye tam sayıya çevrilir.
    /// </summary>
    private static string Metin(IXLCell hucre)
    {
        if (hucre.DataType == XLDataType.Number)
        {
            return ((long)hucre.GetDouble()).ToString(CultureInfo.InvariantCulture);
        }
        return hucre.GetString().Trim();
    }

    private static bool TarihOku(IXLCell hucre, out DateTime tarih)
    {
        if (hucre.DataType == XLDataType.DateTime)
        {
            tarih = hucre.GetDateTime().Date;
            return true;
        }
        return DateTime.TryParseExact(hucre.GetString().Trim(), TarihBicimleri, CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih);
    }
}
