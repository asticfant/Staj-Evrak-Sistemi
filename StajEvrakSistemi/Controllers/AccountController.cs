using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajEvrakSistemi.Dtos;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Services;

namespace StajEvrakSistemi.Controllers;

/// <summary>Herkese açık sayfalar: kayıt, giriş, çıkış, erişim engellendi (403).</summary>
public class AccountController : Controller
{
    private readonly AuthService _auth;

    public AccountController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpGet]
    public IActionResult Kayit() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kayit(KayitFormu form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var sonuc = await _auth.KayitOl(form.KullaniciAdi, form.Email, form.Sifre, form.SifreTekrar);
        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(form);
        }

        TempData["Mesaj"] = "Kayıt başarılı, giriş yapabilirsiniz.";
        return RedirectToAction(nameof(Giris));
    }

    [HttpGet]
    public IActionResult Giris(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Giris(GirisFormu form, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var sonuc = await _auth.GirisYap(form.KullaniciAdiVeyaEmail, form.Sifre);
        if (!sonuc.Basarili || sonuc.Kullanici is null)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(form);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        // Çok biçimlilik: Student ve Admin AnaSayfa()'yı kendine göre override eder.
        // Hangi rol olduğunu burada sormaya gerek yok.
        return LocalRedirect(sonuc.Kullanici.AnaSayfa());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cikis()
    {
        await _auth.CikisYap();
        return RedirectToAction(nameof(Giris));
    }

    /// <summary>Yanlış role sahip kullanıcı buraya yönlendirilir (Program.cs → AccessDeniedPath).</summary>
    [HttpGet]
    public IActionResult ErisimEngellendi()
    {
        // Gereksinim 4.2: yetkisiz istek 403 döner ve giriş sayfasına yönlendirilir.
        // Refresh başlığı tarayıcıyı 5 saniye sonra giriş sayfasına götürür;
        // bu sürede kullanıcı neden yönlendirildiğini okuyabilir.
        Response.StatusCode = StatusCodes.Status403Forbidden;
        Response.Headers.Append("Refresh", $"5;url={Url.Action(nameof(Giris))}");
        return View();
    }
}
