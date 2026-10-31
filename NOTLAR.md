# Proje Notları

## Tasarım Kararları
- **Veritabanı: SQLite.** Kurulum gerektirmiyor, veritabanı tek bir dosya. 3 kişi farklı bilgisayarlarda sorunsuz çalışabiliyor.
- **Giriş sistemi kendimiz yazdık (ASP.NET Identity yok).** Kodun her satırını açıklayabilmek ve NYP prensiplerini gösterebilmek için.
- **Tek proje, klasörlü yapı.** Projenin boyutu için yeterli, anlatması kolay.
- **Vizede belge okuma: Word (.docx).** Finalde aynı arayüze OCR okuyucu eklenecek.
- **Öğrenci ilk şifresi: 123456.** Veri setinde şifre sütunu yok. Veritabanına hash'lenerek kaydediliyor.
- **Belge onayı: otomatik.** Kontrollerde hata yoksa sistem belgeyi önce Onaylandı, sonra Tamamlandı yapıyor. Gereksinimde admin yetkileri arasında onaylama yok, amaç bölümünde otomatik denetim isteniyor.
- **Kilitlenme: 5 hatalı denemede 15 dakika.**
- **GitHub: Pull Request ile çalışma.** Herkes başkasının kodunu da okumuş oluyor.

## Emirhan

## Muhammed

## Mevlüt
