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

    public Task<Student?> ProfilGetir(int studentId)
    {
        // TODO (Mevlüt): _db.Students.FindAsync(studentId)
        throw new NotImplementedException();
    }

    public Task<IslemSonucu> ProfilGuncelle(int studentId, ProfilFormu form)
    {
        // TODO (Mevlüt):
        // 1) öğrenciyi bul, yoksa hata
        // 2) form alanlarını öğrenciye kopyala
        // 3) SaveChanges, IslemSonucu.Ok("Profil kaydedildi")
        // Not: Format kontrolleri (TC, telefon...) final kapsamında, vizede sadece boş mu kontrolü yeterli.
        throw new NotImplementedException();
    }
}
