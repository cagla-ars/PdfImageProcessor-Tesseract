//using iText.Kernel.Pdf;
//using iText.Kernel.Pdf.Canvas.Parser;
//using iText.Kernel.Pdf.Canvas.Parser.Listener;
//using iText.Kernel.Geom;  // Matrix sınıfını buradan alıyoruz
//using System;
//using iText.Kernel.Pdf.Canvas.Parser.Data;

//class Program
//{
//    static void Main(string[] args)
//    {
//        string pdfPath = @"C:\Users\COZUM\Desktop\1.pdf";  // PDF dosyasının yolu

//        using (PdfReader pdfReader = new PdfReader(pdfPath))
//        using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
//        {
//            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
//            {
//                var strategy = new MyImageRenderListener(page);
//                PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
//                parser.ProcessPageContent(pdfDoc.GetPage(page));
//            }
//        }
//    }
//}

//public class MyImageRenderListener : IEventListener
//{
//    private int pageNumber;
//    private int imageIndex = 1;

//    public MyImageRenderListener(int pageNumber)
//    {
//        this.pageNumber = pageNumber;
//    }

//    public void EventOccurred(IEventData data, EventType type)
//    {
//        if (type == EventType.RENDER_IMAGE)
//        {
//            var renderInfo = (ImageRenderInfo)data;

//            // Görselin konumunu ekrana yazdır
//            var rect = renderInfo.GetImageCtm();
//            Console.WriteLine($"Sayfa: {pageNumber}, Görsel {imageIndex}: Konum - X: {rect.Get(Matrix.I31)}, Y: {rect.Get(Matrix.I32)}, Genişlik: {rect.Get(Matrix.I11)}, Yükseklik: {rect.Get(Matrix.I22)}");

//            imageIndex++;
//        }
//    }

//    public ICollection<EventType> GetSupportedEvents()
//    {
//        return new List<EventType> { EventType.RENDER_IMAGE };
//    }
//}


/*using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Geom;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string inputPdf = @"C:\Users\COZUM\Desktop\1.pdf";  // Giriş PDF dosyası
        string outputPdf = @"C:\Users\COZUM\WithIronOcr\WithIronOcr\pdf\output.pdf";  // Çıkış PDF dosyası

        using (PdfReader pdfReader = new PdfReader(inputPdf))
        using (PdfWriter pdfWriter = new PdfWriter(outputPdf))
        using (PdfDocument pdfDoc = new PdfDocument(pdfReader, pdfWriter))
        {
            // 1. sayfada kırpılacak alanın koordinatlarını belirle (X, Y, genişlik, yükseklik)
            // Koordinatları elle girebilirsin
            Rectangle cropArea = new Rectangle(382.20245f, 555.04803f, 148.34402f, 142.34402f);

            // PDF'deki sayfayı yükle
            PdfPage page = pdfDoc.GetPage(1);

            // Sayfanın kırpılmasını sağlayan fonksiyon
            page.SetCropBox(cropArea);
            pdfDoc.Close();
        }

        Console.WriteLine("Belirtilen alan başarıyla kırpıldı ve yeni PDF'ye kaydedildi.");
    }
}

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Geom;
using System;
using System.IO;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;

class Program
{
    static void Main(string[] args)
    {
        string inputPdf = @"C:\Users\COZUM\Desktop\2.pdf";  // Giriş PDF dosyası
        string outputDirectory = @"C:\Users\COZUM\WithIronOcr\WithIronOcr\pdf\";  // Çıkış dosyası dizini

        using (PdfReader pdfReader = new PdfReader(inputPdf))
        using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
        {
            // Her sayfadaki görsellerin koordinatlarını al
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                var strategy = new MyImageRenderListener(page);
                PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                parser.ProcessPageContent(pdfDoc.GetPage(page));

                // Her bir görsel için yeni bir PDF sayfası oluştur ve kaydet
                int imageCount = 1;
                foreach (var imageCoordinates in strategy.ImageCoordinates)
                {
                    // Yeni PDF dosyası oluştur
                    string outputPdf = System.IO.Path.Combine(outputDirectory, $"output_page_{page}_image_{imageCount}.pdf");
                    using (PdfWriter pdfWriter = new PdfWriter(outputPdf))
                    using (PdfDocument newPdfDoc = new PdfDocument(pdfWriter))
                    {
                        // Yeni bir sayfa oluştur
                        PdfPage newPage = newPdfDoc.AddNewPage(PageSize.A4);  // Sayfa boyutunu buradan ayarlayabilirsiniz

                        // Kırpılacak alanı belirle
                        Rectangle cropArea = new Rectangle(imageCoordinates.GetX(), imageCoordinates.GetY(),
                                                           imageCoordinates.GetWidth(), imageCoordinates.GetHeight());

                        // Yeni sayfaya kırpılan görseli ekle
                        PdfCanvas canvas = new PdfCanvas(newPage);
                        canvas.Rectangle(cropArea);
                        canvas.Clip();
                        canvas.EndPath();

                        // Orijinal sayfadan içeriği al ve yeni sayfaya uygula
                        PdfFormXObject pageCopy = pdfDoc.GetPage(page).CopyAsFormXObject(newPdfDoc);
                        canvas.AddXObjectAt(pageCopy, 0, 0);

                        Console.WriteLine($"Yeni PDF kaydedildi: {outputPdf}");
                    }
                    imageCount++;
                }
            }
        }
    }
}

// Görsel konumlarını almak için dinleyici sınıfı
public class MyImageRenderListener : IEventListener
{
    private int pageNumber;
    private int imageIndex = 1;
    public List<Rectangle> ImageCoordinates { get; private set; } = new List<Rectangle>();

    public MyImageRenderListener(int pageNumber)
    {
        this.pageNumber = pageNumber;
    }

    public void EventOccurred(IEventData data, EventType type)
    {
        if (type == EventType.RENDER_IMAGE)
        {
            var renderInfo = (ImageRenderInfo)data;

            // Görselin konumunu al
            var rect = renderInfo.GetImageCtm();
            float x = rect.Get(Matrix.I31);
            float y = rect.Get(Matrix.I32);
            float width = rect.Get(Matrix.I11);
            float height = rect.Get(Matrix.I22);

            // Koordinatları listede sakla
            ImageCoordinates.Add(new Rectangle(x, y, width, height));

            Console.WriteLine($"Sayfa: {pageNumber}, Görsel {imageIndex}: X: {x}, Y: {y}, Genişlik: {width}, Yükseklik: {height}");
            imageIndex++;
        }
    }

    public ICollection<EventType> GetSupportedEvents()
    {
        return new List<EventType> { EventType.RENDER_IMAGE };
    }
}
*/


