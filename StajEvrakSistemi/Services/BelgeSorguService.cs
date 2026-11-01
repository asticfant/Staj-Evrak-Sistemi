using StajEvrakSistemi.Data;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

/// <summary>Gereksinim 4.2 / 4.7 – listeleme, filtreleme, detay (sadece okuma işlemleri).</summary>
public class BelgeSorguService
{
    private readonly AppDbContext _db;
    private readonly ComparisonService _karsilastirma;

    public BelgeSorguService(AppDbContext db, ComparisonService karsilastirma)
    {
        _db = db;
        _karsilastirma = karsilastirma;
    }

    // Not: Bu metotlar şimdilik boş liste döner ki arayüz (Emirhan) sayfaları
    // backend bitmeden de açabilsin. Gerçek sorgular yazılınca TODO'lar silinecek.

    public Task<List<Document>> Listele(BelgeFiltresi filtre)
    {
        // TODO (Mevlüt): _db.Documents.Include(d => d.Student) + filtre alanlarına göre Where'ler
        return Task.FromResult(new List<Document>());
    }

    public Task<List<Document>> TamamlananlariListele()
    {
        // TODO (Mevlüt): Listele(new BelgeFiltresi { Durum = DocumentDurum.Tamamlandi })
        return Task.FromResult(new List<Document>());
    }

    public Task<List<Document>> OgrencininBelgeleri(int studentId)
    {
        // TODO (Mevlüt): Where(d => d.StudentId == studentId), en yeni üstte
        return Task.FromResult(new List<Document>());
    }

    public Task<BelgeDetay?> DetayGetir(int documentId)
    {
        // TODO (Mevlüt):
        // 1) belgeyi Include(StajBilgisi, OkunanAlanlar, Hatalar, Student) ile getir
        // 2) VeriSetiFarklari = _karsilastirma.VeriSetiIleKarsilastir(belge.OkunanAlanlar)
        //    (admin sayfası için; kaydedilmez, her açılışta hesaplanır)
        return Task.FromResult<BelgeDetay?>(null);
    }
}
