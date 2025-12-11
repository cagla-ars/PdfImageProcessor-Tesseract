# PdfImageProcessor – OCR & Belge İşleme Aracı

**PdfImageProcessor**, PDF belgelerinden veya görüntü dosyalarından (JPG, PNG) otomatik olarak metin çıkartan, **OpenCV** ve **Tesseract OCR** tabanlı bir C# konsol uygulamasıdır.

Uygulama:
- PDF sayfalarını işler ve metinleri `.txt` formatında dışa aktarır.
- Soru bloklarını tespit ederek her birini ayrı görsel olarak kaydeder.
- Görüntülerde perspektif bozulmasını algılar ve gerekiyorsa düzeltme uygular.
- Eğer perspektif problemi yoksa görüntüyü kaydetmeden yalnızca OCR uygular.

---

## İçindekiler
1. Özellikler  
2. Kullanılan Teknolojiler  
3. Proje Yapısı  
4. Kurulum  
   - Tessdata Kurulumu (Önemli)  
5. Kullanım  
   - PDF İşleme  
   - Görsel İşleme  
6. Çıktı Klasör Yapısı  
7. Perspektif Düzeltme Mantığı  
8. İpuçları ve Sınırlamalar  
9. Sonuç  

---

## 1. Özellikler
- PDF sayfalarını yüksek doğrulukla metne dönüştürme  
- Soru bloklarının otomatik algılanması ve ayrı görseller olarak kaydedilmesi  
- Görsellerde perspektif düzeltme (köşe tespiti ile)  
- Perspektif yoksa yalnızca OCR uygulanması  
- Renkli ve yönlendirici konsol arayüzü  
- Tekli veya toplu görsel işleme desteği  

---

## 2. Kullanılan Teknolojiler
- C# / .NET  
- Tesseract OCR  
- OpenCvSharp  
- Pdfium veya iText7  
- System.IO dosya yönetimi  

---

## 3. Proje Yapısı

PdfImageProcessor
│
├── PdfProcessing
│ ├── PdfProcessor.cs
│ └── QuestionExtractor.cs
│
├── ImageProcessing
│ ├── ImageProcessor.cs
│ ├── ImageHandler.cs
│ ├── PerspectiveCorrector.cs
│ └── OCRProcessor.cs
│
├── Utilities
│ ├── FileHelper.cs
│ └── PathUtils.cs
│
└── Program.cs


---

## 4. Kurulum

Projeyi klonlayın:

  -git clone <repo-link>

Projeyi Visual Studio veya Rider ile açın.

NuGet paketlerini yükleyin:

-Tesseract
-OpenCvSharp4
-OpenCvSharp4.runtime.windows


Uygulamayı çalıştırın:

  -dotnet run

---

### Tessdata Kurulumu (Önemli)

OCR işleminin çalışabilmesi için **Tesseract dil dosyalarının** (`.traineddata`) indirilmesi zorunludur.

Desteklenen diller:
- tur.traineddata (Türkçe)
- eng.traineddata (İngilizce)
- rus.traineddata (Rusça)

#### Dil dosyaları nereden indirilir?

Hızlı (daha düşük doğruluk, hızlı işleme):
https://github.com/tesseract-ocr/tessdata_fast


Yüksek doğruluk (yavaş, daha kaliteli OCR):
https://github.com/tesseract-ocr/tessdata_best

Örneğin Türkçe dosyası:
https://github.com/tesseract-ocr/tessdata_fast/blob/main/tur.traineddata

#### Dil dosyalarını projeye ekleme

İndirilen `.traineddata` dosyalarını aşağıdaki klasöre yerleştirin:

/tessdata/
tur.traineddata
eng.traineddata
rus.traineddata


#### Kod içinde kullanım

```
using (var engine = new TesseractEngine(@"./tessdata", "tur", EngineMode.Default))
{
    using var img = Pix.LoadFromFile(imagePath);
    using var page = engine.Process(img);
    var text = page.GetText();
}
Eğer tessdata klasörü exe’nin yanında değilse tam yol kullanılmalıdır:

var engine = new TesseractEngine(@"C:/projeler/PdfImageProcessor/tessdata", "eng", EngineMode.Default);
Önemli Notlar
tessdata klasörü build sonrası exe ile aynı dizinde bulunmalıdır.

## 5. Kullanım

### Ana Menü
Uygulama açıldığında şu seçenekler sunulur:

- PDF dosyası işleme  
- Görsel dosyası işleme  
- Çıkış  

---

### PDF İşleme

- PDF dosya yolu girilir.  
- Çıktı klasörü seçilir.  

Uygulama:
- Sayfa metinlerini `.txt` dosyalarına kaydeder.  
- Soru bloklarını tespit ederek `/questions` klasörüne görsel olarak aktarır.  

**Örnek çıktı:**

Toplam 5 soru bulundu.
5 soru başarıyla .../questions klasörüne kaydedildi.
Sayfa 1 metni page_1_20251211_xxxxx.txt dosyasına kaydedildi.


---

### Görsel İşleme

- Tek bir resim veya klasör seçilebilir.  
- Çıktı klasörü belirtilir.  
- OCR dili (TR/EN/RU) seçilir.  

**Örnek işlem akışı:**

İşlem başlatıldı: C:...\image.jpg
Dört köşe bulunamadı → Perspektif yok → Görsel kaydedilmeyecek.
OCR uygulanıyor...
İşlem tamamlandı → çıktı: ...\ocr_output.txt


---

## 6. Çıktı Klasör Yapısı

### PDF Modu

çıktı/
├── text/
│ └── page_1_xxx.txt
└── questions/
├── question_1.png
├── question_2.png


### Görsel Modu

çıktı/
└── ocr_output_xxx.txt

---

## 7. Perspektif Düzeltme Mantığı

- Gri tonlama  
- Gaussian blur  
- Canny edge detection  
- Kontur analizleri  
- Dört köşe tespiti  
- `WarpPerspective` ile düzeltme  

Eğer dört köşe tespit edilemezse:
- Perspektif düzeltme yapılmaz  
- Görüntü kaydedilmez  
- OCR orijinal görüntü üzerinden yapılır  

---

## 8. İpuçları ve Sınırlamalar

- Görüntü ne kadar net olursa OCR sonuçları o kadar isabetli olur.  
- Belge ile arka plan arasında yeterli kontrast olmalıdır.  
- Perspektif düzeltme için belgenin dört kenarı görünür olmalıdır.  
- PDF’lerde soru numaraları düzenli ise ayrıştırma daha başarılı olur.  

---

## 9. Sonuç

**PdfImageProcessor**, PDF ve görüntü dosyaları üzerinde çalışan, OCR, görüntü işleme ve dosya otomasyonunu bir araya getiren kapsamlı ve pratik bir araçtır.

Taranmış belgeler, test bankaları, ders notları ve metin ağırlıklı dokümanlarla çalışmak için idealdir.


Yanlış klasör adı → “language data not found” hatası.

tessdata_best daha doğrucu, tessdata_fast daha hızlıdır.