//koordinatları alma

/*using System;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Lütfen PDF dosyasının yolunu girin:");
        string pdfPath = Console.ReadLine().Replace("\"", "").Replace("'", "");

        if (File.Exists(pdfPath))
        {
            Console.WriteLine("Lütfen çıktı dizinini veya tam .txt dosya yolunu girin:");
            string outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");
            outputDirectory = CheckAndHandleExistingPath(outputDirectory);

            ExtractTextWithCoordinates(pdfPath, outputDirectory);
        }
        else
        {
            Console.WriteLine("PDF dosyası bulunamadı.");
        }
    }

    static void ExtractTextWithCoordinates(string pdfPath, string outputDirectory)
    {
        try
        {
            using (PdfReader pdfReader = new PdfReader(pdfPath))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                for (int pageIndex = 1; pageIndex <= pdfDocument.GetNumberOfPages(); pageIndex++)
                {
                    var strategy = new MyLocationTextExtractionStrategy();
                    PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageIndex), strategy);

                    // Soruları ve koordinatları bulmak için regex deseni
                    Regex questionRegex = new Regex(@"(\d{1,2}\.)\s", RegexOptions.Compiled);

                    Console.WriteLine($"Sayfa {pageIndex} için soruların koordinatları:");

                    foreach (var textLocation in strategy.TextLocations)
                    {
                        // Soruları tespit etmek için regex kullanıyoruz
                        if (questionRegex.IsMatch(textLocation.Text))
                        {
                            Console.WriteLine($"Soru: {textLocation.Text}");
                            Console.WriteLine($"Koordinatlar - x: {textLocation.Rectangle.GetLeft()}, y: {textLocation.Rectangle.GetBottom()}, genişlik: {textLocation.Rectangle.GetWidth()}, yükseklik: {textLocation.Rectangle.GetHeight()}");
                        }
                    }

                    // Çıktıyı dosyaya kaydetmek
                    string outputPath = System.IO.Path.Combine(outputDirectory, $"page_{pageIndex}_questions.txt");
                    File.WriteAllText(outputPath, string.Join("\n", strategy.TextLocations));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }
    }

    static string CheckAndHandleExistingPath(string outputDirectory)
    {
        if (System.IO.Path.HasExtension(outputDirectory))
        {
            if (File.Exists(outputDirectory))
            {
                Console.WriteLine($"Aynı isimde bir dosya mevcut: {outputDirectory}");
                Console.WriteLine("Dosya zaten mevcut. Değişiklik yapmak istiyor musunuz? (y/n)");
                string userInput = Console.ReadLine().ToLower();

                if (userInput == "y")
                {
                    return outputDirectory;
                }
                else
                {
                    Console.WriteLine("Lütfen yeni bir dosya yolu girin:");
                    outputDirectory = Console.ReadLine();
                }
            }
        }
        else if (!Directory.Exists(outputDirectory))
        {
            Console.WriteLine("Girilen dizin mevcut değil. Dosyanızı masaüstüne kaydetmek ister misiniz? (y/n)");
            string userInput = Console.ReadLine().ToLower();
            if (userInput == "y")
            {
                outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                Console.WriteLine("Lütfen geçerli bir dizin girin:");
                outputDirectory = Console.ReadLine();
            }
        }

        return outputDirectory;
    }
}

// Metin ve koordinat bilgilerini saklamak için kullanılan sınıf
class TextLocation
{
    public string Text { get; set; }
    public Rectangle Rectangle { get; set; }

    public TextLocation(string text, Rectangle rectangle)
    {
        Text = text;
        Rectangle = rectangle;
    }

    public override string ToString()
    {
        return $"{Text} - Koordinatlar: ({Rectangle.GetLeft()}, {Rectangle.GetBottom()}, genişlik: {Rectangle.GetWidth()}, yükseklik: {Rectangle.GetHeight()})";
    }
}

// Metin çıkarımı sırasında metinlerin koordinatlarını saklayan strateji
class MyLocationTextExtractionStrategy : LocationTextExtractionStrategy
{
    public List<TextLocation> TextLocations { get; } = new List<TextLocation>();

    public override void EventOccurred(IEventData data, EventType type)
    {
        if (type == EventType.RENDER_TEXT)
        {
            var renderInfo = (TextRenderInfo)data;

            // Metnin üst ve alt çizgilerini alarak dikdörtgen alanı genişletiyoruz
            var ascentLine = renderInfo.GetAscentLine();  // Metnin üst sınırı
            var descentLine = renderInfo.GetDescentLine(); // Metnin alt sınırı

            // Tam bounding box'ı oluşturmak için ascent ve descent çizgilerinin birleşimi
            var boundingBox = new Rectangle(
                Math.Min(ascentLine.GetStartPoint().Get(Vector.I1), descentLine.GetStartPoint().Get(Vector.I1)),  // Sol kenar (X)
Math.Min(ascentLine.GetEndPoint().Get(Vector.I2), descentLine.GetEndPoint().Get(Vector.I2)),      // Alt kenar (Y)
Math.Abs(ascentLine.GetEndPoint().Get(Vector.I1) - descentLine.GetStartPoint().Get(Vector.I1)),   // Genişlik (Width)
Math.Abs(ascentLine.GetStartPoint().Get(Vector.I2) - descentLine.GetEndPoint().Get(Vector.I2))    // Yükseklik (Height)
            );

            var text = renderInfo.GetText();
            TextLocations.Add(new TextLocation(text, boundingBox));
        }
        base.EventOccurred(data, type);
    }

    public override string GetResultantText()
    {
        return string.Join("\n", TextLocations.Select(tl => $"{tl.Text} - Koordinatlar: ({tl.Rectangle.GetLeft()}, {tl.Rectangle.GetBottom()}, genişlik: {tl.Rectangle.GetWidth()}, yükseklik: {tl.Rectangle.GetHeight()})"));
    }
}

*/

