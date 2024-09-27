/*using OpenCvSharp;
using System;
using System.IO;
using Tesseract;

namespace WithTesseract
{
    class Program2
    {
        static void Main(string[] args)
        {
            List<string> imagePaths = new List<string>();
            string outputDirectory = string.Empty;
            string tessdataPath = string.Empty;
            string language = string.Empty;

            while (true)
            {
                Console.WriteLine("Lütfen görsel dosyasının yolunu girin (çıkmak için boş bırakın):");
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
                else
                {
                    imagePaths.Add(input);
                }
            }

            // Çıktı dizini
            Console.WriteLine("Lütfen çıktı dosyalarının kaydedileceği dizini girin:");
            outputDirectory = Console.ReadLine().Replace("\"", "").Replace("'", "");

            // Dizinin varlığını kontrol et
            if (!Directory.Exists(outputDirectory))
            {
                Console.WriteLine("Girilen dizin mevcut değil. Çıkılıyor...");
                return;
            }

            // tessdata yolunu ayarla
            tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

            // Dil bilgisi
            Console.WriteLine("Lütfen kullanmak istediğiniz dil kodunu girin (örn. 'tur', 'eng'): ");
            language = Console.ReadLine().Replace("\"", "").Replace("'", "");

            string trainedDataPath = Path.Combine(tessdataPath, $"{language}.traineddata");
            if (!File.Exists(trainedDataPath))
            {
                Console.WriteLine($"{language}.traineddata dosyası bulunamadı.");
                return;
            }
            int fileIndex = 1; // Dosya isimlerini numaralandırmak için bir sayaç
            foreach (var imagePath in imagePaths)
            {
                try
                {
                    // 1. Görüntüyü yükleyin
                    Mat image = Cv2.ImRead(imagePath, ImreadModes.Color);

                    // 2. Çözünürlüğü artırma (örneğin 1.5 katına çıkarma)
                    Mat highResImage = new Mat();
                    Cv2.Resize(image, highResImage, new OpenCvSharp.Size(image.Width * 1.5, image.Height * 1.5), 0, 0, InterpolationFlags.Lanczos4);

                    // 3. Kenar tespiti ve döndürme açısı hesaplama
                    Mat edges = new Mat();
                    Cv2.Canny(highResImage, edges, 50, 150);

                    LineSegmentPolar[] lines = Cv2.HoughLines(edges, 1, Cv2.PI / 180, 100);
                    double angle = 0;

                    int numLines = lines.Length;



                    if (numLines == 0)
                    {
                        Console.WriteLine("Hiç çizgi tespit edilemedi. Döndürme yapılmayacak.");
                    }
                    else
                    {
                        // Ortalama döndürme açısını hesaplama
                        foreach (var line in lines)
                        {
                            angle += line.Theta;
                        }
                        angle /= numLines;

                        angle = (angle * 180 / Math.PI) - 90;
                        if (Math.Abs(angle) < 2)
                        {
                            angle = 0; // Eğer açı 2 dereceden küçükse döndürme yapılmaz
                        }
                    }

                    // 4. Döndürme işlemi (eğer açı varsa)
                    Mat rotatedImage = new Mat();
                    if (angle != 0)
                    {
                        Mat rotationMatrix = Cv2.GetRotationMatrix2D(new Point2f(highResImage.Width / 2, highResImage.Height / 2), angle, 1);
                        Cv2.WarpAffine(highResImage, rotatedImage, rotationMatrix, highResImage.Size());
                    }
                    else
                    {
                        rotatedImage = highResImage.Clone();
                    }

                    // 5. Görüntüyü gri tonlamaya çevirme
                    Mat grayImage = new Mat();
                    Cv2.CvtColor(rotatedImage, grayImage, ColorConversionCodes.BGR2GRAY);

                    // 6. Hafif kontrast artırma
                    Mat contrastedImage = new Mat();
                    Cv2.ConvertScaleAbs(grayImage, contrastedImage, 1.3, 10); // Hafif kontrast artırma

                    // 7. Thresholding (daha düşük threshold)
                    Mat thresholdImage = new Mat();
                    Cv2.Threshold(contrastedImage, thresholdImage, 120, 255, ThresholdTypes.Binary); // Daha düşük threshold değeri

                    // 8. İşlenmiş görüntüyü kaydet
                    string outputImagePath = Path.Combine(outputDirectory, $"processed{fileIndex}.png");
                    Cv2.ImWrite(outputImagePath, thresholdImage);

                    // Tesseract OCR işlemi
                    using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
                    {
                        
                        engine.DefaultPageSegMode = PageSegMode.Auto;
                        using (var img = Pix.LoadFromFile(outputImagePath))
                        {
                            using (var page = engine.Process(img))
                            {
                                var extractedText = page.GetText();
                                Console.WriteLine("Çıkarılan Metin:");
                                Console.WriteLine(extractedText);

                                // Çıkarılan metni bir dosyaya kaydet
                                string outputFilePath = Path.Combine(outputDirectory, $"txt{fileIndex}.txt");
                                File.WriteAllText(outputFilePath, extractedText);
                                Console.WriteLine($"Çıkarılan metin kaydedildi: {outputFilePath}");
                            }
                        }
                    }


                    // Kaynakları serbest bırakma
                    image.Dispose();
                    highResImage.Dispose();
                    rotatedImage.Dispose();
                    contrastedImage.Dispose();
                    thresholdImage.Dispose();
                    grayImage.Dispose();
                    edges.Dispose();

                    fileIndex++; // Dosya ismi için sayacı artır
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                }
            }
               
        }
    }
}
*/