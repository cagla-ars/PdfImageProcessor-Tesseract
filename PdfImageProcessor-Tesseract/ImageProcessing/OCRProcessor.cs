// ImageProcessing/OCRProcessor.cs
using OpenCvSharp;
using System;
using System.Globalization;
using System.IO;
using Tesseract;

namespace ImageProcessing
{
    public static class OCRProcessor
    {
        public static void AppendTextToFile(string outputDirectory, ref int folderCounter, string imagePath, Mat image, Mat warped, string language, string tessdataPath)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

            // Ana klasör oluşturma
            string mainFolder = Path.Combine(Path.GetDirectoryName(outputDirectory), "ProcessedImages");
            Directory.CreateDirectory(mainFolder);

            // Her bir görsel için yeni bir klasör oluşturma
            string imageFolder = Path.Combine(mainFolder, $"image{folderCounter}_{timestamp}");
            Directory.CreateDirectory(imageFolder);
            folderCounter++; // Klasör sayacını artır

            // Görselleri kaydet
            string warpedImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_warped.png");
            string rawImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_raw.png");
            Cv2.ImWrite(warpedImagePath, warped);
            Cv2.ImWrite(rawImagePath, image);

            using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(File.ReadAllBytes(warpedImagePath)))
                {
                    using (var page = engine.Process(img))
                    {
                        var extractedText = page.GetText();

                        // Metni dosyaya ekle
                        using (StreamWriter sw = new StreamWriter(outputDirectory, append: true))
                        {
                            sw.WriteLine($"Resim: {imagePath}");
                            sw.WriteLine($"İşlenme tarihi: {timestamp}");
                            sw.WriteLine("Çıkarılan metin:");
                            sw.WriteLine(extractedText);
                            sw.WriteLine("------------------------------");
                        }
                    }
                }
            }
        }

        public static void ProcessDirectory(string outputDirectory, ref int folderCounter, string imagePath, Mat image, Mat warped, string language, string tessdataPath)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

            // Ana klasör oluşturma
            string mainFolder = Path.Combine(outputDirectory, "ProcessedImages");
            Directory.CreateDirectory(mainFolder);

            string imageFolder = Path.Combine(mainFolder, $"image{folderCounter}_{timestamp}");
            Directory.CreateDirectory(imageFolder);
            folderCounter++;

            // Görselleri kaydet
            string warpedImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_warped.png");
            string rawImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_raw.png");
            Cv2.ImWrite(warpedImagePath, warped);
            Cv2.ImWrite(rawImagePath, image);

            using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(File.ReadAllBytes(warpedImagePath)))
                {
                    using (var page = engine.Process(img))
                    {
                        var extractedText = page.GetText();

                        // Metni dosyaya yaz
                        string textFilePath = Path.Combine(imageFolder, $"txt_{timestamp}.txt");
                        File.WriteAllText(textFilePath, extractedText);
                    }
                }
            }
        }
    }
}
