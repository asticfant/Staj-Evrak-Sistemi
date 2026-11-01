using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Services;

namespace StajEvrakSistemi.Controllers;

/// <summary>Sadece Admin rolündeki kullanıcılar erişebilir (Gereksinim 4.2 / 4.7).</summary>
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController : Controller
{
    private readonly AuthService _auth;
    private readonly BelgeSorguService _sorgu;
    private readonly DocumentService _belgeler;
    private readonly DatasetImportService _veriSeti;

    public AdminController(AuthService auth, BelgeSorguService sorgu, DocumentService belgeler, DatasetImportService veriSeti)
    {
        _auth = auth;
        _sorgu = sorgu;
        _belgeler = belgeler;
        _veriSeti = veriSeti;
    }

    // ---------------- Listeleme ----------------

    [HttpGet]
    public async Task<IActionResult> Belgeler([FromQuery] BelgeFiltresi filtre)   // TODO (Emirhan): Views/Admin/Belgeler.cshtml (filtre formu + tablo)
    {
        var belgeler = await _sorgu.Listele(filtre);
        ViewData["Filtre"] = filtre;
        return View(belgeler);
    }

    [HttpGet]
    public async Task<IActionResult> Tamamlananlar()   // TODO (Emirhan): Views/Admin/Tamamlananlar.cshtml ("Tamamlandı" sekmesi)
    {
        var belgeler = await _sorgu.TamamlananlariListele();
        return View(belgeler);
    }

    [HttpGet]
    public async Task<IActionResult> BelgeDetay(int documentId)   // TODO (Emirhan): Views/Admin/BelgeDetay.cshtml (okunanlar + hatalar + veri seti farkları)
    {
        var detay = await _sorgu.DetayGetir(documentId);
        if (detay is null)
        {
            return NotFound();
        }
        return View(detay);
    }

    // ---------------- Durum işlemleri ----------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tamamla(int documentId)
    {
        var admin = await AktifAdmin();
        await _belgeler.Tamamla(documentId, admin);
        TempData["Mesaj"] = "Belge tamamlandı.";
        return RedirectToAction(nameof(BelgeDetay), new { documentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DurumGuncelle(int documentId, DocumentDurum durum)
    {
        var admin = await AktifAdmin();
        await _belgeler.DurumGuncelle(documentId, durum, admin);
        TempData["Mesaj"] = "Belge durumu güncellendi.";
        return RedirectToAction(nameof(BelgeDetay), new { documentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BelgeSil(int documentId)
    {
        var admin = await AktifAdmin();
        await _belgeler.Sil(documentId, admin);
        TempData["Mesaj"] = "Belge silindi.";
        return RedirectToAction(nameof(Belgeler));
    }

    // ---------------- Veri seti ----------------

    [HttpGet]
    public IActionResult VeriSetiYukle() => View();   // TODO (Emirhan): Views/Admin/VeriSetiYukle.cshtml

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VeriSetiYukle(IFormFile dosya)
    {
        var sonuc = await _veriSeti.ExceldenAktar(dosya);
        return View(sonuc);
    }

    // ---------------- Final ----------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Ek17Olustur(int documentId)
    {
        // Final kapsamı (Gereksinim 4.7): Ek17Generator.Olustur(document)
        TempData["Mesaj"] = "Ek-17 üretimi final aşamasında eklenecek.";
        return RedirectToAction(nameof(BelgeDetay), new { documentId });
    }

    // ---------------- Yardımcılar ----------------

    private async Task<Admin> AktifAdmin()
    {
        var kullanici = await _auth.AktifKullaniciGetir();
        return kullanici as Admin
            ?? throw new UnauthorizedAccessException("Giriş yapan kullanıcı admin değil.");
    }
}
