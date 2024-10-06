// ImageProcessing/ImageHandler.cs
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Tesseract;
using Utilities;

namespace ImageProcessing
{
    public static class ImageHandler
    {
        public static List<string> WarpedImagePaths { get; private set; } = new List<string>();

        public static void ProcessImage(string imagePath, string outputDirectory, ref int folderCounter, string language, string tessdataPath)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"İşlem başlatılıyor: {imagePath}");
                Console.ResetColor();

                // Görüntüyü yükleyin ve gri tonlamaya çevirin
                var (image, warped) = PerspectiveCorrector.LoadAndProcessImage(imagePath);

                if (image == null || warped == null)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Görüntü işlenemedi: {imagePath}\n");
                    Console.ResetColor();
                    return;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

                // Dosya yolu mu, dizin yolu mu olduğunu kontrol edelim
                if (Path.HasExtension(outputDirectory))
                {
                    // Tek bir dosyaya OCR çıktısını ekleyelim
                    OCRProcessor.AppendTextToFile(outputDirectory, ref folderCounter, imagePath, image, warped, language, tessdataPath);
                }
                else
                {
                    // Dizin yoluna her görsel için ayrı klasör oluşturalım
                    OCRProcessor.ProcessDirectory(outputDirectory, ref folderCounter, imagePath, image, warped, language, tessdataPath);
                }

                // Warped image path ekle
                string warpedImageFullPath = Path.Combine(outputDirectory, "ProcessedImages", $"image{folderCounter - 1}_{timestamp}", $"{Path.GetFileNameWithoutExtension(imagePath)}_warped.png");
                WarpedImagePaths.Add(warpedImageFullPath);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("İşlem tamamlandı. İlgili dosyalara çıktı dizini üzerinden ulaşabilirsiniz.\n");
                Console.ResetColor();

                // Kaynakları serbest bırakma
                image.Dispose();
                warped.Dispose();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Bir hata oluştu: {ex.Message}\n");
                Console.ResetColor();
            }
        }
    }
}
