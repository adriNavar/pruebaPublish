using System;
using System.Collections.Generic;
using System.Linq;
using it = iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing;
using Microsoft.Win32;
using System.Drawing.Imaging;

public static class PDFUtilities
{
    public static PdfPCell GetCellForTable(string texto, it.Font pdfFont, int align, float espaciado, bool drawBorder)
    {
        it.Phrase phrase = new it.Phrase(texto, pdfFont);
        PdfPCell cell = new PdfPCell(phrase)
        {
            PaddingTop = espaciado / 2,
            PaddingBottom = espaciado / 2,
            PaddingLeft = 2f,
            PaddingRight = 2f,
            HorizontalAlignment = align,
            VerticalAlignment = PdfPCell.ALIGN_MIDDLE
        };

        if (!drawBorder)
        {
            cell.BorderWidth = PdfPCell.NO_BORDER;
        }
        return cell;
    }

    public static void DrawPDFImage(PdfContentByte pdfContentByte, Image image, float x1Pdf, float y1Pdf, float anchoPts, float altoPts)
    {
        DrawPDFImage(pdfContentByte, image, x1Pdf, y1Pdf, anchoPts, altoPts, 0);
    }
    public static void DrawPDFImage(PdfContentByte pdfContentByte, Image image, float x1Pdf, float y1Pdf, float anchoPts, float altoPts, float anguloRotacion)
    {
        pdfContentByte.SaveState();
        it.Image itImage = it.Image.GetInstance(image, ImageFormat.Png);
        itImage.ScaleAbsolute(anchoPts, altoPts);
        itImage.SetAbsolutePosition(x1Pdf, y1Pdf);
        if (anguloRotacion != 0)
        {
            itImage.Rotation = anguloRotacion;
            //itImage.RotationDegrees = anguloRotracionGrados;
        }
        pdfContentByte.AddImage(itImage);
        //pdfDoc.Add(itImage);
        pdfContentByte.RestoreState();

    }

    public static void DrawPDFCircle(PdfContentByte pdfContentByte, float x, float y, float radio, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor)
    {
        pdfContentByte.SaveState();

        SetContentByteState(pdfContentByte, pdfContornoColor, contornoGrosor, pdfRellenoColor);

        pdfContentByte.Circle(x, y, radio);
        if (pdfRellenoColor != null || pdfRellenoColor != null)
        {
            pdfContentByte.FillStroke();
        }
        else
        {
            pdfContentByte.Stroke();
        }
        pdfContentByte.RestoreState();
    }

    public static void DrawPDFRectangle(PdfContentByte pdfContentByte, float x1, float y1, float anchoPts, float altoPts, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor)
    {
        pdfContentByte.SaveState();

        SetContentByteState(pdfContentByte, pdfContornoColor, contornoGrosor, pdfRellenoColor);

        pdfContentByte.MoveTo(x1, y1);
        pdfContentByte.LineTo(x1 + anchoPts, y1);
        pdfContentByte.LineTo(x1 + anchoPts, y1 + altoPts);
        pdfContentByte.LineTo(x1, y1 + altoPts);
        if (pdfRellenoColor != null || pdfRellenoColor != null)
        {
            pdfContentByte.ClosePathFillStroke();
        }
        else
        {
            pdfContentByte.ClosePathStroke();
        }
        pdfContentByte.RestoreState();
    }

