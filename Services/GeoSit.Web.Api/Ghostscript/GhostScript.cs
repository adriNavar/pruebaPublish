using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// required Ghostscript.NET namespaces
using Ghostscript.NET.Rasterizer;

namespace GeoSit.Web.Api.Ghostscript
{
    public class Ghostscript
    {
        public List<byte[]> ConvertPdfToImage(byte[] pdfBytes, int pages, ImageFormat imageFormat)
        {
            return ConvertPdfToImage(pdfBytes, pages, imageFormat, true, 0.25);
        }
        public List<byte[]> ConvertPdfToImage(byte[] pdfBytes, int pages, ImageFormat imageFormat, bool miniatura, double relacionAspecto)
        {
            List<byte[]> lista = new List<byte[]>();

            int desired_x_dpi = 96;
            int desired_y_dpi = 96;

            using (GhostscriptRasterizer _rasterizer = new GhostscriptRasterizer())
            using (MemoryStream source = new MemoryStream(pdfBytes))
            {
                _rasterizer.Open(source);
                int pagesToConvert = Math.Min(_rasterizer.PageCount, pages);
                for (int pageNumber = 1; pageNumber <= pagesToConvert; pageNumber++)
                {
                    Image img = _rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber);
                    if (miniatura)
                    {
                        img = img.GetThumbnailImage((int)Math.Round(img.Width * relacionAspecto), (int)Math.Round(img.Height * relacionAspecto), null, IntPtr.Zero);
                    }
                    using (MemoryStream destination = new MemoryStream())
                    using (img)
                    {
                        img.Save(destination, imageFormat);
                        lista.Add(destination.ToArray());
                    }
                }
                _rasterizer.Close();
                return lista;
            }
        }
    }
}