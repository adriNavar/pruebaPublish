using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Repositories;
using MPFuncionesEspeciales;

namespace GeoSit.Web.Api.Ploteo
{
    public class ModPlotGraphics
    {
        private readonly IPlantillaRepository _plantillaRepository;
        private readonly IPlantillaFondoRepository _plantillaFondoRepository;
        private readonly ILayerGrafRepository _layerGrafRepository;
        private readonly IHojaRepository _hojaRepository;
        private readonly INorteRepository _norteRepository;
        private readonly IParcelaPlotRepository _parcelaPlotRepository;

        public ModPlotGraphics(IPlantillaRepository plantillaRepository, ILayerGrafRepository layerGrafRepository, IPlantillaFondoRepository plantillaFondoRepository, IHojaRepository hojaRepository, INorteRepository norteRepository, IParcelaPlotRepository parcelaPlotRepository)
        {
            _plantillaRepository = plantillaRepository;
            _layerGrafRepository = layerGrafRepository;
            _plantillaFondoRepository = plantillaFondoRepository;
            _hojaRepository = hojaRepository;
            _norteRepository = norteRepository;
            _parcelaPlotRepository = parcelaPlotRepository;
        }

        #region STRUCTS
        public struct PointD
        {
            public double X;
            public double Y;
            public PointD(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        public struct Segmento
        {
            public PointD P1;
            public PointD P2;
            public double Distancia;
            public double Angulo;
            public Segmento(PointD p1, PointD p2, double distancia, double angulo)
            {
                this.P1 = p1;
                this.P2 = p2;
                this.Distancia = distancia;
                this.Angulo = angulo;
            }
        }
        public struct Lado
        {
            public List<Segmento> Segmentos;
            public double Distancia;
            public double Angulo;
            public Lado(List<Segmento> Segmentos, double distancia, double angulo)
            {
                this.Segmentos = Segmentos;
                this.Distancia = distancia;
                this.Angulo = angulo;
            }
        }
        #endregion

        public byte[] GetPlantilla(int idPlantilla, string idObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables)
        {
            byte[] bImage = null;

            //var imagTest = Image.FromFile(@"C:\Temp\PlantillaA0_gimp01.png");

            //string[] aIdObjetoSecundario = idsObjetoSecundario.Split(',');

            Dictionary<string, string> dicTextosVariables = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(textosVariables))
            {
                dicTextosVariables = textosVariables.ToUpper().Split(';').Select(x => x.Split(',')).ToDictionary(x => x[0], x => x[1]);
            }
            Plantilla plantilla = GetPlantillaById(idPlantilla);

            //PlantillaFondo plantillaFondo = plantilla.PlantillaFondos.First(p => p.IdPlantillaFondo == idPlantillaFondo);
            PlantillaFondo plantillaFondo = GetPlantillaFondoById(idPlantillaFondo);
            if (plantilla != null && plantillaFondo != null)
            {
                Hoja hoja = GetHojaById(plantilla.IdHoja);
                plantilla.PPP = plantillaFondo.Resolucion.Valor;
                int anchoPx = Convert.ToInt32(((hoja.Ancho_mm / 10.0) / 2.54) * plantilla.PPP);
                int altoPx = Convert.ToInt32(((hoja.Alto_mm / 10.0) / 2.54) * plantilla.PPP);
                if (plantilla.Orientacion == 0)
                {
                    plantilla.Width = anchoPx;
                    plantilla.Heigth = altoPx;
                }
                else
                {
                    plantilla.Width = altoPx;
                    plantilla.Heigth = anchoPx;
                }
                //paso a metros: 1 pulgada = 2.54cm => 1 pixel = (2.54 cm / 300) 
                double resolucion_m = (2.54 / plantilla.PPP) / 100.0;
                //Resolución del dibujo ppp pasado a metros (creo q no se esta utilizando)
                plantilla.Resolucion = resolucion_m;

                double x = 0, y = 0;
                int x1c = 0, y1c = 0;
                int x2c = 0, y2c = 0;
                //double xMidBase = 0, yMidBase = 0;
                double xCentroidBase = 0, yCentroidBase = 0;
                double escala = 0, factorEscala = 0;
                int widthNewImg = 0, heightNewImg = 0;
                double xMinBuff = 9999999, yMinBuff = 9999999, xMaxBuff = 0, yMaxBuff = 0;
                //double xMinBuffForScale = 9999999, yMinBuffForScale = 9999999, xMaxBuffForScale = 0, yMaxBuffForScale = 0;
                //long featIdManzana = 958670;

                using (var image = plantillaFondo.ImagenImage)
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        //graphics.Clear(Color.White);

                        #region Codigo comentado - TEST - Dibujo limites parte util
                        //using (Pen penLimites = new Pen(Color.Black, 10))
                        //{

                        //    //int xxMin = (int)(plantilla.XMin * (plantilla.PPP / 25.4));
                        //    int xxMin = GetPixels(plantilla.XMin, plantilla.PPP);
                        //    int yyMin = GetPixels(plantilla.YMin, plantilla.PPP);
                        //    int xxMax = GetPixels(plantilla.XMax, plantilla.PPP);
                        //    int yyMax = GetPixels(plantilla.YMax, plantilla.PPP);
                        //    graphics.DrawLine(new Pen(Color.Black, 10), xxMin, yyMin, xxMax, yyMin);
                        //    graphics.DrawLine(new Pen(Color.Black, 10), xxMax, yyMin, xxMax, yyMax);
                        //    graphics.DrawLine(new Pen(Color.Black, 10), xxMax, yyMax, xxMin, yyMax);
                        //    graphics.DrawLine(new Pen(Color.Black, 10), xxMin, yyMax, xxMin, yyMin);
                        //}
                        #endregion

                        //Layer Base
                        LayerGraf layerGrafBase = null;
                        LayerGraf[] aLayerGrafBase = null;
                        
                        DbGeometry geometryBase = null;
                        if (layerGrafBase != null)
                        {
                            geometryBase = layerGrafBase.Geom;
                            xCentroidBase = (double)geometryBase.Centroid.XCoordinate;
                            yCentroidBase = (double)geometryBase.Centroid.YCoordinate;

                            #region Codigo Comentado - Extension de Manzana y Centro calculado que no se esta usando, se usa en vez xCentroidMnz yCentroidMnz
                            //Extension de Manzana y Centro calculado que no se esta usando, se usa en vez xCentroidMnz yCentroidMnz
                            //int cantCoords = (int)geometryBase.PointCount;
                            //for (int i = 1; i <= cantCoords; i++)
                            //{
                            //    x = (double)geometryBase.PointAt(i).XCoordinate;
                            //    //a_Limite(i) := wx;
                            //    //i := i + 1;
                            //    y = (double)geometryBase.PointAt(i).YCoordinate;
                            //    //a_Limite(i) := wy;
                            //    if (i > 0)
                            //    {
                            //        x1 = x2;
                            //        y1 = y2;
                            //        x2 = x;
                            //        y2 = y;
                            //    }
                            //    else
                            //    {
                            //        x1 = x;
                            //        y1 = y;
                            //        x2 = x;
                            //        y2 = y;
                            //    }
                            //    if (x <= xMin)
                            //    {
                            //        xMin = x;
                            //    }
                            //    if (x >= xMax)
                            //    {
                            //        xMax = x;
                            //    }
                            //    if (y <= yMin)
                            //    {
                            //        yMin = y;
                            //    }
                            //    if (y >= yMax)
                            //    {
                            //        yMax = y;
                            //    }
                            //}
                            //xMidBase = xMin + (xMax - xMin) / 2;
                            //yMidBase = yMin + (yMax - yMin) / 2;
                            #endregion
                        }
                        //Buffer
                        DbGeometry bufferBase = layerGrafBase.Geom.Buffer(plantilla.DistBuffer);
                        #region Codigo Comentado - TEST dibujo el buffer para ver como queda
                        //LayerGraf[] aLayerGrafBuff = new LayerGraf[1];
                        //LayerGraf layerGrafBuff = new LayerGraf();
                        //layerGrafBuff.Geom = bufferBase;
                        //layerGrafBuff.Nombre = string.Empty;
                        //aLayerGrafBuff[0] = layerGrafBuff;
                        //var penBuffer = new Pen(Color.Black, 12);
                        //Brush brushBuffer = Brushes.Black;
                        //DibujarLayerGraf<LayerGraf>(graphics, aLayerGrafBuff, fontMejoras, penBuffer, brushBuffer, xCentroidMnz, yCentroidMnz, escala, x_Centro, y_Centro, xMinPlantilla, yMaxPlantilla, widthUtilPlantilla, consMult, false);
                        //Comentar hasta aca el dibujo del buffer
                        #endregion

                        #region Extents del Buffer
                        int cantCoordsBuff = (int)bufferBase.PointCount;
                        for (int i = 1; i <= cantCoordsBuff; i++)
                        {
                            x = (double)bufferBase.PointAt(i).XCoordinate;
                            y = (double)bufferBase.PointAt(i).YCoordinate;
                            if (x <= xMinBuff)
                            {
                                xMinBuff = x;
                            }
                            if (x >= xMaxBuff)
                            {
                                xMaxBuff = x;
                            }
                            if (y <= yMinBuff)
                            {
                                yMinBuff = y;
                            }
                            if (y >= yMaxBuff)
                            {
                                yMaxBuff = y;
                            }
                        }
                        #endregion

                        //DbGeometry bufferForScale = layerGrafBase.Geom.Buffer(plantilla.DistBuffer/2);

                        //Calculo la escala en base a los minimos y maximos del buffer en vez de la manzana
                        #region Calculo de Escala
                        //if (((plantilla.X_Util / 1000) * (yMaxBuff - yMinBuff)) > ((plantilla.Y_Util / 1000) * (xMaxBuff - xMinBuff)))
                        if ((yMaxBuff - yMinBuff) > (xMaxBuff - xMinBuff))
                        {
                            factorEscala = (plantilla.Y_Util / 1000) / (yMaxBuff - yMinBuff);
                        }
                        else
                        {
                            factorEscala = (plantilla.X_Util / 1000) / (xMaxBuff - xMinBuff);
                        }
                        escala = 1 / factorEscala;

                        int escalaPlantilla = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).OrderBy(p => p.Escala).Select(p => p.Escala).First();
                        factorEscala = 1.0 / escalaPlantilla;
                        escala = 1 / factorEscala;
                        //escala = 1 / escala;
                        #endregion

                        #region Rectangulo del buffer en coordenadas canvas
                        x1c = GetXCanvas(xMinBuff, xCentroidBase, factorEscala, plantilla);
                        y1c = GetYCanvas(yMinBuff, yCentroidBase, factorEscala, plantilla);
                        x2c = GetXCanvas(xMaxBuff, xCentroidBase, factorEscala, plantilla);
                        y2c = GetYCanvas(yMaxBuff, yCentroidBase, factorEscala, plantilla);

                        widthNewImg = Math.Abs(x2c - x1c);
                        heightNewImg = Math.Abs(y2c - y1c);
                        //For Testing - Dibujo el xy min max del del buffer para ver si estan bien - comentar
                        //graphics.DrawLine(new Pen(Color.Black, 10), 0, 0, x1c, y1c);
                        //graphics.DrawLine(new Pen(Color.Violet, 10), 0, 0, x2c, y2c);
                        //image.Save(@"C:\Temp\ManzaneroBuff.jpg", ImageFormat.Jpeg);
                        #endregion
                        Rectangle bufferRectangle = new Rectangle(x1c, y2c, widthNewImg, heightNewImg);
                        //graphics.DrawRectangle(new Pen(Color.Red, 10), bufferRectangle);

                        //Seteo los limites de la region en donde se va a poder dibujar (Dentro del buffer)
                        graphics.SetClip(bufferRectangle);

                        double anguloRotacion = 0;
                        bool sentidoHorario = true;
                        if (plantilla.OptimizarTamanioHoja)
                        {
                            #region OptimizarTamanioHoja
                            PointF puntoMedio = new PointF();
                            List<Lado> lados = new List<Lado>();
                            Lado ladoMayor = new Lado();
                            anguloRotacion = GetAnguloRotacion(geometryBase, ref puntoMedio, ref lados, ref ladoMayor);
                            //anguloRotacion = (float)((180 - anguloRotacion) * Math.PI / 180.0);
                            sentidoHorario = true;

                            Segmento segmentoMayor = new Segmento();
                            double distSegmentoMayor = -9999;
                            foreach (var segmento in ladoMayor.Segmentos)
                            {
                                if (segmento.Distancia > distSegmentoMayor)
                                {
                                    distSegmentoMayor = segmento.Distancia;
                                    segmentoMayor = segmento;
                                }
                            }
                            //int x1SegmentoMayor = GetXCanvas(segmentoMayor.P1.X, xCentroidBase, factorEscala, plantilla);
                            //int y1SegmentoMayor = GetYCanvas(segmentoMayor.P1.Y, yCentroidBase, factorEscala, plantilla);
                            //int x2SegmentoMayor = GetXCanvas(segmentoMayor.P2.X, xCentroidBase, factorEscala, plantilla);
                            //int y2SegmentoMayor = GetYCanvas(segmentoMayor.P2.Y, yCentroidBase, factorEscala, plantilla);
                            //graphics.DrawLine(new Pen(Color.Black, 10), x1SegmentoMayor, y1SegmentoMayor, x2SegmentoMayor, y2SegmentoMayor);
                            int x1SegmentoMayor = GetXCanvas(ladoMayor.Segmentos[0].P1.X, xCentroidBase, factorEscala, plantilla);
                            int y1SegmentoMayor = GetYCanvas(ladoMayor.Segmentos[0].P1.Y, yCentroidBase, factorEscala, plantilla);
                            int x2SegmentoMayor = GetXCanvas(ladoMayor.Segmentos[0].P2.X, xCentroidBase, factorEscala, plantilla);
                            int y2SegmentoMayor = GetYCanvas(ladoMayor.Segmentos[0].P2.Y, yCentroidBase, factorEscala, plantilla);
                            //int x2SegmentoMayor = GetXCanvas(ladoMayor.Segmentos[ladoMayor.Segmentos.Count - 1].P2.X, xCentroidBase, factorEscala, plantilla);
                            //int y2SegmentoMayor = GetYCanvas(ladoMayor.Segmentos[ladoMayor.Segmentos.Count - 1].P2.Y, yCentroidBase, factorEscala, plantilla);

                            PointF longSidePoint1 = new PointF((float)x1SegmentoMayor, (float)y1SegmentoMayor);
                            PointF longSidePoint2 = new PointF((float)x2SegmentoMayor, (float)y2SegmentoMayor);
                            PointF origin = new PointF((float)plantilla.X_Centro, (float)plantilla.Y_Centro);
                            anguloRotacion = Theta(longSidePoint1, longSidePoint2, origin);


                            #region Para Testing - Dibujo lados de la geometryBase
                            Random rand = new Random();
                            foreach (var lado in lados)
                            {
                                Color rndColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                                foreach (var segmento in lado.Segmentos)
                                {
                                    x1c = GetXCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                                    y1c = GetYCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                                    x2c = GetXCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                                    y2c = GetYCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                                    graphics.DrawLine(new Pen(rndColor, 10), x1c, y1c, x2c, y2c);
                                }
                            }
                            foreach (var segmento in ladoMayor.Segmentos)
                            {
                                Color rndColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                                x1c = GetXCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                                y1c = GetYCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                                x2c = GetXCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                                y2c = GetYCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                                graphics.DrawLine(new Pen(Color.Black, 10), x1c, y1c, x2c, y2c);
                            }
                            #endregion

                            #endregion
                        }
                        Layer layerSecundario = plantilla.Layers.FirstOrDefault(p => p.Categoria == 2);
                        List<LayerGraf> lstLayerGrafSecundario = new List<LayerGraf>();
                        

                        List<string> lstCoordsBuffImpresion = new List<string>() { xMinBuff.ToString(), yMinBuff.ToString(), xMaxBuff.ToString(), yMaxBuff.ToString() };//Revisar como forma la lista de coordenadas

                        var aLayerGrafSecundario = GetLayerGrafByIds(layerSecundario, layerSecundario.Componente, idsObjetoSecundario, lstCoordsBuffImpresion);

                        Layer layerBase = plantilla.Layers.FirstOrDefault(p => p.Categoria == 1);
                        if (layerBase != null)
                        {
                            aLayerGrafBase = GetLayerGrafById(layerBase, layerBase.Componente, idObjetoGraf, lstCoordsBuffImpresion);
                            if (aLayerGrafBase != null && aLayerGrafBase.Length > 0)
                            {
                                layerGrafBase = aLayerGrafBase[0];
                            }
                        }

                        //Layers ordenados
                        foreach (Layer layer in plantilla.Layers.OrderBy(l => l.Orden))
                        {
                            if (layer.IdLayer == layerBase.IdLayer)
                            {
                                DibujarLayerGraf<LayerGraf>(graphics, aLayerGrafBase, "Nombre", layerBase, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                            }
                            else if (layer.IdLayer == layerSecundario.IdLayer)
                            {
                                DibujarLayerGraf<LayerGraf>(graphics, aLayerGrafSecundario, "Nombre", layerSecundario, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                            }
                            else
                            {
                                //Busco Layer
                                var aLayerGraf = GetLayerGrafByCoords(layer, layer.Componente, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
                                if (aLayerGraf != null && aLayerGraf.Length > 0)
                                {
                                    //DibujoLayer
                                    DibujarLayerGraf<LayerGraf>(graphics, aLayerGraf, "Nombre", layer, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                                }
                            }
                        }

                        //Reseteo los limites de la region en donde se va a poder dibujar
                        Rectangle origRectangle = new Rectangle(0, 0, plantilla.Width, plantilla.Heigth);
                        graphics.SetClip(origRectangle);

                        //Dibujo las Referencias
                        DibujarReferencias(graphics, plantilla);

                        //Busco y dibujo el Norte
                        Norte norte = GetNorteById(plantilla.IdNorte);
                        if (norte != null)
                        {
                            x1c = GetPixels(plantilla.NorteX, plantilla.PPP);
                            y1c = GetPixels(plantilla.NorteY, plantilla.PPP);
                            int norteAnchoPx = GetPixels(plantilla.NorteAncho, plantilla.PPP);
                            int norteAltoPx = GetPixels(plantilla.NorteAlto, plantilla.PPP);
                            Bitmap bmpNorte = (Bitmap)norte.Imagen;
                            if (plantilla.OptimizarTamanioHoja)
                            {
                                bmpNorte = RotateImage(norte.Imagen, (float)(anguloRotacion * 180 / Math.PI));
                                graphics.DrawImage(bmpNorte, x1c, y1c, norteAnchoPx, norteAltoPx);
                            }
                            else
                            {
                                graphics.DrawImage(norte.Imagen, x1c, y1c, norteAnchoPx, norteAltoPx);
                            }
                        }

                        //Dibujo Textos de la plantilla
                        if (plantilla.PlantillaTextos != null)
                        {
                            DibujarTextos(graphics, plantilla, idObjetoGraf, factorEscala, dicTextosVariables);
                        }

                        if (plantilla.IdFuncionAdicional != null && plantilla.IdFuncionAdicional > 0)
                        {
                            //ModPlotFuncionesEspeciales modPlotFuncionesEspeciales = new ModPlotFuncionesEspeciales(_parcelaPlotRepository);
                            //bool ok = modPlotFuncionesEspeciales.EjecutarFuncionEspecial((int)plantilla.IdFuncionAdicional, graphics, plantilla, idPlantillaFondo, idObjetoGraf, idsObjetoSecundario, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
                        }
                        //TODO - Sacar - TEST - save. Testing
                        //image.Save(@"C:\Temp\Manzanero.jpg", ImageFormat.Jpeg);
                        //string fileNameResult = @"C:\Temp\ManzaneroPlantilla" + plantilla.IdPlantilla.ToString() + ".png";
                        //image.Save(fileNameResult, ImageFormat.Png);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, ImageFormat.Png);
                            bImage = ms.ToArray();
                        }
                    }
                }
                //FIN
            }
            return bImage;
        }

