using ClosedXML.Excel;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

/// <summary>
/// Gereksinim 4.3 – Admin'in yüklediği Excel test veri setini VeriSetiKayitlari tablosuna aktarır.
/// Kullanıcı hesabı OLUŞTURMAZ (hocanın cevabı: öğrenciler kendileri kayıt olur).
/// </summary>
public class DatasetImportService
{
    private readonly AppDbContext _db;

    public DatasetImportService(AppDbContext db)
    {
        _db = db;
    }

    public Task<ImportSonucu> ExceldenAktar(IFormFile dosya)
    {
        // TODO (Muhammed):
        // 1) using var workbook = new XLWorkbook(dosya.OpenReadStream());
        // 2) ilk sayfanın başlık satırından sonraki her satır için:
        //    - SatirdanKayitOlustur(satir); hatalıysa sonuc.Hatalar'a satır no + sebep ekle
        //    - KayitVarMi(ogrenciNo) ise AtlananSayisi++, değilse Add + EklenenSayisi++
        // 3) SaveChanges, sonucu dön
        throw new NotImplementedException();
    }

    private bool KayitVarMi(string ogrenciNo)
    {
        // TODO: _db.VeriSetiKayitlari.Any(k => k.OgrenciNo == ogrenciNo)
        throw new NotImplementedException();
    }

    private VeriSetiKaydi SatirdanKayitOlustur(IXLRow satir)
    {
        // TODO: Sütunları sıraya göre değil, başlık satırındaki ADA göre bul
        //   (TestVerileri/OgrenciVeriSeti.xlsx başlıkları):
        //   AdSoyad, TcKimlikNo, Uyruk, OgrenciNo, DogumTarihi, Adres, Bolum,
        //   Ek16TeslimYili, OncekiStajGunSayisi, Email, Telefon, SaglikYardimiStatusu
        //   Fazladan gelen sütunlar (örn. hatalı test dosyasındaki TestNotu) yok sayılır.
        //   TcKimlikNo, OgrenciNo, Telefon hücreleri metin olarak okunmalı (GetString()).
        throw new NotImplementedException();
    }
}
