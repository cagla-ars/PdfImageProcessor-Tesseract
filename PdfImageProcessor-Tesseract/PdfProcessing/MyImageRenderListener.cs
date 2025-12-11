// PdfProcessing/MyImageRenderListener.cs
// Bu sınıfı PdfProcessing klasörüne eklemeyi unutmayın
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Collections.Generic;

namespace PdfProcessing
{
    // Görsel konumlarını almak için dinleyici sınıfı
    public class MyImageRenderListener : IEventListener
    {
        private int pageNumber;
        public List<Rectangle> ImageCoordinates { get; private set; } = new List<Rectangle>();

        public MyImageRenderListener(int pageNumber)
        {
            this.pageNumber = pageNumber;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_IMAGE)
            {
                var renderInfo = (ImageRenderInfo)data;

                // Görselin konumunu al
                var rect = renderInfo.GetImageCtm();
                float x = rect.Get(Matrix.I31);
                float y = rect.Get(Matrix.I32);
                float width = rect.Get(Matrix.I11);
                float height = rect.Get(Matrix.I22);

                // Koordinatları kontrol et
                if (x < 0 || y < 0)
                {
                    // Negatif koordinatlar için görseli atla
                    return;
                }

                // Koordinatları listede sakla
                ImageCoordinates.Add(new Rectangle(x, y, width, height));
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return new List<EventType> { EventType.RENDER_IMAGE };
        }
    }
}

