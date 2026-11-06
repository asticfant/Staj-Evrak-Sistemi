using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Document>> Listele(BelgeFiltresi filtre)
    {
        IQueryable<Document> sorgu = _db.Documents
            .Include(d => d.Student)
            .Include(d => d.StajBilgisi);

        if (filtre.Durum.HasValue)
        {
            sorgu = sorgu.Where(d => d.Durum == filtre.Durum.Value);
        }
        if (!string.IsNullOrWhiteSpace(filtre.OgrenciNo))
        {
            var no = filtre.OgrenciNo.Trim();
            sorgu = sorgu.Where(d => d.Student!.OgrenciNo != null && d.Student.OgrenciNo.Contains(no));
        }
        if (!string.IsNullOrWhiteSpace(filtre.Bolum))
        {
            var bolum = filtre.Bolum.Trim();
            sorgu = sorgu.Where(d => d.Student!.Bolum != null && d.Student.Bolum.Contains(bolum));
        }
        if (filtre.BaslangicTarihi.HasValue)
        {
            var bas = filtre.BaslangicTarihi.Value.Date;
            sorgu = sorgu.Where(d => d.YuklenmeTarihi >= bas);
        }
        if (filtre.BitisTarihi.HasValue)
        {
            var bitis = filtre.BitisTarihi.Value.Date.AddDays(1); // bitiş günü dahil
            sorgu = sorgu.Where(d => d.YuklenmeTarihi < bitis);
        }

        return await sorgu.OrderByDescending(d => d.YuklenmeTarihi).ToListAsync();
    }

    public Task<List<Document>> TamamlananlariListele()
    {
        return Listele(new BelgeFiltresi { Durum = DocumentDurum.Tamamlandi });
    }

    public async Task<List<Document>> OgrencininBelgeleri(int studentId)
    {
        return await _db.Documents
            .Include(d => d.StajBilgisi)
            .Where(d => d.StudentId == studentId)
            .OrderByDescending(d => d.YuklenmeTarihi)
            .ToListAsync();
    }

    public async Task<BelgeDetay?> DetayGetir(int documentId)
    {
        var belge = await _db.Documents
            .Include(d => d.Student)
            .Include(d => d.StajBilgisi)
            .Include(d => d.OkunanAlanlar)
            .Include(d => d.Hatalar)
            .FirstOrDefaultAsync(d => d.Id == documentId);

        if (belge == null)
        {
            return null;
        }

        return new BelgeDetay
        {
            Belge = belge,
            StajBilgisi = belge.StajBilgisi,
            OkunanAlanlar = belge.OkunanAlanlar,
            Hatalar = belge.Hatalar,
            // Admin sayfası için; kaydedilmez, her açılışta hesaplanır
            VeriSetiFarklari = belge.OkunanAlanlar.Count > 0
                ? _karsilastirma.VeriSetiIleKarsilastir(belge.OkunanAlanlar)
                : new List<ValidationIssue>()
        };
    }
}
