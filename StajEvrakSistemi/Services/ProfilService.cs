using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

/// <summary>Öğrencinin kişisel bilgilerini girdiği / güncellediği profil.</summary>
public class ProfilService
{
    private readonly AppDbContext _db;

    public ProfilService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Öğrenciyi Id'sine göre getirir; yoksa null döner.</summary>
    public async Task<Student?> ProfilGetir(int studentId)
    {
        return await _db.Students.FindAsync(studentId);
    }

    /// <summary>
    /// Profil formundaki bilgileri öğrenciye kaydeder.
    /// Vizede sadece zorunlu alanların boş olup olmadığı kontrol edilir;
    /// format kontrolleri (TC, telefon...) final kapsamında.
    /// </summary>
    public async Task<IslemSonucu> ProfilGuncelle(int studentId, ProfilFormu form)
    {
        // 1) Öğrenci var mı?
        var ogrenci = await _db.Students.FindAsync(studentId);
        if (ogrenci == null)
        {
            return IslemSonucu.Hata("Öğrenci bulunamadı");
        }

        // 2) Zorunlu alanlar boş mu? (Student.ProfilTamamMi ile aynı alanlar)
        if (string.IsNullOrWhiteSpace(form.AdSoyad)
            || string.IsNullOrWhiteSpace(form.TcKimlikNo)
            || string.IsNullOrWhiteSpace(form.Uyruk)
            || string.IsNullOrWhiteSpace(form.OgrenciNo)
            || string.IsNullOrWhiteSpace(form.Adres)
            || string.IsNullOrWhiteSpace(form.Bolum)
            || string.IsNullOrWhiteSpace(form.Telefon)
            || form.DogumTarihi == default)
        {
            return IslemSonucu.Hata("Lütfen tüm zorunlu alanları doldurun");
        }

        // 3) Formdaki bilgileri öğrenciye kopyala
        ogrenci.AdSoyad = form.AdSoyad.Trim();
        ogrenci.TcKimlikNo = form.TcKimlikNo.Trim();
        ogrenci.Uyruk = form.Uyruk.Trim();
        ogrenci.OgrenciNo = form.OgrenciNo.Trim();
        ogrenci.DogumTarihi = form.DogumTarihi;
        ogrenci.Adres = form.Adres.Trim();
        ogrenci.Bolum = form.Bolum.Trim();
        ogrenci.Telefon = form.Telefon.Trim();
        ogrenci.SaglikYardimiStatusu = form.SaglikYardimiStatusu?.Trim();
        ogrenci.OncekiStajGunSayisi = form.OncekiStajGunSayisi;

        // 4) Kaydet (öğrenci zaten tabloda, EF değişiklikleri kendisi izliyor)
        await _db.SaveChangesAsync();
        return IslemSonucu.Ok("Profil kaydedildi");
    }
}