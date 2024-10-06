// ImageProcessing/ImageProcessor.cs
using Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Tesseract;
using OpenCvSharp;

namespace ImageProcessing
{
    public static class ImageProcessor
    {
        public static void ProcessImages()
        {
            List<string> imagePaths = new List<string>();
            bool continueProcessing = true;
            string language = string.Empty;
            // Başlık ve önemli bilgilendirmeleri renkli yapıyoruz
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------------HOŞGELDİNİZ---------------\n");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("!!!!!!ÖNEMLİ!!!!!!\n");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Lütfen görsellerinizi düz ve rengi görselinizden farklı olan bir alanda yazılar okunabilecek şekilde çekiniz.");
            Console.ResetColor();


            while (continueProcessing)
            {
                imagePaths.Clear();
                bool save = false;

                // Görselleri toplu ya da tek tek alma işlemi
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nGörselleri nasıl yüklemek istersiniz?");
                Console.ResetColor();
                Console.WriteLine("1. Toplu şekilde yüklemek istiyorum (Klasör)");
                Console.WriteLine("2. Tek tek resim yüklemek istiyorum");
                string selection = Console.ReadLine();

                if (selection == "1")
                {
                    Console.WriteLine("\nLütfen görsellerin bulunduğu klasörün yolunu girin:");
                    string folderPath = Console.ReadLine().Replace("\"", "").Replace("'", "");

                    if (Directory.Exists(folderPath))
                    {
                        string[] files = Directory.GetFiles(folderPath);
                        foreach (string file in files)
                        {
                            if (FileHelper.IsValidImageFile(file))
                            {
                                imagePaths.Add(file);
                            }
                        }
                        if (imagePaths.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Bu klasörde işlenebilecek geçerli görsel dosyası bulunamadı.");
                            Console.ResetColor();
                            continue;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Geçerli bir klasör yolu girmediniz.");
                        Console.ResetColor();
                        continue;
                    }
                }
                else if (selection == "2")
                {
                    Console.WriteLine("\nLütfen görsellerinizi tek tek girin. İşlemi bitirdiğinizde ENTER'a basın.");
                    int i = 1;
                    while (true)
                    {
                        Console.WriteLine($"Lütfen {i}. resminizin dosya yolunu giriniz (ENTER'a basarak işlemden çıkabilirsiniz):");
                        string input = Console.ReadLine().Replace("\"", "").Replace("'", "");

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            if (imagePaths.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("En az bir görsel dosyası girmelisiniz.");
                                Console.ResetColor();
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (!File.Exists(input))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Dosya yolunu hatalı girdiniz, dosya bulunamadı. Lütfen tekrar deneyiniz.");
                                Console.ResetColor();
                            }
                            else if (!FileHelper.IsValidImageFile(input))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Bu dosya bir resim formatında değil. Lütfen geçerli bir resim dosyası giriniz (jpg, png, bmp, vb.).");
                                Console.ResetColor();
                            }
                            else
                            {
                                imagePaths.Add(input);
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Geçersiz seçim. Lütfen 1 ya da 2'yi seçin.");
                    Console.ResetColor();
                    continue;
                }

                // Çıktı dizinini alma işlemi
                Console.WriteLine("\nLütfen çıktı dosyalarının kaydedileceği dizini girin:");
                string outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");
                outputDirectory = FileHelper.CheckAndHandleExistingPath(outputDirectory);

                // Dil bilgisi alma
                while (string.IsNullOrEmpty(language))
                {
                    Console.WriteLine("\nLütfen kullanmak istediğiniz dili girin (örn. 'Türkçe', 'İngilizce', 'Rusça'): ");
                    string inputLanguage = Console.ReadLine();
                    language = FileHelper.GetLanguageCode(inputLanguage);

                    string tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                    string trainedDataPath = Path.Combine(tessdataPath, $"{language}.traineddata");

                    if (!File.Exists(trainedDataPath))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{language}.traineddata dosyası bulunamadı.");
                        Console.ResetColor();
                        language = string.Empty;
                    }
                }

                // Görüntü işleme
                int folderCounter = 1;
                foreach (var imagePath in imagePaths)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\nİşlem başlatılıyor: {imagePath}");
                    Console.ResetColor();

                    ImageHandler.ProcessImage(imagePath, outputDirectory, ref folderCounter, language, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"İşlem tamamlandı. İlgili dosyalara şu yoldan ulaşabilirsiniz: {outputDirectory}\n");
                    Console.ResetColor();
                }

                // Yeni işlem yapmak isteyip istemediğini soruyoruz
                Console.WriteLine("Yeni bir işlem yapmak istiyor musunuz? (Evet için 'y', Hayır için 'n')");
                string continueInput = Console.ReadLine().ToLower();
                if (continueInput != "y")
                {
                    continueProcessing = false;
                }
            }
        }
    }
}
