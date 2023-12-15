using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Repositories;
using iTextSharp.text.pdf;
using MPFuncionesEspeciales;
using it = iTextSharp.text;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Common.ExtensionMethods.Componentes;
using GeoSit.Data.DAL.Common.Enums;
using System.Globalization;
using System.Drawing.Imaging;

namespace GeoSit.Web.Api.Ploteo
{
    public class DistanciaArco
    {
        public string Nombre { set; get; }
        public double? Distancia { set; get; }
        public long Id { set; get; }
        public double? Longitud { set; get; }
        public DistanciaArco(string nombre, double? distancia, long id, double? longitud)
        {
            Nombre = nombre;
            Distancia = distancia;
            Id = id;
            Longitud = longitud;
        }
    }

    public class ModPlot
    {
        private readonly IPlantillaRepository _plantillaRepository;
        private readonly IPlantillaFondoRepository _plantillaFondoRepository;
        private readonly ILayerGrafRepository _layerGrafRepository;
        private readonly IHojaRepository _hojaRepository;
        private readonly INorteRepository _norteRepository;
        private readonly IParcelaPlotRepository _parcelaPlotRepository;
        private readonly ICuadraPlotRepository _cuadraPlotRepository;
        private readonly IManzanaPlotRepository _manzanaPlotRepository;
        private readonly ICallePlotRepository _callePlotRepository;
        private readonly IParametroRepository _parametroRepository;
        private readonly IImagenSatelitalRepository _imagenSatelitalRepository;
        private readonly IExpansionPlotRepository _expansionPlotRepository;
        private readonly ITipoPlanoRepository _tipoPlanoRepository;
        private readonly IPartidoRepository _partidoRepository;
        private readonly ICensoRepository _censoRepository;
        private readonly IPloteoFrecuenteRepository _ploteoFrecuenteRepository;
        private readonly IPloteoFrecuenteEspecialRepository _ploteoFrecuenteEspecialRepository;
        private readonly IPlantillaViewportRepository _plantillaViewportRepository;
        private readonly ITipoViewportRepository _tipoViewportRepository;
        private readonly ILayerViewportReposirory _layerViewportReposirory;
        private readonly IAtributoRepository _atributoRepository;
        private readonly IComponenteRepository _componenteRepository;

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
        public struct PointDec
        {
            public decimal X;
            public decimal Y;
            public PointDec(decimal x, decimal y)
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

        private readonly IFormatProvider NumberFormat = CultureInfo.GetCultureInfo("en-us").NumberFormat;

        public ModPlot(IPlantillaRepository plantillaRepository, ILayerGrafRepository layerGrafRepository, IPlantillaFondoRepository plantillaFondoRepository, IHojaRepository hojaRepository, INorteRepository norteRepository, IParcelaPlotRepository parcelaPlotRepository, ICuadraPlotRepository cuadraPlotRepository, IManzanaPlotRepository manzanaPlotRepository, ICallePlotRepository callePlotRepository, IParametroRepository parametroRepository, IImagenSatelitalRepository imagenSatelitalRepository, IExpansionPlotRepository expansionPlotRepository, ITipoPlanoRepository tipoPlanoRepository, IPartidoRepository partidoRepository, ICensoRepository censoRepository, IPloteoFrecuenteRepository ploteoFrecuenteRepository, IPloteoFrecuenteEspecialRepository ploteoFrecuenteEspecialRepository, IPlantillaViewportRepository plantillaViewportRepository, ITipoViewportRepository tipoViewportRepository, ILayerViewportReposirory layerViewportReposirory, IAtributoRepository atributoRepository, IComponenteRepository componenteRepository)
        {
            _plantillaRepository = plantillaRepository;
            _layerGrafRepository = layerGrafRepository;
            _plantillaFondoRepository = plantillaFondoRepository;
            _hojaRepository = hojaRepository;
            _norteRepository = norteRepository;
            _parcelaPlotRepository = parcelaPlotRepository;
            _cuadraPlotRepository = cuadraPlotRepository;
            _manzanaPlotRepository = manzanaPlotRepository;
            _callePlotRepository = callePlotRepository;
            _parametroRepository = parametroRepository;
            _imagenSatelitalRepository = imagenSatelitalRepository;
            _expansionPlotRepository = expansionPlotRepository;
            _tipoPlanoRepository = tipoPlanoRepository;
            _partidoRepository = partidoRepository;
            _censoRepository = censoRepository;
            _ploteoFrecuenteRepository = ploteoFrecuenteRepository;
            _ploteoFrecuenteEspecialRepository = ploteoFrecuenteEspecialRepository;
            _plantillaViewportRepository = plantillaViewportRepository;
            _tipoViewportRepository = tipoViewportRepository;
            _layerViewportReposirory = layerViewportReposirory;
            _atributoRepository = atributoRepository;
            _componenteRepository = componenteRepository;
        }

        public byte[] GetPlantilla(int idPlantilla, string idObjetoGraf, int idComponenteObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, bool verCotas, int idImagenSatelital, float imagenTransparencia, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, string numeroODT, string tipo, string relacion, string direccionCompleta, string datosReferencia, byte[] bPdfCompleto = null, string lstAtributos = "")
        {
            //Viene de GetPlantilla comun (1)
            string extent = string.Empty;
            string scale = string.Empty;
            string layersVisibles = string.Empty;
            bool verIdentificante = true;
            long? idComponentePrincipal = null;
            bool verContexto = false;
            bool esInformeAnual = false;
            string anio = string.Empty;
            return GetPlantilla(idPlantilla, idObjetoGraf, idComponenteObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables, extent, scale, layersVisibles, verCotas, null, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, grafico, leyenda, infoLeyenda, esInformeAnual, anio, numeroODT, tipo, relacion, direccionCompleta, datosReferencia, null, bPdfCompleto, lstAtributos);
        }

        public byte[] GetPlantilla(int idPlantilla, int idPlantillaFondo, string textosVariables, string extent, string scale, string layersVisibles, int idImagenSatelital, float imagenTransparencia, byte[] pdfCompleto = null)
        {
            //Viene de Vista Actual
            string idObjetoGraf = string.Empty;
            string idsObjetoSecundario = string.Empty;
            int idComponenteObjetoGraf = 0;
            bool verCotas = false;
            bool verIdentificante = true;
            long? idComponentePrincipal = null;
            bool verContexto = false;
            return GetPlantilla(idPlantilla, idObjetoGraf, idComponenteObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables, extent, scale, layersVisibles, verCotas, null, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, null);
        }

        public byte[] GetPlantilla(int idPlantilla, int idPlantillaFondo, string extent, string textosVariables, string idDistrito, int idComponenteDistrito, ObjetoResultadoDetalle objetoResultadoDetalle, int idImagenSatelital, float imagenTransparencia, bool verIdentificante, long? idComponentePrincipal, bool verContexto, long? idComponenteTematico)
        {
            //viene de mapa tematico ploteo
            string idObjetoGraf = idDistrito;
            string idsObjetoSecundario = string.Empty;
            int idComponenteObjetoGraf = idComponenteDistrito;
            bool verCotas = false;
            string scale = string.Empty;
            string layersVisibles = string.Empty;
            return GetPlantilla(idPlantilla, idObjetoGraf, idComponenteObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables, extent, scale, layersVisibles, verCotas, objetoResultadoDetalle, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, idComponenteTematico);
        }

        public byte[] GetPlantilla(int idPlantilla, string idObjetoGraf, int idComponenteObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, string extent, string scale, string layersVisibles, bool verCotas, ObjetoResultadoDetalle objetoResultadoDetalle, int idImagenSatelital, float imagenTransparencia, bool verIdentificante, long? idComponentePrincipal, bool verContexto, long? idComponenteTematico)
        {
            //Viene de Vista Actual y de Mapa Tematico (2)
            return GetPlantilla(idPlantilla, idObjetoGraf, idComponenteObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables, extent, scale, layersVisibles, verCotas, objetoResultadoDetalle, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, null, null, null, false, string.Empty, null, null, null, null, null, idComponenteTematico);
        }

        public byte[] GetPlantilla(int idPlantilla, int idPlantillaFondo, int idPartido, string anio, int idComponentePartido, string textosVariables, int idImagenSatelital, float imagenTransparencia)
        {
            //Viene de Informe Anual (3)
            string idObjetoGraf = idPartido.ToString();
            string idsObjetoSecundario = string.Empty;
            int idComponenteObjetoGraf = idComponentePartido;
            bool verCotas = false;
            string scale = string.Empty;
            string layersVisibles = string.Empty;
            string extent = string.Empty;
            bool verIdentificante = true;
            long? idComponentePrincipal = null;
            bool verContexto = false;
            bool esInformeAnual = true;
            return GetPlantilla(idPlantilla, idObjetoGraf, idComponenteObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables, extent, scale, layersVisibles, verCotas, null, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, null, null, null, esInformeAnual, anio, null, null);
        }

        public byte[] GetPlantilla(int idPlantilla, string idObjetoGraf, int idComponenteObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, string extent, string scale, string layersVisibles, bool verCotas, ObjetoResultadoDetalle objetoResultadoDetalle, int idImagenSatelital, float imagenTransparencia, bool verIdentificante, long? idComponentePrincipal, bool verContexto, InformacionComercial grafico = null, InformacionComercial leyenda = null, int? infoLeyenda = null, bool esInformeAnual = false, string anio = "", string numeroODT = "0", string tipo = null, string relacion = null, string direccionCompleta = null, string datosReferencia = null, long? idComponenteTematico = null, byte[] bPdfCompleto = null, string lstAtributos = "", string idsSeleccionados = null, int idComponenteSelec = 0)
        {
            //Viene de (1), (2) y (3)
            try
            {
                //bPdfCompleto = File.ReadAllBytes(@"d:\archivo.pdf"); // test
                DbGeometry bbox = null;
                void calcbbox(IEnumerable<LayerGraf> objetos)
                {
                    var layerbbox = objetos.Skip(1).Aggregate(objetos.First().Geom, (geom, obj) => geom.Union(obj.Geom));
                    bbox = bbox?.Union(layerbbox) ?? layerbbox;
                }

                long ID_COMPONENTE_ARCO = GetComponenteArco();

                Plantilla plantilla = GetPlantillaById(idPlantilla);
                PlantillaFondo plantillaFondo = GetPlantillaFondoById(idPlantillaFondo);

                if (plantilla != null && plantillaFondo != null)
                {
                    #region Configuración plantilla
                    Hoja hoja = GetHojaById(plantilla.IdHoja);
                    plantilla.PPP = plantillaFondo.Resolucion.Valor;
                    float anchoPts = it.Utilities.MillimetersToPoints((float)hoja.Ancho_mm);
                    float altoPts = it.Utilities.MillimetersToPoints((float)hoja.Alto_mm);
                    if (plantilla.Orientacion == 0)
                    {
                        plantilla.WidthPts = anchoPts;
                        plantilla.HeigthPts = altoPts;
                    }
                    else
                    {
                        plantilla.WidthPts = altoPts;
                        plantilla.HeigthPts = anchoPts;
                        anchoPts = it.Utilities.MillimetersToPoints((float)hoja.Alto_mm);
                        altoPts = it.Utilities.MillimetersToPoints((float)hoja.Ancho_mm);
                    }
                    float impresionXMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMin);
                    float impresionYMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMin);
                    float impresionXMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMax);
                    float impresionYMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMax);

                    float grosorMaximo = 0;

                    double x = 0, y = 0, escala = 0, xCentroidBase = 0, yCentroidBase = 0,
                           factorEscala = 0, xMinBuff = 9999999, yMinBuff = 9999999,
                           xMaxBuff = 0, yMaxBuff = 0, anguloRotacion = 0, anguloRotacionFiltro = 0;

                    float pdfx1c = 0, pdfy1c = 0;

                    LayerGraf[] aLayerGrafBase = null;
                    LayerGraf layerGrafBase = null;
                    Layer layerBase = plantilla.Layers.FirstOrDefault(p => p.Categoria == 1 && p.ComponenteId == idComponenteObjetoGraf && p.FechaBaja == null);
                    DbGeometry geometryBase = null;
                    DbGeometry bufferBase = null;

                    var lstCoordsGeometryBase = new List<string>();
                    var lstCoordsBuffImpresion = new List<string>();
                    var lstLayersReferencia = new List<Layer>();

                    bool sentidoHorario = false;

                    string imagenSatelitalName = string.Empty;
                    #endregion