    public static void DrawPDFRectangle2(PdfContentByte pdfContentByte, float x1, float y1, float x2, float y2, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor, PdfPatternPainter pdfPatternPainter)
    {
        DrawPDFRectangle2(pdfContentByte, x1, y1, x2, y2, pdfContornoColor, contornoGrosor, pdfRellenoColor, pdfPatternPainter, null);
    }
    public static void DrawPDFRectangle2(PdfContentByte pdfContentByte, float x1, float y1, float x2, float y2, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor, PdfPatternPainter pdfPatternPainter, string lineDash)
    {
        pdfContentByte.SaveState();

        SetContentByteState(pdfContentByte, pdfContornoColor, contornoGrosor, pdfRellenoColor);

        if (!string.IsNullOrEmpty(lineDash))
        {
            float[] dash = Array.ConvertAll(lineDash.Split(','), new Converter<string, float>(float.Parse));
            pdfContentByte.SetLineDash(dash, 0);
        }

        pdfContentByte.MoveTo(x1, y1);
        pdfContentByte.LineTo(x2, y1);
        pdfContentByte.LineTo(x2, y2);
        pdfContentByte.LineTo(x1, y2);
        if (pdfRellenoColor != null)
        {
            if (pdfPatternPainter != null)
            {
                pdfContentByte.SetPatternFill(pdfPatternPainter, pdfRellenoColor);
            }
            pdfContentByte.ClosePathFillStroke();
        }
        else
        {
            pdfContentByte.ClosePathStroke();
        }
        pdfContentByte.RestoreState();
    }
    public static void DrawPDFPolygon(PdfContentByte pdfContentByte, List<PointF> lstPointF, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor)
    {
        pdfContentByte.SaveState();

        SetContentByteState(pdfContentByte, pdfContornoColor, contornoGrosor, pdfRellenoColor);

        pdfContentByte.MoveTo(lstPointF[0].X, lstPointF[0].Y);
        for (int i = 1; i < lstPointF.Count; i++)
        {
            pdfContentByte.LineTo(lstPointF[i].X, lstPointF[i].Y);
        }
        if (pdfRellenoColor != null)
        {
            pdfContentByte.ClosePathFillStroke();
        }
        else
        {
            pdfContentByte.ClosePathStroke();
        }
        pdfContentByte.RestoreState();
    }

