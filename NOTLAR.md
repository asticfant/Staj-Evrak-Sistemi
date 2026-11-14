# Proje Notları

## Tasarım Kararları
- **Veritabanı: SQLite.** Kurulum gerektirmiyor, veritabanı tek bir dosya. 3 kişi farklı bilgisayarlarda sorunsuz çalışabiliyor.
- **Giriş sistemi kendimiz yazdık (ASP.NET Identity yok).** Kodun her satırını açıklayabilmek ve NYP prensiplerini gösterebilmek için.
- **Tek proje, klasörlü yapı.** Models / Data / Services / Readers / Dtos / Controllers / Views.
- **Kilitlenme: 5 hatalı denemede 15 dakika.**
- **GitHub: Pull Request ile çalışma.** Herkes başkasının kodunu da okumuş oluyor.

## Hocanın cevaplarından sonra değişen kararlar (01.11)
- **Öğrenciler kendileri kayıt olur, şifresini kendisi belirler.** (Eski karar "ilk şifre 123456" iptal.)
- **Excel veri seti kullanıcı hesabı oluşturmaz.** `VeriSetiKayitlari` tablosuna gider, sadece admin tarafında belgelerle karşılaştırmak için kullanılır.
- **Öğrenci bilgileri iki yerden gelir:**
  - Kişisel bilgiler: kayıttan sonra profil formunda bir kez (`Student`)
  - Staj bilgileri: her belge yüklemesinde, dosyayla aynı sayfadaki formda (`StajBilgisi`)
- **Belge hem okunur hem formla karşılaştırılır.** Uyuşmazlık varsa belge Hatalı/Eksik olur; rapor "girdiğin / belgede okunan" değerleri yan yana gösterir.
- **Vizede belge okuma:** .docx (OpenXml) ve metin tabanlı PDF (PdfPig). Resim / taranmış PDF / .doc finalde OCR ile.
- **Vize kontrolleri sadece dosya formatı ve uzantı.** Bölüm 6 kuralları (TC, telefon, tarih, staj günü, mühendis...) finale kaldı.
- **Belge onayı:** Uyuşmazlık yoksa sistem otomatik **Onaylandı** yapar, **Tamamlandı**'ya admin geçirir. (Eski karar "otomatik Tamamlandı" iptal.)
- **Tamamlandı belge:** Öğrenci değiştiremez/silemez, **admin güncelleyip silebilir** (hocanın cevabı).
- **Okunamayan belge türü (vizede resim):** Belge "İnceleniyor"da kalır, admin elle inceler.

## Vize öncesi düzeltmeler (14.11)
- **Çok biçimlilik güçlendirildi:**
  - `User.AnaSayfa()` abstract; `Student` ve `Admin` override eder. Girişten sonra yönlendirme `Rol == Admin ?` yerine `AnaSayfa()` ile.
  - `DocxReader` yazıldı. PDF ve DOCX okuyucular ortak `EtiketliOkuyucu` üst sınıfından türer (Template Method: `Oku()` ortak, `SatirlariOku()` override).
- **403:** Erişim engellendi sayfası 403 döner ve 5 saniye sonra giriş sayfasına yönlendirir (Gereksinim 4.2).
- **Alan kuralları (Bölüm 6.1 / 6.2) vizeye çekildi:** `Validators/` klasöründe `IFieldValidator` ve gerçeklemeleri (TC, yabancı kimlik, telefon, e-posta, adres). Profil formu ve Excel aktarımı aynı sınıfları kullanır. Adres artık sırayı ve İlçe/İl'i de kontrol eder. Belge kuralları (`IBelgeKurali`: teslim tarihi, staj günü, öğretim yılı, mühendis sayısı) hâlâ final.
- **Models değişikliği migration gerektirmez** (sadece metot eklendi, sütun yok).
- **Sunumda söylenecek:** Vizede okunan belgeler bizim test şablonumuzdaki "Etiket: değer" biçimindeki metin tabanlı PDF/DOCX. Resmi taranmış Ek-16 → final (OCR).
- **Çıkış sorusu gelirse:** Cookie silinir; kopyalanmış bir cookie süresi (2 saat) dolana kadar geçerli kalır. Finalde kullanıcıya SecurityStamp eklenip her istekte kontrol edilebilir.

## Görev Dağılımı
- **Mevlüt:** Diyagramlar, Models + AppDbContext, AuthService, ProfilService, DocumentService, BelgeSorguService
- **Muhammed:** DocxReader, PdfReader, ComparisonService (Normalize dahil), DatasetImportService, BelgeIslemeService
- **Emirhan:** Tüm Controller'lar ve sayfalar (kayıt, giriş, profil, evrak yükleme + staj formu, rapor, admin paneli, 403)
- **Models klasörüne sadece Mevlüt dokunur.** Alan gerekirse ona söyleyin (migration çakışmasın).
- **Migration'ı sadece Models'i değiştiren kişi oluşturur** ve aynı PR'da gönderir; diğerleri `Add-Migration` çalıştırmaz.
- **Test verileri:** `TestVerileri/` klasöründe. Ana veri setindeki T.C. kimlik numaraları algoritmaya uyacak şekilde düzeltildi (ilk 9 hane aynı, son 2 hane yeniden hesaplandı).

## Diyagramlar
`Diyagramlar/` klasöründe. `.puml` dosyası kaynak, `.png` rapora girecek resim.
Rapor için: use case, ER, durum, 5 sınıf diyagramı ve 2 akış diyagramı (`akis-giris`, `akis-belge-yukleme`).
Kod değişirse önce diyagram güncellenir.

## Emirhan

## Muhammed

## Mevlüt
