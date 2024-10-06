// PdfProcessing/PdfProcessor.cs
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas;
using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Utilities;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Xobject;

namespace PdfProcessing
{
    public static class PdfProcessor
    {
        // Minimum boyutları tanımlayın
        private const float MIN_WIDTH = 40.0f;  // Minimum genişlik
        private const float MIN_HEIGHT = 40.0f; // Minimum yükseklik

        public static void ProcessPdf()
        {
            Console.Write("Lütfen PDF dosyasının yolunu girin: ");
            string pdfPath = Console.ReadLine().Replace("\"", "").Replace("'", "");

            if (!File.Exists(pdfPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("PDF dosyası bulunamadı.\n");
                Console.ResetColor();
                return;
            }

            Console.Write("Lütfen çıktı dizinini girin: ");
            string outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");
            outputDirectory = FileHelper.CheckAndHandleExistingPath(outputDirectory);

            // Çıkış dizinlerini oluştur
            string pdfsDirectory = System.IO.Path.Combine(outputDirectory, "pdfs");
            string imagesDirectory = System.IO.Path.Combine(outputDirectory, "images");
            string textDirectory = System.IO.Path.Combine(outputDirectory, "text");
            string questionsDirectory = System.IO.Path.Combine(outputDirectory, "questions");

            Directory.CreateDirectory(pdfsDirectory);
            Directory.CreateDirectory(imagesDirectory);
            Directory.CreateDirectory(textDirectory);
            Directory.CreateDirectory(questionsDirectory);

            // PDF'ten soru bölgelerini çıkar
            List<QuestionRegion> questionRegions = PdfQuestionExtractor.ExtractQuestionRegions(pdfPath);

            if (questionRegions.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Hiç soru bulunamadı.\n");
                Console.ResetColor();
            }
            else
            {
                // Soruları kırp ve ayrı PDF'ler olarak kaydet
                int savedQuestions = PdfCropper.CropQuestions(pdfPath, questionsDirectory, questionRegions);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Toplam {questionRegions.Count} soru bulundu.");
                Console.WriteLine($"{savedQuestions} soru başarıyla '{questionsDirectory}' klasörüne kaydedildi.\n");
                Console.ResetColor();
            }

            // PDF'ten görselleri çıkarma işlemi
            ExtractImagesFromPdf(pdfPath, pdfsDirectory, imagesDirectory);

            // PDF'ten metin çıkarma işlemi
            ExtractTextFromPdf(pdfPath, textDirectory);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("İşlem tamamlandı. İlgili dosyalara çıktı dizini üzerinden ulaşabilirsiniz.\n");
            Console.ResetColor();
        }

        private static void ExtractImagesFromPdf(string inputPdf, string pdfsDirectory, string imagesDirectory)
        {
            using (PdfReader pdfReader = new PdfReader(inputPdf))
            using (iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader))
            {
                int totalPdfs = 0;

                // Her sayfadaki görsellerin koordinatlarını al
                for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                {
                    var strategy = new MyImageRenderListener(page);
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                    parser.ProcessPageContent(pdfDoc.GetPage(page));

                    // Sayfa başına bilgilendirme mesajı
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Sayfa {page} işleniyor...");
                    Console.ResetColor();

                    // Her bir görsel için yeni bir PDF dosyası oluştur ve kaydet
                    int imageCount = 1;
                    foreach (var imageCoordinates in strategy.ImageCoordinates)
                    {
                        // Görselin boyutlarını al
                        float cropWidth = imageCoordinates.GetWidth();
                        float cropHeight = imageCoordinates.GetHeight();

                        // Geçersiz veya çok küçük boyutları kontrol et
                        if (cropWidth <= MIN_WIDTH || cropHeight <= MIN_HEIGHT)
                        {
                            // Çok küçük veya geçersiz görselleri atla
                            continue;
                        }

                        // Zaman damgası oluştur
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

                        // Dosya adını oluştur
                        string outputPdf = System.IO.Path.Combine(pdfsDirectory, $"page{page}_image{imageCount}_{timestamp}.pdf");
                        using (PdfWriter pdfWriter = new PdfWriter(outputPdf))
                        using (iText.Kernel.Pdf.PdfDocument newPdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfWriter))
                        {
                            try
                            {
                                // Kırpılacak alanın boyutlarına göre yeni sayfa boyutunu belirle
                                PageSize newPageSize = new PageSize(cropWidth, cropHeight);

                                // Yeni bir sayfa oluştur
                                PdfPage newPage = newPdfDoc.AddNewPage(newPageSize);

                                // Kırpılacak alanı belirle
                                Rectangle cropArea = new Rectangle(0, 0, cropWidth, cropHeight);

                                // Yeni sayfaya kırpılan görseli ekle
                                PdfCanvas canvas = new PdfCanvas(newPage);
                                canvas.Rectangle(cropArea);
                                canvas.Clip();
                                canvas.EndPath();

                                // Orijinal sayfadan içeriği al ve yeni sayfaya uygula
                                PdfFormXObject pageCopy = pdfDoc.GetPage(page).CopyAsFormXObject(newPdfDoc);

                                // Görseli yeni sayfaya eklerken (-x, -y) koordinatlarını kullanıyoruz
                                float x = imageCoordinates.GetX();
                                float y = imageCoordinates.GetY();

                                canvas.AddXObjectAt(pageCopy, -x, -y);

                                // Toplam PDF sayısını artır
                                totalPdfs++;
                                imageCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"Sayfa {page}, Görsel {imageCount} için PDF kaydedilirken hata oluştu: {ex.Message}");
                                Console.ResetColor();
                            }
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Toplam {totalPdfs} görsel PDF olarak kaydedildi.");
                Console.ResetColor();
            }

            // PDF'den görselleri çıkarıp resim olarak kaydetmek için
            ExtractImages(pdfsDirectory, imagesDirectory);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Toplam {GetImageCount(imagesDirectory)} görsel resim olarak kaydedildi.\n");
            Console.ResetColor();
        }

        private static void ExtractImages(string pdfsDirectory, string imagesDirectory)
        {
            if (!Directory.Exists(pdfsDirectory))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"PDF dizini bulunamadı: {pdfsDirectory}");
                Console.ResetColor();
                return;
            }

