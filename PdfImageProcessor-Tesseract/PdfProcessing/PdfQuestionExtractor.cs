// PdfProcessing/PdfQuestionExtractor.cs
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Collections.Generic;

namespace PdfProcessing
{
    public static class PdfQuestionExtractor
    {
        public static List<QuestionRegion> ExtractQuestionRegions(string pdfPath)
        {
            List<QuestionRegion> questionRegions = new List<QuestionRegion>();

            using (PdfReader reader = new PdfReader(pdfPath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                int numPages = pdfDoc.GetNumberOfPages();

                for (int page = 1; page <= numPages; page++)
                {
                    PdfPage currentPdfPage = pdfDoc.GetPage(page);
                    float currentPageHeight = currentPdfPage.GetPageSize().GetHeight();

                    QuestionRegionEventListener listener = new QuestionRegionEventListener();
                    listener.SetCurrentPage(page, currentPageHeight);

                    PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
                    processor.ProcessPageContent(currentPdfPage);

                    questionRegions.AddRange(listener.QuestionRegions);
                }
            }

            return questionRegions;
        }
    }
}
