/*using OpenCvSharp;
using Tesseract;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        bool continueProcessing = true; // Programın devam edip etmeyeceğini kontrol etmek için
        bool save = false; // Masaüstüne kaydetmeyi kontrol etmek için
        List<string> imagePaths = new List<string>();
        List<string> warpedImagePaths = new List<string>(); // Perspektif düzeltilmiş resimler için liste
        string outputDirectory = string.Empty;
        string tessdataPath = string.Empty;
        int folderCounter = 1; // Klasör isimlendirmesi için sayaç

        while (continueProcessing)
        {
            imagePaths.Clear(); // Her işlem için imagePaths listesini temizle
            warpedImagePaths.Clear(); // Her işlem için warpedImagePaths listesini temizle
            save = false; // Her yeni işlem için masaüstüne kaydetme seçeneğini sıfırla
            string language = string.Empty; // Her işlemde dil seçimi yapılması için sıfırlama


            Console.WriteLine("---------------HOŞGELDİNİZ---------------");
            Console.WriteLine(); // Boş satır eklemek için
            Console.WriteLine("!!!!!!ÖNEMLİ!!!!!!");
            Console.WriteLine(); // Boş satır eklemek için
            Console.WriteLine("Lütfen görsellerinizi düz ve rengi görselinizden farklı olan bir alanda yazılar okunabilecek şekilde çekiniz.");
            Console.WriteLine("Dilerseniz ek olarak görsellerinizi pdf formatına da çevirebiliriz.");
            Console.WriteLine(); // Boş satır eklemek için
            Console.WriteLine("Görselleri nasıl yüklemek istersiniz?");
            Console.WriteLine("1 VE 2 seçeneğini kullanınız.");
            Console.WriteLine();
            Console.WriteLine("1. Toplu şekilde yüklemek istiyorum (Görseller bir klasör içerisinde olmalı.)");
            Console.WriteLine("2. Tek tek resim yüklemek istiyorum");

            string selection = Console.ReadLine();

            if (selection == "1")
            {
                // Klasör yolu üzerinden toplu görsel işleme
                Console.WriteLine("Lütfen görsellerin bulunduğu klasörün yolunu girin:");
                string folderPath = Console.ReadLine().Replace("\"", "").Replace("'", "");

                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath);
                    foreach (string file in files)
                    {
                        if (IsValidImageFile(file))
                        {
                            imagePaths.Add(file);
                        }
                    }
                    if (imagePaths.Count == 0)
                    {
                        Console.WriteLine("Bu klasörde işlenebilecek geçerli görsel dosyası bulunamadı.");
                        continue; // İşlenebilecek görsel yoksa tekrar başa dön
                    }
                }
                else
                {
                    Console.WriteLine("Geçerli bir klasör yolu girmediniz.");
                    continue; // Geçerli bir klasör girilmediyse tekrar başa dön
                }
            }
            else if (selection == "2")
            {
                // Tek tek resim dosyası alma
                Console.WriteLine("Lütfen görsellerinizi tek tek girin. İşlemi bitirdiğinizde ENTER'a basın.");
                int i = 1;
                while (true)
                {
                    Console.WriteLine($"Lütfen {i}. resminizin dosya yolunu giriniz (ENTER'a basarak işlemden çıkabilirsiniz):");
                    string input = Console.ReadLine().Replace("\"", "").Replace("'", "");

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        if (imagePaths.Count == 0)
                        {
                            Console.WriteLine("En az bir görsel dosyası girmelisiniz.");
                        }
                        else
                        {
                            break; // En az bir resim girdiyse döngüden çık
                        }
                    }
                    else
                    {
                        if (!File.Exists(input))
                        {
                            Console.WriteLine("Dosya yolunu hatalı girmiş olabilirsiniz, dosya yolunuz bulunamadı. Lütfen tekrar deneyiniz..");
                        }
                        else if (!IsValidImageFile(input))
                        {
                            Console.WriteLine("Bu dosya bir resim formatında değil. Lütfen geçerli bir resim dosyası giriniz (jpg, png, bmp, vb.).");
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
                Console.WriteLine("Geçersiz bir seçim yaptınız. Lütfen 1 ya da 2'yi seçin.");
                continue; // Geçersiz seçimde tekrar başa dön
            }

            // Çıktı dizinini alma veya masaüstüne kaydetme seçeneği
            while (true)
            {
                Console.WriteLine("Lütfen çıktı dosyalarının kaydedileceği dizini veya tam .txt dosya yolunu girin:");
                outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");

                outputDirectory = CheckAndHandleExistingPath(outputDirectory);

                if (Path.HasExtension(outputDirectory)) // Eğer yol bir dosya ise
                {
                    Console.WriteLine($"Metin dosyası şu yola kaydedilecek: {outputDirectory}");
                    break;
                }
                else
                {
                    Console.WriteLine($"Dosyalar şu dizine kaydedilecek: {outputDirectory}");
                    break;
                }
            }

            // Çıktı dizinini alma veya masaüstüne kaydetme seçeneği
            while (true)
            {
                Console.WriteLine("Lütfen çıktı dosyalarının kaydedileceği dizini veya tam .txt dosya yolunu girin:");
                outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");

                outputDirectory = CheckAndHandleExistingPath(outputDirectory);

                if (Path.HasExtension(outputDirectory)) // Eğer yol bir dosya ise
                {
                    Console.WriteLine($"Metin dosyası şu yola kaydedilecek: {outputDirectory}");
                    break;
                }
                else
                {
                    Console.WriteLine($"Dosyalar şu dizine kaydedilecek: {outputDirectory}");
                    break;
                }
            }
            // Dil bilgisi alma ve dil koduna çevirme işlemi
            while (string.IsNullOrEmpty(language))
            {
                Console.WriteLine("Lütfen kullanmak istediğiniz dili girin (örn. 'Türkçe', 'İngilizce', 'Rusça'): ");
                string inputLanguage = Console.ReadLine().Trim().ToLower();

                switch (inputLanguage)
                {
                    case "türkçe":
                    case "turkish":
                    case "Türkçe":
                    case "TÜRKÇE":
                    case "turkçe":
                    case "turkce":
                    case "TURKÇE":
                    case "TURKCE":
                        language = "tur";
                        break;
                    case "ingilizce":
                    case "english":
                    case "İngilizce":
                    case "İNGİLİZCE":
                        language = "eng";
                        break;
                    case "rusça":
                    case "russian":
                    case "Rusça":
                    case "RUSÇA":
                        language = "rus";
                        break;
                    default:
                        Console.WriteLine("Girdiğiniz dil sistemimizde bulunmamaktadır..Tekrar deneyiniz..");
                        Console.WriteLine(); // Boş satır eklemek için
                        continue;
                }
                tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                string trainedDataPath = Path.Combine(tessdataPath, $"{language}.traineddata");
                if (!File.Exists(trainedDataPath))
                {
                    Console.WriteLine($"{language}.traineddata dosyası bulunamadı.");
                    Console.WriteLine(); // Boş satır eklemek için
                    language = string.Empty; // Geçersiz dil, tekrar sor
                }
            }

            // Görüntü işleme ve OCR işlemi için metodun çağrılması
            foreach (var imagePath in imagePaths)
            {
                ProcessImage(imagePath, outputDirectory, ref folderCounter, language, tessdataPath);
            }
            // Kullanıcıya PDF oluşturma isteği sorulur
            Console.WriteLine("İşlenen perspektif görselleri PDF dosyası olarak kaydetmek ister misiniz? (Evet için 'y', Hayır için 'n')");
            string pdfInput = Console.ReadLine().ToLower();

            if (pdfInput == "y")
            {
                CreatePdfFromImages(warpedImagePaths, outputDirectory);
                Console.WriteLine("PDF başarıyla oluşturuldu.");
            }
            // Tüm işlemler tamamlandıktan sonra taşıma işlemini gerçekleştirin
            if (File.Exists(outputDirectory)) // Eğer outputDirectory bir dosya yolunu belirtiyorsa
            {
                string destinationDirectory = Path.Combine(Path.GetDirectoryName(outputDirectory), "ProcessedImages");
                Directory.CreateDirectory(destinationDirectory); // Ana dizini oluştur
                string destinationTextFilePath = Path.Combine(destinationDirectory, Path.GetFileName(outputDirectory)); // Yeni dosya yolu
                File.Move(outputDirectory, destinationTextFilePath); // Dosyayı taşı
                Console.WriteLine($"Metin dosyası {destinationTextFilePath} dizinine taşındı.");
            }

            // İşlem tamamlandıktan sonra kullanıcıya yeni işlem yapıp yapmayacağını soralım
            while (true) // Kullanıcı doğru bir giriş yapana kadar döngü devam eder
            {
                Console.WriteLine("Yeni bir işlem yapmak istiyor musunuz? (Evet için 'y', Hayır için 'n')");
                string continueInput = Console.ReadLine().ToLower();

                if (continueInput == "y")
                {
                    continueProcessing = true;
                    break; // Geçerli bir değer girdiğinde döngüden çık
                }
                else if (continueInput == "n")
                {
                    Console.WriteLine("Program sonlandırılıyor...");
                    Console.WriteLine(); // Boş satır eklemek için
                    Environment.Exit(0); // Programı sonlandır
                }
                else
                {
                    // Geçersiz bir değer girildiyse kullanıcıya tekrar sor
                    Console.WriteLine("Geçersiz giriş. Lütfen 'y' ya da 'n' tuşlarına basın.");
                }
            }
        }
    }
    static void CreatePdfFromImages(List<string> imagePaths, string outputDirectory)
    {
        PdfDocument pdf = new PdfDocument();
        foreach (var imagePath in imagePaths)
        {
            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XImage img = XImage.FromFile(imagePath);

            page.Width = img.PixelWidth * 72 / img.HorizontalResolution;
            page.Height = img.PixelHeight * 72 / img.VerticalResolution;

            gfx.DrawImage(img, 0, 0, page.Width, page.Height);
        }
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        string pdfPath = Path.Combine(outputDirectory, $"PdfFormat_{timestamp}.pdf");
        pdf.Save(pdfPath);
        Console.WriteLine($"PDF dosyası şu dizine kaydedildi: {pdfPath}");
    }
    static string CheckAndHandleExistingPath(string outputDirectory)
    {
        while (true)
        {
            try
            {
                // Geçerli bir dosya yolu mu kontrol edelim
                if (Path.HasExtension(outputDirectory))
                {
                    // Eğer dosya zaten varsa
                    if (File.Exists(outputDirectory))
                    {
                        Console.WriteLine($"Aynı isimde bir dosya mevcut: {outputDirectory}");
                        Console.WriteLine("Dosya zaten mevcut. Değişiklik yapmak istiyor musunuz? (Evet için 'y', Hayır için 'n')");
                        string userInput = Console.ReadLine().ToLower();

                        if (userInput == "y")
                        {
                            // Mevcut dosyayı kullan
                            return outputDirectory;
                        }
                        else
                        {
                            Console.WriteLine("Lütfen yeni bir dosya yolu girin:");
                            outputDirectory = Console.ReadLine();
                            continue;
                        }
                    }
                    else
                    {
                        // Dosya yoksa yeni dosya oluşturabiliriz
                        return outputDirectory;
                    }
                }
                else
                {
                    // Eğer geçerli bir dizinse
                    if (Directory.Exists(outputDirectory))
                    {
                        return outputDirectory;
                    }
                    else
                    {
                        // Geçerli bir dizin mi kontrol edelim
                        Console.WriteLine("Girilen dizin mevcut değil. Dosyanızı masaüstüne kaydetmek ister misiniz? (Evet için 'y', Hayır için 'n')");
                        string userInput = Console.ReadLine().ToLower();

                        if (userInput == "y")
                        {
                            // Masaüstüne kaydet
                            outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            Console.WriteLine($"Dosyalar masaüstüne kaydedilecek: {outputDirectory}");
                            return outputDirectory;
                        }
                        else if (userInput == "n")
                        {
                            // Tekrar geçerli bir dizin girmesini iste
                            Console.WriteLine("Lütfen geçerli bir dizin girin:");
                            outputDirectory = Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz giriş. Lütfen 'y' ya da 'n' tuşlarına basın.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}. Tekrar deneyin.");
                outputDirectory = Console.ReadLine(); // Kullanıcıdan tekrar yol girmesini iste
            }
        }
    }



    // Resim işlemlerini yapan metod
    static void ProcessImage(string imagePath, string outputDirectory, ref int folderCounter, string language, string tessdataPath)
    {
        try
        {
            Console.WriteLine($"İşlem başlatılıyor: {imagePath}");

            // Görüntüyü yükleyin ve gri tonlamaya çevirin
            var (image, warped) = LoadAndProcessImage(imagePath);

            if (image == null || warped == null)
            {
                Console.WriteLine($"Görüntü işlenemedi: {imagePath}");
                return;
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

            // Dosya yolu mu, dizin yolu mu olduğunu kontrol edelim
            if (Path.HasExtension(outputDirectory))
            {
                // Tek bir dosyaya OCR çıktısını ekleyelim
                AppendTextToFile(outputDirectory, ref folderCounter, imagePath, image, warped, language, tessdataPath);
            }
            else
            {
                // Dizin yoluna her görsel için ayrı klasör oluşturalım
                ProcessDirectory(outputDirectory, ref folderCounter, imagePath, image, warped, language, tessdataPath);
            }

            // Kaynakları serbest bırakma
            image.Dispose();
            warped.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            Console.WriteLine($"Hata detayı: {ex.StackTrace}");
        }

        
    }
    // Ortak görsel işleme işlemi
    static (Mat image, Mat warped) LoadAndProcessImage(string imagePath)
    {
            int j = 1;
            Console.WriteLine($"İşlem başlatılıyor: {imagePath}");

            // Görüntüyü yükleyin
            Mat image = Cv2.ImRead(imagePath, ImreadModes.Color);

            // Görüntüyü gri tonlamaya çevir
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            // Gürültüyü azaltmak için bulanıklaştır
            Mat blurred = new Mat();
            Cv2.GaussianBlur(gray, blurred, new Size(5, 5), 0);

            // Kenarları bulmak için Canny Edge Detection
            Mat edges = new Mat();
            Cv2.Canny(blurred, edges, 100, 20);

            // Konturları bulma
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(edges, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            var largestContour = contours.Where(c => Cv2.ContourArea(c) > 800).OrderByDescending(c => Cv2.ContourArea(c)).FirstOrDefault();

            if (largestContour != null && HasFourCorners(largestContour))
            {
                Console.WriteLine("Dört köşe bulundu, perspektif düzeltme yapılıyor...");
                var epsilon = 0.01 * Cv2.ArcLength(largestContour, true);
                var approx = Cv2.ApproxPolyDP(largestContour, epsilon, true);
                var points = approx.Select(p => new Point2f(p.X, p.Y)).ToArray();
                var rect = SortCorners(points);
                var maxWidth = (int)Math.Max(Distance(rect[2], rect[3]), Distance(rect[1], rect[0]));
                var maxHeight = (int)Math.Max(Distance(rect[1], rect[2]), Distance(rect[0], rect[3]));

                var destinationPoints = new[]
                {
                new Point2f(0, 0),
                new Point2f(maxWidth - 1, 0),
                new Point2f(maxWidth - 1, maxHeight - 1),
                new Point2f(0, maxHeight - 1)
            };

                var matrix = Cv2.GetPerspectiveTransform(rect, destinationPoints);
                Mat warped = new Mat();
                Cv2.WarpPerspective(image, warped, matrix, new Size(maxWidth, maxHeight));
                return (image, warped);
        }

        return (null, null);
    }
    // OCR işlemi ile metni dosyaya ekleme
    static void AppendTextToFile(string outputDirectory, ref int folderCounter, string imagePath, Mat image, Mat warped, string language, string tessdataPath)

    {
        string timestamp = "DateTime.Now:yyyyMMdd_HHmmss";
        int j = 1;
        // Ana klasör oluşturma
        string mainFolder = Path.Combine(Path.GetDirectoryName(outputDirectory), "ProcessedImages");
        Directory.CreateDirectory(mainFolder);

        // Her bir görsel için yeni bir klasör oluşturma
        string imageFolder = Path.Combine(mainFolder, $"image{folderCounter}_{timestamp}");
        Directory.CreateDirectory(imageFolder);
        folderCounter++; // Klasör sayacını artır

        // Görselleri kaydet
        string warpedImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_warped.png");
        string rawImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_raw.png");
        Cv2.ImWrite(warpedImagePath, warped);
        Cv2.ImWrite(rawImagePath, image);

        Console.WriteLine($"Görseller kaydedildi: {warpedImagePath}, {rawImagePath}");
        using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
        {
            using (var memoryStream = new MemoryStream())
            {
                Cv2.ImEncode(".png", warped, out byte[] imageBytes);
                memoryStream.Write(imageBytes, 0, imageBytes.Length);

                using (var img = Pix.LoadFromMemory(imageBytes))
                {
                    using (var page = engine.Process(img))
                    {
                        var extractedText = page.GetText();
                        Console.WriteLine("Çıkarılan Metin: " + extractedText);

                        // Mevcut dosyaya metin ekle
                        using (StreamWriter sw = new StreamWriter(outputDirectory, append: true))
                        {
                            sw.WriteLine($"Resim: {imagePath}");
                            sw.WriteLine($"İşlenme tarihi: {timestamp}");
                            sw.WriteLine("Çıkarılan metin:");
                            sw.WriteLine(extractedText);
                            sw.WriteLine("--------------"+j+"--------------");
                            j++;
                        }
                    }
                }
            }
        }
    }

    // Dizin içine her bir görsel için ayrı klasör ve txt dosyası oluşturma
    static void ProcessDirectory(string outputDirectory, ref int folderCounter, string imagePath, Mat image, Mat warped, string language, string tessdataPath)
    {
        string timestamp= "DateTime.Now:yyyyMMdd_HHmmss";
        // Ana klasör oluşturma
        string mainFolder = Path.Combine(outputDirectory, "ProcessedImages");
        Directory.CreateDirectory(mainFolder);

        string imageFolder = Path.Combine(mainFolder, $"image{folderCounter}_{timestamp}");
        Directory.CreateDirectory(imageFolder);
        folderCounter++;


        // Görselleri kaydet
        string warpedImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_warped.png");
        string rawImagePath = Path.Combine(imageFolder, $"{Path.GetFileNameWithoutExtension(imagePath)}_raw.png");
        Cv2.ImWrite(warpedImagePath, warped);
        Cv2.ImWrite(rawImagePath, image);

        Console.WriteLine($"Görseller kaydedildi: {warpedImagePath}, {rawImagePath}");

        // OCR ile metni çıkar ve dosyaya kaydet
        using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
        {
            using (var memoryStream = new MemoryStream())
            {
                Cv2.ImEncode(".png", warped, out byte[] imageBytes);
                memoryStream.Write(imageBytes, 0, imageBytes.Length);

                using (var img = Pix.LoadFromMemory(imageBytes))
                {
                    using (var page = engine.Process(img))
                    {
                        var extractedText = page.GetText();
                        Console.WriteLine("Çıkarılan Metin: " + extractedText);

                        // Her görsel için yeni bir txt dosyası oluştur ve kaydet
                        string textFilePath = Path.Combine(imageFolder, $"txt_{timestamp}.txt");
                        File.WriteAllText(textFilePath, extractedText);
                        Console.WriteLine($"Çıkarılan metin şu dosyaya kaydedildi: {textFilePath}");
                    }
                }
            }
        }
    }

    // Resim formatı kontrolü yapan metod
    static bool IsValidImageFile(string filePath)
    {
        string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif" };
        string fileExtension = Path.GetExtension(filePath).ToLower();
        return validExtensions.Contains(fileExtension);
    }

    static bool HasFourCorners(OpenCvSharp.Point[] contour)
    {
        var epsilon = 0.01 * Cv2.ArcLength(contour, true);
        var approx = Cv2.ApproxPolyDP(contour, epsilon, true);
        return approx.Length == 4;
    }

    static Point2f[] SortCorners(Point2f[] points)
    {
        Point2f[] rect = new Point2f[4];
        var sum = points.Select(p => p.X + p.Y).ToArray();
        var diff = points.Select(p => p.Y - p.X).ToArray();

        rect[0] = points[Array.IndexOf(sum, sum.Min())]; // sol üst
        rect[2] = points[Array.IndexOf(sum, sum.Max())]; // sağ alt
        rect[1] = points[Array.IndexOf(diff, diff.Min())]; // sağ üst
        rect[3] = points[Array.IndexOf(diff, diff.Max())]; // sol alt

        return rect;
    }

    static double Distance(Point2f p1, Point2f p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }
}*/