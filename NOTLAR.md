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
