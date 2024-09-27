/*using System;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using Tesseract;
using System.Drawing; // Bitmap ve Rectangle sınıfları için
using System.Drawing.Imaging;
using PdfiumViewer; // PdfiumViewer kütüphanesi

namespace PdfCropper
{
    class Program3
    {
        static void Main(string[] args)
        {
            string inputPdfPath = @"C:\Users\COZUM\Desktop\1.pdf";
            string outputPdfSplitPath = @"C:\Users\COZUM\WithIronOcr\WithIronOcr\pdf\output_split.pdf";
            string outputPdfCroppedPath = @"C:\Users\COZUM\WithIronOcr\WithIronOcr\pdf\output.pdf";

            // Önce PDF'yi sütunlara ayıralım
            SplitPdf(inputPdfPath, outputPdfSplitPath);

            // Ardından her sütunu OCR ile kırpalım
            CropPdf(outputPdfSplitPath, outputPdfCroppedPath);
        }

        // PDF'yi sol ve sağ sütunlara ayırma fonksiyonu
        static void SplitPdf(string inputPdfPath, string outputPdfPath)
        {
            using (PdfSharp.Pdf.PdfDocument inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import))
            using (PdfSharp.Pdf.PdfDocument outputPdf = new PdfSharp.Pdf.PdfDocument())
            {
                for (int pageIndex = 0; pageIndex < inputPdf.Pages.Count; pageIndex++)
                {
                    PdfPage page = inputPdf.Pages[pageIndex];
                    double pageWidth = page.Width.Point;
                    double pageHeight = page.Height.Point;

                    // Sol sütun
                    PdfPage leftPage = outputPdf.AddPage(page);
                    leftPage.CropBox = new PdfSharp.Pdf.PdfRectangle(new XPoint(0, 0), new XPoint(pageWidth / 2, pageHeight));

                    // Sağ sütun
                    PdfPage rightPage = outputPdf.AddPage(page);
                    rightPage.CropBox = new PdfSharp.Pdf.PdfRectangle(new XPoint(pageWidth / 2, 0), new XPoint(pageWidth, pageHeight));
                }

                outputPdf.Save(outputPdfPath);
                Console.WriteLine($"PDF başarıyla ikiye bölündü ve kaydedildi: {outputPdfPath}");
            }
        }

        // Bölünen PDF'deki sayfaları kırpma fonksiyonu
        static void CropPdf(string inputPdfPath, string outputPdfPath)
        {
            using (PdfSharp.Pdf.PdfDocument inputPdf = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Import))
            using (PdfSharp.Pdf.PdfDocument outputPdf = new PdfSharp.Pdf.PdfDocument())
            {
                for (int pageIndex = 0; pageIndex < inputPdf.Pages.Count; pageIndex++)
                {
                    PdfPage page = inputPdf.Pages[pageIndex];

                    // Sayfayı bitmap'e dönüştürüyoruz
                    Bitmap pageImage = ConvertPageToImage(inputPdfPath, pageIndex + 1);

                    // OCR işlemiyle sayfadaki metni algıla ve metin alanlarını tespit et
                    var cropRect = GetTextBoundingBoxFromImage(pageImage);

                    // OCR başarısız olursa yedek olarak sayfanın kenarlarını kırp
                    if (cropRect == Rectangle.Empty)
                    {
                        cropRect = GetBackupCropRect(pageImage);
                    }

                    // Sayfa kırpma işlemi (CropBox değerlerini uygun şekilde ayarla)
                    if (cropRect != Rectangle.Empty)
                    {
                        XRect rect = new XRect(cropRect.Left, cropRect.Top, cropRect.Width, cropRect.Height);
                        page.CropBox = new PdfSharp.Pdf.PdfRectangle(rect);
                    }

                    // Yeni PDF'ye kırpılmış sayfayı ekle
                    outputPdf.AddPage(page);
                }

                outputPdf.Save(outputPdfPath);
                Console.WriteLine($"PDF başarıyla kırpıldı ve kaydedildi: {outputPdfPath}");
            }
        }

        // PDF sayfasını bitmap'e dönüştürme fonksiyonu
        static Bitmap ConvertPageToImage(string inputPdfPath, int pageNumber)
        {
            using (var pdfDocument = PdfiumViewer.PdfDocument.Load(inputPdfPath))
            {
                var dpi = 300; // Çözünürlük 

                // Render fonksiyonuyla PDF sayfasını bir Image nesnesi olarak alıyoruz
                using (var pageImage = pdfDocument.Render(pageNumber - 1, dpi, dpi, PdfRenderFlags.ForPrinting))
                {
                    return new Bitmap(pageImage);
                }
            }
        }

        // OCR ile sayfadaki metni bulma ve en küçük dikdörtgeni belirleme fonksiyonu
        static Rectangle GetTextBoundingBoxFromImage(Bitmap pageImage)
        {
            using (var engine = new TesseractEngine(@"C:\Users\COZUM\WithIronOcr\WithIronOcr\tessdata\", "tur", EngineMode.Default))
            {
                using (var pix = ConvertBitmapToPix(pageImage))
                {
                    using (var page = engine.Process(pix))
                    {
                        var iterator = page.GetIterator();
                        iterator.Begin();

                        int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

                        do
                        {
                            if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var boundingBox))
                            {
                                minX = Math.Min(minX, boundingBox.X1);
                                minY = Math.Min(minY, boundingBox.Y1);
                                maxX = Math.Max(maxX, boundingBox.X2);
                                maxY = Math.Max(maxY, boundingBox.Y2);
                            }
                        } while (iterator.Next(PageIteratorLevel.Word));

                        minX = Math.Max(0, minX);
                        minY = Math.Max(0, minY);
                        maxX = Math.Min(pageImage.Width, maxX);
                        maxY = Math.Min(pageImage.Height, maxY);

                        if (minX < maxX && minY < maxY)
                        {
                            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
                        }

                        return Rectangle.Empty;
                    }
                }
            }
        }

        // Yedek kırpma alanı fonksiyonu (OCR başarısız olduğunda kenarları kırpar)
        static Rectangle GetBackupCropRect(Bitmap pageImage)
        {
            int margin = 30; // Kenarlardan 30 piksel kırp
            return new Rectangle(margin, margin, pageImage.Width - 2 * margin, pageImage.Height - 2 * margin);
        }

        // Bitmap'i Pix formatına dönüştürme fonksiyonu (Tesseract için gerekli)
        static Pix ConvertBitmapToPix(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                return Pix.LoadFromMemory(stream.ToArray());
            }
        }
    }
}
*/
