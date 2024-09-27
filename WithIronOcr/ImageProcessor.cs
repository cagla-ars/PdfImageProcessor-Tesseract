/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace WithIronOcr
{
    internal class ImageProcessor
    {
        private readonly string imagePath;

        public ImageProcessor(string imagePath)
        {
            this.imagePath = imagePath;
        }

        public Mat ProcessImage()
        {
            // Görüntüyü yükle ve gri tonlamaya çevir
            Mat image = Cv2.ImRead(imagePath, ImreadModes.Color);
            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            // Gürültü azaltma
            Mat blurred = new Mat();
            Cv2.GaussianBlur(gray, blurred, new Size(5, 5), 0);

            // Kenar bulma
            Mat edges = new Mat();
            Cv2.Canny(blurred, edges, 100, 20);

            // Kontur bulma ve dört köşe kontrolü
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(edges, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
            var largestContour = contours.Where(c => Cv2.ContourArea(c) > 800).OrderByDescending(c => Cv2.ContourArea(c)).FirstOrDefault();

            if (largestContour != null && HasFourCorners(largestContour))
            {
                // Perspektif düzeltme işlemleri (detaylar yukarıdaki gibi)
                var points = Cv2.ApproxPolyDP(largestContour, 0.01 * Cv2.ArcLength(largestContour, true), true)
                                .Select(p => new Point2f(p.X, p.Y)).ToArray();
                var rect = SortCorners(points);
                var maxWidth = Math.Max(Distance(rect[2], rect[3]), Distance(rect[1], rect[0]));
                var maxHeight = Math.Max(Distance(rect[1], rect[2]), Distance(rect[0], rect[3]));

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
                return warped;
            }
            else
            {
                Console.WriteLine("Dört köşe bulunamadı.");
                return image;
            }
        }

        private bool HasFourCorners(OpenCvSharp.Point[] contour)
        {
            var epsilon = 0.01 * Cv2.ArcLength(contour, true);
            var approx = Cv2.ApproxPolyDP(contour, epsilon, true);
            return approx.Length == 4;
        }

        private Point2f[] SortCorners(Point2f[] points)
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

        private double Distance(Point2f p1, Point2f p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
*/