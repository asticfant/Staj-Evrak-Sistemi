using ClosedXML.Excel;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

public class DatasetImportService
{
    private readonly AppDbContext _db;

    public DatasetImportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ImportSonucu> ExceldenAktar(IFormFile dosya)
    {
        var sonuc = new ImportSonucu();
        if (sonuc.Hatalar == null) sonuc.Hatalar = new List<string>();

        using var stream = dosya.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        var firstRow = worksheet.FirstRowUsed();
        var headerColumns = new Dictionary<string, int>();
        foreach (var cell in firstRow.CellsUsed())
        {
            headerColumns[cell.GetString().Trim()] = cell.Address.ColumnNumber;
        }

        var dataRows = worksheet.RowsUsed().Skip(1);

        foreach (var row in dataRows)
        {
            try
            {
                string ogrenciNo = GetCellValue(row, headerColumns, "OgrenciNo");
                if (string.IsNullOrWhiteSpace(ogrenciNo)) continue;

                if (KayitVarMi(ogrenciNo))
                {
                    sonuc.AtlananSayisi++;
                }
                else
                {
                    var yeniKayit = SatirdanKayitOlustur(row, headerColumns);
                    _db.VeriSetiKayitlari.Add(yeniKayit);
                    sonuc.EklenenSayisi++;
                }
            }
            catch (Exception)
            {
                sonuc.Hatalar.Add($"Satır {row.RowNumber()}: Beklenmeyen hata oluştu.");
            }
        }

        await _db.SaveChangesAsync();
        return sonuc;
    }

    private bool KayitVarMi(string ogrenciNo)
    {
        return _db.VeriSetiKayitlari.Any(k => k.OgrenciNo == ogrenciNo);
    }

    private string GetCellValue(IXLRow row, Dictionary<string, int> headers, string columnName)
    {
        if (headers.TryGetValue(columnName, out int colIndex))
        {
            return row.Cell(colIndex).GetString().Trim();
        }
        return string.Empty;
    }

    private VeriSetiKaydi SatirdanKayitOlustur(IXLRow row, Dictionary<string, int> headers)
    {
        DateTime.TryParse(GetCellValue(row, headers, "DogumTarihi"), out DateTime dogumTarihi);
        int.TryParse(GetCellValue(row, headers, "OncekiStajGunSayisi"), out int stajGun);

        return new VeriSetiKaydi
        {
            OgrenciNo = GetCellValue(row, headers, "OgrenciNo"),
            TcKimlikNo = GetCellValue(row, headers, "TcKimlikNo"),
            AdSoyad = GetCellValue(row, headers, "AdSoyad"),
            Uyruk = GetCellValue(row, headers, "Uyruk"),
            DogumTarihi = dogumTarihi,
            Adres = GetCellValue(row, headers, "Adres"),
            Bolum = GetCellValue(row, headers, "Bolum"),
            Ek16TeslimYili = GetCellValue(row, headers, "Ek16TeslimYili"),
            OncekiStajGunSayisi = stajGun,
            Email = GetCellValue(row, headers, "Email"),
            Telefon = GetCellValue(row, headers, "Telefon"),
            SaglikYardimiStatusu = GetCellValue(row, headers, "SaglikYardimiStatusu")
        };
    }
}