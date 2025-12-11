using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Utilities;

namespace ImageProcessing
{
    public static class ImageHandler
    {
        public static List<string> WarpedImagePaths { get; private set; } = new List<string>();

        public static void ProcessImage(
            string imagePath,
            string outputDirectory,
            ref int folderCounter,
            string language,
            string tessdataPath)
        {
            try
            {

                //  Görüntüyü yükle ve perspektif kontrolü yap 
                var (originalImage, warpedImage) = PerspectiveCorrector.LoadAndProcessImage(imagePath);

                // Görsel yüklenememişse işlemi kes
                if (originalImage == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Görüntü işlenemedi\n");
                    Console.ResetColor();
                    return;
                }

                Mat imageForOcr = warpedImage ?? originalImage;

                if (warpedImage == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Perspektif düzeltme uygulanmadı → Sadece OCR yapılacak, görsel kaydedilmeyecek.");
                    Console.ResetColor();
                }

                if (Path.HasExtension(outputDirectory))
                {
                    OCRProcessor.AppendTextToFile(
                        outputDirectory,
                        ref folderCounter,
                        imagePath,
                        originalImage,
                        imageForOcr,
                        language,
                        tessdataPath);


                    originalImage?.Dispose();
                    warpedImage?.Dispose();
                    return;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                string mainFolder = Path.Combine(outputDirectory, "ProcessedImages");
                Directory.CreateDirectory(mainFolder);

                string imageFolder = Path.Combine(mainFolder, $"image{folderCounter}_{timestamp}");
                Directory.CreateDirectory(imageFolder);

                folderCounter++;

                string warpedImagePath = null;


                if (warpedImage != null)
                {
                    warpedImagePath = Path.Combine(
                        imageFolder,
                        $"{Path.GetFileNameWithoutExtension(imagePath)}_warped.png");

                    Cv2.ImWrite(warpedImagePath, warpedImage);
                    WarpedImagePaths.Add(warpedImagePath);
                }

                OCRProcessor.ProcessDirectory(
                    outputDirectory,
                    ref folderCounter,
                    imagePath,
                    originalImage,
                    imageForOcr,
                    language,
                    tessdataPath,
                    imageFolder);


                originalImage?.Dispose();
                warpedImage?.Dispose();
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
