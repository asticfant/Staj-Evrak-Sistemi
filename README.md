# Staj Evrak Doğrulama ve Yönetim Sistemi

Programlama Laboratuvarı III – 2026-2027 Güz dönemi projesi.

Öğrencilerin Ek-16 staj başvuru belgesini yükleyip doğruladığı, adminin belgeleri incelediği web uygulaması.

## Teknolojiler
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core + SQLite
- BCrypt (şifre hash), ClosedXML (Excel), DocumentFormat.OpenXml (Word)

## Çalıştırma
1. Projeyi Visual Studio ile aç (`StajEvrakSistemi/StajEvrakSistemi.slnx`).
2. Çalıştır (F5). Veritabanı (`stajevrak.db`) ilk açılışta kendiliğinden oluşur.
3. Varsayılan admin: `admin` / `Admin123!`

### Migration kuralı
- `Models` klasöründe değişiklik yapan kişi (Models sorumlusu) `Add-Migration <Ad>` komutunu çalıştırır ve `Migrations` klasörünü **aynı PR'da** gönderir.
- Diğerleri `Add-Migration` çalıştırmaz; sadece Pull yapıp F5'e basar.
- Herkes kendi migration'ını üretirse aynı tablolar için farklı migration'lar oluşur ve çakışır.

## Test verileri
`TestVerileri/` klasöründe:
- `OgrenciVeriSeti.xlsx` – 30 öğrencilik ana veri seti (admin paneline yüklenir)
- `OgrenciVeriSeti_HataliTest.xlsx` – içe aktarma kontrollerini denemek için 8 hatalı satır (`TestNotu` sütunu hatayı açıklar)

## Sayfalar
| Rol | Sayfa | Adres |
|---|---|---|
| Herkes | Ana sayfa, giriş, kayıt, 403 | `/`, `/Account/Giris`, `/Account/Kayit`, `/Account/ErisimEngellendi` |
| Öğrenci | Profil | `/Student/Profil` |
| Öğrenci | Ek-16 yükleme (dosya + staj formu) | `/Student/EvrakYukle` |
| Öğrenci | Belgelerim, kontrol raporu, staj bilgisi düzeltme | `/Student/Belgelerim`, `/Student/Rapor?documentId=..` |
| Admin | Belgeler (filtreli), Tamamlananlar, belge detayı | `/Admin/Belgeler`, `/Admin/Tamamlananlar`, `/Admin/BelgeDetay?documentId=..` |
| Admin | Test veri seti yükleme | `/Admin/VeriSetiYukle` |

Tasarım `wwwroot/css/site.css` (Bootstrap üzerine), tarayıcı tarafı kontroller `wwwroot/js/site.js`.
Yazı tipleri (IBM Plex) ve Bootstrap Icons `wwwroot/lib` içinde; internet olmadan da çalışır.

## Klasör yapısı
| Klasör | İçerik |
|---|---|
| `Models` | Veritabanı sınıfları ve enumlar |
| `Data` | `AppDbContext`, `DbSeeder` |
| `Services` | İş mantığı (giriş, belge, karşılaştırma, veri seti) |
| `Readers` | Belge okuyucular (Word, PDF, finalde OCR) |
| `Dtos` | Form ve sonuç nesneleri |
| `Controllers` / `Views` | Sayfalar |
| `../Diyagramlar` | UML, ER, durum ve use case diyagramları |

Tasarım kararları ve görev dağılımı için `NOTLAR.md`.
