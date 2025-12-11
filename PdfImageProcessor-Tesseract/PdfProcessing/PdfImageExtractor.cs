// PdfProcessing/PdfImageExtractor.cs
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

namespace PdfProcessing
{
    public static class PdfImageExtractor
    {
        // Minimum boyutları tanımlayın
        const float MIN_WIDTH = 40.0f;  // Minimum genişlik
        const float MIN_HEIGHT = 40.0f; // Minimum yükseklik

        public static void ExtractImagesFromPdf(string inputPdf, string pdfsDirectory, string imagesDirectory)
        {
            using (PdfReader pdfReader = new PdfReader(inputPdf))
            using (iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader))
            {
                int totalPdfs = 0;

                // Her sayfadaki görsellerin koordinatlarını al
                for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                {
                    var strategy = new PdfProcessing.MyImageRenderListener(page);
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(strategy);
                    parser.ProcessPageContent(pdfDoc.GetPage(page));

                    // Sayfa başına bilgilendirme mesajı
                    Console.WriteLine($"Sayfa {page} işleniyor...");

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
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Sayfa {page}, Görsel {imageCount} için PDF kaydedilirken hata oluştu: {ex.Message}");
                                Console.ResetColor();
                            }
                        }
                    }
                }

                Console.WriteLine($"Toplam {totalPdfs} görsel PDF olarak kaydedildi.");
            }

            // PDF'den görselleri çıkarıp resim olarak kaydetmek için
            ExtractImages(pdfsDirectory, imagesDirectory);

        }

        static void ExtractImages(string pdfsDirectory, string imagesDirectory)
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
                                string imagePath = System.IO.Path.Combine(imagesDirectory, $"{pdfFileName}.png");

                                // Görseli kaydet
                                image.Save(imagePath, ImageFormat.Png);
                                totalImages++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"PDF dosyası {pdfFile} işlenirken hata oluştu: {ex.Message}");
                    Console.ResetColor();
                }
            }

        }

        static int GetImageCount(string imagesDirectory)
        {
            if (!Directory.Exists(imagesDirectory))
                return 0;

            string[] files = Directory.GetFiles(imagesDirectory);
            return files.Length;
        }
    }
}