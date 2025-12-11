using Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImageProcessing
{
    public static class ImageProcessor
    {
        public static void ProcessImages()
        {
            List<string> imagePaths = new List<string>();
            bool continueProcessing = true;

            while (continueProcessing)
            {
                imagePaths.Clear();
                string language = string.Empty;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("---------------HOŞGELDİNİZ---------------\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("!!!!!!ÖNEMLİ!!!!!!\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Lütfen görsellerinizi düz ve rengi görselinizden farklı olan bir alanda yazılar okunabilecek şekilde çekiniz.\n");
                Console.ResetColor();


                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nGörselleri nasıl yüklemek istersiniz?");
                Console.ResetColor();
                Console.WriteLine("1. Toplu şekilde yüklemek istiyorum (Klasör)");
                Console.WriteLine("2. Tek tek resim yüklemek istiyorum");

                string selection = Console.ReadLine();


                if (selection == "1")
                {
                    Console.WriteLine("\nLütfen görsellerin bulunduğu klasörün yolunu girin:");
                    string folderPath = Console.ReadLine().Trim('"', '\'');

                    if (!Directory.Exists(folderPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Geçerli bir klasör yolu girmediniz.");
                        Console.ResetColor();
                        continue;
                    }

                    foreach (string file in Directory.GetFiles(folderPath))
                    {
                        if (FileHelper.IsValidImageFile(file))
                            imagePaths.Add(file);
                    }

                    if (imagePaths.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bu klasörde işlenebilecek geçerli görsel dosyası bulunamadı.");
                        Console.ResetColor();
                        continue;
                    }
                }

                else if (selection == "2")
                {
                    Console.WriteLine("\nGörsellerinizi tek tek girin. Bitirmek için ENTER'a basın.");
                    int i = 1;

                    while (true)
                    {
                        Console.WriteLine($"Lütfen {i}. resminizi giriniz (boş ENTER = bitir):");
                        string input = Console.ReadLine().Trim('"', '\'');

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            if (imagePaths.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("En az bir görsel dosyası girmelisiniz.");
                                Console.ResetColor();
                                continue;
                            }
                            break;
                        }

                        if (!File.Exists(input))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Dosya bulunamadı.");
                            Console.ResetColor();
                            continue;
                        }

                        if (!FileHelper.IsValidImageFile(input))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Bu bir resim formatı değil.");
                            Console.ResetColor();
                            continue;
                        }

                        imagePaths.Add(input);
                        i++;
                    }
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Geçersiz seçim.");
                    Console.ResetColor();
                    continue;
                }


                Console.WriteLine("\nÇıktı dosyalarının kaydedileceği dizini girin:");
                string outputDirectory = FileHelper.CheckAndHandleExistingPath(
                    Console.ReadLine().Trim('"', '\'')
                );


                while (string.IsNullOrEmpty(language))
                {
                    Console.WriteLine("\nLütfen kullanmak istediğiniz dili girin (Türkçe / İngilizce / Rusça):");
                    string langInput = Console.ReadLine();

                    language = FileHelper.GetLanguageCode(langInput);

                    string tessFile = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "tessdata",
                        $"{language}.traineddata"
                    );

                    if (!File.Exists(tessFile))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{language}.traineddata bulunamadı!");
                        Console.ResetColor();
                        language = "";
                    }
                }

                int folderCounter = 1;

                foreach (var imagePath in imagePaths)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\nİşlem başlatılıyor: {imagePath}");
                    Console.ResetColor();

                    ImageHandler.ProcessImage(
                        imagePath,
                        outputDirectory,
                        ref folderCounter,
                        language,
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata")
                    );

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"İşlem tamamlandı. Çıktılar: {outputDirectory}\n");
                    Console.ResetColor();
                }

                Console.WriteLine("\nYeni bir işlem yapmak istiyor musunuz?");
                Console.WriteLine("y = tekrar görsel işle");
                Console.WriteLine("n = programdan çık");
                Console.WriteLine("m = ANA MENÜYE dön");

                string input2 = Console.ReadLine().ToLower();

                if (input2 == "m")
                {
                    return;
                }
                else if (input2 != "y")
                {
                    continueProcessing = false;
                }
            }
        }
    }
}
