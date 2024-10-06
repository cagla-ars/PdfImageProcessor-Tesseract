// Program.cs
using PdfProcessing;
using ImageProcessing;
using Utilities;
using System;

namespace PdfImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            bool continueProcessing = true;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("--------------- PdfImageProcessor ---------------\n");
            Console.ResetColor();

            while (continueProcessing)
            {
                // Seçenekler
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Lütfen yapmak istediğiniz işlemi seçin:");
                Console.WriteLine("1. PDF dosyası işleme");
                Console.WriteLine("2. Görsel dosyası işleme");
                Console.WriteLine("Çıkmak için 'q' tuşlayın.\n");
                Console.ResetColor();

                string selection = Console.ReadLine();

                if (selection == "1")
                {
                    PdfProcessor.ProcessPdf();
                }
                else if (selection == "2")
                {
                    ImageProcessor.ProcessImages();
                }
                else if (selection.ToLower() == "q")
                {
                    continueProcessing = false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.\n");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Program sonlandırılıyor...");
            Console.ResetColor();
        }
    }
}
