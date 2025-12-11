// PdfProcessing/PdfTextExtractor.cs
using PdfiumViewer;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PdfProcessing
{
    public static class PdfTextExtractor
    {
        public static void ExtractTextFromPdf(string pdfPath, string textDirectory)
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
                                string textFilePath = Path.Combine(textDirectory, $"page_{i + 1}_{timestamp}.txt");
                                File.WriteAllText(textFilePath, formattedText);
                                Console.WriteLine($"Sayfa {i + 1} metni {textFilePath} dosyasına kaydedildi.");
                            }
                        }
                        catch (Exception pageEx)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
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
