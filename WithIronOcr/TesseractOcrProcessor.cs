/*using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace WithIronOcr
{
    public class TesseractOcrProcessor : IOcrProcessor
    {
        private readonly string language;
        private readonly string tessdataPath;

        public TesseractOcrProcessor(string language, string tessdataPath)
        {
            this.language = language;
            this.tessdataPath = tessdataPath;
        }

        public void PerformOcr(string imagePath, string outputDirectory)
        {
            try
            {
                // Tesseract OCR işlemi
                using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
                {
                    engine.DefaultPageSegMode = PageSegMode.Auto;
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var extractedText = page.GetText();
                            Console.WriteLine("Çıkarılan Metin:");
                            Console.WriteLine(extractedText);

                            // Çıkarılan metni dosyaya kaydedelim
                            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            string outputFilePath = Path.Combine(outputDirectory, $"ocr_output_{timestamp}.txt");
                            File.WriteAllText(outputFilePath, extractedText);
                            Console.WriteLine($"Çıkarılan metin kaydedildi: {outputFilePath}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OCR işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }
    }
}
*/