                    #region Objeto de Layer Base
                    if (!string.IsNullOrEmpty(idObjetoGraf))
                    {
                        //Layer Base
                        if (layerBase != null)
                        {
                            aLayerGrafBase = GetLayerGrafById(layerBase, layerBase.Componente, idObjetoGraf, lstCoordsBuffImpresion);
                            if (aLayerGrafBase != null && aLayerGrafBase.Length > 0)
                            {
                                geometryBase = aLayerGrafBase[0].Geom;
                                for (int i = 1; i < aLayerGrafBase.Length; i++)
                                {
                                    geometryBase = geometryBase.Union(aLayerGrafBase[i].Geom);
                                }
                            }
                            else
                            {
                                Global.GetLogger().LogInfo(string.Format("El objeto {0} del componente {1} no tiene gráficos. No se puede graficar el objeto base por lo que se cancela el ploteo", idObjetoGraf, layerBase.Componente.Nombre));
                                return bPdfCompleto;
                            }
                        }
                        if (geometryBase != null)
                        {
                            if (geometryBase.Centroid != null)
                            {
                                xCentroidBase = (double)geometryBase.Centroid.XCoordinate;
                                yCentroidBase = (double)geometryBase.Centroid.YCoordinate;
                            }
                            else
                            {
                                var centroide = geometryBase.Envelope.Centroid;
                                xCentroidBase = (double)centroide.XCoordinate;
                                yCentroidBase = (double)centroide.YCoordinate;
                            }

                            int cantCoordsObjBase = geometryBase.PointCount.HasValue ? geometryBase.PointCount.Value : 1;
                            for (int i = 1; i <= cantCoordsObjBase; i++)
                            {
                                x = (double)geometryBase.PointAt(i).XCoordinate;
                                y = (double)geometryBase.PointAt(i).YCoordinate;
                                lstCoordsGeometryBase.Add(x.ToString().Replace(",", ".") + ", " + y.ToString().Replace(",", "."));
                            }

                            //Buffer
                            if (plantilla.DistBuffer == 0)
                            {
                                bufferBase = geometryBase;
                            }
                            else
                            {
                                bufferBase = geometryBase.Buffer(plantilla.DistBuffer);
                            }

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

                            xCentroidBase = xMinBuff + ((xMaxBuff - xMinBuff) / 2.0);
                            yCentroidBase = yMinBuff + ((yMaxBuff - yMinBuff) / 2.0);

                            //Calculo la escala en base a los minimos y maximos del buffer en vez de la manzana
                            #region Calculo de Escala
                            if (((plantilla.X_Util / 1000) * (yMaxBuff - yMinBuff)) > ((plantilla.Y_Util / 1000) * (xMaxBuff - xMinBuff)))
                            {
                                factorEscala = (plantilla.Y_Util / 1000) / (yMaxBuff - yMinBuff);
                            }
                            else
                            {
                                factorEscala = (plantilla.X_Util / 1000) / (xMaxBuff - xMinBuff);
                            }
                            escala = 1 / factorEscala;

                            int escalaPlantilla = Convert.ToInt32(Math.Ceiling(escala));
                            var lstEscala = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).ToList();
                            if (lstEscala != null && lstEscala.Count > 0)
                            {
                                escalaPlantilla = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).OrderBy(p => p.Escala).Select(p => p.Escala).First();
                            }
                            factorEscala = 1.0 / escalaPlantilla;
                            escala = 1 / factorEscala;
                            #endregion
                        }
                    }
                    else
                    {
                        var aExtents = extent.Split(',').Take(4).Select(Convert.ToDouble).ToArray();
                        aExtents = TransformCoords(aExtents[0], aExtents[1], aExtents[2], aExtents[3], SRID.DB, SRID.App);
                        if (aExtents.Any())
                        {
                            #region extents
                            int iExt = 0;
                            while (iExt < aExtents.Length)
                            {
                                x = Convert.ToDouble(aExtents[iExt]);
                                iExt++;
                                y = Convert.ToDouble(aExtents[iExt]);
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
                                if (aExtents.Length > 4)
                                {
                                    //Salteo la que viene en 0
                                    iExt++;
                                }
                                iExt++;
                            }
                            xCentroidBase = xMinBuff + ((xMaxBuff - xMinBuff) / 2.0);
                            yCentroidBase = yMinBuff + ((yMaxBuff - yMinBuff) / 2.0);

                            if (idComponentePrincipal.GetValueOrDefault() > 0)
                            {
                                layerGrafBase = null;
                                //Plotear por componente principal. Vuelvo a calcular el extent en base al objeto principal que encuentre en base al extent del MT
                                layerBase = plantilla.Layers.FirstOrDefault(p => p.Categoria == 1 && p.ComponenteId == idComponentePrincipal && p.FechaBaja == null);
                                //Layer Base
                                if (layerBase != null)
                                {
                                    string guid = (objetoResultadoDetalle != null ? objetoResultadoDetalle.GUID : string.Empty);
                                    layerGrafBase = GetLayerGrafByComponentePrincipal(layerBase, layerBase.Componente, guid);
                                }
                                if (layerGrafBase != null)
                                {
                                    geometryBase = layerGrafBase.Geom;
                                    if (geometryBase.Centroid != null)
                                    {
                                        xCentroidBase = (double)geometryBase.Centroid.XCoordinate;
                                        yCentroidBase = (double)geometryBase.Centroid.YCoordinate;
                                        int cantCoordsObjBase = (int)geometryBase.PointCount;
                                        for (int i = 1; i <= cantCoordsObjBase; i++)
                                        {
                                            x = (double)geometryBase.PointAt(i).XCoordinate;
                                            y = (double)geometryBase.PointAt(i).YCoordinate;
                                            lstCoordsGeometryBase.Add(x.ToString().Replace(",", ".") + ", " + y.ToString().Replace(",", "."));
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
                                        xCentroidBase = xMinBuff + ((xMaxBuff - xMinBuff) / 2.0);
                                        yCentroidBase = yMinBuff + ((yMaxBuff - yMinBuff) / 2.0);
                                    }
                                }

                            }
                            #endregion
                        }

                        //Calculo la escala en base a los minimos y maximos del buffer en vez de la manzana
                        #region Calculo de Escala
                        if (((plantilla.X_Util / 1000) * (yMaxBuff - yMinBuff)) > ((plantilla.Y_Util / 1000) * (xMaxBuff - xMinBuff)))
                        {
                            factorEscala = (plantilla.Y_Util / 1000) / (yMaxBuff - yMinBuff);
                        }
                        else
                        {
                            factorEscala = (plantilla.X_Util / 1000) / (xMaxBuff - xMinBuff);
                        }
                        escala = 1 / factorEscala;

                        if (string.IsNullOrEmpty(scale))
                        {
                            //Viene de Mapa Tematico
                            long escalaPlantilla = Convert.ToInt64(Math.Ceiling(escala));
                            var lstEscala = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).ToList();
                            if (lstEscala != null && lstEscala.Count > 0)
                            {
                                escalaPlantilla = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).OrderBy(p => p.Escala).Select(p => p.Escala).First();
                            }
                            factorEscala = 1.0 / escalaPlantilla;
                            escala = 1 / factorEscala;
                        }
                        #endregion
                    }
                    #endregion

                    #region Objetos de Layer Secundario
                    var layerSecundario = plantilla.Layers.FirstOrDefault(p => p.Categoria == 2 && p.FechaBaja == null);
                    var aLayerGrafSecundario = default(LayerGraf[]);

                    if (layerSecundario != null && !string.IsNullOrEmpty(idsObjetoSecundario))
                    {
                        aLayerGrafSecundario = GetLayerGrafByIds(layerSecundario, layerSecundario.Componente, idsObjetoSecundario, lstCoordsBuffImpresion);
                    }
                    #endregion

                    #region Area de Impresion
                    double xMinBuffImpresion = GetXBuffer(plantilla.ImpresionXMin, xCentroidBase, factorEscala, plantilla);
                    double yMinBuffImpresion = GetYBuffer(plantilla.ImpresionYMin, yCentroidBase, factorEscala, plantilla);
                    double xMaxBuffImpresion = GetXBuffer(plantilla.ImpresionXMax, xCentroidBase, factorEscala, plantilla);
                    double yMaxBuffImpresion = GetYBuffer(plantilla.ImpresionYMax, yCentroidBase, factorEscala, plantilla);

                    lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString(NumberFormat) + ", " + yMinBuffImpresion.ToString(NumberFormat));
                    lstCoordsBuffImpresion.Add(xMaxBuffImpresion.ToString(NumberFormat) + ", " + yMinBuffImpresion.ToString(NumberFormat));
                    lstCoordsBuffImpresion.Add(xMaxBuffImpresion.ToString(NumberFormat) + ", " + yMaxBuffImpresion.ToString(NumberFormat));
                    lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString(NumberFormat) + ", " + yMaxBuffImpresion.ToString(NumberFormat));
                    lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString(NumberFormat) + ", " + yMinBuffImpresion.ToString(NumberFormat));

                    if (plantilla.OptimizarTamanioHoja && !string.IsNullOrEmpty(idObjetoGraf) && geometryBase.PointCount.HasValue && geometryBase.PointCount > 1)
                    {
                        #region OptimizarTamanioHoja
                        PointF puntoMedio = new PointF();
                        List<Lado> lados = new List<Lado>();
                        Lado ladoMayor = new Lado();
                        int anguloTolerancia = 10;
                        double distanciaTolerancia = 5;
                        anguloRotacion = GetAnguloRotacion(geometryBase, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);
                        //Paso a radianes
                        anguloRotacion = anguloRotacion * Math.PI / 180;
                        anguloRotacionFiltro = anguloRotacion;
                        sentidoHorario = true;

                        //Segmento Mayor del Lado Mayor
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
                        float x1SegmentoMayor = GetXPDFCanvas(segmentoMayor.P1.X, xCentroidBase, factorEscala, plantilla);
                        float y1SegmentoMayor = GetYPDFCanvas(segmentoMayor.P1.Y, yCentroidBase, factorEscala, plantilla);
                        float x2SegmentoMayor = GetXPDFCanvas(segmentoMayor.P2.X, xCentroidBase, factorEscala, plantilla);
                        float y2SegmentoMayor = GetYPDFCanvas(segmentoMayor.P2.Y, yCentroidBase, factorEscala, plantilla);
                        if (x2SegmentoMayor - x1SegmentoMayor == 0)
                        {
                            anguloRotacion = Math.PI / 2;
                        }
                        else
                        {
                            anguloRotacion = Math.Atan((y2SegmentoMayor - y1SegmentoMayor) / (x2SegmentoMayor - x1SegmentoMayor));
                        }

                        PointF longSidePoint1 = new PointF(x1SegmentoMayor, y1SegmentoMayor);
                        PointF longSidePoint2 = new PointF(x2SegmentoMayor, y2SegmentoMayor);
                        float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                        float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                        PointF origin = new PointF(xCentro, yCentro);
                        //Angulo de rotacion en radianes - Se esta utilizando este para la rotacion
                        anguloRotacion = Theta(longSidePoint1, longSidePoint2, origin);

                        //Recalculo la escala en base a la geometria del buffer segun sus coordenadas max y min rotadas
                        escala = GetEscala(bufferBase, xCentroidBase, yCentroidBase, plantilla, anguloRotacion, sentidoHorario, ref xMinBuff, ref yMinBuff, ref xMaxBuff, ref yMaxBuff);
                        factorEscala = 1 / escala;

                        //Roto los minimos y maximos del bufferBase rotado obtenidos en GetEscala
                        PointD ptBuffImpresionMinMin = RotateD(xMinBuff, yMinBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                        PointD ptBuffImpresionMaxMax = RotateD(xMaxBuff, yMaxBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                        PointD ptBuffImpresionMaxMin = RotateD(xMaxBuff, yMinBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                        PointD ptBuffImpresionMinMax = RotateD(xMinBuff, yMaxBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);

                        var lstX = new[] { ptBuffImpresionMinMin.X, ptBuffImpresionMaxMax.X, ptBuffImpresionMaxMin.X, ptBuffImpresionMinMax.X };
                        var lstY = new[] { ptBuffImpresionMinMin.Y, ptBuffImpresionMaxMax.Y, ptBuffImpresionMaxMin.Y, ptBuffImpresionMinMax.Y };

                        xMinBuffImpresion = lstX.Min();
                        yMinBuffImpresion = lstY.Min();
                        xMaxBuffImpresion = lstX.Max();
                        yMaxBuffImpresion = lstY.Max();

                        //Vuelvo a calcular el centroide
                        xCentroidBase = xMinBuffImpresion + ((xMaxBuffImpresion - xMinBuffImpresion) / 2.0);
                        yCentroidBase = yMinBuffImpresion + ((yMaxBuffImpresion - yMinBuffImpresion) / 2.0);

                        //En base al centroide nuevo vuelvo a sacar las coordenadas
                        xMinBuffImpresion = GetXBuffer(plantilla.ImpresionXMin, xCentroidBase, factorEscala, plantilla);
                        yMinBuffImpresion = GetYBuffer(plantilla.ImpresionYMin, yCentroidBase, factorEscala, plantilla);
                        xMaxBuffImpresion = GetXBuffer(plantilla.ImpresionXMax, xCentroidBase, factorEscala, plantilla);
                        yMaxBuffImpresion = GetYBuffer(plantilla.ImpresionYMax, yCentroidBase, factorEscala, plantilla);

                        //Anti-Roto el buffer de impresion y esas coordenadas son las que voy a usar para consultar a la base
                        ptBuffImpresionMinMin = RotateD(xMinBuffImpresion, yMinBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                        ptBuffImpresionMaxMax = RotateD(xMaxBuffImpresion, yMaxBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                        ptBuffImpresionMaxMin = RotateD(xMaxBuffImpresion, yMinBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                        ptBuffImpresionMinMax = RotateD(xMinBuffImpresion, yMaxBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);

                        lstCoordsBuffImpresion.Clear();
                        lstCoordsBuffImpresion.Add(ptBuffImpresionMinMin.X.ToString(NumberFormat) + ", " + ptBuffImpresionMinMin.Y.ToString(NumberFormat));
                        lstCoordsBuffImpresion.Add(ptBuffImpresionMaxMin.X.ToString(NumberFormat) + ", " + ptBuffImpresionMaxMin.Y.ToString(NumberFormat));
                        lstCoordsBuffImpresion.Add(ptBuffImpresionMaxMax.X.ToString(NumberFormat) + ", " + ptBuffImpresionMaxMax.Y.ToString(NumberFormat));
                        lstCoordsBuffImpresion.Add(ptBuffImpresionMinMax.X.ToString(NumberFormat) + ", " + ptBuffImpresionMinMax.Y.ToString(NumberFormat));
                        lstCoordsBuffImpresion.Add(ptBuffImpresionMinMin.X.ToString(NumberFormat) + ", " + ptBuffImpresionMinMin.Y.ToString(NumberFormat));

                        #endregion
                    }

                    #endregion

                    var lstLayersVisibles = (layersVisibles ?? string.Empty)
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .ToList();
                    #region Buscar Layers a plotear
                    //Filtra por categoria menor a 3. Categoria=3 es para config. texto de mapa tematico
                    var query = plantilla.Layers.Where(pl => pl.FechaBaja == null && pl.Categoria < 3);
                    if (string.IsNullOrEmpty(idObjetoGraf))
                    {
                        query = query.Where(pl => pl.FiltroGeografico == 1 || pl.FiltroGeografico == 0);
                    }
                    if (idComponenteTematico != null)
                    {
                        query = query.Where(p => p.ComponenteId != idComponenteTematico.Value);
                    }
                    var lstLayersOrdenados = query.OrderBy(l => l.Orden).ToList();
                    #endregion

                    #region Dibujo de Plantilla
                    byte[] bPdf;
                    using (var pdfReaderPlantilla = new PdfReader(plantillaFondo.PDFMemoryStream))
                    using (var memStream = new MemoryStream())
                    {
                        void dibujarPlantillaFondo(PdfContentByte pdfContentByte)
                        {
                            pdfContentByte.PdfWriter.ViewerPreferences = PdfWriter.PageModeUseOC;
                            pdfContentByte.PdfWriter.PdfVersion = PdfWriter.VERSION_1_5;

                            var importedPagePlantilla = pdfContentByte.PdfWriter.GetImportedPage(pdfReaderPlantilla, 1);
                            float pageWidth = pdfReaderPlantilla.GetPageSizeWithRotation(1).Width;
                            float pageHeight = pdfReaderPlantilla.GetPageSizeWithRotation(1).Height;

                            switch (importedPagePlantilla.Rotation)
                            {
                                case 0:
                                    pdfContentByte.AddTemplate(importedPagePlantilla, 1f, 0, 0, 1f, 0, 0);
                                    break;
                                case 90:
                                    pdfContentByte.AddTemplate(importedPagePlantilla, 0, -1f, 1f, 0, 0, pageHeight);
                                    break;
                                case 180:
                                    pdfContentByte.AddTemplate(importedPagePlantilla, -1f, 0, 0, -1f, pageWidth, pageHeight);
                                    break;
                                case 270:
                                    pdfContentByte.AddTemplate(importedPagePlantilla, 0, 1f, -1f, 0, pageWidth, 0);
                                    break;
                                default:
                                    pdfContentByte.AddTemplate(importedPagePlantilla, 1f, 0, 0, 1f, 0, 0);
                                    break;
                            }
                        }
                        var pageSize = new it.Rectangle(anchoPts, altoPts);
                        if ((bPdfCompleto?.Length).GetValueOrDefault() > 0)
                        {
                            using (var pdfReader = new PdfReader(bPdfCompleto))
                            using (var pdfStamper = new PdfStamper(pdfReader, memStream))
                            {
                                int numPagina = pdfReader.NumberOfPages + 1;
                                pdfStamper.InsertPage(numPagina, pageSize);
                                dibujarPlantillaFondo(pdfStamper.GetOverContent(numPagina));
                                pdfStamper.Close();
                                pdfReader.Close();
                            }
                        }
                        else
                        {
                            using (var pdfDoc = new it.Document(pageSize, 0, 0, 0, 0))
                            using (var pdfWriter = PdfWriter.GetInstance(pdfDoc, memStream))
                            {
                                pdfDoc.Open();
                                dibujarPlantillaFondo(pdfWriter.DirectContent);
                                pdfDoc.Close();
                                pdfWriter.Close();
                            }
                        }
                        bPdf = memStream.ToArray();
                    }
                    #endregion

                    #region Dibujo Contenido del Ploteo
                    using (var memStream = new MemoryStream())
                    using (var pdfReader = new PdfReader(bPdf))
                    using (var pdfStamper = new PdfStamper(pdfReader, memStream))
                    {
                        PdfContentByte pdfContentByte = null;
                        #region Ploteo de Imagen Satelital
                        if (idImagenSatelital > 0) //Indicador en plantilla de si busca imagenes en el wms
                        {
                            #region Busco y dibujo imagen de fondo del WMS
                            //Le paso el height pero lo recalcula en base al ratio de las coords
                            int wmsWidth = 1295;
                            int wmsHeight = 765;
                            string wmsUrl = @"http://192.168.1.117/erdas-iws/ogc/wms/Mosaico";
                            string wmsLayers = "Mosaico_wgs84_mos2010_ll84.ecw";
                            string wmsSRS = "EPSG:4326";
                            string wmsFormat = "image/png";

                            if (hoja.Ancho_Imagen_px.GetValueOrDefault() > 0)
                            {
                                wmsWidth = Math.Min(hoja.Ancho_Imagen_px.Value, 4000);
                            }
                            var imagenSatelital = GetImagenSatelitalById(idImagenSatelital);
                            if (imagenSatelital != null)
                            {
                                wmsUrl = imagenSatelital.URL;
                                wmsLayers = imagenSatelital.Layers;
                                wmsSRS = imagenSatelital.SRS;
                                wmsFormat = imagenSatelital.Format;
                                imagenSatelitalName = $"{imagenSatelital.Nombre} {(imagenTransparencia > 0f && imagenTransparencia < 1f ? $"al {(imagenTransparencia * 100)}%" : string.Empty)}".Trim();
                            }
                            var wmsImage = GetImageFromWMS(wmsUrl, wmsLayers,
                                                           TransformCoords(xMinBuffImpresion, yMinBuffImpresion, xMaxBuffImpresion, yMaxBuffImpresion),
                                                           wmsWidth, wmsHeight, wmsSRS, wmsFormat);
                            if (wmsImage != null)
                            {
                                //a menor el nro mas transparente. Cuanto menos opaco mas transparente
                                float opacity = 1f - imagenTransparencia;
                                if (opacity > 0f && opacity < 1f)
                                {
                                    wmsImage = SetImageOpacity(wmsImage, opacity);
                                    plantilla.Transparencia = (int?)(imagenTransparencia * 100);
                                    _plantillaRepository.UpdatePlantillaTransparencia(plantilla.IdPlantilla, (int)plantilla.Transparencia);
                                }
                                float wmsAnchoPts = it.Utilities.MillimetersToPoints((float)(plantilla.X_Util));
                                float wmsAltoPts = it.Utilities.MillimetersToPoints((float)(plantilla.Y_Util));
                                pdfx1c = it.Utilities.MillimetersToPoints((float)(plantilla.ImpresionXMin));
                                pdfy1c = it.Utilities.MillimetersToPoints((float)(plantilla.ImpresionYMin));

                                pdfContentByte = pdfStamper.GetOverContent(pdfReader.NumberOfPages); //siempre tomo la ultima

                                if (plantilla.OptimizarTamanioHoja)
                                {
                                    PointD ptBuffImpresionMinMin2 = RotateD(xMinBuffImpresion, yMinBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                                    PointD ptBuffImpresionMaxMax2 = RotateD(xMaxBuffImpresion, yMaxBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                                    PointD ptBuffImpresionMaxMin2 = RotateD(xMaxBuffImpresion, yMinBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
                                    PointD ptBuffImpresionMinMax2 = RotateD(xMinBuffImpresion, yMaxBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);

                                    var lstX2 = new[] { ptBuffImpresionMinMin2.X, ptBuffImpresionMaxMax2.X, ptBuffImpresionMaxMin2.X, ptBuffImpresionMinMax2.X };
                                    var lstY2 = new[] { ptBuffImpresionMinMin2.Y, ptBuffImpresionMaxMax2.Y, ptBuffImpresionMaxMin2.Y, ptBuffImpresionMinMax2.Y };

                                    double xMinBuffImpresion2 = lstX2.Min();
                                    double yMinBuffImpresion2 = lstY2.Min();
                                    double xMaxBuffImpresion2 = lstX2.Max();
                                    double yMaxBuffImpresion2 = lstY2.Max();

                                    float x1Pdf = GetXPDFCanvas(xMinBuffImpresion2, xCentroidBase, factorEscala, plantilla);
                                    float y1Pdf = GetYPDFCanvas(yMinBuffImpresion2, yCentroidBase, factorEscala, plantilla);
                                    float x2Pdf = GetXPDFCanvas(xMaxBuffImpresion2, xCentroidBase, factorEscala, plantilla);
                                    float y2Pdf = GetYPDFCanvas(yMaxBuffImpresion2, yCentroidBase, factorEscala, plantilla);

                                    wmsAnchoPts = x2Pdf - x1Pdf;
                                    wmsAltoPts = y2Pdf - y1Pdf;

                                    pdfx1c = x1Pdf;
                                    pdfy1c = y1Pdf;

                                    float anguloGrados = (float)(anguloRotacion * 180 / Math.PI) * (-1);
                                    PDFUtilities.DrawPDFImage(pdfContentByte, RotateImage2(wmsImage, anguloGrados), pdfx1c, pdfy1c, wmsAnchoPts, wmsAltoPts, 0);
                                }
                                else
                                {
                                    PDFUtilities.DrawPDFImage(pdfContentByte, wmsImage, pdfx1c, pdfy1c, wmsAnchoPts, wmsAltoPts, (float)anguloRotacion);
                                }
                            }
                            #endregion
                        }
                        #endregion
                        #region Ploteo de Objetos
                        void dibujarLayer(Layer layer, LayerGraf[] objetos, Action<LayerGraf[]> dibujar)
                        {
                            lstLayersReferencia.Add(layer);
                            calcbbox(objetos);
                            pdfContentByte.BeginLayer(new PdfLayer(layer.Nombre, pdfContentByte.PdfWriter));
                            dibujar(objetos);
                            pdfContentByte.EndLayer();
                        };
                        foreach (Layer layer in lstLayersOrdenados)
                        {
                            pdfContentByte = pdfStamper.GetOverContent(pdfReader.NumberOfPages); //siempre tomo la ultima
                            #region test temporal
                            //var polygon = new[] {
                            //                new PointF(300, 300),
                            //                new PointF(300, 310),
                            //                new PointF(310, 310),
                            //                new PointF(310, 350),
                            //                new PointF(350, 350),
                            //                new PointF(350, 420),
                            //                new PointF(400, 420),
                            //                new PointF(420, 350),
                            //                new PointF(350, 350),
                            //                new PointF(350, 310),
                            //                new PointF(310, 310),
                            //                new PointF(310, 300),
                            //                new PointF(300, 300)
                            //            }.ToList();

                            //float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;

                            //it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                            //it.BaseColor pdfRellenoColor = null;
                            //if (layer.Relleno)
                            //    pdfRellenoColor = GetAlphaColor(ColorTranslator.FromHtml(layer.RellenoColor), layer.RellenoTransparencia);
                            //PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);
                            //string lineDash = layer.Dash;

                            //Color etiquetaColor = Color.Black;
                            //it.BaseColor pdfEtiquetaColor = null;
                            //float pdfFontSize = 0;
                            //string[] aFontStylePdf = null;
                            //int pdfFontStyle = 0;
                            //BaseFont pdfbaseFont = null;
                            //it.Font pdfFont = null;

                            //if (layer.Etiqueta)
                            //{
                            //    etiquetaColor = (!string.IsNullOrEmpty(layer.EtiquetaColor) ? ColorTranslator.FromHtml(layer.EtiquetaColor) : Color.Black);
                            //    pdfEtiquetaColor = new it.BaseColor(etiquetaColor.R, etiquetaColor.G, etiquetaColor.B);
                            //    pdfFontSize = (layer.EtiquetaFuenteTamanio != null ? it.Utilities.MillimetersToPoints((float)layer.EtiquetaFuenteTamanio.Value) : 0);
                            //    if (pdfFontSize > 0)
                            //    {
                            //        PDFUtilities.RegisterBaseFont(layer.EtiquetaFuenteNombre, pdfFontSize);
                            //        aFontStylePdf = layer.EtiquetaFuenteEstilo.Split(',');
                            //        pdfFontStyle = aFontStylePdf.Select(fs => Convert.ToInt32(fs)).Sum();
                            //        pdfbaseFont = it.FontFactory.GetFont(layer.EtiquetaFuenteNombre, pdfFontSize, pdfFontStyle, pdfEtiquetaColor).BaseFont;
                            //        pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfEtiquetaColor);

                            //    }
                            //}
                            //pdfContentByte.BeginLayer(new PdfLayer(layer.Nombre, pdfContentByte.PdfWriter));
                            //PDFUtilities.DrawPDFPolygon(pdfContentByte, polygon, pdfContornoColor, 1, pdfRellenoColor);
                            //PDFUtilities.DrawPDFText(pdfContentByte, layer.Nombre, 360, 522, new it.Font(it.FontFactory.GetFont("Helvetica").BaseFont, 10), 10, 0);
                            //pdfContentByte.EndLayer();
                            #endregion
                            #region Dibujar Layer
                            //LayerPDF
                            if (layerBase != null && layer.IdLayer == layerBase.IdLayer && !string.IsNullOrEmpty(idObjetoGraf))
                            {
                                dibujarLayer(layer, aLayerGrafBase, (objetos) =>
                                {
                                    DibujarLayerGraf(pdfContentByte, objetos, "Nombre", layerBase, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                                });
                            }
                            else if (layerSecundario != null && layer.IdLayer == layerSecundario.IdLayer)
                            {
                                if (!string.IsNullOrEmpty(idsObjetoSecundario))
                                {
                                    dibujarLayer(layer, aLayerGrafSecundario, (objetos) =>
                                    {
                                        DibujarLayerGraf(pdfContentByte, objetos, "Nombre", layerSecundario, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                                    });
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(idObjetoGraf) || (!string.IsNullOrEmpty(layer.Componente.Capa) && lstLayersVisibles.Any(l => layer.Componente.EsComponenteByCapa(l))))
                                {
                                    //Busco Layer
                                    LayerGraf[] aLayerGraf = null;
                                    if (layer.FiltroGeografico == 1)
                                    {
                                        if (!string.IsNullOrEmpty(idObjetoGraf))
                                        {
                                            aLayerGraf = GetLayerGrafByObjetoBase(layer, layer.Componente, layerBase.Componente, lstCoordsBuffImpresion, idObjetoGraf, anio, esInformeAnual);
                                        }
                                        else
                                        {
                                            layer.FiltroGeografico = 0;
                                            aLayerGraf = GetLayerGrafByCoords(layer, layer.Componente, lstCoordsBuffImpresion, anio);
                                        }

                                        if (layer.Componente.ComponenteId == ID_COMPONENTE_ARCO && aLayerGraf != null)
                                        {
                                            aLayerGraf = GetLayersGraf(aLayerGraf);
                                        }
                                    }
                                    else if (layer.FiltroGeografico == 2)
                                    {
                                        layer.FiltroGeografico = 1;
                                        LayerGraf[] aLayerGrafInside = GetLayerGrafByObjetoBase(layer, layer.Componente, layerBase.Componente, lstCoordsBuffImpresion, idObjetoGraf, anio, esInformeAnual);

                                        layer.FiltroGeografico = 0;
                                        LayerGraf[] aLayerGrafAnyinteract = GetLayerGrafByCoords(layer, layer.Componente, lstCoordsBuffImpresion, anio);
                                        if (aLayerGrafInside != null && aLayerGrafAnyinteract != null)
                                        {
                                            var list2Lookup = aLayerGrafInside.ToList().ToLookup(lyGraf => lyGraf.FeatId);
                                            var listdiff = aLayerGrafAnyinteract.ToList().Where(lyGraf => (!list2Lookup.Contains(lyGraf.FeatId)));
                                            aLayerGraf = listdiff.ToArray();
                                        }
                                        else
                                        {
                                            if (aLayerGrafAnyinteract != null)
                                            {
                                                aLayerGraf = aLayerGrafAnyinteract;
                                            }
                                        }
                                        layer.FiltroGeografico = 2;

                                        if (layer.Componente.ComponenteId == ID_COMPONENTE_ARCO && aLayerGraf != null)
                                        {
                                            aLayerGraf = GetLayersGraf(aLayerGraf);
                                        }
                                    }
                                    else if (layer.FiltroGeografico == 3)
                                    {
                                        aLayerGraf = GetLayerGrafByObjetoBaseIntersect(layer, layer.Componente, layerBase.Componente, idObjetoGraf, anio, esInformeAnual);
                                    }
                                    else
                                    {
                                        aLayerGraf = GetLayerGrafByCoords(layer, layer.Componente, lstCoordsBuffImpresion, anio);
                                        if (layer.Componente.ComponenteId == ID_COMPONENTE_ARCO && aLayerGraf != null)
                                        {
                                            aLayerGraf = GetLayersGraf(aLayerGraf);
                                        }
                                    }
                                    if (aLayerGraf != null && aLayerGraf.Length > 0)
                                    {
                                        dibujarLayer(layer, aLayerGraf, (objetos) =>
                                        {
                                            DibujarLayerGraf(pdfContentByte, objetos, "Nombre", layer, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                                        });
                                    }
                                }
                                else if (string.IsNullOrEmpty(idObjetoGraf) && !string.IsNullOrEmpty(layer.Componente.Capa) && !lstLayersVisibles.Any(l => layer.Componente.EsComponenteByCapa(l)))
                                { // tiene filtro por layers visibles y el layer en cuestion no está prendido en el mapa
                                    continue;
                                }
                                else
                                {
                                    //Viene de Mapa Tematico
                                    //Busco Layer
                                    LayerGraf[] aLayerGraf = null;
                                    if (idComponentePrincipal != null && !verContexto && layerGrafBase != null)
                                    {
                                        layer.FiltroGeografico = 1;
                                        aLayerGraf = GetLayerGrafByObjetoBase(layer, layer.Componente, layerBase.Componente, lstCoordsBuffImpresion, layerGrafBase.FeatId.ToString(), anio, esInformeAnual);
                                    }
                                    else
                                    {
                                        layer.FiltroGeografico = 0;
                                        aLayerGraf = GetLayerGrafByCoords(layer, layer.Componente, lstCoordsBuffImpresion, anio);
                                    }
                                    if (aLayerGraf != null && aLayerGraf.Length > 0)
                                    {
                                        dibujarLayer(layer, aLayerGraf, (objetos) =>
                                        {
                                            DibujarLayerGraf(pdfContentByte, objetos, "Nombre", layer, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                                        });
                                    }
                                }
                            }
                            #endregion
                        }
                        if (objetoResultadoDetalle != null && !string.IsNullOrEmpty(objetoResultadoDetalle.GUID))
                        {
                            //Viene de Mapa Tematico
                            //Dibujar a partir de MT_OBJETO_RESULTADO
                            LayerGraf[] aLayerGrafMT = GetLayerGrafByMapaTematico(objetoResultadoDetalle.GUID);
                            if (aLayerGrafMT != null && aLayerGrafMT.Length > 0)
                            {
                                DibujarLayerGrafMapaTematico(pdfContentByte, aLayerGrafMT, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario, objetoResultadoDetalle, verIdentificante);
                            }
                        }

                        grosorMaximo = it.Utilities.MillimetersToPoints((float)lstLayersReferencia.Select(l => l.ContornoGrosor).Max());
                        #endregion

                        #region Dibujo Extras
                        pdfContentByte = pdfStamper.GetOverContent(pdfReader.NumberOfPages); //siempre tomo la ultima
                        if (objetoResultadoDetalle != null && !string.IsNullOrEmpty(objetoResultadoDetalle.GUID))
                        {
                            int puntoPredeterminado = objetoResultadoDetalle.GeometryType == 1 ? 1 : 2;
                            short iOrd = 99;
                            lstLayersReferencia = objetoResultadoDetalle
                                                        .Rangos
                                                        .Select(rango => new Layer()
                                                        {
                                                            Orden = iOrd--,
                                                            Nombre = rango.Leyenda,
                                                            ContornoColor = "#" + rango.ColorBorde,
                                                            ContornoGrosor = rango.AnchoBorde,
                                                            Relleno = true,
                                                            RellenoColor = "#" + rango.Color,
                                                            RellenoTransparencia = Convert.ToInt32(objetoResultadoDetalle.Transparencia),
                                                            Contorno = rango.AnchoBorde > 0,
                                                            PuntoRepresentacion = 1,
                                                            PuntoPredeterminado = puntoPredeterminado
                                                        }).ToList();
                        }

                        if (!esInformeAnual)
                        {
                            //Dibujo Referencias
                            DibujarReferencias(pdfContentByte, plantilla, lstLayersReferencia);
                        }

                        //Busco y dibujo el Norte
                        Norte norte = GetNorteById(plantilla.IdNorte);
                        if (norte != null)
                        {
                            #region Norte
                            float norteAnchoPts = it.Utilities.MillimetersToPoints((float)plantilla.NorteAncho);
                            float norteAltoPts = it.Utilities.MillimetersToPoints((float)plantilla.NorteAlto);
                            pdfx1c = it.Utilities.MillimetersToPoints((float)plantilla.NorteX);
                            pdfy1c = it.Utilities.MillimetersToPoints((float)plantilla.NorteY);
                            Bitmap bmpNorte = (Bitmap)norte.Imagen;
                            if (anguloRotacion != 0)
                            {
                                //Roto ptos de imagen Norte
                                double xNorMin = plantilla.NorteX;
                                double yNorMin = plantilla.NorteY;
                                double xNorMax = plantilla.NorteX + plantilla.NorteAncho;
                                double yNorMax = plantilla.NorteY + plantilla.NorteAlto;
                                double xCentroidPlantilla = xNorMin + (xNorMax - xNorMin) / 2;
                                double yCentroidPlantilla = yNorMin + (yNorMax - yNorMin) / 2;

                                PointF ptNorMinMin = Rotate((float)xNorMin, (float)yNorMin, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);
                                PointF ptNorMaxMax = Rotate((float)xNorMax, (float)yNorMax, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);
                                PointF ptNorMaxMin = Rotate((float)xNorMax, (float)yNorMin, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);
                                PointF ptNorMinMax = Rotate((float)xNorMin, (float)yNorMax, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);

                                var lstX = new[] { ptNorMinMin.X, ptNorMaxMax.X, ptNorMaxMin.X, ptNorMinMax.X };
                                var lstY = new[] { ptNorMinMin.Y, ptNorMaxMax.Y, ptNorMaxMin.Y, ptNorMinMax.Y };

                                xNorMin = lstX.Min();
                                yNorMin = lstY.Min();
                                xNorMax = lstX.Max();
                                yNorMax = lstY.Max();

                                pdfx1c = it.Utilities.MillimetersToPoints((float)xNorMin);
                                pdfy1c = it.Utilities.MillimetersToPoints((float)yNorMin);
                                float pdfx2c = it.Utilities.MillimetersToPoints((float)xNorMax);
                                float pdfy2c = it.Utilities.MillimetersToPoints((float)yNorMax);
                                norteAnchoPts = pdfx2c - pdfx1c;
                                norteAltoPts = pdfy2c - pdfy1c;

                                float anguloGrados = (float)(anguloRotacion * 180 / Math.PI) * (-1);
                                Bitmap bmpNorteRotado = RotateImage2(norte.Imagen, anguloGrados);
                                PDFUtilities.DrawPDFImage(pdfContentByte, bmpNorteRotado, pdfx1c, pdfy1c, norteAnchoPts, norteAltoPts, 0);
                            }
                            else
                            {
                                PDFUtilities.DrawPDFImage(pdfContentByte, bmpNorte, pdfx1c, pdfy1c, norteAnchoPts, norteAltoPts, (float)anguloRotacion);
                            }
                            #endregion
                        }

                        //Dibujo Textos de la plantilla
                        if (plantilla.PlantillaTextos != null)
                        {
                            Dictionary<string, string> dicTextosVariables = new Dictionary<string, string>();
                            if (!string.IsNullOrEmpty(textosVariables))
                            {
                                dicTextosVariables = textosVariables.Split(';').Select(t => t.Split(',')).ToDictionary(t => t[0].ToUpper(), t => t[1]);
                            }
                            DibujarTextos(pdfContentByte, plantilla, idObjetoGraf, idComponenteObjetoGraf, factorEscala, dicTextosVariables, xCentroidBase, yCentroidBase, imagenSatelitalName, numeroODT, tipo, relacion, direccionCompleta, datosReferencia);
                        }

                        //Funciones Especiales
                        if (verCotas && plantilla.FuncionAdicional != null && plantilla.FuncionAdicional.IdFuncionAdicional == 2)
                        {
                            plantilla.IdFuncionAdicional = 0;
                        }
                        if (plantilla.IdFuncionAdicional != null && plantilla.IdFuncionAdicional > 0)
                        {
                            var modPlotFuncionesEspeciales = new ModPlotFuncionesEspeciales(_parcelaPlotRepository, _cuadraPlotRepository, _layerGrafRepository, _plantillaRepository, _manzanaPlotRepository, _callePlotRepository, _expansionPlotRepository, _parametroRepository, _tipoPlanoRepository, _partidoRepository, _censoRepository, _atributoRepository, _componenteRepository);
                            //_ = modPlotFuncionesEspeciales.EjecutarFuncionEspecial((int)plantilla.IdFuncionAdicional, new PdfDocument(), pdfContentByte, plantilla, idPlantillaFondo, layerBase.Componente, idObjetoGraf, idsObjetoSecundario, lstCoordsGeometryBase, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, grafico, leyenda, infoLeyenda, lstLayersReferencia, anio, lstCoordsBuffImpresion, null, lstAtributos, idsSeleccionados, idComponenteSelec);
                            _ = modPlotFuncionesEspeciales.EjecutarFuncionEspecial((int)plantilla.IdFuncionAdicional, pdfContentByte, plantilla, layerBase.Componente, idObjetoGraf, idsObjetoSecundario, lstCoordsGeometryBase, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro);
                        }
                        if (verCotas)
                        {
                            int? old = plantilla.IdFuncionAdicional;
                            plantilla.IdFuncionAdicional = 2;
                            plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                            var modPlotFuncionesEspeciales = new ModPlotFuncionesEspeciales(_parcelaPlotRepository, _cuadraPlotRepository, _layerGrafRepository, _plantillaRepository, _manzanaPlotRepository, _callePlotRepository, _expansionPlotRepository, _parametroRepository, _tipoPlanoRepository, _partidoRepository, _censoRepository, _atributoRepository, _componenteRepository);
                            //_ = modPlotFuncionesEspeciales.EjecutarFuncionEspecial((int)plantilla.IdFuncionAdicional, new PdfDocument(), pdfContentByte, plantilla, idPlantillaFondo, layerBase.Componente, idObjetoGraf, idsObjetoSecundario, lstCoordsGeometryBase, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, grafico, leyenda, infoLeyenda, lstLayersReferencia, anio, lstCoordsBuffImpresion, null, lstAtributos);
                            _ = modPlotFuncionesEspeciales.EjecutarFuncionEspecial((int)plantilla.IdFuncionAdicional, pdfContentByte, plantilla, layerBase.Componente, idObjetoGraf, idsObjetoSecundario, lstCoordsGeometryBase, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro);
                            plantilla.IdFuncionAdicional = old;
                        }


                        //no se por qué dibuja éste recuadro y hasta saberlo lo comento
                        #region Dibujo un cuadrado blanco para borrar el borde
                        var puntos = Enumerable.Range(1, 4).Select(num => bbox.Envelope.PointAt(num));
                        impresionXMin = GetXPDFCanvas((float)puntos.Min(p => p.XCoordinate), xCentroidBase, factorEscala, plantilla);
                        impresionYMin = GetYPDFCanvas((float)puntos.Min(p => p.YCoordinate), yCentroidBase, factorEscala, plantilla);
                        impresionXMax = GetXPDFCanvas((float)puntos.Max(p => p.XCoordinate), xCentroidBase, factorEscala, plantilla);
                        impresionYMax = GetYPDFCanvas((float)puntos.Max(p => p.YCoordinate), yCentroidBase, factorEscala, plantilla);
                        grosorMaximo += grosorMaximo * 10 / 100;
                        PDFUtilities.DrawPDFRectangle2(pdfContentByte, (float)impresionXMin, (float)impresionYMin, (float)impresionXMax, (float)impresionYMax, new it.BaseColor(Color.White), (float)grosorMaximo, null, null);
                        #endregion
                        #endregion

                        memStream.Flush();
                        pdfStamper.Close();
                        pdfReader.Close();
                        bPdf = null;
                        return memStream.ToArray();
                    }
                    #endregion
                }
            }
            catch (ArgumentNullException ex)
            {
                Global.GetLogger().LogError("GetPlantilla", ex);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("GetPlantilla", ex);
            }
            return null;
        }

        //public byte[] GetPlantillaFrecuente(int idPlantilla, string idObjetoGraf, int idComponenteObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, string extent, string scale, string layersVisibles, bool verCotas, ObjetoResultadoDetalle objetoResultadoDetalle, int idImagenSatelital, float imagenTransparencia, bool verIdentificante, long? idComponentePrincipal, bool verContexto, InformacionComercial grafico = null, InformacionComercial leyenda = null, int? infoLeyenda = null, bool esInformeAnual = false, string anio = "", int? idPlotFrec = null)
        //{
        //    //Viene de PloteoFrecuente

        //    byte[] bPdf = null;
        //    try
        //    {
        //        long ID_COMPONENTE_ARCO = GetComponenteArco();

        //        //En MP_PLANTILLA_VIEWPORT tiene los viewport y la plantilla principal, por lo que no se usa plantillaPrincipal.

        //        //Agregar la plantilla principal. 
        //        List<Plantilla> lstViewports = _plantillaViewportRepository.ObtenerAllPlantillasViewPortsInPlantillaByPloteoFrecuente((int)idPlotFrec, idObjetoGraf);

        //        //Idea:
        //        //Cargar todas las plantillas con las coordenadas de su geometria.
        //        Plantilla plantillaPrincipal = GetPlantillaById(lstViewports[0].IdPlantilla);//Todos los viewport de un ploteo frecuente tienen la misma plantilla


        //        List<Plantilla> lstPlantilla = new List<Plantilla>() { };
        //        lstPlantilla.AddRange(lstViewports);

        //        PlantillaFondo plantillaFondo = GetPlantillaFondoById(idPlantillaFondo);
        //        if (plantillaPrincipal != null && plantillaFondo != null)
        //        {
        //            #region configuracion plantilla
        //            Hoja hoja = GetHojaById(plantillaPrincipal.IdHoja);
        //            plantillaPrincipal.PPP = plantillaFondo.Resolucion.Valor;
        //            float anchoPts = it.Utilities.MillimetersToPoints((float)hoja.Ancho_mm);
        //            float altoPts = it.Utilities.MillimetersToPoints((float)hoja.Alto_mm);
        //            if (plantillaPrincipal.Orientacion == 0)
        //            {
        //                plantillaPrincipal.WidthPts = anchoPts;
        //                plantillaPrincipal.HeigthPts = altoPts;
        //            }
        //            else
        //            {
        //                plantillaPrincipal.WidthPts = altoPts;
        //                plantillaPrincipal.HeigthPts = anchoPts;
        //                anchoPts = it.Utilities.MillimetersToPoints((float)hoja.Alto_mm);
        //                altoPts = it.Utilities.MillimetersToPoints((float)hoja.Ancho_mm);
        //            }
        //            List<Layer> lstLayersReferencia = new List<Layer>();

        //            float impresionXMinPrincipal = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.ImpresionXMin);
        //            float impresionYMinPrincipal = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.ImpresionYMin);
        //            float impresionXMaxPrincipal = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.ImpresionXMax);
        //            float impresionYMaxPrincipal = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.ImpresionYMax);

        //            var pdfDoc = new it.Document(new it.Rectangle(anchoPts, altoPts), 0, 0, 0, 0);

        //            double xPrincipal = 0, yPrincipal = 0;
        //            float pdfx1cPrincipal = 0, pdfy1cPrincipal = 0;
        //            double xCentroidBasePrincipal = 0, yCentroidBasePrincipal = 0;
        //            double escalaPrincipal = 0, factorEscalaPrincipal = 0;
        //            double xMinBuffPrincipal = 9999999, yMinBuffPrincipal = 9999999, xMaxBuffPrincipal = 0, yMaxBuffPrincipal = 0;

        //            #endregion

        //            //Puede que convenga ponerlo dentro del contexto de dibujar viewports
        //            #region Configuracion Principal

        //            if (plantillaPrincipal.Layers == null || plantillaPrincipal.Layers.Count == 0)
        //                throw new Exception("La plantilla seleccionada no tiene layers asignados");

        //            float anguloRotacionPrincipal = 0;

        //            bool sentidoHorarioPrincipal = false;

        //            #endregion configuracion

        //            using (var pdfReaderPlantilla = new PdfReader(plantillaFondo.PDFMemoryStream))
        //            using (var memStreamOut = new MemoryStream())
        //            {
        //                #region Dibujo Plantilla Fondo
        //                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, memStreamOut);
        //                pdfDoc.Open();

        //                pdfWriter.ViewerPreferences = PdfWriter.PageModeUseOC;
        //                pdfWriter.PdfVersion = PdfWriter.VERSION_1_5;

        //                PdfContentByte pdfContentByteOut = pdfWriter.DirectContent;

        //                #region AddTemplate con Posible Rotation en Plantilla PDF

        //                var importedPagePlantilla = pdfWriter.GetImportedPage(pdfReaderPlantilla, 1);
        //                var pageWidth = pdfReaderPlantilla.GetPageSizeWithRotation(1).Width;
        //                var pageHeight = pdfReaderPlantilla.GetPageSizeWithRotation(1).Height;

        //                switch (importedPagePlantilla.Rotation)
        //                {
        //                    case 0:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 1f, 0, 0, 1f, 0, 0);
        //                        break;
        //                    case 90:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 0, -1f, 1f, 0, 0, pageHeight);
        //                        break;
        //                    case 180:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, -1f, 0, 0, -1f, pageWidth, pageHeight);
        //                        break;
        //                    case 270:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 0, 1f, -1f, 0, pageWidth, 0);
        //                        break;
        //                    default:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 1f, 0, 0, 1f, 0, 0);
        //                        break;
        //                }
        //                #endregion

        //                pdfDoc.Close();
        //                pdfWriter.Close();

        //                bPdf = memStreamOut.ToArray();
        //                #endregion
        //            }


        //            using (PdfReader pdfReader = new PdfReader(bPdf))
        //            using (MemoryStream memStreamTemp = new MemoryStream())
        //            {
        //                #region Dibujo Viewports c/ funciones adicionales

        //                PdfStamper pdfStamper = new PdfStamper(pdfReader, memStreamTemp);
        //                PdfWriter pdfWriter = pdfStamper.Writer;
        //                PdfContentByte pdfContentByte = pdfStamper.GetOverContent(1);

        //                foreach (var plantilla in lstPlantilla)
        //                {
        //                    #region configuracion
        //                    float impresionXMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMin);
        //                    float impresionYMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMin);
        //                    float impresionXMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMax);
        //                    float impresionYMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMax);


        //                    impresionXMinPrincipal = impresionXMinPrincipal < impresionXMin ? impresionXMinPrincipal : impresionXMin;
        //                    impresionYMinPrincipal = impresionYMinPrincipal < impresionYMin ? impresionYMinPrincipal : impresionYMin;
        //                    impresionXMaxPrincipal = impresionXMaxPrincipal > impresionXMax ? impresionXMaxPrincipal : impresionXMax;
        //                    impresionYMaxPrincipal = impresionYMaxPrincipal > impresionYMax ? impresionYMaxPrincipal : impresionYMax;

        //                    double x = 0, y = 0;
        //                    double xCentroidBase = 0, yCentroidBase = 0;
        //                    double escala = 0, factorEscala = 0;
        //                    double xMinBuff = 9999999, yMinBuff = 9999999, xMaxBuff = 0, yMaxBuff = 0;


        //                    double grosorMaximo = 0;//Para dibujar el recuadro blanco para tapar el recorte de las geometrias.

        //                    LayerGraf[] aLayerGrafBase = null;
        //                    LayerGraf layerGrafBase = null;
        //                    if (plantilla.Layers == null || plantilla.Layers.Count == 0)
        //                        throw new Exception("La plantilla seleccionada no tiene layers asignados");
        //                    DbGeometry geometryBase = null;
        //                    DbGeometry bufferBase = null;

        //                    List<string> lstCoordsGeometryBase = new List<string>();
        //                    List<string> lstCoordsBuffImpresion = new List<string>();

        //                    if (!string.IsNullOrEmpty(idObjetoGraf))
        //                    {
        //                        if (plantilla.esViewport)//Si es viewport, usa la geometria de l aplantilla.
        //                            aLayerGrafBase = plantilla.Geometry;

        //                        if (aLayerGrafBase != null && aLayerGrafBase.Length > 0)
        //                        {
        //                            geometryBase = aLayerGrafBase[0].Geom;
        //                            for (int i = 1; i < aLayerGrafBase.Length; i++)
        //                            {
        //                                geometryBase = geometryBase.Union(aLayerGrafBase[i].Geom);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //Global.GetLogger().LogInfo(string.Format("El objeto {0} del componente {1} no tiene gráficos. No se puede graficar el objeto base por lo que se cancela el ploteo", idObjetoGraf, layerBase.Componente.Nombre));
        //                            return null;
        //                        }
        //                        if (geometryBase != null)
        //                        {
        //                            if (geometryBase.Centroid != null)
        //                            {
        //                                xCentroidBase = (double)geometryBase.Centroid.XCoordinate;
        //                                yCentroidBase = (double)geometryBase.Centroid.YCoordinate;
        //                            }
        //                            else
        //                            {
        //                                var centroide = geometryBase.Envelope.Centroid;
        //                                xCentroidBase = (double)centroide.XCoordinate;
        //                                yCentroidBase = (double)centroide.YCoordinate;
        //                            }

        //                            int cantCoordsObjBase = geometryBase.PointCount.HasValue ? geometryBase.PointCount.Value : 1;
        //                            for (int i = 1; i <= cantCoordsObjBase; i++)
        //                            {
        //                                x = (double)geometryBase.PointAt(i).XCoordinate;
        //                                y = (double)geometryBase.PointAt(i).YCoordinate;
        //                                lstCoordsGeometryBase.Add(x.ToString().Replace(",", ".") + ", " + y.ToString().Replace(",", "."));
        //                            }

        //                            //Buffer
        //                            if (plantilla.DistBuffer == 0)
        //                            {
        //                                bufferBase = geometryBase;
        //                            }
        //                            else
        //                            {
        //                                bufferBase = geometryBase.Buffer(plantilla.DistBuffer);
        //                            }

        //                            #region Extents del Buffer
        //                            int cantCoordsBuff = (int)bufferBase.PointCount;
        //                            for (int i = 1; i <= cantCoordsBuff; i++)
        //                            {
        //                                x = (double)bufferBase.PointAt(i).XCoordinate;
        //                                y = (double)bufferBase.PointAt(i).YCoordinate;
        //                                if (x <= xMinBuff)
        //                                {
        //                                    xMinBuff = x;
        //                                }
        //                                if (x >= xMaxBuff)
        //                                {
        //                                    xMaxBuff = x;
        //                                }
        //                                if (y <= yMinBuff)
        //                                {
        //                                    yMinBuff = y;
        //                                }
        //                                if (y >= yMaxBuff)
        //                                {
        //                                    yMaxBuff = y;
        //                                }
        //                            }
        //                            #endregion

        //                            xCentroidBase = xMinBuff + ((xMaxBuff - xMinBuff) / 2.0);
        //                            yCentroidBase = yMinBuff + ((yMaxBuff - yMinBuff) / 2.0);

        //                            //Calculo la escala en base a los minimos y maximos del buffer en vez de la manzana
        //                            #region Calculo de Escala
        //                            if (((plantilla.X_Util / 1000) * (yMaxBuff - yMinBuff)) > ((plantilla.Y_Util / 1000) * (xMaxBuff - xMinBuff)))
        //                            {
        //                                factorEscala = (plantilla.Y_Util / 1000) / (yMaxBuff - yMinBuff);
        //                            }
        //                            else
        //                            {
        //                                factorEscala = (plantilla.X_Util / 1000) / (xMaxBuff - xMinBuff);
        //                            }
        //                            escala = 1 / factorEscala;

        //                            int escalaPlantilla = Convert.ToInt32(Math.Ceiling(escala));
        //                            var lstEscala = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).ToList();
        //                            if (lstEscala != null && lstEscala.Count > 0)
        //                            {
        //                                escalaPlantilla = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).OrderBy(p => p.Escala).Select(p => p.Escala).First();
        //                            }
        //                            factorEscala = 1.0 / escalaPlantilla;
        //                            escala = 1 / factorEscala;
        //                            #endregion
        //                        }
        //                    }

        //                    //Obtengo el buffer con respecto al area de impresion en coordenadas cartograficas
        //                    double xMinBuffImpresion = GetXBuffer(plantilla.ImpresionXMin, xCentroidBase, factorEscala, plantilla);
        //                    double yMinBuffImpresion = GetYBuffer(plantilla.ImpresionYMin, yCentroidBase, factorEscala, plantilla);
        //                    double xMaxBuffImpresion = GetXBuffer(plantilla.ImpresionXMax, xCentroidBase, factorEscala, plantilla);
        //                    double yMaxBuffImpresion = GetYBuffer(plantilla.ImpresionYMax, yCentroidBase, factorEscala, plantilla);

        //                    lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString().Replace(",", ".") + ", " + yMinBuffImpresion.ToString().Replace(",", "."));
        //                    lstCoordsBuffImpresion.Add(xMaxBuffImpresion.ToString().Replace(",", ".") + ", " + yMinBuffImpresion.ToString().Replace(",", "."));
        //                    lstCoordsBuffImpresion.Add(xMaxBuffImpresion.ToString().Replace(",", ".") + ", " + yMaxBuffImpresion.ToString().Replace(",", "."));
        //                    lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString().Replace(",", ".") + ", " + yMaxBuffImpresion.ToString().Replace(",", "."));
        //                    lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString().Replace(",", ".") + ", " + yMinBuffImpresion.ToString().Replace(",", "."));

        //                    //Array utilizado para buscar imagen wms de fondo
        //                    double[] aTransformCoords = TransformCoords(xMinBuffImpresion, yMinBuffImpresion, xMaxBuffImpresion, yMaxBuffImpresion);

        //                    double anguloRotacion = 0;
        //                    double anguloRotacionFiltro = 0;
        //                    bool sentidoHorario = false;
        //                    if (plantilla.OptimizarTamanioHoja && !string.IsNullOrEmpty(idObjetoGraf) && geometryBase != null && geometryBase.PointCount.HasValue && geometryBase.PointCount > 1)
        //                    {
        //                        #region OptimizarTamanioHoja
        //                        PointF puntoMedio = new PointF();
        //                        List<Lado> lados = new List<Lado>();
        //                        Lado ladoMayor = new Lado();
        //                        int anguloTolerancia = 10;
        //                        double distanciaTolerancia = 5;
        //                        anguloRotacion = GetAnguloRotacion(geometryBase, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);
        //                        //Paso a radianes
        //                        anguloRotacion = anguloRotacion * Math.PI / 180;
        //                        anguloRotacionFiltro = anguloRotacion;
        //                        sentidoHorario = true;

        //                        #region Para Testing - Dibujo el primer segmento del primer lado y el ultimo segmento del ultimo lado
        //                        //float x1Segmento1 = GetXPDFCanvas(lados[0].Segmentos[0].P1.X, xCentroidBase, factorEscala, plantilla);
        //                        //float y1Segmento1 = GetYPDFCanvas(lados[0].Segmentos[0].P1.Y, yCentroidBase, factorEscala, plantilla);
        //                        //float x2Segmento1 = GetXPDFCanvas(lados[0].Segmentos[0].P2.X, xCentroidBase, factorEscala, plantilla);
        //                        //float y2Segmento1 = GetYPDFCanvas(lados[0].Segmentos[0].P2.Y, yCentroidBase, factorEscala, plantilla);
        //                        //PDFUtilities.DrawPDFLine(pdfContentByte, x1Segmento1, y1Segmento1, x2Segmento1, y2Segmento1, it.Utilities.MillimetersToPoints((float)1.27), it.BaseColor.BLACK);
        //                        //int iUltLado = lados.Count - 1;
        //                        //int iUltSegmento = lados[iUltLado].Segmentos.Count - 1;
        //                        //float x1Segmento2 = GetXPDFCanvas(lados[iUltLado].Segmentos[iUltSegmento].P1.X, xCentroidBase, factorEscala, plantilla);
        //                        //float y1Segmento2 = GetYPDFCanvas(lados[iUltLado].Segmentos[iUltSegmento].P1.Y, yCentroidBase, factorEscala, plantilla);
        //                        //float x2Segmento2 = GetXPDFCanvas(lados[iUltLado].Segmentos[iUltSegmento].P2.X, xCentroidBase, factorEscala, plantilla);
        //                        //float y2Segmento2 = GetYPDFCanvas(lados[iUltLado].Segmentos[iUltSegmento].P2.Y, yCentroidBase, factorEscala, plantilla);
        //                        //PDFUtilities.DrawPDFLine(pdfContentByte, x1Segmento2, y1Segmento2, x2Segmento2, y2Segmento2, it.Utilities.MillimetersToPoints((float)1.27), it.BaseColor.MAGENTA);
        //                        #endregion

        //                        //Segmento Mayor del Lado Mayor
        //                        Segmento segmentoMayor = new Segmento();
        //                        double distSegmentoMayor = -9999;
        //                        foreach (var segmento in ladoMayor.Segmentos)
        //                        {
        //                            if (segmento.Distancia > distSegmentoMayor)
        //                            {
        //                                distSegmentoMayor = segmento.Distancia;
        //                                segmentoMayor = segmento;
        //                            }
        //                        }

        //                        float x1SegmentoMayor = GetXPDFCanvas(segmentoMayor.P1.X, xCentroidBase, factorEscala, plantilla);
        //                        float y1SegmentoMayor = GetYPDFCanvas(segmentoMayor.P1.Y, yCentroidBase, factorEscala, plantilla);
        //                        float x2SegmentoMayor = GetXPDFCanvas(segmentoMayor.P2.X, xCentroidBase, factorEscala, plantilla);
        //                        float y2SegmentoMayor = GetYPDFCanvas(segmentoMayor.P2.Y, yCentroidBase, factorEscala, plantilla);


        //                        if (x2SegmentoMayor - x1SegmentoMayor == 0)
        //                        {
        //                            anguloRotacion = Math.PI / 2;
        //                        }
        //                        else
        //                        {
        //                            anguloRotacion = Math.Atan((y2SegmentoMayor - y1SegmentoMayor) / (x2SegmentoMayor - x1SegmentoMayor));
        //                        }
        //                        //anguloRotacion = anguloRotacion * Math.PI / 180;
        //                        //float x1SegmentoMayor = GetXPDFCanvas(ladoMayor.Segmentos[0].P1.X, xCentroidBase, factorEscala, plantilla);
        //                        //float y1SegmentoMayor = GetYPDFCanvas(ladoMayor.Segmentos[0].P1.Y, yCentroidBase, factorEscala, plantilla);
        //                        //float x2SegmentoMayor = GetXPDFCanvas(ladoMayor.Segmentos[0].P2.X, xCentroidBase, factorEscala, plantilla);
        //                        //float y2SegmentoMayor = GetYPDFCanvas(ladoMayor.Segmentos[0].P2.Y, yCentroidBase, factorEscala, plantilla);

        //                        #region Para Testing - Dibujo el Segmento Mayor
        //                        //Color contornoColor = Color.Yellow;
        //                        //it.BaseColor pdfContornoColor = new it.BaseColor(contornoColor.R, contornoColor.G, contornoColor.B);
        //                        //float pdfContornoGrosor = 2f;
        //                        //PDFUtilities.DrawPDFLine(pdfContentByte, x1SegmentoMayor, y1SegmentoMayor, x2SegmentoMayor, y2SegmentoMayor, pdfContornoGrosor, pdfContornoColor);
        //                        #endregion

        //                        PointF longSidePoint1 = new PointF(x1SegmentoMayor, y1SegmentoMayor);
        //                        PointF longSidePoint2 = new PointF(x2SegmentoMayor, y2SegmentoMayor);
        //                        float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
        //                        float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
        //                        PointF origin = new PointF(xCentro, yCentro);
        //                        //Angulo de rotacion en radianes - Se esta utilizando este para la rotacion
        //                        anguloRotacion = Theta(longSidePoint1, longSidePoint2, origin);

        //                        #region Para Testing - Dibujo lados de la geometryBase
        //                        //Random rand = new Random();
        //                        //foreach (var lado in lados)
        //                        //{
        //                        //    Color rndColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        //                        //    foreach (var segmento in lado.Segmentos)
        //                        //    {
        //                        //        pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
        //                        //        pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
        //                        //        pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
        //                        //        pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
        //                        //        it.BaseColor pdfContornoColorRnd = new it.BaseColor(rndColor.R, rndColor.G, rndColor.B);
        //                        //        PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, pdfContornoGrosor, pdfContornoColorRnd);
        //                        //    }
        //                        //}
        //                        //foreach (var segmento in ladoMayor.Segmentos)
        //                        //{
        //                        //    pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
        //                        //    pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
        //                        //    pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
        //                        //    pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
        //                        //    PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, pdfContornoGrosor, pdfContornoColor);
        //                        //}
        //                        #endregion

        //                        //El lado mas grande esta hacia abajo.
        //                        //Lo roto segun el lado mas largo de la plantilla. SOLO APLICA A PLOTEO FRECUENTE. MULTIPLES VIEWPORT.
        //                        if (impresionXMax - impresionXMin < impresionYMax - impresionYMin)
        //                        {
        //                            double ang = 90 * Math.PI / 180;
        //                            ang = Math.PI / 2;
        //                            anguloRotacion = anguloRotacion + ang;
        //                        }



        //                        //Recalculo la escala en base a la geometria del buffer segun sus coordenadas max y min rotadas
        //                        escala = GetEscala(bufferBase, xCentroidBase, yCentroidBase, plantilla, anguloRotacion, sentidoHorario, ref xMinBuff, ref yMinBuff, ref xMaxBuff, ref yMaxBuff);
        //                        factorEscala = 1 / escala;

        //                        //Roto los minimos y maximos del bufferBase rotado obtenidos en GetEscala
        //                        PointD ptBuffImpresionMinMin = RotateD(xMinBuff, yMinBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
        //                        PointD ptBuffImpresionMaxMax = RotateD(xMaxBuff, yMaxBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
        //                        PointD ptBuffImpresionMaxMin = RotateD(xMaxBuff, yMinBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
        //                        PointD ptBuffImpresionMinMax = RotateD(xMinBuff, yMaxBuff, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);

        //                        List<double> lstX = new List<double>();
        //                        lstX.Add(ptBuffImpresionMinMin.X);
        //                        lstX.Add(ptBuffImpresionMaxMax.X);
        //                        lstX.Add(ptBuffImpresionMaxMin.X);
        //                        lstX.Add(ptBuffImpresionMinMax.X);
        //                        List<double> lstY = new List<double>();
        //                        lstY.Add(ptBuffImpresionMinMin.Y);
        //                        lstY.Add(ptBuffImpresionMaxMax.Y);
        //                        lstY.Add(ptBuffImpresionMaxMin.Y);
        //                        lstY.Add(ptBuffImpresionMinMax.Y);

        //                        xMinBuffImpresion = lstX.Min();
        //                        yMinBuffImpresion = lstY.Min();
        //                        xMaxBuffImpresion = lstX.Max();
        //                        yMaxBuffImpresion = lstY.Max();

        //                        //Vuelvo a calcular el centroide
        //                        xCentroidBase = xMinBuffImpresion + ((xMaxBuffImpresion - xMinBuffImpresion) / 2.0);
        //                        yCentroidBase = yMinBuffImpresion + ((yMaxBuffImpresion - yMinBuffImpresion) / 2.0);

        //                        //En base al centroide nuevo vuelvo a sacar las coordenadas
        //                        xMinBuffImpresion = GetXBuffer(plantilla.ImpresionXMin, xCentroidBase, factorEscala, plantilla);
        //                        yMinBuffImpresion = GetYBuffer(plantilla.ImpresionYMin, yCentroidBase, factorEscala, plantilla);
        //                        xMaxBuffImpresion = GetXBuffer(plantilla.ImpresionXMax, xCentroidBase, factorEscala, plantilla);
        //                        yMaxBuffImpresion = GetYBuffer(plantilla.ImpresionYMax, yCentroidBase, factorEscala, plantilla);

        //                        //Anti-Roto el buffer de impresion y esas coordenadas son las que voy a usar para consultar a la base
        //                        ptBuffImpresionMinMin = RotateD(xMinBuffImpresion, yMinBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
        //                        ptBuffImpresionMaxMax = RotateD(xMaxBuffImpresion, yMaxBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
        //                        ptBuffImpresionMaxMin = RotateD(xMaxBuffImpresion, yMinBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);
        //                        ptBuffImpresionMinMax = RotateD(xMinBuffImpresion, yMaxBuffImpresion, (xCentroidBase), (yCentroidBase), anguloRotacion, !sentidoHorario);

        //                        lstCoordsBuffImpresion.Clear();
        //                        lstCoordsBuffImpresion.Add(ptBuffImpresionMinMin.X.ToString().Replace(",", ".") + ", " + ptBuffImpresionMinMin.Y.ToString().Replace(",", "."));
        //                        lstCoordsBuffImpresion.Add(ptBuffImpresionMaxMin.X.ToString().Replace(",", ".") + ", " + ptBuffImpresionMaxMin.Y.ToString().Replace(",", "."));
        //                        lstCoordsBuffImpresion.Add(ptBuffImpresionMaxMax.X.ToString().Replace(",", ".") + ", " + ptBuffImpresionMaxMax.Y.ToString().Replace(",", "."));
        //                        lstCoordsBuffImpresion.Add(ptBuffImpresionMinMax.X.ToString().Replace(",", ".") + ", " + ptBuffImpresionMinMax.Y.ToString().Replace(",", "."));
        //                        lstCoordsBuffImpresion.Add(ptBuffImpresionMinMin.X.ToString().Replace(",", ".") + ", " + ptBuffImpresionMinMin.Y.ToString().Replace(",", "."));

        //                        //Array utilizado para buscar imagen wms de fondo
        //                        aTransformCoords = TransformCoords(xMinBuffImpresion, yMinBuffImpresion, xMaxBuffImpresion, yMaxBuffImpresion);
        //                        #endregion
        //                    }
        //                    #endregion configuracion

        //                    Layer layerSecundario = plantilla.Layers.FirstOrDefault(p => p.Categoria == 2 && p.FechaBaja == null);
        //                    LayerGraf[] aLayerGrafSecundario = null;
        //                    if (layerSecundario != null && !string.IsNullOrEmpty(idsObjetoSecundario))
        //                    {
        //                        aLayerGrafSecundario = GetLayerGrafByIds(layerSecundario, layerSecundario.Componente, idsObjetoSecundario, lstCoordsBuffImpresion);
        //                    }

        //                    //Layers ordenados
        //                    List<Layer> lstLayersOrdenados = new List<Layer>();
        //                    if (!string.IsNullOrEmpty(idObjetoGraf))
        //                    {
        //                        // Filtra por categoria menor a 3. Categoria=3 es para config. texto de mapa tematico
        //                        lstLayersOrdenados = plantilla.Layers.Where(pl => pl.FechaBaja == null && pl.Categoria < 3).OrderBy(l => l.Orden).ToList();
        //                    }
        //                    else
        //                    {
        //                        // Filtra por categoria menor a 3. Categoria=3 es para config. texto de mapa tematico
        //                        lstLayersOrdenados = plantilla.Layers.Where(pl => pl.FechaBaja == null && (pl.FiltroGeografico == 1 || pl.FiltroGeografico == 0) && pl.Categoria < 3).ToList();
        //                        if (lstLayersOrdenados != null)
        //                        {
        //                            lstLayersOrdenados = lstLayersOrdenados.OrderBy(l => l.Orden).ToList();
        //                        }
        //                    }


        //                    foreach (Layer layer in lstLayersOrdenados)
        //                    {
        //                        #region Layers

        //                        var layersPDF = pdfStamper.GetPdfLayers();

        //                        try
        //                        {
        //                            var lay = layersPDF[layer.Nombre];
        //                            pdfContentByte.BeginLayer(lay);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            PdfLayer ads = new PdfLayer(layer.Nombre, pdfWriter);
        //                            ads.On = true;
        //                            pdfContentByte.BeginLayer(ads);
        //                        }

        //                        LayerGraf[] aLayerGraf = null;
        //                        #region Plantilla viewport

        //                        if (layer.FiltroGeografico == 1)
        //                        {//Busco por las coordenadas de la geometria de la plantilla.
        //                            aLayerGraf = GetLayerGrafByObjetoBase(layer, layer.Componente, layer.Componente, lstCoordsBuffImpresion, idObjetoGraf, anio, esInformeAnual);
        //                        }
        //                        else if (layer.FiltroGeografico == 0)
        //                        {//Busco por las coordenadas del rectangulo de la vista.
        //                            aLayerGraf = GetLayerGrafByCoords(layer, layer.Componente, lstCoordsBuffImpresion, anio);
        //                        }

        //                        if (aLayerGraf != null && aLayerGraf.Length > 0)
        //                        {
        //                            DibujarLayerGraf<LayerGraf>(pdfContentByte, aLayerGraf, "Nombre", layer, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
        //                            lstLayersReferencia.Add(layer);

        //                            grosorMaximo = layer.ContornoGrosor.HasValue && grosorMaximo < layer.ContornoGrosor ? (double)layer.ContornoGrosor : grosorMaximo;
        //                        }

        //                        #endregion Plantilla viewport
        //                        pdfContentByte.EndLayer();

        //                        #endregion Layers
        //                    }

        //                    //Funciones Especiales
        //                    if (plantilla.IdFuncionAdicional != null && plantilla.IdFuncionAdicional > 0)
        //                    {
        //                        ModPlotFuncionesEspeciales modPlotFuncionesEspeciales = new ModPlotFuncionesEspeciales(_parcelaPlotRepository, _cuadraPlotRepository, _layerGrafRepository, _plantillaRepository, _manzanaPlotRepository, _callePlotRepository, _expansionPlotRepository, _parametroRepository, _tipoPlanoRepository, _partidoRepository, _censoRepository, _atributoRepository, _componenteRepository);
        //                        bool ok = modPlotFuncionesEspeciales.EjecutarFuncionEspecial((int)plantilla.IdFuncionAdicional, pdfDoc, pdfContentByte, plantilla, idPlantillaFondo, null, idObjetoGraf, idsObjetoSecundario, lstCoordsGeometryBase, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, grafico, leyenda, infoLeyenda, lstLayersReferencia, anio, lstCoordsBuffImpresion);
        //                    }

        //                    #region Dibujo un rectangulo blanco en los puntos min y max para tapar el recorte que genera la funcion de oracle

        //                    /*
        //                     xMinBuffImpresion
        //                     yMinBuffImpresion
        //                     xMaxBuffImpresion
        //                     yMaxBuffImpresion
        //                     */
        //                    float grosor = it.Utilities.MillimetersToPoints((float)grosorMaximo);
        //                    grosor += grosor * 10 / 100;
        //                    PDFUtilities.DrawPDFRectangle2(pdfContentByte, (float)impresionXMinPrincipal, (float)impresionYMinPrincipal, (float)impresionXMaxPrincipal, (float)impresionYMaxPrincipal, new it.BaseColor(Color.White), (float)grosor, null, null);
        //                    #endregion
        //                }

        //                pdfStamper.Close();
        //                bPdf = memStreamTemp.ToArray();

        //                #endregion Dibujo 1
        //            }


        //            using (var pdfReaderTemp = new PdfReader(bPdf.ToArray()))
        //            using (var memStreamOut = new MemoryStream())
        //            {

        //                #region Dibujo extras
        //                var pdfDocOut = new it.Document(new it.Rectangle(anchoPts, altoPts), 0, 0, 0, 0);
        //                var pdfWriterOut = PdfWriter.GetInstance(pdfDocOut, memStreamOut);
        //                pdfWriterOut.ViewerPreferences = PdfWriter.PageModeUseOC;
        //                pdfWriterOut.PdfVersion = PdfWriter.VERSION_1_5;
        //                pdfDocOut.Open();


        //                PdfStamper pdfStamper2 = new PdfStamper(pdfReaderTemp, memStreamOut);

        //                pdfStamper2.Writer.ViewerPreferences = PdfWriter.PageModeUseOC;
        //                pdfStamper2.Writer.PdfVersion = PdfWriter.VERSION_1_5;


        //                PdfContentByte pdfContentByteOut = pdfStamper2.GetOverContent(1);


        //                DibujarReferencias(pdfContentByteOut, plantillaPrincipal, lstLayersReferencia.Distinct().ToList());

        //                //Busco y dibujo el Norte
        //                Norte norte = GetNorteById(plantillaPrincipal.IdNorte);
        //                if (norte != null)
        //                {
        //                    #region Norte
        //                    float norteAnchoPts = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.NorteAncho);
        //                    float norteAltoPts = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.NorteAlto);
        //                    pdfx1cPrincipal = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.NorteX);
        //                    pdfy1cPrincipal = it.Utilities.MillimetersToPoints((float)plantillaPrincipal.NorteY);
        //                    Bitmap bmpNorte = (Bitmap)norte.Imagen;
        //                    if (anguloRotacionPrincipal != 0)
        //                    {
        //                        //Roto ptos de imagen Norte
        //                        double xNorMin = plantillaPrincipal.NorteX;
        //                        double yNorMin = plantillaPrincipal.NorteY;
        //                        double xNorMax = plantillaPrincipal.NorteX + plantillaPrincipal.NorteAncho;
        //                        double yNorMax = plantillaPrincipal.NorteY + plantillaPrincipal.NorteAlto;
        //                        double xCentroidPlantilla = xNorMin + (xNorMax - xNorMin) / 2;
        //                        double yCentroidPlantilla = yNorMin + (yNorMax - yNorMin) / 2;

        //                        PointF ptNorMinMin = Rotate((float)xNorMin, (float)yNorMin, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacionPrincipal, sentidoHorarioPrincipal);
        //                        PointF ptNorMaxMax = Rotate((float)xNorMax, (float)yNorMax, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacionPrincipal, sentidoHorarioPrincipal);
        //                        PointF ptNorMaxMin = Rotate((float)xNorMax, (float)yNorMin, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacionPrincipal, sentidoHorarioPrincipal);
        //                        PointF ptNorMinMax = Rotate((float)xNorMin, (float)yNorMax, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacionPrincipal, sentidoHorarioPrincipal);
        //                        List<float> lstX = new List<float>();
        //                        lstX.Add(ptNorMinMin.X);
        //                        lstX.Add(ptNorMaxMax.X);
        //                        lstX.Add(ptNorMaxMin.X);
        //                        lstX.Add(ptNorMinMax.X);
        //                        List<float> lstY = new List<float>();
        //                        lstY.Add(ptNorMinMin.Y);
        //                        lstY.Add(ptNorMaxMax.Y);
        //                        lstY.Add(ptNorMaxMin.Y);
        //                        lstY.Add(ptNorMinMax.Y);

        //                        xNorMin = lstX.Min();
        //                        yNorMin = lstY.Min();
        //                        xNorMax = lstX.Max();
        //                        yNorMax = lstY.Max();

        //                        pdfx1cPrincipal = it.Utilities.MillimetersToPoints((float)xNorMin);
        //                        pdfy1cPrincipal = it.Utilities.MillimetersToPoints((float)yNorMin);
        //                        float pdfx2c = it.Utilities.MillimetersToPoints((float)xNorMax);
        //                        float pdfy2c = it.Utilities.MillimetersToPoints((float)yNorMax);
        //                        norteAnchoPts = pdfx2c - pdfx1cPrincipal;
        //                        norteAltoPts = pdfy2c - pdfy1cPrincipal;

        //                        float anguloGrados = (float)(anguloRotacionPrincipal * 180 / Math.PI) * (-1);
        //                        Bitmap bmpNorteRotado = RotateImage2(norte.Imagen, anguloGrados);
        //                        PDFUtilities.DrawPDFImage(pdfContentByteOut, bmpNorteRotado, pdfx1cPrincipal, pdfy1cPrincipal, norteAnchoPts, norteAltoPts, 0);
        //                    }
        //                    else
        //                    {
        //                        PDFUtilities.DrawPDFImage(pdfContentByteOut, bmpNorte, pdfx1cPrincipal, pdfy1cPrincipal, norteAnchoPts, norteAltoPts, (float)anguloRotacionPrincipal);
        //                    }
        //                    #endregion
        //                }

        //                //Dibujo Textos de la plantilla
        //                if (plantillaPrincipal.PlantillaTextos != null)
        //                {
        //                    Dictionary<string, string> dicTextosVariables = new Dictionary<string, string>();
        //                    if (!string.IsNullOrEmpty(textosVariables))
        //                    {
        //                        dicTextosVariables = textosVariables.Split(';').Select(x => x.Split(',')).ToDictionary(x => x[0].ToUpper(), x => x[1]);
        //                    }
        //                    DibujarTextos(pdfContentByteOut, plantillaPrincipal, idObjetoGraf, idComponenteObjetoGraf, factorEscalaPrincipal, dicTextosVariables, xCentroidBasePrincipal, yCentroidBasePrincipal, null);
        //                }


        //                pdfStamper2.Close();

        //                bPdf = memStreamOut.ToArray();
        //                #endregion Dibujo 2
        //            }
        //            //FIN
        //        }
        //    }
        //    catch (ArgumentNullException argErr)
        //    {
        //        Global.GetLogger().LogError("GetPlantilla", argErr);
        //    }
        //    catch (Exception e)
        //    {
        //        Global.GetLogger().LogError("GetPlantilla", e);
        //    }

        //    return bPdf;
        //}

        #region Comento porque no se usa. se borra en futuras entregas
        //public byte[] GetPlantillaLGC(int idPlantilla, string idObjetoGraf, int idComponenteObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, string extent, string scale, string layersVisibles, bool verCotas, ObjetoResultadoDetalle objetoResultadoDetalle, int idImagenSatelital, float imagenTransparencia, bool verIdentificante, long? idComponentePrincipal, bool verContexto, InformacionComercial grafico = null, InformacionComercial leyenda = null, int? infoLeyenda = null, bool esInformeAnual = false, string anio = "", string numeroODT = "0", string tipo = null, string relacion = null, string direccionCompleta = null, string datosReferencia = null, long? idComponenteTematico = null, List<Relacion> relaciones = null, string apicLGC = null)
        //{
        //    byte[] bPdf = null;
        //    try
        //    {
        //        long ID_COMPONENTE_ARCO = GetComponenteArco();
        //        Plantilla plantilla = GetPlantillaById(idPlantilla);

        //        //PlantillaFondo plantillaFondo = plantilla.PlantillaFondos.First(p => p.IdPlantillaFondo == idPlantillaFondo);
        //        PlantillaFondo plantillaFondo = GetPlantillaFondoById(idPlantillaFondo);

        //        extent = string.Join(",", _layerGrafRepository.GetCoordsByRelaciones(relaciones));

        //        if (plantilla != null && plantillaFondo != null)
        //        {
        //            #region configPDF
        //            Hoja hoja = GetHojaById(plantilla.IdHoja);
        //            plantilla.PPP = plantillaFondo.Resolucion.Valor;
        //            float anchoPts = it.Utilities.MillimetersToPoints((float)hoja.Ancho_mm);
        //            float altoPts = it.Utilities.MillimetersToPoints((float)hoja.Alto_mm);
        //            if (plantilla.Orientacion == 0)
        //            {
        //                plantilla.WidthPts = anchoPts;
        //                plantilla.HeigthPts = altoPts;
        //            }
        //            else
        //            {
        //                plantilla.WidthPts = altoPts;
        //                plantilla.HeigthPts = anchoPts;
        //                anchoPts = it.Utilities.MillimetersToPoints((float)hoja.Alto_mm);
        //                altoPts = it.Utilities.MillimetersToPoints((float)hoja.Ancho_mm);
        //            }
        //            float impresionXMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMin);
        //            float impresionYMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMin);
        //            float impresionXMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMax);
        //            float impresionYMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMax);

        //            var pdfDoc = new it.Document(new it.Rectangle(anchoPts, altoPts), 0, 0, 0, 0);
        //            #endregion

        //            float grosorMaximo = 0;

        //            List<Layer> lstLayersReferencia = new List<Layer>();
        //            float pdfx1c = 0, pdfy1c = 0;
        //            double anguloRotacion = 0;
        //            double anguloRotacionFiltro = 0;
        //            bool sentidoHorario = false;

        //            double xCentroidBase = 0, yCentroidBase = 0;
        //            double escala = 0, factorEscala = 0;
        //            double xMinBuff = 9999999, yMinBuff = 9999999, xMaxBuff = 0, yMaxBuff = 0;

        //            List<string> lstCoordsGeometryBase = new List<string>();
        //            List<LayerGraf> lstLayerGraftRelaciones = new List<LayerGraf>();

        //            using (var pdfReaderPlantilla = new PdfReader(plantillaFondo.PDFMemoryStream))
        //            using (var memStreamOut = new MemoryStream())
        //            {
        //                #region Dibujo Plantilla Fondo
        //                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, memStreamOut);
        //                pdfDoc.Open();


        //                pdfWriter.ViewerPreferences = PdfWriter.PageModeUseOC;
        //                pdfWriter.PdfVersion = PdfWriter.VERSION_1_5;


        //                PdfContentByte pdfContentByteOut = pdfWriter.DirectContent;

        //                var importedPagePlantilla = pdfWriter.GetImportedPage(pdfReaderPlantilla, 1);
        //                #region AddTemplate con Posible Rotation en Plantilla PDF
        //                var pageWidth = pdfReaderPlantilla.GetPageSizeWithRotation(1).Width;
        //                var pageHeight = pdfReaderPlantilla.GetPageSizeWithRotation(1).Height;

        //                switch (importedPagePlantilla.Rotation)
        //                {
        //                    case 0:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 1f, 0, 0, 1f, 0, 0);
        //                        break;
        //                    case 90:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 0, -1f, 1f, 0, 0, pageHeight);
        //                        break;
        //                    case 180:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, -1f, 0, 0, -1f, pageWidth, pageHeight);
        //                        break;
        //                    case 270:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 0, 1f, -1f, 0, pageWidth, 0);
        //                        break;
        //                    default:
        //                        pdfContentByteOut.AddTemplate(importedPagePlantilla, 1f, 0, 0, 1f, 0, 0);
        //                        break;
        //                }
        //                #endregion

        //                pdfDoc.Close();
        //                pdfWriter.Close();

        //                bPdf = memStreamOut.ToArray();
        //                #endregion
        //            }

        //            using (PdfReader pdfReader = new PdfReader(bPdf))
        //            using (MemoryStream memStreamTemp = new MemoryStream())
        //            {
        //                #region ConfigPloteo

        //                PdfStamper pdfStamper = new PdfStamper(pdfReader, memStreamTemp);

        //                PdfContentByte pdfContentByte = pdfStamper.GetOverContent(1);
        //                double x = 0, y = 0;

        //                Layer layerBase = plantilla.Layers.FirstOrDefault(p => p.Categoria == 1 && p.ComponenteId == idComponenteObjetoGraf && p.FechaBaja == null);
        //                List<string> lstCoordsBuffImpresion = new List<string>();

        //                string[] aExtents = extent.Split(',');
        //                if (aExtents.Length > 0)
        //                {
        //                    #region extents
        //                    int iExt = 0;
        //                    while (iExt < aExtents.Length)
        //                    {
        //                        x = Convert.ToDouble(aExtents[iExt]);
        //                        iExt++;
        //                        y = Convert.ToDouble(aExtents[iExt]);
        //                        if (x <= xMinBuff)
        //                        {
        //                            xMinBuff = x;
        //                        }
        //                        if (x >= xMaxBuff)
        //                        {
        //                            xMaxBuff = x;
        //                        }
        //                        if (y <= yMinBuff)
        //                        {
        //                            yMinBuff = y;
        //                        }
        //                        if (y >= yMaxBuff)
        //                        {
        //                            yMaxBuff = y;
        //                        }
        //                        if (aExtents.Length > 4)
        //                        {
        //                            //Salteo la que viene en 0
        //                            iExt++;
        //                        }
        //                        iExt++;
        //                    }
        //                    xCentroidBase = xMinBuff + ((xMaxBuff - xMinBuff) / 2.0);
        //                    yCentroidBase = yMinBuff + ((yMaxBuff - yMinBuff) / 2.0);

        //                    #endregion
        //                }

        //                //Calculo la escala en base a los minimos y maximos del buffer
        //                #region Calculo de Escala
        //                if (((plantilla.X_Util / 1000) * (yMaxBuff - yMinBuff)) > ((plantilla.Y_Util / 1000) * (xMaxBuff - xMinBuff)))
        //                {
        //                    factorEscala = (plantilla.Y_Util / 1000) / (yMaxBuff - yMinBuff);
        //                }
        //                else
        //                {
        //                    factorEscala = (plantilla.X_Util / 1000) / (xMaxBuff - xMinBuff);
        //                }
        //                escala = 1 / factorEscala;

        //                //Hasta aca, calcula la escala exacta para que se muestre toda la geometria.
        //                //Recomendacion: Disminuir la escala un 5% para el caso que un extremo sea una imagen y que no salga cortada.


        //                //Aca busca en las escalas de las plantillas la siguiente escala que sea mayor a la calculada, para que se pueda ver todo.
        //                //Supongo que es para estandarizar las escalas de los ploteos.
        //                long escalaPlantilla = Convert.ToInt64(Math.Ceiling(escala));
        //                var lstEscala = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).ToList();
        //                if (lstEscala != null && lstEscala.Count > 0)
        //                {
        //                    escalaPlantilla = plantilla.PlantillaEscalas.Where(p => p.Escala >= escala).OrderBy(p => p.Escala).Select(p => p.Escala).First();
        //                }
        //                factorEscala = 1.0 / escalaPlantilla;
        //                escala = 1 / factorEscala;

        //                #endregion

        //                //Obtengo el buffer con respecto al area de impresion en coordenadas cartograficas
        //                double xMinBuffImpresion = GetXBuffer(plantilla.ImpresionXMin, xCentroidBase, factorEscala, plantilla);
        //                double yMinBuffImpresion = GetYBuffer(plantilla.ImpresionYMin, yCentroidBase, factorEscala, plantilla);
        //                double xMaxBuffImpresion = GetXBuffer(plantilla.ImpresionXMax, xCentroidBase, factorEscala, plantilla);
        //                double yMaxBuffImpresion = GetYBuffer(plantilla.ImpresionYMax, yCentroidBase, factorEscala, plantilla);

        //                lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString().Replace(",", ".") + ", " + yMinBuffImpresion.ToString().Replace(",", "."));
        //                lstCoordsBuffImpresion.Add(xMaxBuffImpresion.ToString().Replace(",", ".") + ", " + yMinBuffImpresion.ToString().Replace(",", "."));
        //                lstCoordsBuffImpresion.Add(xMaxBuffImpresion.ToString().Replace(",", ".") + ", " + yMaxBuffImpresion.ToString().Replace(",", "."));
        //                lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString().Replace(",", ".") + ", " + yMaxBuffImpresion.ToString().Replace(",", "."));
        //                lstCoordsBuffImpresion.Add(xMinBuffImpresion.ToString().Replace(",", ".") + ", " + yMinBuffImpresion.ToString().Replace(",", "."));

        //                //Array utilizado para buscar imagen wms de fondo
        //                double[] aTransformCoords = TransformCoords(xMinBuffImpresion, yMinBuffImpresion, xMaxBuffImpresion, yMaxBuffImpresion);

        //                #endregion
        //                #region layers


        //                //Layers ordenados
        //                List<Layer> lstLayersOrdenados = new List<Layer>();

        //                // Filtra por categoria menor a 3. Categoria=3 es para config. texto de mapa tematico
        //                lstLayersOrdenados = plantilla.Layers.Where(pl => pl.FechaBaja == null && (pl.FiltroGeografico == 1 || pl.FiltroGeografico == 0) && pl.Categoria < 3).ToList();
        //                if (lstLayersOrdenados != null)
        //                {
        //                    lstLayersOrdenados = lstLayersOrdenados.OrderBy(l => l.Orden).ToList();
        //                }

        //                List<Layer> lstLayersRedesOrdenados = lstLayersOrdenados.ToList();
        //                #endregion


        //                foreach (Layer layer in lstLayersOrdenados)
        //                {
        //                    #region dibujarLayers
        //                    //Imprime todos los layer de la plantilla que no sean de redes. Y los de redes, solo los que tengan el id seleccionado.
        //                    LayerGraf[] aLayerGraf;

        //                    if (lstLayersRedesOrdenados.Contains(layer))
        //                    {
        //                        aLayerGraf = GetLayerGrafByCoordsAndIds(layer, layer.Componente, lstCoordsBuffImpresion, anio, relaciones.Where(r => r.IdTablaHijo.HasValue && r.TablaHijo == layer.Componente.Tabla).Select(r => (long)r.IdTablaHijo).ToList());
        //                        if (aLayerGraf != null && aLayerGraf.Length > 0)
        //                        {
        //                            lstLayerGraftRelaciones.AddRange(aLayerGraf);//Agrego los layergraf a una lista para despues resaltarlos con la funcion especial

        //                            //if (!layer.Etiqueta && servicio == GranConducto.TipoServicio.Agua)
        //                            if (!layer.Etiqueta)
        //                            {//En servicio agua, si no tiene etiqueta usa el codigo de la LGC
        //                                foreach (var layergraft in aLayerGraf)
        //                                {
        //                                    layergraft.Nombre = apicLGC;
        //                                }
        //                                layer.Etiqueta = true;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        aLayerGraf = GetLayerGrafByCoords(layer, layer.Componente, lstCoordsBuffImpresion, anio);
        //                    }

        //                    if (aLayerGraf != null && aLayerGraf.Length > 0)
        //                    {
        //                        if (lstLayersRedesOrdenados.Contains(layer))
        //                        {
        //                            //No filtro por coordenadas xq en teoria tienen que estar adentro las relaciones.

        //                            //if (!layer.Etiqueta && servicio == GranConducto.TipoServicio.Agua)
        //                            if (!layer.Etiqueta)
        //                            {//En servicio agua, si no tiene etiqueta usa el codigo de la LGC
        //                                foreach (var layergraft in aLayerGraf)
        //                                {
        //                                    layergraft.Nombre = apicLGC;
        //                                }
        //                                layer.Etiqueta = true;
        //                            }
        //                        }
        //                        if (aLayerGraf != null && aLayerGraf.Length > 0)
        //                        {
        //                            //DibujoLayer
        //                            lstLayersReferencia.Add(layer);


        //                            var lstLayerPDF = pdfStamper.GetPdfLayers();


        //                            PdfLayer lay = null;

        //                            try
        //                            {//Mejorar esto
        //                                lay = lstLayerPDF[layer.Nombre];
        //                            }
        //                            catch (Exception ex)
        //                            {

        //                            }
        //                            if (lay == null)
        //                            {
        //                                lay = new PdfLayer(layer.Nombre, pdfStamper.Writer);
        //                            }


        //                            pdfContentByte.BeginLayer(lay);

        //                            DibujarLayerGraf<LayerGraf>(pdfContentByte, aLayerGraf, "Nombre", layer, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);

        //                            pdfContentByte.EndLayer();
        //                        }
        //                    }
        //                    #endregion
        //                }

        //                grosorMaximo = it.Utilities.MillimetersToPoints((float)lstLayersReferencia.Select(l => l.ContornoGrosor).Max());

        //                grosorMaximo = grosorMaximo + grosorMaximo * 10 / 100;

        //                PDFUtilities.DrawPDFRectangle2(pdfContentByte, (float)impresionXMin, (float)impresionYMin, (float)impresionXMax, (float)impresionYMax, new it.BaseColor(Color.White), (float)grosorMaximo, null, null);

        //                pdfStamper.Close();
        //                bPdf = memStreamTemp.ToArray();
        //            }

        //            using (var pdfReaderPlantilla = new PdfReader(plantillaFondo.PDFMemoryStream))
        //            using (var pdfReaderTemp = new PdfReader(bPdf))
        //            using (var memStreamOut = new MemoryStream())
        //            {
        //                #region Dibujo 2
        //                var pdfDocOut = new it.Document(new it.Rectangle(anchoPts, altoPts), 0, 0, 0, 0);
        //                var pdfWriterOut = PdfWriter.GetInstance(pdfDocOut, memStreamOut);
        //                pdfWriterOut.ViewerPreferences = PdfWriter.PageModeUseOC;
        //                pdfWriterOut.PdfVersion = PdfWriter.VERSION_1_5;
        //                pdfDocOut.Open();

        //                PdfStamper pdfStamper = new PdfStamper(pdfReaderTemp, memStreamOut);

        //                pdfStamper.Writer.ViewerPreferences = PdfWriter.PageModeUseOC;
        //                pdfStamper.Writer.PdfVersion = PdfWriter.VERSION_1_5;

        //                pdfStamper.GetPdfLayers();

        //                PdfContentByte pdfContentByteOut = pdfStamper.GetOverContent(1);

        //                DibujarReferencias(pdfContentByteOut, plantilla, lstLayersReferencia.Distinct().ToList());

        //                //Busco y dibujo el Norte
        //                Norte norte = GetNorteById(plantilla.IdNorte);
        //                if (norte != null)
        //                {
        //                    #region Norte
        //                    float norteAnchoPts = it.Utilities.MillimetersToPoints((float)plantilla.NorteAncho);
        //                    float norteAltoPts = it.Utilities.MillimetersToPoints((float)plantilla.NorteAlto);
        //                    pdfx1c = it.Utilities.MillimetersToPoints((float)plantilla.NorteX);
        //                    pdfy1c = it.Utilities.MillimetersToPoints((float)plantilla.NorteY);
        //                    Bitmap bmpNorte = (Bitmap)norte.Imagen;
        //                    if (anguloRotacion != 0)
        //                    {
        //                        //Roto ptos de imagen Norte
        //                        double xNorMin = plantilla.NorteX;
        //                        double yNorMin = plantilla.NorteY;
        //                        double xNorMax = plantilla.NorteX + plantilla.NorteAncho;
        //                        double yNorMax = plantilla.NorteY + plantilla.NorteAlto;
        //                        double xCentroidPlantilla = xNorMin + (xNorMax - xNorMin) / 2;
        //                        double yCentroidPlantilla = yNorMin + (yNorMax - yNorMin) / 2;

        //                        PointF ptNorMinMin = Rotate((float)xNorMin, (float)yNorMin, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);
        //                        PointF ptNorMaxMax = Rotate((float)xNorMax, (float)yNorMax, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);
        //                        PointF ptNorMaxMin = Rotate((float)xNorMax, (float)yNorMin, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);
        //                        PointF ptNorMinMax = Rotate((float)xNorMin, (float)yNorMax, (float)(xCentroidPlantilla), (float)(yCentroidPlantilla), anguloRotacion, sentidoHorario);
        //                        List<float> lstX = new List<float>();
        //                        lstX.Add(ptNorMinMin.X);
        //                        lstX.Add(ptNorMaxMax.X);
        //                        lstX.Add(ptNorMaxMin.X);
        //                        lstX.Add(ptNorMinMax.X);
        //                        List<float> lstY = new List<float>();
        //                        lstY.Add(ptNorMinMin.Y);
        //                        lstY.Add(ptNorMaxMax.Y);
        //                        lstY.Add(ptNorMaxMin.Y);
        //                        lstY.Add(ptNorMinMax.Y);

        //                        xNorMin = lstX.Min();
        //                        yNorMin = lstY.Min();
        //                        xNorMax = lstX.Max();
        //                        yNorMax = lstY.Max();

        //                        pdfx1c = it.Utilities.MillimetersToPoints((float)xNorMin);
        //                        pdfy1c = it.Utilities.MillimetersToPoints((float)yNorMin);
        //                        float pdfx2c = it.Utilities.MillimetersToPoints((float)xNorMax);
        //                        float pdfy2c = it.Utilities.MillimetersToPoints((float)yNorMax);
        //                        norteAnchoPts = pdfx2c - pdfx1c;
        //                        norteAltoPts = pdfy2c - pdfy1c;

        //                        float anguloGrados = (float)(anguloRotacion * 180 / Math.PI) * (-1);
        //                        Bitmap bmpNorteRotado = RotateImage2(norte.Imagen, anguloGrados);
        //                        PDFUtilities.DrawPDFImage(pdfContentByteOut, bmpNorteRotado, pdfx1c, pdfy1c, norteAnchoPts, norteAltoPts, 0);
        //                    }
        //                    else
        //                    {
        //                        PDFUtilities.DrawPDFImage(pdfContentByteOut, bmpNorte, pdfx1c, pdfy1c, norteAnchoPts, norteAltoPts, (float)anguloRotacion);
        //                    }
        //                    #endregion
        //                }

        //                //Dibujo Textos de la plantilla
        //                if (plantilla.PlantillaTextos != null)
        //                {
        //                    #region dibujar textos
        //                    Dictionary<string, string> dicTextosVariables = new Dictionary<string, string>();
        //                    if (!string.IsNullOrEmpty(textosVariables))
        //                    {
        //                        dicTextosVariables = textosVariables.Split(';').Select(u => u.Split(',')).ToDictionary(u => u[0].ToUpper(), u => u[1]);
        //                    }
        //                    DibujarTextos(pdfContentByteOut, plantilla, idObjetoGraf, idComponenteObjetoGraf, factorEscala, dicTextosVariables, xCentroidBase, yCentroidBase, null, numeroODT, tipo, relacion, direccionCompleta, datosReferencia, null, apicLGC);
        //                    #endregion
        //                }

        //                //Funciones especiales
        //                if (plantilla.IdFuncionAdicional != null && plantilla.IdFuncionAdicional > 0)
        //                {
        //                    #region funciones especiales
        //                    ModPlotFuncionesEspeciales modPlotFuncionesEspeciales = new ModPlotFuncionesEspeciales(_parcelaPlotRepository, _cuadraPlotRepository, _layerGrafRepository, _plantillaRepository, _manzanaPlotRepository, _callePlotRepository, _expansionPlotRepository, _parametroRepository, _tipoPlanoRepository, _partidoRepository, _censoRepository, _atributoRepository, _componenteRepository);

        //                    PdfContentByte cb = pdfStamper.GetUnderContent(1);

        //                    modPlotFuncionesEspeciales.EjecutarFuncionEspecial((int)plantilla.IdFuncionAdicional, pdfDocOut, cb, plantilla, idPlantillaFondo, null, idObjetoGraf, idsObjetoSecundario, lstCoordsGeometryBase, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, grafico, leyenda, infoLeyenda, lstLayersReferencia, anio, null, lstLayerGraftRelaciones.ToArray());
        //                    #endregion
        //                }

        //                pdfStamper.Close();

        //                bPdf = memStreamOut.ToArray();
        //                #endregion Dibujo 2
        //            }
        //            //FIN
        //        }
        //    }
        //    catch (ArgumentNullException argErr)
        //    {
        //        Global.GetLogger().LogError("GetPlantilla", argErr);
        //    }
        //    catch (Exception e)
        //    {
        //        Global.GetLogger().LogError("GetPlantilla", e);
        //    }
        //    return bPdf;
        //} 
        #endregion

        private long GetComponenteArco()
        {
            return long.Parse(this._parametroRepository.GetParametroByDescripcion("ID_COMPONENTE_ARCO"));
        }

        private void DibujarReferencias(PdfContentByte pdfContentByte, Plantilla plantilla, List<Layer> lstLayers)
        {
            float xMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMin);
            float yMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMin);
            float xMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
            float yMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMax);
            float espaciado = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaEspaciado);

            // DibujarReferencias - DrawPDFRectangle de mins maxs
            //DrawPDFRectangle(pdfContentByte, xMinReferencia, yMinReferencia, xMaxReferencia - xMinReferencia, yMaxReferencia - yMinReferencia, new it.BaseColor(Color.Gray.R, Color.Gray.G, Color.Gray.B), 1.0f, null, null);

            Color colorReferencia = System.Drawing.ColorTranslator.FromHtml(plantilla.ReferenciaColor);
            it.BaseColor pdfColorTexto = new it.BaseColor(colorReferencia.R, colorReferencia.G, colorReferencia.B);

            int alignment = it.Element.ALIGN_LEFT + it.Element.ALIGN_BOTTOM;

            float pdfFontSize = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaFuenteTamanio);

            PDFUtilities.RegisterBaseFont(plantilla.ReferenciaFuenteNombre, pdfFontSize);
            string[] aFontStyle = plantilla.ReferenciaFuenteEstilo.Split(',');
            int pdfFontStyle = aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
            BaseFont pdfbaseFont = it.FontFactory.GetFont(plantilla.ReferenciaFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
            it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

            float x1c = xMinReferencia;
            float y1c = yMaxReferencia - espaciado - pdfFontSize;
            float x2c = 0;
            float y2c = 0;
            x1c += pdfFontSize / 2;
            int col = 1;
            foreach (Layer layer in lstLayers.OrderByDescending(l => l.Orden))
            {
                //Si el contorno grosor es mayor a uno le pone uno porq sino se empasta y no se ve lo del medio
                float pdfContornoGrosor = layer.ContornoGrosor != null && (float)layer.ContornoGrosor.Value > 0 ? 1 : 0;

                it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                it.BaseColor pdfRellenoColor = GetAlphaColor(layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent, layer.RellenoTransparencia);
                PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);

                string texto = layer.Nombre;
                x2c = x1c + pdfFontSize;
                if (layer.PuntoRepresentacion == 0)
                {
                    //DIBUJAR AREAS
                    if (layer.Componente.Graficos == 1)
                    {
                        y2c = y1c + pdfFontSize;
                        PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y1c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter);
                    }
                    //dibujar linea
                    else if (layer.Componente.Graficos == 2)
                    {
                        y2c = y1c + (pdfFontSize / 2);
                        PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y2c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, null, null);
                    }
                }
                else if (layer.PuntoRepresentacion == 2)
                {
                    if (layer.PuntoImagen != null)
                    {
                        //DIBUJAR ICONO IMAGEN
                        float puntoAnchoPts = pdfFontSize;
                        float puntoAltoPts = pdfFontSize;
                        PDFUtilities.DrawPDFImage(pdfContentByte, layer.PuntoImagen, x1c, y1c, puntoAnchoPts, puntoAltoPts);
                    }
                }
                else if (layer.PuntoRepresentacion == 1)
                {
                    //DIBUJAR UN CIRCULO O RECTANGULO
                    float puntoAnchoPts = pdfFontSize;
                    float puntoAltoPts = pdfFontSize;
                    if (layer.PuntoPredeterminado == 2)
                    {
                        //Cuadrado  y linea contempla las 2
                        PDFUtilities.DrawPDFRectangle(pdfContentByte, x1c, y1c, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                    }
                    else if (layer.PuntoPredeterminado == 1)
                    {
                        //Circulo
                        float radio = pdfFontSize / 2;
                        PDFUtilities.DrawPDFCircle(pdfContentByte, (x1c + pdfFontSize / 2), (y1c + pdfFontSize / 2), radio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                    }
                }
                x2c = x2c + pdfFontSize / 2;
                float yTexto = y1c;
                PDFUtilities.DrawPDFText(pdfContentByte, texto, x2c, yTexto, pdfFont, pdfFontSize, alignment);

                y1c = y1c - pdfFontSize - espaciado;
                if (y1c <= yMinReferencia)
                {
                    y1c = yMaxReferencia - espaciado - pdfFontSize;
                    col++;
                    if (col > plantilla.ReferenciaColumnas)
                    {
                        break;
                    }
                    else
                    {
                        x1c += ((xMaxReferencia - xMinReferencia) / plantilla.ReferenciaColumnas);
                    }
                }
            }
        }

        private void DibujarTextos(PdfContentByte pdfContentByte, Plantilla plantilla, string idObjetoBase, int idComponenteObjetoBase, double factorEscala, Dictionary<string, string> dicTextosVariables, double xCentroidBase, double yCentroidBase, string imageName, string numeroODT = "", string tipo = "", string relacion = "", string direccionCompleta = "", string datosReferencia = "", string lstAtributos = "", string codigoLGC = "")
        {
            Layer layerBase = plantilla.Layers.FirstOrDefault(p => p.Categoria == 1 && p.ComponenteId == idComponenteObjetoBase && p.FechaBaja == null);

            string direccion = "";
            string localidad = "";
            string[] separarDireccion;
            if (direccionCompleta != null)
            {
                try
                {
                    separarDireccion = direccionCompleta.Split('-');
                    direccion = separarDireccion[0];
                    localidad = separarDireccion[1];
                }
                catch (Exception ex)
                {
                }
            }

            foreach (var plantillaTexto in plantilla.PlantillaTextos)
            {
                Color colorTexto = ColorTranslator.FromHtml(plantillaTexto.FuenteColor);
                int alignment = plantillaTexto.FuenteAlineacion + it.Element.ALIGN_MIDDLE;

                float pdfFontSize = it.Utilities.MillimetersToPoints((float)plantillaTexto.FuenteTamanio);
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);

                PDFUtilities.RegisterBaseFont(plantillaTexto.FuenteNombre, pdfFontSize);
                string[] aFontStylePdf = plantillaTexto.FuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(x => Convert.ToInt32(x)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(plantillaTexto.FuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                string texto = string.Empty;
                if (plantillaTexto.Tipo == 1)
                {
                    #region Texto Fijo
                    texto = plantillaTexto.Origen;
                    #endregion
                }
                else if (plantillaTexto.Tipo == 2)
                {
                    #region Texto Variable
                    //Se muestran solo las variables Escala y fecha y las que vienen definidas como parametro
                    string variable = plantillaTexto.Origen.ToUpper();
                    if (variable.Equals("@ESCALA"))
                    {
                        texto = "1:" + Math.Round((1 / factorEscala), 0).ToString();
                    }
                    else if (variable.Equals("@FECHA"))
                    {
                        texto = DateTime.Now.ToShortDateString();
                    }
                    else if (variable.Equals("@DISTRITO"))
                    {
                        if (layerBase != null)
                        {
                            if (!string.IsNullOrEmpty(idObjetoBase))
                            {
                                texto = GetLayerGrafDistritoById(layerBase.Componente, idObjetoBase);
                            }
                        }
                    }
                    else if (variable.Contains("@DISTRITOGRAFICO"))
                    {
                        texto = GetLayerGrafDistritoByCoords(xCentroidBase, yCentroidBase);
                    }
                    else if (variable.Equals("@IMAGEN"))
                    {
                        texto = imageName;
                    }
                    else if (variable.Equals("@DIRECCION"))
                    {

                        texto = direccion;
                    }
                    else if (variable.Equals("@ODT"))
                    {
                        texto = numeroODT;
                    }
                    else if (variable.Equals("@CTM"))
                    {
                        texto = relacion;
                    }
                    else if (variable.Equals("@LOCALIDAD"))
                    {
                        texto = localidad;
                    }
                    else if (variable.Equals("@REFERENCIA"))
                    {
                        #region Referencia
                        texto = datosReferencia;
                        double x = plantillaTexto.X;
                        double y = plantillaTexto.Y;

                        float xMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMin);
                        float yMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMin);
                        float xMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
                        float yMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMax);

                        float xPDF = (float)it.Utilities.MillimetersToPoints((float)x);
                        float yPDF = it.Utilities.MillimetersToPoints((float)y);
                        //Dibujamo
                        float yAux = yPDF;
                        int maxRenglones = 8;

                        FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "MAX_RENGLONES_REFERENCIA");
                        if (funcAdicParametro != null)
                        {
                            maxRenglones = Convert.ToInt32(funcAdicParametro.Valor);
                        }
                        PDFUtilities.DrawPDFTextMaxSteel(pdfContentByte, texto, xPDF, ref yPDF, xMaxReferencia, pdfFont, pdfFontSize, alignment, maxRenglones);

                        continue;
                        #endregion
                    }
                    else if (variable.Equals("@CODIGOLGC"))//Solo para LGC //Se envia por aca para evitar tener un layerbase y objetobase.
                    {
                        texto = codigoLGC;
                    }
                    else if (variable.Contains("@REGIONGRAFICO"))
                    {
                        texto = GetLayerGrafRegionByCoords(xCentroidBase, yCentroidBase);
                    }
                    else if (variable.Contains("@KMREDGC"))
                    {
                        /*Componente compCañeria = _componenteRepository.GetComponenteById(Convert.ToInt32(idComponenteObjetoBase));

                        List<string> lstTipo = new List<string>();
                        //Tipo red 2

                        string IdTipoRed2 = this._parametroRepository.GetParametroByDescripcion("TIPO_RED_CAN_MAES_CLOC_MAX");
                        lstTipo.Add(IdTipoRed2);
                        if (compCañeria.IdServicio == 2)
                        {//Tipo red 3
                            string IdTipoRed3 = this._parametroRepository.GetParametroByDescripcion("TIPO_RED_SUBTERRANEO");
                            lstTipo.Add(IdTipoRed3);
                        }

                        texto = GetLayerGrafKMMallaByCoords(Convert.ToInt32(idObjetoBase), compCañeria, lstTipo);
                        if (string.IsNullOrEmpty(texto))
                        {
                            texto = "0";
                        }*/
                    }
                    else if (variable.Contains("@KMREDDIST"))
                    {
                        Componente compCañeria = _componenteRepository.GetComponenteById(Convert.ToInt32(idComponenteObjetoBase));

                        List<string> lstTipo = new List<string>();
                        //Tipo red 1
                        string IdTipoRed = this._parametroRepository.GetParametroByDescripcion("TIPO_RED_SECUNDARIA");
                        lstTipo.Add(IdTipoRed);

                        texto = GetLayerGrafKMMallaByCoords(Convert.ToInt32(idObjetoBase), compCañeria, lstTipo);
                    }
                    else
                    {
                        if (dicTextosVariables.ContainsKey(variable))
                        {
                            texto = dicTextosVariables[variable];
                        }
                    }
                    #endregion
                }
                else if (plantillaTexto.Tipo == 3)
                {
                    #region Texto Atributos
                    //Textos de Datos del objeto base
                    if (layerBase != null)
                    {
                        if (!string.IsNullOrEmpty(idObjetoBase))
                        {
                            texto = GetLayerGrafTextById(layerBase.Componente, idObjetoBase, plantillaTexto.AtributoId.Value);
                        }
                    }
                    #endregion
                }
                if (!string.IsNullOrEmpty(texto))
                {
                    #region Dibujar Texto
                    float sizeText = pdfbaseFont.GetWidthPoint(texto, pdfFontSize);
                    float xPDF = (float)it.Utilities.MillimetersToPoints((float)plantillaTexto.X);
                    if (plantillaTexto.FuenteAlineacion == 2)
                    {
                        xPDF -= sizeText / 2;
                    }
                    else if (plantillaTexto.FuenteAlineacion == 3)
                    {
                        xPDF -= sizeText;
                    }
                    float yPDF = it.Utilities.MillimetersToPoints((float)(plantillaTexto.Y));
                    PDFUtilities.DrawPDFText(pdfContentByte, texto, xPDF, yPDF, pdfFont, pdfFontSize, alignment);
                    #endregion
                }
            }
        }

        private PdfPatternPainter GetPattern(PdfContentByte cb, float width, float heigth, double lineWidth)
        {
            PdfPatternPainter pattern = cb.CreatePattern(width, heigth, null);
            pattern.SetLineWidth(lineWidth);
            pattern.MoveTo(-(width / 2), 0);
            pattern.LineTo(width, heigth + (heigth / 2));
            pattern.Stroke();
            pattern.MoveTo(0, -(heigth / 2));
            pattern.LineTo(width + (width / 2), heigth);
            pattern.Stroke();
            return pattern;
        }

        private void DibujarLayerGraf<T>(PdfContentByte pdfContentByte, T[] aLayerGraf, string layerGrafLabelProperty, Layer layer, double xCentroidBase, double yCentroidBase, double escala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario)
        {
            /*
             * it.Utilities.MillimetersToPoints() -> Transforma unidad de medida de milimetros a puntos.
             * GetWidthPoint() -> Devuelve el ancho del texto en puntos. (No contempla los espacios)
             * Hay 72 puntos en 1 pulgada
             * xPuntos / 72 = xPulgadas
             * xPulgadas * 2.54 = xCentimetros
             * El FontSize CREO que esta en unidad PUNTOS.
             */
            try
            {
                float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;

                it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                it.BaseColor pdfRellenoColor = GetAlphaColor(Color.White, 99);
                if (layer.Relleno)
                {
                    pdfRellenoColor = GetAlphaColor(ColorTranslator.FromHtml(layer.RellenoColor), layer.RellenoTransparencia);
                }
                PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);
                string lineDash = layer.Dash;

                int alignment = it.Element.ALIGN_MIDDLE | it.Element.ALIGN_CENTER;

                PropertyInfo propertyEtiqueta = null;
                Color etiquetaColor = Color.Black;
                it.BaseColor pdfEtiquetaColor = null;
                float pdfFontSize = 0;
                string[] aFontStylePdf = null;
                int pdfFontStyle = 0;
                BaseFont pdfbaseFont = null;
                it.Font pdfFont = null;

                if (layer.Etiqueta)
                {
                    propertyEtiqueta = typeof(T).GetProperty(layerGrafLabelProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    etiquetaColor = (!string.IsNullOrEmpty(layer.EtiquetaColor) ? ColorTranslator.FromHtml(layer.EtiquetaColor) : Color.Black);
                    pdfEtiquetaColor = new it.BaseColor(etiquetaColor.R, etiquetaColor.G, etiquetaColor.B);
                    pdfFontSize = (layer.EtiquetaFuenteTamanio != null ? it.Utilities.MillimetersToPoints((float)layer.EtiquetaFuenteTamanio.Value) : 0);
                    if (pdfFontSize > 0)
                    {
                        PDFUtilities.RegisterBaseFont(layer.EtiquetaFuenteNombre, pdfFontSize);
                        aFontStylePdf = layer.EtiquetaFuenteEstilo.Split(',');
                        pdfFontStyle = aFontStylePdf.Select(fs => Convert.ToInt32(fs)).Sum();
                        pdfbaseFont = it.FontFactory.GetFont(layer.EtiquetaFuenteNombre, pdfFontSize, pdfFontStyle, pdfEtiquetaColor).BaseFont;
                        pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfEtiquetaColor);
                    }
                }
                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                float x1Pdf = 0, x2Pdf = 0;
                float y1Pdf = 0, y2Pdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);

                float xMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMax);
                float xMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMin);
                float yMax = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMax);
                float yMin = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMin);

                double anguloRotacionPdf = 0;
                var propertyRotacion = typeof(T).GetProperty("Rotation");
                foreach (var layerGraf in aLayerGraf)
                {
                    if (layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf) != null)
                    {
                        DbGeometry geometryLayerGraf = (DbGeometry)layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf);
                        string wkt = geometryLayerGraf.AsText();

                        if (wkt.Contains("POLYG"))
                        {
                            #region POLYGON/MULTIPOLYGON
                            int elementCount = (geometryLayerGraf.ElementCount != null ? (int)geometryLayerGraf.ElementCount : 1);
                            for (int iElem = 1; iElem <= elementCount; iElem++)
                            {
                                DbGeometry geometryLayerGrafElem = geometryLayerGraf;
                                if (geometryLayerGraf.ExteriorRing != null)
                                {
                                    geometryLayerGrafElem = geometryLayerGraf.ExteriorRing;
                                }
                                if (geometryLayerGraf.ElementCount != null)
                                {
                                    geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem);
                                    if (geometryLayerGraf.ElementAt(iElem).ExteriorRing != null)
                                    {
                                        geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem).ExteriorRing;
                                    }
                                }
                                List<Ring> lstRing = new List<Ring>();
                                int cantCoords = (int)geometryLayerGrafElem.PointCount;
                                List<PointF> lstPointPDF = new List<PointF>();
                                for (int i = 1; i <= cantCoords; i++)
                                {
                                    x = (double)geometryLayerGrafElem.PointAt(i).XCoordinate;
                                    y = (double)geometryLayerGrafElem.PointAt(i).YCoordinate;
                                    if (i > 1)
                                    {
                                        x1 = x2;
                                        y1 = y2;
                                    }
                                    else
                                    {
                                        x1 = x;
                                        y1 = y;
                                    }
                                    x2 = x;
                                    y2 = y;

                                    if (i > 1)
                                    {
                                        #region Procesar punto
                                        x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                        y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                        x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                        y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);

                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                            anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                            x1Pdf = ptRotado1.X;
                                            y1Pdf = ptRotado1.Y;
                                            x2Pdf = ptRotado2.X;
                                            y2Pdf = ptRotado2.Y;
                                        }
                                        if (i == 2) //Para agregar el 1er punto
                                        {
                                            lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
                                        }
                                        lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
                                        #endregion
                                    }
                                }
                                lstRing.Add(new Ring() { Interior = false, Puntos = lstPointPDF });
                                x2 = x;
                                y2 = y;

                                if (geometryLayerGraf.ElementAt(iElem).InteriorRingCount > 0)
                                {//Para las islas (geometrias poligonos internos)
                                    #region Islas Internas
                                    for (int iRing = 1; iRing <= geometryLayerGraf.ElementAt(iElem).InteriorRingCount; iRing++)
                                    {
                                        DbGeometry geomInteriorRing = geometryLayerGraf.ElementAt(iElem).InteriorRingAt(iRing);
                                        int cantCoordsIntRing = (int)geomInteriorRing.PointCount;
                                        List<PointF> lstPointPDFRing = new List<PointF>();
                                        for (int i = 1; i <= cantCoordsIntRing; i++)
                                        {
                                            x = (double)geomInteriorRing.PointAt(i).XCoordinate;
                                            y = (double)geomInteriorRing.PointAt(i).YCoordinate;
                                            if (i > 1)
                                            {
                                                x1 = x2;
                                                y1 = y2;
                                            }
                                            else
                                            {
                                                x1 = x;
                                                y1 = y;
                                            }
                                            x2 = x;
                                            y2 = y;

                                            if (i > 1)
                                            {
                                                #region Dibujar
                                                x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                                y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                                x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                                y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
                                                if (plantilla.OptimizarTamanioHoja)
                                                {
                                                    PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                    anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                                    x1Pdf = ptRotado1.X;
                                                    y1Pdf = ptRotado1.Y;
                                                    x2Pdf = ptRotado2.X;
                                                    y2Pdf = ptRotado2.Y;
                                                }
                                                if (i == 2) //Para agregar el 1er punto
                                                {
                                                    lstPointPDFRing.Add(new PointF(x1Pdf, y1Pdf));
                                                }
                                                lstPointPDFRing.Add(new PointF(x2Pdf, y2Pdf));
                                                #endregion
                                            }
                                        }
                                        lstRing.Add(new Ring() { Interior = true, Puntos = lstPointPDFRing });
                                    }
                                    #endregion
                                }
                                //Dibuja
                                PDFUtilities.DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                            }
                            #endregion
                        }
                        else if (wkt.Contains("MULTILINE"))
                        {
                            #region MULTILINE
                            int elementCount = (geometryLayerGraf.ElementCount != null ? (int)geometryLayerGraf.ElementCount : 1);
                            for (int iElem = 1; iElem <= elementCount; iElem++)
                            {
                                DbGeometry geometryLayerGrafElem = geometryLayerGraf;
                                if (geometryLayerGraf.ExteriorRing != null)
                                {
                                    geometryLayerGrafElem = geometryLayerGraf.ExteriorRing;
                                }
                                if (geometryLayerGraf.ElementCount != null)
                                {
                                    geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem);
                                    if (geometryLayerGraf.ElementAt(iElem).ExteriorRing != null)
                                    {
                                        geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem).ExteriorRing;
                                    }
                                }
                                List<Ring> lstRing = new List<Ring>();
                                int cantCoords = (int)geometryLayerGrafElem.PointCount;
                                List<PointF> lstPointPDF = new List<PointF>();
                                for (int i = 1; i <= cantCoords; i++)
                                {
                                    x = (double)geometryLayerGrafElem.PointAt(i).XCoordinate;
                                    y = (double)geometryLayerGrafElem.PointAt(i).YCoordinate;
                                    if (i > 1)
                                    {
                                        x1 = x2;
                                        y1 = y2;
                                    }
                                    else
                                    {
                                        x1 = x;
                                        y1 = y;
                                    }
                                    x2 = x;
                                    y2 = y;

                                    if (i > 1)
                                    {
                                        #region Procesar
                                        x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                        y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                        x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                        y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                            anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                            x1Pdf = ptRotado1.X;
                                            y1Pdf = ptRotado1.Y;
                                            x2Pdf = ptRotado2.X;
                                            y2Pdf = ptRotado2.Y;
                                        }
                                        if (i == 2) //Para agregar el 1er punto
                                        {
                                            lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
                                        }
                                        lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
                                        #endregion
                                    }
                                }
                                lstRing.Add(new Ring() { Interior = false, Puntos = lstPointPDF });
                                x2 = x;
                                y2 = y;

                                if (geometryLayerGraf.ElementAt(iElem).InteriorRingCount > 0)
                                {
                                    for (int iRing = 1; iRing <= geometryLayerGraf.ElementAt(iElem).InteriorRingCount; iRing++)
                                    {
                                        #region Islas Internas
                                        DbGeometry geomInteriorRing = geometryLayerGraf.ElementAt(iElem).InteriorRingAt(iRing);
                                        int cantCoordsIntRing = (int)geomInteriorRing.PointCount;
                                        List<PointF> lstPointPDFRing = new List<PointF>();
                                        for (int i = 1; i <= cantCoordsIntRing; i++)
                                        {
                                            x = (double)geomInteriorRing.PointAt(i).XCoordinate;
                                            y = (double)geomInteriorRing.PointAt(i).YCoordinate;
                                            if (i > 1)
                                            {
                                                x1 = x2;
                                                y1 = y2;
                                            }
                                            else
                                            {
                                                x1 = x;
                                                y1 = y;
                                            }
                                            x2 = x;
                                            y2 = y;

                                            if (i > 1)
                                            {
                                                #region Procesar
                                                x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                                y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                                x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                                y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
                                                if (plantilla.OptimizarTamanioHoja)
                                                {
                                                    PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                    anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                                    x1Pdf = ptRotado1.X;
                                                    y1Pdf = ptRotado1.Y;
                                                    x2Pdf = ptRotado2.X;
                                                    y2Pdf = ptRotado2.Y;
                                                }
                                                if (i == 2) //Para agregar el 1er punto
                                                {
                                                    lstPointPDFRing.Add(new PointF(x1Pdf, y1Pdf));
                                                }
                                                lstPointPDFRing.Add(new PointF(x2Pdf, y2Pdf));
                                                #endregion
                                            }
                                        }
                                        lstRing.Add(new Ring() { Interior = true, Puntos = lstPointPDFRing });
                                        #endregion
                                    }
                                }
                                //Dibuja
                                PDFUtilities.DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                            }
                            #endregion
                        }
                        else if (wkt.Contains("LINE"))
                        {
                            #region LINE
                            List<PointD> lstCoordsGeometry = GetlstCoordsGeometry(geometryLayerGraf, wkt);
                            List<PointF> lstPointPDF = new List<PointF>();
                            List<Ring> lstRing = new List<Ring>();
                            for (int i = 0; i < lstCoordsGeometry.Count; i++)
                            {
                                x = lstCoordsGeometry[i].X;
                                y = lstCoordsGeometry[i].Y;

                                if (i > 0)
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

                                x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
                                if (layer.Contorno)
                                {
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);

                                        anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                        anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;

                                        if (i == 0)
                                        {
                                            lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                        }
                                        else
                                        {
                                            lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                        }
                                    }
                                    else
                                    {
                                        if (i == 0)
                                        {
                                            lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
                                        }
                                        else
                                        {
                                            lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
                                        }
                                    }
                                }
                            }
                            lstRing.Add(new Ring() { Puntos = lstPointPDF });
                            PDFUtilities.DrawPDFLine(pdfContentByte, lstRing, pdfContornoGrosor, pdfContornoColor, lineDash);
                            x2 = x;
                            y2 = y;
                            #endregion
                        }
                        else if (wkt.Contains("POINT"))
                        {
                            #region POINT
                            x = (double)geometryLayerGraf.XCoordinate;
                            y = (double)geometryLayerGraf.YCoordinate;
                            x1Pdf = GetXPDFCanvas(x, xCentroidBase, escala, plantilla);
                            y1Pdf = GetYPDFCanvas(y, yCentroidBase, escala, plantilla);
                            float rotationRad = 0;
                            float rotation = Convert.ToSingle((int?)propertyRotacion.GetValue(layerGraf) ?? 0);
                            if (rotation != 0f)
                            {
                                rotationRad = (float)(rotation / 10 * Math.PI / 180);
                            }
                            if (layer.PuntoRepresentacion == 2)
                            {
                                if (layer.PuntoImagen != null)
                                {
                                    #region Imagen
                                    float anchoEscalado = (float)layer.PuntoAncho.Value;
                                    float altoEscalado = (float)layer.PuntoAlto.Value;
                                    double escalaReal = 1 / escala;
                                    float puntoAnchoPts = it.Utilities.MillimetersToPoints(anchoEscalado);
                                    float puntoAltoPts = it.Utilities.MillimetersToPoints(altoEscalado);

                                    Bitmap bmpPunto = (Bitmap)layer.PuntoImagen;

                                    float x1PdfCentroCordenadas = 0;
                                    float y1PdfCentroCordenadas = 0;

                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        rotationRad = !sentidoHorario ? rotationRad - (float)anguloRotacion : rotationRad + (float)anguloRotacion;
                                        x1PdfCentroCordenadas = ptRotado.X - (puntoAnchoPts / 2);
                                        y1PdfCentroCordenadas = ptRotado.Y - (puntoAltoPts / 2);
                                    }
                                    else
                                    {
                                        x1PdfCentroCordenadas = x1Pdf - (puntoAnchoPts / 2);
                                        y1PdfCentroCordenadas = y1Pdf - (puntoAltoPts / 2);
                                    }

                                    if ((x1PdfCentroCordenadas + puntoAnchoPts) * 1.05 > xMax)
                                    {
                                        continue;
                                    }
                                    else if ((x1PdfCentroCordenadas - puntoAnchoPts / 2) * 1.05 < xMin)
                                    {
                                        continue;
                                    }
                                    if ((y1PdfCentroCordenadas + puntoAltoPts / 2) * 1.05 > yMax)
                                    {
                                        continue;
                                    }
                                    else if ((y1PdfCentroCordenadas - puntoAltoPts / 2) * 1.05 < yMin)
                                    {
                                        continue;
                                    }

                                    PDFUtilities.DrawPDFImage(pdfContentByte, bmpPunto, x1PdfCentroCordenadas, y1PdfCentroCordenadas, puntoAnchoPts, puntoAltoPts, rotationRad);

                                    #endregion
                                }
                            }
                            else if (layer.PuntoRepresentacion == 1 && layer.Contorno)
                            {
                                float anchoEscalado = (float)layer.PuntoAncho.Value;
                                float altoEscalado = (float)layer.PuntoAlto.Value;
                                double escalaReal = 1 / escala;
                                if (layer.PuntoEscala.HasValue && layer.PuntoEscala.Value != escalaReal)
                                {
                                    anchoEscalado = (float)(layer.PuntoAncho.Value * (1 / escalaReal) / (1 / (float)layer.PuntoEscala.Value));
                                    altoEscalado = (float)(layer.PuntoAlto.Value * (1 / escalaReal) / (1 / (float)layer.PuntoEscala.Value));
                                }
                                float puntoAnchoPts = it.Utilities.MillimetersToPoints(anchoEscalado);
                                float puntoAltoPts = it.Utilities.MillimetersToPoints(altoEscalado);
                                float puntoRadio = it.Utilities.MillimetersToPoints(anchoEscalado / 2);
                                if (layer.PuntoPredeterminado == 1)
                                {
                                    #region Circulo
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        PDFUtilities.DrawPDFCircle(pdfContentByte, ptRotado.X, ptRotado.Y, puntoRadio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                    else
                                    {
                                        PDFUtilities.DrawPDFCircle(pdfContentByte, x1Pdf, y1Pdf, puntoRadio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                    #endregion
                                }
                                else if (layer.PuntoPredeterminado == 2)
                                {
                                    #region Cuadrado
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        PDFUtilities.DrawPDFRectangle(pdfContentByte, ptRotado.X, ptRotado.Y, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                    else
                                    {
                                        PDFUtilities.DrawPDFRectangle(pdfContentByte, x1Pdf, y1Pdf, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        if (layer.Etiqueta)
                        {
                            #region Etiqueta
                            string layerGrafNombre = string.Empty;
                            if (!string.IsNullOrEmpty(layerGrafLabelProperty))
                            {
                                layerGrafNombre = propertyEtiqueta.GetValue(layerGraf)?.ToString();
                            }
                            if (!string.IsNullOrEmpty(layerGrafNombre))
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

                                float sizeText = pdfbaseFont.GetWidthPoint(layerGrafNombre, pdfFontSize);
                                var x1PdfCentroCordenadas = GetXPDFCanvas(xCentroidLayerGraf, xCentroidBase, escala, plantilla);
                                var y1PdfCentroCordenadas = GetYPDFCanvas(yCentroidLayerGraf, yCentroidBase, escala, plantilla);

                                if (!layer.EtiquetaMantieneOrientacion)
                                {
                                    #region EtiquetaNoMantieneOrientacion

                                    float xDrawPoint = x1PdfCentroCordenadas;
                                    float yDrawPoint = y1PdfCentroCordenadas;
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1PdfCentroCordenadas, y1PdfCentroCordenadas, xCentro, yCentro, anguloRotacion, sentidoHorario);

                                        x1PdfCentroCordenadas = ptRotado.X;
                                        y1PdfCentroCordenadas = ptRotado.Y;


                                        //xDrawPoint = ptRotado.X - sizeText / 2;//Restar el ancho del texto DESPUES DE ROTAR.
                                        yDrawPoint = ptRotado.Y - pdfFontSize / 2;//Restar el alto del texto DESPUES DE ROTAR.
                                                                                  //Pareciera ser que el fontsize ya esta en unidad de puntos.
                                    }
                                    /*
                                     else
                                     {
                                       Deberia crear una funcion para que reste x e y dependiendo el angulo. y no:
                                       xDrawPoint = ptRotado.X - sizeText / 2;
                                       yDrawPoint = ptRotado.Y - pdfFontSize / 2
                                       porque solo sirve con angulo 0°.
                                     */

                                    float sizeTxt = pdfFont.BaseFont.GetWidthPoint(layerGrafNombre + "xx", pdfFontSize);
                                    xDrawPoint = x1PdfCentroCordenadas - (sizeTxt / 2);
                                    //xDrawPoint = y1PdfCentroCordenadas - pdfFontSize / 2;

                                    //Deberia calcular si entre el alto y el ancho cual es el mas grande y usar ese valor para comparar.
                                    //Pero doy por echo que siempre el ancho va a ser mas grande que el alto.

                                    if (x1PdfCentroCordenadas + sizeText / 2 > xMax)
                                    {
                                        continue;
                                    }
                                    else if (x1PdfCentroCordenadas - sizeText / 2 < xMin)
                                    {
                                        continue;
                                    }

                                    if (y1PdfCentroCordenadas + sizeText / 2 > yMax)
                                    {
                                        continue;
                                    }
                                    else if (y1PdfCentroCordenadas - sizeText / 2 < yMin)
                                    {
                                        continue;
                                    }

                                    if (pdfFont != null && pdfFontSize > 0)
                                    {
                                        PDFUtilities.DrawPDFText(pdfContentByte, layerGrafNombre, xDrawPoint, yDrawPoint, pdfFont, pdfFontSize, alignment);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Aplicar Angulo a Texto
                                    //Aca no resto el alto y ancho de texto porque el angulo no es 0°
                                    //y no se calcular el x e y que deberia restar :c
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

                                    angulo = angulo * 180 / Math.PI;

                                    double anguloTexto = 0;

                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        x1PdfCentroCordenadas = ptRotado.X;
                                        y1PdfCentroCordenadas = ptRotado.Y;
                                        anguloTexto = anguloRotacionPdf;
                                    }
                                    else
                                    {
                                        double anguloPdf = Math.Atan((y2 - y1) / (x2 - x1));
                                        anguloTexto = anguloPdf * 180 / Math.PI;
                                    }

                                    if (x1PdfCentroCordenadas + sizeText / 2 > xMax)
                                    {
                                        continue;
                                    }
                                    else if (x1PdfCentroCordenadas - sizeText / 2 < xMin)
                                    {
                                        continue;
                                    }

                                    if (y1PdfCentroCordenadas + sizeText / 2 > yMax)
                                    {
                                        continue;
                                    }
                                    else if (y1PdfCentroCordenadas - sizeText / 2 < yMin)
                                    {
                                        continue;
                                    }

                                    if (pdfFont != null && pdfFontSize > 0)
                                    {
                                        PDFUtilities.DrawPDFText(pdfContentByte, layerGrafNombre, x1PdfCentroCordenadas, y1PdfCentroCordenadas, pdfbaseFont, pdfFontSize, etiquetaColor, anguloTexto);
                                    }

                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError(string.Format("DibujarLayerGraf - Layer({0})", layer.Nombre), ex);
            }
        }

        private void DibujarLayerGrafMapaTematico<T>(PdfContentByte pdfContentByte, T[] aLayerGraf, double xCentroidBase, double yCentroidBase, double escala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario, ObjetoResultadoDetalle objetoResultadoDetalle, bool verIdentificante)
        {
            try
            {
                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                float x1Pdf = 0, x2Pdf = 0;
                float y1Pdf = 0, y2Pdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                double anguloRotacionPdf = 0;

                Color etiquetaColor = Color.Black;
                it.BaseColor pdfEtiquetaColor = null;
                float pdfFontSize = 0;
                string[] aFontStylePdf = null;
                int pdfFontStyle = 0;
                BaseFont pdfbaseFont = null;
                it.Font pdfFont = null;

                Layer layerMapaTematico = plantilla.Layers.FirstOrDefault(p => p.Categoria == 3 && p.FechaBaja == null);

                //layerMapaTematico.EtiquetaFuenteTamanio = 1.0;
                //agregar una validacion , una bandera si esta en true que pinte los textos.
                //tambien agregar en base a la escala que llega, de achicar la letra o agrandarla
                //escalareferencia seria layerMapaTematico.EscalaReferencia que vendria del mp layer 
                double escalaReal = 1 / escala;
                if (layerMapaTematico.EtiquetaEscalaReferencia != escalaReal)
                {

                    layerMapaTematico.EtiquetaFuenteTamanio = layerMapaTematico.EtiquetaFuenteTamanio * layerMapaTematico.EtiquetaEscalaReferencia / escalaReal;

                }
                if (layerMapaTematico != null)
                {
                    etiquetaColor = (!string.IsNullOrEmpty(layerMapaTematico.EtiquetaColor) ? ColorTranslator.FromHtml(layerMapaTematico.EtiquetaColor) : Color.Black);
                    pdfEtiquetaColor = new it.BaseColor(etiquetaColor.R, etiquetaColor.G, etiquetaColor.B);
                    pdfFontSize = (layerMapaTematico.EtiquetaFuenteTamanio != null ? it.Utilities.MillimetersToPoints((float)layerMapaTematico.EtiquetaFuenteTamanio.Value) : 0);
                    if (pdfFontSize > 0)
                    {
                        PDFUtilities.RegisterBaseFont(layerMapaTematico.EtiquetaFuenteNombre, pdfFontSize);
                        aFontStylePdf = layerMapaTematico.EtiquetaFuenteEstilo.Split(',');
                        pdfFontStyle = aFontStylePdf.Select(fs => Convert.ToInt32(fs)).Sum();
                        pdfbaseFont = it.FontFactory.GetFont(layerMapaTematico.EtiquetaFuenteNombre, pdfFontSize, pdfFontStyle, pdfEtiquetaColor).BaseFont;
                        pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfEtiquetaColor);
                    }
                }
                foreach (var layerGraf in aLayerGraf)
                {
                    string layerGrafNombre = layerGraf.GetType().GetProperty("Nombre", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(layerGraf).ToString();
                    string layerGrafDescripcion = layerGraf.GetType().GetProperty("Descripcion", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(layerGraf).ToString();
                    int nroRango = GetRangoNro(layerGrafNombre) - 1;

                    if (nroRango < 0)
                        continue;

                    Rango rango = objetoResultadoDetalle.Rangos[nroRango];

                    float anchoBorde = rango.AnchoBorde != 0 ? it.Utilities.MillimetersToPoints((float)(rango.AnchoBorde / 10.0)) : 0;
                    float pdfContornoGrosor = anchoBorde;

                    it.BaseColor pdfContornoColor = GetAlphaColor(ColorTranslator.FromHtml("#" + rango.ColorBorde), (int)objetoResultadoDetalle.Transparencia);
                    it.BaseColor pdfRellenoColor = GetAlphaColor(ColorTranslator.FromHtml("#" + rango.Color), (int)objetoResultadoDetalle.Transparencia);

                    int alignment = it.Element.ALIGN_CENTER + it.Element.ALIGN_MIDDLE;
                    int puntoAncho = 4;
                    int puntoAlto = 4;

                    if (layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf) != null)
                    {
                        DbGeometry geometryLayerGraf = (DbGeometry)layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf);
                        string wkt = geometryLayerGraf.AsText();

                        if (wkt.Contains("POLYG"))
                        {
                            #region POLYGON/MULTIPOLYGON
                            int elementCount = (geometryLayerGraf.ElementCount != null ? (int)geometryLayerGraf.ElementCount : 1);
                            for (int iElem = 1; iElem <= elementCount; iElem++)
                            {
                                DbGeometry geometryLayerGrafElem = geometryLayerGraf;
                                if (geometryLayerGraf.ExteriorRing != null)
                                {
                                    geometryLayerGrafElem = geometryLayerGraf.ExteriorRing;
                                }
                                if (geometryLayerGraf.ElementCount != null)
                                {
                                    geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem);
                                    if (geometryLayerGraf.ElementAt(iElem).ExteriorRing != null)
                                    {
                                        geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem).ExteriorRing;
                                    }
                                }
                                List<Ring> lstRing = new List<Ring>();
                                int cantCoords = (int)geometryLayerGrafElem.PointCount;
                                List<PointF> lstPointPDF = new List<PointF>();
                                for (int i = 1; i <= cantCoords; i++)
                                {
                                    x = (double)geometryLayerGrafElem.PointAt(i).XCoordinate;
                                    y = (double)geometryLayerGrafElem.PointAt(i).YCoordinate;
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
                                        #region Dibujar
                                        x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                        y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                        x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                        y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            if (i == 2)//Para anotar la 1er coordenada
                                                lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                            lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));

                                            anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                            anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                        }
                                        else
                                        {
                                            if (i == 2)//Para anotar la 1er coordenada
                                                lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
                                            lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
                                        }
                                        #endregion
                                    }
                                }
                                Ring ring = new Ring();
                                ring.Interior = false;
                                ring.Puntos = lstPointPDF;
                                lstRing.Add(ring);
                                x2 = x;
                                y2 = y;

                                if (geometryLayerGraf.ElementAt(iElem).InteriorRingCount > 0)
                                {
                                    #region Islas Internas
                                    for (int iRing = 1; iRing <= geometryLayerGraf.ElementAt(iElem).InteriorRingCount; iRing++)
                                    {
                                        DbGeometry geomInteriorRing = geometryLayerGraf.ElementAt(iElem).InteriorRingAt(iRing);
                                        int cantCoordsIntRing = (int)geomInteriorRing.PointCount;
                                        List<PointF> lstPointPDFRing = new List<PointF>();
                                        for (int i = 1; i <= cantCoordsIntRing; i++)
                                        {
                                            x = (double)geomInteriorRing.PointAt(i).XCoordinate;
                                            y = (double)geomInteriorRing.PointAt(i).YCoordinate;
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
                                                #region Dibujar
                                                x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                                y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                                x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                                y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
                                                if (plantilla.OptimizarTamanioHoja)
                                                {
                                                    PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    if (i == 2)//Para anotar la 1er coordenada
                                                        lstPointPDFRing.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                                    lstPointPDFRing.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                                    anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                    anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                                }
                                                else
                                                {
                                                    if (i == 2)//Para anotar la 1er coordenada
                                                        lstPointPDFRing.Add(new PointF(x1Pdf, y1Pdf));
                                                    lstPointPDFRing.Add(new PointF(x2Pdf, y2Pdf));
                                                }
                                                #endregion
                                            }
                                        }
                                        ring = new Ring();
                                        ring.Interior = true;
                                        ring.Puntos = lstPointPDFRing;
                                        lstRing.Add(ring);
                                    }
                                    #endregion
                                }

                                if (pdfContornoGrosor == 0)
                                {
                                    pdfContornoColor = pdfRellenoColor;
                                }

                                PDFUtilities.DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
                            #endregion
                        }
                        else if (wkt.Contains("LINE"))
                        {
                            if (rango.AnchoBorde == 0)
                            {
                                anchoBorde = it.Utilities.MillimetersToPoints((float)(1 / 10));
                                pdfContornoGrosor = anchoBorde;
                            }
                            #region LINE
                            DbGeometry geometryLayerGrafElem = geometryLayerGraf;
                            List<PointD> lstCoordsGeometry = GetlstCoordsGeometry(geometryLayerGraf, wkt);
                            List<PointF> lstPointPDF = new List<PointF>();
                            List<Ring> lstRing = new List<Ring>();
                            for (int i = 0; i < lstCoordsGeometry.Count; i++)
                            {
                                x = lstCoordsGeometry[i].X;
                                y = lstCoordsGeometry[i].Y;
                                if (i > 0)
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
                                if (i > 0)
                                {
                                    //dibujar
                                    x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
                                    y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
                                    x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
                                    y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        if (i == 2)//Para anotar la 1er coordenada
                                            lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                        lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                        anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                        anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                    }
                                    else
                                    {
                                        if (i == 2)//Para anotar la 1er coordenada
                                            lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
                                        lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
                                    }
                                }
                            }
                            Ring ring = new Ring();
                            ring.Puntos = lstPointPDF;
                            lstRing.Add(ring);
                            PDFUtilities.DrawPDFLine(pdfContentByte, lstRing, pdfContornoGrosor, pdfContornoColor, "");
                            x2 = x;
                            y2 = y;
                            #endregion
                        }
                        else if (wkt.Contains("POINT"))
                        {
                            #region POINT
                            x = (double)geometryLayerGraf.XCoordinate;
                            y = (double)geometryLayerGraf.YCoordinate;
                            x1Pdf = GetXPDFCanvas(x, xCentroidBase, escala, plantilla);
                            y1Pdf = GetYPDFCanvas(y, yCentroidBase, escala, plantilla);
                            float puntoAnchoPts = it.Utilities.MillimetersToPoints((float)puntoAncho);
                            float puntoAltoPts = it.Utilities.MillimetersToPoints((float)puntoAlto);
                            float puntoRadio = it.Utilities.MillimetersToPoints((float)(puntoAncho / 2));
                            if (plantilla.OptimizarTamanioHoja)
                            {
                                PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                PDFUtilities.DrawPDFCircle(pdfContentByte, ptRotado.X, ptRotado.Y, puntoRadio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
                            else
                            {
                                PDFUtilities.DrawPDFCircle(pdfContentByte, x1Pdf, y1Pdf, puntoRadio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
                            #endregion
                        }
                        if (verIdentificante && !string.IsNullOrEmpty(layerGrafDescripcion) && pdfFontSize > 0)
                        {
                            #region Poner Label
                            double xCentroidLayerGraf = 0;
                            double yCentroidLayerGraf = 0;
                            if (wkt.Contains("LINE"))
                            {
                                //Si es Linea 
                                double xEndPoint, yEndPoint, xStartPoint, yStartPoint;
                                if (geometryLayerGraf.IsValid)
                                {
                                    xEndPoint = (double)geometryLayerGraf.EndPoint.XCoordinate;
                                    yEndPoint = (double)geometryLayerGraf.EndPoint.YCoordinate;
                                    xStartPoint = (double)geometryLayerGraf.StartPoint.XCoordinate;
                                    yStartPoint = (double)geometryLayerGraf.StartPoint.YCoordinate;
                                }
                                else
                                {
                                    List<PointD> lstCoordsGeometry = GetlstCoordsGeometry(geometryLayerGraf, wkt);
                                    xEndPoint = lstCoordsGeometry[lstCoordsGeometry.Count - 1].X;
                                    yEndPoint = lstCoordsGeometry[lstCoordsGeometry.Count - 1].Y;
                                    xStartPoint = lstCoordsGeometry[0].X;
                                    yStartPoint = lstCoordsGeometry[0].Y;
                                }
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
                                xCentroidLayerGraf = (double)geometryLayerGraf.XCoordinate;
                                yCentroidLayerGraf = (double)geometryLayerGraf.YCoordinate;
                            }
                            if (layerMapaTematico != null && !layerMapaTematico.EtiquetaMantieneOrientacion)
                            {
                                float sizeText = pdfbaseFont.GetWidthPoint(layerGrafDescripcion, pdfFontSize);
                                x1Pdf = GetXPDFCanvas(xCentroidLayerGraf, xCentroidBase, escala, plantilla) - sizeText / 2;
                                y1Pdf = GetYPDFCanvas(yCentroidLayerGraf, yCentroidBase, escala, plantilla);

                                float xDrawPoint = x1Pdf;
                                float yDrawPoint = y1Pdf;
                                if (plantilla.OptimizarTamanioHoja)
                                {
                                    PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                    xDrawPoint = ptRotado.X - sizeText / 2;
                                    yDrawPoint = ptRotado.Y + pdfFontSize / 2;
                                }
                                else
                                {
                                    xDrawPoint = x1Pdf;
                                    yDrawPoint = y1Pdf;
                                }
                                if (pdfFont != null && pdfFontSize > 0)
                                {
                                    //dibuja el texto de la parcela
                                    PDFUtilities.DrawPDFText(pdfContentByte, layerGrafDescripcion, xDrawPoint, yDrawPoint, pdfFont, pdfFontSize, alignment);
                                }
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

                                double xCalc = xCentroidLayerGraf;
                                double yCalc = yCentroidLayerGraf;

                                angulo = angulo * 180 / Math.PI;

                                x1Pdf = GetXPDFCanvas(xCalc, xCentroidBase, escala, plantilla);
                                y1Pdf = GetYPDFCanvas(yCalc, yCentroidBase, escala, plantilla);
                                if (plantilla.OptimizarTamanioHoja)
                                {
                                    PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                    PDFUtilities.DrawPDFText(pdfContentByte, layerGrafDescripcion, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, etiquetaColor, anguloRotacionPdf);
                                }
                                else
                                {
                                    double anguloPdf = Math.Atan((y2 - y1) / (x2 - x1));
                                    anguloPdf = anguloPdf * 180 / Math.PI;
                                    PDFUtilities.DrawPDFText(pdfContentByte, layerGrafDescripcion, x1Pdf, y1Pdf, pdfbaseFont, pdfFontSize, etiquetaColor, anguloPdf);
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("DibujarLayerGrafMapaTematico", ex);
            }
        }

        private it.BaseColor GetAlphaColor(Color color, int? alpha)
        {
            Color alphaColor = color;
            if (alpha.HasValue && alpha.Value > 0)
            {
                alphaColor = Color.FromArgb(255 - (255 * alpha.Value / 100), color.R, color.G, color.B);
            }
            return new it.BaseColor(alphaColor.R, alphaColor.G, alphaColor.B, alphaColor.A);
        }

        private int GetRangoNro(string layerGrafNombre)
        {
            int nroRango = 0;
            try
            {
                nroRango = Convert.ToInt32(layerGrafNombre);
            }
            catch
            {
            }
            return nroRango;
        }

        private float GetXPDFCanvas(double x, double xCentroidBase, double escala, Plantilla plantilla)
        {
            return it.Utilities.MillimetersToPoints((float)(((x - xCentroidBase) * escala) * 1000 + plantilla.X_Centro));
        }

        private float GetYPDFCanvas(double y, double yCentroidBase, double escala, Plantilla plantilla)
        {
            return it.Utilities.MillimetersToPoints((float)(((y - yCentroidBase) * escala * 1000) + plantilla.Y_Centro));
        }

        private double GetXBuffer(double xc, double xCentroidBase, double factorEscala, Plantilla plantilla)
        {
            return (((xc - plantilla.X_Centro) / 1000) / factorEscala) + xCentroidBase;
        }

        private double GetYBuffer(double yc, double yCentroidBase, double factorEscala, Plantilla plantilla)
        {
            return (((yc - plantilla.Y_Centro) / 1000) / factorEscala) + yCentroidBase;
        }

        /// <summary>
        /// Rota una imagen
        /// </summary>
        /// <param name="image">Imagen a rotar</param>
        /// <param name="angle">angulo de rotacion en grados</param>
        /// <returns></returns>
        public PointF Rotate(float xToRotate, float yToRotate, float xOrigen, float yOrigen, double angulo, bool sentidoHorario)
        {
            PointD punto = RotateD((double)xToRotate, (double)yToRotate, (double)xOrigen, (double)yOrigen, angulo, sentidoHorario);
            return new PointF((float)punto.X, (float)punto.Y);
        }

        public PointD RotateD(double xToRotate, double yToRotate, double xOrigen, double yOrigen, double angulo, bool sentidoHorario)
        {
            double xRotated;
            double yRotated;
            if (sentidoHorario)
            {
                xRotated = -Math.Sin(angulo) * (yToRotate - yOrigen) + Math.Cos(angulo) * (xToRotate - xOrigen) + xOrigen;
                yRotated = Math.Sin(angulo) * (xToRotate - xOrigen) + Math.Cos(angulo) * (yToRotate - yOrigen) + yOrigen;
            }
            else
            {
                xRotated = Math.Sin(angulo) * (yToRotate - yOrigen) + Math.Cos(angulo) * (xToRotate - xOrigen) + xOrigen;
                yRotated = -Math.Sin(angulo) * (xToRotate - xOrigen) + Math.Cos(angulo) * (yToRotate - yOrigen) + yOrigen;
            }
            return new PointD(xRotated, yRotated);
        }

        public PointF Rotate(PointF toRotate, PointF origen, double angulo, bool sentidoHorario)
        {
            return Rotate(toRotate.X, toRotate.Y, origen.X, origen.Y, angulo, sentidoHorario);
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
                //trunco a dos decimale sin redondear
                double stepper = Math.Pow(10.0, (double)2);
                int temp = (int)(stepper * slope);
                slope = (double)(temp / stepper);

                i++;
            }
            if (longSidePoint1.Y > origin.Y && longSidePoint2.Y > origin.Y)
                i += 180;

            return i * Math.PI / 180;
        }

        private double GetAnguloRotacion(DbGeometry geometryLayerGraf, int anguloTolerancia, double distanciaTolerancia, ref PointF ptoMedio, ref List<Lado> lados, ref Lado ladoMayor)
        {
            double angulo = 0;
            //Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
            try
            {
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
                    int elementCount = (geometryLayerGraf.ElementCount != null ? (int)geometryLayerGraf.ElementCount : 1);
                    for (int iElem = 1; iElem <= elementCount; iElem++)
                    {
                        List<Lado> lstLadosElem = new List<Lado>();
                        DbGeometry geometryLayerGrafElem = geometryLayerGraf;
                        if (geometryLayerGraf.ExteriorRing != null)
                        {
                            geometryLayerGrafElem = geometryLayerGraf.ExteriorRing;
                        }
                        if (geometryLayerGraf.ElementCount != null)
                        {
                            geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem);
                            if (geometryLayerGraf.ElementAt(iElem).ExteriorRing != null)
                            {
                                geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem).ExteriorRing;
                            }
                        }
                        int cantCoords = (int)geometryLayerGrafElem.PointCount;
                        List<Segmento> Segmentos = new List<Segmento>();
                        List<Point> lstPoint = new List<Point>();
                        for (int i = 1; i <= cantCoords; i++)
                        {
                            x = (double)geometryLayerGrafElem.PointAt(i).XCoordinate;
                            y = (double)geometryLayerGrafElem.PointAt(i).YCoordinate;
                            if (i > 1)
                            {
                                x1 = x2;
                                y1 = y2;
                            }
                            else
                            {
                                x1 = x;
                                y1 = y;
                            }
                            x2 = x;
                            y2 = y;

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
                                angulo = angulo * 180 / Math.PI;
                                Segmento segmento = new Segmento(new PointD(x1, y1), new PointD(x2, y2), dist, angulo);
                                Segmentos.Add(segmento);
                            }
                        }
                        Lado lado = new Lado() { Segmentos = new List<Segmento>() };
                        double ladoDist = 0;
                        double ladoAng = 0;
                        if (Segmentos.Count > 0)
                        {
                            ladoDist = Segmentos[0].Distancia;
                            ladoAng = Segmentos[0].Angulo;

                            lado.Segmentos.Add(Segmentos[0]);
                        }
                        for (int i = 0; i < Segmentos.Count; i++)
                        {
                            if (i > 0)
                            {
                                //Cambio de lado. Esta determinado por la diferencia de angulo >= 10 y distancia >= 5m
                                if (Math.Abs(Segmentos[i - 1].Angulo - Segmentos[i].Angulo) < anguloTolerancia || Segmentos[i].Distancia < distanciaTolerancia)
                                {
                                    ladoDist += Segmentos[i].Distancia;
                                    ladoAng = Segmentos[i].Angulo;
                                    lado.Segmentos.Add(Segmentos[i]);
                                }
                                else
                                {
                                    lado.Distancia = ladoDist;
                                    Segmento segmentoMayorLado = new Segmento();
                                    double distSegmentoMayorLado = -9999;
                                    foreach (var segmento in lado.Segmentos)
                                    {
                                        if (segmento.Distancia > distSegmentoMayorLado)
                                        {
                                            distSegmentoMayorLado = segmento.Distancia;
                                            segmentoMayorLado = segmento;
                                        }
                                    }
                                    lado.Angulo = segmentoMayorLado.Angulo;
                                    lstLadosElem.Add(lado);
                                    ladoDist = 0;
                                    lado = new Lado() { Segmentos = new List<Segmento>(new[] { Segmentos[i] }) };
                                    ladoAng = Segmentos[i].Angulo;
                                    ladoDist += Segmentos[i].Distancia;
                                }
                            }
                        }
                        if (Segmentos.Count > 0)
                        {
                            lado.Distancia = ladoDist;
                            Segmento segmentoMayorLado = new Segmento();
                            double distSegmentoMayorLado = -9999;
                            foreach (var segmento in lado.Segmentos)
                            {
                                if (segmento.Distancia > distSegmentoMayorLado)
                                {
                                    distSegmentoMayorLado = segmento.Distancia;
                                    segmentoMayorLado = segmento;
                                }
                            }
                            lado.Angulo = segmentoMayorLado.Angulo;
                            lstLadosElem.Add(lado);
                        }
                        //Verificar si el primer lado y el ultimo pertenecen al mismo lado
                        if (lstLadosElem.Count > 0 && lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count > 0)
                        {
                            int cantSegmentosUltLado = lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count - 1;
                            if (Math.Abs(lstLadosElem[0].Segmentos[0].Angulo - lstLadosElem[lstLadosElem.Count - 1].Segmentos[cantSegmentosUltLado].Angulo) < anguloTolerancia)
                            {
                                List<Lado> ladosTemp = new List<Lado>();
                                Lado ladoPrimero = new Lado() { Segmentos = new List<Segmento>() };
                                foreach (var segmento in lstLadosElem[lstLadosElem.Count - 1].Segmentos)
                                {
                                    ladoPrimero.Segmentos.Add(segmento);
                                }
                                foreach (var segmento in lstLadosElem[0].Segmentos)
                                {
                                    ladoPrimero.Segmentos.Add(segmento);
                                }
                                ladoPrimero.Distancia = ladoPrimero.Segmentos.Sum(s => s.Distancia);
                                ladoPrimero.Angulo = lstLadosElem[0].Segmentos[0].Angulo;
                                ladosTemp.Add(ladoPrimero);
                                //Recorro hasta el anteultimo lado
                                for (int iLado = 1; iLado < lstLadosElem.Count - 1; iLado++)
                                {
                                    ladosTemp.Add(lstLadosElem[iLado]);
                                }
                                lstLadosElem = ladosTemp;
                            }
                            lados.AddRange(lstLadosElem);
                        }

                        //InteriorRing
                        if (geometryLayerGraf.ElementAt(iElem).InteriorRingCount > 0)
                        {
                            for (int iRing = 1; iRing <= geometryLayerGraf.ElementAt(iElem).InteriorRingCount; iRing++)
                            {
                                DbGeometry geomInteriorRing = geometryLayerGraf.ElementAt(iElem).InteriorRingAt(iRing);
                                int cantCoordsIntRing = (int)geomInteriorRing.PointCount;
                                List<PointF> lstPointPDFRing = new List<PointF>();
                                lstLadosElem = new List<Lado>();
                                Segmentos = new List<Segmento>();
                                for (int i = 1; i <= cantCoordsIntRing; i++)
                                {
                                    x = (double)geomInteriorRing.PointAt(i).XCoordinate;
                                    y = (double)geomInteriorRing.PointAt(i).YCoordinate;
                                    if (i > 1)
                                    {
                                        x1 = x2;
                                        y1 = y2;
                                    }
                                    else
                                    {
                                        x1 = x;
                                        y1 = y;
                                    }
                                    x2 = x;
                                    y2 = y;

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
                                        angulo = angulo * 180 / Math.PI;
                                        Segmento segmento = new Segmento(new PointD(x1, y1), new PointD(x2, y2), dist, angulo);
                                        Segmentos.Add(segmento);
                                    }
                                }
                                lado = new Lado() { Segmentos = new List<Segmento>() };
                                ladoDist = 0;
                                ladoAng = 0;
                                if (Segmentos.Count > 0)
                                {
                                    ladoDist = Segmentos[0].Distancia;
                                    ladoAng = Segmentos[0].Angulo;

                                    lado.Segmentos.Add(Segmentos[0]);
                                }
                                for (int i = 0; i < Segmentos.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        //Cambio de lado. Esta determinado por la diferencia de angulo >= 10 y distancia >= 5m
                                        if (Math.Abs(Math.Abs(Segmentos[i - 1].Angulo) - Math.Abs(Segmentos[i].Angulo)) < anguloTolerancia || Segmentos[i].Distancia < distanciaTolerancia)
                                        {
                                            ladoDist += Segmentos[i].Distancia;
                                            ladoAng = Segmentos[i].Angulo;
                                            lado.Segmentos.Add(Segmentos[i]);
                                        }
                                        else
                                        {
                                            lado.Distancia = ladoDist;
                                            Segmento segmentoMayorLado = new Segmento();
                                            double distSegmentoMayorLado = -9999;
                                            foreach (var segmento in lado.Segmentos)
                                            {
                                                if (segmento.Distancia > distSegmentoMayorLado)
                                                {
                                                    distSegmentoMayorLado = segmento.Distancia;
                                                    segmentoMayorLado = segmento;
                                                }
                                            }
                                            lado.Angulo = segmentoMayorLado.Angulo;
                                            lstLadosElem.Add(lado);
                                            ladoDist = 0;
                                            lado = new Lado() { Segmentos = new List<Segmento>(new[] { Segmentos[i] }) };
                                            ladoAng = Segmentos[i].Angulo;
                                            ladoDist += Segmentos[i].Distancia;
                                        }
                                    }
                                }
                                if (Segmentos.Count > 0)
                                {
                                    lado.Distancia = ladoDist;
                                    Segmento segmentoMayorLado = new Segmento();
                                    double distSegmentoMayorLado = -9999;
                                    foreach (var segmento in lado.Segmentos)
                                    {
                                        if (segmento.Distancia > distSegmentoMayorLado)
                                        {
                                            distSegmentoMayorLado = segmento.Distancia;
                                            segmentoMayorLado = segmento;
                                        }
                                    }
                                    lado.Angulo = segmentoMayorLado.Angulo;
                                    lstLadosElem.Add(lado);
                                }
                                //Verificar si el primer lado y el ultimo pertenecen al mismo lado
                                if (lstLadosElem.Count > 0 && lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count > 0)
                                {
                                    int cantSegmentosUltLado = lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count - 1;
                                    if (Math.Abs(Math.Abs(lstLadosElem[0].Segmentos[0].Angulo) - Math.Abs(lstLadosElem[lstLadosElem.Count - 1].Segmentos[cantSegmentosUltLado].Angulo)) < anguloTolerancia)
                                    {
                                        List<Lado> ladosTemp = new List<Lado>();
                                        Lado ladoPrimero = new Lado();
                                        ladoPrimero.Segmentos = new List<Segmento>();
                                        foreach (var segmento in lstLadosElem[lstLadosElem.Count - 1].Segmentos)
                                        {
                                            ladoPrimero.Segmentos.Add(segmento);
                                        }
                                        foreach (var segmento in lstLadosElem[0].Segmentos)
                                        {
                                            ladoPrimero.Segmentos.Add(segmento);
                                        }
                                        ladoPrimero.Distancia = ladoPrimero.Segmentos.Sum(s => s.Distancia);
                                        ladoPrimero.Angulo = lstLadosElem[0].Segmentos[0].Angulo;
                                        ladosTemp.Add(ladoPrimero);
                                        //Recorro hasta el anteultimo lado
                                        for (int iLado = 1; iLado < lstLadosElem.Count - 1; iLado++)
                                        {
                                            ladosTemp.Add(lstLadosElem[iLado]);
                                        }
                                        lstLadosElem = ladosTemp;
                                    }
                                    lados.AddRange(lstLadosElem);
                                }
                            }

                        }
                        //Fin Interior Ring
                    }
                }

                ladoMayor.Distancia = -1;
                foreach (var lado in lados)
                {
                    if (lado.Distancia > ladoMayor.Distancia)
                    {
                        ladoMayor = lado;
                    }
                }

                //Segmento Mayor del Lado Mayor
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
                angulo = segmentoMayor.Angulo;
                ptoMedio.X = (float)(ladoMayor.Segmentos[0].P1.X + ladoMayor.Segmentos[0].P2.X) / 2;
                ptoMedio.Y = (float)(ladoMayor.Segmentos[0].P1.Y + ladoMayor.Segmentos[0].P2.Y) / 2;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return angulo;
        }

        //Devuelve la escala de los puntos rotados y el nuevo buffer rotado
        private double GetEscala(DbGeometry geometry, double xCentroidBase, double yCentroidBase, Plantilla plantilla, double anguloRotacion, bool sentidoHorario, ref double xMinBuff, ref double yMinBuff, ref double xMaxBuff, ref double yMaxBuff)
        {
            double escala = 0;
            try
            {
                xMinBuff = 9999999;
                yMinBuff = 9999999;
                xMaxBuff = 0;
                yMaxBuff = 0;
                double factorEscala = 0;
                double x = 0, y = 0;

                string wkt = geometry.AsText();
                if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                {
                    int cantCoords = (int)geometry.PointCount;
                    List<PointD> lstPointPDF = new List<PointD>();
                    List<string> lstCoordsRotado = new List<string>();
                    for (int i = 1; i <= cantCoords; i++)
                    {
                        x = (double)geometry.PointAt(i).XCoordinate;
                        y = (double)geometry.PointAt(i).YCoordinate;
                        if (plantilla.OptimizarTamanioHoja)
                        {
                            PointD ptRotado = RotateD(x, y, xCentroidBase, yCentroidBase, anguloRotacion, sentidoHorario);
                            lstPointPDF.Add(new PointD(ptRotado.X, ptRotado.Y));
                            lstCoordsRotado.Add(x.ToString().Replace(",", ".") + ", " + y.ToString().Replace(",", "."));
                            if (ptRotado.X <= xMinBuff)
                            {
                                xMinBuff = ptRotado.X;
                            }
                            if (ptRotado.X >= xMaxBuff)
                            {
                                xMaxBuff = ptRotado.X;
                            }
                            if (ptRotado.Y <= yMinBuff)
                            {
                                yMinBuff = ptRotado.Y;
                            }
                            if (ptRotado.Y >= yMaxBuff)
                            {
                                yMaxBuff = ptRotado.Y;
                            }
                        }
                    }
                }
                if (((plantilla.X_Util / 1000) * (yMaxBuff - yMinBuff)) > ((plantilla.Y_Util / 1000) * (xMaxBuff - xMinBuff)))
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

            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return escala;
        }

        public Plantilla GetPlantillaById(int idPlantilla)
        {
            return _plantillaRepository.GetPlantillaById(idPlantilla);
        }

        public PlantillaFondo GetPlantillaFondoById(int idPlantillaFondo)
        {
            return _plantillaFondoRepository.GetPlantillaFondoById(idPlantillaFondo);
        }

        public ImagenSatelital GetImagenSatelitalById(int idImagenSatelital)
        {
            return _imagenSatelitalRepository.GetImagenSatelitalById(idImagenSatelital);
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

        private LayerGraf GetLayerGrafByComponentePrincipal(Layer layerBase, Componente componente, string guid)
        {
            return _layerGrafRepository.GetLayerGrafByComponentePrincipal(layerBase, componente, guid);
        }

        public LayerGraf[] GetLayerGrafByMapaTematico(string guid)
        {

            return _layerGrafRepository.GetLayerGrafByMapaTematico(guid);
        }

        public string GetLayerGrafTextById(Componente componente, string id, long idAtributo)
        {

            return _layerGrafRepository.GetLayerGrafTextById(componente, id, idAtributo);
        }

        public string GetLayerGrafDistritoById(Componente componente, string id)
        {

            return _layerGrafRepository.GetLayerGrafDistritoById(componente, id);
        }

        public string GetLayerGrafDistritoByCoords(double x, double y)
        {
            return _layerGrafRepository.GetLayerGrafDistritoByCoords(x, y);
        }

        public string GetLayerGrafKMMallaByCoords(int pIdMalla, Componente compCañeria, List<string> lstTipo)
        {

            return _layerGrafRepository.GetLayerGrafKMMallaByCoords(pIdMalla, compCañeria, lstTipo);
        }

        public string GetLayerGrafRegionByCoords(double x, double y)
        {

            return _layerGrafRepository.GetLayerGrafRegionByCoords(x, y);
        }

        public LayerGraf[] GetLayerGrafByIds(Layer layer, Componente componente, string ids, List<string> lstCoordsGeometry)
        {

            return _layerGrafRepository.GetLayerGrafByIds(layer, componente, ids, lstCoordsGeometry);
        }

        public LayerGraf[] GetLayerGrafByCoords(Layer layer, Componente componente, List<string> lstCoordsGeometry, string anio)
        {
            return _layerGrafRepository.GetLayerGrafByCoordsAndIds(layer, componente, lstCoordsGeometry, anio, null);
        }

        #region comento porque no se usa, se borra mas adelante
        //public LayerGraf[] GetLayerGrafByCoordsAndIds(Layer layer, Componente componente, List<string> lstCoordsGeometry, string anio, List<long> ids = null)
        //{
        //    return _layerGrafRepository.GetLayerGrafByCoordsAndIds(layer, componente, lstCoordsGeometry, anio, ids);
        //} 
        #endregion

        public LayerGraf[] GetLayerGrafByObjetoBaseIntersect(Layer layer, Componente componente, Componente componenteBase, string idObjetoBase, string anio, bool esInformeAnual)
        {
            return _layerGrafRepository.GetLayerGrafByObjetoBaseIntersect(layer, componente, componenteBase, idObjetoBase, anio, esInformeAnual);
        }

        public LayerGraf[] GetLayerGrafByObjetoBase(Layer layer, Componente componente, Componente componenteBase, List<string> lstCoordsGeometry, string idObjetoBase, string anio, bool esInformeAnual)
        {
            return _layerGrafRepository.GetLayerGrafByObjetoBase(layer, componente, componenteBase, lstCoordsGeometry, idObjetoBase, anio, esInformeAnual);
        }

        public List<ParametrosGenerales> GetParametrosGenerales()
        {
            return _plantillaRepository.GetParametrosGenerales();
        }

        private LayerGraf[] GetLayersGraf(LayerGraf[] aLayerGraf)
        {
            IDictionary<string, DbGeometry> arcosByStreet = GetDictionaryGeometry(aLayerGraf);
            List<DistanciaArco> result = GetResultsOrdered(aLayerGraf, arcosByStreet);
            List<long> idsToRemove = new List<long>();
            string previousName = string.Empty;
            int intervalo = 0;
            int pos = 0;
            int maxIntervalo = Int32.Parse(GetParametrosGenerales().Where(pg => pg.Descripcion == "EspacioNombreCallePloteos").FirstOrDefault().Valor);
            foreach (DistanciaArco arc in result)
            {
                if (previousName != arc.Nombre)
                {
                    pos = (int)(result.Count(r => r.Nombre == arc.Nombre) / maxIntervalo);
                    pos = pos > maxIntervalo ? maxIntervalo : pos;
                    previousName = arc.Nombre;
                    intervalo = 0;
                }
                if (intervalo != pos && previousName.Equals(arc.Nombre))
                {
                    idsToRemove.Add(arc.Id);
                }

                intervalo++;
                intervalo = intervalo > maxIntervalo ? 0 : intervalo;
            }
            return aLayerGraf.Where(l => !idsToRemove.Contains(l.FeatId)).OrderBy(l => l.Nombre).ToArray();
        }

        private List<DistanciaArco> GetResultsOrdered(LayerGraf[] aLayerGraf, IDictionary<string, DbGeometry> arcosByStreet)
        {
            List<DistanciaArco> result = new List<DistanciaArco>();
            foreach (var graf in aLayerGraf)
            {
                DbGeometry tot = arcosByStreet[graf.Nombre];
                var distInic = tot.Distance(graf.Geom);
                result.Add(new DistanciaArco(graf.Nombre, distInic, graf.FeatId, graf.Geom.Length));
            }

            return result.OrderBy(r => r.Nombre).ThenBy(r => r.Distancia).ToList();
        }

        private IDictionary<string, DbGeometry> GetDictionaryGeometry(LayerGraf[] aLayerGraf)
        {
            IDictionary<string, DbGeometry> arcosByStreet = new Dictionary<string, DbGeometry>();
            foreach (var graf in aLayerGraf)
            {
                if (arcosByStreet.ContainsKey(graf.Nombre))
                {
                    DbGeometry geom = arcosByStreet[graf.Nombre];
                    geom = geom.Union(graf.Geom);
                }
                else
                {
                    DbGeometry geom = graf.Geom;
                    arcosByStreet.Add(graf.Nombre, geom);
                }

            }
            return arcosByStreet;
        }

        private List<PointD> GetlstCoordsGeometry(DbGeometry geometryLayerGraf, string wkt)
        {
            List<PointD> lstCoordsGeometry = new List<PointD>();
            if (geometryLayerGraf.IsValid)
            {
                int cantCoords = (int)geometryLayerGraf.PointCount;
                for (int j = 1; j <= cantCoords; j++)
                {
                    PointD pt = new PointD();
                    pt.X = (double)geometryLayerGraf.PointAt(j).XCoordinate;
                    pt.Y = (double)geometryLayerGraf.PointAt(j).YCoordinate;
                    lstCoordsGeometry.Add(pt);
                }
            }
            else
            {
                int iInicio = wkt.IndexOf('(') + 1;
                int iFin = wkt.IndexOf(')');
                string coords = wkt.Substring(iInicio, (iFin - iInicio));
                List<string> lstCoords = coords.Split(',').ToList();
                foreach (var coord in lstCoords)
                {
                    string[] aCoord = coord.Trim().Split(' ');
                    PointD pt = new PointD();
                    pt.X = Convert.ToDouble(aCoord[0]);
                    pt.Y = Convert.ToDouble(aCoord[1]);
                    lstCoordsGeometry.Add(pt);
                }

            }
            return lstCoordsGeometry;
        }

        public Image GetImageFromWMS(string url, string layers, double[] coords, int width, int height, string srs, string format)
        {
            try
            {
                double xMin = coords[0], yMin = coords[1], xMax = coords[2], yMax = coords[3];
                string bbox = string.Join(",", coords.Select(c => c.ToString(NumberFormat)));

                double distanciaW = GetDistance(yMin, xMin, yMin, xMax) * 1000;
                double distanciaH = GetDistance(yMin, xMin, yMax, xMin) * 1000;
                double ratio = distanciaW / distanciaH;
                height = Convert.ToInt32(Math.Floor(width / ratio));

                var uri = new UriBuilder(url)
                {
                    Query = $"service=WMS&version=1.1.1&request=GetMap&layers={layers}&styles=&bbox={bbox}&width={width.ToString(NumberFormat)}&height={height.ToString(NumberFormat)}&srs={srs}&format={format}"
                };
                using (var response = System.Net.WebRequest.Create(uri.Uri).GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var image = Image.FromStream(stream);
                    response.Close();
                    return image;
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("GetImageFromWMS", ex);
                return null;
            }
        }

        public const double EarthRadius = 6371;//Meter dentro de la funcion

        public static double GetDistance(double lat_1, double lon_1, double lat_2, double lon_2)
        {
            double Lat = (lat_2 - lat_1) * (Math.PI / 180);
            double Lon = (lon_2 - lon_1) * (Math.PI / 180);
            double a = Math.Sin(Lat / 2) * Math.Sin(Lat / 2) + Math.Cos(lat_1 * (Math.PI / 180)) * Math.Cos(lat_2 * (Math.PI / 180)) * Math.Sin(Lon / 2) * Math.Sin(Lon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadius * c;
        }

        /// <summary>
        /// Transfoma coordenadas de 22195 a 8307
        /// </summary>
        /// <param name="x1">Coordenada x1</param>
        /// <param name="y1">Coordenada y1</param>
        /// <param name="x2">Coordenada x2</param>
        /// <param name="y2">Coordenada y2</param>
        /// <returns>Devuelve lista de coordenadas transformadas</returns>
        public double[] TransformCoords(double x1, double y1, double x2, double y2) => _layerGrafRepository.TransformCoords(x1, y1, x2, y2, SRID.App, SRID.DB);
        public double[] TransformCoords(double x1, double y1, double x2, double y2, SRID origen, SRID destino)
        {
            return _layerGrafRepository.TransformCoords(x1, y1, x2, y2, origen, destino);
        }

        /// <summary>
        /// Rota una imagen
        /// </summary>
        /// <param name="image">Imagen a rotar</param>
        /// <param name="angle">angulo de rotacion en grados</param>
        /// <returns></returns>
        public static Bitmap RotateImage2(Image image, float angle)
        {
            try
            {
                if (image == null)
                {
                    return null;
                }
                const double pi2 = Math.PI / 2.0;
                double oldWidth = (double)image.Width;
                double oldHeight = (double)image.Height;
                // Convert degrees to radians
                double theta = ((double)angle) * Math.PI / 180.0;
                double locked_theta = theta;

                while (locked_theta < 0.0)
                    locked_theta += 2 * Math.PI;

                double newWidth, newHeight;
                int nWidth, nHeight; // The newWidth/newHeight expressed as ints
                double adjacentTop, oppositeTop;
                double adjacentBottom, oppositeBottom;
                if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                    (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
                {
                    adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                    oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                    adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                    oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                }
                else
                {
                    adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                    oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                    adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                    oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                }
                newWidth = adjacentTop + oppositeBottom;
                newHeight = adjacentBottom + oppositeTop;
                nWidth = (int)Math.Ceiling(newWidth);
                nHeight = (int)Math.Ceiling(newHeight);

                Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);
                using (Graphics g = Graphics.FromImage(rotatedBmp))
                {
                    Point[] points;
                    if (locked_theta >= 0.0 && locked_theta < pi2)
                    {
                        points = new Point[] {
                                           new Point( (int) oppositeBottom, 0 ),
                                           new Point( nWidth, (int) oppositeTop ),
                                           new Point( 0, (int) adjacentBottom )
                                       };
                    }
                    else if (locked_theta >= pi2 && locked_theta < Math.PI)
                    {
                        points = new Point[] {
                                           new Point( nWidth, (int) oppositeTop ),
                                           new Point( (int) adjacentTop, nHeight ),
                                           new Point( (int) oppositeBottom, 0 )
                                       };
                    }
                    else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                    {
                        points = new Point[] {
                                           new Point( (int) adjacentTop, nHeight ),
                                           new Point( 0, (int) adjacentBottom ),
                                           new Point( nWidth, (int) oppositeTop )
                                       };
                    }
                    else
                    {
                        points = new Point[] {
                                           new Point( 0, (int) adjacentBottom ),
                                           new Point( (int) oppositeBottom, 0 ),
                                           new Point( (int) adjacentTop, nHeight )
                                       };
                    }
                    g.DrawImage(image, points);
                }
                return rotatedBmp;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }

        /// <summary>  
        /// method for changing the opacity of an image  
        /// </summary>  
        /// <param name="image">image to set opacity on</param>  
        /// <param name="opacity">percentage of opacity</param>  
        /// <returns></returns>  
        public Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);
                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    //create a color matrix object  
                    System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix();
                    //set the opacity  
                    colorMatrix.Matrix33 = opacity;
                    //create image attributes  
                    System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(colorMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetEstampilla(long idParcela)
        {
            int idPlantilla = Convert.ToInt32(_parametroRepository.GetParametro("ID_PLANTILLA_ESTAMPILLA").Valor);
            int idComponenteParcela = Convert.ToInt32(_parametroRepository.GetParametro("ID_COMPONENTE_PARCELA").Valor);
            int idPlantillaFondo = _plantillaFondoRepository.GetPlantillaFondoByIdPlantilla(idPlantilla).IdPlantillaFondo;
            byte[] pdf = this.GetPlantilla(idPlantilla, idParcela.ToString(), idComponenteParcela, idPlantillaFondo, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, null, 0, 0, true, null, true, null);
            if (pdf != null)
            {
                Ghostscript.Ghostscript converter = new Ghostscript.Ghostscript();
                byte[] imagen = converter.ConvertPdfToImage(pdf, 1, ImageFormat.Png, false, 0).FirstOrDefault();
                return $"data:image/png;base64,{Convert.ToBase64String(imagen)}";
            }
            else
            {
                return null;
            }
        }
    }
}