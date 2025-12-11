using OpenCvSharp;
using System;
using System.Globalization;
using System.IO;
using Tesseract;

namespace ImageProcessing
{
    public static class OCRProcessor
    {

        public static void AppendTextToFile(
            string outputFile,
            ref int folderCounter,
            string imagePath,
            Mat original,
            Mat imageForOcr,
            string language,
            string tessdataPath)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

                using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
                using (var img = Pix.LoadFromMemory(imageForOcr.ToBytes()))
                using (var page = engine.Process(img))
                {
                    string extractedText = page.GetText();

                    using (StreamWriter sw = new StreamWriter(outputFile, append: true))
                    {
                        sw.WriteLine($"Resim: {imagePath}");
                        sw.WriteLine($"İşlenme Tarihi: {timestamp}");
                        sw.WriteLine("OCR Çıktısı:");
                        sw.WriteLine(extractedText);
                        sw.WriteLine("------------------------------");
                    }
                }

                folderCounter++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("OCR işleminde hata: " + ex.Message);
                Console.ResetColor();
            }
        }


        public static void ProcessDirectory(
            string outputDirectory,
            ref int folderCounter,
            string imagePath,
            Mat original,
            Mat imageForOcr,
            string language,
            string tessdataPath,
            string imageFolder)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

  

                string textFilePath = Path.Combine(imageFolder, $"txt_{timestamp}.txt");

                using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
                using (var img = Pix.LoadFromMemory(imageForOcr.ToBytes()))
                using (var page = engine.Process(img))
                {
                    string extractedText = page.GetText();
                    File.WriteAllText(textFilePath, extractedText);
                }

                folderCounter++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("OCR işleminde hata: " + ex.Message);
                Console.ResetColor();
            }
        }
    }
}