//örnek kırpma
/*
using System;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf.Canvas;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser.Data;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Lütfen PDF dosyasının yolunu girin:");
        string pdfPath = Console.ReadLine().Replace("\"", "").Replace("'", "");

        if (File.Exists(pdfPath))
        {
            Console.WriteLine("Lütfen çıktı dizinini girin:");
            string outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");
            outputDirectory = CheckAndHandleExistingPath(outputDirectory);

            ExtractTextWithCoordinatesAndSaveInOnePDF(pdfPath, outputDirectory);
        }
        else
        {
            Console.WriteLine("PDF dosyası bulunamadı.");
        }
    }

    static void ExtractTextWithCoordinatesAndSaveInOnePDF(string pdfPath, string outputDirectory)
    {
        string outputPath = System.IO.Path.Combine(outputDirectory, "questions_output.pdf");

        try
        {
            using (PdfReader pdfReader = new PdfReader(pdfPath))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            using (PdfWriter pdfWriter = new PdfWriter(outputPath))
            using (PdfDocument outputPdfDocument = new PdfDocument(pdfWriter))
            {
                int questionCount = 1;

                for (int pageIndex = 1; pageIndex <= pdfDocument.GetNumberOfPages(); pageIndex++)
                {
                    var strategy = new MyLocationTextExtractionStrategy();
                    PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageIndex), strategy);

                    // Soruları ve koordinatları bulmak için regex deseni
                    Regex questionRegex = new Regex(@"(\d{1,2}\.)\s", RegexOptions.Compiled);

                    Console.WriteLine($"Sayfa {pageIndex} için metin ve koordinat bilgileri:");

                    // Sayfada tespit edilen metin alanlarının en üst ve en alt noktalarını buluyoruz
                    float top = float.MinValue;
                    float bottom = float.MaxValue;
                    float left = float.MaxValue;
                    float right = float.MinValue;

                    foreach (var textLocation in strategy.TextLocations)
                    {
                        // Her metnin koordinatlarını ve içeriğini yazdır
                        Console.WriteLine($"Metin: {textLocation.Text}");
                        Console.WriteLine($"Koordinatlar - x: {textLocation.Rectangle.GetLeft()}, y: {textLocation.Rectangle.GetBottom()}, genişlik: {textLocation.Rectangle.GetWidth()}, yükseklik: {textLocation.Rectangle.GetHeight()}");

                        // Soru tespit etme
                        if (questionRegex.IsMatch(textLocation.Text))
                        {
                            // Kesme işlemi için alanı belirleme
                            top = Math.Max(top, textLocation.Rectangle.GetTop());
                            bottom = Math.Min(bottom, textLocation.Rectangle.GetBottom());
                            left = Math.Min(left, textLocation.Rectangle.GetLeft());
                            right = Math.Max(right, textLocation.Rectangle.GetRight());

                            Console.WriteLine($"Soru tespit edildi: {textLocation.Text}");
                        }
                    }

                    // Eğer kesilecek bir alan bulunduysa (toplam metin alanı)
                    if (top != float.MinValue && bottom != float.MaxValue && left != float.MaxValue && right != float.MinValue)
                    {
                        var cropArea = new Rectangle(left, bottom, right - left, top - bottom);
                        CropAndAddToPDF(pdfDocument, pageIndex, cropArea, outputPdfDocument);
                        questionCount++;
                    }
                }
            }

            Console.WriteLine($"Sorular {outputPath} dosyasına kaydedildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }
    }

    static void CropAndAddToPDF(PdfDocument originalDocument, int pageIndex, Rectangle cropArea, PdfDocument outputPdfDocument)
    {
        PdfPage originalPage = originalDocument.GetPage(pageIndex);
        PdfFormXObject pageCopy = originalPage.CopyAsFormXObject(outputPdfDocument);
        PdfCanvas canvas = new PdfCanvas(outputPdfDocument.AddNewPage());

        // Kesilecek alanı belirliyoruz (crop)
        canvas.Rectangle(cropArea.GetLeft(), cropArea.GetBottom(), cropArea.GetWidth(), cropArea.GetHeight());
        canvas.Clip();

        // 'NewPath' yerine, direk Clip sonrası yeni bir path başlatmadan devam ediyoruz
        // Bu işlemde sadece kesilen bölgenin çizimi yapılıyor

        // Kesilmiş içeriği yeni sayfaya çiz
        canvas.AddXObjectAt(pageCopy, -cropArea.GetLeft(), -cropArea.GetBottom());
    }


    static string CheckAndHandleExistingPath(string outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        return outputDirectory;
    }
}

// Metin ve koordinat bilgilerini saklamak için kullanılan sınıf
class TextLocation
{
    public string Text { get; set; }
    public Rectangle Rectangle { get; set; }

    public TextLocation(string text, Rectangle rectangle)
    {
        Text = text;
        Rectangle = rectangle;
    }
}

// Metin çıkarımı sırasında metinlerin koordinatlarını saklayan strateji
class MyLocationTextExtractionStrategy : LocationTextExtractionStrategy
{
    public List<TextLocation> TextLocations { get; } = new List<TextLocation>();

    public override void EventOccurred(IEventData data, EventType type)
    {
        if (type == EventType.RENDER_TEXT)
        {
            var renderInfo = (TextRenderInfo)data;

            // Metnin üst ve alt çizgilerini alarak dikdörtgen alanı genişletiyoruz
            var ascentLine = renderInfo.GetAscentLine();  // Metnin üst sınırı
            var descentLine = renderInfo.GetDescentLine(); // Metnin alt sınırı

            // Tam bounding box'ı oluşturmak için ascent ve descent çizgilerinin birleşimi
            var boundingBox = new Rectangle(
                Math.Min(ascentLine.GetStartPoint().Get(Vector.I1), descentLine.GetStartPoint().Get(Vector.I1)),  // Sol kenar (X)
                Math.Min(ascentLine.GetEndPoint().Get(Vector.I2), descentLine.GetEndPoint().Get(Vector.I2)),      // Alt kenar (Y)
                Math.Abs(ascentLine.GetEndPoint().Get(Vector.I1) - descentLine.GetStartPoint().Get(Vector.I1)),   // Genişlik (Width)
                Math.Abs(ascentLine.GetStartPoint().Get(Vector.I2) - descentLine.GetEndPoint().Get(Vector.I2))    // Yükseklik (Height)
            );

            var text = renderInfo.GetText();
            TextLocations.Add(new TextLocation(text, boundingBox));
        }
        base.EventOccurred(data, type);
    }
}
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;

namespace PdfQuestionCropper
{
    // Soru bölgelerini temsil eden sınıf
    public class QuestionRegion
    {
        public int PageNumber { get; set; }
        public Rectangle BoundingBox { get; set; }
    }

    // Metin işleme olaylarını dinleyen sınıf
    public class QuestionRegionEventListener : IEventListener
    {
        public List<QuestionRegion> QuestionRegions { get; } = new List<QuestionRegion>();
        private readonly Regex questionStartRegex = new Regex(@"^\d+\.");
        private readonly Regex questionEndRegex = new Regex(@"E\)$", RegexOptions.Multiline);

        // Koordinatları doğrudan saklamak için değişkenler
        private float minX = float.MaxValue;
        private float minY = float.MaxValue;
        private float maxX = float.MinValue;
        private float maxY = float.MinValue;

        private bool isInQuestion = false;
        private int currentPage;

        // Mevcut sayfa numarasını ayarlayan metod
        public void SetCurrentPage(int pageNumber)
        {
            currentPage = pageNumber;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type != EventType.RENDER_TEXT)
                return;

            TextRenderInfo renderInfo = (TextRenderInfo)data;
            string text = renderInfo.GetText();

            if (string.IsNullOrWhiteSpace(text))
                return;

            if (questionStartRegex.IsMatch(text.Trim()))
            {
                if (isInQuestion)
                {
                    // Önceki soruyu kaydet
                    SaveCurrentQuestion();
                }

                // Yeni soru başlıyor
                isInQuestion = true;
                ResetBoundingBox();
                UpdateBoundingBox(renderInfo);
            }
            else if (isInQuestion)
            {
                UpdateBoundingBox(renderInfo);

                if (questionEndRegex.IsMatch(text.Trim()))
                {
                    // Soru bitiyor
                    SaveCurrentQuestion();
                    isInQuestion = false;
                }
            }
        }

        private void ResetBoundingBox()
        {
            minX = float.MaxValue;
            minY = float.MaxValue;
            maxX = float.MinValue;
            maxY = float.MinValue;
        }

        private void UpdateBoundingBox(TextRenderInfo renderInfo)
        {
            try
            {
                var descentLine = renderInfo.GetDescentLine();
                var ascentLine = renderInfo.GetAscentLine();

                minX = Math.Min(minX, descentLine.GetStartPoint().Get(0));
                minY = Math.Min(minY, descentLine.GetStartPoint().Get(1));
                maxX = Math.Max(maxX, ascentLine.GetEndPoint().Get(0));
                maxY = Math.Max(maxY, ascentLine.GetEndPoint().Get(1));
            }
            catch (Exception ex)
            {
                // Loglama yapabilir veya başka bir işlem yapabilirsiniz
                Console.WriteLine($"Bounding box güncellenirken hata: {ex.Message}");
            }
        }

        private void SaveCurrentQuestion()
        {
            if (minX == float.MaxValue || minY == float.MaxValue || maxX == float.MinValue || maxY == float.MinValue)
                return;

            // Bounding box'u genişlik ve yükseklikle birlikte saklayın
            Rectangle rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            QuestionRegions.Add(new QuestionRegion
            {
                PageNumber = currentPage,
                BoundingBox = rect
            });
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return new HashSet<EventType> { EventType.RENDER_TEXT };
        }
    }

    // PDF'den soru bölgelerini çıkaran sınıf
    public class PdfQuestionExtractor
    {
        public static List<QuestionRegion> ExtractQuestionRegions(string pdfPath)
        {
            List<QuestionRegion> questionRegions = new List<QuestionRegion>();

            using (PdfReader reader = new PdfReader(pdfPath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                int numPages = pdfDoc.GetNumberOfPages();

                for (int page = 1; page <= numPages; page++)
                {
                    QuestionRegionEventListener listener = new QuestionRegionEventListener();
                    listener.SetCurrentPage(page); // Mevcut sayfa numarasını ayarla

                    PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
                    processor.ProcessPageContent(pdfDoc.GetPage(page));

                    foreach (var qr in listener.QuestionRegions)
                    {
                        questionRegions.Add(qr);
                    }
                }
            }

            return questionRegions;
        }
    }

    // PDF'yi kırpan sınıf
    public class PdfCropper
    {
        public static void CropQuestions(string inputPdfPath, string outputFolder, List<QuestionRegion> questionRegions)
        {
            using (PdfReader reader = new PdfReader(inputPdfPath))
            using (PdfDocument inputPdf = new PdfDocument(reader))
            {
                int questionNumber = 1;

                foreach (var qr in questionRegions)
                {
                    string outputPdfPath = System.IO.Path.Combine(outputFolder, $"question_{questionNumber}.pdf");

                    using (PdfWriter writer = new PdfWriter(outputPdfPath))
                    using (PdfDocument outputPdf = new PdfDocument(writer))
                    {
                        // Sayfa boyutunu sınır kutusuna göre ayarla
                        Rectangle cropBox = qr.BoundingBox;

                        // PageSize oluşturmak için sadece genişlik ve yükseklik kullan
                        PageSize pageSize = new PageSize(cropBox.GetWidth(), cropBox.GetHeight());

                        outputPdf.SetDefaultPageSize(pageSize);

                        // Yeni sayfa ekle
                        PdfPage newPage = outputPdf.AddNewPage(pageSize);

                        // Kaynak sayfayı al
                        PdfPage sourcePage = inputPdf.GetPage(qr.PageNumber);

                        // Sayfa içeriğini XObject olarak kopyala
                        PdfFormXObject pageCopy = sourcePage.CopyAsFormXObject(outputPdf);

                        // XObject'i yeni sayfanın canvas'ına ekle ve kırp
                        PdfCanvas canvas = new PdfCanvas(newPage);
                        canvas.AddXObjectAt(pageCopy, -cropBox.GetX(), -cropBox.GetY());

                        // Çıkış PDF'sini kapat
                        outputPdf.Close();
                    }

                    Console.WriteLine($"Soru {questionNumber} '{outputPdfPath}' olarak kırpıldı.");
                    questionNumber++;
                }
            }
        }
    }

    // Ana Program
    class Program
{
    static void Main(string[] args)
    {
        string inputPdfPath = @"C:\Users\COZUM\Desktop\3.pdf"; // Girdi PDF yolu
        string outputFolder = @"C:\Users\COZUM\WithIronOcr\WithIronOcr\pdf\";    // Çıktı klasörü

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            try
            {
                // PDF'den soru bölgelerini çıkar
                List<QuestionRegion> questionRegions = PdfQuestionExtractor.ExtractQuestionRegions(inputPdfPath);

                Console.WriteLine($"Toplam {questionRegions.Count} soru bulundu.");

                // Soruları kırp ve ayrı PDF'ler olarak kaydet
                PdfCropper.CropQuestions(inputPdfPath, outputFolder, questionRegions);

                Console.WriteLine("Tüm sorular kırpıldı ve ayrı PDF'ler oluşturuldu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }

            // Programın hemen kapanmaması için bekle
            Console.WriteLine("Bu pencereyi kapatmak için bir tuşa basın...");
            Console.ReadKey();
        }
    }
}