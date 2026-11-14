using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Services;

namespace StajEvrakSistemi.Controllers;

/// <summary>Sadece Student rolündeki kullanıcılar erişebilir (Gereksinim 4.2).</summary>
[Authorize(Roles = nameof(UserRole.Student))]
public class StudentController : Controller
{
    private readonly AuthService _auth;
    private readonly ProfilService _profil;
    private readonly DocumentService _belgeler;
    private readonly BelgeSorguService _sorgu;

    public StudentController(AuthService auth, ProfilService profil, DocumentService belgeler, BelgeSorguService sorgu)
    {
        _auth = auth;
        _profil = profil;
        _belgeler = belgeler;
        _sorgu = sorgu;
    }

    // ---------------- Profil ----------------

    [HttpGet]
    public async Task<IActionResult> Profil()
    {
        var ogrenci = await AktifOgrenci();
        return View(ogrenci);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProfilKaydet(ProfilFormu form)
    {
        var ogrenci = await AktifOgrenci();
        var sonuc = await _profil.ProfilGuncelle(ogrenci.Id, form);
        TempData["Mesaj"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Profil));
    }

    // ---------------- Evrak yükleme (dosya + staj formu aynı sayfada) ----------------

    [HttpGet]
    public IActionResult EvrakYukle() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EvrakYukle(
        IFormFile dosya,
        [Bind(StajFormAlanlari)] StajBilgisi staj)
    {
        var ogrenci = await AktifOgrenci();
        var sonuc = await _belgeler.BelgeYukle(dosya, staj, ogrenci);
        if (!sonuc.Basarili || sonuc.Belge is null)
        {
            // Geçersiz format / uzantı uyarısı (Gereksinim 4.4)
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(staj);
        }

        return RedirectToAction(nameof(Rapor), new { documentId = sonuc.Belge.Id });
    }

    [HttpGet]
    public async Task<IActionResult> StajBilgisiDuzelt(int documentId)
    {
        var detay = await KendiBelgesininDetayi(documentId);
        if (detay is null)
        {
            return NotFound();
        }

        // Sadece Hatalı/Eksik belge düzeltilebilir; diğerlerinde form hiç açılmaz.
        if (detay.Belge.Durum != DocumentDurum.HataliEksik)
        {
            TempData["Mesaj"] = $"\"{detay.Belge.Durum.GorunenAd()}\" durumundaki belgenin bilgileri düzeltilemez.";
            return RedirectToAction(nameof(Rapor), new { documentId });
        }
        return View(detay.StajBilgisi);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StajBilgisiDuzelt(int documentId, [Bind(StajFormAlanlari)] StajBilgisi staj)
    {
        var ogrenci = await AktifOgrenci();
        var sonuc = await _belgeler.StajBilgisiDuzelt(documentId, staj, ogrenci);
        TempData["Mesaj"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Rapor), new { documentId });
    }

    // ---------------- Belgeler ve rapor ----------------

    [HttpGet]
    public async Task<IActionResult> Belgelerim()
    {
        var ogrenci = await AktifOgrenci();
        var belgeler = await _sorgu.OgrencininBelgeleri(ogrenci.Id);
        return View(belgeler);
    }

    [HttpGet]
    public async Task<IActionResult> Rapor(int documentId)
    {
        var detay = await KendiBelgesininDetayi(documentId);
        if (detay is null)
        {
            return NotFound();
        }
        return View(detay);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GoogleFormUrlKaydet(string url)
    {
        // Final kapsamı (Gereksinim 4.6): GoogleFormUrlValidator + Student.GoogleFormUrl
        TempData["Mesaj"] = "Google Form entegrasyonu final aşamasında eklenecek.";
        return RedirectToAction(nameof(Profil));
    }

    // ---------------- Yardımcılar ----------------

    // Overposting önlemi: formdan sadece bu alanlar alınır (Id, DocumentId alınmaz).
    private const string StajFormAlanlari =
        "Ek16TeslimYili,StajBaslangic,StajBitis,CalisilanGunler,IsyeriAdi,IsyeriAdresi," +
        "IsyeriTelefon,IsyeriEmail,MuhendisSayisi,MuhendisBolumu";

    private async Task<Student> AktifOgrenci()
    {
        var kullanici = await _auth.AktifKullaniciGetir();
        return kullanici as Student
            ?? throw new UnauthorizedAccessException("Giriş yapan kullanıcı öğrenci değil.");
    }

    /// <summary>Öğrenci sadece kendi belgesini görebilir.</summary>
    private async Task<BelgeDetay?> KendiBelgesininDetayi(int documentId)
    {
        var ogrenci = await AktifOgrenci();
        var detay = await _sorgu.DetayGetir(documentId);
        return detay is not null && detay.Belge.StudentId == ogrenci.Id ? detay : null;
    }
}