            string[] pdfFiles = Directory.GetFiles(pdfsDirectory, "*.pdf");
            int totalImages = 0;

            foreach (var pdfFile in pdfFiles)
            {
                try
                {
                    using (var document = PdfiumViewer.PdfDocument.Load(pdfFile))
                    {
                        // Her PDF dosyası için sadece bir sayfa olduğunu varsayıyoruz
                        for (int page = 0; page < document.PageCount; page++)
                        {
                            // PDF sayfasını resim olarak render et
                            using (var image = document.Render(page, 300, 300, true))
                            {
                                // Orijinal PDF dosyasının adını al
                                string pdfFileName = System.IO.Path.GetFileNameWithoutExtension(pdfFile);
                                // Yeni resim dosyası adı oluştur
                                string imagePath = System.IO.Path.Combine(imagesDirectory, $"{pdfFileName}_image{page + 1}.png");

                                // Görseli kaydet
                                image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                                totalImages++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"PDF dosyası {pdfFile} işlenirken hata oluştu: {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Toplam {totalImages} görsel resim olarak kaydedildi.");
            Console.ResetColor();
        }

        private static int GetImageCount(string imagesDirectory)
        {
            if (!Directory.Exists(imagesDirectory))
                return 0;

            string[] files = Directory.GetFiles(imagesDirectory);
            return files.Length;
        }

        private static void ExtractTextFromPdf(string pdfPath, string textDirectory)
        {
            try
            {
                using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
                {
                    for (int i = 0; i < document.PageCount; i++)
                    {
                        try
                        {
                            // Her sayfanın metnini çıkartıyoruz
                            string pageText = document.GetPdfText(i);

                            // Eğer sayfa metni boş değilse, işlemi gerçekleştir
                            if (!string.IsNullOrEmpty(pageText))
                            {
                                // Soruları numara desenine göre bölüyoruz
                                string[] questions = Regex.Split(pageText, @"(?=\d+\.)");

                                // Soruların sonuna iki satır boşluk ekleyelim
                                string formattedText = "";
                                foreach (var question in questions)
                                {
                                    formattedText += question.Trim() + "\n\n";
                                }
                                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                                // Sayfa için metni .txt dosyasına kaydediyoruz
                                string textFilePath = System.IO.Path.Combine(textDirectory, $"page_{i + 1}_{timestamp}.txt");
                                File.WriteAllText(textFilePath, formattedText);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Sayfa {i + 1} metni {textFilePath} dosyasına kaydedildi.");
                                Console.ResetColor();
                            }
                        }
                        catch (Exception pageEx)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Sayfa {i + 1} metni çıkartılamadı: {pageEx.Message}");
                            Console.ResetColor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