        private void DibujarReferencias(Graphics graphics, Plantilla plantilla)
        {
            int xMinReferencia = GetPixels(plantilla.ReferenciaXMin, plantilla.PPP);
            int yMinReferencia = GetPixels(plantilla.ReferenciaYMin, plantilla.PPP);
            int xMaxReferencia = GetPixels(plantilla.ReferenciaXMax, plantilla.PPP);
            int yMaxReferencia = GetPixels(plantilla.ReferenciaYMax, plantilla.PPP);
            int espaciado = GetPixels(plantilla.ReferenciaEspaciado, plantilla.PPP);
            int x1c = xMinReferencia;
            int y1c = yMinReferencia + espaciado;
            int x2c = 0;
            int y2c = 0;
            Color colorReferencia = System.Drawing.ColorTranslator.FromHtml(plantilla.ReferenciaColor);

            //int widthLimites = 4;
            //Pen penLimites = new Pen(colorReferencia, widthLimites);
            //graphics.DrawLine(penLimites, xMinReferencia, yMinReferencia, xMaxReferencia, yMinReferencia);
            //graphics.DrawLine(penLimites, xMaxReferencia, yMinReferencia, xMaxReferencia, yMaxReferencia);
            //graphics.DrawLine(penLimites, xMaxReferencia, yMaxReferencia, xMinReferencia, yMaxReferencia);
            //graphics.DrawLine(penLimites, xMinReferencia, yMaxReferencia, xMinReferencia, yMinReferencia);

            int fontSizeRef = GetPixels(plantilla.ReferenciaFuenteTamanio, plantilla.PPP);
            string[] aFontStyle = plantilla.ReferenciaFuenteEstilo.Split(',');
            FontStyle fontStyle = (FontStyle)aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
            using (Font fontTexto = new Font(plantilla.ReferenciaFuenteNombre, fontSizeRef, fontStyle))
            using (Brush brushTexto = new SolidBrush(colorReferencia))
            {
                SizeF sizeTexto = graphics.MeasureString("TEXTO", fontTexto);
                x1c += (int)((int)sizeTexto.Height / 2);
                int col = 1;
                foreach (Layer layer in plantilla.Layers.OrderBy(l => l.Orden))
                {
                    Color solidColor = layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent;
                    Pen penContorno = layer.ContornoGrosor != null ?
                                      new Pen(ColorTranslator.FromHtml(layer.ContornoColor), GetPixels(layer.ContornoGrosor.Value, plantilla.PPP)) : null;
                    Brush brushRelleno = layer.Relleno ? (layer.RellenoTransparencia != null ?
                                      new SolidBrush(Color.FromArgb((int)layer.RellenoTransparencia, solidColor.R, solidColor.G, solidColor.B)) : new SolidBrush(solidColor)) : null;

                    string texto = layer.Nombre;
                    //SizeF sizeTexto = graphics.MeasureString(texto, fontTexto);
                    //x2c = x1c + espaciado;
                    x2c = x1c + (int)sizeTexto.Height;
                    if (layer.Relleno)
                    {
                        //y2c = y1c + espaciado;
                        y2c = y1c + (int)sizeTexto.Height;
                        List<Point> lstPoint = new List<Point>();
                        lstPoint.Add(new Point(x1c, y1c));
                        lstPoint.Add(new Point(x2c, y1c));
                        lstPoint.Add(new Point(x2c, y2c));
                        lstPoint.Add(new Point(x1c, y2c));
                        if (layer.Contorno)
                        {
                            graphics.DrawPolygon(penContorno, lstPoint.ToArray());
                        }
                        graphics.FillPolygon(brushRelleno, lstPoint.ToArray());
                    }
                    else if (layer.Contorno)
                    {
                        graphics.DrawLine(penContorno, x1c, y1c + (int)(sizeTexto.Height / 2), x2c, y1c + (int)(sizeTexto.Height / 2));
                    }
                    if (layer.PuntoRepresentacion == 2)
                    {
                        if (layer.PuntoImagen != null)
                        {
                            //y2c = y1c + espaciado;
                            //y2c = y1c + (int)sizeTexto.Height;
                            //int puntoAnchoPx = espaciado;
                            //int puntoAltoPx = espaciado;
                            int puntoAnchoPx = (int)sizeTexto.Height;
                            int puntoAltoPx = (int)sizeTexto.Height;
                            graphics.DrawImage(layer.PuntoImagen, x1c, y1c, puntoAnchoPx, puntoAltoPx);
                        }
                    }
                    else if (layer.PuntoRepresentacion == 1)
                    {
                        int puntoAnchoPx = GetPixels(layer.ContornoGrosor.Value, plantilla.PPP);
                        int puntoAltoPx = puntoAnchoPx;
                        if (layer.PuntoPredeterminado == 1)
                        {
                            //Circulo
                            graphics.DrawEllipse(penContorno, x1c, y1c, puntoAnchoPx, puntoAltoPx);
                        }
                        else if (layer.PuntoPredeterminado == 2)
                        {
                            //Cuadrado
                            graphics.FillRectangle(brushRelleno, x1c, y1c, puntoAnchoPx, puntoAltoPx);
                        }
                    }
                    //x2c = x2c + espaciado / 2;
                    x2c = x2c + (int)((int)sizeTexto.Height / 2);
                    int yTexto = y1c;
                    graphics.DrawString(texto, fontTexto, brushTexto, new Point(x2c, yTexto));

                    //y1c += (int)(sizeTexto.Height + espaciado);
                    y1c += (int)sizeTexto.Height + espaciado;
                    if (y1c >= yMaxReferencia)
                    {
                        y1c = yMinReferencia + espaciado;
                        col++;
                        if (col > plantilla.ReferenciaColumnas)
                        {
                            break;
                        }
                        else
                        {
                            x1c = xMinReferencia + (xMaxReferencia - xMinReferencia) / col;
                        }
                    }
                }
            }
        }

