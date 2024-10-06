// PdfProcessing/QuestionRegionEventListener.cs
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PdfProcessing
{
    // Soru bölgelerini dinleyen ve çıkaran sınıf
    public class QuestionRegionEventListener : IEventListener
    {
        public List<QuestionRegion> QuestionRegions { get; } = new List<QuestionRegion>();

        // Metin ve sayıyla başlayan sorular için regexler
        private readonly Regex questionStartRegex = new Regex(@"\b[A-ZÖÇŞÜĞİ]+\s*\d+|^\d+\.", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private readonly Regex questionEndRegex = new Regex(@"E\)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        // Koordinatları doğrudan saklamak için değişkenler
        private float minX = float.MaxValue;
        private float minY = float.MaxValue;
        private float maxX = float.MinValue;
        private float maxY = float.MinValue;

        private bool isInQuestion = false;
        private int currentPage;

        // Atlanacak alanın üst sınırını tanımlayın 
        private const float SkipAreaHeight = 80.0f;

        // Sayfanın yüksekliğini saklamak için değişken
        private float pageHeight;

        // Mevcut sayfa numarasını ve sayfa yüksekliğini ayarlayan metod
        public void SetCurrentPage(int pageNumber, float currentPageHeight)
        {
            currentPage = pageNumber;
            pageHeight = currentPageHeight;

            // Soru durumunu sıfırla
            isInQuestion = false;

            // Bounding box'u sıfırla
            ResetBoundingBox();
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type != EventType.RENDER_TEXT)
                return;

            TextRenderInfo renderInfo = (TextRenderInfo)data;
            string text = renderInfo.GetText();

            if (string.IsNullOrWhiteSpace(text))
                return;

            text = text.Trim();

            // Metnin bulunduğu Y koordinatını al (başlangıç noktası)
            float currentY = renderInfo.GetBaseline().GetStartPoint().Get(1);

            // Atlanacak alanın üst sınırını hesapla
            float yThreshold = pageHeight - SkipAreaHeight;

            // Atlanacak alanın üstünde mı kontrol et (Y koordinatı yThreshold'tan büyük mü)
            if (currentY > yThreshold)
            {
                // Atlanacak alanın içinde, metni işlemeyin
                return;
            }

            // Atlanacak alanın altında, metni işlemeye devam et

            // Eğer yeni soru başlıyorsa ve önceki soru bitmişse, önceki soruyu kaydet
            if (questionStartRegex.IsMatch(text))
            {
                if (isInQuestion && questionEndRegex.IsMatch(text.Trim())) // Şıklar tamamlandıysa
                {
                    SaveCurrentQuestion();
                    isInQuestion = false;
                }

                if (!isInQuestion)
                {
                    isInQuestion = true;
                    ResetBoundingBox();
                    UpdateBoundingBox(renderInfo);
                }
            }
            else if (isInQuestion)
            {
                UpdateBoundingBox(renderInfo);

                // Eğer E şıkkı tespit edilirse soru bitiyor
                if (questionEndRegex.IsMatch(text.Trim()))
                {
                    SaveCurrentQuestion();
                    isInQuestion = false;
                }
            }
        }

        private void ResetBoundingBox()
        {
            minX = float.MaxValue;
            minY = float.MaxValue;
            maxX = float.MinValue;
            maxY = float.MinValue;
        }

        private void UpdateBoundingBox(TextRenderInfo renderInfo)
        {
            try
            {
                var descentLine = renderInfo.GetDescentLine();
                var ascentLine = renderInfo.GetAscentLine();

                minX = Math.Min(minX, descentLine.GetStartPoint().Get(0));
                minY = Math.Min(minY, descentLine.GetStartPoint().Get(1));
                maxX = Math.Max(maxX, ascentLine.GetEndPoint().Get(0));
                maxY = Math.Max(maxY, ascentLine.GetEndPoint().Get(1));
            }
            catch (Exception ex)
            {
                // Hata yönetimi yapılabilir
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Bounding box güncellenirken hata: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void SaveCurrentQuestion()
        {
            if (minX == float.MaxValue || minY == float.MaxValue || maxX == float.MinValue || maxY == float.MinValue)
                return;

            // Bounding box'u genişlik ve yükseklikle birlikte kaydet
            Rectangle rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            QuestionRegions.Add(new QuestionRegion
            {
                PageNumber = currentPage,
                BoundingBox = rect
            });
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return new HashSet<EventType> { EventType.RENDER_TEXT };
        }
    }
}

