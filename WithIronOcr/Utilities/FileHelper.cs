// Utilities/FileHelper.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utilities
{
    public static class FileHelper
    {
        /// <summary>
        /// Çıkış yolunu kontrol eder ve kullanıcıya uygun seçenekleri sunar.
        /// Eğer yol bir dosya ise mevcut olup olmadığını kontrol eder ve kullanıcıya değişiklik yapma seçeneği sunar.
        /// Eğer yol bir dizin ise mevcut olup olmadığını kontrol eder ve kullanıcıya masaüstüne kaydetme seçeneği sunar.
        /// </summary>
        /// <param name="outputDirectory">Kullanıcının belirttiği çıkış dizini veya dosya yolu.</param>
        /// <returns>Geçerli bir çıkış dizini yolu.</returns>
        public static string CheckAndHandleExistingPath(string outputDirectory)
        {
            // Desteklenen dosya formatları
            var imageFormats = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
            var textFormats = new HashSet<string> { ".txt", ".csv", ".log" };

            while (true)
            {
                try
                {
                    if (Path.HasExtension(outputDirectory))
                    {
                        // Yol bir dosya yolu
                        string fileExtension = Path.GetExtension(outputDirectory).ToLower();

                        if (imageFormats.Contains(fileExtension))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Hata: Bir dizin yolu girmediniz. Lütfen bir dizin yolu girin.");
                            Console.ResetColor();
                            outputDirectory = PromptForPath();
                            continue;
                        }
                        else if (textFormats.Contains(fileExtension))
                        {
                            // Yol bir metin dosyası yolu
                            if (File.Exists(outputDirectory))
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"Aynı isimde bir dosya mevcut: {outputDirectory}");
                                Console.WriteLine("Dosya zaten mevcut. Değişiklik yapmak istiyor musunuz? (Evet için 'y', Hayır için 'n')");
                                Console.ResetColor();
                                string userInput = Console.ReadLine().ToLower();

                                if (userInput == "y")
                                {
                                    // Kullanıcı mevcut dosyayı kullanmayı seçti
                                    return outputDirectory;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Lütfen yeni bir dosya yolu girin:");
                                    Console.ResetColor();
                                    outputDirectory = Console.ReadLine();
                                    continue;
                                }
                            }
                            else
                            {
                                // Dosya mevcut değil, yolu kullan
                                return outputDirectory;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Girilen dosya formatı desteklenmiyor. Lütfen bir metin dosya formatı girin.");
                            Console.ResetColor();
                            outputDirectory = PromptForPath();
                            continue;
                        }
                    }
                    else
                    {
                        // Yol bir dizin yolu
                        if (Directory.Exists(outputDirectory))
                        {
                            return outputDirectory;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Girilen dizin mevcut değil. Dosyanızı masaüstüne kaydetmek ister misiniz? (Evet için 'y', Hayır için 'n')");
                            Console.ResetColor();
                            string userInput = Console.ReadLine().ToLower();

                            if (userInput == "y")
                            {
                                // Masaüstü dizinini kullan
                                outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Dosyalar masaüstüne kaydedilecek: {outputDirectory}");
                                Console.ResetColor();
                                return outputDirectory;
                            }
                            else if (userInput == "n")
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Lütfen geçerli bir dizin girin:");
                                Console.ResetColor();
                                outputDirectory = Console.ReadLine();
                                continue;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Geçersiz giriş. Lütfen 'y' ya da 'n' tuşlarına basın.");
                                Console.ResetColor();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Bir hata oluştu: {ex.Message}. Tekrar deneyin.");
                    Console.ResetColor();
                    outputDirectory = PromptForPath();
                }
            }
        }

        /// <summary>
        /// Prompts the user to enter a valid file or directory path.
        /// </summary>
        /// <returns>User-entered path.</returns>
        private static string PromptForPath()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Lütfen geçerli bir dosya veya dizin yolu girin:");
            Console.ResetColor();
            return Console.ReadLine();
        }

        /// <summary>
        /// Checks if the provided file path has a valid image file extension.
        /// </summary>
        /// <param name="filePath">File path to check.</param>
        /// <returns>True if valid image file, else false.</returns>
        public static bool IsValidImageFile(string filePath)
        {
            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif" };
            string fileExtension = Path.GetExtension(filePath).ToLower();
            return validExtensions.Contains(fileExtension);
        }

        /// <summary>
        /// Converts user input language string to Tesseract language code.
        /// </summary>
        /// <param name="inputLanguage">User input language string.</param>
        /// <returns>Tesseract language code.</returns>
        public static string GetLanguageCode(string inputLanguage)
        {
            switch (inputLanguage.Trim().ToLower())
            {
                case "türkçe":
                case "turkish":
                case "turkce":
                case "türkce":
                    return "tur";
                case "ingilizce":
                case "english":
                    return "eng";
                case "rusça":
                case "russian":
                    return "rus";
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Girdiğiniz dil sistemimizde bulunmamaktadır. Tekrar deneyiniz.");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Lütfen kullanmak istediğiniz dili tekrar girin (örn. 'Türkçe', 'İngilizce', 'Rusça'):");
                    Console.ResetColor();
                    string newInput = Console.ReadLine();
                    return GetLanguageCode(newInput);
            }
        }
    }
}
