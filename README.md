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
- `OgrenciVeriSeti_Ek16.xlsx` – test PDF'lerindeki 10 öğrencinin doğru bilgileri (öğrenci no 230000001–230000010); admin taraflı veri seti karşılaştırması bununla gösterilir
- `Ek16_Validasyon_Test_Evraklari.zip` – 10 adet doldurulmuş Ek-16 PDF'i; bazılarında bilerek hata var (örn. 8: telefon boş, 9: ad "Cem123")

### Sunum için demo senaryosu
1. Admin (`admin` / `Admin123!`) → Veri seti → `OgrenciVeriSeti_Ek16.xlsx` yükle (10 eklendi).
2. Öğrenci olarak kayıt ol; e-posta `ahmet@ogr.kou.edu.tr`. Profili 1. PDF'teki bilgilerle doldur
   (Ahmet Yilmaz, 12345678901, T.C., 230000001, 01.01.2001, Yazılım Mühendisliği, +90 530 111 22 33, Ailesinden alıyor, önceki staj 20,
   adres: Cumhuriyet Mah, Inonu Cad, Bina No: 1, Daire No: 1, Izmit, Kocaeli).
3. `1_Ahmet_Yilmaz_Ek16.pdf` yükle; staj formunda teslim yılı 2025-2026, mühendis sayısı 4 (bilerek yanlış), bölüm Yazılım Mühendisliği → **Hatalı/Eksik**.
4. Staj bilgisini düzelt, mühendis sayısı 3 → **Onaylandı**.
5. Admin → belge detayı (veri seti farkı yok) → **Tamamla** → Tamamlananlar sekmesi.
6. Aynı öğrenciyle `9_Cem123_Arslan_Ek16.pdf` yükle → çok sayıda uyuşmazlık; admin detayında veri setindeki "Cem Arslan" kaydıyla farklar.
7. Güvenlik: adı `.pdf` yapılmış `.txt` dosyası reddedilir; öğrenci `/Admin/Belgeler` açınca 403; 5 yanlış şifrede hesap kilitlenir.

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