        private void DibujarTextos(Graphics graphics, Plantilla plantilla, string idObjetoBase, double factorEscala, Dictionary<string, string> dicTextosVariables)
        {
            Layer layerBase = plantilla.Layers.FirstOrDefault(p => p.Categoria == 1);
            foreach (var plantillaTexto in plantilla.PlantillaTextos)
            {
                Color colorTexto = System.Drawing.ColorTranslator.FromHtml(plantillaTexto.FuenteColor);
                int fontSize = GetPixels(plantillaTexto.FuenteTamanio, plantilla.PPP);
                string[] aFontStyle = plantillaTexto.FuenteEstilo.Split(',');
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                FontStyle fontStyle = (FontStyle)aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
                using (Font fontTexto = new Font(plantillaTexto.FuenteNombre, fontSize, fontStyle))
                using (Brush brushTexto = new SolidBrush(colorTexto))
                {
                    string texto = string.Empty;
                    if (plantillaTexto.Tipo == 1)
                    {
                        texto = plantillaTexto.Origen;
                    }
                    else if (plantillaTexto.Tipo == 2)
                    {
                        //Se muestran solo las variables Escala y fecha y las que vienen definidas como parametro
                        string variable = plantillaTexto.Origen.ToUpper();
                        if (variable.Contains("@ESCALA"))
                        {
                            texto = "1:" + (1 / factorEscala).ToString();
                        }
                        else if (variable.Contains("@FECHA"))
                        {
                            texto = DateTime.Now.ToShortDateString();
                        }
                        else
                        {
                            if (dicTextosVariables.ContainsKey(variable))
                            {
                                texto = dicTextosVariables[variable];
                            }
                        }
                    }
                    else if (plantillaTexto.Tipo == 3)
                    {
                        //Textos de Datos del objeto base
                        if (layerBase != null)
                        {
                            texto = GetLayerGrafTextById(layerBase.Componente, idObjetoBase, plantillaTexto.AtributoId.Value);
                        }
                    }
                    if (texto != string.Empty)
                    {
                        int x1c = GetPixels(plantillaTexto.X, plantilla.PPP);
                        int y1c = GetPixels(plantillaTexto.Y, plantilla.PPP);
                        SizeF sizeTexto = graphics.MeasureString(texto, fontTexto);
                        y1c = (int)(y1c - sizeTexto.Height / 2);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.LineAlignment = StringAlignment.Center;
                        stringFormat.Alignment = StringAlignment.Near;

                        if (plantillaTexto.FuenteAlineacion == 2)
                        {
                            stringFormat.Alignment = StringAlignment.Center;
                            x1c = (int)(x1c - sizeTexto.Width / 2);
                        }
                        else if (plantillaTexto.FuenteAlineacion == 3)
                        {
                            stringFormat.Alignment = StringAlignment.Far;
                            //graphics.DrawString(texto, fontTexto, brushTexto, rectTexto, stringFormat);
                            //x1c = (int)(x1c + sizeTexto.Width);
                        }
                        //graphics.DrawString(texto, fontTexto, brushTexto, new Point(x1c, y1c));
                        Rectangle rectTexto = new Rectangle(x1c, y1c, (int)Math.Ceiling(sizeTexto.Width), (int)Math.Ceiling(sizeTexto.Height));
                        graphics.DrawString(texto, fontTexto, brushTexto, rectTexto, stringFormat);
                        //graphics.DrawRectangle(Pens.Black, rectTexto);
                    }
                }
            }
        }

