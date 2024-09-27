/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithIronOcr
{
    internal class UserInputHandler
    {
public List<string> GetImagePaths()
        {
            List<string> imagePaths = new List<string>();
            int i = 1;
            while (true)
            {
                Console.WriteLine($"Lütfen {i}. resminizin dosya yolunu giriniz (enter'a basarak işlemi sonlandırabilirsiniz):");
                string input = Console.ReadLine().Replace("\"", "").Replace("'", "");

                if (string.IsNullOrWhiteSpace(input))
                {
                    if (imagePaths.Count == 0)
                    {
                        Console.WriteLine("En az bir görsel dosyası girmelisiniz.");
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!File.Exists(input))
                {
                    Console.WriteLine("Dosya bulunamadı. Lütfen tekrar deneyiniz.");
                }
                else
                {
                    imagePaths.Add(input);
                    i++;
                }
            }
            return imagePaths;
        }

        public string GetOutputDirectory()
        {
            while (true)
            {
                Console.WriteLine("Lütfen çıktı dosyalarının kaydedileceği dizini girin:");
                string outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");

                if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("Girilen dizin mevcut değil. Dosyanızı masaüstüne kaydetmek ister misiniz? (Evet için 'y', Hayır için 'n' tuşuna basın)");
                    string userInput = Console.ReadLine().ToLower();
                    if (userInput == "y")
                    {
                        outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        Console.WriteLine($"Dosyalar masaüstüne kaydedilecek: {outputDirectory}");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Lütfen geçerli bir dizin girin.");
                    }
                }
                else
                {
                    return outputDirectory;
                }
            }
        }

        public string GetLanguage()
        {
            while (true)
            {
                Console.WriteLine("Lütfen kullanmak istediğiniz dili girin (örn. 'Türkçe', 'English'): ");
                string inputLanguage = Console.ReadLine().Trim().ToLower();

                switch (inputLanguage)
                {
                    case "türkçe":
                    case "turkish":
                        return "tur";
                    case "ingilizce":
                    case "english":
                        return "eng";
                    default:
                        Console.WriteLine("Geçersiz dil girdiniz. Lütfen 'Türkçe' veya 'English' giriniz.");
                        break;
                }
            }
        }
    }
}
*/