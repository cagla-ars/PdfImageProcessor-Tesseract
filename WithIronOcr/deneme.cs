/*using OpenCvSharp;
using Tesseract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Görüntüyü yükleyin
        Mat image = Cv2.ImRead(@"C:\Users\COZUM\Desktop\WhatsApp Image 2024-09-11 at 16.58.35.jpeg");
        Mat originalImage = image.Clone();

        // Görüntüyü gri tonlamaya çevirin
        Mat gray = new Mat();
        Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

        // Geliþmiþ threshold iþlemi (daha doðru kenar tespiti için)
        Mat thresh = new Mat();
        Cv2.AdaptiveThreshold(gray, thresh, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 11, 2);

        // Tesseract ile metin bölgelerini tespit edip beyazla maskele
        var textRegions = GetTextRegions(image);
        foreach (var rect in textRegions)
        {
            Cv2.Rectangle(image, rect, Scalar.White, -1);  // Yazý bölgelerini beyaz ile maskele
        }

        // Kenar tespiti için Canny algoritmasý (daha iyi kenar algýlama için)
        Mat edges = new Mat();
        Cv2.Canny(image, edges, 100, 200);  // Canny kenar tespiti, eþik deðerleri ayarlandý

        // Dilate iþlemi: kenarlarý geniþletme (konturlarý daha belirgin hale getirmek için)
        Mat dilated = new Mat();
        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
        Cv2.Dilate(edges, dilated, kernel);

        // Konturlarý bulma
        OpenCvSharp.Point[][] contours;
        HierarchyIndex[] hierarchy;
        Cv2.FindContours(dilated, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        // Sadece büyük konturlarý iþleme (küçük konturlarý atla)
        foreach (var contour in contours)
        {
            var boundingBox = Cv2.BoundingRect(contour);

            // Daha hassas alan kontrolü: küçük konturlarý göz ardý et
            if (boundingBox.Width * boundingBox.Height > 5000)  // Daha büyük alanlarý hedefleyin
            {
                // Konturun metin bölgeleriyle kesiþip kesiþmediðini kontrol et
                bool isTextRegion = false;
                foreach (var textRect in textRegions)
                {
                    if (textRect.IntersectsWith(boundingBox))
                    {
                        isTextRegion = true;  // Eðer kesiþme varsa, bu kontur metin alanýdýr
                        break;
                    }
                }

                // Eðer kontur metin bölgesiyle kesiþmiyorsa kýrp
                if (!isTextRegion)
                {
                    Mat cropped = new Mat(originalImage, boundingBox);
                    Cv2.ImShow($"Kýrpýlmýþ Görüntü {boundingBox.X},{boundingBox.Y}", cropped);
                }
            }
        }

        // Ýþlenmiþ son görüntüyü ekranda göster
        Cv2.ImShow("Son Görüntü (Yazý Hariç)", image);
        Cv2.WaitKey(0);
    }

    // Tesseract ile metin bölgelerini tespit et
    public static List<OpenCvSharp.Rect> GetTextRegions(Mat image)
    {
        List<OpenCvSharp.Rect> textRegions = new List<OpenCvSharp.Rect>();

        using (var engine = new TesseractEngine(@"C:\Users\COZUM\WithIronOcr\WithIronOcr\tessdata\", "tur", EngineMode.Default))
        {
            // Tesseract PSM modunu 'PsmSingleBlock' olarak ayarlayalým (büyük bloklar)
            engine.DefaultPageSegMode = PageSegMode.SingleColumn;  // Single block yerine tek kolon olarak ayarla

            var bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                using (var pix = Pix.LoadFromMemory(stream.ToArray()))
                {
                    using (var page = engine.Process(pix))
                    {
                        var iterator = page.GetIterator();
                        iterator.Begin();
                        do
                        {
                            // Metin içeriði kontrolü: yalnýzca belirli karakter setlerini algýla
                            string wordText = iterator.GetText(PageIteratorLevel.Word);
                            if (IsValidText(wordText))
                            {
                                if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var bbox))
                                {
                                    textRegions.Add(new OpenCvSharp.Rect(bbox.X1, bbox.Y1, bbox.X2 - bbox.X1, bbox.Y2 - bbox.Y1));
                                }
                            }
                        } while (iterator.Next(PageIteratorLevel.Word));
                    }
                }
            }
        }

        return textRegions;
    }

    // Metnin geçerli olup olmadýðýný kontrol eden bir fonksiyon (sayýlar, özel karakterler, vb. dahil)
    public static bool IsValidText(string text)
    {
        // Tüm harfler, sayýlar ve yaygýn özel karakterleri dahil eden karakter kümesi
        string validCharacters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                 "!@#$%^&*()-_=+[]{};:'\"\\|,.<>?/`~";

        // Özel karakterleri, büyük/küçük harfleri ve sayýlarý kontrol et
        return !string.IsNullOrEmpty(text) && text.All(c => validCharacters.Contains(c));
    }
}*/
