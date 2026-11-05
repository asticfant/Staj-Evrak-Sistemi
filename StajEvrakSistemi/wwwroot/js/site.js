// =====================================================================
//  Tarayıcı tarafı kontroller
//
//  Bu kontroller sadece kullanıcıya ANINDA uyarı vermek içindir.
//  Asıl güvenlik kontrolleri sunucuda yapılır (DocumentService: uzantı,
//  dosya imzası, boyut). JavaScript kapatılabilir veya atlatılabilir;
//  bu yüzden buradaki hiçbir kontrol sunucudakinin yerine geçmez.
//
//  Hangi alanın hangi kontrolü alacağı View'lardaki data-... nitelikleriyle seçilir:
//    data-dosya-kontrol  : <input type="file"> için uzantı + boyut kontrolü
//    data-tarih-baslangic / data-tarih-bitis : bitiş başlangıçtan önce olamaz
//    data-gun-grubu      : en az bir checkbox seçilmeli
//    data-tek-gonderim   : form gönderilince buton kilitlenir (iki kez gönderilmesin)
// =====================================================================

(function () {
    "use strict";

    // Bir alanın altına hata mesajı yazar (yoksa oluşturur) ve alanı kırmızı yapar.
    function hataGoster(alan, mesaj, kapsayici) {
        var yer = kapsayici || alan.parentElement;
        var kutu = yer.querySelector(".istemci-hata");
        if (!kutu) {
            kutu = document.createElement("div");
            kutu.className = "istemci-hata text-danger small mt-1";
            kutu.setAttribute("role", "alert");
            yer.appendChild(kutu);
        }
        kutu.textContent = mesaj;
        alan.classList.add("is-invalid");
    }

    function hatayiTemizle(alan, kapsayici) {
        var yer = kapsayici || alan.parentElement;
        var kutu = yer.querySelector(".istemci-hata");
        if (kutu) kutu.remove();
        alan.classList.remove("is-invalid");
    }

    // ---------- 1) Dosya: uzantı ve boyut ----------
    // Örnek: <input type="file" data-dosya-kontrol data-uzantilar=".pdf,.png" data-max-mb="10">
    function dosyaGecerliMi(input) {
        var dosya = input.files && input.files[0];
        if (!dosya) return true; // boş bırakılmışsa "required" zaten uyarır

        var izinli = (input.dataset.uzantilar || "").split(",").map(function (u) { return u.trim().toLowerCase(); });
        var nokta = dosya.name.lastIndexOf(".");
        var uzanti = nokta >= 0 ? dosya.name.substring(nokta).toLowerCase() : "";
        if (izinli.length > 0 && izinli.indexOf(uzanti) === -1) {
            hataGoster(input, "Bu dosya türü kabul edilmiyor. Kabul edilenler: " + izinli.join(", "));
            return false;
        }

        var maxMb = parseFloat(input.dataset.maxMb || "0");
        if (maxMb > 0 && dosya.size > maxMb * 1024 * 1024) {
            hataGoster(input, "Dosya " + (dosya.size / 1024 / 1024).toFixed(1) + " MB; en fazla " + maxMb + " MB olabilir.");
            return false;
        }

        hatayiTemizle(input);
        return true;
    }

    // ---------- 2) Tarih: bitiş başlangıçtan önce olamaz ----------
    function tarihlerGecerliMi(form) {
        var bas = form.querySelector("[data-tarih-baslangic]");
        var bit = form.querySelector("[data-tarih-bitis]");
        if (!bas || !bit || !bas.value || !bit.value) return true;

        // type="date" değerleri "yyyy-MM-dd" biçiminde; bu biçimde metin karşılaştırması tarih sırasını verir.
        if (bit.value < bas.value) {
            hataGoster(bit, "Bitiş tarihi başlangıç tarihinden önce olamaz.");
            return false;
        }
        hatayiTemizle(bit);
        return true;
    }

    // ---------- 3) Günler: en az bir gün seçilmeli ----------
    // HTML'de checkbox grubuna "en az biri zorunlu" demenin hazır yolu yok; bu yüzden elle kontrol.
    function gunlerGecerliMi(form) {
        var grup = form.querySelector("[data-gun-grubu]");
        if (!grup) return true;
        var secili = grup.querySelectorAll("input[type=checkbox]:checked").length;
        if (secili === 0) {
            hataGoster(grup, "Staj yapacağın en az bir günü seç.", grup.parentElement);
            return false;
        }
        hatayiTemizle(grup, grup.parentElement);
        return true;
    }

    document.addEventListener("DOMContentLoaded", function () {
        // Dosya seçilir seçilmez kontrol et
        document.querySelectorAll("input[type=file][data-dosya-kontrol]").forEach(function (input) {
            input.addEventListener("change", function () { dosyaGecerliMi(input); });
        });

        // Tarih ve gün alanları değişince uyarıyı güncelle
        document.querySelectorAll("form").forEach(function (form) {
            form.querySelectorAll("[data-tarih-baslangic], [data-tarih-bitis]").forEach(function (alan) {
                alan.addEventListener("change", function () { tarihlerGecerliMi(form); });
            });
            form.querySelectorAll("[data-gun-grubu] input[type=checkbox]").forEach(function (kutu) {
                kutu.addEventListener("change", function () { gunlerGecerliMi(form); });
            });

            // Gönderirken hepsini bir kez daha kontrol et; biri bile hatalıysa gönderme
            form.addEventListener("submit", function (olay) {
                var gecerli = true;
                form.querySelectorAll("input[type=file][data-dosya-kontrol]").forEach(function (input) {
                    if (!dosyaGecerliMi(input)) gecerli = false;
                });
                if (!tarihlerGecerliMi(form)) gecerli = false;
                if (!gunlerGecerliMi(form)) gecerli = false;

                if (!gecerli) {
                    olay.preventDefault();
                    var ilkHata = form.querySelector(".is-invalid");
                    if (ilkHata) ilkHata.focus();
                    return;
                }

                // Büyük dosya yüklenirken butona tekrar basılıp aynı belge iki kez kaydedilmesin
                if (form.hasAttribute("data-tek-gonderim")) {
                    var buton = form.querySelector("button[type=submit]");
                    if (buton) {
                        buton.disabled = true;
                        buton.textContent = "Gönderiliyor…";
                    }
                }
            });
        });
    });
})();
