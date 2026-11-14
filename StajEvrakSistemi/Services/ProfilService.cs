using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Validators;

namespace StajEvrakSistemi.Services;

/// <summary>Öğrencinin kişisel bilgilerini girdiği / güncellediği profil.</summary>
public class ProfilService
{
    private static readonly IFieldValidator Telefon = new TelefonValidator();
    private static readonly IFieldValidator Adres = new AdresValidator();

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
    /// Zorunlu alanlar boş olmamalı; kimlik no (T.C./yabancı), telefon ve adres
    /// Validators klasöründeki kurallara uymalı.
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

        // 3) Biçim kontrolleri (Gereksinim 6.1 / 6.2). Kimlik kuralı uyruğa göre seçilir.
        var hatalar = new[]
            {
                KimlikValidatorSecici.Sec(form.Uyruk).Validate(form.TcKimlikNo),
                Telefon.Validate(form.Telefon),
                Adres.Validate(form.Adres)
            }
            .Where(h => h != null)
            .ToList();
        if (hatalar.Count > 0)
        {
            return IslemSonucu.Hata("Profil kaydedilmedi: " + string.Join("; ", hatalar));
        }

        // 4) Formdaki bilgileri öğrenciye kopyala
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

        // 5) Kaydet (öğrenci zaten tabloda, EF değişiklikleri kendisi izliyor)
        await _db.SaveChangesAsync();
        return IslemSonucu.Ok("Profil kaydedildi");
    }
}