PdfImageProcessor
<!-- Projenizin logosunu varsa buraya ekleyebilirsiniz -->

İçindekiler
Genel Bakış
Özellikler
Gereksinimler
Kurulum
Kullanım
Proje Yapısı
Katkıda Bulunma
Lisans
İletişim
Genel Bakış
PdfImageProcessor, PDF dosyalarını ve görselleri işlemek için tasarlanmış kapsamlı bir C# uygulamasıdır. PDF'lerden soru bölgelerini çıkarır, bu bölgeleri ayrı PDF dosyaları olarak kırpar, PDF'lerden görselleri çıkarır, görseller üzerinde perspektif düzeltme yapar ve Görüntü İşleme (OCR) kullanarak görsellerden metin çıkarır. Uygulama, kullanıcı deneyimini artırmak için renkli ve düzenli çıktılar sunan kullanıcı dostu bir konsol arayüzü ile gelir.

Özellikler
PDF İşleme:

PDF'lerden soru bölgelerini çıkarır.
Çıkarılan bölgeleri ayrı PDF dosyaları olarak kırpar.
PDF dosyalarından görselleri çıkarır.
PDF'lerden metin çıkarır ve .txt dosyaları olarak kaydeder.
Görsel İşleme:

Belirtilen bir klasörden veya bireysel olarak görselleri yükler.
Perspektif düzeltme yaparak görsel kalitesini artırır.
Tesseract OCR kullanarak görsellerden metin çıkarır.
İşlenmiş görselleri ve çıkarılan metni düzenli dizinlerde kaydeder.
Kullanıcı Dostu Konsol Arayüzü:

Daha iyi okunabilirlik için renk kodlu mesajlar.
Sorunsuz kullanıcı deneyimi için etkileşimli istemler.
Hataları anlamlı mesajlarla zarif bir şekilde yönetir.
Gereksinimler
Projeyi çalıştırmadan önce, aşağıdaki gereksinimlerin karşılandığından emin olun:

İşletim Sistemi: Windows, macOS veya Linux.
.NET SDK: .NET 6.0 SDK veya daha yeni bir sürüm.
Visual Studio 2022 veya herhangi bir C# uyumlu IDE/metin editörü.
Tesseract OCR Veri Dosyaları: tessdata klasörünün, gerekli .traineddata dosyaları (örneğin, eng.traineddata, tur.traineddata, rus.traineddata) ile birlikte proje dizininizde bulunduğundan emin olun.
