// PdfProcessing/QuestionRegion.cs
using iText.Kernel.Geom;

namespace PdfProcessing
{
    // Soru bölgelerini temsil eden sınıf
    public class QuestionRegion
    {
        public int PageNumber { get; set; }
        public Rectangle BoundingBox { get; set; }
    }
}
