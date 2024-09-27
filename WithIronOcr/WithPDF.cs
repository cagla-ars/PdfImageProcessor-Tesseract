/*using PdfiumViewer;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        bool continueProcessing = true;
        string outputDirectory = string.Empty;

        while (continueProcessing)
        {
            Console.WriteLine("Lütfen PDF dosyasının yolunu girin:");
            string pdfPath = Console.ReadLine().Replace("\"", "").Replace("'", "");

            if (File.Exists(pdfPath))
            {
                Console.WriteLine("Lütfen çıktı dizinini veya tam .txt dosya yolunu girin:");
                outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");
                outputDirectory = CheckAndHandleExistingPath(outputDirectory);

                // PDF'den metin çıkarma işlemini başlatıyoruz.
                ExtractTextFromPdf(pdfPath, outputDirectory);
            }
            else
            {
                Console.WriteLine("PDF dosyası bulunamadı.");
                continue;
            }

            Console.WriteLine("Yeni bir işlem yapmak istiyor musunuz? (y/n)");
            string continueInput = Console.ReadLine().ToLower();
            if (continueInput != "y")
            {
                continueProcessing = false;
            }
        }
    }

    static void ExtractTextFromPdf(string pdfPath, string outputDirectory)
    {
        try
        {
            using (var document = PdfDocument.Load(pdfPath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    try
                    {
                        // Her sayfanın metnini çıkartıyoruz
                        string pageText = document.GetPdfText(i);

                        // Eğer sayfa metni boş değilse, sorular arasında boşluk ekleyelim
                        if (!string.IsNullOrEmpty(pageText))
                        {
                            // Soruların numaralandırılmış olduğunu varsayarak numara desenine göre bölüyoruz.
                            // Örneğin, sorular "1. Soru", "2. Soru" gibi numaralarla başlıyorsa:
                            string[] questions = System.Text.RegularExpressions.Regex.Split(pageText, @"(?=\d+\.)");

                            // Soruların sonuna iki satır boşluk ekleyelim
                            string formattedText = "";
                            foreach (var question in questions)
                            {
                                formattedText += question.Trim() + "\n\n";  // Her soru arasına iki satır boşluk ekliyoruz
                            }

                            // Sayfa için metni .txt dosyasına kaydediyoruz
                            string textFilePath = $"{outputDirectory}\\page_{i + 1}.txt";
                            File.WriteAllText(textFilePath, formattedText);
                            Console.WriteLine($"Sayfa {i + 1} metni {textFilePath} dosyasına kaydedildi.");
                        }
                    }
                    catch (Exception pageEx)
                    {
                        Console.WriteLine($"Sayfa {i + 1} metni çıkartılamadı: {pageEx.Message}");
                    }
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
        if (Path.HasExtension(outputDirectory))
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
}*/
