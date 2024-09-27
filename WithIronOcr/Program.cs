/*using System;

namespace MyProjectWithTesseract
{
    class Program
    {
        static void Main(string[] args)
        {
            bool continueProcessing = true;
            while (continueProcessing)
            {
                // Kullanıcıdan giriş alalım
                UserInputHandler inputHandler = new UserInputHandler();
                var imagePaths = inputHandler.GetImagePaths();
                string outputDirectory = inputHandler.GetOutputDirectory();
                string language = inputHandler.GetLanguage();

                // OCR işlemi için Tesseract implementasyonunu kullanıyoruz
                IOcrProcessor ocrProcessor = new TesseractOcrProcessor(language, AppDomain.CurrentDomain.BaseDirectory + "tessdata");

                // Her bir resim için OCR işlemi yapalım
                foreach (var imagePath in imagePaths)
                {
                    try
                    {
                        ocrProcessor.PerformOcr(imagePath, outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                    }
                }

                // Kullanıcıya devam edip etmeyeceğini soralım
                Console.WriteLine("Yeni bir işlem yapmak istiyor musunuz? (Evet için 'y', Hayır için 'n')");
                string continueInput = Console.ReadLine().ToLower();
                if (continueInput != "y")
                {
                    continueProcessing = false;
                    Console.WriteLine("Program sonlandırılıyor...");
                }
            }
        }
    }
}

*/