        private void DibujarLayerGraf(Graphics graphics, LayerGraf[] aLayerGraf, Font fontLayerGraf, Pen penLayerGraf, Brush brushLayerGraf, double xCentroidMnz, double yCentroidMnz, double escala, double x_Centro, double y_Centro, double widthUtilPlantilla, double xMinPlantilla, double yMaxPlantilla, int consMult, bool aplicarLabelAngulo)
        {
            bool moveCoords = true;
            DibujarLayerGraf(graphics, aLayerGraf, fontLayerGraf, penLayerGraf, brushLayerGraf, xCentroidMnz, yCentroidMnz, escala, x_Centro, y_Centro, widthUtilPlantilla, xMinPlantilla, yMaxPlantilla, consMult, aplicarLabelAngulo, moveCoords);
        }
        private void DibujarLayerGraf(Graphics graphics, LayerGraf[] aLayerGraf, Font fontLayerGraf, Pen penLayerGraf, Brush brushLayerGraf, double xCentroidMnz, double yCentroidMnz, double escala, double x_Centro, double y_Centro, double widthUtilPlantilla, double xMinPlantilla, double yMaxPlantilla, int consMult, bool aplicarLabelAngulo, bool moveCoords)
        {
            try
            {
                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                int x1c = 0, y1c = 0;
                int x2c = 0, y2c = 0;
                foreach (var layerGraf in aLayerGraf)
                {
                    string layerGrafNombre = layerGraf.Nombre;
                    DbGeometry geometryLayerGraf = layerGraf.Geom;
                    string wkt = geometryLayerGraf.AsText();
                    if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                    {
                        int cantCoords = (int)geometryLayerGraf.PointCount;
                        for (int i = 1; i <= cantCoords; i++)
                        {
                            x = (double)geometryLayerGraf.PointAt(i).XCoordinate;
                            y = (double)geometryLayerGraf.PointAt(i).YCoordinate;
                            if (i > 1)
                            {
                                x1 = x2;
                                y1 = y2;
                                x2 = x;
                                y2 = y;
                            }
                            else
                            {
                                x1 = x;
                                y1 = y;
                                x2 = x;
                                y2 = y;
                            }
                            if (i > 1)
                            {
                                //dibujar
                                if (moveCoords)
                                {
                                    x1c = (int)((((x1 - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                    y1c = (int)(((((y1 - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                    x2c = (int)((((x2 - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                    y2c = (int)(((((y2 - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                }
                                else
                                {
                                    x1c = (int)((x1 - xMinPlantilla) * widthUtilPlantilla);
                                    y1c = (int)(((y1 - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                    x2c = (int)((x2 - xMinPlantilla) * widthUtilPlantilla);
                                    y2c = (int)(((y2 - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                }
                                x1c = x1c * consMult;
                                y1c = y1c * consMult;
                                x2c = x2c * consMult;
                                y2c = y2c * consMult;
                                graphics.DrawLine(penLayerGraf, x1c, y1c, x2c, y2c);
                            }
                        }
                        x2 = x;
                        y2 = y;
                    }
                    if (layerGrafNombre != string.Empty)
                    {
                        double xCentroidLayerGraf = 0;
                        double yCentroidLayerGraf = 0;
                        if (wkt.Contains("LINE"))
                        {
                            //Si es Linea 
                            double xEndPoint = (double)geometryLayerGraf.EndPoint.XCoordinate;
                            double yEndPoint = (double)geometryLayerGraf.EndPoint.YCoordinate;
                            double xStartPoint = (double)geometryLayerGraf.StartPoint.XCoordinate;
                            double yStartPoint = (double)geometryLayerGraf.StartPoint.YCoordinate;
                            xCentroidLayerGraf = (xEndPoint + xStartPoint) / 2;
                            yCentroidLayerGraf = (yEndPoint + yStartPoint) / 2;
                        }
                        else if (wkt.Contains("POLYG"))
                        {
                            xCentroidLayerGraf = (double)geometryLayerGraf.Centroid.XCoordinate;
                            yCentroidLayerGraf = (double)geometryLayerGraf.Centroid.YCoordinate;
                        }
                        else if (wkt.Contains("POINT"))
                        {
                            aplicarLabelAngulo = false;
                            xCentroidLayerGraf = (double)geometryLayerGraf.XCoordinate;
                            yCentroidLayerGraf = (double)geometryLayerGraf.YCoordinate;
                        }
                        if (!aplicarLabelAngulo)
                        {
                            if (moveCoords)
                            {
                                x1c = (int)((((xCentroidLayerGraf - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((((yCentroidLayerGraf - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                            }
                            else
                            {
                                x1c = (int)((xCentroidLayerGraf - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((yCentroidLayerGraf - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                            }
                            x1c = x1c * consMult;
                            y1c = y1c * consMult;
                            SizeF size = graphics.MeasureString(layerGrafNombre, fontLayerGraf);
                            //int xDrawPoint = (int)(x1c - size.Width / 2);
                            //int yDrawPoint = (int)(y1c - size.Height / 2);
                            int xDrawPoint = x1c;
                            int yDrawPoint = y1c;
                            //PointF drawPoint = new PointF((x1c - size.Width / 2f), (y1c-size.Height / 2f));
                            Point drawPoint = new Point(xDrawPoint, yDrawPoint);
                            graphics.DrawString(layerGrafNombre, fontLayerGraf, brushLayerGraf, drawPoint);
                        }
                        else
                        {
                            #region Aplicar Angulo a Texto
                            double angulo = 90;
                            if (x2 - x1 == 0)
                            {
                                angulo = 90;
                            }
                            else
                            {
                                angulo = Math.Atan((y2 - y1) / (x2 - x1));
                                angulo = angulo * 180 / Math.PI;
                            }
                            if (angulo <= Math.PI && angulo > (Math.PI / 2))
                            {
                                angulo = angulo + Math.PI;
                            }
                            if (angulo <= (3 / 2) * Math.PI && angulo > Math.PI)
                            {
                                angulo = angulo - Math.PI;
                            }
                            //height = gHeightLabelParc * escala;
                            if (angulo < 0)
                            {
                                angulo = angulo * (-1);
                            }
                            double heightLabel = 0.0025 * escala;
                            //desplazamiento = (height / 2) + pDesplazamLabelCalles;
                            double desplazam = (heightLabel / 2) + 0.3;
                            double beta = angulo - (Math.PI / 2);
                            double xCalc = xCentroidLayerGraf + desplazam * Math.Cos(beta);
                            double yCalc = yCentroidLayerGraf + desplazam * Math.Sin(beta);
                            if (moveCoords)
                            {
                                x1c = (int)((((xCalc - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((((yCalc - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                //x1c = (int)((((xCentroidLayerGraf - xCentroidMnz) * escala) + x_Centro + 0.0109) * widthUtilPlantilla);
                                //y1c = (int)(((((yCentroidLayerGraf - yCentroidMnz) * escala) + y_Centro - 0.2047)) * (-1) * widthUtilPlantilla);
                            }
                            else
                            {
                                x1c = (int)((xCalc - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((yCalc - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                            }
                            x1c = x1c * consMult;
                            y1c = y1c * consMult;
                            DrawRotatedTextAt(graphics, (float)angulo, layerGrafNombre, x1c, y1c, fontLayerGraf, brushLayerGraf);
                            #endregion
                        }

                    }
                }
                //penLayerGraf.Dispose();
                //brushLayerGraf.Dispose();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void DibujarLayerGraf<T>(Graphics graphics, T[] aLayerGraf, Font fontLayerGraf, Pen penLayerGraf, Brush brushLayerGraf, double xCentroidMnz, double yCentroidMnz, double escala, double x_Centro, double y_Centro, double xMinPlantilla, double yMaxPlantilla, double widthUtilPlantilla, int consMult, bool aplicarLabelAngulo)
        {
            bool moveCoords = true;
            DibujarLayerGraf<T>(graphics, aLayerGraf, fontLayerGraf, penLayerGraf, brushLayerGraf, xCentroidMnz, yCentroidMnz, escala, x_Centro, y_Centro, xMinPlantilla, yMaxPlantilla, widthUtilPlantilla, consMult, aplicarLabelAngulo, moveCoords);
        }
        private void DibujarLayerGraf<T>(Graphics graphics, T[] aLayerGraf, Font fontLayerGraf, Pen penLayerGraf, Brush brushLayerGraf, double xCentroidMnz, double yCentroidMnz, double escala, double x_Centro, double y_Centro, double xMinPlantilla, double yMaxPlantilla, double widthUtilPlantilla, int consMult, bool aplicarLabelAngulo, bool moveCoords)
        {
            string layerGrafLabelProperty = "Nombre";
            bool centrarLabel = false;
            DibujarLayerGraf<T>(graphics, aLayerGraf, layerGrafLabelProperty, fontLayerGraf, penLayerGraf, brushLayerGraf, xCentroidMnz, yCentroidMnz, escala, x_Centro, y_Centro, xMinPlantilla, yMaxPlantilla, widthUtilPlantilla, consMult, aplicarLabelAngulo, centrarLabel, moveCoords);
        }
        private void DibujarLayerGraf<T>(Graphics graphics, T[] aLayerGraf, string layerGrafLabelProperty, Font fontLayerGraf, Pen penLayerGraf, Brush brushLayerGraf, double xCentroidMnz, double yCentroidMnz, double escala, double x_Centro, double y_Centro, double xMinPlantilla, double yMaxPlantilla, double widthUtilPlantilla, int consMult, bool aplicarLabelAngulo, bool centrarLabel, bool moveCoords)
        {
            try
            {
                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                int x1c = 0, y1c = 0;
                int x2c = 0, y2c = 0;
                //int widthUtilPlantilla = 2408;
                //double widthUtilPlantilla = 2535;
                //int widthUtilPlantilla = 2520;
                foreach (var layerGraf in aLayerGraf)
                {
                    string layerGrafNombre = string.Empty;
                    if (layerGrafLabelProperty != string.Empty)
                    {
                        //layerGrafNombre = layerGraf.GetType().GetProperty("Nombre", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(layerGraf).ToString();
                        layerGrafNombre = layerGraf.GetType().GetProperty(layerGrafLabelProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(layerGraf).ToString();
                    }
                    DbGeometry geometryLayerGraf = (DbGeometry)layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf);
                    string wkt = geometryLayerGraf.AsText();
                    if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                    {
                        int cantCoords = (int)geometryLayerGraf.PointCount;
                        for (int i = 1; i <= cantCoords; i++)
                        {
                            x = (double)geometryLayerGraf.PointAt(i).XCoordinate;
                            y = (double)geometryLayerGraf.PointAt(i).YCoordinate;
                            if (i > 1)
                            {
                                x1 = x2;
                                y1 = y2;
                                x2 = x;
                                y2 = y;
                            }
                            else
                            {
                                x1 = x;
                                y1 = y;
                                x2 = x;
                                y2 = y;
                            }
                            if (i > 1)
                            {
                                //dibujar
                                if (moveCoords)
                                {
                                    //x1c = (int)((((x1 - xCentroidMnz) * escala) + x_Centro + 0.0109) * 2520);
                                    x1c = (int)((((x1 - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                    y1c = (int)(((((y1 - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                    x2c = (int)((((x2 - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                    y2c = (int)(((((y2 - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                }
                                else
                                {
                                    x1c = (int)((x1 - xMinPlantilla) * widthUtilPlantilla);
                                    y1c = (int)(((y1 - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                    x2c = (int)((x2 - xMinPlantilla) * widthUtilPlantilla);
                                    y2c = (int)(((y2 - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                }
                                x1c = x1c * consMult;
                                y1c = y1c * consMult;
                                x2c = x2c * consMult;
                                y2c = y2c * consMult;
                                graphics.DrawLine(penLayerGraf, x1c, y1c, x2c, y2c);

                                //var brush = new SolidBrush(Color.Red);
                                //graphics.FillRectangle(brush, new Rectangle(x1c, y1c, x2c, y2c));
                            }
                        }
                        x2 = x;
                        y2 = y;
                    }
                    if (layerGrafNombre != string.Empty)
                    {
                        double xCentroidLayerGraf = 0;
                        double yCentroidLayerGraf = 0;
                        if (wkt.Contains("LINE"))
                        {
                            //Si es Linea 
                            double xEndPoint = (double)geometryLayerGraf.EndPoint.XCoordinate;
                            double yEndPoint = (double)geometryLayerGraf.EndPoint.YCoordinate;
                            double xStartPoint = (double)geometryLayerGraf.StartPoint.XCoordinate;
                            double yStartPoint = (double)geometryLayerGraf.StartPoint.YCoordinate;
                            xCentroidLayerGraf = (xEndPoint + xStartPoint) / 2;
                            yCentroidLayerGraf = (yEndPoint + yStartPoint) / 2;
                        }
                        else if (wkt.Contains("POLYG"))
                        {
                            xCentroidLayerGraf = (double)geometryLayerGraf.Centroid.XCoordinate;
                            yCentroidLayerGraf = (double)geometryLayerGraf.Centroid.YCoordinate;
                        }
                        else if (wkt.Contains("POINT"))
                        {
                            aplicarLabelAngulo = false;
                            xCentroidLayerGraf = (double)geometryLayerGraf.XCoordinate;
                            yCentroidLayerGraf = (double)geometryLayerGraf.YCoordinate;
                        }
                        if (!aplicarLabelAngulo)
                        {
                            if (moveCoords)
                            {
                                x1c = (int)((((xCentroidLayerGraf - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((((yCentroidLayerGraf - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                            }
                            else
                            {
                                x1c = (int)((xCentroidLayerGraf - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((yCentroidLayerGraf - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                            }
                            x1c = x1c * consMult;
                            y1c = y1c * consMult;
                            int xDrawPoint = x1c;
                            int yDrawPoint = y1c;
                            if (centrarLabel)
                            {
                                SizeF size = graphics.MeasureString(layerGrafNombre, fontLayerGraf);
                                xDrawPoint = (int)(x1c - size.Width / 2);
                                yDrawPoint = (int)(y1c - size.Height / 2);
                            }
                            Point drawPoint = new Point(xDrawPoint, yDrawPoint);
                            graphics.DrawString(layerGrafNombre, fontLayerGraf, brushLayerGraf, drawPoint);
                        }
                        else
                        {
                            #region Aplicar Angulo a Texto
                            double angulo = 90;
                            if (x2 - x1 == 0)
                            {
                                angulo = 90;
                            }
                            else
                            {
                                angulo = Math.Atan((y2 - y1) / (x2 - x1));
                                angulo = angulo * 180 / Math.PI;
                            }
                            if (angulo <= Math.PI && angulo > (Math.PI / 2))
                            {
                                angulo = angulo + Math.PI;
                            }
                            if (angulo <= (3 / 2) * Math.PI && angulo > Math.PI)
                            {
                                angulo = angulo - Math.PI;
                            }
                            ////height = gHeightLabelParc * escala;
                            //if (angulo < 0)
                            //{
                            //    angulo = angulo * (-1);
                            //}
                            double heightLabel = 0.0025 * escala;
                            //desplazamiento = (height / 2) + pDesplazamLabelCalles;
                            double desplazam = (heightLabel / 2) + 0.3;
                            double beta = angulo - (Math.PI / 2);
                            double xCalc = xCentroidLayerGraf + desplazam * Math.Cos(beta);
                            double yCalc = yCentroidLayerGraf + desplazam * Math.Sin(beta);
                            if (moveCoords)
                            {
                                x1c = (int)((((xCalc - xCentroidMnz) * escala) + x_Centro - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((((yCalc - yCentroidMnz) * escala) + y_Centro - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                                //x1c = (int)((((xCentroidLayerGraf - xCentroidMnz) * escala) + x_Centro + 0.0109) * 2520);
                                //y1c = (int)(((((yCentroidLayerGraf - yCentroidMnz) * escala) + y_Centro - 0.2047)) * (-1) * 2520);
                            }
                            else
                            {
                                x1c = (int)((xCalc - xMinPlantilla) * widthUtilPlantilla);
                                y1c = (int)(((yCalc - yMaxPlantilla)) * (-1) * widthUtilPlantilla);
                            }
                            x1c = x1c * consMult;
                            y1c = y1c * consMult;
                            DrawRotatedTextAt(graphics, (float)angulo, layerGrafNombre, x1c, y1c, fontLayerGraf, brushLayerGraf);
                            #endregion
                        }

                    }
                }
                //penLayerGraf.Dispose();
                //brushLayerGraf.Dispose();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void DibujarLayerGraf<T>(Graphics graphics, T[] aLayerGraf, string layerGrafLabelProperty, Layer layer, double xCentroidBase, double yCentroidBase, double escala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario)
        {
            //Este es el metodo que se esta utilizando
            try
            {
                //var imageTest = Image.FromFile(@"C:\Temp\Bandera1_300ppp.png");

                Color solidColor = layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent;
                string[] aFontStyle = layer.EtiquetaFuenteEstilo.Split(',');
                FontStyle fontStyle = (FontStyle)aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
                using (Pen penContorno = layer.ContornoGrosor != null ?
                    new Pen(ColorTranslator.FromHtml(layer.ContornoColor), GetPixels(layer.ContornoGrosor.Value, plantilla.PPP)) : null)
                using (Font fontEtiqueta = layer.EtiquetaFuenteTamanio != null && layer.EtiquetaFuenteTamanio > 0 ?
                    new Font(layer.EtiquetaFuenteNombre, GetPixels(layer.EtiquetaFuenteTamanio.Value, plantilla.PPP), fontStyle) : null)
                using (Brush brushEtiqueta = !string.IsNullOrEmpty(layer.EtiquetaColor) ? new SolidBrush(System.Drawing.ColorTranslator.FromHtml(layer.EtiquetaColor)) : null)
                using (Brush brushRelleno = layer.Relleno ? (layer.RellenoTransparencia != null ? new SolidBrush(Color.FromArgb((int)layer.RellenoTransparencia, solidColor.R, solidColor.G, solidColor.B)) : new SolidBrush(solidColor)) : null)
                {
                    double x = 0, y = 0;
                    double x1 = 0, y1 = 0;
                    double x2 = 0, y2 = 0;
                    int x1c = 0, y1c = 0;
                    int x2c = 0, y2c = 0;
                    foreach (var layerGraf in aLayerGraf)
                    {
                        string layerGrafNombre = string.Empty;
                        if (layerGrafLabelProperty != string.Empty)
                        {
                            layerGrafNombre = layerGraf.GetType().GetProperty("Nombre", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(layerGraf).ToString();
                        }
                        DbGeometry geometryLayerGraf = (DbGeometry)layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf);
                        string wkt = geometryLayerGraf.AsText();
                        if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                        {
                            int cantCoords = (int)geometryLayerGraf.PointCount;
                            List<PointF> lstPoint = new List<PointF>();
                            for (int i = 1; i <= cantCoords; i++)
                            {
                                x = (double)geometryLayerGraf.PointAt(i).XCoordinate;
                                y = (double)geometryLayerGraf.PointAt(i).YCoordinate;
                                if (i > 1)
                                {
                                    x1 = x2;
                                    y1 = y2;
                                    x2 = x;
                                    y2 = y;
                                }
                                else
                                {
                                    x1 = x;
                                    y1 = y;
                                    x2 = x;
                                    y2 = y;
                                }
                                if (i > 1)
                                {
                                    //dibujar
                                    x1c = GetXCanvas(x1, xCentroidBase, escala, plantilla);
                                    y1c = GetYCanvas(y1, yCentroidBase, escala, plantilla);
                                    x2c = GetXCanvas(x2, xCentroidBase, escala, plantilla);
                                    y2c = GetYCanvas(y2, yCentroidBase, escala, plantilla);
                                    if (layer.Contorno)
                                    {
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1c, y1c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2c, y2c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                            graphics.DrawLine(penContorno, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y);
                                        }
                                        else
                                        {
                                            graphics.DrawLine(penContorno, x1c, y1c, x2c, y2c);
                                        }
                                    }
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado1 = Rotate(x1c, y1c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                        PointF ptRotado2 = Rotate(x2c, y2c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                        lstPoint.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                        lstPoint.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                    }
                                    else
                                    {
                                        lstPoint.Add(new PointF(x1c, y1c));
                                        lstPoint.Add(new PointF(x2c, y2c));
                                    }
                                }
                            }
                            x2 = x;
                            y2 = y;
                            if (layer.Relleno)
                            {
                                graphics.FillPolygon(brushRelleno, lstPoint.ToArray());
                            }
                        }
                        else if (wkt.Contains("POINT"))
                        {
                            x = (double)geometryLayerGraf.XCoordinate;
                            y = (double)geometryLayerGraf.YCoordinate;
                            x1c = GetXCanvas(x, xCentroidBase, escala, plantilla);
                            y1c = GetYCanvas(y, yCentroidBase, escala, plantilla);
                            if (layer.PuntoRepresentacion == 2)
                            {
                                if (layer.PuntoImagen != null)
                                {
                                    int puntoAnchoPx = GetPixels(layer.PuntoAncho.Value, plantilla.PPP);
                                    int puntoAltoPx = GetPixels(layer.PuntoAlto.Value, plantilla.PPP);
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1c, y1c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                        graphics.DrawImage(layer.PuntoImagen, ptRotado.X, ptRotado.Y, puntoAnchoPx, puntoAltoPx);
                                    }
                                    else
                                    {
                                        graphics.DrawImage(layer.PuntoImagen, x1c, y1c, puntoAnchoPx, puntoAltoPx);
                                    }
                                }
                            }
                            else if (layer.PuntoRepresentacion == 1)
                            {
                                int puntoAnchoPx = GetPixels(layer.ContornoGrosor.Value, plantilla.PPP);
                                int puntoAltoPx = puntoAnchoPx;
                                if (layer.PuntoPredeterminado == 1)
                                {
                                    //Circulo
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1c, y1c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                        graphics.DrawEllipse(penContorno, ptRotado.X, ptRotado.Y, puntoAnchoPx, puntoAltoPx);
                                    }
                                    else
                                    {
                                        graphics.DrawEllipse(penContorno, x1c, y1c, puntoAnchoPx, puntoAltoPx);
                                    }
                                }
                                else if (layer.PuntoPredeterminado == 2)
                                {
                                    //Cuadrado
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1c, y1c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                        graphics.FillRectangle(brushRelleno, ptRotado.X, ptRotado.Y, puntoAnchoPx, puntoAltoPx);
                                    }
                                    else
                                    {
                                        graphics.FillRectangle(brushRelleno, x1c, y1c, puntoAnchoPx, puntoAltoPx);
                                    }
                                }
                            }
                        }
                        if (layer.Etiqueta)
                        {
                            if (layerGrafNombre != string.Empty)
                            {
                                double xCentroidLayerGraf = 0;
                                double yCentroidLayerGraf = 0;
                                if (wkt.Contains("LINE"))
                                {
                                    //Si es Linea 
                                    double xEndPoint = (double)geometryLayerGraf.EndPoint.XCoordinate;
                                    double yEndPoint = (double)geometryLayerGraf.EndPoint.YCoordinate;
                                    double xStartPoint = (double)geometryLayerGraf.StartPoint.XCoordinate;
                                    double yStartPoint = (double)geometryLayerGraf.StartPoint.YCoordinate;
                                    xCentroidLayerGraf = (xEndPoint + xStartPoint) / 2;
                                    yCentroidLayerGraf = (yEndPoint + yStartPoint) / 2;
                                }
                                else if (wkt.Contains("POLYG"))
                                {
                                    xCentroidLayerGraf = (double)geometryLayerGraf.Centroid.XCoordinate;
                                    yCentroidLayerGraf = (double)geometryLayerGraf.Centroid.YCoordinate;
                                }
                                else if (wkt.Contains("POINT"))
                                {
                                    layer.EtiquetaMantieneOrientacion = false;
                                    xCentroidLayerGraf = (double)geometryLayerGraf.XCoordinate;
                                    yCentroidLayerGraf = (double)geometryLayerGraf.YCoordinate;
                                }
                                if (!layer.EtiquetaMantieneOrientacion)
                                {
                                    x1c = GetXCanvas(xCentroidLayerGraf, xCentroidBase, escala, plantilla);
                                    y1c = GetYCanvas(yCentroidLayerGraf, yCentroidBase, escala, plantilla);
                                    int xDrawPoint = x1c;
                                    int yDrawPoint = y1c;
                                    SizeF size = graphics.MeasureString(layerGrafNombre, fontEtiqueta);
                                    Point drawPoint = new Point(x1c, y1c);
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1c, y1c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                        xDrawPoint = (int)(ptRotado.X - size.Width / 2);
                                        yDrawPoint = (int)(ptRotado.Y - size.Height / 2);
                                        drawPoint = new Point(xDrawPoint, yDrawPoint);
                                    }
                                    else
                                    {
                                        xDrawPoint = (int)(x1c - size.Width / 2);
                                        yDrawPoint = (int)(y1c - size.Height / 2);
                                        drawPoint = new Point(xDrawPoint, yDrawPoint);
                                        //graphics.DrawString(layerGrafNombre, fontEtiqueta, brushEtiqueta, drawPoint);
                                    }
                                    graphics.DrawString(layerGrafNombre, fontEtiqueta, brushEtiqueta, drawPoint);

                                    //TODO - Sacar - for testing image como icono
                                    //graphics.DrawImage(imageTest, xDrawPoint, yDrawPoint, 80, 80);
                                    //graphics.Restore(graphicsState);
                                }
                                else
                                {
                                    #region Aplicar Angulo a Texto
                                    double angulo = 0;
                                    if (x2 - x1 == 0)
                                    {
                                        angulo = Math.PI / 2;
                                    }
                                    else
                                    {
                                        angulo = Math.Atan((y2 - y1) / (x2 - x1));
                                    }

                                    angulo = angulo * (-1);
                                    double heightLabel = 0.0025 * escala;
                                    //double heightLabel = fontLayerGraf.Size;
                                    //desplazamiento = (height / 2) + pDesplazamLabelCalles;
                                    double desplazam = (heightLabel / 2) - 0.3;

                                    double beta = angulo;
                                    double xCalc = xCentroidLayerGraf + desplazam * (Math.Cos(beta) * 180 / Math.PI);
                                    double yCalc = yCentroidLayerGraf + desplazam * (Math.Sin(beta) * 180 / Math.PI);
                                    xCalc = xCentroidLayerGraf;
                                    yCalc = yCentroidLayerGraf;
                                    //double xCalc = xCentroidLayerGraf - desplazam * (Math.Cos(beta) * 180 / Math.PI);
                                    //double yCalc = yCentroidLayerGraf - desplazam * (Math.Sin(beta) * 180 / Math.PI);
                                    //double xCalc = xCentroidLayerGraf * (Math.Cos(beta) * 180 / Math.PI);
                                    //double yCalc = yCentroidLayerGraf * (Math.Sin(beta) * 180 / Math.PI);
                                    x1c = GetXCanvas(xCalc, xCentroidBase, escala, plantilla);
                                    y1c = GetYCanvas(yCalc, yCentroidBase, escala, plantilla);
                                    SizeF sizeTexto = graphics.MeasureString(layerGrafNombre, fontEtiqueta);
                                    if (Math.Abs(Math.Sin(angulo)) <= Math.Sin(45 * Math.PI / 180))
                                    {
                                        x1c = (int)(x1c - (sizeTexto.Width / 2));
                                        y1c = (int)(y1c - (sizeTexto.Height / 2));
                                    }
                                    else
                                    {
                                        y1c = (int)(y1c + (sizeTexto.Width / 2));
                                        x1c = (int)(x1c - (sizeTexto.Height / 2));
                                    }

                                    angulo = angulo * 180 / Math.PI;

                                    int xDrawPoint = x1c;
                                    int yDrawPoint = y1c;
                                    Point drawPoint = new Point(xDrawPoint, yDrawPoint);

                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1c, y1c, GetPixels(plantilla.X_Centro, plantilla.PPP), GetPixels(plantilla.Y_Centro, plantilla.PPP), anguloRotacion, sentidoHorario);
                                        xDrawPoint = (int)ptRotado.X;
                                        yDrawPoint = (int)ptRotado.Y;
                                        drawPoint = new Point(xDrawPoint, yDrawPoint);
                                        DrawRotatedTextAt(graphics, (float)angulo, layerGrafNombre, xDrawPoint, yDrawPoint, fontEtiqueta, brushEtiqueta);
                                        //graphics.DrawString(layerGrafNombre, fontEtiqueta, brushEtiqueta, drawPoint);
                                    }
                                    else
                                    {
                                        DrawRotatedTextAt(graphics, (float)angulo, layerGrafNombre, xDrawPoint, yDrawPoint, fontEtiqueta, brushEtiqueta);
                                    }
                                    //DrawRotatedTextAt(graphics, (float)45, "45", x1c, y1c, fontLayerGraf, brushLayerGraf);
                                    //DrawRotatedTextAt(graphics, (float)130, "130", x1c, y1c, fontLayerGraf, brushLayerGraf);
                                    //DrawRotatedTextAt(graphics, (float)250, "250", x1c, y1c, fontLayerGraf, brushLayerGraf);
                                    //DrawRotatedTextAt(graphics, (float)340, "340", x1c, y1c, fontLayerGraf, brushLayerGraf);
                                    #endregion
                                }
                            }
                        }
                    }
                }
                //penLayerGraf.Dispose();
                //fontLayerGraf.Dispose();
                //brushLayerGraf.Dispose();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private int GetXCanvas(double x, double xCentroidBase, double escala, Plantilla plantilla)
        {
            bool moveCoords = true;
            return GetXCanvas(x, xCentroidBase, escala, plantilla, moveCoords);
        }
        private int GetXCanvas(double x, double xCentroidBase, double escala, Plantilla plantilla, bool moveCoords)
        {
            int xc = 0;
            if (moveCoords)
            {
                //xc = (int)(((((x - xCentroidBase) * escala) + (plantilla.X_Centro * plantilla.Resolucion)) * 100 / 2.54 * plantilla.PPP));
                //xc = GetPixels((((x - xCentroidBase) * escala) + (plantilla.X_Centro * plantilla.Resolucion)), plantilla.PPP);
                xc = GetPixels((((x - xCentroidBase) * escala) * 1000 + plantilla.X_Centro), plantilla.PPP);
            }
            else
            {
                //Verificar si se usa si esta bien
                //xc = (int)(((x * plantilla.Resolucion) * (100 / 2.54)) * plantilla.PPP);
                xc = GetPixels(x, plantilla.PPP);
            }
            //xc = xc * plantilla.ConstMult;
            return xc;
        }
        private int GetYCanvas(double y, double yCentroidBase, double escala, Plantilla plantilla)
        {
            bool moveCoords = true;
            return GetYCanvas(y, yCentroidBase, escala, plantilla, moveCoords);
        }
        private int GetYCanvas(double y, double yCentroidBase, double escala, Plantilla plantilla, bool moveCoords)
        {
            int yc = 0;
            if (moveCoords)
            {
                //yc = (int)(((((y - yCentroidBase) * escala) + ((plantilla.Y_Centro * plantilla.Resolucion) - (plantilla.YMax * plantilla.Resolucion)))) * (-1) * (1 / plantilla.Resolucion));
                //yc = (int)(((((y - yCentroidBase) * escala) + (plantilla.Y_Centro * plantilla.Resolucion) - (plantilla.YMax * plantilla.Resolucion)) * (-1) * 100 / 2.54 * plantilla.PPP));
                //yc = GetPixels(((((y - yCentroidBase) * escala) + (plantilla.Y_Centro * plantilla.Resolucion) - (plantilla.YMax * plantilla.Resolucion)) * (-1) ), plantilla.PPP);
                yc = GetPixels(((((y - yCentroidBase) * escala * 1000) + plantilla.Y_Centro - plantilla.ImpresionYMax) * (-1)), plantilla.PPP);
            }
            else
            {
                //yc = (int)(((y * plantilla.Resolucion) * (100 / 2.54)) * plantilla.PPP);
                yc = GetPixels(y * (-1), plantilla.PPP);
            }
            //yc = yc * plantilla.ConstMult;
            return yc;
        }

        private int GetPixels(double x, int resolucionPPP)
        {
            int px = (int)(x * (resolucionPPP / 25.4));
            return px;
        }

        // Draw a rotated string at a particular position.
        private void DrawRotatedTextAt(Graphics gr, float angle, string txt, int x, int y, Font the_font, Brush the_brush)
        {
            // Save the graphics state.
            GraphicsState state = gr.Save();
            gr.ResetTransform();

            // Rotate.
            gr.RotateTransform(angle);

            // Translate to desired position. Be sure to append
            // the rotation so it occurs after the rotation.
            gr.TranslateTransform(x, y, MatrixOrder.Append);

            // Draw the text at the origin.
            //StringFormat stringFormat = new StringFormat();
            //stringFormat.LineAlignment = StringAlignment.Center;
            //stringFormat.Alignment = StringAlignment.Center;
            //SizeF sizeTexto = gr.MeasureString(txt, the_font);
            //Rectangle rectTexto = new Rectangle(0, 0, (int)Math.Ceiling(sizeTexto.Width), (int)Math.Ceiling(sizeTexto.Height));
            //gr.DrawString(txt, the_font, the_brush, rectTexto, stringFormat);
            gr.DrawString(txt, the_font, the_brush, 0, 0);

            // Restore the graphics state.
            gr.Restore(state);
        }

        private Bitmap RotateImage(Image image, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(image.Width, image.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)image.Width / 2, (float)image.Height / 2);
            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)image.Width / 2, -(float)image.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(image, new Point(0, 0));
            return returnBitmap;
        }

        public PointF Rotate(PointF toRotate, PointF origin, double angulo)
        {
            //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
            var xRotated = Math.Cos(angulo) * (toRotate.X - origin.X) + Math.Sin(angulo) * (toRotate.Y - origin.Y) + origin.X;
            var yRotated = -Math.Sin(angulo) * (toRotate.X - origin.X) + Math.Cos(angulo) * (toRotate.Y - origin.Y) + origin.Y;
            return new PointF((float)xRotated, (float)yRotated);
        }

        public PointF Rotate(int xToRotate, int yToRotate, int xOrigen, int yOrigen, double angulo)
        {
            return Rotate(xToRotate, yToRotate, xOrigen, yOrigen, angulo, false);
        }
        public PointF Rotate(int xToRotate, int yToRotate, int xOrigen, int yOrigen, double angulo, bool sentidoHorario)
        {
            //angulo = 95 * Math.PI / 180;
            //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
            double xRotated = 0;
            double yRotated = 0;
            if (sentidoHorario)
            {
                xRotated = Math.Cos(angulo) * (xToRotate - xOrigen) - Math.Sin(angulo) * (yToRotate - yOrigen) + xOrigen;
                yRotated = Math.Sin(angulo) * (xToRotate - xOrigen) + Math.Cos(angulo) * (yToRotate - yOrigen) + yOrigen;
            }
            else
            {
                xRotated = Math.Cos(angulo) * (xToRotate - xOrigen) + Math.Sin(angulo) * (yToRotate - yOrigen) + xOrigen;
                yRotated = -Math.Sin(angulo) * (xToRotate - xOrigen) + Math.Cos(angulo) * (yToRotate - yOrigen) + yOrigen;
            }
            return new PointF((float)xRotated, (float)yRotated);
        }
        public PointF Rotate(PointF toRotate, PointF origin, double theta, bool clockwise)
        {
            //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
            double xRotated = 0;
            double yRotated = 0;
            if (clockwise)
            {
                xRotated = Math.Cos(theta) * (toRotate.X - origin.X) - Math.Sin(theta) * (toRotate.Y - origin.Y) + origin.X;
                yRotated = Math.Sin(theta) * (toRotate.X - origin.X) + Math.Cos(theta) * (toRotate.Y - origin.Y) + origin.Y;
            }
            else
            {
                xRotated = Math.Cos(theta) * (toRotate.X - origin.X) + Math.Sin(theta) * (toRotate.Y - origin.Y) + origin.X;
                yRotated = -Math.Sin(theta) * (toRotate.X - origin.X) + Math.Cos(theta) * (toRotate.Y - origin.Y) + origin.Y;
            }
            return new PointF((float)xRotated, (float)yRotated);
        }
        public double Theta(PointF longSidePoint1, PointF longSidePoint2, PointF origin)
        {
            double slope = Math.Abs(longSidePoint1.Y - longSidePoint2.Y) / Math.Abs(longSidePoint1.X - longSidePoint2.X);
            slope = Math.Round(slope, 2);

            var i = 0;
            const double radians = 1 * Math.PI / 180;

            while (slope > 0)
            {
                longSidePoint1 = Rotate(longSidePoint1, origin, radians, true);
                longSidePoint2 = Rotate(longSidePoint2, origin, radians, true);

                slope = Math.Abs(longSidePoint1.Y - longSidePoint2.Y) / Math.Abs(longSidePoint1.X - longSidePoint2.X);
                slope = Math.Round(slope, 2);
                //slope = Math.Round(slope, 1);

                i++;
            }

            if (longSidePoint1.Y < origin.Y && longSidePoint2.Y < origin.Y)
                i += 180;

            return i * Math.PI / 180;
        }

        private double GetAnguloRotacion(DbGeometry geometryLayerGraf, ref PointF ptoMedio, ref List<Lado> lados, ref Lado ladoMayor)
        {
            double angulo = 0;
            try
            {
                //List<Lado> lados = new List<Lado>();

                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                double dist = 0;
                double distAux = 0;
                double x1Aux = 0, y1Aux = 0;
                double x2Aux = 0, y2Aux = 0;

                string wkt = geometryLayerGraf.AsText();
                if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                {
                    int cantCoords = (int)geometryLayerGraf.PointCount;
                    List<Segmento> Segmentos = new List<Segmento>();
                    List<Point> lstPoint = new List<Point>();
                    for (int i = 1; i <= cantCoords; i++)
                    {
                        x = (double)geometryLayerGraf.PointAt(i).XCoordinate;
                        y = (double)geometryLayerGraf.PointAt(i).YCoordinate;
                        if (i > 1)
                        {
                            x1 = x2;
                            y1 = y2;
                            x2 = x;
                            y2 = y;
                        }
                        else
                        {
                            x1 = x;
                            y1 = y;
                            x2 = x;
                            y2 = y;
                        }
                        if (i > 1)
                        {
                            //Calculo distancia del segmento
                            dist = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
                            if (dist > distAux)
                            {
                                distAux = dist;
                                x1Aux = x1;
                                y1Aux = y1;
                                x2Aux = x2;
                                y2Aux = y2;
                            }
                            if (x2 - x1 == 0)
                            {
                                angulo = Math.PI / 2;
                            }
                            else
                            {
                                angulo = Math.Atan((y2 - y1) / (x2 - x1));
                            }
                            //angulo = angulo * (-1);
                            angulo = angulo * 180 / Math.PI;
                            Segmento segmento = new Segmento(new PointD(x1, y1), new PointD(x2, y2), dist, angulo);
                            Segmentos.Add(segmento);
                        }
                    }
                    Lado lado = new Lado();
                    lado.Segmentos = new List<Segmento>();
                    double ladoDist = (Segmentos.Count > 0 ? Segmentos[0].Distancia : 0);
                    double ladoAng = (Segmentos.Count > 0 ? Segmentos[0].Angulo : 0);
                    if (Segmentos.Count > 0)
                    {
                        lado.Segmentos.Add(Segmentos[0]);
                    }
                    for (int i = 0; i < Segmentos.Count; i++)
                    {
                        if (i > 0)
                        {
                            //TODO - GetAnguloRotacion - Cambio de lado. Esta determinado por la diferencia de angulo >= 10 y distancia >= 5m
                            if (Math.Abs(Segmentos[i - 1].Angulo - Segmentos[i].Angulo) < 10 || Segmentos[i].Distancia < 5)
                            {
                                ladoDist += Segmentos[i].Distancia;
                                ladoAng = Segmentos[i].Angulo;
                                lado.Segmentos.Add(Segmentos[i]);
                            }
                            else
                            {
                                lado.Distancia = ladoDist;
                                //ver si vuelvo a calcular el angulo en base al primer punto del primer segmento y el ultimo segmento del lado
                                //TODO - GetAnguloRotacion - el lado esta tomando el angulo del ultimo segmento, optimizar probar promedio de angulos
                                lado.Angulo = ladoAng;
                                lados.Add(lado);
                                ladoDist = 0;
                                lado = new Lado();
                                lado.Segmentos = new List<Segmento>();
                                lado.Segmentos.Add(Segmentos[i]);
                                ladoAng = Segmentos[i].Angulo;
                                ladoDist += Segmentos[i].Distancia;
                            }
                        }
                    }
                    if (Segmentos.Count > 0)
                    {
                        lado.Distancia = ladoDist;
                        lado.Angulo = ladoAng;
                        lados.Add(lado);
                    }
                }
                //Lado ladoMayor = new Lado();
                ladoMayor.Distancia = -1;
                foreach (var lado in lados)
                {
                    if (lado.Distancia > ladoMayor.Distancia)
                    {
                        ladoMayor = lado;
                    }
                }
                angulo = ladoMayor.Angulo;
                ptoMedio.X = (float)(ladoMayor.Segmentos[0].P1.X + ladoMayor.Segmentos[0].P2.X) / 2;
                ptoMedio.Y = (float)(ladoMayor.Segmentos[0].P1.Y + ladoMayor.Segmentos[0].P2.Y) / 2;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return angulo;
        }

        public Plantilla GetPlantillaById(int idPlantilla)
        {
            return _plantillaRepository.GetPlantillaById(idPlantilla);
        }
        public PlantillaFondo GetPlantillaFondoById(int idPlantillaFondo)
        {
            return _plantillaFondoRepository.GetPlantillaFondoById(idPlantillaFondo);
        }

        public Hoja GetHojaById(int idHoja)
        {
            return _hojaRepository.GetHojaById(idHoja);
        }

        public Norte GetNorteById(int idNorte)
        {
            return _norteRepository.GetNorteById(idNorte);
        }

        public LayerGraf[] GetLayerGrafById(Layer layer, Componente componente, string id, List<string> lstCoordsGeometry)
        {

            return _layerGrafRepository.GetLayerGrafById(layer, componente, id, lstCoordsGeometry);
        }

        public string GetLayerGrafTextById(Componente componente, string id, long idAtributo)
        {

            return _layerGrafRepository.GetLayerGrafTextById(componente, id, idAtributo);
        }

        public LayerGraf[] GetLayerGrafByIds(Layer layer, Componente componente, string ids, List<string> lstCoordsBuffImpresion)
        {

            return _layerGrafRepository.GetLayerGrafByIds(layer, componente, ids, lstCoordsBuffImpresion);
        }

        public LayerGraf[] GetLayerGrafByCoords(Layer layer, Componente componente, double x1, double y1, double x2, double y2)
        {

            return _layerGrafRepository.GetLayerGrafByCoords(layer, componente, x1, y1, x2, y2);
        }

        public Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public Image ResizeImage(Image image, int width, int height)
        {
            //float width = 1024;
            //float height = 768;
            //var brush = new SolidBrush(Color.Black);

            //Your original file:
            //var image = new Bitmap(file);

            //Target sizing (scale factor):
            float scale = Math.Min(width / image.Width, height / image.Height);

            //The resize including brushing canvas first:
            var scaleImage = new Bitmap(width, height);
            var graph = Graphics.FromImage(scaleImage);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            //graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, new Rectangle(((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight));
            return scaleImage;
        }

    }
}