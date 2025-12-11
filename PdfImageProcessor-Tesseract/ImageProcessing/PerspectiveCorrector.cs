using OpenCvSharp;
using System;
using System.Linq;

namespace ImageProcessing
{
    public class PerspectiveCorrector
    {
        public static (Mat image, Mat warped) LoadAndProcessImage(string imagePath)
        {

            Mat image = Cv2.ImRead(imagePath, ImreadModes.Color);
            if (image.Empty())
            {
                Console.WriteLine("Görüntü yüklenemedi.");
                return (null, null);
            }

         
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            Mat blurred = new Mat();
            Cv2.GaussianBlur(gray, blurred, new Size(5, 5), 0);

            Mat edges = new Mat();

    
            Cv2.Canny(blurred, edges, 20, 100);

     
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(edges, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

           
            var largestContour = contours
                .Where(c => Cv2.ContourArea(c) > 1500) 
                .OrderByDescending(c => Cv2.ContourArea(c))
                .FirstOrDefault();

            if (largestContour != null)
            {
                var epsilon = 0.02 * Cv2.ArcLength(largestContour, true);
                var approx = Cv2.ApproxPolyDP(largestContour, epsilon, true);

                
                if (approx.Length == 4)
                {
                    Console.WriteLine("Dört köşe bulundu, perspektif düzeltme yapılıyor...");

                    var points = approx.Select(p => new Point2f(p.X, p.Y)).ToArray();
                    var rect = SortCorners(points);

                    int maxWidth = (int)Math.Max(Distance(rect[2], rect[3]), Distance(rect[1], rect[0]));
                    int maxHeight = (int)Math.Max(Distance(rect[1], rect[2]), Distance(rect[0], rect[3]));

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
            }

            Console.WriteLine("Dört köşe bulunamadı, perspektif düzeltme yapılmadı. Orijinal resim ile devam ediliyor.");
            return (image, null); 
        }

        private static Point2f[] SortCorners(Point2f[] points)
        {
            Point2f[] rect = new Point2f[4];
            var sum = points.Select(p => p.X + p.Y).ToArray();
            var diff = points.Select(p => p.Y - p.X).ToArray();

            rect[0] = points[Array.IndexOf(sum, sum.Min())];
            rect[2] = points[Array.IndexOf(sum, sum.Max())]; 
            rect[1] = points[Array.IndexOf(diff, diff.Min())]; 
            rect[3] = points[Array.IndexOf(diff, diff.Max())]; 

            return rect;
        }

        private static double Distance(Point2f p1, Point2f p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