    public static void DrawPDFPolygon(PdfContentByte pdfContentByte, List<Ring> lstRing, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor)
    {
        DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, contornoGrosor, pdfRellenoColor, null, null);
    }
    public static void DrawPDFPolygon(PdfContentByte pdfContentByte, List<Ring> lstRing, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor, PdfPatternPainter pdfPatternPainter, string lineDash)
    {
        pdfContentByte.SaveState();

        SetContentByteState(pdfContentByte, pdfContornoColor, contornoGrosor, pdfRellenoColor);

        if (!string.IsNullOrEmpty(lineDash))
        {
            float[] dash = Array.ConvertAll(lineDash.Split(','), new Converter<string, float>(float.Parse));
            pdfContentByte.SetLineDash(dash, 0);
        }
        foreach (Ring ring in lstRing)
        {
            var puntos = ring.Puntos;
            pdfContentByte.MoveTo(puntos[0].X, puntos[0].Y);
            for (int i = 1; i < puntos.Count; i++)
            {
                pdfContentByte.LineTo(puntos[i].X, puntos[i].Y);
            }
        }
        if (pdfRellenoColor != null)
        {
            if (pdfPatternPainter != null)
            {
                pdfContentByte.SetPatternFill(pdfPatternPainter, pdfRellenoColor);
            }
            pdfContentByte.ClosePathEoFillStroke();
        }
        else
        {
            pdfContentByte.ClosePathStroke();
        }
        pdfContentByte.RestoreState();
    }

    public static void DrawPDFLine(PdfContentByte pdfContentByte, float x1, float y1, float x2, float y2, float lineWidth, it.BaseColor color)
    {
        DrawPDFLine(pdfContentByte, x1, y1, x2, y2, lineWidth, color, string.Empty);
    }
    public static void DrawPDFLine(PdfContentByte pdfContentByte, float x1, float y1, float x2, float y2, float lineWidth, it.BaseColor color, string lineDash)
    {
        //pdfContentByte.SetColorStroke(color);
        //pdfContentByte.SetLineWidth(lineWidth);
        //if (!string.IsNullOrEmpty(lineDash))
        //{
        //    float[] dash = Array.ConvertAll(lineDash.Split(','), new Converter<string, float>(float.Parse));
        //    pdfContentByte.SetLineDash(dash, 0);
        //}
        //pdfContentByte.MoveTo(x1, y1);
        //pdfContentByte.LineTo(x2, y2);
        //pdfContentByte.Stroke();
        var lstRing = new[] { new Ring { Puntos = new[] { new PointF(x1, y1), new PointF(x2, y2) }.ToList() } }.ToList();
        DrawPDFLine(pdfContentByte, lstRing, lineWidth, color, lineDash);
    }

    public static void DrawPDFLine(PdfContentByte pdfContentByte, List<Ring> lstRing, float lineWidth, it.BaseColor color, string lineDash)
    {
        pdfContentByte.SaveState();
        SetContentByteState(pdfContentByte, color, lineWidth, null);
        if (!string.IsNullOrEmpty(lineDash))
        {
            float[] dash = Array.ConvertAll(lineDash.Split(','), new Converter<string, float>(float.Parse));
            pdfContentByte.SetLineDash(dash, 0);
        }
        foreach (Ring ring in lstRing)
        {
            var puntos = ring.Puntos;
            pdfContentByte.MoveTo(puntos[0].X, puntos[0].Y);
            for (int i = 1; i < puntos.Count; i++)
            {
                pdfContentByte.LineTo(puntos[i].X, puntos[i].Y);
            }
        }
        pdfContentByte.Stroke();
        pdfContentByte.RestoreState();
    }

    public static void DrawPDFText(PdfContentByte pdfContentByte, string texto, float x, float y, it.Font pdfFont, BaseFont baseFont, float size, Color color, int alignment)
    {
        float sizeText = pdfFont.BaseFont.GetWidthPoint(texto + "xx", size);
        ColumnText ct = new ColumnText(pdfContentByte);
        it.Phrase phrase = new it.Phrase(texto, pdfFont);
        ct.SetSimpleColumn(phrase, x, y - size, x + sizeText, y, 0, alignment);
        ct.Go();
    }
    public static void DrawPDFText(PdfContentByte pdfContentByte, string texto, float x, float y, it.Font pdfFont, float size, int alignment)
    {
        float sizeText = pdfFont.BaseFont.GetWidthPoint(texto + "xx", size);
        ColumnText ct = new ColumnText(pdfContentByte);
        it.Phrase phrase = new it.Phrase(texto, pdfFont);

        ct.SetSimpleColumn(phrase, x, y - size, x + sizeText, y, 0, alignment);
        ct.Go();
    }
    public static void DrawPDFText(PdfContentByte pdfContentByte, string texto, float x, float y, BaseFont baseFont, float size, Color color, double angulo)
    {
        pdfContentByte.BeginText();
        pdfContentByte.SetFontAndSize(baseFont, size);
        pdfContentByte.SetRGBColorFill(color.R, color.G, color.B);
        pdfContentByte.SetRGBColorStroke(color.R, color.G, color.B);

        int cont = 0;
        foreach (var line in texto.Split('\n'))
        {
            x = angulo >= 45 ? x + cont * size : x;
            y = angulo < 45 ? y - cont * size : y;
            pdfContentByte.ShowTextAligned(it.Element.ALIGN_CENTER, line, x, y, (float)angulo);
            cont++;
        }
        pdfContentByte.EndText();
    }
    public static void DrawPDFTextMax(PdfContentByte pdfContentByte, string texto, float x, ref float y, float xMax, it.Font pdfFont, float fontSize, int alignment)
    {
        float widthTexto = pdfFont.BaseFont.GetWidthPoint(texto + "xx", fontSize);
        float cantRenglones = (widthTexto > (xMax - x) ? (float)Math.Ceiling(widthTexto / (xMax - x)) : 1);
        float llx = x;
        float ury = y + fontSize;
        float urx = xMax;
        //le sumo el 1% del fontSize porque en AYSA no salia el texto porque el fontSize daba mas grande que el alto del renglon
        float lly = (float)ury - (cantRenglones * fontSize + (fontSize * 1 / 100));
        ColumnText ct = new ColumnText(pdfContentByte);
        ct.SetSimpleColumn(new it.Phrase(texto, pdfFont), llx, lly, urx, ury, fontSize, alignment);
        ct.Go();
    }


    public static void DrawPDFTextMaxSteel(PdfContentByte pdfContentByte, string texto, float x, ref float y, float xMax, it.Font pdfFont, float fontSize, int alignment, int maxRenglones = 99)
    {
        pdfFont.Size = fontSize;
        it.Phrase phrase = new it.Phrase(texto, pdfFont);

        float widthTexto = phrase[0].Chunks[0].GetWidthPoint() + phrase[0].Chunks[0].GetWidthPoint() * 5 / 100;
        float cantRenglones = (widthTexto > (xMax - x) ? (float)Math.Ceiling(widthTexto / (xMax - x)) : 1);

        cantRenglones = Math.Max(maxRenglones, cantRenglones);
        float llx = x;
        float ury = y + fontSize;
        float urx = xMax;

        float lly = (float)ury - (cantRenglones * (fontSize + 3F));

        ColumnText ct = new ColumnText(pdfContentByte);
        ct.SetSimpleColumn(phrase, llx, lly, urx, ury, fontSize, alignment);
        ct.Go();

        y = cantRenglones;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="phrase">Texto con font</param>
    /// <param name="xMax">Xmax del rotulo</param>
    /// <param name="x">Donde empieza a dibujar el texto</param>
    private static int PruebaCalcularRenglones(string texto, it.Font formato, float xMax, float x)
    {

        it.Phrase phrase = new it.Phrase(texto, formato);

        float cantRenglones = 0;

        float tamañoTexto = phrase[0].Chunks[0].GetWidthPoint();

        float tamañoRenglon = xMax - x;

        if (tamañoTexto < tamañoRenglon)
        {
            cantRenglones = 0;
        }
        else
        {
            var palabras = phrase[0].Chunks[0].Content.Split(' ');

            float calcularRenglon = 0;

            List<string> textoXrenglon = new List<string> { ":" };
            for (int i = 0; i < palabras.Count(); i++)
            {
                it.Phrase phrasePalabra = new it.Phrase(palabras[i] + " ", formato);
                float tamañoPalabra = phrasePalabra[0].Chunks[0].GetWidthPoint();

                if (calcularRenglon + tamañoPalabra < tamañoRenglon)
                {
                    calcularRenglon += tamañoPalabra;
                    textoXrenglon[textoXrenglon.Count - 1] += palabras[i] + " ";
                }
                else
                {//LO CALCULA SIN ESPACIO Y SE FIJA SI ENTRA EN EL RENGLON.
                    phrasePalabra = new it.Phrase(palabras[i], formato);
                    tamañoPalabra = phrasePalabra[0].Chunks[0].GetWidthPoint();

                    if (calcularRenglon + tamañoPalabra < tamañoRenglon)
                    {
                        calcularRenglon += tamañoPalabra;
                        textoXrenglon[textoXrenglon.Count - 1] += palabras[i];
                    }
                    else
                    {
                        cantRenglones++;
                        calcularRenglon = 0;
                        i--;
                        textoXrenglon.Add("");
                    }
                }
            }
        }//funciona a la perfeccion

        return (int)cantRenglones + 1;//+1 xq empieza en 0;
    }

    public static void RegisterBaseFont(string fontName, float fontSize)
    {
        if (!it.FontFactory.IsRegistered(fontName))
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                GetSystemFontFileName(new Font(new FontFamily(fontName), fontSize)));
            it.FontFactory.Register(path);
        }
    }

    public static string GetSystemFontFileName(Font font)
    {
        string fontname = $"{font.Name} (TrueType)";
        RegistryKey fonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts", false)
                            ?? Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Fonts", false);
        if (fonts != null)
        {
            return
                (from fntkey in fonts.GetValueNames()
                 where fntkey == fontname
                 select fonts.GetValue(fntkey).ToString())
                    .FirstOrDefault();
        }
        else
        {
            //throw new Exception("Can't find font registry database.");
            return null;
        }
    }

    private static void SetContentByteState(PdfContentByte pdfContentByte, it.BaseColor pdfContornoColor, float contornoGrosor, it.BaseColor pdfRellenoColor)
    {
        pdfContentByte.SetGState(new PdfGState() { FillOpacity = pdfContornoColor.A / 255f });
        if (pdfRellenoColor != null)
        {
            pdfContentByte.SetColorFill(pdfRellenoColor);
        }
        if (contornoGrosor > 0)
        {
            pdfContentByte.SetLineWidth(contornoGrosor);
        }
        if (pdfContornoColor != null)
        {
            pdfContentByte.SetColorStroke(pdfContornoColor);
        }
    }
}