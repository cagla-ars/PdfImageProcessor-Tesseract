// PdfProcessing/PdfCropper.cs
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfProcessing
{
    public static class PdfCropper
    {
        public static int CropQuestions(string inputPdfPath, string outputFolder, List<QuestionRegion> questionRegions)
        {
            int savedCount = 0;

            using (PdfReader reader = new PdfReader(inputPdfPath))
            using (PdfDocument inputPdf = new PdfDocument(reader))
            {
                foreach (var qr in questionRegions)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                    string outputFileName = $"sayfa{qr.PageNumber}_soru{savedCount + 1}_{timestamp}.pdf";
                    string outputPdfPath = System.IO.Path.Combine(outputFolder, outputFileName);

                    using (PdfWriter writer = new PdfWriter(outputPdfPath))
                    using (PdfDocument outputPdf = new PdfDocument(writer))
                    {
                        Rectangle cropBox = qr.BoundingBox;
                        PageSize pageSize = new PageSize(cropBox.GetWidth(), cropBox.GetHeight());

                        outputPdf.SetDefaultPageSize(pageSize);
                        PdfPage newPage = outputPdf.AddNewPage(pageSize);
                        PdfPage sourcePage = inputPdf.GetPage(qr.PageNumber);

                        PdfFormXObject pageCopy = sourcePage.CopyAsFormXObject(outputPdf);

                        PdfCanvas canvas = new PdfCanvas(newPage);
                        canvas.AddXObjectAt(pageCopy, -cropBox.GetX(), -cropBox.GetY());

                        outputPdf.Close();
                    }

                    savedCount++;
                }
            }

            return savedCount;
        }
    }
}
