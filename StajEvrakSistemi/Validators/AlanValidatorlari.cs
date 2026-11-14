using System.Globalization;
using System.Text.RegularExpressions;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Validators;

/// <summary>
/// Tek bir alanın biçim kontrolü (Gereksinim 6.1 / 6.2).
/// Geçerliyse null, değilse kullanıcıya gösterilecek hata mesajı döner.
/// Profil formu ve Excel veri seti aynı kuralları bu sınıflar üzerinden kullanır;
/// finalde belgeden okunan değerler de aynı sınıflarla kontrol edilecek.
/// </summary>
public interface IFieldValidator
{
    string? Validate(string? deger);
}

/// <summary>T.C. uyruklu: 11 hane, sadece rakam, ilk hane 0 olamaz.</summary>
public class TcKimlikValidator : IFieldValidator
{
    public string? Validate(string? deger)
    {
        var tc = deger?.Trim() ?? string.Empty;
        if (!Regex.IsMatch(tc, @"^\d{11}$"))
        {
            return "kimlik no 11 haneli olmalı ve sadece rakam içermeli";
        }
        if (tc[0] == '0')
        {
            return "kimlik no 0 ile başlayamaz";
        }
        return null;
    }
}

/// <summary>Yabancı uyruklu: 11 hane ve 99 ile başlamalı.</summary>
public class YabanciKimlikValidator : IFieldValidator
{
    public string? Validate(string? deger)
    {
        var no = deger?.Trim() ?? string.Empty;
        if (!Regex.IsMatch(no, @"^\d{11}$"))
        {
            return "kimlik no 11 haneli olmalı ve sadece rakam içermeli";
        }
        if (!no.StartsWith("99"))
        {
            return "yabancı uyruklu öğrencinin kimlik numarası 99 ile başlamalı";
        }
        return null;
    }
}

/// <summary>+90 xxx xxx xx xx (boşluklar isteğe bağlı).</summary>
public class TelefonValidator : IFieldValidator
{
    public string? Validate(string? deger)
    {
        var tel = deger?.Trim() ?? string.Empty;
        return Regex.IsMatch(tel, @"^\+90 ?\d{3} ?\d{3} ?\d{2} ?\d{2}$")
            ? null
            : "telefon +90 5xx xxx xx xx biçiminde olmalı";
    }
}

/// <summary>ad@alanadi.uzanti biçimi; boş olamaz.</summary>
public class EmailValidator : IFieldValidator
{
    public string? Validate(string? deger)
    {
        var email = deger?.Trim() ?? string.Empty;
        if (email.Length == 0)
        {
            return "e-posta boş";
        }
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") ? null : "e-posta geçersiz";
    }
}

/// <summary>
/// Açık adres: Mahalle, Cadde/Sokak, Bina No, Daire No bu SIRAYLA bulunmalı
/// ve Daire No'dan sonra İlçe/İl gelmeli. Örn: "Cumhuriyet Mah., Lale Cad., Bina No:5, Daire No:2, İzmit, Kocaeli"
/// </summary>
public class AdresValidator : IFieldValidator
{
    private const string Mesaj = "adres açık adres değil (Mahalle, Cadde/Sokak, Bina No, Daire No, İlçe/İl sırasıyla olmalı)";

    public string? Validate(string? deger)
    {
        var a = (deger ?? string.Empty).ToUpper(new CultureInfo("tr-TR")).Replace('İ', 'I');

        var mahalle = a.IndexOf("MAH", StringComparison.Ordinal);
        var cadde = a.IndexOf("CAD", StringComparison.Ordinal);
        var sokak = a.IndexOf("SOK", StringComparison.Ordinal);
        var caddeSokak = cadde < 0 ? sokak : (sokak < 0 ? cadde : Math.Min(cadde, sokak));
        var bina = a.IndexOf("BINA NO", StringComparison.Ordinal);
        var daire = a.IndexOf("DAIRE NO", StringComparison.Ordinal);

        // Dördü de var mı ve sırası doğru mu?
        if (mahalle < 0 || caddeSokak < 0 || bina < 0 || daire < 0
            || !(mahalle < caddeSokak && caddeSokak < bina && bina < daire))
        {
            return Mesaj;
        }

        // Daire No'dan sonra virgülle ayrılmış en az bir parça (İlçe/İl) olmalı
        var ilceIl = Regex.Match(a[daire..], @"^DAIRE NO\s*:?\s*\S+?\s*,(.+)$");
        if (!ilceIl.Success || ilceIl.Groups[1].Value.Split(',').All(p => p.Trim().Length == 0))
        {
            return Mesaj;
        }

        return null;
    }
}

/// <summary>
/// Uyruğa göre doğru kimlik doğrulayıcısını seçer (Gereksinim 6.2: "uyruk seçimine göre uygun kontrol").
/// Dönüş tipi arayüz: çağıran taraf T.C. mi yabancı mı diye if yazmaz (çok biçimlilik).
/// </summary>
public static class KimlikValidatorSecici
{
    private static readonly IFieldValidator Tc = new TcKimlikValidator();
    private static readonly IFieldValidator Yabanci = new YabanciKimlikValidator();

    public static IFieldValidator Sec(string? uyruk)
    {
        return Student.YabanciUyrukMu(uyruk) ? Yabanci : Tc;
    }
}
