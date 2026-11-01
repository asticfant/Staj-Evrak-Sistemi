using StajEvrakSistemi.Data;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

/// <summary>
/// İki karşılaştırma:
///  - FormIleKarsilastir   : belgeden okunan  &lt;-&gt; öğrencinin girdiği (öğrenci tarafı, "teslime hazır mı?")
///  - VeriSetiIleKarsilastir: belgeden okunan &lt;-&gt; Excel veri seti (admin tarafı)
/// Alan adları Readers/Ek16Alanlari sınıfındaki sabitlerdir.
/// </summary>
public class ComparisonService
{
    private readonly AppDbContext _db;

    public ComparisonService(AppDbContext db)
    {
        _db = db;
    }

    public List<ValidationIssue> FormIleKarsilastir(Student student, StajBilgisi staj, List<OkunanAlan> okunanAlanlar)
    {
        // TODO (Muhammed):
        // her okunan alan için:
        //   girilen = GirilenDeger(student, staj, alan.AlanAdi)
        //   alan.Deger boşsa -> Tur = Okunamadi
        //   Normalize(girilen) != Normalize(okunan) ise -> Tur = Uyusmazlik,
        //   GirilenDeger ve OkunanDeger doldurulur (raporda yan yana gösterilecek)
        throw new NotImplementedException();
    }

    public List<ValidationIssue> VeriSetiIleKarsilastir(List<OkunanAlan> okunanAlanlar)
    {
        // TODO (Muhammed):
        // 1) okunan alanlardan OgrenciNo'yu bul
        // 2) _db.VeriSetiKayitlari'nda bu numarayı ara; yoksa tek bir uyarı dön ("Veri setinde kayıt yok")
        // 3) alan alan karşılaştır (Normalize ile), farkları ValidationIssue olarak dön
        throw new NotImplementedException();
    }

    private string? GirilenDeger(Student student, StajBilgisi staj, string alanAdi)
    {
        // TODO (Muhammed): alanAdi'na göre doğru özelliği döndür, örn.
        //   Ek16Alanlari.AdSoyad      => student.AdSoyad
        //   Ek16Alanlari.StajBaslangic => staj.StajBaslangic.ToString("dd.MM.yyyy")
        throw new NotImplementedException();
    }

    private string Normalize(string alanAdi, string? deger)
    {
        // TODO (Muhammed):
        // - null -> ""
        // - Trim, fazla boşlukları teke indir
        // - ToUpper(new CultureInfo("tr-TR"))   (i / İ sorunu için)
        // - TC ve telefon alanlarında sadece rakamlar kalsın
        // - tarih alanlarında DateTime'a çevirip "yyyy-MM-dd" biçimine getir
        throw new NotImplementedException();
    }
}
