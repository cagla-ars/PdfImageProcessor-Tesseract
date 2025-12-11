using PdfProcessing;
using ImageProcessing;
using Utilities;
using System;
using System.IO;
using System.Text;

namespace PdfImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var standardOutput = Console.Out; 
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);

            bool continueProcessing = true;


            Console.SetOut(standardOutput);
            Console.SetError(standardOutput);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("--------------- PdfImageProcessor ---------------\n");
            Console.ResetColor();

            while (continueProcessing)
            {
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
