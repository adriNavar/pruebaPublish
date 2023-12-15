using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using iTextSharp.text.pdf;
using it = iTextSharp.text;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Repositories;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Text.RegularExpressions;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.Reflection;

namespace MPFuncionesEspeciales
{
    public class ModPlotFuncionesEspeciales
    {
        private readonly IParcelaPlotRepository _parcelaPlotRepository;
        private readonly ILayerGrafRepository _layerGrafRepository;
        private readonly ICuadraPlotRepository _cuadraPlotRepository;
        private readonly IPlantillaRepository _plantillaRepository;
        private readonly IManzanaPlotRepository _manzanaPlotRepository;
        private readonly ICallePlotRepository _callePlotRepository;
        private readonly IExpansionPlotRepository _expansionPlotRepository;
        private readonly IParametroRepository _parametroRepository;
        private readonly ITipoPlanoRepository _tipoPlanoRepository;
        private readonly IPartidoRepository _partidoRepository;
        private readonly ICensoRepository _censoRepository;
        private readonly IAtributoRepository _atributoRepository;
        private readonly IComponenteRepository _componenteRepository;
        public ModPlotFuncionesEspeciales(IParcelaPlotRepository parcelaPlotRepository, ICuadraPlotRepository cuadraPlotRepository, ILayerGrafRepository layerGrafRepository, IPlantillaRepository plantillaRepository, IManzanaPlotRepository manzanaPlotRepository, ICallePlotRepository callePlotRepository, IExpansionPlotRepository expansionPlotRepository, IParametroRepository parametroRepository, ITipoPlanoRepository tipoPlanoRepository, IPartidoRepository partidoRepository, ICensoRepository censoRepository, IAtributoRepository atributoRepository, IComponenteRepository componenteRepository)
        {
            _parcelaPlotRepository = parcelaPlotRepository;
            _layerGrafRepository = layerGrafRepository;
            _cuadraPlotRepository = cuadraPlotRepository;
            _plantillaRepository = plantillaRepository;
            _manzanaPlotRepository = manzanaPlotRepository;
            _callePlotRepository = callePlotRepository;
            _expansionPlotRepository = expansionPlotRepository;
            _parametroRepository = parametroRepository;
            _tipoPlanoRepository = tipoPlanoRepository;
            _partidoRepository = partidoRepository;
            _censoRepository = censoRepository;
            _atributoRepository = atributoRepository;
            _componenteRepository = componenteRepository;
        }

        #region STRUCTS
        //public struct PointD
        //{
        //    public double X;
        //    public double Y;
        //    public PointD(double x, double y)
        //    {
        //        this.X = x;
        //        this.Y = y;
        //    }
        //}
        //public struct Segmento
        //{
        //    public PointD P1;
        //    public PointD P2;
        //    public double Distancia;
        //    public double Angulo;
        //    public Segmento(PointD p1, PointD p2, double distancia, double angulo)
        //    {
        //        this.P1 = p1;
        //        this.P2 = p2;
        //        this.Distancia = distancia;
        //        this.Angulo = angulo;
        //    }
        //}
        //public struct Lado
        //{
        //    public List<Segmento> Segmentos;
        //    public double Distancia;
        //    public double Angulo;
        //    public long IdCuadra;
        //    public Lado(List<Segmento> Segmentos, double distancia, double angulo, long idCuadra)
        //    {
        //        this.Segmentos = Segmentos;
        //        this.Distancia = distancia;
        //        this.Angulo = angulo;
        //        this.IdCuadra = idCuadra;
        //    }
        //}
        #endregion

        public bool EjecutarFuncionEspecial(int idFuncionEspecial, Graphics graphics, Plantilla plantilla, string idsObjetoSecundario, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff)
        {
            bool ok = false;
            switch (idFuncionEspecial)
            {
                case 1:
                    ok = FuncionEspecial1(graphics, plantilla, idsObjetoSecundario, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
                    break;
                case 2:
                    break;
                default:
                    break;
            }
            return ok;
        }
        public bool EjecutarFuncionEspecial(int idFuncionEspecial/*, it.Document pdfDoc*/, PdfContentByte pdfContentByte, Plantilla plantilla/*, int idPlantillaFondo*/, Componente componenteBase, string idObjetoGraf, string idsObjetoSecundario, List<string> lstCoordenadas, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff, double xCentroidBase, double yCentroidBase, double factorEscala, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro/*, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, List<Layer> lstLayersReferencia, string anio, List<string> lstCoordsBuffImpresion, LayerGraf[] lstLayerGrafRelaciones = null, string lstIdsAtributos = null, string idsSeleccionados = null, int idComponenteSelec = 0*/)
        {
            bool ok = false;
            ParcelaPlot[] aParcelaPlot = null;
            ManzanaPlot manzanaPlot = null;
            //bool refEnPagina2 = false;
            //int cantPaginas = 1;
            switch (idFuncionEspecial)
            {
                case 1:
                    ok = FuncionEspecial1(pdfContentByte, plantilla, idsObjetoSecundario, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
                    break;
                case 2:
                    ok = FuncionEspecialDibujarCotas(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot);
                    break;
                case 3:
                    ok = FuncionEspecialDibujarNroPuerta(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot);
                    break;
                case 4:
                    ok = FuncionEspecialDibujarNombreCalle(pdfContentByte, plantilla, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idObjetoGraf, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref manzanaPlot);
                    break;
                case 5:
                    ////plantilla.IdFuncionAdicional = 2;
                    ////plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    ////ok = FuncionEspecialDibujarCotas(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot);
                    //plantilla.IdFuncionAdicional = 3;
                    //plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    //ok = FuncionEspecialDibujarNroPuerta(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot);
                    //plantilla.IdFuncionAdicional = 4;
                    //plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    //ok = FuncionEspecialDibujarNombreCalle(pdfContentByte, plantilla, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idObjetoGraf, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref manzanaPlot);
                    //plantilla.IdFuncionAdicional = 6;
                    //plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    //ok = FuncionEspecialDibujarDatosParcelaReferencia(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idObjetoGraf, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot, ref manzanaPlot);
                    //plantilla.IdFuncionAdicional = 5;//Siempre termina con el IdFuncionAdicional por defecto por si se hacer un ploteo de multiples objetos.
                    break;
                case 6:
                    //ok = FuncionEspecialDibujarDatosParcelaReferencia(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idObjetoGraf, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot, ref manzanaPlot);
                    break;
                case 7:
                    ////AYSA20180425 se agrego que dibuje nro de puerta y calles
                    //plantilla.IdFuncionAdicional = 3;
                    //plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    //ok = FuncionEspecialDibujarNroPuerta(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot);
                    //plantilla.IdFuncionAdicional = 4;
                    //plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    //ok = FuncionEspecialDibujarNombreCalle(pdfContentByte, plantilla, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idObjetoGraf, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref manzanaPlot);
                    //plantilla.IdFuncionAdicional = 6;
                    //plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    //ok = FuncionEspecialDibujarDatosParcelaComercial(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idObjetoGraf, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref aParcelaPlot, ref manzanaPlot, grafico, leyenda, infoLeyenda);
                    //plantilla.IdFuncionAdicional = 7;//Siempre termina con el IdFuncionAdicional por defecto por si se hacer un ploteo de multiples objetos.
                    break;
                case 8:
                    ////InformeAnual Referencia Expansion Agua - EA
                    //ok = DibujarNombreCallesEnPlano(idFuncionEspecial, pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, componenteBase, idObjetoGraf, anio, anguloRotacion, sentidoHorario, anguloRotacionFiltro);
                    //refEnPagina2 = false;
                    //cantPaginas = GetCantidadPaginasInformeAnualEA(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio, ref refEnPagina2);
                    //if (!refEnPagina2)
                    //{
                    //    ok = FuncEspDibujarInformeAnualReferenciaEA(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio);
                    //    ok = FuncEspDibujarInformeAnualReferenciaTitulos(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, anio, componenteBase, idObjetoGraf);
                    //}
                    //else
                    //{
                    //    ok = FuncEspDibujarInformeAnualReferenciaEAPagina2(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio, cantPaginas);
                    //}
                    break;
                case 9:
                    ////InformeAnual Referencia Expansion Cloaca - EC
                    ////ok = FuncEspDibujarInformeAnualReferenciaEA(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio);
                    ////ok = DibujarNombreCallesEnPlano(idFuncionEspecial, pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, componenteBase, idObjetoGraf, anio, anguloRotacion, sentidoHorario, anguloRotacionFiltro);
                    //ok = DibujarNombreCallesEnPlano(idFuncionEspecial, pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, componenteBase, idObjetoGraf, anio, anguloRotacion, sentidoHorario, anguloRotacionFiltro);
                    //refEnPagina2 = false;
                    //cantPaginas = GetCantidadPaginasInformeAnualEA(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio, ref refEnPagina2);
                    //if (!refEnPagina2)
                    //{
                    //    ok = FuncEspDibujarInformeAnualReferenciaEA(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio);
                    //    ok = FuncEspDibujarInformeAnualReferenciaTitulos(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, anio, componenteBase, idObjetoGraf);
                    //}
                    //else
                    //{
                    //    ok = FuncEspDibujarInformeAnualReferenciaEAPagina2(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio, cantPaginas);
                    //}
                    break;
                case 10:
                    //InformeAnual Referencia Rehabilitacion Hidraulica y Estructural Cloaca RHEC
                    //ok = DibujarNombreCallesEnPlano(idFuncionEspecial, pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, componenteBase, idObjetoGraf, anio, anguloRotacion, sentidoHorario, anguloRotacionFiltro);
                    //ok = FuncEspDibujarInformeAnualReferenciaRHEC(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio);
                    //ok = FuncEspDibujarInformeAnualReferenciaTitulos(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, anio, componenteBase, idObjetoGraf);
                    break;
                case 11:
                    //InformeAnual Ref. Informe Anual Renovación y Resfuerzos Agua RRRA
                    //ok = DibujarNombreCallesEnPlano(idFuncionEspecial, pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, componenteBase, idObjetoGraf, anio, anguloRotacion, sentidoHorario, anguloRotacionFiltro);
                    //ok = FuncEspDibujarInformeAnualReferenciaRHEC(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, lstLayersReferencia, componenteBase, idObjetoGraf, lstCoordenadas, anio);
                    //ok = FuncEspDibujarInformeAnualReferenciaTitulos(idFuncionEspecial, pdfDoc, pdfContentByte, plantilla, anio, componenteBase, idObjetoGraf);
                    break;
                case 12:
                    ////Plancheta SAR
                    //plantilla = _plantillaRepository.GetFuncionAdicional(plantilla);
                    //LayerGraf[] parcelaDeSar = new LayerGraf[1];
                    //parcelaDeSar[0] = GetParcelaSAR(idObjetoGraf);

                    //ParcelaPlot parcelaPlotear = new ParcelaPlot();
                    //parcelaPlotear = GetParcelaPlotByIdObjGraf(idObjetoGraf);

                    //Layer layerParcela = lstLayersReferencia.Where(p => p.Nombre.Contains("Parcela")).FirstOrDefault();
                    //LayerGraf puerta = new LayerGraf();
                    //puerta = GetPuerta(idObjetoGraf);
                    //string idManzana = idObjetoGraf;
                    //if (plantilla.IdPlantillaCategoria == 4 || plantilla.IdPlantillaCategoria == 5)
                    //{
                    //    idManzana = GetIdManzanaByIdObjetoGraf(Convert.ToInt32(idObjetoGraf)).ToString();
                    //}
                    ////ok = FuncionEspecialDibujarNombreCalle(pdfContentByte, plantilla, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idManzana, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref manzanaPlot);//idObjetoGraf obtener idmanzana
                    //ok = ResaltarParcela(pdfDoc, pdfContentByte, parcelaDeSar, "Nombre", layerParcela, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario, parcelaPlotear);
                    //ok = ResaltarFrenteParcela(pdfContentByte, plantilla, componenteBase, idObjetoGraf, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref parcelaPlotear, layerParcela, factorEscala);
                    //ok = DibujarDireccionDeParcela(pdfContentByte, plantilla, idObjetoGraf, xCentroidBase, yCentroidBase, factorEscala, parcelaPlotear, sentidoHorario);

                    //ok = DibujarDatosCañeria(pdfContentByte, lstCoordsBuffImpresion, xCentroidBase, yCentroidBase, factorEscala, plantilla, sentidoHorario);
                    break;
                case 13:
                    //Resalta los objetos relacionados seleccionados de la linea de grandes conductos.
                    //Y muestra el nombre de las calles.
                    //ok = FuncionEspecialDibujarNombreCalle(pdfContentByte, plantilla, lstCoordenadas, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff, xCentroidBase, yCentroidBase, factorEscala, idObjetoGraf, anguloRotacion, sentidoHorario, anguloRotacionFiltro, ref manzanaPlot);
                    //PdfLayer ads = new PdfLayer("Resaltar", pdfContentByte.PdfWriter);
                    //ads.On = true;
                    //pdfContentByte.BeginLayer(ads);
                    //ok = ResaltarRelacionesLGC(pdfDoc, pdfContentByte, plantilla.IdPlantilla, lstLayerGrafRelaciones, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
                    //pdfContentByte.EndLayer();
                    //break;
                case 14:
                    //ok = DibujarAtributosObras(pdfContentByte, plantilla, lstIdsAtributos, componenteBase, idObjetoGraf);
                    break;
                case 19:
                    //PdfLayer layerP = new PdfLayer("Resaltar", pdfContentByte.PdfWriter);
                    //layerP.On = true;
                    //pdfContentByte.BeginLayer(layerP);
                    //ok = ResaltarObjSelecPloteosMasivos(pdfDoc, pdfContentByte, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario, idsSeleccionados, idComponenteSelec, lstCoordsBuffImpresion);
                    //pdfContentByte.EndLayer();
                    break;
                default:
                    break;
            }
            return ok;
        }



        private bool ResaltarObjSelecPloteosMasivos(it.Document pdfDoc, PdfContentByte pdfContentByte, double xCentroidBase, double yCentroidBase, double escala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario, string idsSeleccionados, int idComponenteSelec, List<string> lstCoordsBuffImpresion)
        {//Esta funcion es general, pero esta pensada para usarse con subcuencas y mallas.
            //Configuracion de layer personalizado para relacionesLGC
            Layer layerRelaciones = new Layer();
            layerRelaciones.Contorno = true;
            layerRelaciones.Relleno = false;
            layerRelaciones.PuntoRepresentacion = 1;
            layerRelaciones.Contorno = true;
            #region CargarLayerRelaciones

            FuncAdicParametro funcAdicParametro = null;


            int idMalla = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_COMPONENTE_A_MALLA")); //119
            int idSubcuenca = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_COMPONENTE_C_SUBCUE"));//118;

            if (idComponenteSelec == idMalla)
            {//Configuracion especifica para componente MALLA
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESA_RELA_CONTOR_COL_A_MALLA");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.ContornoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESA_RELA_RELLE_COL_A_MALLA");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.RellenoColor = funcAdicParametro.Valor;
                }
            }
            else if (idComponenteSelec == idSubcuenca)
            {//Configuracion especifica para componente SUBCUENCA
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESA_RELA_CONTOR_COL_C_SUBCUE");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.ContornoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESA_RELA_RELLE_COL_C_SUBCUE");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.RellenoColor = funcAdicParametro.Valor;
                }
            }
            else
            {//Configuracion para cualquier otro componente
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_CONTORNO_COL");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.ContornoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_RELLENO_COLO");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.RellenoColor = funcAdicParametro.Valor;
                }
            }


            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_RELLENO_TRAN");
            if (funcAdicParametro != null)
            {
                layerRelaciones.RellenoTransparencia = Convert.ToInt32(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_CONTORNO_GRO");
            if (funcAdicParametro != null)
            {
                layerRelaciones.ContornoGrosor = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_PUNTO_ANCHO");
            if (funcAdicParametro != null)
            {
                layerRelaciones.PuntoAncho = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_PUNTO_ALTO");
            if (funcAdicParametro != null)
            {
                layerRelaciones.PuntoAlto = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_PUNTO_PREDET");
            if (funcAdicParametro != null)
            {
                layerRelaciones.PuntoPredeterminado = Convert.ToInt32(funcAdicParametro.Valor);
            }
            #endregion

            LayerGraf[] alayergraft = null;

            Componente comp = _componenteRepository.GetComponenteById(idComponenteSelec);

            alayergraft = _layerGrafRepository.GetLayerGrafByIds(layerRelaciones, comp, idsSeleccionados, lstCoordsBuffImpresion);



            ResaltarLayerGraft(pdfDoc, pdfContentByte, alayergraft, layerRelaciones, xCentroidBase, yCentroidBase, escala, plantilla, anguloRotacion, sentidoHorario);

            return true;
        }



        /// <summary>
        /// Dibuja los atributos del componente principal. Se implementa en obras con los atributos seleccionadso en el formulario.
        /// </summary>
        /// <returns></returns>
        private bool DibujarAtributosObras(PdfContentByte pdfContentByte, Plantilla plantilla, string lstIdAtributos, Componente compBase, string idObjGraf)
        {
            bool res = false;

            float xMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
            float yMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMax);

            int fuenteAlineacion = 0;
            double xPdf = 1010;
            double yPdf = 800;

            string textoColor = "#000000";

            int alignment = fuenteAlineacion + it.Element.ALIGN_MIDDLE;

            int textoTamaño = 8;

            string textoFuente = "Arial";

            string textoEstilo = "0,0,0,0";

            float tamañoTabla = 450f;

            #region PARAMETROS
            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ALINEACION");
            if (funcAdicParametro != null)
            {
                fuenteAlineacion = Convert.ToInt32(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "X_INICIO_PLOTEO");
            if (funcAdicParametro != null)
            {
                xPdf = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "Y_INICIO_PLOTEO");
            if (funcAdicParametro != null)
            {
                yPdf = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TEXTO_COLOR");
            if (funcAdicParametro != null)
            {
                textoColor = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TEXTO_TAMANIO");
            if (funcAdicParametro != null)
            {
                textoTamaño = Convert.ToInt32(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TEXTO_FUENTE");
            if (funcAdicParametro != null)
            {
                textoFuente = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TEXTO_ESTILO");
            if (funcAdicParametro != null)
            {
                textoEstilo = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TAMAÑO_ANCHO_TABLA");
            if (funcAdicParametro != null)
            {
                tamañoTabla = (float)Convert.ToInt32(funcAdicParametro.Valor);
            }
            #endregion

            float pdfFontSize = it.Utilities.MillimetersToPoints(textoTamaño);

            Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);

            it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);

            PDFUtilities.RegisterBaseFont(textoFuente, pdfFontSize);
            string[] aFontStylePdf = textoEstilo.Split(',');
            int pdfFontStyle = aFontStylePdf.Select(x => Convert.ToInt32(x)).Sum();
            BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuente, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
            it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);


            string[] idsAtributos = lstIdAtributos.Split(',');


            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = tamañoTabla;

            float[] widths = { (float)(tamañoTabla * 0.25), (float)(tamañoTabla * 0.75) };
            table.SetWidths(widths);
            table.DefaultCell.FixedHeight = 10f;
            table.LockedWidth = true;

            foreach (var item in idsAtributos)
            {
                string texto = _layerGrafRepository.GetLayerGrafTextById(compBase, idObjGraf, (long)Convert.ToInt32(item));

                Atributo atributo = _atributoRepository.GetAtributoById(Convert.ToInt32(item));

                string nomAtri = atributo.Campo;

                table.AddCell(new it.Phrase(nomAtri, pdfFont));


                var textoFormateado = new it.Phrase(texto, pdfFont);

                int IdAtrDescripcioncObraAgua = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_ATRIBUTO_DESCRIPCION_OBRA_CLOACA"));
                int IdAtrDescripcioncObraCloaca = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_ATRIBUTO_DESCRIPCION_OBRA_AGUA"));


                int IdAtrNombreObraAgua = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_ATRIBUTO_NOMBRE_OBRA_AGUA"));
                int IdAtrNombreObraCloaca = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_ATRIBUTO_NOMBRE_OBRA_CLOACA"));

                int[] lstIdAtrDescripcioncObra = new int[2] { IdAtrDescripcioncObraAgua, IdAtrDescripcioncObraCloaca };

                int[] lstIdAtrNombrecObra = new int[2] { IdAtrNombreObraAgua, IdAtrNombreObraCloaca };

                if (lstIdAtrDescripcioncObra.Contains(Convert.ToInt32(item)))
                {//Se agrega maximo 3 renglones para que las descripcion no haga que la tabla se salga del ploteo

                    var cell = new PdfPCell(textoFormateado);

                    var textoHeight = textoFormateado.Chunks[0].GetWidthPoint();

                    if (textoHeight > widths[1])
                    {//El texto ocupas mas de 1 renglon
                        if (textoHeight > widths[1] * 2)
                        {//El texto ocupa mas de 2 renglones
                            cell.FixedHeight = 24f;
                        }
                        else
                        {
                            cell.FixedHeight = 16f;
                        }
                    }

                    table.AddCell(cell);
                }
                else if (lstIdAtrNombrecObra.Contains(Convert.ToInt32(item)))
                {
                    var cell = new PdfPCell(textoFormateado);

                    var textoHeight = textoFormateado.Chunks[0].GetWidthPoint();

                    if (textoHeight > widths[1])
                    {//El texto ocupas mas de 1 renglon
                        cell.FixedHeight = 16f;
                    }

                    table.AddCell(cell);
                }
                else
                {

                    table.AddCell(textoFormateado);
                }



                table.CompleteRow();
            }



            var x1 = it.Utilities.MillimetersToPoints((float)xPdf);
            var y1 = it.Utilities.MillimetersToPoints((float)yPdf);
            table.WriteSelectedRows(0, -1, (int)x1, (int)y1, pdfContentByte);


            return res;
        }


        /// <summary>
        /// Crea el layer desde la configuracion de la bd para resaltar las relaciones de lineas de grandes conductos.
        /// Cambia el color dependiendo el idPlantilla (agua o cloaca).
        /// </summary>
        /// <param name="pdfDoc"></param>
        /// <param name="pdfContentByte"></param>
        /// <param name="idPlantilla"></param>
        /// <param name="aLayerGraf"></param>
        /// <param name="xCentroidBase"></param>
        /// <param name="yCentroidBase"></param>
        /// <param name="escala"></param>
        /// <param name="plantilla"></param>
        /// <param name="anguloRotacion"></param>
        /// <param name="sentidoHorario"></param>
        /// <returns></returns>
        private bool ResaltarRelacionesLGC(it.Document pdfDoc, PdfContentByte pdfContentByte, int idPlantilla, LayerGraf[] aLayerGraf, double xCentroidBase, double yCentroidBase, double escala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario)
        {
            //Configuracion de layer personalizado para relacionesLGC



            /*
             * No es generico en base al grosor de layer. 
             * Tendria que ampliar la clase LayerGraft para que permita almacenar el valor del grosor, 
             * o el id del layer para obtener el valor del grosor.
             */
            Layer layerRelaciones = new Layer();
            layerRelaciones.Contorno = true;
            layerRelaciones.Relleno = false;
            layerRelaciones.PuntoRepresentacion = 1;
            layerRelaciones.Contorno = true;
            #region CargarLayerRelaciones


            int idPlantillaAgua = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_PLANTILLA_LGC_AGUA"));
            int idPlantillaCloaca = Convert.ToInt32(_parametroRepository.GetParametroByDescripcion("ID_PLANTILLA_LGC_CLOACA"));
            FuncAdicParametro funcAdicParametro = null;
            if (idPlantilla == idPlantillaAgua)
            {
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_RELAC_CONT_COL_AGUA");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.ContornoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_RELAC_RELL_COL_AGUA");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.RellenoColor = funcAdicParametro.Valor;
                }
            }
            else if (idPlantilla == idPlantillaCloaca)
            {
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_RELAC_CONT_COL_CLOACA");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.ContornoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_RELAC_RELL_COL_CLOACA");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.RellenoColor = funcAdicParametro.Valor;
                }
            }
            else
            {
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_CONTORNO_COL");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.ContornoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_RELLENO_COLO");
                if (funcAdicParametro != null)
                {
                    layerRelaciones.RellenoColor = funcAdicParametro.Valor;
                }
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_RELLENO_TRAN");
            if (funcAdicParametro != null)
            {
                layerRelaciones.RellenoTransparencia = Convert.ToInt32(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_CONTORNO_GRO");
            if (funcAdicParametro != null)
            {
                layerRelaciones.ContornoGrosor = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_PUNTO_ANCHO");
            if (funcAdicParametro != null)
            {
                layerRelaciones.PuntoAncho = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_PUNTO_ALTO");
            if (funcAdicParametro != null)
            {
                layerRelaciones.PuntoAlto = Convert.ToDouble(funcAdicParametro.Valor);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_RELACION_PUNTO_PREDET");
            if (funcAdicParametro != null)
            {
                layerRelaciones.PuntoPredeterminado = Convert.ToInt32(funcAdicParametro.Valor);
            }
            #endregion

            ResaltarLayerGraft(pdfDoc, pdfContentByte, aLayerGraf, layerRelaciones, xCentroidBase, yCentroidBase, escala, plantilla, anguloRotacion, sentidoHorario);

            return true;
        }


        /// <summary>
        /// Resalta el aLayerGraf que recibe por parametro, con la configuracion del layer.
        /// </summary>
        /// <param name="pdfDoc"></param>
        /// <param name="pdfContentByte"></param>
        /// <param name="aLayerGraf"></param>
        /// <param name="layer"></param>
        /// <param name="xCentroidBase"></param>
        /// <param name="yCentroidBase"></param>
        /// <param name="escala"></param>
        /// <param name="plantilla"></param>
        /// <param name="anguloRotacion"></param>
        /// <param name="sentidoHorario"></param>
        private void ResaltarLayerGraft(it.Document pdfDoc, PdfContentByte pdfContentByte, LayerGraf[] aLayerGraf, Layer layer, double xCentroidBase, double yCentroidBase, double escala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario)
        {//Funcion global
            //Si no se le pasa un layer, usa el predefinido de la bd.
            try
            {

                float pdfContornoGrosor = 5f;
                PdfPatternPainter pdfPatternPainter = null;
                string lineDash = null;

                string paContornoColor = "#960909";
                string paRellenoColor = "#960909";
                int? paTransparencia = 80;

                if (layer != null)
                {//Obtiene la config del layer.
                    #region configLayer
                    paContornoColor = layer.ContornoColor;
                    paRellenoColor = layer.RellenoColor;
                    paTransparencia = layer.RellenoTransparencia;
                    pdfContornoGrosor = (int)layer.ContornoGrosor;

                    #endregion
                }
                else
                {//Obtiene la Configuracion default.
                    #region Parametros
                    FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_CONTORNO_COLO");
                    if (funcAdicParametro != null)
                    {
                        paContornoColor = funcAdicParametro.Valor;
                    }
                    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_RELLENO_COLOR");
                    if (funcAdicParametro != null)
                    {
                        paRellenoColor = funcAdicParametro.Valor;
                    }
                    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_TRANSPARENCIA");
                    if (funcAdicParametro != null)
                    {
                        paTransparencia = Convert.ToInt32(funcAdicParametro.Valor);
                    }
                    #endregion
                }
                //En caso de que dibuje el punto, utiliza el punto ancho y punto alto del layer. Deberia cambiarse.

                it.BaseColor pdfContornoColor = GetAlphaColor(ColorTranslator.FromHtml(paContornoColor), 1);//Layer no tiene transparencia para contorno. Se puede parametrizar
                it.BaseColor pdfRellenoColor = GetAlphaColor(ColorTranslator.FromHtml(paRellenoColor), paTransparencia);

                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                float x1Pdf = 0, x2Pdf = 0;
                float y1Pdf = 0, y2Pdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                double anguloRotacionPdf = 0;

                foreach (var layerGraf in aLayerGraf)
                {
                    if (layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf) != null)
                    {
                        DbGeometry geometryLayerGraf = (DbGeometry)layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf);
                        string wkt = geometryLayerGraf.AsText();

                        if (wkt.Contains("POLYG"))//Solo esta configurado para polygonos.
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
                                        if (layer.Contorno)
                                        {
                                            if (plantilla.OptimizarTamanioHoja)
                                            {
                                                PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
                                                anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                            }
                                            else
                                            {
                                                //PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);
                                            }
                                        }
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                            lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                        }
                                        else
                                        {
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
                                                if (layer.Contorno)
                                                {
                                                    if (plantilla.OptimizarTamanioHoja)
                                                    {
                                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                        PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
                                                        anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                        anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                                    }
                                                    else
                                                    {
                                                        PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);
                                                    }
                                                }
                                                if (plantilla.OptimizarTamanioHoja)
                                                {
                                                    PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    lstPointPDFRing.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                                    lstPointPDFRing.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                                }
                                                else
                                                {
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
                                }
                                else
                                {
                                }
                                PDFUtilities.DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);

                            }
                            #endregion
                        }
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
                                        if (layer.Contorno)
                                        {
                                            if (plantilla.OptimizarTamanioHoja)
                                            {
                                                PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
                                                anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                            }
                                            /* else if (plantilla.Rotacion > 0)
                                             {
                                                 PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, plantilla.Rotacion * 180 / Math.PI, sentidoHorario);
                                                 PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, plantilla.Rotacion * 180 / Math.PI, sentidoHorario);
                                                 PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
                                             }*/
                                            else
                                                PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);

                                        }
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                            lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                        }
                                        /*else if (plantilla.Rotacion != null && plantilla.Rotacion > 0)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, plantilla.Rotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, plantilla.Rotacion, sentidoHorario);
                                            lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                            lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));

                                        }*/
                                        else
                                        {
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
                                                if (layer.Contorno)
                                                {
                                                    if (plantilla.OptimizarTamanioHoja)
                                                    {
                                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                        PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
                                                        anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                        anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                                    }
                                                    /*else if (plantilla.Rotacion != null && plantilla.Rotacion > 0)
                                                    {
                                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, plantilla.Rotacion, sentidoHorario);
                                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, plantilla.Rotacion, sentidoHorario);
                                                        lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                                        lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                                    }*/
                                                    else
                                                    {
                                                        PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);
                                                    }
                                                }
                                                if (plantilla.OptimizarTamanioHoja)
                                                {
                                                    PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    lstPointPDFRing.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                                    lstPointPDFRing.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                                }
                                                else
                                                {
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
                                }
                                if (layer.Relleno)
                                {
                                    PDFUtilities.DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                                }
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
                                        if (layer.Contorno)
                                        {
                                            if (plantilla.OptimizarTamanioHoja)
                                            {
                                                PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
                                                anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                            }
                                            else
                                            {
                                                PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);
                                            }
                                        }
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                            lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                        }
                                        else
                                        {
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
                                                if (layer.Contorno)
                                                {
                                                    if (plantilla.OptimizarTamanioHoja)
                                                    {
                                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                        PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
                                                        anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                                        anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                                    }
                                                    else
                                                    {
                                                        PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);
                                                    }
                                                }
                                                if (plantilla.OptimizarTamanioHoja)
                                                {
                                                    PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                                    lstPointPDFRing.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                                    lstPointPDFRing.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                                }
                                                else
                                                {
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
                                }
                                if (layer.Relleno)
                                {
                                    PDFUtilities.DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                                }
                            }
                            #endregion
                        }
                        else if (wkt.Contains("LINE"))
                        {
                            #region LINE
                            List<PointD> lstCoordsGeometry = GetlstCoordsGeometry(geometryLayerGraf, wkt);
                            List<PointF> lstPointPDF = new List<PointF>();
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
                                    if (layer.Contorno)
                                    {
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor, lineDash);
                                            anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
                                            anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
                                        }
                                        else
                                        {
                                            PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor, lineDash);
                                        }
                                    }
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
                                        lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
                                    }
                                    else
                                    {
                                        lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
                                        lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
                                    }
                                }
                            }
                            x2 = x;
                            y2 = y;
                            if (layer.Relleno)
                            {
                                PDFUtilities.DrawPDFPolygon(pdfContentByte, lstPointPDF, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
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
                            if (anguloRotacion != 0)
                            {
                                rotationRad = (float)(anguloRotacion / 10 * Math.PI / 180);
                            }
                            if (layer.PuntoRepresentacion == 2)
                            {
                                if (layer.PuntoImagen != null)
                                {
                                    float anchoEscalado = (float)layer.PuntoAncho.Value;
                                    float altoEscalado = (float)layer.PuntoAlto.Value;
                                    double escalaReal = 1 / escala;
                                    /*if (layer.PuntoEscala != escalaReal)
                                    {
                                        anchoEscalado = (float)(layer.PuntoAncho.Value * (1 / escalaReal) / (1 / (float)layer.PuntoEscala.Value));
                                        altoEscalado = (float)(layer.PuntoAlto.Value * (1 / escalaReal) / (1 / (float)layer.PuntoEscala.Value));
                                    }*/
                                    float puntoAnchoPts = it.Utilities.MillimetersToPoints(anchoEscalado);
                                    float puntoAltoPts = it.Utilities.MillimetersToPoints(altoEscalado);

                                    Bitmap bmpPunto = (Bitmap)layer.PuntoImagen;
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        rotationRad = !sentidoHorario ? rotationRad - (float)anguloRotacion : rotationRad + (float)anguloRotacion;
                                        PDFUtilities.DrawPDFImage(/*pdfDoc,*/ pdfContentByte, bmpPunto, ptRotado.X - (puntoAnchoPts / 2), ptRotado.Y - (puntoAltoPts / 2), puntoAnchoPts, puntoAltoPts, rotationRad);
                                    }
                                    else
                                    {
                                        PDFUtilities.DrawPDFImage(/*pdfDoc,*/ pdfContentByte, bmpPunto, x1Pdf - (puntoAnchoPts / 2), y1Pdf - (puntoAltoPts / 2), puntoAnchoPts, puntoAltoPts, rotationRad);
                                    }
                                }
                            }
                            else if (layer.PuntoRepresentacion == 1 && layer.Contorno)
                            {
                                //float puntoRadio = it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value);
                                float anchoEscalado = (float)layer.PuntoAncho.Value;
                                float altoEscalado = (float)layer.PuntoAlto.Value;
                                double escalaReal = 1 / escala;
                                if (layer.PuntoEscala.HasValue && layer.PuntoEscala.Value != escalaReal)//
                                {
                                    anchoEscalado = (float)(layer.PuntoAncho.Value * (1 / escalaReal) / (1 / (float)layer.PuntoEscala.Value));
                                    altoEscalado = (float)(layer.PuntoAlto.Value * (1 / escalaReal) / (1 / (float)layer.PuntoEscala.Value));
                                }
                                float puntoAnchoPts = it.Utilities.MillimetersToPoints(anchoEscalado);
                                float puntoAltoPts = it.Utilities.MillimetersToPoints(altoEscalado);
                                float puntoRadio = it.Utilities.MillimetersToPoints(anchoEscalado / 2);
                                if (layer.PuntoPredeterminado == 1)
                                {
                                    //Circulo
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        PDFUtilities.DrawPDFCircle(pdfContentByte, ptRotado.X, ptRotado.Y, puntoRadio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                    else
                                    {
                                        PDFUtilities.DrawPDFCircle(pdfContentByte, x1Pdf, y1Pdf, puntoRadio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                }
                                else if (layer.PuntoPredeterminado == 2)
                                {
                                    //Cuadrado
                                    if (plantilla.OptimizarTamanioHoja)
                                    {
                                        PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                        PDFUtilities.DrawPDFRectangle(pdfContentByte, ptRotado.X, ptRotado.Y, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                    else
                                    {
                                        PDFUtilities.DrawPDFRectangle(pdfContentByte, x1Pdf, y1Pdf, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RESALTAR PARCELA: Error al resaltar parcela.");
            }
        }

        #region comento porque no se usa, asociado a funcionalidades de SAR
        //private bool ResaltarParcela(it.Document pdfDoc, PdfContentByte pdfContentByte, LayerGraf[] parcelaDeSar, string nombre, Layer layerParcela, double xCentroidBase, double yCentroidBase, double factorEscala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario, ParcelaPlot puerta)
        //{

        //    if (layerParcela == null)
        //    {
        //        double? cotGros = 0.5f;
        //        bool contorno = true;
        //        #region parametros
        //        FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_CONTORNO_GROS");
        //        if (funcAdicParametro != null)
        //        {
        //            cotGros = Convert.ToDouble(funcAdicParametro.Valor);
        //        }
        //        funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_CONTORNO");
        //        if (funcAdicParametro != null)
        //        {
        //            if (funcAdicParametro.Valor == "1")
        //                contorno = true;
        //            else
        //                contorno = false;
        //        }
        //        #endregion
        //        layerParcela = new Layer();
        //        layerParcela.ContornoGrosor = cotGros;
        //        layerParcela.Contorno = contorno;
        //    }

        //    ResaltarParcela(pdfDoc, pdfContentByte, parcelaDeSar, nombre, layerParcela, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloRotacion, sentidoHorario);
        //    return true;
        //} 

        //private void ResaltarParcela(it.Document pdfDoc, PdfContentByte pdfContentByte, LayerGraf[] aLayerGraf, string layerGrafLabelProperty, Layer layer, double xCentroidBase, double yCentroidBase, double escala, Plantilla plantilla, double anguloRotacion, bool sentidoHorario)
        //{
        //    try
        //    {

        //        //Variables
        //        /*float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;
        //        PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);
        //        string lineDash = layer.Dash;*/
        //        //Ahora viene x mp_func_adic_parametro ya que no quieren plotear parcela. Gracias. Salu3. Mas facil lo hardcodeo aca. Bye.

        //        float pdfContornoGrosor = 0.5f;
        //        PdfPatternPainter pdfPatternPainter = null;
        //        string lineDash = null;

        //        string paContornoColor = "#960909";
        //        string paRellenoColor = "#960909";
        //        int paTransparencia = 80;
        //        //Fin Variables

        //        //Parametros
        //        #region Parametros
        //        FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_CONTORNO_COLO");
        //        if (funcAdicParametro != null)
        //        {
        //            paContornoColor = funcAdicParametro.Valor;
        //        }
        //        funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_RELLENO_COLOR");
        //        if (funcAdicParametro != null)
        //        {
        //            paRellenoColor = funcAdicParametro.Valor;
        //        }
        //        funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_PARCELA_TRANSPARENCIA");
        //        if (funcAdicParametro != null)
        //        {
        //            paTransparencia = Convert.ToInt32(funcAdicParametro.Valor);
        //        }
        //        #endregion
        //        //FinParametros



        //        it.BaseColor pdfContornoColor = GetAlphaColor(ColorTranslator.FromHtml(paContornoColor), paTransparencia);
        //        it.BaseColor pdfRellenoColor = GetAlphaColor(ColorTranslator.FromHtml(paRellenoColor), paTransparencia);

        //        double x = 0, y = 0;
        //        double x1 = 0, y1 = 0;
        //        double x2 = 0, y2 = 0;
        //        float x1Pdf = 0, x2Pdf = 0;
        //        float y1Pdf = 0, y2Pdf = 0;
        //        float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
        //        float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
        //        double anguloRotacionPdf = 0;
        //        foreach (var layerGraf in aLayerGraf)
        //        {
        //            string layerGrafNombre = string.Empty;
        //            if (!string.IsNullOrEmpty(layerGrafLabelProperty))
        //            {
        //                layerGrafNombre = layerGraf.GetType().GetProperty("Nombre", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(layerGraf).ToString();
        //            }
        //            if (layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf) != null)
        //            {
        //                DbGeometry geometryLayerGraf = (DbGeometry)layerGraf.GetType().GetProperty("Geom").GetValue(layerGraf);
        //                string wkt = geometryLayerGraf.AsText();

        //                if (wkt.Contains("POLYG"))//Solo esta configurado para polygonos.
        //                {
        //                    #region POLYGON/MULTIPOLYGON
        //                    int elementCount = (geometryLayerGraf.ElementCount != null ? (int)geometryLayerGraf.ElementCount : 1);
        //                    for (int iElem = 1; iElem <= elementCount; iElem++)
        //                    {
        //                        DbGeometry geometryLayerGrafElem = geometryLayerGraf;
        //                        if (geometryLayerGraf.ExteriorRing != null)
        //                        {
        //                            geometryLayerGrafElem = geometryLayerGraf.ExteriorRing;
        //                        }
        //                        if (geometryLayerGraf.ElementCount != null)
        //                        {
        //                            geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem);
        //                            if (geometryLayerGraf.ElementAt(iElem).ExteriorRing != null)
        //                            {
        //                                geometryLayerGrafElem = geometryLayerGraf.ElementAt(iElem).ExteriorRing;
        //                            }
        //                        }
        //                        List<Ring> lstRing = new List<Ring>();
        //                        int cantCoords = (int)geometryLayerGrafElem.PointCount;
        //                        List<PointF> lstPointPDF = new List<PointF>();
        //                        for (int i = 1; i <= cantCoords; i++)
        //                        {
        //                            x = (double)geometryLayerGrafElem.PointAt(i).XCoordinate;
        //                            y = (double)geometryLayerGrafElem.PointAt(i).YCoordinate;
        //                            if (i > 1)
        //                            {
        //                                x1 = x2;
        //                                y1 = y2;
        //                                x2 = x;
        //                                y2 = y;
        //                            }
        //                            else
        //                            {
        //                                x1 = x;
        //                                y1 = y;
        //                                x2 = x;
        //                                y2 = y;
        //                            }
        //                            if (i > 1)
        //                            {
        //                                #region Dibujar
        //                                x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
        //                                y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
        //                                x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
        //                                y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
        //                                if (layer.Contorno)
        //                                {
        //                                    if (plantilla.OptimizarTamanioHoja)
        //                                    {
        //                                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                        PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
        //                                        anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
        //                                        anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
        //                                    }
        //                                    else
        //                                    {
        //                                        //PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);
        //                                    }
        //                                }
        //                                if (plantilla.OptimizarTamanioHoja)
        //                                {
        //                                    PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                    PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                    lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
        //                                    lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
        //                                }
        //                                else
        //                                {
        //                                    lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
        //                                    lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
        //                                }
        //                                #endregion
        //                            }
        //                        }
        //                        Ring ring = new Ring();
        //                        ring.Interior = false;
        //                        ring.Puntos = lstPointPDF;
        //                        lstRing.Add(ring);
        //                        x2 = x;
        //                        y2 = y;

        //                        if (geometryLayerGraf.ElementAt(iElem).InteriorRingCount > 0)
        //                        {
        //                            for (int iRing = 1; iRing <= geometryLayerGraf.ElementAt(iElem).InteriorRingCount; iRing++)
        //                            {
        //                                DbGeometry geomInteriorRing = geometryLayerGraf.ElementAt(iElem).InteriorRingAt(iRing);
        //                                int cantCoordsIntRing = (int)geomInteriorRing.PointCount;
        //                                List<PointF> lstPointPDFRing = new List<PointF>();
        //                                for (int i = 1; i <= cantCoordsIntRing; i++)
        //                                {
        //                                    x = (double)geomInteriorRing.PointAt(i).XCoordinate;
        //                                    y = (double)geomInteriorRing.PointAt(i).YCoordinate;
        //                                    if (i > 1)
        //                                    {
        //                                        x1 = x2;
        //                                        y1 = y2;
        //                                        x2 = x;
        //                                        y2 = y;
        //                                    }
        //                                    else
        //                                    {
        //                                        x1 = x;
        //                                        y1 = y;
        //                                        x2 = x;
        //                                        y2 = y;
        //                                    }
        //                                    if (i > 1)
        //                                    {
        //                                        #region Dibujar
        //                                        x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
        //                                        y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
        //                                        x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
        //                                        y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
        //                                        if (layer.Contorno)
        //                                        {
        //                                            if (plantilla.OptimizarTamanioHoja)
        //                                            {
        //                                                PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                                PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                                PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor);
        //                                                anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
        //                                                anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
        //                                            }
        //                                            else
        //                                            {
        //                                                PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor);
        //                                            }
        //                                        }
        //                                        if (plantilla.OptimizarTamanioHoja)
        //                                        {
        //                                            PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                            PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                                            lstPointPDFRing.Add(new PointF(ptRotado1.X, ptRotado1.Y));
        //                                            lstPointPDFRing.Add(new PointF(ptRotado2.X, ptRotado2.Y));
        //                                        }
        //                                        else
        //                                        {
        //                                            lstPointPDFRing.Add(new PointF(x1Pdf, y1Pdf));
        //                                            lstPointPDFRing.Add(new PointF(x2Pdf, y2Pdf));
        //                                        }
        //                                        #endregion
        //                                    }
        //                                }
        //                                ring = new Ring();
        //                                ring.Interior = true;
        //                                ring.Puntos = lstPointPDFRing;
        //                                lstRing.Add(ring);
        //                            }
        //                        }
        //                        else
        //                        {
        //                        }
        //                        PDFUtilities.DrawPDFPolygon(pdfContentByte, lstRing, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);

        //                    }
        //                    #endregion
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("RESALTAR PARCELA: Error al resaltar parcela.");
        //    }
        //}
        #endregion

        /*private bool DibujarDatosCañeria(PdfContentByte pdfContentByte, List<string> lstCoordsBuffImpresion, double xCentroidBase, double yCentroidBase, double factorEscala, Plantilla plantilla, bool sentidoHorario)
        {
            string esquema = "GIS_AYSA";
            string tablaParcela = "VW_CT_PARCELA";

            string strColorTexto = "#027b91";
            float FuenteTamanio = 0.85f;

            #region Parametros
            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquema = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TABLA_PARCELA");
            if (funcAdicParametro != null)
            {
                tablaParcela = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "CAÑERIA_COLOR_TEXTO");
            if (funcAdicParametro != null)
            {
                strColorTexto = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "CAÑERIA_FUENTE_TAMAÑO");
            if (funcAdicParametro != null)
            {
                FuenteTamanio = (float)Convert.ToDecimal(funcAdicParametro.Valor);
            }
            #endregion

            //double desplazamientoCalle = 7;
            //FinVariables Parametro

            float xPdf = 0;
            float yPdf = 0;
            float yPDF = (float)it.Utilities.MillimetersToPoints((float)yCentroidBase);//Centro del pdf
            float xPDF = (float)it.Utilities.MillimetersToPoints((float)xCentroidBase);//Centro del pdf

            string FuenteNombre = "Arial";
            string FuenteEstilo = "0,0,0,0";
            Color colorTexto = System.Drawing.ColorTranslator.FromHtml(strColorTexto);

            float pdfFontSize = it.Utilities.MillimetersToPoints(FuenteTamanio);
            it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
            PDFUtilities.RegisterBaseFont(FuenteNombre, pdfFontSize);
            string[] aFontStylePdf = FuenteEstilo.Split(',');
            int pdfFontStyle = aFontStylePdf.Select(l => Convert.ToInt32(l)).Sum();
            BaseFont pdfbaseFont = it.FontFactory.GetFont(FuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
            it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);


            Canieria[] canierias = _layerGrafRepository.GetDatosCañeriaPlotByCoords(lstCoordsBuffImpresion);

            foreach (var cañeria in canierias)
            {
                DbGeometry geometryLayerGraf = (DbGeometry)cañeria.GetType().GetProperty("Geom").GetValue(cañeria);
                string wkt = geometryLayerGraf.AsText();


                List<PointD> lstCoordsGeometry = GetlstCoordsGeometry(geometryLayerGraf, wkt);


                double xs = (double)cañeria.Geom.StartPoint.XCoordinate;//5649653.51973581
                double ys = (double)cañeria.Geom.StartPoint.YCoordinate;//6169770.08937463
                double xe = (double)cañeria.Geom.EndPoint.XCoordinate;//5649653.92767972
                double ye = (double)cañeria.Geom.EndPoint.YCoordinate;//6169748.93481045
                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                float x1Pdf = 0, x2Pdf = 0;
                float y1Pdf = 0, y2Pdf = 0;

                double beta = 0;
                if (lstCoordsGeometry.Count > 2)
                {
                    var coord = lstCoordsGeometry[1];
                    xe = coord.X;
                    ye = coord.Y;
                }

                string texto = cañeria.Diametro.ToString() + "-" + cañeria.Material;

                var alignment = 0;

                xe = xe + (xs - xe) / 2;
                ye = ye + (ys - ye) / 2;
                xPdf = GetXPDFCanvas(xe, xCentroidBase, factorEscala, plantilla);
                yPdf = GetYPDFCanvas(ye, yCentroidBase, factorEscala, plantilla);
                /*
                if (plantilla.OptimizarTamanioHoja)
                {
                    PointF ptRotado = Rotate(xPdf, yPdf, (float)xCentroidBase, (float)yCentroidBase, alignment, sentidoHorario);
                    double anguloPdf = alignment - (alignment * 180 / Math.PI);
                    PDFUtilities.DrawPDFText(pdfContentByte, texto, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTexto, anguloPdf);
                }
                else
                {
                    PDFUtilities.DrawPDFText(pdfContentByte, texto, xPdf, yPdf, pdfbaseFont, pdfFontSize, colorTexto, alignment);
                }--

                #region Dibujar

                for (var i = 0; lstCoordsGeometry.Count > i; i++)
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
                        x1Pdf = GetXPDFCanvas(x1, xCentroidBase, factorEscala, plantilla);
                        y1Pdf = GetYPDFCanvas(y1, yCentroidBase, factorEscala, plantilla);
                        x2Pdf = GetXPDFCanvas(x2, xCentroidBase, factorEscala, plantilla);
                        y2Pdf = GetYPDFCanvas(y2, yCentroidBase, factorEscala, plantilla);

                        double largox = (y1Pdf - y2Pdf);
                        double largoy = (x1Pdf - x2Pdf);
                        largox = largox > 0 ? largox : largox * -1;
                        largoy = largoy > 0 ? largoy : largoy * -1;
                        double largo = largox + largoy;
                        //largo = largo > 0 ? largo : largo * -1;
                        if (largo < 15)
                        {
                            continue;
                        }

                        var xaux = x2 + (x1 - x2) / 2;
                        var yaux = y2 + (y1 - y2) / 2;

                        var xplot = GetXPDFCanvas(xaux, xCentroidBase, factorEscala, plantilla);
                        var yplot = GetYPDFCanvas(yaux, yCentroidBase, factorEscala, plantilla);

                        //var alignment = 0;//(Math.Atan2(xPdf, yPdf) * 180 / Math.PI);

                        //xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
                        //yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
                        PDFUtilities.DrawPDFText(pdfContentByte, texto, xplot, yplot, pdfbaseFont, pdfFontSize, colorTexto, alignment);
                    }
                }
                #endregion

            }

            return true;
        }*/

        #region comento porque no se usa.
        //private int GetIdManzanaByIdObjetoGraf(int pIdObjGrafico)
        //{
        //    int idManzana = 0;
        //    idManzana = _layerGrafRepository.GetIdManzanaByIdObjetoGraf(pIdObjGrafico);
        //    return idManzana;
        //} 
        #endregion

        #region comento porque se usa solo para plancheta SAR, propia de aysa
        //private bool DibujarDireccionDeParcela(PdfContentByte pdfContentByte, Plantilla plantilla, string idObjetoBase, double xCentroidBase, double yCentroidBase, double factorEscala, ParcelaPlot parcelaPlot, bool sentidoHorario)
        //{
        //    //Variables Parametro

        //    string esquema = "GIS_AYSA";
        //    string tablaParcela = "VW_CT_PARCELA";

        //    string strColorTexto = "#027b91";
        //    float FuenteTamanio = 2;
        //    double desplazamientoCalle = 7;
        //    //FinVariables Parametro

        //    //Parametros
        //    #region Parametros
        //    FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "ESQUEMA");
        //    if (funcAdicParametro != null)
        //    {
        //        esquema = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TABLA_PARCELA");
        //    if (funcAdicParametro != null)
        //    {
        //        tablaParcela = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "DIRECCION_COLOR_TEXTO");
        //    if (funcAdicParametro != null)
        //    {
        //        strColorTexto = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "DIRECCION_FUENTE_TAMAÑO");
        //    if (funcAdicParametro != null)
        //    {
        //        FuenteTamanio = (float)Convert.ToDecimal(funcAdicParametro.Valor);
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "DIRECCION_DESPLAZAMIENTO_CALLE");
        //    if (funcAdicParametro != null)
        //    {
        //        desplazamientoCalle = Convert.ToDouble(funcAdicParametro.Valor);
        //    }
        //    #endregion
        //    //FinParametros


        //    LayerGraf puerta = GetPuerta(idObjetoBase);

        //    float xPdf = 0;
        //    float yPdf = 0;
        //    float yPDF = (float)it.Utilities.MillimetersToPoints((float)yCentroidBase);//Centro del pdf
        //    float xPDF = (float)it.Utilities.MillimetersToPoints((float)xCentroidBase);//Centro del pdf

        //    string FuenteNombre = "Arial";
        //    string FuenteEstilo = "0,0,0,0";
        //    Color colorTexto = System.Drawing.ColorTranslator.FromHtml(strColorTexto);

        //    float pdfFontSize = it.Utilities.MillimetersToPoints(FuenteTamanio);
        //    it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
        //    PDFUtilities.RegisterBaseFont(FuenteNombre, pdfFontSize);
        //    string[] aFontStylePdf = FuenteEstilo.Split(',');
        //    int pdfFontStyle = aFontStylePdf.Select(l => Convert.ToInt32(l)).Sum();
        //    BaseFont pdfbaseFont = it.FontFactory.GetFont(FuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
        //    it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);
        //    double alignment = Convert.ToDouble(puerta.Descripcion); ;//D1||epende del angulo de la puerta

        //    string texto = GetDireccionByIdObjGraf(idObjetoBase);

        //    double xMin = (double)puerta.Geom.Centroid.XCoordinate;
        //    double yMin = (double)puerta.Geom.Centroid.YCoordinate;
        //    double xMax = (double)puerta.Geom.Centroid.XCoordinate;
        //    double yMax = (double)puerta.Geom.Centroid.YCoordinate;
        //    double beta = 0;

        //    double x = 0, y = 0;


        //    #region Ubico el texto
        //    beta = alignment;
        //    if (alignment < 0)
        //    {
        //        beta = alignment + 360;
        //    }

        //    double alfa = beta;

        //    double textRotation = alfa;

        //    if (alfa >= 0 && alfa <= 90)
        //    {
        //        beta = 90 - alfa;
        //        //textRotation = alfa + 90 + 180;
        //    }
        //    else if (alfa > 90 && alfa <= 180)
        //    {
        //        beta = alfa - 90;
        //        //textRotation = beta;
        //    }
        //    else if (alfa > 180 && alfa <= 270)
        //    {
        //        beta = 270 - alfa;
        //        //textRotation = alfa - 90;
        //    }
        //    else if (alfa > 270 && alfa <= 360)
        //    {
        //        beta = alfa - 270;
        //        //textRotation = beta;
        //    }

        //    double betaRad = beta * Math.PI / 180.0;

        //    x = xMin + (xMax - xMin) / 2;
        //    y = yMin + (yMax - yMin) / 2;

        //    double desplazamientoX = desplazamientoCalle * Math.Cos(betaRad);
        //    double desplazamientoY = desplazamientoCalle * Math.Sin(betaRad);

        //    double x1Des = x + desplazamientoCalle * Math.Cos(betaRad);
        //    double y1Des = y + desplazamientoCalle * Math.Sin(betaRad);
        //    double x2Des = x - desplazamientoCalle * Math.Cos(betaRad);
        //    double y2Des = y - desplazamientoCalle * Math.Sin(betaRad);

        //    x1Des = x + desplazamientoX;
        //    x2Des = x - desplazamientoX;
        //    if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
        //    {
        //        y1Des = y - desplazamientoY;
        //        y2Des = y + desplazamientoY;
        //    }
        //    else if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
        //    {
        //        y1Des = y + desplazamientoY;
        //        y2Des = y - desplazamientoY;
        //    }

        //    ParcelaPlot[] aParcelaPlotDes = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tablaParcela, x1Des, y1Des);
        //    ParcelaPlot[] aParcelaPlotDes2 = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tablaParcela, x2Des, y2Des);
        //    if (aParcelaPlotDes != null && aParcelaPlotDes.Length > 0 && parcelaPlot.FeatId == aParcelaPlotDes[0].FeatId)
        //    {
        //        x = x2Des;
        //        y = y2Des;
        //    }
        //    else
        //    {
        //        x = x1Des;
        //        y = y1Des;
        //    }
        //    xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
        //    yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
        //    if (plantilla.OptimizarTamanioHoja)
        //    {
        //        PointF ptRotado = Rotate(xPdf, yPdf, (float)xCentroidBase, (float)yCentroidBase, alignment, sentidoHorario);
        //        double anguloPdf = alignment - (alignment * 180 / Math.PI);
        //        PDFUtilities.DrawPDFText(pdfContentByte, texto, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTexto, anguloPdf);
        //    }
        //    else
        //    {
        //        PDFUtilities.DrawPDFText(pdfContentByte, texto, xPdf, yPdf, pdfbaseFont, pdfFontSize, colorTexto, alignment);
        //    }
        //    #endregion
        //    return true;
        //} 

        //private bool ResaltarFrenteParcela(PdfContentByte pdfContentByte, Plantilla plantilla, Componente componenteBase, string idObjetoBase, List<string> lstCoordenadas, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff, double xCentroidBase, double yCentroidBase, double factorEscala, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, ref ParcelaPlot aParcelaPlot, Layer layer, double escala)
        //{
        //    //Valriables
        //    string esquema = "gis_aysa";
        //    string tablaCuadra = "CT_CUADRA";
        //    string campoGeometryCuadra = "GEOMETRY";
        //    string paContornoColor = "#ff0000";
        //    string paRellenoTransparencia = "#ff0000";

        //    string campoIdCuadra = "ID_CUADRA";
        //    int anguloTolerancia = 2;//Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
        //    double distanciaTolerancia = 0.1;
        //    double distBuffer = 6;
        //    //FinVariables

        //    NumberFormatInfo numberFormat = new NumberFormatInfo();
        //    numberFormat.CurrencyDecimalDigits = 4;
        //    numberFormat.CurrencyDecimalSeparator = ".";
        //    numberFormat.NumberDecimalDigits = 4;
        //    numberFormat.NumberDecimalSeparator = ".";
        //    numberFormat.PercentDecimalDigits = 2;
        //    numberFormat.PercentDecimalSeparator = ".";


        //    //Parametrizar
        //    layer = new Layer();
        //    layer.RellenoColor = "#ff0000";
        //    layer.ContornoColor = "#ff0000";

        //    //Parametros
        //    #region Parametros de la DB
        //    FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "ESQUEMA");
        //    if (funcAdicParametro != null)
        //    {
        //        esquema = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "TABLA_CUADRA");
        //    if (funcAdicParametro != null)
        //    {
        //        tablaCuadra = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "CAMPO_GEOMETRY");
        //    if (funcAdicParametro != null)
        //    {
        //        campoGeometryCuadra = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_FRENT_PARC_CONTORNO_COLO");
        //    if (funcAdicParametro != null)
        //    {
        //        paContornoColor = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_FRENT_PARC_RELLENO_COLO");
        //    if (funcAdicParametro != null)
        //    {
        //        paRellenoTransparencia = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "CAMPO_ID_CUADRA");
        //    if (funcAdicParametro != null)
        //    {
        //        campoIdCuadra = funcAdicParametro.Valor;
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_FRENT_PARC_ANGUL_TOLERAN");
        //    if (funcAdicParametro != null)
        //    {
        //        int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESAL_FRENT_PARC_DISTA_TOLERAN");
        //    if (funcAdicParametro != null)
        //    {
        //        distanciaTolerancia = Convert.ToDouble(funcAdicParametro.Valor);
        //    }
        //    funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == plantilla.IdFuncionAdicional && p.Campo.ToUpper() == "RESALTAR_FRENTE_PARCELA_BUFFER");
        //    if (funcAdicParametro != null)
        //    {
        //        distBuffer = Convert.ToDouble(funcAdicParametro.Valor);
        //    }

        //    #endregion
        //    //Fin Parametros


        //    //Variables
        //    DbGeometry geometryParcelaPlot = (DbGeometry)aParcelaPlot.GetType().GetProperty("Geom").GetValue(aParcelaPlot); //Obtengo la geometria de la parcela
        //    PointF puntoMedio = new PointF();
        //    List<Lado> lados = new List<Lado>();
        //    Lado ladoMayor = new Lado();
        //    double anguloRotacionLadoMayor = GetAnguloRotacion(geometryParcelaPlot, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);


        //    double x = 0, y = 0;
        //    double xMin = 9999999, yMin = 9999999;
        //    double xMax = 0, yMax = 0;
        //    string sDistancia = string.Empty;
        //    float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
        //    float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
        //    string nroPuerta = string.Empty;
        //    double x1Cuadra = 0;
        //    double y1Cuadra = 0;
        //    double x2Cuadra = 0;
        //    double y2Cuadra = 0;
        //    CuadraPlot[] aCuadraPlot = _cuadraPlotRepository.GetCuadraPlotByIdCuadra(esquema, tablaCuadra, campoGeometryCuadra, campoIdCuadra, aParcelaPlot.IdCuadra);
        //    //Fin Variables

        //    //Si encuentra la cuadra de la parcela
        //    if (aCuadraPlot != null && aCuadraPlot.Length > 0)
        //    {
        //        CuadraPlot cuadra = aCuadraPlot[0];
        //        DbGeometry geometryCuadra = (DbGeometry)cuadra.GetType().GetProperty("Geom").GetValue(cuadra);
        //        string wkt = geometryCuadra.AsText();
        //        if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
        //        {
        //            int cantCoords = (int)geometryCuadra.PointCount;
        //            x1Cuadra = (double)geometryCuadra.PointAt(1).XCoordinate;
        //            y1Cuadra = (double)geometryCuadra.PointAt(1).YCoordinate;
        //            x2Cuadra = (double)geometryCuadra.PointAt(cantCoords).XCoordinate;
        //            y2Cuadra = (double)geometryCuadra.PointAt(cantCoords).YCoordinate;
        //        }
        //        List<Lado> lstLadoCuadra = new List<Lado>();
        //        //Guardo el wkt y geometry
        //        string wktLadoG = "";
        //        DbGeometry geometryLadoG = (DbGeometry)cuadra.GetType().GetProperty("Geom").GetValue(cuadra);

        //        foreach (var lado in lados)
        //        {
        //            //Arma las coordenadas de los segmentos
        //            xMin = 9999999;
        //            yMin = 9999999;
        //            xMax = 0;
        //            yMax = 0;
        //            List<string> lstCoordsGeometry = new List<string>();
        //            List<string> lstCoordsGeometryLado = new List<string>();
        //            string coordsLado = string.Empty;
        //            foreach (var segmento in lado.Segmentos)
        //            {
        //                #region Determinar xMin yMin xMax yMax
        //                if (xMin >= segmento.P1.X)
        //                {
        //                    xMin = segmento.P1.X;
        //                }
        //                if (xMax < segmento.P1.X)
        //                {
        //                    xMax = segmento.P1.X;
        //                }
        //                if (yMin >= segmento.P1.Y)
        //                {
        //                    yMin = segmento.P1.Y;
        //                }
        //                if (yMax < segmento.P1.Y)
        //                {
        //                    yMax = segmento.P1.Y;
        //                }
        //                if (xMin >= segmento.P2.X)
        //                {
        //                    xMin = segmento.P2.X;
        //                }
        //                if (xMax < segmento.P2.X)
        //                {
        //                    xMax = segmento.P2.X;
        //                }
        //                if (yMin >= segmento.P2.Y)
        //                {
        //                    yMin = segmento.P2.Y;
        //                }
        //                if (yMax < segmento.P2.Y)
        //                {
        //                    yMax = segmento.P2.Y;
        //                }
        //                #endregion

        //                lstCoordsGeometry.Add(segmento.P1.X.ToString().Replace(",", ".") + ", " + segmento.P1.Y.ToString().Replace(",", "."));
        //                lstCoordsGeometry.Add(segmento.P2.X.ToString().Replace(",", ".") + ", " + segmento.P2.Y.ToString().Replace(",", "."));

        //                coordsLado += segmento.P1.X.ToString().Replace(",", ".") + " " + segmento.P1.Y.ToString().Replace(",", ".") + ",";
        //                coordsLado += segmento.P2.X.ToString().Replace(",", ".") + " " + segmento.P2.Y.ToString().Replace(",", ".") + ",";
        //            }
        //            if (coordsLado != string.Empty)
        //            {
        //                coordsLado = coordsLado.Substring(0, coordsLado.Length - 1);
        //            }
        //            //Fin arma las coordenadas de los segmentos

        //            string wktLado = "LINESTRING (" + coordsLado + ")";//Armar la wkt del lado con los segmentos
        //            DbGeometry geometryLado = DbGeometry.LineFromText(wktLado, 22195);//Armar la dbgeometry del lado con los segmentos

        //            //buscar en geometryCuadra.buffer. si el dbgeometry cae adentro
        //            bool contieneLado = false;
        //            try
        //            {
        //                contieneLado = geometryCuadra.Buffer(distBuffer).Contains(geometryLado);
        //            }
        //            catch { }
        //            //----------
        //            if (contieneLado)
        //            {//Si el frente tiene mas de un "lado" solo va a pintar uno. Se puede mejorar
        //                lstLadoCuadra.Add(lado);
        //                wktLadoG = wktLado;
        //                geometryLadoG = geometryLado;
        //            }
        //        }
        //        if (lstLadoCuadra.Count > 0)
        //        {
        //            #region dibujar

        //            //Variables
        //            float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;
        //            string lineDash = layer.Dash;

        //            it.BaseColor pdfContornoColor = GetAlphaColor(ColorTranslator.FromHtml(paContornoColor), layer.RellenoTransparencia);
        //            it.BaseColor pdfRellenoColor = GetAlphaColor(ColorTranslator.FromHtml(paRellenoTransparencia), layer.RellenoTransparencia);

        //            x = y = 0;
        //            double x1 = 0, y1 = 0;
        //            double x2 = 0, y2 = 0;
        //            float x1Pdf = 0, x2Pdf = 0;
        //            float y1Pdf = 0, y2Pdf = 0;
        //            double anguloRotacionPdf = 0;
        //            //Fin Variables


        //            #region Dibuja Linea
        //            List<PointD> lstCoordsGeometry2 = GetlstCoordsGeometry(geometryLadoG, wktLadoG);
        //            List<PointF> lstPointPDF = new List<PointF>();
        //            for (int i = 0; i < lstCoordsGeometry2.Count; i++)
        //            {
        //                x = lstCoordsGeometry2[i].X;
        //                y = lstCoordsGeometry2[i].Y;

        //                if (i > 0)
        //                {
        //                    x1 = x2;
        //                    y1 = y2;
        //                    x2 = x;
        //                    y2 = y;
        //                }
        //                else
        //                {
        //                    x1 = x;
        //                    y1 = y;
        //                    x2 = x;
        //                    y2 = y;
        //                }
        //                if (i > 0)
        //                {
        //                    x1Pdf = GetXPDFCanvas(x1, xCentroidBase, escala, plantilla);
        //                    y1Pdf = GetYPDFCanvas(y1, yCentroidBase, escala, plantilla);
        //                    x2Pdf = GetXPDFCanvas(x2, xCentroidBase, escala, plantilla);
        //                    y2Pdf = GetYPDFCanvas(y2, yCentroidBase, escala, plantilla);
        //                    if (plantilla.OptimizarTamanioHoja)
        //                    {
        //                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                        PDFUtilities.DrawPDFLine(pdfContentByte, ptRotado1.X, ptRotado1.Y, ptRotado2.X, ptRotado2.Y, pdfContornoGrosor, pdfContornoColor, lineDash);
        //                        anguloRotacionPdf = Math.Atan((ptRotado2.Y - ptRotado1.Y) / (ptRotado2.X - ptRotado1.X));
        //                        anguloRotacionPdf = anguloRotacionPdf * 180 / Math.PI;
        //                    }
        //                    else
        //                    {
        //                        PDFUtilities.DrawPDFLine(pdfContentByte, x1Pdf, y1Pdf, x2Pdf, y2Pdf, pdfContornoGrosor, pdfContornoColor, lineDash);
        //                    }

        //                    if (plantilla.OptimizarTamanioHoja)
        //                    {
        //                        PointF ptRotado1 = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                        PointF ptRotado2 = Rotate(x2Pdf, y2Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
        //                        lstPointPDF.Add(new PointF(ptRotado1.X, ptRotado1.Y));
        //                        lstPointPDF.Add(new PointF(ptRotado2.X, ptRotado2.Y));
        //                    }
        //                    else
        //                    {
        //                        lstPointPDF.Add(new PointF(x1Pdf, y1Pdf));
        //                        lstPointPDF.Add(new PointF(x2Pdf, y2Pdf));
        //                    }
        //                }
        //            }
        //            x2 = x;
        //            y2 = y;
        //            if (true)
        //            {
        //                PDFUtilities.DrawPDFPolygon(pdfContentByte, lstPointPDF, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
        //            }
        //            #endregion
        //            #endregion dibujar
        //        }

        //    }
        //    else//Si no encuentra la cuadra de la parcela
        //    {
        //        throw new Exception("RESALTAR FRENTE PARCELA - No se encontro la manzana de la parcela");
        //    }
        //    return true;

        //}

        //private LayerGraf GetPuerta(string idObjGraf)
        //{
        //    LayerGraf puerta = new LayerGraf();
        //    puerta = _layerGrafRepository.GetPuerta(idObjGraf);
        //    return puerta;
        //}

        //private LayerGraf GetParcelaSAR(string pIdUbicacionPloteo)
        //{
        //    LayerGraf parcelaSar = new LayerGraf();
        //    parcelaSar = _layerGrafRepository.GetParcelaSAR(pIdUbicacionPloteo);
        //    return parcelaSar;
        //}
        #endregion

        #region comento porque no se usa, asociado a plancheta SAR
        //private string GetDireccionByIdObjGraf(string pIdObjGraf)
        //{
        //    return _layerGrafRepository.GetDireccionByIdObjGraf(pIdObjGraf);
        //} 

        //private ParcelaPlot GetParcelaPlotByIdObjGraf(string pIdUbicacionPloteo)
        //{
        //    ParcelaPlot parcelaSar = new ParcelaPlot();
        //    parcelaSar = _parcelaPlotRepository.GetParcelaPlotByIdObjGraf(pIdUbicacionPloteo);
        //    return parcelaSar;
        //}
        #endregion

        private List<PointD> GetlstCoordsGeometry(DbGeometry geometryLayerGraf, string wkt)
        {
            List<PointD> lstCoordsGeometry = new List<PointD>();
            if (geometryLayerGraf.IsValid)
            {
                int cantCoords = (int)geometryLayerGraf.PointCount;
                for (int j = 1; j <= cantCoords; j++)
                {
                    double xaux = (double)geometryLayerGraf.PointAt(j).XCoordinate;
                    double yaux = (double)geometryLayerGraf.PointAt(j).YCoordinate;
                    PointD pt = new PointD(xaux, yaux);
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
                    double xaux = Convert.ToDouble(aCoord[0]);
                    double yaux = Convert.ToDouble(aCoord[1]);
                    PointD pt = new PointD(xaux, yaux);
                    lstCoordsGeometry.Add(pt);
                }

            }
            return lstCoordsGeometry;
        }

        private bool FuncEspDibujarInformeAnualReferenciaTitulos(int idFuncionAdicional, it.Document pdfDoc, PdfContentByte pdfContentByte, Plantilla plantilla, string anio, Componente componentePartido, string idPartido)
        {
            bool ret = true;
            try
            {
                #region Inicializacion de Variables y defaults

                float textoEspaciadoMM = (float)1.41083;
                string textoFuenteNombre = "Arial";
                double textoFuenteTamanio = 0.8;
                string textoFuenteEstilo = "0,0,0,0";
                string textoColor = "#000000";
                float espaciado = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaEspaciado);

                #endregion

                #region NumberFormat
                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.CurrencyDecimalDigits = 4;
                numberFormat.CurrencyDecimalSeparator = ".";
                numberFormat.NumberDecimalDigits = 4;
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.PercentDecimalDigits = 2;
                numberFormat.PercentDecimalSeparator = ".";
                #endregion

                #region Parametros de la DB
                #region Font
                FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
                if (funcAdicParametro != null)
                {
                    float.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoEspaciadoMM);
                }
                #endregion
                #endregion

                float pdfFontSizeTabla = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);
                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSizeTabla);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStyleTabla = textoFuenteEstilo.Split(',');
                int pdfFontStyleTabla = aFontStyleTabla.Select(x => Convert.ToInt32(x)).Sum();
                Color colorTabla = System.Drawing.ColorTranslator.FromHtml(textoColor);
                it.BaseColor pdfColorTextoTabla = new it.BaseColor(colorTabla.R, colorTabla.G, colorTabla.B);
                BaseFont pdfbaseFontTabla = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSizeTabla, pdfFontStyleTabla, pdfColorTextoTabla).BaseFont;
                it.Font pdfFontTabla = new it.Font(pdfbaseFontTabla, pdfFontSizeTabla, pdfFontStyleTabla, pdfColorTextoTabla);

                List<TipoPlano> lstTipoPlano = _tipoPlanoRepository.GetTipoPlanos().ToList();
                TipoPlano tipoPlano = lstTipoPlano.FirstOrDefault(p => p.IdPlantilla == plantilla.IdPlantilla);
                Partido partido = _partidoRepository.GetPartidoById(Convert.ToInt64(idPartido));
                string regionNombre = _partidoRepository.GetRegionNombre(componentePartido, Convert.ToInt64(idPartido));
                Censo censo = _censoRepository.GetCensos().OrderByDescending(p => p.Anio).ToList()[0];

                float x1c = it.Utilities.MillimetersToPoints((float)705);
                //float x1c = it.Utilities.MillimetersToPoints((float)723);
                float y1c = it.Utilities.MillimetersToPoints((float)72);
                textoEspaciadoMM = 10;
                //float tableWidth = it.Utilities.MillimetersToPoints((float)68);
                float tableWidth = it.Utilities.MillimetersToPoints((float)104);
                //float tableWidth = it.Utilities.MillimetersToPoints((float)100);

                int cols = 1;
                PdfPTable pdfTable;
                pdfTable = new PdfPTable(cols);
                pdfTable.TotalWidth = tableWidth;
                float[] widths = new float[1] { tableWidth };
                pdfTable.SetWidths(widths);
                bool drawBorder = false;

                PdfPCell pdfCell = new PdfPCell();
                int alignmentCol1 = it.Element.ALIGN_CENTER;

                string texto = tipoPlano.Tema;
                float textoSize = (float)5.8;
                int textoFontStyle = 1;
                it.Font textoFont = new it.Font(pdfbaseFontTabla, it.Utilities.MillimetersToPoints(textoSize), textoFontStyle, pdfColorTextoTabla);
                pdfCell = new PdfPCell();
                pdfCell = PDFUtilities.GetCellForTable(texto, textoFont, alignmentCol1, textoEspaciadoMM, drawBorder);
                pdfCell.UseBorderPadding = true;
                pdfTable.AddCell(pdfCell);

                texto = tipoPlano.Servicio;
                textoSize = (float)3.9;
                textoFontStyle = 0;
                textoFont = new it.Font(pdfbaseFontTabla, it.Utilities.MillimetersToPoints(textoSize), textoFontStyle, pdfColorTextoTabla);
                pdfCell = new PdfPCell();
                pdfCell = PDFUtilities.GetCellForTable(texto, textoFont, alignmentCol1, textoEspaciadoMM, drawBorder);
                pdfCell.UseBorderPadding = true;
                pdfTable.AddCell(pdfCell);

                texto = "PARTIDO " + partido.Nombre;
                textoSize = (float)5.8;
                textoFontStyle = 1;
                textoFont = new it.Font(pdfbaseFontTabla, it.Utilities.MillimetersToPoints(textoSize), textoFontStyle, pdfColorTextoTabla);
                pdfCell = new PdfPCell();
                pdfCell = PDFUtilities.GetCellForTable(texto, textoFont, alignmentCol1, textoEspaciadoMM, drawBorder);
                pdfCell.UseBorderPadding = true;
                pdfTable.AddCell(pdfCell);

                texto = regionNombre;
                textoSize = (float)3.25;
                textoFontStyle = 1;
                textoFont = new it.Font(pdfbaseFontTabla, it.Utilities.MillimetersToPoints(textoSize), textoFontStyle, pdfColorTextoTabla);
                pdfCell = new PdfPCell();
                pdfCell = PDFUtilities.GetCellForTable(texto, textoFont, alignmentCol1, textoEspaciadoMM, drawBorder);
                pdfCell.UseBorderPadding = true;
                pdfTable.AddCell(pdfCell);

                texto = anio;
                textoSize = (float)3.25;
                textoFontStyle = 1;
                textoFont = new it.Font(pdfbaseFontTabla, it.Utilities.MillimetersToPoints(textoSize), textoFontStyle, pdfColorTextoTabla);
                pdfCell = new PdfPCell();
                pdfCell = PDFUtilities.GetCellForTable(texto, textoFont, alignmentCol1, textoEspaciadoMM, drawBorder);
                pdfCell.UseBorderPadding = true;
                pdfTable.AddCell(pdfCell);

                pdfTable.WriteSelectedRows(0, -1, x1c, y1c - (textoEspaciadoMM * 2), pdfContentByte);

                //Tabla 2
                x1c = it.Utilities.MillimetersToPoints((float)814);
                y1c = it.Utilities.MillimetersToPoints((float)35);
                tableWidth = it.Utilities.MillimetersToPoints((float)22);
                textoEspaciadoMM = 6;
                pdfTable = new PdfPTable(cols);
                pdfTable.TotalWidth = tableWidth;
                widths = new float[1] { tableWidth };
                pdfTable.SetWidths(widths);

                drawBorder = false;

                pdfCell = new PdfPCell();
                alignmentCol1 = it.Element.ALIGN_CENTER;

                int iAnio = Convert.ToInt32(anio) - 5 - 2000;
                texto = tipoPlano.CodigoPlano + partido.Abrev + iAnio.ToString();
                textoSize = (float)3;
                textoFontStyle = 0;
                textoFont = new it.Font(pdfbaseFontTabla, it.Utilities.MillimetersToPoints(textoSize), textoFontStyle, pdfColorTextoTabla);
                pdfCell = new PdfPCell();
                pdfCell = PDFUtilities.GetCellForTable(texto, textoFont, alignmentCol1, textoEspaciadoMM, drawBorder);
                pdfCell.UseBorderPadding = true;
                pdfTable.AddCell(pdfCell);

                texto = censo.Descripcion;
                textoSize = (float)3;
                textoFontStyle = 0;
                textoFont = new it.Font(pdfbaseFontTabla, it.Utilities.MillimetersToPoints(textoSize), textoFontStyle, pdfColorTextoTabla);
                pdfCell = new PdfPCell();
                pdfCell = PDFUtilities.GetCellForTable(texto, textoFont, alignmentCol1, textoEspaciadoMM, drawBorder);
                pdfCell.UseBorderPadding = true;
                pdfTable.AddCell(pdfCell);

                pdfTable.WriteSelectedRows(0, -1, x1c, y1c - (textoEspaciadoMM * 2), pdfContentByte);

            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return ret;
        }

        private bool DibujarNombreCallesEnPlano(int idFuncionAdicional, PdfContentByte pdfContentByte, Plantilla plantilla, double xCentroidBase, double yCentroidBase, double factorEscala, Componente componenteBase, string idObjetoGraf, string anio, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro)
        {
            bool ret = true;
            try
            {
                #region Inicializacion de Variables y defaults
                int filtroGeografico = 1;
                //Cuadra
                bool nombreCallesEnPlano = true;
                string cuadraEsquema = "AYSA_GERED_DEV";
                string cuadraTabla = "CT_CUADRA";
                string cuadraCampoGeometry = "GEOMETRY";
                string cuadraCampoIdCuadra = "ID_CUADRA";
                string cuadraCampoIdManzana = "ID_MANZANA";
                string cuadraCampoIdCalle = "ID_CALLE";
                string cuadraCampoAlturaMin = "ALTURA_MIN";
                string cuadraCampoAlturaMax = "ALTURA_MAX";
                string cuadraCampoIdParidad = "ID_PARIDAD";
                //Calle
                string calleEsquema = "AYSA_GERED_DEV";
                string calleTabla = "CT_CALLE";
                string calleCampoIdCalle = "ID_CALLE";
                string calleCampoNombre = "NOMBRE";
                string calleCampoCodigo = "APIC_ID";
                //Manzana
                string manzanaEsquema = "AYSA_GERED_DEV";
                string manzanaTabla = "CT_MANZANA";
                string manzanaCampoGeometry = "GEOMETRY";
                string manzanaCampoIdManzana = "ID_MANZANA";
                //Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
                int anguloTolerancia = 2;
                double distanciaTolerancia = 0.1;
                //double distBuffer = 8;
                double desplazamientoCalle = 5;
                //double distanciaLadoOchava = 10;
                string textoFuenteNombre = "Arial";
                double textoFuenteTamanio = 0.2;
                string textoFuenteEstilo = "0,0,0,0";
                string textoColor = "#696969";

                #endregion

                #region NumberFormat
                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.CurrencyDecimalDigits = 4;
                numberFormat.CurrencyDecimalSeparator = ".";
                numberFormat.NumberDecimalDigits = 4;
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.PercentDecimalDigits = 2;
                numberFormat.PercentDecimalSeparator = ".";
                #endregion

                #region Parametros de la DB
                FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "NOMBRE_CALLES_EN_PLANO");
                if (funcAdicParametro != null)
                {
                    //int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
                    nombreCallesEnPlano = (funcAdicParametro.Valor == "1" ? true : false);
                }


                #region Cuadra
                //ParametrosGenerales parametroGeneral = db.ParametrosGenerales.FirstOrDefault(p => p.Descripcion.ToUpper() == "ID_COMPONENTE_DISTRITO");
                //cuadraEsquema = _parametroRepository.GetParametroByDescripcion("CUADRA_ESQUEMA");

                //FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_COMPONENTE_ID");
                //if (funcAdicParametro != null)
                //{
                //    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out cuadraComponenteId);
                //}
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    cuadraEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_TABLA");
                if (funcAdicParametro != null)
                {
                    cuadraTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    cuadraCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_CUADRA");
                if (funcAdicParametro != null)
                {
                    cuadraCampoIdCuadra = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_MANZANA");
                if (funcAdicParametro != null)
                {
                    cuadraCampoIdManzana = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_CALLE");
                if (funcAdicParametro != null)
                {
                    cuadraCampoIdCalle = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ALTURA_MIN");
                if (funcAdicParametro != null)
                {
                    cuadraCampoAlturaMin = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ALTURA_MAX");
                if (funcAdicParametro != null)
                {
                    cuadraCampoAlturaMax = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_PARIDAD");
                if (funcAdicParametro != null)
                {
                    cuadraCampoIdParidad = funcAdicParametro.Valor;
                }
                #endregion

                #region Calle
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    calleEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_TABLA");
                if (funcAdicParametro != null)
                {
                    calleTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    calleCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_CAMPO_ID_CALLE");
                if (funcAdicParametro != null)
                {
                    calleCampoIdCalle = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_CAMPO_CODIGO");
                if (funcAdicParametro != null)
                {
                    calleCampoCodigo = funcAdicParametro.Valor;
                }
                #endregion

                #region Manzana
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    manzanaEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_TABLA");
                if (funcAdicParametro != null)
                {
                    manzanaTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_ID_MANZANA");
                if (funcAdicParametro != null)
                {
                    manzanaCampoIdManzana = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    manzanaCampoGeometry = funcAdicParametro.Valor;
                }
                #endregion

                #region Tolerancias
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ANGULO_TOLERANCIA");
                if (funcAdicParametro != null)
                {
                    int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_TOLERANCIA");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaTolerancia);
                }
                //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_BUFFER");
                //if (funcAdicParametro != null)
                //{
                //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distBuffer);
                //}
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DESPLAZAMIENTO_CALLE");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out desplazamientoCalle);
                }
                //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_LADO_OCHAVA");
                //if (funcAdicParametro != null)
                //{
                //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaLadoOchava);
                //}
                #endregion

                #region Fuentes
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
                }
                #endregion
                #endregion

                if (nombreCallesEnPlano)
                {
                    Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);

                    it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                    if ((1 / factorEscala) >= 5000)
                    {
                        textoFuenteTamanio = textoFuenteTamanio * 2.0;
                    }
                    float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                    PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                    //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                    string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                    int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                    BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;

                    double x = 0, y = 0;
                    double xMin = 9999999, yMin = 9999999;
                    double xMax = 0, yMax = 0;
                    double beta = 0;
                    double rotation = 0;
                    string sDistancia = string.Empty;
                    float xPdf = 0;
                    float yPdf = 0;
                    float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                    float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                    string textoCalle = string.Empty;

                    CuadraPlot[] aCuadraPlot = _cuadraPlotRepository.GetCuadraPlotByObjetoBase(componenteBase, idObjetoGraf, cuadraEsquema, cuadraTabla, cuadraCampoGeometry, cuadraCampoIdCuadra, cuadraCampoIdManzana, cuadraCampoIdCalle, cuadraCampoAlturaMin, cuadraCampoAlturaMax, cuadraCampoIdParidad, filtroGeografico);
                    if (aCuadraPlot != null && aCuadraPlot.Count() > 0)
                    {
                        List<long> lstDistinctIdCalle = aCuadraPlot.Select(p => p.IdCalle).Distinct().ToList();
                        foreach (var idCalle in lstDistinctIdCalle)
                        {
                            List<CuadraPlot> lstCuadraPlotByCalle = aCuadraPlot.Where(p => p.IdCalle == idCalle).OrderBy(p => p.AlturaMin).ToList();
                            int iCalle = 1;
                            int stepCalles = 4;
                            if (lstCuadraPlotByCalle.Count() > 16)
                            {
                                stepCalles = 8;
                            }
                            foreach (CuadraPlot cuadraPlot in lstCuadraPlotByCalle)
                            {
                                if ((iCalle % stepCalles) == 0 || lstCuadraPlotByCalle.Count() < stepCalles)
                                {
                                    //Dibujar Nombre de calle en plano
                                    PointF puntoMedio = new PointF();
                                    List<Lado> lados = new List<Lado>();
                                    Lado ladoMayor = new Lado();
                                    double anguloRotacionLadoMayor = GetAnguloRotacion(cuadraPlot.Geom, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);
                                    //Paso a radianes
                                    anguloRotacionLadoMayor = anguloRotacionLadoMayor * Math.PI / 180;
                                    if ((1 / factorEscala) >= 5000)
                                    {
                                        textoFuenteTamanio = textoFuenteTamanio / 2.0;
                                    }


                                    //ladoMayor = lstLadoCuadraFiltro.OrderByDescending(l => l.Distancia).FirstOrDefault();
                                    xMin = 9999999;
                                    yMin = 9999999;
                                    xMax = 0;
                                    yMax = 0;
                                    foreach (var segmento in ladoMayor.Segmentos)
                                    {
                                        #region Determinar xMin yMin xMax yMax
                                        if (xMin >= segmento.P1.X)
                                        {
                                            xMin = segmento.P1.X;
                                        }
                                        if (xMax < segmento.P1.X)
                                        {
                                            xMax = segmento.P1.X;
                                        }
                                        if (yMin >= segmento.P1.Y)
                                        {
                                            yMin = segmento.P1.Y;
                                        }
                                        if (yMax < segmento.P1.Y)
                                        {
                                            yMax = segmento.P1.Y;
                                        }
                                        if (xMin >= segmento.P2.X)
                                        {
                                            xMin = segmento.P2.X;
                                        }
                                        if (xMax < segmento.P2.X)
                                        {
                                            xMax = segmento.P2.X;
                                        }
                                        if (yMin >= segmento.P2.Y)
                                        {
                                            yMin = segmento.P2.Y;
                                        }
                                        if (yMax < segmento.P2.Y)
                                        {
                                            yMax = segmento.P2.Y;
                                        }
                                        #endregion
                                        #region Codigo comentado - Dibujo lado mayor de cuadra para Testing
                                        //if (ladoMayor.IdCuadra == 186487)
                                        //{
                                        //    float pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                                        //    float pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                                        //    float pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                                        //    float pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                                        //    it.BaseColor pdfContornoColorRnd = new it.BaseColor(Color.Black.R, Color.Black.G, Color.Black.B);
                                        //    PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, pdfContornoGrosor, pdfContornoColorRnd);
                                        //}
                                        #endregion
                                    }

                                    CallePlot[] aCallePlot = _callePlotRepository.GetCallePlotByIdCalle(calleEsquema, calleTabla, calleCampoIdCalle, calleCampoNombre, calleCampoCodigo, cuadraPlot.IdCalle);
                                    if (aCallePlot != null && aCallePlot.Length > 0)
                                    {
                                        CallePlot callePlot = aCallePlot[0];

                                        string alturas = " [";
                                        if (cuadraPlot.AlturaMin == cuadraPlot.AlturaMax)
                                        {
                                            alturas = alturas + cuadraPlot.AlturaMin.ToString() + "]";
                                        }
                                        else
                                        {
                                            alturas = alturas + cuadraPlot.AlturaMin.ToString() + "-" + cuadraPlot.AlturaMax.ToString() + "]";
                                        }
                                        textoCalle = callePlot.Nombre + alturas;
                                        //textoCalle = "(" + callePlot.Codigo + ") " + callePlot.Nombre + alturas;

                                        #region Ubico el texto
                                        rotation = ladoMayor.Angulo;

                                        beta = rotation;
                                        if (rotation < 0)
                                        {
                                            beta = rotation + 360;
                                        }

                                        double alfa = beta;

                                        double textRotation = alfa;

                                        if (alfa >= 0 && alfa <= 90)
                                        {
                                            beta = 90 - alfa;
                                            //textRotation = alfa + 90 + 180;
                                        }
                                        else if (alfa > 90 && alfa <= 180)
                                        {
                                            beta = alfa - 90;
                                            //textRotation = beta;
                                        }
                                        else if (alfa > 180 && alfa <= 270)
                                        {
                                            beta = 270 - alfa;
                                            //textRotation = alfa - 90;
                                        }
                                        else if (alfa > 270 && alfa <= 360)
                                        {
                                            beta = alfa - 270;
                                            //textRotation = beta;
                                        }

                                        double betaRad = beta * Math.PI / 180.0;

                                        x = xMin + (xMax - xMin) / 2;
                                        y = yMin + (yMax - yMin) / 2;

                                        double desplazamientoX = desplazamientoCalle * Math.Cos(betaRad);
                                        double desplazamientoY = desplazamientoCalle * Math.Sin(betaRad);

                                        double x1Des = x + desplazamientoCalle * Math.Cos(betaRad);
                                        double y1Des = y + desplazamientoCalle * Math.Sin(betaRad);
                                        double x2Des = x - desplazamientoCalle * Math.Cos(betaRad);
                                        double y2Des = y - desplazamientoCalle * Math.Sin(betaRad);

                                        x1Des = x + desplazamientoX;
                                        x2Des = x - desplazamientoX;
                                        if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
                                        {
                                            y1Des = y - desplazamientoY;
                                            y2Des = y + desplazamientoY;
                                        }
                                        else if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
                                        {
                                            y1Des = y + desplazamientoY;
                                            y2Des = y - desplazamientoY;
                                        }

                                        //TODO Test - Dibujo ptos para testing
                                        Color colorTexto2 = System.Drawing.ColorTranslator.FromHtml("#006400");
                                        it.BaseColor pdfColor2 = new it.BaseColor(colorTexto2.R, colorTexto2.G, colorTexto2.B);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x1Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y1Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x2Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y2Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColor2, (float)0.1, pdfColor2, 50);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                        //PDFUtilities.DrawPDFText(pdfContentByte, (Math.Truncate(100 * beta) / 100).ToString(), GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), pdfbaseFont, pdfFontSize, colorTexto, lado.Angulo);
                                        ////

                                        ManzanaPlot[] aManzanaPlotDes = _manzanaPlotRepository.GetManzanaPlotByCoords(manzanaEsquema, manzanaTabla, manzanaCampoGeometry, manzanaCampoIdManzana, x1Des, y1Des);
                                        if (aManzanaPlotDes != null && aManzanaPlotDes.Length > 0 && cuadraPlot.IdManzana == aManzanaPlotDes[0].FeatId)
                                        {
                                            x = x2Des;
                                            y = y2Des;
                                        }
                                        else
                                        {
                                            x = x1Des;
                                            y = y1Des;
                                        }
                                        xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
                                        yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado = Rotate(xPdf, yPdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            double anguloPdf = rotation - (anguloRotacionFiltro * 180 / Math.PI);
                                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTexto, anguloPdf);
                                        }
                                        else
                                        {
                                            //PDFUtilities.DrawPDFCircle(pdfContentByte, xPdf, yPdf, it.Utilities.MillimetersToPoints((float)0.1), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, xPdf, yPdf, pdfbaseFont, pdfFontSize, colorTexto, rotation);
                                        }
                                        #endregion
                                    }
                                    if (lstCuadraPlotByCalle.Count() < stepCalles)
                                    {
                                        break;
                                    }
                                }
                                iCalle++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return ret;
        }

        private bool FuncEspDibujarInformeAnualReferenciaEA(int idFuncionAdicional, it.Document pdfDoc, PdfContentByte pdfContentByte, Plantilla plantilla, List<Layer> lstLayers, Componente componenteBase, string idObjetoGraf, List<string> lstCoordenadas, string anio)
        {
            bool ret = true;
            try
            {
                #region Inicializacion de Variables
                int filtroGeografico = 1;
                float xMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMin);
                float yMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMin);
                float xMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
                float yMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMax);
                float espaciado = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaEspaciado);

                //Definicion de vbles y defaults

                //Agua y Cloaca. Cambian sus valores en los parametros de la DB
                long expansionComponenteId = 2302;
                string expansionEsquema = "AYSA_GERED_DEV";
                string expansionTabla = "VW_ZEXPANA";
                string expansionCampoGeometry = "GEOMETRY";
                string expansionCampoId = "ID_REFERENCIA";
                string expansionCampoNombre = "NOMBRE";
                string expansionTituloId = "Identificador";
                string expansionTituloNombre = "Nombre";

                //Agua y Cloaca. Cambian sus valores en los parametros de la DB
                long opctMpgComponenteId = 2303;
                string opctMpgEsquema = "AYSA_GERED_DEV";
                string opctMpgTabla = "VW_OPCT_MPG_AT";
                string opctMpgCampoGeometry = "GEOMETRY";
                string opctMpgCampoId = "ID_REFERENCIA";
                string opctMpgCampoNombre = "NOMBRE";
                string opctMpgTituloId = "Identificador";
                string opctMpgTituloNombre = "Nombre";

                //Agua
                long aDesvinComponenteId = 2304;
                string aDesvinEsquema = "AYSA_GERED_DEV";
                string aDesvinTabla = "VW_A_DESVIN";
                string aDesvinCampoGeometry = "GEOMETRY";
                string aDesvinCampoId = "ID_REFERENCIA";
                string aDesvinCampoNombre = "NOMBRE";
                string aDesvinTituloId = "Identificador";
                string aDesvinTituloNombre = "Nombre";

                long aCompRegComponenteId = 2305;
                string aCompRegEsquema = "AYSA_GERED_DEV";
                string aCompRegTabla = "VW_ACOMPREG";
                string aCompRegCampoGeometry = "GEOMETRY";
                string aCompRegCampoId = "ID_REFERENCIA";
                string aCompRegCampoNombre = string.Empty;
                //string aCompRegCampoNombre = "NOMBRE";
                string aCompRegTituloId = "Identificador";
                //string aCompRegTituloNombre = "Nombre";
                string aCompRegTituloNombre = string.Empty;

                //Cloaca
                long cDesvinComponenteId = 2315;
                string cDesvinEsquema = "AYSA_GERED_DEV";
                string cDesvinTabla = "VW_C_DESVIN";
                string cDesvinCampoGeometry = "GEOMETRY";
                string cDesvinCampoId = "ID_REFERENCIA";
                string cDesvinCampoNombre = "NOMBRE";
                string cDesvinTituloId = "Identificador";
                string cDesvinTituloNombre = "Nombre";

                long cCompRegComponenteId = 2316;
                string cCompRegEsquema = "AYSA_GERED_DEV";
                string cCompRegTabla = "VW_CCOMPREG";
                string cCompRegCampoGeometry = "GEOMETRY";
                string cCompRegCampoId = "ID_REFERENCIA";
                string cCompRegCampoNombre = "NOMBRE";
                string cCompRegTituloId = "Identificador";
                string cCompRegTituloNombre = "Nombre";


                float textoEspaciadoMM = (float)1.41083;
                string textoFuenteNombre = "Arial";
                double textoFuenteTamanio = 0.8;
                string textoFuenteEstilo = "0,0,0,0";
                string textoColor = "#000000";

                string textoCalleFuenteNombre = "Arial";
                double textoCalleFuenteTamanio = 0.8;
                string textoCalleFuenteEstilo = "0,0,0,0";
                string textoCalleColor = "#000000";

                bool nombreCallesEnPlano = false;
                #endregion

                #region NumberFormat
                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.CurrencyDecimalDigits = 4;
                numberFormat.CurrencyDecimalSeparator = ".";
                numberFormat.NumberDecimalDigits = 4;
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.PercentDecimalDigits = 2;
                numberFormat.PercentDecimalSeparator = ".";
                #endregion

                #region Parametros de la DB
                //Agua y Cloaca
                #region Expansion
                FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out expansionComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    expansionEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TABLA");
                if (funcAdicParametro != null)
                {
                    expansionTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    expansionCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    expansionCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    expansionCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    expansionTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    expansionTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua y Cloaca
                #region OPCT_MPG_AT
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out opctMpgComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    opctMpgEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TABLA");
                if (funcAdicParametro != null)
                {
                    opctMpgTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    opctMpgTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    opctMpgTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua
                #region A_DESVIN
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out aDesvinComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    aDesvinEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TABLA");
                if (funcAdicParametro != null)
                {
                    aDesvinTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    aDesvinTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aDesvinTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua
                #region ACOMPREG
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out aCompRegComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    aCompRegEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TABLA");
                if (funcAdicParametro != null)
                {
                    aCompRegTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    aCompRegCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    aCompRegCampoId = funcAdicParametro.Valor;
                }
                //tfs 9546
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aCompRegCampoNombre = funcAdicParametro.Valor;
                }
                //aCompRegCampoNombre = string.Empty;

                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    aCompRegTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aCompRegTituloNombre = funcAdicParametro.Valor;
                }
                #endregion

                //Cloaca
                #region C_DESVIN
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out cDesvinComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    cDesvinEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TABLA");
                if (funcAdicParametro != null)
                {
                    cDesvinTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    cDesvinTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cDesvinTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Cloaca
                #region CCOMPREG
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out cCompRegComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    cCompRegEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TABLA");
                if (funcAdicParametro != null)
                {
                    cCompRegTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    cCompRegCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    cCompRegCampoId = funcAdicParametro.Valor;
                }
                // tfs 9546
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cCompRegCampoNombre = funcAdicParametro.Valor;
                }
                // cCompRegCampoNombre = string.Empty;
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    cCompRegTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cCompRegTituloNombre = funcAdicParametro.Valor;
                }
                #endregion

                #region Font
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
                if (funcAdicParametro != null)
                {
                    float.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoEspaciadoMM);
                }
                #endregion

                #region Fuentes para nombres de Calles
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoCalleFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoCalleColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoCalleFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoCalleFuenteTamanio);
                }
                #endregion

                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "NOMBRE_CALLES_EN_PLANO");
                if (funcAdicParametro != null)
                {
                    //int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
                    nombreCallesEnPlano = (funcAdicParametro.Valor == "1" ? true : false);
                }

                #endregion

                Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);

                // DibujarReferencias - DrawPDFRectangle de mins maxs
                //DrawPDFRectangle(pdfContentByte, xMinReferencia, yMinReferencia, xMaxReferencia - xMinReferencia, yMaxReferencia - yMinReferencia, new it.BaseColor(Color.Gray.R, Color.Gray.G, Color.Gray.B), 1.0f, null, null);

                Color colorReferencia = System.Drawing.ColorTranslator.FromHtml(plantilla.ReferenciaColor);
                it.BaseColor pdfColorTexto = new it.BaseColor(colorReferencia.R, colorReferencia.G, colorReferencia.B);

                int alignment = it.Element.ALIGN_LEFT + it.Element.ALIGN_BOTTOM;

                float pdfFontSize = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaFuenteTamanio);

                PDFUtilities.RegisterBaseFont(plantilla.ReferenciaFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStyle = plantilla.ReferenciaFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(plantilla.ReferenciaFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                float x1c = xMinReferencia;
                float y1c = yMaxReferencia - espaciado - pdfFontSize;
                float x2c = 0;
                float y2c = 0;

                x1c += pdfFontSize / 2;

                //string[] aFontStyleTitRef = plantilla.ReferenciaFuenteEstilo.Split(',');
                //int pdfFontStyleTitRef = aFontStyleTitRef.Select(x => Convert.ToInt32(x)).Sum();
                int pdfFontStyleTitRef = 1;
                BaseFont pdfbaseFontTitRef = it.FontFactory.GetFont(plantilla.ReferenciaFuenteNombre, pdfFontSize, pdfFontStyleTitRef, pdfColorTexto).BaseFont;
                it.Font pdfFontTitRef = new it.Font(pdfbaseFontTitRef, pdfFontSize, pdfFontStyleTitRef, pdfColorTexto);

                x2c = x1c + pdfFontSize / 2;
                PDFUtilities.DrawPDFText(pdfContentByte, "REFERENCIAS", x2c, y1c, pdfFontTitRef, pdfFontSize, alignment);
                y1c -= (pdfFontSize + espaciado) * 2;
                //tfs 9546
                if (nombreCallesEnPlano)
                {
                    Color colorTextoCalle = System.Drawing.ColorTranslator.FromHtml(textoCalleColor);
                    it.BaseColor pdfColorTextoCalle = new it.BaseColor(colorTextoCalle.R, colorTextoCalle.G, colorTextoCalle.B);
                    //float pdfFontSizeCalle = it.Utilities.MillimetersToPoints((float)textoCalleFuenteTamanio);
                    PDFUtilities.RegisterBaseFont(textoCalleFuenteNombre, pdfFontSize);
                    //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                    string[] aFontStyleCalle = textoCalleFuenteEstilo.Split(',');
                    int pdfFontStyleCalle = aFontStyleCalle.Select(x => Convert.ToInt32(x)).Sum();
                    BaseFont pdfbaseFontCalle = it.FontFactory.GetFont(textoCalleFuenteNombre, pdfFontSize, pdfFontStyleCalle, pdfColorTextoCalle).BaseFont;
                    it.Font pdfFontCalle = new it.Font(pdfbaseFontCalle, pdfFontSize, pdfFontStyleCalle, pdfColorTextoCalle);
                    PDFUtilities.DrawPDFText(pdfContentByte, "CALLE", x1c, y1c, pdfFontCalle, pdfFontSize, alignment);

                    x2c = x1c + pdfFontSize * 4;
                    PDFUtilities.DrawPDFText(pdfContentByte, "NOMBRES DE CALLE", x2c, y1c, pdfFont, pdfFontSize, alignment);
                }

                y1c -= (pdfFontSize + espaciado) * 2;

                //int col = 1;
                //foreach (Layer layer in plantilla.Layers.OrderByDescending(l => l.Orden))
                foreach (Layer layer in lstLayers.OrderByDescending(l => l.Orden))
                {
                    //Si el contorno grosor es mayor a uno le pone uno porq sino se empasta y no se ve lo del medio
                    //float pdfContornoGrosor = layer.ContornoGrosor != null && (float)layer.ContornoGrosor.Value > 0 ? 1 : 0;
                    float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;

                    it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                    it.BaseColor pdfRellenoColor = GetAlphaColor(layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent, layer.RellenoTransparencia);
                    PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);
                    //PdfPatternPainter pdfPatternPainter = GetPattern(pdfContentByte, 4, 4, 0.2);

                    float pdfFontSizeTabla = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                    string lineDash = layer.Dash;

                    string texto = layer.Nombre;

                    x2c = x1c + pdfFontSize * 2;
                    if (layer.PuntoRepresentacion == 0)
                    {
                        if (layer.Relleno)
                        {
                            y2c = y1c + pdfFontSize;
                            PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y1c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                        }
                        else if (layer.Contorno)
                        {
                            y2c = y1c + (pdfFontSize / 2);
                            PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y2c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, null, null);
                        }
                    }
                    else if (layer.PuntoRepresentacion == 2)
                    {
                        if (layer.PuntoImagen != null)
                        {
                            float puntoAnchoPts = pdfFontSize;
                            float puntoAltoPts = pdfFontSize;
                            PDFUtilities.DrawPDFImage(pdfContentByte, layer.PuntoImagen, x1c, y1c, puntoAnchoPts, puntoAltoPts);
                        }
                    }
                    else if (layer.PuntoRepresentacion == 1)
                    {
                        float puntoAnchoPts = pdfFontSize;
                        float puntoAltoPts = pdfFontSize;
                        if (layer.PuntoPredeterminado == 1)
                        {
                            //Circulo
                            float radio = pdfFontSize / 2;
                            PDFUtilities.DrawPDFCircle(pdfContentByte, (x1c + pdfFontSize / 2), (y1c + pdfFontSize / 2), radio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                        }
                        else if (layer.PuntoPredeterminado == 2)
                        {
                            //Cuadrado
                            PDFUtilities.DrawPDFRectangle(pdfContentByte, x1c, y1c, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                        }
                    }
                    x2c += pdfFontSize / 2;
                    float yTexto = y1c;
                    PDFUtilities.DrawPDFText(pdfContentByte, texto, x2c, yTexto, pdfFont, pdfFontSize, alignment);

                    int cantRenglonAdd = 0;
                    if (layer.ComponenteId == aCompRegComponenteId || layer.ComponenteId == cCompRegComponenteId || layer.ComponenteId == expansionComponenteId || layer.ComponenteId == opctMpgComponenteId || layer.ComponenteId == aDesvinComponenteId || layer.ComponenteId == cDesvinComponenteId)
                    {
                        #region Tabla de Datos
                        #region Seteo de parametros esquem tabla etc
                        string esquema = expansionEsquema;
                        string tabla = expansionTabla;
                        string campoGeometry = expansionCampoGeometry;
                        string campoId = expansionCampoId;
                        string campoNombre = expansionCampoNombre;
                        string tituloId = expansionTituloId;
                        string tituloNombre = expansionTituloNombre;
                        if (layer.ComponenteId == expansionComponenteId)
                        {
                            esquema = expansionEsquema;
                            tabla = expansionTabla;
                            campoGeometry = expansionCampoGeometry;
                            campoId = expansionCampoId;
                            campoNombre = expansionCampoNombre;
                            tituloId = expansionTituloId;
                            tituloNombre = expansionTituloNombre;
                        }
                        else if (layer.ComponenteId == opctMpgComponenteId)
                        {
                            esquema = opctMpgEsquema;
                            tabla = opctMpgTabla;
                            campoGeometry = opctMpgCampoGeometry;
                            campoId = opctMpgCampoId;
                            campoNombre = opctMpgCampoNombre;
                            tituloId = opctMpgTituloId;
                            tituloNombre = opctMpgTituloNombre;
                        }
                        else if (layer.ComponenteId == aDesvinComponenteId)
                        {
                            esquema = aDesvinEsquema;
                            tabla = aDesvinTabla;
                            campoGeometry = aDesvinCampoGeometry;
                            campoId = aDesvinCampoId;
                            campoNombre = aDesvinCampoNombre;
                            tituloId = aDesvinTituloId;
                            tituloNombre = aDesvinTituloNombre;
                        }
                        else if (layer.ComponenteId == aCompRegComponenteId)
                        {
                            esquema = aCompRegEsquema;
                            tabla = aCompRegTabla;
                            campoGeometry = aCompRegCampoGeometry;
                            campoId = aCompRegCampoId;
                            campoNombre = aCompRegCampoNombre;
                            tituloId = aCompRegTituloId;
                            tituloNombre = aCompRegTituloNombre;
                        }
                        else if (layer.ComponenteId == cDesvinComponenteId)
                        {
                            esquema = cDesvinEsquema;
                            tabla = cDesvinTabla;
                            campoGeometry = cDesvinCampoGeometry;
                            campoId = cDesvinCampoId;
                            campoNombre = cDesvinCampoNombre;
                            tituloId = cDesvinTituloId;
                            tituloNombre = cDesvinTituloNombre;
                        }
                        else if (layer.ComponenteId == cCompRegComponenteId)
                        {
                            esquema = cCompRegEsquema;
                            tabla = cCompRegTabla;
                            campoGeometry = cCompRegCampoGeometry;
                            campoId = cCompRegCampoId;
                            campoNombre = cCompRegCampoNombre;
                            tituloId = cCompRegTituloId;
                            tituloNombre = cCompRegTituloNombre;
                        }
                        //prefijoId = "E"; //no se esta usando el prefijo, ya esta puesto en el campo idReferencia de la vista 
                        #endregion
                        ExpansionPlot[] aExpansionPlot = _expansionPlotRepository.GetExpansionPlotByObjetoBase(componenteBase, esquema, tabla, campoGeometry, campoId, campoNombre, filtroGeografico, idObjetoGraf, layer.FiltroIdAtributo, anio);
                        if (aExpansionPlot != null && aExpansionPlot.Length > 0)
                        {
                            y1c = y1c - (pdfFontSize - espaciado) / 2;
                            if (y1c <= yMinReferencia)
                            {
                                pdfDoc.NewPage();
                                x1c = xMinReferencia + pdfFontSize / 2;
                                y1c = yMaxReferencia - espaciado - pdfFontSize;
                            }
                            PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSizeTabla);
                            //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                            string[] aFontStyleTabla = textoFuenteEstilo.Split(',');
                            int pdfFontStyleTabla = aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
                            Color colorTabla = System.Drawing.ColorTranslator.FromHtml(textoColor);
                            it.BaseColor pdfColorTextoTabla = new it.BaseColor(colorTabla.R, colorTabla.G, colorTabla.B);
                            BaseFont pdfbaseFontTabla = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSizeTabla, pdfFontStyleTabla, pdfColorTextoTabla).BaseFont;
                            it.Font pdfFontTabla = new it.Font(pdfbaseFontTabla, pdfFontSizeTabla, pdfFontStyleTabla, pdfColorTextoTabla);

                            float tableWidth = xMaxReferencia - xMinReferencia - (pdfFontSizeTabla / 2);

                            //Tabla para datos
                            int cols = 2;
                            PdfPTable pdfTable;
                            if (campoNombre == string.Empty)
                            {
                                cols = 1;
                                pdfTable = new PdfPTable(cols);
                                pdfTable.TotalWidth = (float)(tableWidth * 0.5);
                                float[] widths = new float[1] { (float)(tableWidth * 0.5) };
                                pdfTable.SetWidths(widths);
                            }
                            else
                            {
                                cols = 2;
                                pdfTable = new PdfPTable(cols);
                                pdfTable.TotalWidth = tableWidth;
                                float[] widths = { (float)(tableWidth * 0.2), (float)(tableWidth * 0.8) };
                                pdfTable.SetWidths(widths);
                            }

                            bool drawBorder = true;

                            PdfPCell pdfCell = new PdfPCell();
                            int alignmentCol1 = it.Element.ALIGN_LEFT;
                            int alignmentCol2 = it.Element.ALIGN_LEFT;

                            List<ExpansionPlot> lstExpansionPlot = aExpansionPlot.OrderBy(e => e.Id).ToList();
                            int rows = lstExpansionPlot.Count + 1;
                            int iRow = 1;
                            float yRow = y1c;

                            it.BaseColor pdfColorCeldasTitulosTabla = new it.BaseColor(169, 169, 169);
                            pdfCell = new PdfPCell();
                            texto = tituloId;
                            pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol1, textoEspaciadoMM, drawBorder);
                            pdfCell.UseBorderPadding = true;
                            if (campoNombre != string.Empty)
                            {
                                pdfCell.BorderWidthRight = 0;
                            }
                            pdfCell.BackgroundColor = pdfColorCeldasTitulosTabla;
                            pdfTable.AddCell(pdfCell);
                            if (campoNombre != string.Empty)
                            {
                                pdfCell = new PdfPCell();
                                texto = tituloNombre;
                                pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol2, textoEspaciadoMM, drawBorder);
                                pdfCell.UseBorderPadding = true;
                                pdfCell.BorderWidthLeft = 0;
                                pdfCell.BackgroundColor = pdfColorCeldasTitulosTabla;
                                pdfTable.AddCell(pdfCell);
                            }
                            foreach (var expansionPlot in lstExpansionPlot)
                            {
                                pdfCell = new PdfPCell();
                                texto = expansionPlot.IdReferencia;
                                pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol1, textoEspaciadoMM, drawBorder);
                                pdfCell.UseBorderPadding = true;
                                if (campoNombre != string.Empty)
                                {
                                    pdfCell.BorderWidthRight = 0;
                                }
                                pdfTable.AddCell(pdfCell);
                                if (campoNombre != string.Empty)
                                {
                                    pdfCell = new PdfPCell();
                                    texto = expansionPlot.Nombre;
                                    pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol2, textoEspaciadoMM, drawBorder);
                                    pdfCell.UseBorderPadding = true;
                                    pdfCell.BorderWidthLeft = 0;
                                    pdfTable.AddCell(pdfCell);

                                    //verificar si ocupa mas de una linea
                                    float colNombreWidth = (float)(tableWidth * 0.8);
                                    float sizeTextNombre = pdfbaseFontTabla.GetWidthPoint(texto, pdfFontSizeTabla);
                                    if (sizeTextNombre > colNombreWidth)
                                    {
                                        cantRenglonAdd++;
                                    }
                                }
                                iRow++;
                                yRow = yRow - (pdfFontSizeTabla + textoEspaciadoMM);
                                if (yRow <= yMinReferencia)
                                {
                                    pdfTable.WriteSelectedRows(0, -1, 0, iRow, x1c, y1c - (textoEspaciadoMM * 2), pdfContentByte);
                                    pdfDoc.NewPage();
                                    x1c = xMinReferencia + pdfFontSize / 2;
                                    y1c = yMaxReferencia - espaciado - pdfFontSize;
                                    yRow = y1c;
                                    if (campoNombre == string.Empty)
                                    {
                                        cols = 1;
                                        pdfTable = new PdfPTable(cols);
                                        pdfTable.TotalWidth = (float)(tableWidth * 0.5);
                                        float[] widths = new float[1] { (float)(tableWidth * 0.5) };
                                        pdfTable.SetWidths(widths);
                                    }
                                    else
                                    {
                                        cols = 2;
                                        pdfTable = new PdfPTable(cols);
                                        pdfTable.TotalWidth = tableWidth;
                                        float[] widths = { (float)(tableWidth * 0.2), (float)(tableWidth * 0.8) };
                                        pdfTable.SetWidths(widths);
                                    }
                                    iRow = 0;
                                }
                            }
                            pdfTable.WriteSelectedRows(0, -1, x1c, y1c - (textoEspaciadoMM * 2), pdfContentByte);

                            y1c = y1c - (rows * (pdfFontSizeTabla + textoEspaciadoMM));

                        }
                        #endregion
                    }

                    y1c -= (pdfFontSize + espaciado) * 2;
                    if (cantRenglonAdd > 0)
                    {
                        y1c -= (pdfFontSizeTabla + textoEspaciadoMM) * cantRenglonAdd;
                    }
                    if (y1c <= yMinReferencia)
                    {
                        pdfDoc.NewPage();
                        x1c = xMinReferencia + pdfFontSize / 2;
                        y1c = yMaxReferencia - espaciado - pdfFontSize;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return ret;
        }

        private bool FuncEspDibujarInformeAnualReferenciaEAPagina2(int idFuncionAdicional, it.Document pdfDoc, PdfContentByte pdfContentByte, Plantilla plantilla, List<Layer> lstLayers, Componente componenteBase, string idObjetoGraf, List<string> lstCoordenadas, string anio, int cantPaginas)
        {
            bool ret = true;
            try
            {
                #region Inicializacion de Variables
                int filtroGeografico = 1;
                float xMinImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMin);
                float yMinImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMin);
                float xMaxImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
                float yMaxImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMax);

                float xMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMin);
                float yMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMin);
                float xMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
                float yMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMax);

                float espaciado = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaEspaciado);

                float xPagina = it.Utilities.MillimetersToPoints((float)793);
                float yPagina = it.Utilities.MillimetersToPoints((float)586);

                //Definicion de vbles y defaults

                //Agua y Cloaca. Cambian sus valores en los parametros de la DB
                long expansionComponenteId = 2302;
                string expansionEsquema = "AYSA_GERED_DEV";
                string expansionTabla = "VW_ZEXPANA";
                string expansionCampoGeometry = "GEOMETRY";
                string expansionCampoId = "ID_REFERENCIA";
                string expansionCampoNombre = "NOMBRE";
                string expansionTituloId = "Identificador";
                string expansionTituloNombre = "Nombre";

                //Agua y Cloaca. Cambian sus valores en los parametros de la DB
                long opctMpgComponenteId = 2303;
                string opctMpgEsquema = "AYSA_GERED_DEV";
                string opctMpgTabla = "VW_OPCT_MPG_AT";
                string opctMpgCampoGeometry = "GEOMETRY";
                string opctMpgCampoId = "ID_REFERENCIA";
                string opctMpgCampoNombre = "NOMBRE";
                string opctMpgTituloId = "Identificador";
                string opctMpgTituloNombre = "Nombre";

                //Agua
                long aDesvinComponenteId = 2304;
                string aDesvinEsquema = "AYSA_GERED_DEV";
                string aDesvinTabla = "VW_A_DESVIN";
                string aDesvinCampoGeometry = "GEOMETRY";
                string aDesvinCampoId = "ID_REFERENCIA";
                string aDesvinCampoNombre = "NOMBRE";
                string aDesvinTituloId = "Identificador";
                string aDesvinTituloNombre = "Nombre";

                long aCompRegComponenteId = 2305;
                string aCompRegEsquema = "AYSA_GERED_DEV";
                string aCompRegTabla = "VW_ACOMPREG";
                string aCompRegCampoGeometry = "GEOMETRY";
                string aCompRegCampoId = "ID_REFERENCIA";
                string aCompRegCampoNombre = string.Empty;
                //string aCompRegCampoNombre = "NOMBRE";
                string aCompRegTituloId = "Identificador";
                //string aCompRegTituloNombre = "Nombre";
                string aCompRegTituloNombre = string.Empty;

                //Cloaca
                long cDesvinComponenteId = 2315;
                string cDesvinEsquema = "AYSA_GERED_DEV";
                string cDesvinTabla = "VW_C_DESVIN";
                string cDesvinCampoGeometry = "GEOMETRY";
                string cDesvinCampoId = "ID_REFERENCIA";
                string cDesvinCampoNombre = "NOMBRE";
                string cDesvinTituloId = "Identificador";
                string cDesvinTituloNombre = "Nombre";

                long cCompRegComponenteId = 2316;
                string cCompRegEsquema = "AYSA_GERED_DEV";
                string cCompRegTabla = "VW_CCOMPREG";
                string cCompRegCampoGeometry = "GEOMETRY";
                string cCompRegCampoId = "ID_REFERENCIA";
                string cCompRegCampoNombre = "NOMBRE";
                string cCompRegTituloId = "Identificador";
                string cCompRegTituloNombre = "Nombre";


                float textoEspaciadoMM = (float)1.41083;
                string textoFuenteNombre = "Arial";
                double textoFuenteTamanio = 0.8;
                string textoFuenteEstilo = "0,0,0,0";
                string textoColor = "#000000";

                string textoCalleFuenteNombre = "Arial";
                double textoCalleFuenteTamanio = 0.8;
                string textoCalleFuenteEstilo = "0,0,0,0";
                string textoCalleColor = "#000000";

                bool nombreCallesEnPlano = false;
                #endregion

                #region NumberFormat
                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.CurrencyDecimalDigits = 4;
                numberFormat.CurrencyDecimalSeparator = ".";
                numberFormat.NumberDecimalDigits = 4;
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.PercentDecimalDigits = 2;
                numberFormat.PercentDecimalSeparator = ".";
                #endregion

                #region Parametros de la DB
                #region vbles xPagina, yPagina
                FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "X_PAGINA");
                if (funcAdicParametro != null)
                {
                    float.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xPagina);
                    xPagina = it.Utilities.MillimetersToPoints((float)xPagina);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "Y_PAGINA");
                if (funcAdicParametro != null)
                {
                    float.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yPagina);
                    yPagina = it.Utilities.MillimetersToPoints((float)yPagina);
                }
                #endregion
                //Agua y Cloaca
                #region Expansion
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out expansionComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    expansionEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TABLA");
                if (funcAdicParametro != null)
                {
                    expansionTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    expansionCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    expansionCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    expansionCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    expansionTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    expansionTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua y Cloaca
                #region OPCT_MPG_AT
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out opctMpgComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    opctMpgEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TABLA");
                if (funcAdicParametro != null)
                {
                    opctMpgTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    opctMpgTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    opctMpgTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua
                #region A_DESVIN
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out aDesvinComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    aDesvinEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TABLA");
                if (funcAdicParametro != null)
                {
                    aDesvinTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    aDesvinTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aDesvinTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua
                #region ACOMPREG
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out aCompRegComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    aCompRegEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TABLA");
                if (funcAdicParametro != null)
                {
                    aCompRegTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    aCompRegCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    aCompRegCampoId = funcAdicParametro.Valor;
                }
                //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_NOMBRE");
                //if (funcAdicParametro != null)
                //{
                //    aCompRegCampoNombre = funcAdicParametro.Valor;
                //}
                aCompRegCampoNombre = string.Empty;
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    aCompRegTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aCompRegTituloNombre = funcAdicParametro.Valor;
                }
                #endregion

                //Cloaca
                #region C_DESVIN
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out cDesvinComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    cDesvinEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TABLA");
                if (funcAdicParametro != null)
                {
                    cDesvinTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    cDesvinTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cDesvinTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Cloaca
                #region CCOMPREG
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out cCompRegComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    cCompRegEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TABLA");
                if (funcAdicParametro != null)
                {
                    cCompRegTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    cCompRegCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    cCompRegCampoId = funcAdicParametro.Valor;
                }
                //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_NOMBRE");
                //if (funcAdicParametro != null)
                //{
                //    cCompRegCampoNombre = funcAdicParametro.Valor;
                //}
                cCompRegCampoNombre = string.Empty;
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    cCompRegTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cCompRegTituloNombre = funcAdicParametro.Valor;
                }
                #endregion

                #region Font
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
                if (funcAdicParametro != null)
                {
                    float.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoEspaciadoMM);
                }
                #endregion

                #region Fuentes para nombres de Calles
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoCalleFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoCalleColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoCalleFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoCalleFuenteTamanio);
                }
                #endregion

                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "NOMBRE_CALLES_EN_PLANO");
                if (funcAdicParametro != null)
                {
                    //int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
                    nombreCallesEnPlano = (funcAdicParametro.Valor == "1" ? true : false);
                }
                #endregion

                Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);

                // DibujarReferencias - DrawPDFRectangle de mins maxs
                //DrawPDFRectangle(pdfContentByte, xMinReferencia, yMinReferencia, xMaxReferencia - xMinReferencia, yMaxReferencia - yMinReferencia, new it.BaseColor(Color.Gray.R, Color.Gray.G, Color.Gray.B), 1.0f, null, null);

                Color colorReferencia = System.Drawing.ColorTranslator.FromHtml(plantilla.ReferenciaColor);
                it.BaseColor pdfColorTexto = new it.BaseColor(colorReferencia.R, colorReferencia.G, colorReferencia.B);

                int alignment = it.Element.ALIGN_LEFT + it.Element.ALIGN_BOTTOM;

                float pdfFontSize = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaFuenteTamanio);

                PDFUtilities.RegisterBaseFont(plantilla.ReferenciaFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStyle = plantilla.ReferenciaFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(plantilla.ReferenciaFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                float xTxt = xMinReferencia + pdfFontSize / 2;
                float yTxt = yMaxReferencia - espaciado - pdfFontSize;
                PDFUtilities.DrawPDFText(pdfContentByte, "Ver Referencias en pág. 2", xTxt, yTxt, pdfFont, pdfFontSize, alignment);

                int iPagina = 1;

                string sPagina = "pág." + iPagina.ToString() + "/" + cantPaginas.ToString();
                PDFUtilities.DrawPDFText(pdfContentByte, sPagina, xPagina, yPagina, pdfFont, pdfFontSize, alignment);

                pdfDoc.NewPage();
                iPagina++;
                sPagina = "pág." + iPagina.ToString() + "/" + cantPaginas.ToString();
                PDFUtilities.DrawPDFText(pdfContentByte, sPagina, xPagina, yPagina - (pdfFontSize + espaciado) * 2, pdfFont, pdfFontSize, alignment);

                int cantColumnasMax = Convert.ToInt32(Math.Floor((xMaxImpresion - xMinImpresion) / (xMaxReferencia - xMinReferencia)));
                int col = 1;
                //float x1c = xMinReferencia;
                //float y1c = yMaxReferencia - espaciado - pdfFontSize;
                float x1c = xMinImpresion;
                float y1c = yMaxImpresion - espaciado - pdfFontSize;
                float x2c = 0;
                float y2c = 0;

                x1c += pdfFontSize / 2;

                //string[] aFontStyleTitRef = plantilla.ReferenciaFuenteEstilo.Split(',');
                //int pdfFontStyleTitRef = aFontStyleTitRef.Select(x => Convert.ToInt32(x)).Sum();
                int pdfFontStyleTitRef = 1;
                BaseFont pdfbaseFontTitRef = it.FontFactory.GetFont(plantilla.ReferenciaFuenteNombre, pdfFontSize, pdfFontStyleTitRef, pdfColorTexto).BaseFont;
                it.Font pdfFontTitRef = new it.Font(pdfbaseFontTitRef, pdfFontSize, pdfFontStyleTitRef, pdfColorTexto);

                x2c = x1c + pdfFontSize / 2;
                PDFUtilities.DrawPDFText(pdfContentByte, "REFERENCIAS", x2c, y1c, pdfFontTitRef, pdfFontSize, alignment);
                y1c = y1c - (pdfFontSize + espaciado) * 2;

                //foreach (Layer layer in plantilla.Layers.OrderByDescending(l => l.Orden))
                foreach (Layer layer in lstLayers.OrderByDescending(l => l.Orden))
                {
                    //Si el contorno grosor es mayor a uno le pone uno porq sino se empasta y no se ve lo del medio
                    //float pdfContornoGrosor = layer.ContornoGrosor != null && (float)layer.ContornoGrosor.Value > 0 ? 1 : 0;
                    float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;

                    it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                    it.BaseColor pdfRellenoColor = GetAlphaColor(layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent, layer.RellenoTransparencia);
                    PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);
                    //PdfPatternPainter pdfPatternPainter = GetPattern(pdfContentByte, 4, 4, 0.2);

                    string lineDash = layer.Dash;

                    string texto = layer.Nombre;
                    #region Referencia color y nombre Layer
                    x2c = x1c + pdfFontSize * 2;
                    if (layer.PuntoRepresentacion == 0)
                    {
                        if (layer.Relleno)
                        {
                            y2c = y1c + pdfFontSize;
                            PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y1c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                        }
                        else if (layer.Contorno)
                        {
                            //graphics.DrawLine(penContorno, x1c, y1c + (int)(sizeTexto.Height / 2), x2c, y1c + (int)(sizeTexto.Height / 2));
                            y2c = y1c + (pdfFontSize / 2);
                            PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y2c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, null, null);
                        }
                    }
                    else if (layer.PuntoRepresentacion == 2)
                    {
                        if (layer.PuntoImagen != null)
                        {
                            float puntoAnchoPts = pdfFontSize;
                            float puntoAltoPts = pdfFontSize;
                            PDFUtilities.DrawPDFImage(/*pdfDoc,*/ pdfContentByte, layer.PuntoImagen, x1c, y1c, puntoAnchoPts, puntoAltoPts);
                        }
                    }
                    else if (layer.PuntoRepresentacion == 1)
                    {
                        float puntoAnchoPts = pdfFontSize;
                        float puntoAltoPts = pdfFontSize;
                        if (layer.PuntoPredeterminado == 1)
                        {
                            //Circulo
                            float radio = pdfFontSize / 2;
                            PDFUtilities.DrawPDFCircle(pdfContentByte, (x1c + pdfFontSize / 2), (y1c + pdfFontSize / 2), radio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                        }
                        else if (layer.PuntoPredeterminado == 2)
                        {
                            //Cuadrado
                            PDFUtilities.DrawPDFRectangle(pdfContentByte, x1c, y1c, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                        }
                    }
                    x2c = x2c + pdfFontSize / 2;
                    float yTexto = y1c;
                    PDFUtilities.DrawPDFText(pdfContentByte, texto, x2c, yTexto, pdfFont, pdfFontSize, alignment);
                    #endregion

                    //Se pone la tabla segun corresponda
                    //|| layer.ComponenteId == aCompRegComponenteId || layer.ComponenteId == cCompRegComponenteId)
                    if (layer.ComponenteId == expansionComponenteId || layer.ComponenteId == opctMpgComponenteId || layer.ComponenteId == aDesvinComponenteId || layer.ComponenteId == cDesvinComponenteId)
                    {
                        #region Tabla de Datos
                        #region Seteo de parametros esquem tabla etc
                        string esquema = expansionEsquema;
                        string tabla = expansionTabla;
                        string campoGeometry = expansionCampoGeometry;
                        string campoId = expansionCampoId;
                        string campoNombre = expansionCampoNombre;
                        string tituloId = expansionTituloId;
                        string tituloNombre = expansionTituloNombre;
                        if (layer.ComponenteId == expansionComponenteId)
                        {
                            esquema = expansionEsquema;
                            tabla = expansionTabla;
                            campoGeometry = expansionCampoGeometry;
                            campoId = expansionCampoId;
                            campoNombre = expansionCampoNombre;
                            tituloId = expansionTituloId;
                            tituloNombre = expansionTituloNombre;
                        }
                        else if (layer.ComponenteId == opctMpgComponenteId)
                        {
                            esquema = opctMpgEsquema;
                            tabla = opctMpgTabla;
                            campoGeometry = opctMpgCampoGeometry;
                            campoId = opctMpgCampoId;
                            campoNombre = opctMpgCampoNombre;
                            tituloId = opctMpgTituloId;
                            tituloNombre = opctMpgTituloNombre;
                        }
                        else if (layer.ComponenteId == aDesvinComponenteId)
                        {
                            esquema = aDesvinEsquema;
                            tabla = aDesvinTabla;
                            campoGeometry = aDesvinCampoGeometry;
                            campoId = aDesvinCampoId;
                            campoNombre = aDesvinCampoNombre;
                            tituloId = aDesvinTituloId;
                            tituloNombre = aDesvinTituloNombre;
                        }
                        else if (layer.ComponenteId == aCompRegComponenteId)
                        {
                            esquema = aCompRegEsquema;
                            tabla = aCompRegTabla;
                            campoGeometry = aCompRegCampoGeometry;
                            campoId = aCompRegCampoId;
                            campoNombre = aCompRegCampoNombre;
                            tituloId = aCompRegTituloId;
                            tituloNombre = aCompRegTituloNombre;
                        }
                        else if (layer.ComponenteId == cDesvinComponenteId)
                        {
                            esquema = cDesvinEsquema;
                            tabla = cDesvinTabla;
                            campoGeometry = cDesvinCampoGeometry;
                            campoId = cDesvinCampoId;
                            campoNombre = cDesvinCampoNombre;
                            tituloId = cDesvinTituloId;
                            tituloNombre = cDesvinTituloNombre;
                        }
                        else if (layer.ComponenteId == cCompRegComponenteId)
                        {
                            esquema = cCompRegEsquema;
                            tabla = cCompRegTabla;
                            campoGeometry = cCompRegCampoGeometry;
                            campoId = cCompRegCampoId;
                            campoNombre = cCompRegCampoNombre;
                            tituloId = cCompRegTituloId;
                            tituloNombre = cCompRegTituloNombre;
                        }
                        //prefijoId = "E"; //no se esta usando el prefijo, ya esta puesto en el campo idReferencia de la vista 
                        #endregion
                        ExpansionPlot[] aExpansionPlot = _expansionPlotRepository.GetExpansionPlotByObjetoBase(componenteBase, esquema, tabla, campoGeometry, campoId, campoNombre, filtroGeografico, idObjetoGraf, layer.FiltroIdAtributo, anio);
                        if (aExpansionPlot != null && aExpansionPlot.Length > 0)
                        {
                            y1c = y1c - (pdfFontSize - espaciado) / 2;
                            if (y1c <= yMinImpresion)
                            {
                                //pdfDoc.NewPage();
                                col++;
                                if (col > cantColumnasMax)
                                {
                                    col = 1;
                                    pdfDoc.NewPage();
                                    iPagina++;
                                    sPagina = "pág." + iPagina.ToString() + "/" + cantPaginas.ToString();
                                    PDFUtilities.DrawPDFText(pdfContentByte, sPagina, xPagina, yPagina, pdfFont, pdfFontSize, alignment);

                                    x1c = xMinImpresion + pdfFontSize / 2;
                                }
                                else
                                {
                                    //x1c = xMinReferencia + pdfFontSize / 2;
                                    x1c += ((xMaxImpresion - xMinImpresion) / cantColumnasMax);
                                }
                                y1c = yMaxImpresion - espaciado - pdfFontSize;
                            }
                            float pdfFontSizeTabla = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);
                            PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSizeTabla);
                            //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                            string[] aFontStyleTabla = textoFuenteEstilo.Split(',');
                            int pdfFontStyleTabla = aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
                            Color colorTabla = System.Drawing.ColorTranslator.FromHtml(textoColor);
                            it.BaseColor pdfColorTextoTabla = new it.BaseColor(colorTabla.R, colorTabla.G, colorTabla.B);
                            BaseFont pdfbaseFontTabla = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSizeTabla, pdfFontStyleTabla, pdfColorTextoTabla).BaseFont;
                            it.Font pdfFontTabla = new it.Font(pdfbaseFontTabla, pdfFontSizeTabla, pdfFontStyleTabla, pdfColorTextoTabla);

                            float tableWidth = xMaxReferencia - xMinReferencia - (pdfFontSizeTabla / 2);

                            //Tabla para datos
                            int cols = 2;
                            PdfPTable pdfTable;
                            if (campoNombre == string.Empty)
                            {
                                cols = 1;
                                pdfTable = new PdfPTable(cols);
                                pdfTable.TotalWidth = (float)(tableWidth * 0.5);
                                float[] widths = new float[1] { (float)(tableWidth * 0.5) };
                                pdfTable.SetWidths(widths);
                            }
                            else
                            {
                                cols = 2;
                                pdfTable = new PdfPTable(cols);
                                pdfTable.TotalWidth = tableWidth;
                                float[] widths = { (float)(tableWidth * 0.2), (float)(tableWidth * 0.8) };
                                pdfTable.SetWidths(widths);
                            }

                            bool drawBorder = true;

                            PdfPCell pdfCell = new PdfPCell();
                            int alignmentCol1 = it.Element.ALIGN_LEFT;
                            int alignmentCol2 = it.Element.ALIGN_LEFT;
                            float sizeTexto = pdfbaseFontTabla.GetWidthPoint("99999.00", pdfFontSizeTabla);

                            List<ExpansionPlot> lstExpansionPlot = aExpansionPlot.OrderBy(e => e.Id).ToList();
                            int rows = lstExpansionPlot.Count + 1;
                            int iRow = 1;
                            float yRow = y1c;

                            it.BaseColor pdfColorCeldasTitulosTabla = new it.BaseColor(169, 169, 169);
                            pdfCell = new PdfPCell();
                            texto = tituloId;
                            pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol1, textoEspaciadoMM, drawBorder);
                            pdfCell.UseBorderPadding = true;
                            if (campoNombre != string.Empty)
                            {
                                pdfCell.BorderWidthRight = 0;
                            }
                            pdfCell.BackgroundColor = pdfColorCeldasTitulosTabla;
                            pdfTable.AddCell(pdfCell);
                            if (campoNombre != string.Empty)
                            {
                                pdfCell = new PdfPCell();
                                texto = tituloNombre;
                                pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol2, textoEspaciadoMM, drawBorder);
                                pdfCell.UseBorderPadding = true;
                                pdfCell.BorderWidthLeft = 0;
                                pdfCell.BackgroundColor = pdfColorCeldasTitulosTabla;
                                pdfTable.AddCell(pdfCell);
                            }
                            foreach (var expansionPlot in lstExpansionPlot)
                            {
                                pdfCell = new PdfPCell();
                                texto = expansionPlot.IdReferencia;
                                pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol1, textoEspaciadoMM, drawBorder);
                                pdfCell.UseBorderPadding = true;
                                if (campoNombre != string.Empty)
                                {
                                    pdfCell.BorderWidthRight = 0;
                                }
                                pdfTable.AddCell(pdfCell);
                                if (campoNombre != string.Empty)
                                {
                                    pdfCell = new PdfPCell();
                                    texto = expansionPlot.Nombre;
                                    pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontTabla, alignmentCol2, textoEspaciadoMM, drawBorder);
                                    pdfCell.UseBorderPadding = true;
                                    pdfCell.BorderWidthLeft = 0;
                                    pdfTable.AddCell(pdfCell);
                                }
                                iRow++;
                                yRow = yRow - (pdfFontSizeTabla + textoEspaciadoMM);
                                if (yRow <= yMinImpresion + 4 * (pdfFontSizeTabla + textoEspaciadoMM))
                                {
                                    pdfTable.WriteSelectedRows(0, -1, 0, iRow, x1c, y1c - (textoEspaciadoMM * 2), pdfContentByte);
                                    //pdfDoc.NewPage();
                                    iRow = 0;
                                    col++;
                                    if (col > cantColumnasMax)
                                    {
                                        col = 1;
                                        pdfDoc.NewPage();
                                        iPagina++;
                                        sPagina = "pág." + iPagina.ToString() + "/" + cantPaginas.ToString();
                                        PDFUtilities.DrawPDFText(pdfContentByte, sPagina, xPagina, yPagina, pdfFont, pdfFontSize, alignment);

                                        x1c = xMinImpresion + pdfFontSize / 2;
                                    }
                                    else
                                    {
                                        //x1c = xMinReferencia + pdfFontSize / 2;
                                        x1c += ((xMaxImpresion - xMinImpresion) / cantColumnasMax);
                                    }
                                    y1c = yMaxImpresion - espaciado - pdfFontSize;
                                    yRow = y1c;
                                    if (campoNombre == string.Empty)
                                    {
                                        cols = 1;
                                        pdfTable = new PdfPTable(cols);
                                        pdfTable.TotalWidth = (float)(tableWidth * 0.5);
                                        float[] widths = new float[1] { (float)(tableWidth * 0.5) };
                                        pdfTable.SetWidths(widths);
                                    }
                                    else
                                    {
                                        cols = 2;
                                        pdfTable = new PdfPTable(cols);
                                        pdfTable.TotalWidth = tableWidth;
                                        float[] widths = { (float)(tableWidth * 0.2), (float)(tableWidth * 0.8) };
                                        pdfTable.SetWidths(widths);
                                    }
                                }
                            }
                            pdfTable.WriteSelectedRows(0, -1, x1c, y1c - (textoEspaciadoMM * 2), pdfContentByte);

                            //y1c = y1c - (rows * (pdfFontSizeTabla + textoEspaciadoMM));
                            y1c = y1c - (iRow * (pdfFontSizeTabla + textoEspaciadoMM));
                            //y1c = y1c - (pdfFontSize + espaciado) * 4;
                        }
                        #endregion
                    }

                    //y1c = y1c - (pdfFontSize - espaciado) * 2;
                    y1c = y1c - (pdfFontSize + espaciado) * 2;
                    if (y1c <= yMinImpresion + 4 * (pdfFontSize + espaciado))
                    {
                        //pdfDoc.NewPage();
                        col++;
                        if (col > cantColumnasMax)
                        {
                            col = 1;
                            pdfDoc.NewPage();
                            iPagina++;
                            sPagina = "pág." + iPagina.ToString() + "/" + cantPaginas.ToString();
                            PDFUtilities.DrawPDFText(pdfContentByte, sPagina, xPagina, yPagina, pdfFont, pdfFontSize, alignment);

                            x1c = xMinImpresion + pdfFontSize / 2;
                        }
                        else
                        {
                            //x1c = xMinReferencia + pdfFontSize / 2;
                            x1c += ((xMaxImpresion - xMinImpresion) / cantColumnasMax);
                        }
                        y1c = yMaxImpresion - espaciado - pdfFontSize;
                        //y1c = yMaxReferencia - espaciado - pdfFontSize;
                        //col++;
                        //if (col > plantilla.ReferenciaColumnas)
                        //{
                        //    break;
                        //}
                        //else
                        //{
                        //    x1c += ((xMaxReferencia - xMinReferencia) / plantilla.ReferenciaColumnas);
                        //}
                    }
                }
                if (nombreCallesEnPlano)
                {
                    Color colorTextoCalle = System.Drawing.ColorTranslator.FromHtml(textoCalleColor);
                    it.BaseColor pdfColorTextoCalle = new it.BaseColor(colorTextoCalle.R, colorTextoCalle.G, colorTextoCalle.B);
                    //float pdfFontSizeCalle = it.Utilities.MillimetersToPoints((float)textoCalleFuenteTamanio);
                    PDFUtilities.RegisterBaseFont(textoCalleFuenteNombre, pdfFontSize);
                    //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                    string[] aFontStyleCalle = textoCalleFuenteEstilo.Split(',');
                    int pdfFontStyleCalle = aFontStyleCalle.Select(x => Convert.ToInt32(x)).Sum();
                    BaseFont pdfbaseFontCalle = it.FontFactory.GetFont(textoCalleFuenteNombre, pdfFontSize, pdfFontStyleCalle, pdfColorTextoCalle).BaseFont;
                    it.Font pdfFontCalle = new it.Font(pdfbaseFontCalle, pdfFontSize, pdfFontStyleCalle, pdfColorTextoCalle);
                    PDFUtilities.DrawPDFText(pdfContentByte, "CALLE", x1c, y1c, pdfFontCalle, pdfFontSize, alignment);

                    x2c = x1c + pdfFontSize * 4;
                    PDFUtilities.DrawPDFText(pdfContentByte, "NOMBRES DE CALLE", x2c, y1c, pdfFont, pdfFontSize, alignment);
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// Determina si se imprimen las referencias a partir de la pagina 2 y devuelve la cantidad de paginas
        /// </summary>
        private int GetCantidadPaginasInformeAnualEA(int idFuncionAdicional, it.Document pdfDoc, PdfContentByte pdfContentByte, Plantilla plantilla, List<Layer> lstLayers, Componente componenteBase, string idObjetoGraf, List<string> lstCoordenadas, string anio, ref bool refEnPagina2)
        {
            refEnPagina2 = false;
            int cantPaginas = 1;
            try
            {
                #region Inicializacion de Variables
                int filtroGeografico = 1;

                float xMinImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMin);
                float yMinImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMin);
                float xMaxImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionXMax);
                float yMaxImpresion = it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMax);

                float xMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMin);
                float yMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMin);
                float xMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
                float yMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMax);

                float espaciado = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaEspaciado);

                //Definicion de vbles y defaults

                //Agua y Cloaca. Cambian sus valores en los parametros de la DB
                long expansionComponenteId = 2302;
                string expansionEsquema = "AYSA_GERED_DEV";
                string expansionTabla = "VW_ZEXPANA";
                string expansionCampoGeometry = "GEOMETRY";
                string expansionCampoId = "ID_REFERENCIA";
                string expansionCampoNombre = "NOMBRE";
                string expansionTituloId = "Identificador";
                string expansionTituloNombre = "Nombre";

                //Agua y Cloaca. Cambian sus valores en los parametros de la DB
                long opctMpgComponenteId = 2303;
                string opctMpgEsquema = "AYSA_GERED_DEV";
                string opctMpgTabla = "VW_OPCT_MPG_AT";
                string opctMpgCampoGeometry = "GEOMETRY";
                string opctMpgCampoId = "ID_REFERENCIA";
                string opctMpgCampoNombre = "NOMBRE";
                string opctMpgTituloId = "Identificador";
                string opctMpgTituloNombre = "Nombre";

                //Agua
                long aDesvinComponenteId = 2304;
                string aDesvinEsquema = "AYSA_GERED_DEV";
                string aDesvinTabla = "VW_A_DESVIN";
                string aDesvinCampoGeometry = "GEOMETRY";
                string aDesvinCampoId = "ID_REFERENCIA";
                string aDesvinCampoNombre = "NOMBRE";
                string aDesvinTituloId = "Identificador";
                string aDesvinTituloNombre = "Nombre";

                long aCompRegComponenteId = 2305;
                string aCompRegEsquema = "AYSA_GERED_DEV";
                string aCompRegTabla = "VW_ACOMPREG";
                string aCompRegCampoGeometry = "GEOMETRY";
                string aCompRegCampoId = "ID_REFERENCIA";
                string aCompRegCampoNombre = string.Empty;
                //string aCompRegCampoNombre = "NOMBRE";
                string aCompRegTituloId = "Identificador";
                //string aCompRegTituloNombre = "Nombre";
                string aCompRegTituloNombre = string.Empty;

                //Cloaca
                long cDesvinComponenteId = 2315;
                string cDesvinEsquema = "AYSA_GERED_DEV";
                string cDesvinTabla = "VW_C_DESVIN";
                string cDesvinCampoGeometry = "GEOMETRY";
                string cDesvinCampoId = "ID_REFERENCIA";
                string cDesvinCampoNombre = "NOMBRE";
                string cDesvinTituloId = "Identificador";
                string cDesvinTituloNombre = "Nombre";

                long cCompRegComponenteId = 2316;
                string cCompRegEsquema = "AYSA_GERED_DEV";
                string cCompRegTabla = "VW_CCOMPREG";
                string cCompRegCampoGeometry = "GEOMETRY";
                string cCompRegCampoId = "ID_REFERENCIA";
                string cCompRegCampoNombre = "NOMBRE";
                string cCompRegTituloId = "Identificador";
                string cCompRegTituloNombre = "Nombre";


                float textoEspaciadoMM = (float)1.41083;
                string textoFuenteNombre = "Arial";
                double textoFuenteTamanio = 0.8;
                string textoFuenteEstilo = "0,0,0,0";
                string textoColor = "#000000";

                #endregion

                #region NumberFormat
                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.CurrencyDecimalDigits = 4;
                numberFormat.CurrencyDecimalSeparator = ".";
                numberFormat.NumberDecimalDigits = 4;
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.PercentDecimalDigits = 2;
                numberFormat.PercentDecimalSeparator = ".";
                #endregion

                #region Parametros de la DB
                //Agua y Cloaca
                #region Expansion
                FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out expansionComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    expansionEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TABLA");
                if (funcAdicParametro != null)
                {
                    expansionTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    expansionCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    expansionCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    expansionCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    expansionTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "EXPANSIONES_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    expansionTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua y Cloaca
                #region OPCT_MPG_AT
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out opctMpgComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    opctMpgEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TABLA");
                if (funcAdicParametro != null)
                {
                    opctMpgTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    opctMpgCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    opctMpgTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "OPCT_MPG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    opctMpgTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua
                #region A_DESVIN
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out aDesvinComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    aDesvinEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TABLA");
                if (funcAdicParametro != null)
                {
                    aDesvinTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aDesvinCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    aDesvinTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "A_DESVIN_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aDesvinTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Agua
                #region ACOMPREG
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out aCompRegComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    aCompRegEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TABLA");
                if (funcAdicParametro != null)
                {
                    aCompRegTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    aCompRegCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    aCompRegCampoId = funcAdicParametro.Valor;
                }
                //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_CAMPO_NOMBRE");
                //if (funcAdicParametro != null)
                //{
                //    aCompRegCampoNombre = funcAdicParametro.Valor;
                //}
                aCompRegCampoNombre = string.Empty;
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    aCompRegTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ACOMPREG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    aCompRegTituloNombre = funcAdicParametro.Valor;
                }
                #endregion

                //Cloaca
                #region C_DESVIN
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out cDesvinComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    cDesvinEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TABLA");
                if (funcAdicParametro != null)
                {
                    cDesvinTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_CAMPO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cDesvinCampoNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    cDesvinTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "C_DESVIN_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cDesvinTituloNombre = funcAdicParametro.Valor;
                }
                #endregion
                //Cloaca
                #region CCOMPREG
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_COMPONENTE_ID");
                if (funcAdicParametro != null)
                {
                    long.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out cCompRegComponenteId);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_ESQUEMA");
                if (funcAdicParametro != null)
                {
                    cCompRegEsquema = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TABLA");
                if (funcAdicParametro != null)
                {
                    cCompRegTabla = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_GEOMETRY");
                if (funcAdicParametro != null)
                {
                    cCompRegCampoGeometry = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_ID");
                if (funcAdicParametro != null)
                {
                    cCompRegCampoId = funcAdicParametro.Valor;
                }
                //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_CAMPO_NOMBRE");
                //if (funcAdicParametro != null)
                //{
                //    cCompRegCampoNombre = funcAdicParametro.Valor;
                //}
                cCompRegCampoNombre = string.Empty;
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TITULO_ID");
                if (funcAdicParametro != null)
                {
                    cCompRegTituloId = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CCOMPREG_TITULO_NOMBRE");
                if (funcAdicParametro != null)
                {
                    cCompRegTituloNombre = funcAdicParametro.Valor;
                }
                #endregion

                #region Font
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
                if (funcAdicParametro != null)
                {
                    float.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoEspaciadoMM);
                }
                #endregion
                #endregion
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaFuenteTamanio);

                int cantColumnasMax = Convert.ToInt32(Math.Floor((xMaxImpresion - xMinImpresion) / (xMaxReferencia - xMinReferencia)));
                int col = 1;

                float y1c = yMaxReferencia - espaciado - pdfFontSize;
                float y1r = yMaxImpresion - espaciado - pdfFontSize;

                foreach (Layer layer in lstLayers.OrderByDescending(l => l.Orden))
                {
                    //Se pone la tabla segun corresponda
                    //|| layer.ComponenteId == aCompRegComponenteId || layer.ComponenteId == cCompRegComponenteId)
                    if (layer.ComponenteId == expansionComponenteId || layer.ComponenteId == opctMpgComponenteId || layer.ComponenteId == aDesvinComponenteId || layer.ComponenteId == cDesvinComponenteId)
                    {
                        #region Tabla de Datos
                        #region Seteo de parametros esquem tabla etc
                        string esquema = expansionEsquema;
                        string tabla = expansionTabla;
                        string campoGeometry = expansionCampoGeometry;
                        string campoId = expansionCampoId;
                        string campoNombre = expansionCampoNombre;
                        string tituloId = expansionTituloId;
                        string tituloNombre = expansionTituloNombre;
                        if (layer.ComponenteId == expansionComponenteId)
                        {
                            esquema = expansionEsquema;
                            tabla = expansionTabla;
                            campoGeometry = expansionCampoGeometry;
                            campoId = expansionCampoId;
                            campoNombre = expansionCampoNombre;
                            tituloId = expansionTituloId;
                            tituloNombre = expansionTituloNombre;
                        }
                        else if (layer.ComponenteId == opctMpgComponenteId)
                        {
                            esquema = opctMpgEsquema;
                            tabla = opctMpgTabla;
                            campoGeometry = opctMpgCampoGeometry;
                            campoId = opctMpgCampoId;
                            campoNombre = opctMpgCampoNombre;
                            tituloId = opctMpgTituloId;
                            tituloNombre = opctMpgTituloNombre;
                        }
                        else if (layer.ComponenteId == aDesvinComponenteId)
                        {
                            esquema = aDesvinEsquema;
                            tabla = aDesvinTabla;
                            campoGeometry = aDesvinCampoGeometry;
                            campoId = aDesvinCampoId;
                            campoNombre = aDesvinCampoNombre;
                            tituloId = aDesvinTituloId;
                            tituloNombre = aDesvinTituloNombre;
                        }
                        else if (layer.ComponenteId == aCompRegComponenteId)
                        {
                            esquema = aCompRegEsquema;
                            tabla = aCompRegTabla;
                            campoGeometry = aCompRegCampoGeometry;
                            campoId = aCompRegCampoId;
                            campoNombre = aCompRegCampoNombre;
                            tituloId = aCompRegTituloId;
                            tituloNombre = aCompRegTituloNombre;
                        }
                        else if (layer.ComponenteId == cDesvinComponenteId)
                        {
                            esquema = cDesvinEsquema;
                            tabla = cDesvinTabla;
                            campoGeometry = cDesvinCampoGeometry;
                            campoId = cDesvinCampoId;
                            campoNombre = cDesvinCampoNombre;
                            tituloId = cDesvinTituloId;
                            tituloNombre = cDesvinTituloNombre;
                        }
                        else if (layer.ComponenteId == cCompRegComponenteId)
                        {
                            esquema = cCompRegEsquema;
                            tabla = cCompRegTabla;
                            campoGeometry = cCompRegCampoGeometry;
                            campoId = cCompRegCampoId;
                            campoNombre = cCompRegCampoNombre;
                            tituloId = cCompRegTituloId;
                            tituloNombre = cCompRegTituloNombre;
                        }
                        //prefijoId = "E"; //no se esta usando el prefijo, ya esta puesto en el campo idReferencia de la vista 
                        #endregion
                        ExpansionPlot[] aExpansionPlot = _expansionPlotRepository.GetExpansionPlotByObjetoBase(componenteBase, esquema, tabla, campoGeometry, campoId, campoNombre, filtroGeografico, idObjetoGraf, layer.FiltroIdAtributo, anio);
                        if (aExpansionPlot != null && aExpansionPlot.Length > 0)
                        {
                            y1c = y1c - (pdfFontSize - espaciado) / 2;
                            if (y1c <= yMinReferencia)
                            {
                                refEnPagina2 = true;
                                //pdfDoc.NewPage();
                                y1c = yMaxReferencia - espaciado - pdfFontSize;
                            }

                            y1r = y1r - (pdfFontSize - espaciado) / 2;
                            if (y1r <= yMinImpresion)
                            {
                                col++;
                                if (col > cantColumnasMax)
                                {
                                    col = 1;
                                    //pdfDoc.NewPage();
                                    cantPaginas++;
                                }
                                y1r = yMaxImpresion - espaciado - pdfFontSize;
                            }

                            float pdfFontSizeTabla = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                            List<ExpansionPlot> lstExpansionPlot = aExpansionPlot.OrderBy(e => e.Id).ToList();
                            int rows = lstExpansionPlot.Count + 1;
                            int iRow = 1;
                            float yRow = y1c;
                            float yRow_r = y1r;
                            foreach (var expansionPlot in lstExpansionPlot)
                            {
                                iRow++;
                                yRow = yRow - (pdfFontSizeTabla + textoEspaciadoMM);
                                if (yRow <= yMinReferencia)
                                {
                                    refEnPagina2 = true;
                                    //pdfDoc.NewPage();
                                    y1c = yMaxReferencia - espaciado - pdfFontSize;
                                    yRow = y1c;
                                }

                                yRow_r = yRow_r - (pdfFontSizeTabla + textoEspaciadoMM);
                                if (yRow_r <= yMinImpresion + 4 * (pdfFontSizeTabla + textoEspaciadoMM))
                                {
                                    iRow = 0;
                                    col++;
                                    if (col > cantColumnasMax)
                                    {
                                        col = 1;
                                        //pdfDoc.NewPage();
                                        cantPaginas++;
                                    }
                                    y1r = yMaxImpresion - espaciado - pdfFontSize;
                                    yRow_r = y1r;
                                }
                            }
                            y1c = y1c - (rows * (pdfFontSizeTabla + textoEspaciadoMM));
                            y1r = y1r - (iRow * (pdfFontSizeTabla + textoEspaciadoMM));
                        }
                        #endregion
                    }
                    y1c = y1c - (pdfFontSize + espaciado) * 2;
                    if (y1c <= yMinReferencia)
                    {
                        //pdfDoc.NewPage();
                        refEnPagina2 = true;
                        y1c = yMaxReferencia - espaciado - pdfFontSize;
                    }

                    y1r = y1r - (pdfFontSize + espaciado) * 2;
                    if (y1r <= yMinImpresion + 4 * (pdfFontSize + espaciado))
                    {
                        col++;
                        if (col > cantColumnasMax)
                        {
                            col = 1;
                            //pdfDoc.NewPage();
                            cantPaginas++;
                        }
                        y1r = yMaxImpresion - espaciado - pdfFontSize;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return cantPaginas;
        }

        private bool FuncEspDibujarInformeAnualReferenciaRHEC(int idFuncionAdicional, it.Document pdfDoc, PdfContentByte pdfContentByte, Plantilla plantilla, List<Layer> lstLayers, Componente componenteBase, string idObjetoGraf, List<string> lstCoordenadas, string anio)
        {
            bool ret = true;
            try
            {
                #region Inicializacion de Variables
                float xMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMin);
                float yMinReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMin);
                float xMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaXMax);
                float yMaxReferencia = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaYMax);
                float espaciado = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaEspaciado);

                //Definicion de vbles y defaults

                float textoEspaciadoMM = (float)1.41083;
                string textoFuenteNombre = "Arial";
                double textoFuenteTamanio = 0.8;
                string textoFuenteEstilo = "0,0,0,0";
                string textoColor = "#000000";

                float subtituloEspaciadoMM = (float)1.41083;
                string subtituloFuenteNombre = "Arial";
                double subtituloFuenteTamanio = 0.8;
                string subtituloFuenteEstilo = "0,0,0,0";
                string subtituloColor = "#000000";

                string idsDirRegional = string.Empty;
                string idsGrandesConductos = string.Empty;
                string subtituloDirRegional = "DIRECCION REGIONAL";
                string subtituloGrandesConductos = "GRANDES CONDUCTOS";

                string textoCalleFuenteNombre = "Arial";
                double textoCalleFuenteTamanio = 0.8;
                string textoCalleFuenteEstilo = "0,0,0,0";
                string textoCalleColor = "#000000";

                bool nombreCallesEnPlano = false;
                #endregion

                #region NumberFormat
                NumberFormatInfo numberFormat = new NumberFormatInfo();
                numberFormat.CurrencyDecimalDigits = 4;
                numberFormat.CurrencyDecimalSeparator = ".";
                numberFormat.NumberDecimalDigits = 4;
                numberFormat.NumberDecimalSeparator = ".";
                numberFormat.PercentDecimalDigits = 2;
                numberFormat.PercentDecimalSeparator = ".";
                #endregion

                #region Parametros de la DB

                #region Direccion Regional y Grandes Conductos
                FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "IDS_DIR_REGIONAL");
                if (funcAdicParametro != null)
                {
                    idsDirRegional = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "IDS_GRANDES_CONDUCTOS");
                if (funcAdicParametro != null)
                {
                    idsGrandesConductos = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "SUBTITULO_DIR_REGIONAL");
                if (funcAdicParametro != null)
                {
                    subtituloDirRegional = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "SUBTITULO_GRANDES_CONDUCTOS");
                if (funcAdicParametro != null)
                {
                    subtituloGrandesConductos = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "SUBTITULO_FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    subtituloFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "SUBTITULO_FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    subtituloColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "SUBTITULO_FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    subtituloFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "SUBTITULO_FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out subtituloFuenteTamanio);
                }
                #endregion

                #region Font
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
                if (funcAdicParametro != null)
                {
                    float.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoEspaciadoMM);
                }
                #endregion

                #region Fuentes para nombres de Calles
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_NOMBRE");
                if (funcAdicParametro != null)
                {
                    textoCalleFuenteNombre = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_COLOR");
                if (funcAdicParametro != null)
                {
                    textoCalleColor = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_ESTILO");
                if (funcAdicParametro != null)
                {
                    textoCalleFuenteEstilo = funcAdicParametro.Valor;
                }
                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_FUENTE_TAMANIO");
                if (funcAdicParametro != null)
                {
                    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoCalleFuenteTamanio);
                }
                #endregion

                funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "NOMBRE_CALLES_EN_PLANO");
                if (funcAdicParametro != null)
                {
                    //int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
                    nombreCallesEnPlano = (funcAdicParametro.Valor == "1" ? true : false);
                }
                #endregion

                float pdfFontSizeSubtitulo = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);
                PDFUtilities.RegisterBaseFont(subtituloFuenteNombre, pdfFontSizeSubtitulo);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStyleSubtitulo = subtituloFuenteEstilo.Split(',');
                int pdfFontStyleTabla = aFontStyleSubtitulo.Select(x => Convert.ToInt32(x)).Sum();
                Color colorSubtitulo = System.Drawing.ColorTranslator.FromHtml(subtituloColor);
                it.BaseColor pdfColorSubtitulo = new it.BaseColor(colorSubtitulo.R, colorSubtitulo.G, colorSubtitulo.B);
                BaseFont pdfbaseFontSubtitulo = it.FontFactory.GetFont(subtituloFuenteNombre, pdfFontSizeSubtitulo, pdfFontStyleTabla, pdfColorSubtitulo).BaseFont;
                it.Font pdfFontSubtitulo = new it.Font(pdfbaseFontSubtitulo, pdfFontSizeSubtitulo, pdfFontStyleTabla, pdfColorSubtitulo);

                // DibujarReferencias - DrawPDFRectangle de mins maxs
                //DrawPDFRectangle(pdfContentByte, xMinReferencia, yMinReferencia, xMaxReferencia - xMinReferencia, yMaxReferencia - yMinReferencia, new it.BaseColor(Color.Gray.R, Color.Gray.G, Color.Gray.B), 1.0f, null, null);

                Color colorReferencia = System.Drawing.ColorTranslator.FromHtml(plantilla.ReferenciaColor);
                it.BaseColor pdfColorTexto = new it.BaseColor(colorReferencia.R, colorReferencia.G, colorReferencia.B);

                int alignment = it.Element.ALIGN_LEFT + it.Element.ALIGN_BOTTOM;

                float pdfFontSize = it.Utilities.MillimetersToPoints((float)plantilla.ReferenciaFuenteTamanio);

                PDFUtilities.RegisterBaseFont(plantilla.ReferenciaFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStyle = plantilla.ReferenciaFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(plantilla.ReferenciaFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                float x1c = xMinReferencia;
                float y1c = yMaxReferencia - espaciado - pdfFontSize;
                float x2c = 0;
                float y2c = 0;

                x1c += pdfFontSize / 2;

                //string[] aFontStyleTitRef = plantilla.ReferenciaFuenteEstilo.Split(',');
                //int pdfFontStyleTitRef = aFontStyleTitRef.Select(x => Convert.ToInt32(x)).Sum();
                int pdfFontStyleTitRef = 1;
                BaseFont pdfbaseFontTitRef = it.FontFactory.GetFont(plantilla.ReferenciaFuenteNombre, pdfFontSize, pdfFontStyleTitRef, pdfColorTexto).BaseFont;
                it.Font pdfFontTitRef = new it.Font(pdfbaseFontTitRef, pdfFontSize, pdfFontStyleTitRef, pdfColorTexto);

                x2c = x1c + pdfFontSize / 2;
                PDFUtilities.DrawPDFText(pdfContentByte, "REFERENCIAS", x2c, y1c, pdfFontTitRef, pdfFontSize, alignment);
                y1c = y1c - (pdfFontSize + espaciado) * 2;

                List<Layer> lstLayerDirRegional = lstLayers.Where(p => idsDirRegional.Contains(p.IdLayer.ToString())).ToList();
                //List<Layer> lstLayerGrandesConductos = lstLayers.Where(p => p.IdLayer.ToString().Contains(idsDirRegional)).ToList();
                List<Layer> lstLayerGrandesConductos = lstLayers.Where(p => idsGrandesConductos.Contains(p.IdLayer.ToString())).ToList();
                //var list2Lookup = aLayerGrafInside.ToList().ToLookup(lyGraf => lyGraf.FeatId);
                //                              var listdiff = aLayerGrafAnyinteract.ToList().Where(lyGraf => (!list2Lookup.Contains(lyGraf.FeatId)));
                List<Layer> lstLayerGenerales = lstLayers.Where(p => (!lstLayerDirRegional.ToLookup(l => l.IdLayer).Contains(p.IdLayer))
                                                                  && (!lstLayerGrandesConductos.ToLookup(l => l.IdLayer).Contains(p.IdLayer))).ToList();

                #region Layers Generales
                foreach (Layer layer in lstLayerGenerales.OrderByDescending(l => l.Orden))
                {
                    //Si el contorno grosor es mayor a uno le pone uno porq sino se empasta y no se ve lo del medio
                    //float pdfContornoGrosor = layer.ContornoGrosor != null && (float)layer.ContornoGrosor.Value > 0 ? 1 : 0;
                    float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;

                    it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                    it.BaseColor pdfRellenoColor = GetAlphaColor(layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent, layer.RellenoTransparencia);
                    PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);

                    string lineDash = layer.Dash;

                    string texto = layer.Nombre;

                    x2c = x1c + pdfFontSize * 2;
                    if (layer.PuntoRepresentacion == 0)
                    {
                        if (layer.Relleno)
                        {
                            y2c = y1c + pdfFontSize;
                            PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y1c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                        }
                        else if (layer.Contorno)
                        {
                            //graphics.DrawLine(penContorno, x1c, y1c + (int)(sizeTexto.Height / 2), x2c, y1c + (int)(sizeTexto.Height / 2));
                            y2c = y1c + (pdfFontSize / 2);
                            PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y2c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, null, null);
                        }
                    }
                    else if (layer.PuntoRepresentacion == 2)
                    {
                        if (layer.PuntoImagen != null)
                        {
                            float puntoAnchoPts = pdfFontSize;
                            float puntoAltoPts = pdfFontSize;
                            PDFUtilities.DrawPDFImage(/*pdfDoc,*/ pdfContentByte, layer.PuntoImagen, x1c, y1c, puntoAnchoPts, puntoAltoPts);
                        }
                    }
                    else if (layer.PuntoRepresentacion == 1)
                    {
                        float puntoAnchoPts = pdfFontSize;
                        float puntoAltoPts = pdfFontSize;
                        if (layer.PuntoPredeterminado == 1)
                        {
                            //Circulo
                            float radio = pdfFontSize / 2;
                            PDFUtilities.DrawPDFCircle(pdfContentByte, (x1c + pdfFontSize / 2), (y1c + pdfFontSize / 2), radio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                        }
                        else if (layer.PuntoPredeterminado == 2)
                        {
                            //Cuadrado
                            PDFUtilities.DrawPDFRectangle(pdfContentByte, x1c, y1c, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                        }
                    }
                    x2c = x2c + pdfFontSize / 2;
                    float yTexto = y1c;
                    PDFUtilities.DrawPDFText(pdfContentByte, texto, x2c, yTexto, pdfFont, pdfFontSize, alignment);

                    y1c = y1c - (pdfFontSize + espaciado) * 2;
                    if (y1c <= yMinReferencia)
                    {
                        pdfDoc.NewPage();
                        x1c = xMinReferencia + pdfFontSize / 2;
                        y1c = yMaxReferencia - espaciado - pdfFontSize;
                        //y1c = yMaxReferencia - espaciado - pdfFontSize;
                        //col++;
                        //if (col > plantilla.ReferenciaColumnas)
                        //{
                        //    break;
                        //}
                        //else
                        //{
                        //    x1c += ((xMaxReferencia - xMinReferencia) / plantilla.ReferenciaColumnas);
                        //}
                    }
                }
                #endregion

                if (lstLayerDirRegional.Count() > 0)
                {
                    y1c = y1c - (pdfFontSize + espaciado);
                    PDFUtilities.DrawPDFText(pdfContentByte, subtituloDirRegional, x1c, y1c, pdfFontSubtitulo, pdfFontSizeSubtitulo, alignment);
                    y1c = y1c - (pdfFontSize + espaciado) * 2;

                    #region Layers Direccion Regional
                    foreach (Layer layer in lstLayerDirRegional.OrderByDescending(l => l.Orden))
                    {
                        //Si el contorno grosor es mayor a uno le pone uno porq sino se empasta y no se ve lo del medio
                        //float pdfContornoGrosor = layer.ContornoGrosor != null && (float)layer.ContornoGrosor.Value > 0 ? 1 : 0;
                        float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;

                        it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                        it.BaseColor pdfRellenoColor = GetAlphaColor(layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent, layer.RellenoTransparencia);
                        PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);

                        string lineDash = layer.Dash;

                        string texto = layer.Nombre;

                        x2c = x1c + pdfFontSize * 2;
                        if (layer.PuntoRepresentacion == 0)
                        {
                            if (layer.Relleno)
                            {
                                y2c = y1c + pdfFontSize;
                                PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y1c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                            }
                            else if (layer.Contorno)
                            {
                                //graphics.DrawLine(penContorno, x1c, y1c + (int)(sizeTexto.Height / 2), x2c, y1c + (int)(sizeTexto.Height / 2));
                                y2c = y1c + (pdfFontSize / 2);
                                PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y2c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, null, null);
                            }
                        }
                        else if (layer.PuntoRepresentacion == 2)
                        {
                            if (layer.PuntoImagen != null)
                            {
                                float puntoAnchoPts = pdfFontSize;
                                float puntoAltoPts = pdfFontSize;
                                PDFUtilities.DrawPDFImage(/*pdfDoc,*/ pdfContentByte, layer.PuntoImagen, x1c, y1c, puntoAnchoPts, puntoAltoPts);
                            }
                        }
                        else if (layer.PuntoRepresentacion == 1)
                        {
                            float puntoAnchoPts = pdfFontSize;
                            float puntoAltoPts = pdfFontSize;
                            if (layer.PuntoPredeterminado == 1)
                            {
                                //Circulo
                                float radio = pdfFontSize / 2;
                                PDFUtilities.DrawPDFCircle(pdfContentByte, (x1c + pdfFontSize / 2), (y1c + pdfFontSize / 2), radio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
                            else if (layer.PuntoPredeterminado == 2)
                            {
                                //Cuadrado
                                PDFUtilities.DrawPDFRectangle(pdfContentByte, x1c, y1c, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
                        }
                        x2c = x2c + pdfFontSize / 2;
                        float yTexto = y1c;
                        //PDFUtilities.DrawPDFText(pdfContentByte, texto, x2c, yTexto, pdfFont, pdfFontSize, alignment);
                        PDFUtilities.DrawPDFTextMax(pdfContentByte, texto, x2c, ref y1c, xMaxReferencia, pdfFont, pdfFontSize, alignment);

                        y1c = y1c - (pdfFontSize + espaciado) * 2;
                        if (y1c <= yMinReferencia)
                        {
                            pdfDoc.NewPage();
                            x1c = xMinReferencia + pdfFontSize / 2;
                            y1c = yMaxReferencia - espaciado - pdfFontSize;
                            //y1c = yMaxReferencia - espaciado - pdfFontSize;
                            //col++;
                            //if (col > plantilla.ReferenciaColumnas)
                            //{
                            //    break;
                            //}
                            //else
                            //{
                            //    x1c += ((xMaxReferencia - xMinReferencia) / plantilla.ReferenciaColumnas);
                            //}
                        }
                    }
                    #endregion
                }
                if (lstLayerGrandesConductos.Count() > 0)
                {
                    y1c = y1c - (pdfFontSize + espaciado);
                    PDFUtilities.DrawPDFText(pdfContentByte, subtituloGrandesConductos, x1c, y1c, pdfFontSubtitulo, pdfFontSizeSubtitulo, alignment);
                    y1c = y1c - (pdfFontSize + espaciado) * 2;

                    #region Layers Grandes Conductos
                    foreach (Layer layer in lstLayerGrandesConductos.OrderByDescending(l => l.Orden))
                    {
                        //Si el contorno grosor es mayor a uno le pone uno porq sino se empasta y no se ve lo del medio
                        //float pdfContornoGrosor = layer.ContornoGrosor != null && (float)layer.ContornoGrosor.Value > 0 ? 1 : 0;
                        float pdfContornoGrosor = layer.ContornoGrosor != null ? it.Utilities.MillimetersToPoints((float)layer.ContornoGrosor.Value) : 0;

                        it.BaseColor pdfContornoColor = GetAlphaColor(!string.IsNullOrEmpty(layer.ContornoColor) ? ColorTranslator.FromHtml(layer.ContornoColor) : Color.Black, layer.RellenoTransparencia);
                        it.BaseColor pdfRellenoColor = GetAlphaColor(layer.Relleno ? ColorTranslator.FromHtml(layer.RellenoColor) : Color.Transparent, layer.RellenoTransparencia);
                        PdfPatternPainter pdfPatternPainter = (layer.Pattern ? GetPattern(pdfContentByte, (float)layer.PatternAncho, (float)layer.PatternAlto, (double)layer.PatternLineaAncho) : null);

                        string lineDash = layer.Dash;

                        string texto = layer.Nombre;

                        x2c = x1c + pdfFontSize * 2;
                        if (layer.PuntoRepresentacion == 0)
                        {
                            if (layer.Relleno)
                            {
                                y2c = y1c + pdfFontSize;
                                PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y1c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor, pdfPatternPainter, lineDash);
                            }
                            else if (layer.Contorno)
                            {
                                //graphics.DrawLine(penContorno, x1c, y1c + (int)(sizeTexto.Height / 2), x2c, y1c + (int)(sizeTexto.Height / 2));
                                y2c = y1c + (pdfFontSize / 2);
                                PDFUtilities.DrawPDFRectangle2(pdfContentByte, x1c, y2c, x2c, y2c, pdfContornoColor, pdfContornoGrosor, null, null);
                            }
                        }
                        else if (layer.PuntoRepresentacion == 2)
                        {
                            if (layer.PuntoImagen != null)
                            {
                                float puntoAnchoPts = pdfFontSize;
                                float puntoAltoPts = pdfFontSize;
                                PDFUtilities.DrawPDFImage(/*pdfDoc,*/ pdfContentByte, layer.PuntoImagen, x1c, y1c, puntoAnchoPts, puntoAltoPts);
                            }
                        }
                        else if (layer.PuntoRepresentacion == 1)
                        {
                            float puntoAnchoPts = pdfFontSize;
                            float puntoAltoPts = pdfFontSize;
                            if (layer.PuntoPredeterminado == 1)
                            {
                                //Circulo
                                float radio = pdfFontSize / 2;
                                PDFUtilities.DrawPDFCircle(pdfContentByte, (x1c + pdfFontSize / 2), (y1c + pdfFontSize / 2), radio, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
                            else if (layer.PuntoPredeterminado == 2)
                            {
                                //Cuadrado
                                PDFUtilities.DrawPDFRectangle(pdfContentByte, x1c, y1c, puntoAnchoPts, puntoAltoPts, pdfContornoColor, pdfContornoGrosor, pdfRellenoColor);
                            }
                        }
                        x2c = x2c + pdfFontSize / 2;
                        float yTexto = y1c;
                        //PDFUtilities.DrawPDFText(pdfContentByte, texto, x2c, yTexto, pdfFont, pdfFontSize, alignment);
                        PDFUtilities.DrawPDFTextMax(pdfContentByte, texto, x2c, ref y1c, xMaxReferencia, pdfFont, pdfFontSize, alignment);

                        y1c = y1c - (pdfFontSize + espaciado) * 2;
                        if (y1c <= yMinReferencia)
                        {
                            pdfDoc.NewPage();
                            x1c = xMinReferencia + pdfFontSize / 2;
                            y1c = yMaxReferencia - espaciado - pdfFontSize;
                            //y1c = yMaxReferencia - espaciado - pdfFontSize;
                            //col++;
                            //if (col > plantilla.ReferenciaColumnas)
                            //{
                            //    break;
                            //}
                            //else
                            //{
                            //    x1c += ((xMaxReferencia - xMinReferencia) / plantilla.ReferenciaColumnas);
                            //}
                        }
                    }
                    #endregion
                }
                if (nombreCallesEnPlano)
                {
                    Color colorTextoCalle = System.Drawing.ColorTranslator.FromHtml(textoCalleColor);
                    it.BaseColor pdfColorTextoCalle = new it.BaseColor(colorTextoCalle.R, colorTextoCalle.G, colorTextoCalle.B);
                    //float pdfFontSizeCalle = it.Utilities.MillimetersToPoints((float)textoCalleFuenteTamanio);
                    PDFUtilities.RegisterBaseFont(textoCalleFuenteNombre, pdfFontSize);
                    //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                    string[] aFontStyleCalle = textoCalleFuenteEstilo.Split(',');
                    int pdfFontStyleCalle = aFontStyleCalle.Select(x => Convert.ToInt32(x)).Sum();
                    BaseFont pdfbaseFontCalle = it.FontFactory.GetFont(textoCalleFuenteNombre, pdfFontSize, pdfFontStyleCalle, pdfColorTextoCalle).BaseFont;
                    it.Font pdfFontCalle = new it.Font(pdfbaseFontCalle, pdfFontSize, pdfFontStyleCalle, pdfColorTextoCalle);
                    y1c = y1c - (pdfFontSize + espaciado);
                    PDFUtilities.DrawPDFText(pdfContentByte, "CALLE", x1c, y1c, pdfFontCalle, pdfFontSize, alignment);

                    x2c = x1c + pdfFontSize * 4;
                    PDFUtilities.DrawPDFText(pdfContentByte, "NOMBRES DE CALLE", x2c, y1c, pdfFont, pdfFontSize, alignment);
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return ret;
        }

        public bool FuncionEspecial1(PdfContentByte pdfContentByte, Plantilla plantilla, string idsObjetoSecundario, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff)
        {
            bool ret = true;
            ParcelaPlot[] aParcelaPlot = null;//_parcelaPlotRepository.GetParcelaPlotByCoords(xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
            if (aParcelaPlot != null && aParcelaPlot.Count() > 0)
            {
                DibujarDatosParcelasTest(pdfContentByte, plantilla, idsObjetoSecundario, aParcelaPlot);
            }
            return ret;
        }
        private void DibujarDatosParcelasTest(PdfContentByte pdfContentByte, Plantilla plantilla, string idsObjetoSecundario, ParcelaPlot[] aParcelaPlot)
        {
            double xMin = 0;
            double yMin = 0;
            double xMax = 0;
            double yMax = 0;
            double espaciadoMM = 1.41083;
            string fuenteNombre = "Arial";
            double fuenteTamanio = 2.1166;
            string fuenteColor = "#808080";
            string fuenteEstilo = "0,0,0,0";
            int maxRows = 30;

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.CurrencyDecimalDigits = 4;
            numberFormat.CurrencyDecimalSeparator = ".";
            numberFormat.NumberDecimalDigits = 4;
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.PercentDecimalDigits = 2;
            numberFormat.PercentDecimalSeparator = ".";

            #region Parametros de la DB - xMin yMin etc
            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "XMIN");
            if (funcAdicParametro != null)
            {
                //xMin = Convert.ToDouble(funcAdicParametro.Valor.ToString(CultureInfo.InvariantCulture));
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMin);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "YMIN");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMin);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "XMAX");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMax);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "YMAX");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMax);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "ESPACIADO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out espaciadoMM);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "FUENTE_NOMBRE");
            if (funcAdicParametro != null)
            {
                fuenteNombre = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "FUENTE_COLOR");
            if (funcAdicParametro != null)
            {
                fuenteColor = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "FUENTE_ESTILO");
            if (funcAdicParametro != null)
            {
                fuenteEstilo = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "FUENTE_TAMANIO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out fuenteTamanio);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.Campo.ToUpper() == "MAX_ROWS");
            if (funcAdicParametro != null)
            {
                maxRows = ToolsTypes.ToInt32(funcAdicParametro.Valor);
            }
            #endregion
            float xMinReferencia = it.Utilities.MillimetersToPoints((float)xMin);
            float yMinReferencia = it.Utilities.MillimetersToPoints((float)yMin);
            float xMaxReferencia = it.Utilities.MillimetersToPoints((float)xMax);
            float yMaxReferencia = it.Utilities.MillimetersToPoints((float)yMax);
            float espaciado = it.Utilities.MillimetersToPoints((float)espaciadoMM);

            //DrawPDFRectangle2(pdfContentByte, xMinReferencia, yMinReferencia, xMaxReferencia, yMaxReferencia, new it.BaseColor(Color.Gray.R, Color.Gray.G, Color.Gray.B), 1.0f, null, null);

            Color colorFuente = System.Drawing.ColorTranslator.FromHtml(fuenteColor);
            it.BaseColor pdfColorFuente = new it.BaseColor(colorFuente.R, colorFuente.G, colorFuente.B);
            float pdfFontSize = it.Utilities.MillimetersToPoints((float)fuenteTamanio);
            PDFUtilities.RegisterBaseFont(fuenteNombre, pdfFontSize);
            //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
            string[] aFontStyle = fuenteEstilo.Split(',');
            int pdfFontStyle = aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
            BaseFont pdfbaseFont = it.FontFactory.GetFont(fuenteNombre, pdfFontSize, pdfFontStyle, pdfColorFuente).BaseFont;
            it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorFuente);

            string[] aFontStyleBold = fuenteEstilo.Split(',');
            aFontStyleBold[0] = "1";
            int pdfFontStyleBold = aFontStyleBold.Select(x => Convert.ToInt32(x)).Sum();
            it.Font pdfFontBold = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyleBold, pdfColorFuente);

            float x1c = xMinReferencia;
            //float y1c = yMaxReferencia - espaciado - pdfFontSize;
            //float y1c = yMaxReferencia - espaciado;
            float y1c = yMaxReferencia;
            float x2c = 0;
            float y2c = 0;
            int cols = 2;
            int iRow = 1;

            PdfPTable pdfTable = new PdfPTable(2);
            float tableWidth = xMaxReferencia - xMinReferencia - (pdfFontSize / 2);
            pdfTable.TotalWidth = tableWidth;
            float[] widths = { tableWidth / 2, tableWidth / 2 };
            pdfTable.SetWidths(widths);
            bool drawBorder = false;
            //pdfTable.AddCell(new it.Phrase("Parcela", pdfFontBold));
            //pdfTable.AddCell(new it.Phrase("Superficie", pdfFontBold));
            PdfPCell pdfCell = new PdfPCell();
            int alignmentCol1 = it.Element.ALIGN_CENTER;
            int alignmentCol2 = it.Element.ALIGN_RIGHT;
            pdfCell = PDFUtilities.GetCellForTable("Parcela", pdfFontBold, alignmentCol1, espaciado, drawBorder);
            pdfTable.AddCell(pdfCell);
            pdfCell = new PdfPCell();
            pdfCell = PDFUtilities.GetCellForTable("Superficie", pdfFontBold, alignmentCol2, espaciado, drawBorder);
            pdfTable.AddCell(pdfCell);
            float sizeTexto = pdfbaseFont.GetWidthPoint("99999.00", pdfFontSize);
            //x1c = x1c + (pdfFontSize / 2);
            //x2c = xMinReferencia + (xMaxReferencia - xMinReferencia) / cols + (pdfFontSize / 2);
            foreach (var parcelaPlot in aParcelaPlot)
            {
                string texto = parcelaPlot.ParcelaNro;
                string superficie = Math.Round(parcelaPlot.Superficie, 2).ToString();
                //Rectangle rectTextoCol1 = new Rectangle(x1c, y1c, (int)Math.Ceiling(sizeTexto.Width), (int)Math.Ceiling(sizeTexto.Height));
                //Rectangle rectTextoCol2 = new Rectangle(x2c, y1c, (int)sizeTexto.Width, (int)sizeTexto.Height);
                pdfCell = new PdfPCell();
                if (iRow <= maxRows)
                {
                    it.Font pdfFontText = pdfFont;
                    if (idsObjetoSecundario != null && idsObjetoSecundario != string.Empty && idsObjetoSecundario.Contains(parcelaPlot.FeatId.ToString()))
                    {
                        pdfFontText = pdfFontBold;
                    }
                    //DrawPDFText(pdfContentByte, texto, x1c, y1c, pdfFontText, pdfFontSize, alignment);
                    pdfCell = PDFUtilities.GetCellForTable(texto, pdfFontText, alignmentCol1, espaciado, drawBorder);
                    pdfTable.AddCell(pdfCell);
                    //DrawPDFText(pdfContentByte, superficie, x2c, y1c, pdfFontText, pdfFontSize, alignment);
                    pdfCell = PDFUtilities.GetCellForTable(superficie, pdfFontText, alignmentCol2, espaciado, drawBorder);
                    pdfTable.AddCell(pdfCell);
                }
                else
                {
                    texto = ". . .";
                    //DrawPDFText(pdfContentByte, texto, x1c, y1c, pdfFont, pdfFontSize, alignment);
                    pdfCell = PDFUtilities.GetCellForTable(texto, pdfFont, alignmentCol1, espaciado, drawBorder);
                    pdfTable.AddCell(pdfCell);
                    pdfCell = PDFUtilities.GetCellForTable(". . .", pdfFont, alignmentCol2, espaciado, drawBorder);
                    pdfTable.AddCell(pdfCell);
                    break;
                }
                //y1c = y1c - pdfFontSize - espaciado;
                iRow++;
            }
            pdfTable.WriteSelectedRows(0, -1, x1c, y1c, pdfContentByte);
        }

        public bool FuncionEspecial1(Graphics graphics, Plantilla plantilla, string idsObjetoSecundario, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff)
        {
            bool ret = true;
            ParcelaPlot[] aParcelaPlot = null;// _parcelaPlotRepository.GetParcelaPlotByCoords(xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
            if (aParcelaPlot != null && aParcelaPlot.Count() > 0)
            {
                DibujarDatosParcelasTest(graphics, plantilla, idsObjetoSecundario, aParcelaPlot);
            }
            return ret;
        }

        private void DibujarDatosParcelasTest(Graphics graphics, Plantilla plantilla, string idsObjetoSecundario, ParcelaPlot[] aParcelaPlot)
        {
            int idFuncionAdicional = 1;
            double xMin = 0;
            double yMin = 0;
            double xMax = 0;
            double yMax = 0;
            double espaciadoMM = 0.3386;
            string fuenteNombre = "Arial";
            double fuenteTamanio = 0.508;
            string fuenteColor = "#808080";
            string fuenteEstilo = "0,0,0,0";
            int maxRows = 30;

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.CurrencyDecimalDigits = 4;
            numberFormat.CurrencyDecimalSeparator = ".";
            numberFormat.NumberDecimalDigits = 4;
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.PercentDecimalDigits = 2;
            numberFormat.PercentDecimalSeparator = ".";

            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "XMIN");
            if (funcAdicParametro != null)
            {
                //xMin = Convert.ToDouble(funcAdicParametro.Valor.ToString(CultureInfo.InvariantCulture));
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMin);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "YMIN");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMin);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "XMAX");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMax);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "YMAX");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMax);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out espaciadoMM);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
            if (funcAdicParametro != null)
            {
                fuenteNombre = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
            if (funcAdicParametro != null)
            {
                fuenteColor = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
            if (funcAdicParametro != null)
            {
                fuenteEstilo = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out fuenteTamanio);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MAX_ROWS");
            if (funcAdicParametro != null)
            {
                maxRows = ToolsTypes.ToInt32(funcAdicParametro.Valor);
            }
            int xMinReferencia = GetPixels(xMin, plantilla.PPP);
            int yMinReferencia = GetPixels(yMin, plantilla.PPP);
            int xMaxReferencia = GetPixels(xMax, plantilla.PPP);
            int yMaxReferencia = GetPixels(yMax, plantilla.PPP);
            int espaciado = GetPixels(espaciadoMM, plantilla.PPP);
            int x1c = xMinReferencia;
            int y1c = yMinReferencia + espaciado;
            int x2c = 0;
            int y2c = 0;
            int cols = 2;
            int iRow = 1;
            Color colorFuente = System.Drawing.ColorTranslator.FromHtml(fuenteColor);

            //int widthLimites = 4;
            //Pen penLimites = new Pen(colorReferencia, widthLimites);
            //graphics.DrawLine(penLimites, xMinReferencia, yMinReferencia, xMaxReferencia, yMinReferencia);
            //graphics.DrawLine(penLimites, xMaxReferencia, yMinReferencia, xMaxReferencia, yMaxReferencia);
            //graphics.DrawLine(penLimites, xMaxReferencia, yMaxReferencia, xMinReferencia, yMaxReferencia);
            //graphics.DrawLine(penLimites, xMinReferencia, yMaxReferencia, xMinReferencia, yMinReferencia);

            int fontSizeRef = GetPixels(fuenteTamanio, plantilla.PPP);
            string[] aFontStyle = fuenteEstilo.Split(',');
            FontStyle fontStyle = (FontStyle)aFontStyle.Select(x => Convert.ToInt32(x)).Sum();
            string[] aFontStyleBold = fuenteEstilo.Split(',');
            aFontStyleBold[0] = "1";
            FontStyle fontStyleBold = (FontStyle)aFontStyleBold.Select(x => Convert.ToInt32(x)).Sum();
            using (Font fontTexto = new Font(fuenteNombre, fontSizeRef, fontStyle))
            using (Font fontTextoBold = new Font(fuenteNombre, fontSizeRef, fontStyleBold))
            using (Brush brushTexto = new SolidBrush(colorFuente))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.LineAlignment = StringAlignment.Center;
                stringFormat.Alignment = StringAlignment.Near;

                SizeF sizeTexto = graphics.MeasureString("99999.00", fontTexto);
                x1c += (int)((int)sizeTexto.Height / 2);
                x2c = xMinReferencia + (xMaxReferencia - xMinReferencia) / cols + (int)((int)sizeTexto.Height / 2);
                foreach (var parcelaPlot in aParcelaPlot)
                {
                    string texto = parcelaPlot.ParcelaNro;
                    string superficie = Math.Round(parcelaPlot.Superficie, 2).ToString();
                    Rectangle rectTextoCol1 = new Rectangle(x1c, y1c, (int)Math.Ceiling(sizeTexto.Width), (int)Math.Ceiling(sizeTexto.Height));
                    Rectangle rectTextoCol2 = new Rectangle(x2c, y1c, (int)sizeTexto.Width, (int)sizeTexto.Height);
                    if (iRow <= maxRows)
                    {
                        if (idsObjetoSecundario != null && idsObjetoSecundario != string.Empty && idsObjetoSecundario.Contains(parcelaPlot.FeatId.ToString()))
                        {
                            //graphics.DrawString(texto, fontTextoBold, brushTexto, new Point(x1c, y1c));
                            stringFormat.Alignment = StringAlignment.Near;
                            graphics.DrawString(texto, fontTextoBold, brushTexto, rectTextoCol1, stringFormat);
                            stringFormat.Alignment = StringAlignment.Far;
                            graphics.DrawString(superficie, fontTextoBold, brushTexto, rectTextoCol2, stringFormat);
                        }
                        else
                        {
                            //graphics.DrawString(texto, fontTexto, brushTexto, new Point(x1c, y1c));
                            stringFormat.Alignment = StringAlignment.Near;
                            graphics.DrawString(texto, fontTexto, brushTexto, rectTextoCol1, stringFormat);
                            stringFormat.Alignment = StringAlignment.Far;
                            graphics.DrawString(superficie, fontTexto, brushTexto, rectTextoCol2, stringFormat);
                            //graphics.DrawRectangle(Pens.Black, rectTexto);
                        }
                    }
                    else
                    {
                        texto = ". . .";
                        graphics.DrawString(texto, fontTexto, brushTexto, new Point(x1c, y1c));
                        break;
                    }
                    y1c += (int)sizeTexto.Height + espaciado;
                    iRow++;
                }
            }
        }


        public bool FuncionEspecialDibujarCotas(PdfContentByte pdfContentByte, Plantilla plantilla, Componente componenteBase, string idObjetoBase, List<string> lstCoordenadas, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff, double xCentroidBase, double yCentroidBase, double factorEscala, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, ref ParcelaPlot[] aParcelaPlot)
        {
            bool ret = true;
            int idFuncionAdicional = 2;
            int filtroGeografico = 1;
            //Parametros
            string esquema = "gis_aysa_dev";
            string tabla = "CT_PARCELA";
            string campoFeatId = "ID_PARCELA";
            string campoGeometry = "GEOMETRY";
            string campoNroPuerta = "NRO_PUERTA";
            string campoIdCuadra = "ID_CUADRA";
            //Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
            int anguloTolerancia = 2;
            double distanciaTolerancia = 0.1;
            double desplazamientoCotas = 1.5;
            string textoFuenteNombre = "Arial";
            double textoFuenteTamanio = 0.5;
            string textoFuenteEstilo = "0,0,0,0";
            string textoColor = "#808080";

            #region NumberFormat
            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.CurrencyDecimalDigits = 4;
            numberFormat.CurrencyDecimalSeparator = ".";
            numberFormat.NumberDecimalDigits = 4;
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.PercentDecimalDigits = 2;
            numberFormat.PercentDecimalSeparator = ".";
            #endregion

            #region Parametros de la DB
            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquema = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "TABLA");
            if (funcAdicParametro != null)
            {
                tabla = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_ID");
            if (funcAdicParametro != null)
            {
                campoFeatId = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_GEOMETRY");
            if (funcAdicParametro != null)
            {
                campoGeometry = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_NRO_PUERTA");
            if (funcAdicParametro != null)
            {
                campoNroPuerta = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_ID_CUADRA");
            if (funcAdicParametro != null)
            {
                campoIdCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ANGULO_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaTolerancia);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DESPLAZAMIENTO_COTAS");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out desplazamientoCotas);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
            if (funcAdicParametro != null)
            {
                textoFuenteNombre = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
            if (funcAdicParametro != null)
            {
                textoColor = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
            if (funcAdicParametro != null)
            {
                textoFuenteEstilo = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor.Replace(',', '.'), NumberStyles.Any, numberFormat, out textoFuenteTamanio);
            }

            #endregion

            Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);

            //ParcelaPlot[] aParcelaPlot = null;
            if (aParcelaPlot == null || aParcelaPlot.Length == 0)
            {
                if (lstCoordenadas != null && lstCoordenadas.Count > 0)
                {
                    //aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, lstCoordenadas);
                    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByObjetoBase(componenteBase, esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, idObjetoBase);
                }
                else
                {
                    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
                }
            }
            if (aParcelaPlot != null && aParcelaPlot.Count() > 0)
            {
                if ((1 / factorEscala) >= 5000)
                {
                    textoFuenteTamanio = textoFuenteTamanio / 4.0;
                }
                int i = 0;
                foreach (var parcelaPlot in aParcelaPlot)
                {
                    DbGeometry geometryParcelaPlot = (DbGeometry)parcelaPlot.GetType().GetProperty("Geom").GetValue(parcelaPlot);

                    //List<double> lstLimite = GetLimite(geometryParcelaPlot, toleranciaCambioVertice);

                    PointF puntoMedio = new PointF();
                    List<Lado> lados = new List<Lado>();
                    Lado ladoMayor = new Lado();
                    double anguloRotacionLadoMayor = GetAnguloRotacion(geometryParcelaPlot, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);

                    //Paso a radianes
                    anguloRotacionLadoMayor = anguloRotacionLadoMayor * Math.PI / 180;

                    #region Para Testing - Dibujo lados de la geometryBase
                    //float pdfx1c = 0, pdfy1c = 0, pdfx2c = 0, pdfy2c = 0;
                    //Random rand = new Random();
                    //foreach (var lado in lados)
                    //{
                    //    Color rndColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                    //    foreach (var segmento in lado.Segmentos)
                    //    {
                    //        pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                    //        pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                    //        pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                    //        pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                    //        it.BaseColor pdfContornoColorRnd = new it.BaseColor(rndColor.R, rndColor.G, rndColor.B);
                    //        PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, it.Utilities.MillimetersToPoints((float)1.27), pdfContornoColorRnd);
                    //    }
                    //}
                    //foreach (var segmento in ladoMayor.Segmentos)
                    //{
                    //    pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                    //    pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                    //    pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                    //    pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                    //    PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, it.Utilities.MillimetersToPoints((float)1.27), it.BaseColor.BLACK);
                    //}
                    #endregion

                    DibujarCotas(pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, parcelaPlot, lados, desplazamientoCotas, esquema, tabla, textoFuenteNombre, textoFuenteTamanio, textoFuenteEstilo, colorTexto, anguloRotacion, sentidoHorario, anguloRotacionFiltro, anguloRotacionLadoMayor);
                    i++;
                }
            }
            return ret;
        }

        private void DibujarCotas(PdfContentByte pdfContentByte, Plantilla plantilla, double xCentroidBase, double yCentroidBase, double factorEscala, ParcelaPlot parcelaPlot, List<Lado> lstLado, double desplazamientoCotas, string esquema, string tabla, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color textoColor, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, double anguloRotacionLadoMayor)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(textoColor.R, textoColor.G, textoColor.B);
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;

                double x = 0, y = 0;
                //double x1 = 0, y1 = 0;
                //double x2 = 0, y2 = 0;
                //double x0 = 0, y0 = 0;
                double xMin = 9999999, yMin = 9999999;
                double xMax = 0, yMax = 0;
                double beta = 0;
                double rotation = 0;
                string sDistancia = string.Empty;
                float xPdf = 0;
                float yPdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                foreach (var lado in lstLado)
                {
                    xMin = 9999999;
                    yMin = 9999999;
                    xMax = 0;
                    yMax = 0;
                    foreach (var segmento in lado.Segmentos)
                    {
                        //pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                        #region Determinar xMin yMin xMax yMax
                        if (xMin >= segmento.P1.X)
                        {
                            xMin = segmento.P1.X;
                        }
                        if (xMax < segmento.P1.X)
                        {
                            xMax = segmento.P1.X;
                        }
                        if (yMin >= segmento.P1.Y)
                        {
                            yMin = segmento.P1.Y;
                        }
                        if (yMax < segmento.P1.Y)
                        {
                            yMax = segmento.P1.Y;
                        }
                        if (xMin >= segmento.P2.X)
                        {
                            xMin = segmento.P2.X;
                        }
                        if (xMax < segmento.P2.X)
                        {
                            xMax = segmento.P2.X;
                        }
                        if (yMin >= segmento.P2.Y)
                        {
                            yMin = segmento.P2.Y;
                        }
                        if (yMax < segmento.P2.Y)
                        {
                            yMax = segmento.P2.Y;
                        }
                        #endregion

                    }
                    sDistancia = (Math.Truncate(100 * lado.Distancia) / 100).ToString();

                    #region Codigo Comentado
                    //if (plantilla.OptimizarTamanioHoja)
                    //{
                    //    double angulo = 0;
                    //    if (xMax - xMin == 0)
                    //    {
                    //        angulo = Math.PI / 2;
                    //    }
                    //    else
                    //    {
                    //        PointF ptMinRotado = Rotate((float)xMin, (float)yMin, (float)(xCentroidBase), (float)(yCentroidBase), anguloRotacionLadoMayor, sentidoHorario);
                    //        PointF ptMaxRotado = Rotate((float)xMax, (float)yMax, (float)(xCentroidBase), (float)(yCentroidBase), anguloRotacionLadoMayor, sentidoHorario);
                    //        //angulo = Math.Atan((yMax - yMin) / (xMax - xMin));
                    //        angulo = Math.Atan((ptMaxRotado.Y - ptMinRotado.Y) / (ptMaxRotado.X - ptMinRotado.X));
                    //    }
                    //    angulo = angulo * 180 / Math.PI;

                    //    rotation = angulo;
                    //}
                    //else
                    //{
                    //rotation = lado.Angulo;
                    //}
                    #endregion

                    rotation = lado.Angulo;
                    beta = rotation;
                    if (rotation < 0)
                    {
                        beta = rotation + 360;
                    }

                    double alfa = beta;

                    double textRotation = rotation;

                    if (alfa >= 0 && alfa <= 90)
                    {
                        beta = 90 - alfa;
                        //textRotation = alfa + 90 + 180;
                    }
                    else if (alfa > 90 && alfa <= 180)
                    {
                        beta = alfa - 90;
                        //textRotation = beta;
                    }
                    else if (alfa > 180 && alfa <= 270)
                    {
                        beta = 270 - alfa;
                        //textRotation = alfa - 90;
                    }
                    else if (alfa > 270 && alfa <= 360)
                    {
                        beta = alfa - 270;
                        //textRotation = beta;
                    }

                    double betaRad = beta * Math.PI / 180.0;

                    x = xMin + (xMax - xMin) / 2;
                    y = yMin + (yMax - yMin) / 2;

                    double desplazamientoX = desplazamientoCotas * Math.Cos(betaRad);
                    double desplazamientoY = desplazamientoCotas * Math.Sin(betaRad);

                    double x1Des = x + desplazamientoCotas * Math.Cos(betaRad);
                    double y1Des = y + desplazamientoCotas * Math.Sin(betaRad);
                    double x2Des = x - desplazamientoCotas * Math.Cos(betaRad);
                    double y2Des = y - desplazamientoCotas * Math.Sin(betaRad);

                    x1Des = x + desplazamientoX;
                    x2Des = x - desplazamientoX;
                    if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
                    {
                        y1Des = y - desplazamientoY;
                        y2Des = y + desplazamientoY;
                        //textRotation = textRotation + 180;
                    }
                    else if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
                    {
                        y1Des = y + desplazamientoY;
                        y2Des = y - desplazamientoY;
                    }

                    #region Test - Dibujo ptos para testing
                    //TODO Test - Dibujo ptos para testing
                    Color colorTexto2 = System.Drawing.ColorTranslator.FromHtml("#006400");
                    it.BaseColor pdfColor2 = new it.BaseColor(colorTexto2.R, colorTexto2.G, colorTexto2.B);
                    //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x1Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y1Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                    //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x2Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y2Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColor2, (float)0.1, pdfColor2, 50);
                    //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                    //PDFUtilities.DrawPDFText(pdfContentByte, (Math.Truncate(100 * beta) / 100).ToString(), GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), pdfbaseFont, pdfFontSize, colorTexto, lado.Angulo);
                    ////
                    #endregion

                    //DbGeometry geomPtoDes = DbGeometry.FromText("POINT(" + x1Des + " " + y1Des + ")");
                    string wktPointDes = "POINT(" + x1Des + " " + y1Des + ")";
                    //AYSA20180425 se agrego replace
                    wktPointDes = wktPointDes.Replace(',', '.');
                    DbGeometry geomPtoDes = DbGeometry.PointFromText(wktPointDes, 22195);
                    bool contiene = parcelaPlot.Geom.Contains(geomPtoDes);

                    //ParcelaPlot[] aParcelaPlotDes = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, x1Des, y1Des);
                    //if (aParcelaPlotDes != null && aParcelaPlotDes.Length > 0 && parcelaPlot.FeatId == aParcelaPlotDes[0].FeatId)
                    if (contiene)
                    {
                        x = x1Des;
                        y = y1Des;
                        if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
                        {
                            textRotation = textRotation + 180;
                        }
                    }
                    else
                    {
                        x = x2Des;
                        y = y2Des;
                        if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
                        {
                            textRotation = textRotation + 180;
                        }
                    }
                    if (textRotation > 360)
                    {
                        textRotation = textRotation - 360;
                    }
                    xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
                    yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
                    if (plantilla.OptimizarTamanioHoja)
                    {
                        //textRotation = textRotation - anguloRotacionFiltro;

                        //Le aplico la rotacion al angulo del lado en coords normales por eso uso anguloRotacionFiltro (en grados)
                        textRotation = textRotation - (anguloRotacionFiltro * 180 / Math.PI);
                        //Paso a positivo
                        if (textRotation < 0)
                        {
                            textRotation = textRotation + 360;
                        }
                        //Invierto el angulo
                        //textRotation = textRotation + 180;
                        //Paso a cuadrante normal si se pasa del IV cuadrante
                        if (textRotation > 360)
                        {
                            textRotation = textRotation - 360;
                        }
                        //Es lo mismo rotar el pto original que el pto del pdf
                        //PointF ptRotado = Rotate((float)x, (float)y, (float)xCentroidBase, (float)yCentroidBase, anguloRotacion, sentidoHorario);
                        //xPdf = GetXPDFCanvas(ptRotado.X, xCentroidBase, factorEscala, plantilla);
                        //yPdf = GetYPDFCanvas(ptRotado.Y, yCentroidBase, factorEscala, plantilla);
                        //PDFUtilities.DrawPDFText(pdfContentByte, sDistancia, xPdf, yPdf, pdfbaseFont, pdfFontSize, textoColor, textRotation);

                        PointF ptRotado = Rotate(xPdf, yPdf, xCentro, yCentro, anguloRotacion, sentidoHorario);

                        PDFUtilities.DrawPDFText(pdfContentByte, sDistancia, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, textoColor, textRotation);
                    }
                    else
                    {
                        //float sizeText = pdfbaseFont.GetWidthPoint(sDistancia + "xx", pdfFontSize);
                        //xPdf = xPdf - sizeText / 2;
                        //PDFUtilities.DrawPDFCircle(pdfContentByte, xPdf, yPdf, it.Utilities.MillimetersToPoints((float)0.1), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                        //PDFUtilities.DrawPDFText(pdfContentByte, sDistancia, xPdf, yPdf, pdfbaseFont, pdfFontSize, textoColor, rotation);
                        PDFUtilities.DrawPDFText(pdfContentByte, sDistancia, xPdf, yPdf, pdfbaseFont, pdfFontSize, textoColor, textRotation);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        public bool FuncionEspecialDibujarNroPuerta(PdfContentByte pdfContentByte, Plantilla plantilla, Componente componenteBase, string idObjetoBase, List<string> lstCoordenadas, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff, double xCentroidBase, double yCentroidBase, double factorEscala, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, ref ParcelaPlot[] aParcelaPlot)
        {
            bool ret = true;
            int idFuncionAdicional = 3;
            int filtroGeografico = 1;
            //Parametros
            string esquema = "gis_aysa_dev";
            string tabla = "CT_PARCELA";
            string campoFeatId = "ID_PARCELA";
            string campoGeometry = "GEOMETRY";
            string campoNroPuerta = "NRO_PUERTA";
            string campoIdCuadra = "ID_CUADRA";
            //Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
            int anguloTolerancia = 2;
            double distanciaTolerancia = 0.1;
            //double distanciaLadoOchava = 10;
            double distBuffer = 2;
            double desplazamientoNroPuerta = 1.5;
            string textoFuenteNombre = "Arial";
            double textoFuenteTamanio = 0.5;
            string textoFuenteEstilo = "0,0,0,0";
            string textoColor = "#808080";

            string esquemaCuadra = "gis_aysa_dev";
            string tablaCuadra = "CT_CUADRA";
            string campoGeometryCuadra = "GEOMETRY";
            string campoIdCuadraCuadra = "ID_CUADRA";

            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.CurrencyDecimalDigits = 4;
            numberFormat.CurrencyDecimalSeparator = ".";
            numberFormat.NumberDecimalDigits = 4;
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.PercentDecimalDigits = 2;
            numberFormat.PercentDecimalSeparator = ".";

            #region Parametros de la DB
            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquema = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "TABLA");
            if (funcAdicParametro != null)
            {
                tabla = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_ID");
            if (funcAdicParametro != null)
            {
                campoFeatId = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_GEOMETRY");
            if (funcAdicParametro != null)
            {
                campoGeometry = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_NRO_PUERTA");
            if (funcAdicParametro != null)
            {
                campoNroPuerta = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CAMPO_ID_CUADRA");
            if (funcAdicParametro != null)
            {
                campoIdCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ANGULO_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaTolerancia);
            }
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_LADO_OCHAVA");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaLadoOchava);
            //}
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_BUFFER");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distBuffer);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DESPLAZAMIENTO_NRO_PUERTA");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out desplazamientoNroPuerta);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
            if (funcAdicParametro != null)
            {
                textoFuenteNombre = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
            if (funcAdicParametro != null)
            {
                textoColor = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
            if (funcAdicParametro != null)
            {
                textoFuenteEstilo = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
            }

            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquemaCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_TABLA");
            if (funcAdicParametro != null)
            {
                tablaCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_GEOMETRY");
            if (funcAdicParametro != null)
            {
                campoGeometryCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_CUADRA");
            if (funcAdicParametro != null)
            {
                campoIdCuadraCuadra = funcAdicParametro.Valor;
            }
            #endregion

            Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);

            //ParcelaPlot[] aParcelaPlot = null;
            if (aParcelaPlot == null || aParcelaPlot.Length == 0)
            {
                if (lstCoordenadas != null && lstCoordenadas.Count > 0)
                {
                    //aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, lstCoordenadas);
                    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByObjetoBase(componenteBase, esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, idObjetoBase);
                }
                else
                {
                    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
                }
            }
            if (aParcelaPlot != null && aParcelaPlot.Count() > 0)
            {
                if ((1 / factorEscala) >= 5000)
                {
                    textoFuenteTamanio = textoFuenteTamanio / 4.0;
                }
                foreach (var parcelaPlot in aParcelaPlot)
                {
                    DbGeometry geometryParcelaPlot = (DbGeometry)parcelaPlot.GetType().GetProperty("Geom").GetValue(parcelaPlot);

                    //List<double> lstLimite = GetLimite(geometryParcelaPlot, toleranciaCambioVertice);

                    PointF puntoMedio = new PointF();
                    List<Lado> lados = new List<Lado>();
                    Lado ladoMayor = new Lado();
                    double anguloRotacionLadoMayor = GetAnguloRotacion(geometryParcelaPlot, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);
                    //Paso a radianes
                    anguloRotacionLadoMayor = anguloRotacionLadoMayor * Math.PI / 180;

                    #region Para Testing - Dibujo lados de la geometryBase
                    //float pdfx1c = 0, pdfy1c = 0, pdfx2c = 0, pdfy2c = 0;
                    //Random rand = new Random();
                    //foreach (var lado in lados)
                    //{
                    //    Color rndColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                    //    foreach (var segmento in lado.Segmentos)
                    //    {
                    //        pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                    //        pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                    //        pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                    //        pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                    //        it.BaseColor pdfContornoColorRnd = new it.BaseColor(rndColor.R, rndColor.G, rndColor.B);
                    //        PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, it.Utilities.MillimetersToPoints((float)1.27), pdfContornoColorRnd);
                    //    }
                    //}
                    //foreach (var segmento in ladoMayor.Segmentos)
                    //{
                    //    pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                    //    pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                    //    pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                    //    pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                    //    PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, it.Utilities.MillimetersToPoints((float)1.27), it.BaseColor.BLACK);
                    //}
                    #endregion

                    DibujarNroPuerta(pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, parcelaPlot, lados, desplazamientoNroPuerta, esquema, tabla, campoGeometry, campoIdCuadra, esquemaCuadra, tablaCuadra, campoGeometryCuadra, campoIdCuadraCuadra, distBuffer, textoFuenteNombre, textoFuenteTamanio, textoFuenteEstilo, colorTexto, anguloRotacion, sentidoHorario, anguloRotacionFiltro, anguloRotacionLadoMayor);
                }
            }
            return ret;
        }

        private void DibujarNroPuerta(PdfContentByte pdfContentByte, Plantilla plantilla, double xCentroidBase, double yCentroidBase, double factorEscala, ParcelaPlot parcelaPlot, List<Lado> lstLado, double desplazamientoNroPuerta, string esquema, string tabla, string campoGeometry, string campoIdCuadra, string esquemaCuadra, string tablaCuadra, string campoGeometryCuadra, string campoIdCuadraCuadra, double distBuffer, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color colorTexto, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, double anguloRotacionLadoMayor)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;

                double x = 0, y = 0;
                double xMin = 9999999, yMin = 9999999;
                double xMax = 0, yMax = 0;
                double beta = 0;
                double rotation = 0;
                string sDistancia = string.Empty;
                float xPdf = 0;
                float yPdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                string nroPuerta = string.Empty;
                double x1Cuadra = 0;
                double y1Cuadra = 0;
                double x2Cuadra = 0;
                double y2Cuadra = 0;
                CuadraPlot[] aCuadraPlot = _cuadraPlotRepository.GetCuadraPlotByIdCuadra(esquemaCuadra, tablaCuadra, campoGeometryCuadra, campoIdCuadra, parcelaPlot.IdCuadra);
                if (aCuadraPlot != null && aCuadraPlot.Length > 0)
                {
                    CuadraPlot cuadra = aCuadraPlot[0];
                    DbGeometry geometryCuadra = (DbGeometry)cuadra.GetType().GetProperty("Geom").GetValue(cuadra);
                    string wkt = geometryCuadra.AsText();
                    if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                    {
                        int cantCoords = (int)geometryCuadra.PointCount;
                        x1Cuadra = (double)geometryCuadra.PointAt(1).XCoordinate;
                        y1Cuadra = (double)geometryCuadra.PointAt(1).YCoordinate;
                        x2Cuadra = (double)geometryCuadra.PointAt(cantCoords).XCoordinate;
                        y2Cuadra = (double)geometryCuadra.PointAt(cantCoords).YCoordinate;
                    }
                    List<Lado> lstLadoCuadra = new List<Lado>();
                    foreach (var lado in lstLado)
                    {
                        xMin = 9999999;
                        yMin = 9999999;
                        xMax = 0;
                        yMax = 0;
                        List<string> lstCoordsGeometry = new List<string>();
                        List<string> lstCoordsGeometryLado = new List<string>();
                        string coordsLado = string.Empty;
                        foreach (var segmento in lado.Segmentos)
                        {
                            //pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                            #region Determinar xMin yMin xMax yMax
                            if (xMin >= segmento.P1.X)
                            {
                                xMin = segmento.P1.X;
                            }
                            if (xMax < segmento.P1.X)
                            {
                                xMax = segmento.P1.X;
                            }
                            if (yMin >= segmento.P1.Y)
                            {
                                yMin = segmento.P1.Y;
                            }
                            if (yMax < segmento.P1.Y)
                            {
                                yMax = segmento.P1.Y;
                            }
                            if (xMin >= segmento.P2.X)
                            {
                                xMin = segmento.P2.X;
                            }
                            if (xMax < segmento.P2.X)
                            {
                                xMax = segmento.P2.X;
                            }
                            if (yMin >= segmento.P2.Y)
                            {
                                yMin = segmento.P2.Y;
                            }
                            if (yMax < segmento.P2.Y)
                            {
                                yMax = segmento.P2.Y;
                            }
                            #endregion

                            lstCoordsGeometry.Add(segmento.P1.X.ToString().Replace(",", ".") + ", " + segmento.P1.Y.ToString().Replace(",", "."));
                            lstCoordsGeometry.Add(segmento.P2.X.ToString().Replace(",", ".") + ", " + segmento.P2.Y.ToString().Replace(",", "."));

                            coordsLado += segmento.P1.X.ToString().Replace(",", ".") + " " + segmento.P1.Y.ToString().Replace(",", ".") + ",";
                            coordsLado += segmento.P2.X.ToString().Replace(",", ".") + " " + segmento.P2.Y.ToString().Replace(",", ".") + ",";
                        }
                        if (coordsLado != string.Empty)
                        {
                            coordsLado = coordsLado.Substring(0, coordsLado.Length - 1);
                        }
                        //armar dbgeometry con los segmentos del lado
                        string wktLado = "LINESTRING (" + coordsLado + ")";
                        DbGeometry geometryLado = DbGeometry.LineFromText(wktLado, 22195);
                        //buscar en geometryCuadra.buffer. si el dbgeometry cae adentro
                        bool contieneLado = false;
                        try
                        {
                            contieneLado = geometryCuadra.Buffer(distBuffer).Contains(geometryLado);
                        }
                        catch { }
                        //x = xMin + (xMax - xMin) / 2;
                        //y = yMin + (yMax - yMin) / 2;
                        //double nearX = 0;
                        //double nearY = 0;
                        //bool isLock = false;
                        //double dist = DistToSegment(x, y, x1Cuadra, y1Cuadra, x2Cuadra, y2Cuadra, out nearX, out nearY, out isLock);
                        //if (isLock)
                        if (contieneLado)
                        {
                            lstLadoCuadra.Add(lado);
                        }
                    }
                    if (lstLadoCuadra.Count > 0)
                    {
                        Lado ladoMayor = lstLadoCuadra.OrderByDescending(l => l.Distancia).FirstOrDefault();

                        xMin = 9999999;
                        yMin = 9999999;
                        xMax = 0;
                        yMax = 0;
                        foreach (var segmento in ladoMayor.Segmentos)
                        {
                            #region Determinar xMin yMin xMax yMax
                            if (xMin >= segmento.P1.X)
                            {
                                xMin = segmento.P1.X;
                            }
                            if (xMax < segmento.P1.X)
                            {
                                xMax = segmento.P1.X;
                            }
                            if (yMin >= segmento.P1.Y)
                            {
                                yMin = segmento.P1.Y;
                            }
                            if (yMax < segmento.P1.Y)
                            {
                                yMax = segmento.P1.Y;
                            }
                            if (xMin >= segmento.P2.X)
                            {
                                xMin = segmento.P2.X;
                            }
                            if (xMax < segmento.P2.X)
                            {
                                xMax = segmento.P2.X;
                            }
                            if (yMin >= segmento.P2.Y)
                            {
                                yMin = segmento.P2.Y;
                            }
                            if (yMax < segmento.P2.Y)
                            {
                                yMax = segmento.P2.Y;
                            }
                            #endregion
                        }

                        nroPuerta = parcelaPlot.NroPuerta;

                        rotation = ladoMayor.Angulo;

                        beta = rotation;
                        if (rotation < 0)
                        {
                            beta = rotation + 360;
                        }

                        double alfa = beta;
                        double textRotation = beta;

                        if (alfa >= 0 && alfa <= 90)
                        {
                            beta = 90 - alfa;
                            textRotation = alfa + 90 + 180;
                        }
                        else if (alfa > 90 && alfa <= 180)
                        {
                            beta = alfa - 90;
                            textRotation = beta;
                        }
                        else if (alfa > 180 && alfa <= 270)
                        {
                            beta = 270 - alfa;
                            textRotation = alfa - 90;
                        }
                        else if (alfa > 270 && alfa <= 360)
                        {
                            beta = alfa - 270;
                            textRotation = beta;
                        }

                        double betaRad = beta * Math.PI / 180.0;

                        x = xMin + (xMax - xMin) / 2;
                        y = yMin + (yMax - yMin) / 2;

                        double desplazamientoX = desplazamientoNroPuerta * Math.Cos(betaRad);
                        double desplazamientoY = desplazamientoNroPuerta * Math.Sin(betaRad);

                        double x1Des = x + desplazamientoNroPuerta * Math.Cos(betaRad);
                        double y1Des = y + desplazamientoNroPuerta * Math.Sin(betaRad);
                        double x2Des = x - desplazamientoNroPuerta * Math.Cos(betaRad);
                        double y2Des = y - desplazamientoNroPuerta * Math.Sin(betaRad);

                        x1Des = x + desplazamientoX;
                        x2Des = x - desplazamientoX;
                        if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
                        {
                            y1Des = y - desplazamientoY;
                            y2Des = y + desplazamientoY;
                        }
                        else if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
                        {
                            y1Des = y + desplazamientoY;
                            y2Des = y - desplazamientoY;
                        }

                        //TODO Dibujo ptos para testing
                        Color colorTexto2 = System.Drawing.ColorTranslator.FromHtml("#006400");
                        it.BaseColor pdfColor2 = new it.BaseColor(colorTexto2.R, colorTexto2.G, colorTexto2.B);
                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x1Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y1Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x2Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y2Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColor2, (float)0.1, pdfColor2, 50);
                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                        //PDFUtilities.DrawPDFText(pdfContentByte, (Math.Truncate(100 * beta) / 100).ToString(), GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), pdfbaseFont, pdfFontSize, colorTexto, lado.Angulo);
                        ////

                        string wktPointDes = "POINT(" + x1Des + " " + y1Des + ")";
                        //AYSA20180425 se agrego replace
                        wktPointDes = wktPointDes.Replace(',', '.');
                        DbGeometry geomPtoDes = DbGeometry.PointFromText(wktPointDes, 22195);
                        bool contiene = parcelaPlot.Geom.Contains(geomPtoDes);

                        //ParcelaPlot[] aParcelaPlotDes = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, x1Des, y1Des);
                        //if (aParcelaPlotDes != null && aParcelaPlotDes.Length > 0 && parcelaPlot.FeatId == aParcelaPlotDes[0].FeatId)
                        if (contiene)
                        {
                            x = x2Des;
                            y = y2Des;
                        }
                        else
                        {
                            x = x1Des;
                            y = y1Des;
                        }
                        //--If GDibujoTexto(pThisDrawing, "MP_TextosParc", wTexto, wX, wY, gColorLabelCotas, wHeight, wAlign, wRotation, gStyleTexto) Then
                        xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
                        yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
                        //DibujarTextos(pdfContentByte, sDistancia, xPdf, yPdf, rotation);
                        float sizeText = pdfbaseFont.GetWidthPoint(nroPuerta, pdfFontSize);
                        //xPdf = xPdf - sizeText / 2;
                        if (plantilla.OptimizarTamanioHoja)
                        {
                            //textRotation = textRotation - anguloRotacion;
                            //if (textRotation < 0)
                            //{
                            //    textRotation = textRotation + 360;
                            //}
                            PointF ptRotado = Rotate(xPdf, yPdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                            double anguloPdf = textRotation - (anguloRotacionFiltro * 180 / Math.PI);
                            PDFUtilities.DrawPDFText(pdfContentByte, nroPuerta, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTexto, anguloPdf);
                        }
                        else
                        {
                            PDFUtilities.DrawPDFText(pdfContentByte, nroPuerta, xPdf, yPdf, pdfbaseFont, pdfFontSize, colorTexto, textRotation);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        #region comento porque no se usa, se borra en un futuro proximo
        //private bool ResaltarDireccion(PdfContentByte pdfContentByte, Plantilla plantilla, string idObjetoBase, int idComponenteObjetoBase, double xCentroidBase, double yCentroidBase, long X, long Y)
        //{
        //    Layer layerBase = plantilla.Layers.FirstOrDefault(p => p.Categoria == 1 && p.ComponenteId == idComponenteObjetoBase && p.FechaBaja == null);
        //    foreach (var plantillaTexto in plantilla.PlantillaTextos)
        //    {
        //        Color colorTexto = System.Drawing.ColorTranslator.FromHtml(plantillaTexto.FuenteColor);
        //        int alignment = plantillaTexto.FuenteAlineacion + it.Element.ALIGN_MIDDLE;

        //        //BaseFont pdfbaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //        float pdfFontSize = it.Utilities.MillimetersToPoints((float)plantillaTexto.FuenteTamanio);
        //        it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);

        //        PDFUtilities.RegisterBaseFont(plantillaTexto.FuenteNombre, pdfFontSize);
        //        string[] aFontStylePdf = plantillaTexto.FuenteEstilo.Split(',');
        //        //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
        //        int pdfFontStyle = aFontStylePdf.Select(x => Convert.ToInt32(x)).Sum();
        //        //BaseFont pdfbaseFont = it.FontFactory.GetFont(plantillaTexto.FuenteNombre, pdfFontSize, it.Font.NORMAL, pdfColorTexto).BaseFont;
        //        BaseFont pdfbaseFont = it.FontFactory.GetFont(plantillaTexto.FuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
        //        it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

        //        string texto = "";

        //        texto = GetDireccionByIdObjGraf(idObjetoBase);

        //        if (!string.IsNullOrEmpty(texto))
        //        {
        //            float sizeText = pdfbaseFont.GetWidthPoint(texto, pdfFontSize);
        //            float xPDF = (float)it.Utilities.MillimetersToPoints((float)plantillaTexto.X);
        //            if (plantillaTexto.FuenteAlineacion == 2)
        //            {
        //                xPDF = xPDF - sizeText / 2;
        //            }
        //            else if (plantillaTexto.FuenteAlineacion == 3)
        //            {
        //                xPDF = xPDF - sizeText;
        //            }
        //            float yPDF = it.Utilities.MillimetersToPoints((float)(plantillaTexto.Y));
        //            PDFUtilities.DrawPDFText(pdfContentByte, texto, xPDF, yPDF, pdfFont, pdfFontSize, alignment);
        //        }
        //    }
        //    return true;
        //} 
        #endregion

        public bool FuncionEspecialDibujarNombreCalle(PdfContentByte pdfContentByte, Plantilla plantilla, List<string> lstCoordenadas, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff, double xCentroidBase, double yCentroidBase, double factorEscala, string idManzana, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, ref ManzanaPlot manzanaPlot)
        {
            bool ret = true;
            int idFuncionAdicional = 4;
            //Parametros
            //Manzana
            string esquema = "gis_aysa";
            string tablaManzana = "CT_MANZANA";
            string campoIdManzana = "ID_MANZANA";
            string campoGeometry = "GEOMETRY";
            //Cuadra
            string esquemaCuadra = "gis_aysa";
            string tablaCuadra = "CT_CUADRA";
            string campoGeometryCuadra = "GEOMETRY";
            string campoIdCuadraCuadra = "ID_CUADRA";
            string campoIdManzanaCuadra = "ID_MANZANA";
            string campoIdCalleCuadra = "ID_CALLE";
            string campoAlturaMin = "ALTURA_MIN";
            string campoAlturaMax = "ALTURA_MAX";
            //Calle
            string esquemaCalle = "gis_aysa";
            string tablaCalle = "CT_CALLE";
            string campoIdCalleCalle = "ID_CALLE";
            string campoNombreCalle = "NOMBRE";
            string campoCodigoCalle = "APIC_ID";
            //Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
            int anguloTolerancia = 2;
            double distanciaTolerancia = 0.1;
            double distBuffer = 8;
            double desplazamientoCalle = 5;
            double distanciaLadoOchava = 10;
            string textoFuenteNombre = "Arial";
            double textoFuenteTamanio = 2;
            string textoFuenteEstilo = "0,0,0,0";
            string textoColor = "#696969";

            #region NumberFormat
            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.CurrencyDecimalDigits = 4;
            numberFormat.CurrencyDecimalSeparator = ".";
            numberFormat.NumberDecimalDigits = 4;
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.PercentDecimalDigits = 2;
            numberFormat.PercentDecimalSeparator = ".";
            #endregion

            #region Parametros de la DB
            #region Manzana
            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquema = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_TABLA");
            if (funcAdicParametro != null)
            {
                tablaManzana = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_ID_MANZANA");
            if (funcAdicParametro != null)
            {
                campoIdManzana = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_GEOMETRY");
            if (funcAdicParametro != null)
            {
                campoGeometry = funcAdicParametro.Valor;
            }
            #endregion
            #region Cuadra
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquemaCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_TABLA");
            if (funcAdicParametro != null)
            {
                tablaCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_GEOMETRY");
            if (funcAdicParametro != null)
            {
                campoGeometryCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_CUADRA");
            if (funcAdicParametro != null)
            {
                campoIdCuadraCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_MANZANA");
            if (funcAdicParametro != null)
            {
                campoIdManzanaCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ID_CALLE");
            if (funcAdicParametro != null)
            {
                campoIdCalleCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ALTURA_MIN");
            if (funcAdicParametro != null)
            {
                campoAlturaMin = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CUADRA_CAMPO_ALTURA_MAX");
            if (funcAdicParametro != null)
            {
                campoAlturaMax = funcAdicParametro.Valor;
            }
            #endregion
            #region Calle
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquemaCalle = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_TABLA");
            if (funcAdicParametro != null)
            {
                tablaCalle = funcAdicParametro.Valor;
            }

            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_CAMPO_NOMBRE");
            if (funcAdicParametro != null)
            {
                campoNombreCalle = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "CALLE_CAMPO_CODIGO");
            if (funcAdicParametro != null)
            {
                campoCodigoCalle = funcAdicParametro.Valor;
            }
            #endregion
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ANGULO_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaTolerancia);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_BUFFER");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distBuffer);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DESPLAZAMIENTO_CALLE");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out desplazamientoCalle);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_LADO_OCHAVA");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaLadoOchava);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
            if (funcAdicParametro != null)
            {
                textoFuenteNombre = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
            if (funcAdicParametro != null)
            {
                textoColor = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
            if (funcAdicParametro != null)
            {
                textoFuenteEstilo = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
            }
            #endregion

            Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);

            //if (lstCoordenadas != null && lstCoordenadas.Count > 0)
            //{
            //    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, lstCoordenadas);
            //}
            //else
            //{
            //    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, filtroGeografico, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
            //}
            long idManzanaPlot = 0;
            bool idOk = long.TryParse(idManzana, out idManzanaPlot);

            if (manzanaPlot == null)
            {
                ManzanaPlot[] aManzanaPlot = _manzanaPlotRepository.GetManzanaPlotByIdManzana(esquema, tablaManzana, campoGeometry, campoIdManzana, idManzanaPlot);
                if (aManzanaPlot != null && aManzanaPlot.Count() > 0)
                {
                    manzanaPlot = aManzanaPlot[0];
                }
            }
            if (manzanaPlot != null)
            {
                DbGeometry geometryManzanaPlot = (DbGeometry)manzanaPlot.GetType().GetProperty("Geom").GetValue(manzanaPlot);

                //List<double> lstLimite = GetLimite(geometryParcelaPlot, toleranciaCambioVertice);

                PointF puntoMedio = new PointF();
                List<Lado> lados = new List<Lado>();
                Lado ladoMayor = new Lado();
                double anguloRotacionLadoMayor = GetAnguloRotacion(geometryManzanaPlot, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);
                //Paso a radianes
                anguloRotacionLadoMayor = anguloRotacionLadoMayor * Math.PI / 180;

                #region Para Testing - Dibujo lados de la geometryBase
                //float pdfx1c = 0, pdfy1c = 0, pdfx2c = 0, pdfy2c = 0;
                //Random rand = new Random();
                //foreach (var lado in lados)
                //{
                //    Color rndColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                //    foreach (var segmento in lado.Segmentos)
                //    {
                //        pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                //        pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                //        pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                //        pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                //        it.BaseColor pdfContornoColorRnd = new it.BaseColor(rndColor.R, rndColor.G, rndColor.B);
                //        PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, it.Utilities.MillimetersToPoints((float)1.27), pdfContornoColorRnd);
                //    }
                //}
                //foreach (var segmento in ladoMayor.Segmentos)
                //{
                //    pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                //    pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                //    pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                //    pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                //    PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, it.Utilities.MillimetersToPoints((float)1.27), it.BaseColor.BLACK);
                //}
                #endregion
                if ((1 / factorEscala) >= 5000)
                {
                    textoFuenteTamanio = textoFuenteTamanio / 2.0;
                }
                DibujarNombreCalle(pdfContentByte, plantilla, xCentroidBase, yCentroidBase, factorEscala, manzanaPlot, lados, distanciaLadoOchava, desplazamientoCalle, esquemaCalle, tablaCalle, campoGeometry, campoIdManzana, esquemaCuadra, tablaCuadra, campoGeometryCuadra, campoIdCuadraCuadra, campoIdCalleCuadra, campoAlturaMin, campoAlturaMax, esquemaCalle, tablaCalle, campoIdCalleCalle, campoNombreCalle, campoCodigoCalle, distBuffer, textoFuenteNombre, textoFuenteTamanio, textoFuenteEstilo, colorTexto, anguloRotacion, sentidoHorario, anguloRotacionFiltro, anguloRotacionLadoMayor, tablaManzana);

            }
            return ret;
        }

        private void DibujarNombreCalle(PdfContentByte pdfContentByte, Plantilla plantilla, double xCentroidBase, double yCentroidBase, double factorEscala, ManzanaPlot manzanaPlot, List<Lado> lstLado, double distanciaLadoOchava, double desplazamientoCalle, string esquema, string tabla, string campoGeometry, string campoIdManzana, string esquemaCuadra, string tablaCuadra, string campoGeometryCuadra, string campoIdCuadraCuadra, string campoIdCalleCuadra, string campoAlturaMin, string campoAlturaMax, string esquemaCalle, string tablaCalle, string campoIdCalleCalle, string campoNombreCalle, string campoCodigoCalle, double distBuffer, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color colorTexto, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, double anguloRotacionLadoMayor, string tablaManzana)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                if ((1 / factorEscala) >= 5000)
                {
                    textoFuenteTamanio = textoFuenteTamanio * 2.0;
                }
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;

                double x = 0, y = 0;
                double xMin = 9999999, yMin = 9999999;
                double xMax = 0, yMax = 0;
                double beta = 0;
                double rotation = 0;
                string sDistancia = string.Empty;
                float xPdf = 0;
                float yPdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                string textoCalle = string.Empty;

                CuadraPlot[] aCuadraPlot = _cuadraPlotRepository.GetCuadraPlotByIdManzana(esquemaCuadra, tablaCuadra, campoGeometryCuadra, campoIdCuadraCuadra, campoIdManzana, campoIdCalleCuadra, campoAlturaMin, campoAlturaMax, manzanaPlot.FeatId);
                if (aCuadraPlot != null && aCuadraPlot.Length > 0)
                {
                    CuadraPlot cuadraAnt = new CuadraPlot();
                    cuadraAnt.IdCuadra = 0;
                    List<Lado> lstLadoCuadra = new List<Lado>();
                    CuadraPlot cuadraLado = null;
                    CallePlot[] aCallePlot = null;
                    #region Codigo comentado para test - dibujo los lados en distintos colores
                    //Random rand = new Random();
                    ////Color contornoColor = Color.Black;
                    ////it.BaseColor pdfContornoColor = new it.BaseColor(contornoColor.R, contornoColor.G, contornoColor.B);
                    //float pdfContornoGrosor = 2f;
                    //foreach (var lado in lstLado)
                    //{
                    //    Color rndColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                    //    foreach (var segmento in lado.Segmentos)
                    //    {
                    //        float pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                    //        float pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                    //        float pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                    //        float pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                    //        it.BaseColor pdfContornoColorRnd = new it.BaseColor(rndColor.R, rndColor.G, rndColor.B);
                    //        PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, pdfContornoGrosor, pdfContornoColorRnd);
                    //    }
                    //}
                    #endregion
                    Lado ladoMayor = new Lado();
                    foreach (var lado in lstLado)
                    {
                        if (lado.Distancia > distanciaLadoOchava)
                        {
                            xMin = 9999999;
                            yMin = 9999999;
                            xMax = 0;
                            yMax = 0;
                            List<string> lstCoordsGeometry = new List<string>();
                            List<string> lstCoordsGeometryLado = new List<string>();
                            string coordsLado = string.Empty;
                            //string coordsToMap = string.Empty;
                            foreach (var segmento in lado.Segmentos)
                            {
                                //pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                                #region Determinar xMin yMin xMax yMax
                                if (xMin >= segmento.P1.X)
                                {
                                    xMin = segmento.P1.X;
                                }
                                if (xMax < segmento.P1.X)
                                {
                                    xMax = segmento.P1.X;
                                }
                                if (yMin >= segmento.P1.Y)
                                {
                                    yMin = segmento.P1.Y;
                                }
                                if (yMax < segmento.P1.Y)
                                {
                                    yMax = segmento.P1.Y;
                                }
                                if (xMin >= segmento.P2.X)
                                {
                                    xMin = segmento.P2.X;
                                }
                                if (xMax < segmento.P2.X)
                                {
                                    xMax = segmento.P2.X;
                                }
                                if (yMin >= segmento.P2.Y)
                                {
                                    yMin = segmento.P2.Y;
                                }
                                if (yMax < segmento.P2.Y)
                                {
                                    yMax = segmento.P2.Y;
                                }
                                #endregion

                                coordsLado += segmento.P1.X.ToString().Replace(",", ".") + " " + segmento.P1.Y.ToString().Replace(",", ".") + ",";
                                coordsLado += segmento.P2.X.ToString().Replace(",", ".") + " " + segmento.P2.Y.ToString().Replace(",", ".") + ",";
                            }
                            if (coordsLado != string.Empty)
                            {
                                coordsLado = coordsLado.Substring(0, coordsLado.Length - 1);
                            }
                            //armar DBGeometry con los segmentos del lado
                            string wktLado = "LINESTRING (" + coordsLado + ")";
                            DbGeometry geometryLado = DbGeometry.LineFromText(wktLado, 22195);

                            cuadraLado = null;
                            foreach (CuadraPlot cuadra in aCuadraPlot)
                            {
                                DbGeometry geometryCuadra = (DbGeometry)cuadra.GetType().GetProperty("Geom").GetValue(cuadra);

                                bool contieneLado = geometryCuadra.Buffer(distBuffer).Contains(geometryLado);
                                if (contieneLado)
                                {
                                    cuadraLado = cuadra;
                                    break;
                                }
                            }
                            if (cuadraLado != null)
                            {
                                lado.IdCuadra = cuadraLado.IdCuadra;
                                lstLadoCuadra.Add(lado);
                            }
                        }
                    }
                    if (lstLadoCuadra.Count > 0)
                    {
                        foreach (var cuadra in aCuadraPlot)
                        {
                            if (lstLadoCuadra.Exists(c => c.IdCuadra == cuadra.IdCuadra))
                            {
                                List<Lado> lstLadoCuadraFiltro = lstLadoCuadra.Where(p => p.IdCuadra == cuadra.IdCuadra).ToList();
                                if (lstLadoCuadraFiltro.Count > 0)
                                {
                                    ladoMayor = lstLadoCuadraFiltro.OrderByDescending(l => l.Distancia).FirstOrDefault();
                                    xMin = 9999999;
                                    yMin = 9999999;
                                    xMax = 0;
                                    yMax = 0;
                                    foreach (var segmento in ladoMayor.Segmentos)
                                    {
                                        #region Determinar xMin yMin xMax yMax
                                        if (xMin >= segmento.P1.X)
                                        {
                                            xMin = segmento.P1.X;
                                        }
                                        if (xMax < segmento.P1.X)
                                        {
                                            xMax = segmento.P1.X;
                                        }
                                        if (yMin >= segmento.P1.Y)
                                        {
                                            yMin = segmento.P1.Y;
                                        }
                                        if (yMax < segmento.P1.Y)
                                        {
                                            yMax = segmento.P1.Y;
                                        }
                                        if (xMin >= segmento.P2.X)
                                        {
                                            xMin = segmento.P2.X;
                                        }
                                        if (xMax < segmento.P2.X)
                                        {
                                            xMax = segmento.P2.X;
                                        }
                                        if (yMin >= segmento.P2.Y)
                                        {
                                            yMin = segmento.P2.Y;
                                        }
                                        if (yMax < segmento.P2.Y)
                                        {
                                            yMax = segmento.P2.Y;
                                        }
                                        #endregion
                                        #region Codigo comentado - Dibujo lado mayor de cuadra para Testing
                                        //if (ladoMayor.IdCuadra == 186487)
                                        //{
                                        //    float pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                                        //    float pdfy1c = GetYPDFCanvas(segmento.P1.Y, yCentroidBase, factorEscala, plantilla);
                                        //    float pdfx2c = GetXPDFCanvas(segmento.P2.X, xCentroidBase, factorEscala, plantilla);
                                        //    float pdfy2c = GetYPDFCanvas(segmento.P2.Y, yCentroidBase, factorEscala, plantilla);
                                        //    it.BaseColor pdfContornoColorRnd = new it.BaseColor(Color.Black.R, Color.Black.G, Color.Black.B);
                                        //    PDFUtilities.DrawPDFLine(pdfContentByte, pdfx1c, pdfy1c, pdfx2c, pdfy2c, pdfContornoGrosor, pdfContornoColorRnd);
                                        //}
                                        #endregion
                                    }

                                    aCallePlot = _callePlotRepository.GetCallePlotByIdCalle(esquemaCalle, tablaCalle, campoIdCalleCalle, campoNombreCalle, campoCodigoCalle, cuadra.IdCalle);
                                    if (aCallePlot != null && aCallePlot.Length > 0)
                                    {
                                        CallePlot callePlot = aCallePlot[0];

                                        string alturas = " [";
                                        if (cuadra.AlturaMin == cuadra.AlturaMax)
                                        {
                                            alturas = alturas + cuadra.AlturaMin.ToString() + "]";
                                        }
                                        else
                                        {
                                            alturas = alturas + cuadra.AlturaMin.ToString() + "-" + cuadra.AlturaMax.ToString() + "]";
                                        }
                                        textoCalle = "(" + callePlot.Codigo + ") " + callePlot.Nombre + alturas;

                                        #region Ubico el texto
                                        rotation = ladoMayor.Angulo;

                                        beta = rotation;
                                        if (rotation < 0)
                                        {
                                            beta = rotation + 360;
                                        }

                                        double alfa = beta;

                                        double textRotation = alfa;

                                        if (alfa >= 0 && alfa <= 90)
                                        {
                                            beta = 90 - alfa;
                                            //textRotation = alfa + 90 + 180;
                                        }
                                        else if (alfa > 90 && alfa <= 180)
                                        {
                                            beta = alfa - 90;
                                            //textRotation = beta;
                                        }
                                        else if (alfa > 180 && alfa <= 270)
                                        {
                                            beta = 270 - alfa;
                                            //textRotation = alfa - 90;
                                        }
                                        else if (alfa > 270 && alfa <= 360)
                                        {
                                            beta = alfa - 270;
                                            //textRotation = beta;
                                        }

                                        double betaRad = beta * Math.PI / 180.0;

                                        x = xMin + (xMax - xMin) / 2;
                                        y = yMin + (yMax - yMin) / 2;

                                        double desplazamientoX = desplazamientoCalle * Math.Cos(betaRad);
                                        double desplazamientoY = desplazamientoCalle * Math.Sin(betaRad);

                                        double x1Des = x + desplazamientoCalle * Math.Cos(betaRad);
                                        double y1Des = y + desplazamientoCalle * Math.Sin(betaRad);
                                        double x2Des = x - desplazamientoCalle * Math.Cos(betaRad);
                                        double y2Des = y - desplazamientoCalle * Math.Sin(betaRad);

                                        x1Des = x + desplazamientoX;
                                        x2Des = x - desplazamientoX;
                                        if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
                                        {
                                            y1Des = y - desplazamientoY;
                                            y2Des = y + desplazamientoY;
                                        }
                                        else if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
                                        {
                                            y1Des = y + desplazamientoY;
                                            y2Des = y - desplazamientoY;
                                        }

                                        //TODO Test - Dibujo ptos para testing
                                        Color colorTexto2 = System.Drawing.ColorTranslator.FromHtml("#006400");
                                        it.BaseColor pdfColor2 = new it.BaseColor(colorTexto2.R, colorTexto2.G, colorTexto2.B);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x1Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y1Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x2Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y2Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColor2, (float)0.1, pdfColor2, 50);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                        //PDFUtilities.DrawPDFText(pdfContentByte, (Math.Truncate(100 * beta) / 100).ToString(), GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), pdfbaseFont, pdfFontSize, colorTexto, lado.Angulo);
                                        ////

                                        ManzanaPlot[] aManzanaPlotDes = _manzanaPlotRepository.GetManzanaPlotByCoords(esquema, tablaManzana, campoGeometry, campoIdManzana, x1Des, y1Des);
                                        if (aManzanaPlotDes != null && aManzanaPlotDes.Length > 0 && manzanaPlot.FeatId == aManzanaPlotDes[0].FeatId)
                                        {
                                            x = x2Des;
                                            y = y2Des;
                                        }
                                        else
                                        {
                                            x = x1Des;
                                            y = y1Des;
                                        }
                                        xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
                                        yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado = Rotate(xPdf, yPdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            double anguloPdf = rotation - (anguloRotacionFiltro * 180 / Math.PI);
                                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTexto, anguloPdf);
                                        }
                                        else
                                        {
                                            //PDFUtilities.DrawPDFCircle(pdfContentByte, xPdf, yPdf, it.Utilities.MillimetersToPoints((float)0.1), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, xPdf, yPdf, pdfbaseFont, pdfFontSize, colorTexto, rotation);
                                        }
                                        #endregion
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void DibujarNombreCalleAnt(PdfContentByte pdfContentByte, Plantilla plantilla, double xCentroidBase, double yCentroidBase, double factorEscala, ManzanaPlot manzanaPlot, List<Lado> lstLado, double distanciaLadoOchava, double desplazamientoCalle, string esquema, string tabla, string campoGeometry, string campoIdManzana, string esquemaCuadra, string tablaCuadra, string campoGeometryCuadra, string campoIdCuadraCuadra, string campoIdCalleCuadra, string campoAlturaMin, string campoAlturaMax, string esquemaCalle, string tablaCalle, string campoIdCalleCalle, string campoNombreCalle, string campoCodigoCalle, double distBuffer, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color colorTexto, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, double anguloRotacionLadoMayor)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;

                double x = 0, y = 0;
                double xMin = 9999999, yMin = 9999999;
                double xMax = 0, yMax = 0;
                double beta = 0;
                double rotation = 0;
                string sDistancia = string.Empty;
                float xPdf = 0;
                float yPdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                string textoCalle = string.Empty;

                CuadraPlot[] aCuadraPlot = _cuadraPlotRepository.GetCuadraPlotByIdManzana(esquemaCuadra, tablaCuadra, campoGeometryCuadra, campoIdCuadraCuadra, campoIdManzana, campoIdCalleCuadra, campoAlturaMin, campoAlturaMax, manzanaPlot.FeatId);
                if (aCuadraPlot != null && aCuadraPlot.Length > 0)
                {
                    CuadraPlot cuadraAnt = new CuadraPlot();
                    cuadraAnt.IdCuadra = 0;
                    List<Lado> lstLadoCuadra = new List<Lado>();
                    CuadraPlot cuadraLado = null;
                    CallePlot[] aCallePlot = null;
                    Lado ladoMayor = new Lado();
                    Lado ladoMayorAnt = new Lado();
                    foreach (var lado in lstLado)
                    {
                        if (lado.Distancia > distanciaLadoOchava)
                        {
                            xMin = 9999999;
                            yMin = 9999999;
                            xMax = 0;
                            yMax = 0;
                            List<string> lstCoordsGeometry = new List<string>();
                            List<string> lstCoordsGeometryLado = new List<string>();
                            string coordsLado = string.Empty;
                            string coordsToMap = string.Empty;
                            foreach (var segmento in lado.Segmentos)
                            {
                                //pdfx1c = GetXPDFCanvas(segmento.P1.X, xCentroidBase, factorEscala, plantilla);
                                #region Determinar xMin yMin xMax yMax
                                if (xMin >= segmento.P1.X)
                                {
                                    xMin = segmento.P1.X;
                                }
                                if (xMax < segmento.P1.X)
                                {
                                    xMax = segmento.P1.X;
                                }
                                if (yMin >= segmento.P1.Y)
                                {
                                    yMin = segmento.P1.Y;
                                }
                                if (yMax < segmento.P1.Y)
                                {
                                    yMax = segmento.P1.Y;
                                }
                                if (xMin >= segmento.P2.X)
                                {
                                    xMin = segmento.P2.X;
                                }
                                if (xMax < segmento.P2.X)
                                {
                                    xMax = segmento.P2.X;
                                }
                                if (yMin >= segmento.P2.Y)
                                {
                                    yMin = segmento.P2.Y;
                                }
                                if (yMax < segmento.P2.Y)
                                {
                                    yMax = segmento.P2.Y;
                                }
                                #endregion

                                lstCoordsGeometry.Add(segmento.P1.X.ToString().Replace(",", ".") + ", " + segmento.P1.Y.ToString().Replace(",", "."));
                                lstCoordsGeometry.Add(segmento.P2.X.ToString().Replace(",", ".") + ", " + segmento.P2.Y.ToString().Replace(",", "."));

                                coordsLado += segmento.P1.X.ToString().Replace(",", ".") + " " + segmento.P1.Y.ToString().Replace(",", ".") + ",";
                                coordsLado += segmento.P2.X.ToString().Replace(",", ".") + " " + segmento.P2.Y.ToString().Replace(",", ".") + ",";

                                coordsToMap += segmento.P1.X.ToString().Replace(",", ".") + "," + segmento.P1.Y.ToString().Replace(",", ".") + ",";
                                coordsToMap += segmento.P2.X.ToString().Replace(",", ".") + "," + segmento.P2.Y.ToString().Replace(",", ".") + ",";

                            }
                            if (coordsLado != string.Empty)
                            {
                                coordsLado = coordsLado.Substring(0, coordsLado.Length - 1);
                            }
                            //armar dbgeometry con los segmentos del lado
                            string wktLado = "LINESTRING (" + coordsLado + ")";
                            DbGeometry geometryLado = DbGeometry.LineFromText(wktLado, 22195);

                            foreach (CuadraPlot cuadra in aCuadraPlot)
                            {
                                DbGeometry geometryCuadra = (DbGeometry)cuadra.GetType().GetProperty("Geom").GetValue(cuadra);

                                bool contieneLado = geometryCuadra.Buffer(distBuffer).Contains(geometryLado);
                                if (contieneLado)
                                {
                                    cuadraLado = cuadra;
                                    break;
                                }
                            }
                            if (cuadraLado != null)
                            {
                                if (cuadraLado.IdCuadra != cuadraAnt.IdCuadra && cuadraAnt.IdCuadra > 0)
                                {
                                    if (lstLadoCuadra.Count > 0)
                                    {
                                        ladoMayor = lstLadoCuadra.OrderByDescending(l => l.Distancia).FirstOrDefault();
                                        xMin = 9999999;
                                        yMin = 9999999;
                                        xMax = 0;
                                        yMax = 0;
                                        coordsLado = string.Empty;
                                        foreach (var segmento in ladoMayor.Segmentos)
                                        {
                                            #region Determinar xMin yMin xMax yMax
                                            if (xMin >= segmento.P1.X)
                                            {
                                                xMin = segmento.P1.X;
                                            }
                                            if (xMax < segmento.P1.X)
                                            {
                                                xMax = segmento.P1.X;
                                            }
                                            if (yMin >= segmento.P1.Y)
                                            {
                                                yMin = segmento.P1.Y;
                                            }
                                            if (yMax < segmento.P1.Y)
                                            {
                                                yMax = segmento.P1.Y;
                                            }
                                            if (xMin >= segmento.P2.X)
                                            {
                                                xMin = segmento.P2.X;
                                            }
                                            if (xMax < segmento.P2.X)
                                            {
                                                xMax = segmento.P2.X;
                                            }
                                            if (yMin >= segmento.P2.Y)
                                            {
                                                yMin = segmento.P2.Y;
                                            }
                                            if (yMax < segmento.P2.Y)
                                            {
                                                yMax = segmento.P2.Y;
                                            }
                                            #endregion
                                        }

                                        lstLadoCuadra.Clear();
                                    }
                                    lstLadoCuadra.Add(lado);

                                    aCallePlot = _callePlotRepository.GetCallePlotByIdCalle(esquemaCalle, tablaCalle, campoIdCalleCalle, campoNombreCalle, campoCodigoCalle, cuadraAnt.IdCalle);
                                    if (aCallePlot != null && aCallePlot.Length > 0)
                                    {
                                        CallePlot callePlot = aCallePlot[0];

                                        string alturas = " [";
                                        if (cuadraAnt.AlturaMin == cuadraAnt.AlturaMax)
                                        {
                                            alturas = alturas + cuadraAnt.AlturaMin.ToString() + "]";
                                        }
                                        else
                                        {
                                            alturas = alturas + cuadraAnt.AlturaMin.ToString() + "-" + cuadraAnt.AlturaMax.ToString() + "]";
                                        }
                                        textoCalle = "(" + callePlot.Codigo + ") " + callePlot.Nombre + alturas;

                                        rotation = ladoMayor.Angulo;

                                        beta = rotation;
                                        if (rotation < 0)
                                        {
                                            beta = rotation + 360;
                                        }

                                        double alfa = beta;

                                        double textRotation = alfa;

                                        if (alfa >= 0 && alfa <= 90)
                                        {
                                            beta = 90 - alfa;
                                            //textRotation = alfa + 90 + 180;
                                        }
                                        else if (alfa > 90 && alfa <= 180)
                                        {
                                            beta = alfa - 90;
                                            //textRotation = beta;
                                        }
                                        else if (alfa > 180 && alfa <= 270)
                                        {
                                            beta = 270 - alfa;
                                            //textRotation = alfa - 90;
                                        }
                                        else if (alfa > 270 && alfa <= 360)
                                        {
                                            beta = alfa - 270;
                                            //textRotation = beta;
                                        }

                                        double betaRad = beta * Math.PI / 180.0;

                                        x = xMin + (xMax - xMin) / 2;
                                        y = yMin + (yMax - yMin) / 2;

                                        double desplazamientoX = desplazamientoCalle * Math.Cos(betaRad);
                                        double desplazamientoY = desplazamientoCalle * Math.Sin(betaRad);

                                        double x1Des = x + desplazamientoCalle * Math.Cos(betaRad);
                                        double y1Des = y + desplazamientoCalle * Math.Sin(betaRad);
                                        double x2Des = x - desplazamientoCalle * Math.Cos(betaRad);
                                        double y2Des = y - desplazamientoCalle * Math.Sin(betaRad);

                                        x1Des = x + desplazamientoX;
                                        x2Des = x - desplazamientoX;
                                        if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
                                        {
                                            y1Des = y - desplazamientoY;
                                            y2Des = y + desplazamientoY;
                                        }
                                        else if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
                                        {
                                            y1Des = y + desplazamientoY;
                                            y2Des = y - desplazamientoY;
                                        }

                                        //TODO Test - Dibujo ptos para testing
                                        Color colorTexto2 = System.Drawing.ColorTranslator.FromHtml("#006400");
                                        it.BaseColor pdfColor2 = new it.BaseColor(colorTexto2.R, colorTexto2.G, colorTexto2.B);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x1Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y1Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x2Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y2Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColor2, (float)0.1, pdfColor2, 50);
                                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                        //PDFUtilities.DrawPDFText(pdfContentByte, (Math.Truncate(100 * beta) / 100).ToString(), GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), pdfbaseFont, pdfFontSize, colorTexto, lado.Angulo);
                                        ////

                                        ManzanaPlot[] aManzanaPlotDes = _manzanaPlotRepository.GetManzanaPlotByCoords(esquema, tabla, campoGeometry, campoIdManzana, x1Des, y1Des);
                                        if (aManzanaPlotDes != null && aManzanaPlotDes.Length > 0 && manzanaPlot.FeatId == aManzanaPlotDes[0].FeatId)
                                        {
                                            x = x2Des;
                                            y = y2Des;
                                        }
                                        else
                                        {
                                            x = x1Des;
                                            y = y1Des;
                                        }
                                        xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
                                        yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
                                        if (plantilla.OptimizarTamanioHoja)
                                        {
                                            PointF ptRotado = Rotate(xPdf, yPdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                                            double anguloPdf = rotation - (anguloRotacionFiltro * 180 / Math.PI);
                                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTexto, anguloPdf);
                                        }
                                        else
                                        {
                                            //PDFUtilities.DrawPDFCircle(pdfContentByte, xPdf, yPdf, it.Utilities.MillimetersToPoints((float)0.1), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, xPdf, yPdf, pdfbaseFont, pdfFontSize, colorTexto, rotation);
                                        }
                                    }
                                    cuadraAnt = cuadraLado;
                                }
                                else
                                {
                                    lstLadoCuadra.Add(lado);
                                    cuadraAnt = cuadraLado;
                                }
                            }
                        }
                    }

                    if (lstLadoCuadra.Count > 0)
                    {
                        ladoMayor = lstLadoCuadra.OrderByDescending(l => l.Distancia).FirstOrDefault();
                        xMin = 9999999;
                        yMin = 9999999;
                        xMax = 0;
                        yMax = 0;
                        foreach (var segmento in ladoMayor.Segmentos)
                        {
                            #region Determinar xMin yMin xMax yMax
                            if (xMin >= segmento.P1.X)
                            {
                                xMin = segmento.P1.X;
                            }
                            if (xMax < segmento.P1.X)
                            {
                                xMax = segmento.P1.X;
                            }
                            if (yMin >= segmento.P1.Y)
                            {
                                yMin = segmento.P1.Y;
                            }
                            if (yMax < segmento.P1.Y)
                            {
                                yMax = segmento.P1.Y;
                            }
                            if (xMin >= segmento.P2.X)
                            {
                                xMin = segmento.P2.X;
                            }
                            if (xMax < segmento.P2.X)
                            {
                                xMax = segmento.P2.X;
                            }
                            if (yMin >= segmento.P2.Y)
                            {
                                yMin = segmento.P2.Y;
                            }
                            if (yMax < segmento.P2.Y)
                            {
                                yMax = segmento.P2.Y;
                            }
                            #endregion
                        }

                        lstLadoCuadra.Clear();
                    }
                    aCallePlot = _callePlotRepository.GetCallePlotByIdCalle(esquemaCalle, tablaCalle, campoIdCalleCalle, campoNombreCalle, campoCodigoCalle, cuadraLado.IdCalle);
                    if (aCallePlot != null && aCallePlot.Length > 0)
                    {
                        CallePlot callePlot = aCallePlot[0];

                        string alturas = " [";
                        if (cuadraLado.AlturaMin == cuadraLado.AlturaMax)
                        {
                            alturas = alturas + cuadraLado.AlturaMin.ToString() + "]";
                        }
                        else
                        {
                            alturas = alturas + cuadraLado.AlturaMin.ToString() + "-" + cuadraLado.AlturaMax.ToString() + "]";
                        }
                        textoCalle = "(" + callePlot.Codigo + ") " + callePlot.Nombre + alturas;

                        rotation = ladoMayor.Angulo;

                        beta = rotation;
                        if (rotation < 0)
                        {
                            beta = rotation + 360;
                        }

                        double alfa = beta;

                        double textRotation = alfa;

                        if (alfa >= 0 && alfa <= 90)
                        {
                            beta = 90 - alfa;
                            //textRotation = alfa + 90 + 180;
                        }
                        else if (alfa > 90 && alfa <= 180)
                        {
                            beta = alfa - 90;
                            //textRotation = beta;
                        }
                        else if (alfa > 180 && alfa <= 270)
                        {
                            beta = 270 - alfa;
                            //textRotation = alfa - 90;
                        }
                        else if (alfa > 270 && alfa <= 360)
                        {
                            beta = alfa - 270;
                            //textRotation = beta;
                        }

                        double betaRad = beta * Math.PI / 180.0;

                        x = xMin + (xMax - xMin) / 2;
                        y = yMin + (yMax - yMin) / 2;

                        double desplazamientoX = desplazamientoCalle * Math.Cos(betaRad);
                        double desplazamientoY = desplazamientoCalle * Math.Sin(betaRad);

                        double x1Des = x + desplazamientoCalle * Math.Cos(betaRad);
                        double y1Des = y + desplazamientoCalle * Math.Sin(betaRad);
                        double x2Des = x - desplazamientoCalle * Math.Cos(betaRad);
                        double y2Des = y - desplazamientoCalle * Math.Sin(betaRad);

                        x1Des = x + desplazamientoX;
                        x2Des = x - desplazamientoX;
                        if ((alfa >= 0 && alfa <= 90) || (alfa > 180 && alfa <= 270))
                        {
                            y1Des = y - desplazamientoY;
                            y2Des = y + desplazamientoY;
                        }
                        else if ((alfa > 90 && alfa <= 180) || (alfa > 270 && alfa <= 360))
                        {
                            y1Des = y + desplazamientoY;
                            y2Des = y - desplazamientoY;
                        }

                        //TODO Test - Dibujo ptos para testing
                        Color colorTexto2 = System.Drawing.ColorTranslator.FromHtml("#006400");
                        it.BaseColor pdfColor2 = new it.BaseColor(colorTexto2.R, colorTexto2.G, colorTexto2.B);
                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x1Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y1Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x2Des, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y2Des, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColor2, (float)0.1, pdfColor2, 50);
                        //PDFUtilities.DrawPDFCircle(pdfContentByte, GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), it.Utilities.MillimetersToPoints((float)0.5), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                        //PDFUtilities.DrawPDFText(pdfContentByte, (Math.Truncate(100 * beta) / 100).ToString(), GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla), GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla), pdfbaseFont, pdfFontSize, colorTexto, lado.Angulo);
                        ////

                        ManzanaPlot[] aManzanaPlotDes = _manzanaPlotRepository.GetManzanaPlotByCoords(esquema, tabla, campoGeometry, campoIdManzana, x1Des, y1Des);
                        if (aManzanaPlotDes != null && aManzanaPlotDes.Length > 0 && manzanaPlot.FeatId == aManzanaPlotDes[0].FeatId)
                        {
                            x = x2Des;
                            y = y2Des;
                        }
                        else
                        {
                            x = x1Des;
                            y = y1Des;
                        }
                        xPdf = GetXPDFCanvas(x, xCentroidBase, factorEscala, plantilla);
                        yPdf = GetYPDFCanvas(y, yCentroidBase, factorEscala, plantilla);
                        if (plantilla.OptimizarTamanioHoja)
                        {
                            PointF ptRotado = Rotate(xPdf, yPdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                            double anguloPdf = textRotation - (anguloRotacionFiltro * 180 / Math.PI);
                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTexto, anguloPdf);
                        }
                        else
                        {
                            //PDFUtilities.DrawPDFCircle(pdfContentByte, xPdf, yPdf, it.Utilities.MillimetersToPoints((float)0.1), pdfColorTexto, (float)0.1, pdfColorTexto, 50);
                            PDFUtilities.DrawPDFText(pdfContentByte, textoCalle, xPdf, yPdf, pdfbaseFont, pdfFontSize, colorTexto, rotation);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        public bool FuncionEspecialDibujarDatosParcelaReferencia(PdfContentByte pdfContentByte, Plantilla plantilla, Componente componenteBase, string idObjetoBase, List<string> lstCoordenadas, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff, double xCentroidBase, double yCentroidBase, double factorEscala, string idManzana, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, ref ParcelaPlot[] aParcelaPlot, ref ManzanaPlot manzanaPlot)
        {
            bool ret = true;
            int idFuncionAdicional = 6;
            int filtroGeografico = 1;
            //Parametros
            //Parcela
            string esquema = "gis_aysa_dev";
            string tabla = "CT_PARCELA";
            string campoFeatId = "ID_PARCELA";
            string campoGeometry = "GEOMETRY";
            string campoNroPuerta = "NRO_PUERTA";
            string campoIdCuadra = "ID_CUADRA";
            string campoExpediente = "NUMERO";
            string campoNomCatastral = "NOM_CATASTRAL";
            string campoIdClienteTipo = "ID_CLIENTE_TIPO";
            //Manzana
            string esquemaManzana = "gis_aysa_dev";
            string tablaManzana = "CT_MANZANA";
            string campoIdManzanaManzana = "ID_MANZANA";
            string campoGeometryManzana = "GEOMETRY";
            //Min Max Referencia
            double xMin = 240;
            double yMin = 51;
            double xMax = 280;
            double yMax = 165;
            double espaciadoMM = 0.7;
            int maxParcelas = 100;
            int maxLimite1 = 25;
            int maxLimite2 = 50;
            //Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
            int anguloTolerancia = 2;
            double distanciaTolerancia = 0.1;
            int porcentajeAreaIncluida = 90;

            string textoFuenteNombre = "Arial";
            double textoFuenteTamanio = 2;
            string textoFuenteEstilo = "0,0,0,0";
            string textoColor = "#000000";
            string textoColorBarrioCarenciado = "#0000ff";
            string textoColorGranUsuario = "#008000";
            string textoFuenteNombreGraf = "Arial";
            double textoFuenteTamanioGraf = 3;
            string textoFuenteEstiloGraf = "0,0,0,0";
            string textoColorGraf = "#ff9147";

            #region NumberFormat
            NumberFormatInfo numberFormat = new NumberFormatInfo();
            numberFormat.CurrencyDecimalDigits = 4;
            numberFormat.CurrencyDecimalSeparator = ".";
            numberFormat.NumberDecimalDigits = 4;
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.PercentDecimalDigits = 2;
            numberFormat.PercentDecimalSeparator = ".";
            #endregion

            #region Parametros de la DB
            #region Parcela
            FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquema = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_TABLA");
            if (funcAdicParametro != null)
            {
                tabla = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_ID");
            if (funcAdicParametro != null)
            {
                campoFeatId = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_GEOMETRY");
            if (funcAdicParametro != null)
            {
                campoGeometry = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_NRO_PUERTA");
            if (funcAdicParametro != null)
            {
                campoNroPuerta = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_ID_CUADRA");
            if (funcAdicParametro != null)
            {
                campoIdCuadra = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_EXPEDIENTE");
            if (funcAdicParametro != null)
            {
                campoExpediente = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_NOM_CATASTRAL");
            if (funcAdicParametro != null)
            {
                campoNomCatastral = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_ID_CLIENTE_TIPO");
            if (funcAdicParametro != null)
            {
                campoIdClienteTipo = funcAdicParametro.Valor;
            }
            #endregion
            #region Manzana
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_ESQUEMA");
            if (funcAdicParametro != null)
            {
                esquemaManzana = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_TABLA");
            if (funcAdicParametro != null)
            {
                tablaManzana = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_ID_MANZANA");
            if (funcAdicParametro != null)
            {
                campoIdManzanaManzana = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_GEOMETRY");
            if (funcAdicParametro != null)
            {
                campoGeometryManzana = funcAdicParametro.Valor;
            }
            #endregion
            #region xMin yMin etc
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "XMIN");
            if (funcAdicParametro != null)
            {
                //xMin = Convert.ToDouble(funcAdicParametro.Valor.ToString(CultureInfo.InvariantCulture));
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMin);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "YMIN");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMin);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "XMAX");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMax);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "YMAX");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMax);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out espaciadoMM);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MAX_PARCELAS");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out maxParcelas);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MAX_LIMITE1");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out maxLimite1);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MAX_LIMITE2");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out maxLimite2);
            }
            #endregion
            #region Parametros Tolerancia
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ANGULO_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_TOLERANCIA");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaTolerancia);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PORCENTAJE_AREA_INCLUIDA");
            if (funcAdicParametro != null)
            {
                int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out porcentajeAreaIncluida);
            }
            #endregion
            #region Font
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
            if (funcAdicParametro != null)
            {
                textoFuenteNombre = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
            if (funcAdicParametro != null)
            {
                textoColor = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
            if (funcAdicParametro != null)
            {
                textoFuenteEstilo = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR_BARRIO_CARENCIADO");
            if (funcAdicParametro != null)
            {
                textoColorBarrioCarenciado = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR_GRAN_USUARIO");
            if (funcAdicParametro != null)
            {
                textoColorGranUsuario = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE_GRAF");
            if (funcAdicParametro != null)
            {
                textoFuenteNombreGraf = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR_GRAF");
            if (funcAdicParametro != null)
            {
                textoColorGraf = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO_GRAF");
            if (funcAdicParametro != null)
            {
                textoFuenteEstiloGraf = funcAdicParametro.Valor;
            }
            funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO_GRAF");
            if (funcAdicParametro != null)
            {
                double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanioGraf);
            }

            #endregion
            #endregion

            Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);
            Color colorTextoGraf = System.Drawing.ColorTranslator.FromHtml(textoColorGraf);
            Color colorTextoBarrioCarenciado = System.Drawing.ColorTranslator.FromHtml(textoColorBarrioCarenciado);
            Color colorTextoGranUsuario = System.Drawing.ColorTranslator.FromHtml(textoColorGranUsuario);

            //ParcelaPlot[] aParcelaPlot = null;
            if (aParcelaPlot == null || aParcelaPlot.Length == 0)
            {
                if (lstCoordenadas != null && lstCoordenadas.Count > 0)
                {
                    //aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, lstCoordenadas);
                    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByObjetoBase(componenteBase, esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, idObjetoBase);
                }
                else
                {
                    aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
                }
            }
            if (aParcelaPlot != null && aParcelaPlot.Count() > 0)
            {
                bool existeEspacioVerde = false;
                //if (manzanaPlot == null)
                //{
                long idManzanaPlot = 0;
                bool idOk = long.TryParse(idManzana, out idManzanaPlot);
                ManzanaPlot[] aManzanaPlot = _manzanaPlotRepository.GetManzanaPlotByIdManzana(esquemaManzana, tablaManzana, campoGeometryManzana, campoIdManzanaManzana, idManzanaPlot);
                if (aManzanaPlot != null && aManzanaPlot.Count() > 0)
                {
                    manzanaPlot = aManzanaPlot[0];
                    existeEspacioVerde = _manzanaPlotRepository.GetExisteEspacioVerde(esquemaManzana, tablaManzana, campoGeometryManzana, campoIdManzanaManzana, idManzanaPlot, 9);
                }
                //}
                int idOCTipo = 9;
                DibujarDatosParcelaReferencia(pdfContentByte, plantilla, aParcelaPlot, manzanaPlot, xMin, yMin, xMax, yMax, espaciadoMM, maxParcelas, maxLimite1, maxLimite2, existeEspacioVerde, esquema, tabla, idOCTipo, porcentajeAreaIncluida, textoFuenteNombre, textoFuenteTamanio, textoFuenteEstilo, colorTexto);

                bool etiquetaMantieneOrientacion = true;
                //double anguloRotacion = 0;
                //bool sentidoHorario = false;
                if ((1 / factorEscala) >= 5000)
                {
                    textoFuenteTamanioGraf = textoFuenteTamanioGraf / 4.0;
                }
                DibujarParcelaEnGraf(pdfContentByte, aParcelaPlot, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloTolerancia, distanciaTolerancia, etiquetaMantieneOrientacion, anguloRotacion, sentidoHorario, anguloRotacionFiltro, textoFuenteNombreGraf, textoFuenteTamanioGraf, textoFuenteEstiloGraf, colorTextoGraf, colorTextoGranUsuario, colorTextoBarrioCarenciado);
            }
            return ret;
        }

        public bool FuncionEspecialDibujarDatosParcelaComercial(PdfContentByte pdfContentByte, Plantilla plantilla, Componente componenteBase, string idObjetoBase, List<string> lstCoordenadas, double xMinBuff, double yMinBuff, double xMaxBuff, double yMaxBuff, double xCentroidBase, double yCentroidBase, double factorEscala, string idManzana, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, ref ParcelaPlot[] aParcelaPlot, ref ManzanaPlot manzanaPlot, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda)
        {
            return false;
            //bool ret = true;
            //int idFuncionAdicional = 6;
            //int filtroGeografico = 1;
            ////Parametros
            ////Parcela
            //string esquema = "GIS_AYSA";
            //string tabla = "CT_PARCELA";
            //string campoFeatId = "ID_PARCELA";
            //string campoGeometry = "GEOMETRY";
            //string campoNroPuerta = "NRO_PUERTA";
            //string campoIdCuadra = "ID_CUADRA";
            //string campoExpediente = "NUMERO";
            //string campoNomCatastral = "NOM_CATASTRAL";
            //string campoIdClienteTipo = "ID_CLIENTE_TIPO";
            ////Manzana
            //string esquemaManzana = "GIS_AYSA";
            //string tablaManzana = "CT_MANZANA";
            //string campoIdManzanaManzana = "ID_MANZANA";
            //string campoGeometryManzana = "GEOMETRY";
            ////Min Max Referencia
            //double xMin = 240;
            //double yMin = 15;
            //double xMax = 288;
            //double yMax = 148;
            //double espaciadoMM = 1;
            //int maxParcelas = 100;
            //int maxLimite1 = 25;
            //int maxLimite2 = 50;
            ////Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
            //int anguloTolerancia = 4;
            //double distanciaTolerancia = 0.1;
            //int porcentajeAreaIncluida = 90;

            //string textoFuenteNombre = "Arial";
            //double textoFuenteTamanio = 2;
            //string textoFuenteEstilo = "0,0,0,0";
            //string textoColor = "#000000";
            //string textoColorBarrioCarenciado = "#0000ff";
            //string textoColorGranUsuario = "#008000";
            //string textoFuenteNombreGraf = "Arial";
            //double textoFuenteTamanioGraf = 3;
            //string textoFuenteEstiloGraf = "0,0,0,0";
            //string textoColorGraf = "#ff9147";

            //#region NumberFormat
            //NumberFormatInfo numberFormat = new NumberFormatInfo();
            //numberFormat.CurrencyDecimalDigits = 4;
            //numberFormat.CurrencyDecimalSeparator = ".";
            //numberFormat.NumberDecimalDigits = 4;
            //numberFormat.NumberDecimalSeparator = ".";
            //numberFormat.PercentDecimalDigits = 2;
            //numberFormat.PercentDecimalSeparator = ".";
            //#endregion

            //#region Parametros de la DB
            //#region Parcela
            //FuncAdicParametro funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_ESQUEMA");
            //if (funcAdicParametro != null)
            //{
            //    esquema = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_TABLA");
            //if (funcAdicParametro != null)
            //{
            //    tabla = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_ID");
            //if (funcAdicParametro != null)
            //{
            //    campoFeatId = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_GEOMETRY");
            //if (funcAdicParametro != null)
            //{
            //    campoGeometry = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_NRO_PUERTA");
            //if (funcAdicParametro != null)
            //{
            //    campoNroPuerta = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_ID_CUADRA");
            //if (funcAdicParametro != null)
            //{
            //    campoIdCuadra = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_EXPEDIENTE");
            //if (funcAdicParametro != null)
            //{
            //    campoExpediente = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_NOM_CATASTRAL");
            //if (funcAdicParametro != null)
            //{
            //    campoNomCatastral = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PARCELA_CAMPO_ID_CLIENTE_TIPO");
            //if (funcAdicParametro != null)
            //{
            //    campoIdClienteTipo = funcAdicParametro.Valor;
            //}
            //#endregion
            //#region Manzana
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_ESQUEMA");
            //if (funcAdicParametro != null)
            //{
            //    esquemaManzana = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_TABLA");
            //if (funcAdicParametro != null)
            //{
            //    tablaManzana = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_ID_MANZANA");
            //if (funcAdicParametro != null)
            //{
            //    campoIdManzanaManzana = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MANZANA_CAMPO_GEOMETRY");
            //if (funcAdicParametro != null)
            //{
            //    campoGeometryManzana = funcAdicParametro.Valor;
            //}
            //#endregion
            //#region xMin yMin etc
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "XMIN");
            //if (funcAdicParametro != null)
            //{
            //    //xMin = Convert.ToDouble(funcAdicParametro.Valor.ToString(CultureInfo.InvariantCulture));
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMin);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "YMIN");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMin);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "XMAX");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out xMax);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "YMAX");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out yMax);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ESPACIADO");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out espaciadoMM);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MAX_PARCELAS");
            //if (funcAdicParametro != null)
            //{
            //    int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out maxParcelas);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MAX_LIMITE1");
            //if (funcAdicParametro != null)
            //{
            //    int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out maxLimite1);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "MAX_LIMITE2");
            //if (funcAdicParametro != null)
            //{
            //    int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out maxLimite2);
            //}
            //#endregion
            //#region Parametros Tolerancia
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "ANGULO_TOLERANCIA");
            //if (funcAdicParametro != null)
            //{
            //    int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out anguloTolerancia);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "DISTANCIA_TOLERANCIA");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out distanciaTolerancia);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "PORCENTAJE_AREA_INCLUIDA");
            //if (funcAdicParametro != null)
            //{
            //    int.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out porcentajeAreaIncluida);
            //}
            //#endregion
            //#region Font
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE");
            //if (funcAdicParametro != null)
            //{
            //    textoFuenteNombre = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR");
            //if (funcAdicParametro != null)
            //{
            //    textoColor = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO");
            //if (funcAdicParametro != null)
            //{
            //    textoFuenteEstilo = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanio);
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR_BARRIO_CARENCIADO");
            //if (funcAdicParametro != null)
            //{
            //    textoColorBarrioCarenciado = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR_GRAN_USUARIO");
            //if (funcAdicParametro != null)
            //{
            //    textoColorGranUsuario = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_NOMBRE_GRAF");
            //if (funcAdicParametro != null)
            //{
            //    textoFuenteNombreGraf = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_COLOR_GRAF");
            //if (funcAdicParametro != null)
            //{
            //    textoColorGraf = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_ESTILO_GRAF");
            //if (funcAdicParametro != null)
            //{
            //    textoFuenteEstiloGraf = funcAdicParametro.Valor;
            //}
            //funcAdicParametro = plantilla.FuncionAdicional.FuncAdicParametros.FirstOrDefault(p => p.IdFuncionAdicional == idFuncionAdicional && p.Campo.ToUpper() == "FUENTE_TAMANIO_GRAF");
            //if (funcAdicParametro != null)
            //{
            //    double.TryParse(funcAdicParametro.Valor, NumberStyles.Any, numberFormat, out textoFuenteTamanioGraf);
            //}

            //#endregion
            //#endregion

            //Color colorTexto = System.Drawing.ColorTranslator.FromHtml(textoColor);
            //Color colorTextoGraf = System.Drawing.ColorTranslator.FromHtml(textoColorGraf);
            //Color colorTextoBarrioCarenciado = System.Drawing.ColorTranslator.FromHtml(textoColorBarrioCarenciado);
            //Color colorTextoGranUsuario = System.Drawing.ColorTranslator.FromHtml(textoColorGranUsuario);

            ////ParcelaPlot[] aParcelaPlot = null;
            //if (aParcelaPlot == null || aParcelaPlot.Length == 0)
            //{
            //    if (lstCoordenadas != null && lstCoordenadas.Count > 0)
            //    {
            //        //aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, lstCoordenadas);
            //        aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByObjetoBase(componenteBase, esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, idObjetoBase);
            //    }
            //    else
            //    {
            //        aParcelaPlot = _parcelaPlotRepository.GetParcelaPlotByCoords(esquema, tabla, campoFeatId, campoGeometry, campoNroPuerta, campoIdCuadra, campoExpediente, campoNomCatastral, campoIdClienteTipo, filtroGeografico, xMinBuff, yMinBuff, xMaxBuff, yMaxBuff);
            //    }
            //}
            //if (aParcelaPlot != null && aParcelaPlot.Count() > 0)
            //{
            //    bool existeEspacioVerde = false;
            //    //if (manzanaPlot == null)
            //    //{
            //    long idManzanaPlot = 0;
            //    bool idOk = long.TryParse(idManzana, out idManzanaPlot);
            //    ManzanaPlot[] aManzanaPlot = _manzanaPlotRepository.GetManzanaPlotByIdManzana(esquemaManzana, tablaManzana, campoGeometryManzana, campoIdManzanaManzana, idManzanaPlot);
            //    if (aManzanaPlot != null && aManzanaPlot.Count() > 0)
            //    {
            //        manzanaPlot = aManzanaPlot[0];
            //        existeEspacioVerde = _manzanaPlotRepository.GetExisteEspacioVerde(esquemaManzana, tablaManzana, campoGeometryManzana, campoIdManzanaManzana, idManzanaPlot, 9);
            //    }

            //    //Descarga de información comercial
            //    aParcelaPlot = _parcelaPlotRepository.GetInformacionComercial(aParcelaPlot);

            //    //}
            //    int idOCTipo = 9;
            //    //Info columna de la derecha
            //    DibujarDatosParcelaReferenciaComercial(pdfContentByte, plantilla, aParcelaPlot, manzanaPlot, xMin, yMin, xMax, yMax, espaciadoMM, maxParcelas, maxLimite1, maxLimite2, existeEspacioVerde, esquema, tabla, idOCTipo, porcentajeAreaIncluida, textoFuenteNombre, textoFuenteTamanio, textoFuenteEstilo, colorTexto, leyenda, infoLeyenda);

            //    bool etiquetaMantieneOrientacion = true;
            //    //double anguloRotacion = 0;
            //    //bool sentidoHorario = false;
            //    if ((1 / factorEscala) >= 5000)
            //    {
            //        textoFuenteTamanioGraf = textoFuenteTamanioGraf / 4.0;
            //    }
            //    //Info parcelas
            //    DibujarParcelaEnGrafComercial(pdfContentByte, aParcelaPlot, xCentroidBase, yCentroidBase, factorEscala, plantilla, anguloTolerancia, distanciaTolerancia, etiquetaMantieneOrientacion, anguloRotacion, sentidoHorario, anguloRotacionFiltro, textoFuenteNombreGraf, textoFuenteTamanioGraf, textoFuenteEstiloGraf, colorTextoGraf, colorTextoGranUsuario, colorTextoBarrioCarenciado, grafico);
            //}
            //return ret;
        }

        private void DibujarDatosParcelaReferencia(PdfContentByte pdfContentByte, Plantilla plantilla, ParcelaPlot[] aParcelaPlot, ManzanaPlot manzanaPlot, double xMin, double yMin, double xMax, double yMax, double espaciadoMM, int maxParcelas, int maxLimite1, int maxLimite2, bool existeEspacioVerde, string esquemaParcela, string tablaParcela, int idOCTipo, int porcentajeAreaIncluida, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color colorTexto)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                float xMinReferencia = it.Utilities.MillimetersToPoints((float)xMin);
                float yMinReferencia = it.Utilities.MillimetersToPoints((float)yMin);
                float xMaxReferencia = it.Utilities.MillimetersToPoints((float)xMax);
                float yMaxReferencia = it.Utilities.MillimetersToPoints((float)yMax);
                float espaciado = it.Utilities.MillimetersToPoints((float)espaciadoMM);

                //PDFUtilities.DrawPDFRectangle2(pdfContentByte, xMinReferencia, yMinReferencia, xMaxReferencia, yMaxReferencia, new it.BaseColor(Color.Gray.R, Color.Gray.G, Color.Gray.B), 1.0f, null, null);

                float x1c = xMinReferencia;
                //float y1c = yMaxReferencia - espaciado - pdfFontSize;
                //float y1c = yMaxReferencia - espaciado;
                float y1c = yMaxReferencia;
                float x2c = 0;
                float y2c = 0;
                int cols = 2;
                int iRow = 1;

                float tableWidth = xMaxReferencia - xMinReferencia - (pdfFontSize / 2);
                int alignmentCol = it.Element.ALIGN_LEFT;

                //Tabla para datos Manzana
                PdfPTable pdfTableManz = new PdfPTable(1);
                pdfTableManz.TotalWidth = tableWidth;
                bool drawBorderManz = false;
                PdfPCell pdfCellManz = new PdfPCell();

                //Tabla para datos Parcela
                PdfPTable pdfTable = new PdfPTable(cols);
                //float tableWidth = xMaxReferencia - xMinReferencia - (pdfFontSize / 2);
                pdfTable.TotalWidth = tableWidth;
                float[] widths = { tableWidth / 2, tableWidth / 2 };
                pdfTable.SetWidths(widths);
                bool drawBorder = false;
                //pdfTable.AddCell(new it.Phrase("Parcela", pdfFontBold));
                //pdfTable.AddCell(new it.Phrase("Superficie", pdfFontBold));
                PdfPCell pdfCell = new PdfPCell();
                int alignmentCol1 = it.Element.ALIGN_LEFT;
                int alignmentCol2 = it.Element.ALIGN_LEFT;
                //pdfCell = PDFUtilities.GetCellForTable("Parcela", pdfFontBold, alignmentCol1, espaciado, drawBorder);
                //pdfTable.AddCell(pdfCell);
                //pdfCell = new PdfPCell();
                //pdfCell = PDFUtilities.GetCellForTable("Superficie", pdfFontBold, alignmentCol2, espaciado, drawBorder);
                //pdfTable.AddCell(pdfCell);
                float sizeTexto = pdfbaseFont.GetWidthPoint("99999.00", pdfFontSize);
                //x1c = x1c + (pdfFontSize / 2);
                //x2c = xMinReferencia + (xMaxReferencia - xMinReferencia) / cols + (pdfFontSize / 2);

                //Tabla para mensaje Cant expedientes no impresos
                PdfPTable pdfTableNoImpresos = new PdfPTable(1);
                pdfTableNoImpresos.TotalWidth = tableWidth;
                bool drawBorderNoImpresos = false;
                PdfPCell pdfCellNoImpresos = new PdfPCell();


                //List<ParcelaPlot> lstParcelaPlot = aParcelaPlot.ToList().OrderBy(p => Convert.ToInt64(p.Expediente)).ToList();
                //List<ParcelaPlot> lstParcelaPlot = aParcelaPlot.ToList();
                //var myComparer = new CustomComparer();
                //lstParcelaPlot.Sort(myComparer);
                List<ParcelaPlot> lstParcelaPlot = aParcelaPlot.ToList().OrderBy(x => x, new SemiNumericComparer()).ToList();

                int cantRows = maxParcelas / 2;
                int rowsColumn = Convert.ToInt32(Math.Ceiling(aParcelaPlot.Length / 2.0));
                if (rowsColumn <= maxLimite1)
                {
                    espaciado = espaciado * 2;
                }
                else if (rowsColumn > maxLimite2)
                {
                    rowsColumn = maxLimite2;
                }
                int cantExpNoImpresos = 0;
                if (lstParcelaPlot.Count() > maxParcelas)
                {
                    cantExpNoImpresos = lstParcelaPlot.Count() - maxParcelas;
                    lstParcelaPlot.RemoveRange(maxParcelas, cantExpNoImpresos);
                }

                List<ParcelaPlot> lstParcelaPlotSecuencial = new List<ParcelaPlot>();
                int iRowCol2 = rowsColumn;
                double sumSuperficieParc = 0;
                for (int i = 0; i < rowsColumn && i < lstParcelaPlot.Count(); i++)
                {
                    sumSuperficieParc += lstParcelaPlot[i].Superficie;
                    lstParcelaPlotSecuencial.Add(lstParcelaPlot[i]);
                    if (iRowCol2 < lstParcelaPlot.Count)
                    {
                        sumSuperficieParc += lstParcelaPlot[iRowCol2].Superficie;
                        lstParcelaPlotSecuencial.Add(lstParcelaPlot[iRowCol2]);
                        iRowCol2++;
                    }
                }
                y2c = y1c - (espaciado * 2) - (rowsColumn * espaciado * 2);
                y2c = yMinReferencia + espaciado;
                string texto = string.Empty;
                if (manzanaPlot != null)
                {
                    texto = "SUP MANZANA: " + Math.Round(manzanaPlot.Superficie, 0).ToString() + " / " + Math.Round(sumSuperficieParc, 0).ToString();
                    pdfCellManz = PDFUtilities.GetCellForTable(texto, pdfFont, alignmentCol, espaciado, drawBorderManz);
                    pdfTableManz.AddCell(pdfCellManz);
                    pdfCellManz = PDFUtilities.GetCellForTable(" ", pdfFont, alignmentCol, espaciado, drawBorderManz);
                    pdfTableManz.AddCell(pdfCellManz);
                    pdfTableManz.WriteSelectedRows(0, -1, x1c, y1c, pdfContentByte);
                }
                int cantParc = 1;
                foreach (var parcelaPlot in lstParcelaPlotSecuencial)
                {
                    //Rectangle rectTextoCol1 = new Rectangle(x1c, y1c, (int)Math.Ceiling(sizeTexto.Width), (int)Math.Ceiling(sizeTexto.Height));
                    //Rectangle rectTextoCol2 = new Rectangle(x2c, y1c, (int)sizeTexto.Width, (int)sizeTexto.Height);
                    pdfCell = new PdfPCell();
                    if (cantParc <= maxParcelas)
                    {
                        bool parcelaEnEspacioVerde = false;
                        if (existeEspacioVerde)
                        {
                            parcelaEnEspacioVerde = _parcelaPlotRepository.GetSuperposicionOCObjetoByIdParcela(esquemaParcela, tablaParcela, parcelaPlot.FeatId, idOCTipo, porcentajeAreaIncluida);
                            parcelaPlot.EsEspacioVerde = parcelaEnEspacioVerde;
                        }
                        texto = (parcelaPlot.Expediente.Contains("#") ? parcelaPlot.Expediente : parcelaPlot.Expediente.Trim().PadLeft(6, '0'));
                        texto = texto + (parcelaPlot.IdClienteTipo == 1 ? "-GU" : string.Empty);
                        texto = texto + (parcelaPlot.EsBarrioCarenciado ? "-BC" : string.Empty);
                        texto = texto + (parcelaPlot.EsEspacioVerde ? "-EV" : string.Empty);
                        //texto = texto + " (" + parcelaPlot.NomCatastral + ")";
                        texto = texto + (parcelaPlot.NomCatastral != string.Empty ? " (" + parcelaPlot.NomCatastral + ")" : "");
                        string superficie = Math.Round(parcelaPlot.Superficie, 0).ToString();
                        texto = texto + ": " + superficie;
                        //DrawPDFText(pdfContentByte, texto, x1c, y1c, pdfFontText, pdfFontSize, alignment);
                        pdfCell = PDFUtilities.GetCellForTable(texto, pdfFont, alignmentCol1, espaciado, drawBorder);
                        pdfTable.AddCell(pdfCell);
                        //texto = (parcelaPlot.Expediente.Contains("#") ? parcelaPlot.Expediente : parcelaPlot.Expediente.Trim().PadLeft(6, '0'));
                        //texto = texto + (parcelaPlot.IdClienteTipo == 1 ? " - GU" : string.Empty);
                        //texto = texto + " ( " + parcelaPlot.NomCatastral + ")";
                        //string superficie = Math.Round(parcelaPlot.Superficie, 0).ToString();
                        //texto = texto + ": " + superficie;
                        ////DrawPDFText(pdfContentByte, superficie, x2c, y1c, pdfFontText, pdfFontSize, alignment);
                        //pdfCell = PDFUtilities.GetCellForTable(superficie, pdfFont, alignmentCol2, espaciado, drawBorder);
                        //pdfTable.AddCell(pdfCell);

                    }
                    //y1c = y1c - pdfFontSize - espaciado;
                    //if ((iRow % 2) == 0)
                    //{
                    //    iRow++;
                    //}
                    cantParc++;
                }
                if (lstParcelaPlotSecuencial.Count() % 2 != 0)
                {
                    pdfCell = PDFUtilities.GetCellForTable(" ", pdfFont, alignmentCol2, espaciado, drawBorder);
                    pdfTable.AddCell(pdfCell);
                }
                if (cantExpNoImpresos > 0)
                {
                    //int cantExpNoImpresos = aParcelaPlot.Length - (rowsColumn * 2);
                    texto = "Cantidad de Exp. No impresos: " + cantExpNoImpresos.ToString();
                    pdfCellNoImpresos = PDFUtilities.GetCellForTable(texto, pdfFont, alignmentCol, espaciado, drawBorderNoImpresos);
                    pdfTableNoImpresos.AddCell(pdfCellNoImpresos);
                    pdfCellNoImpresos = PDFUtilities.GetCellForTable(" ", pdfFont, alignmentCol, espaciado, drawBorderNoImpresos);
                    pdfTableNoImpresos.AddCell(pdfCellNoImpresos);
                    pdfTableNoImpresos.WriteSelectedRows(0, -1, x1c, y2c, pdfContentByte);
                }
                pdfTable.WriteSelectedRows(0, -1, x1c, y1c - (espaciado * 2), pdfContentByte);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }


        private void DibujarDatosParcelaReferenciaComercial(PdfContentByte pdfContentByte, Plantilla plantilla, ParcelaPlot[] aParcelaPlot, ManzanaPlot manzanaPlot, double xMin, double yMin, double xMax, double yMax, double espaciadoMM, int maxParcelas, int maxLimite1, int maxLimite2, bool existeEspacioVerde, string esquemaParcela, string tablaParcela, int idOCTipo, int porcentajeAreaIncluida, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color colorTexto, InformacionComercial leyenda, int? infoLeyenda)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                float xMinReferencia = it.Utilities.MillimetersToPoints((float)xMin);
                float yMinReferencia = it.Utilities.MillimetersToPoints((float)yMin);
                float xMaxReferencia = it.Utilities.MillimetersToPoints((float)xMax);
                float yMaxReferencia = it.Utilities.MillimetersToPoints((float)yMax);
                float espaciado = it.Utilities.MillimetersToPoints((float)espaciadoMM);

                //PDFUtilities.DrawPDFRectangle2(pdfContentByte, xMinReferencia, yMinReferencia, xMaxReferencia, yMaxReferencia, new it.BaseColor(Color.Gray.R, Color.Gray.G, Color.Gray.B), 1.0f, null, null);

                float x1c = xMinReferencia;
                //float y1c = yMaxReferencia - espaciado - pdfFontSize;
                //float y1c = yMaxReferencia - espaciado;
                float y1c = yMaxReferencia;
                float x2c = 0;
                float y2c = 0;
                int cols = 2;
                int iRow = 1;

                float tableWidth = xMaxReferencia - xMinReferencia - (pdfFontSize / 2);
                int alignmentCol = it.Element.ALIGN_LEFT;

                //Tabla para datos Manzana
                PdfPTable pdfTableManz = new PdfPTable(1);
                pdfTableManz.TotalWidth = tableWidth;
                bool drawBorderManz = false;
                PdfPCell pdfCellManz = new PdfPCell();

                //Tabla para datos Parcela
                PdfPTable pdfTable = new PdfPTable(cols);
                //float tableWidth = xMaxReferencia - xMinReferencia - (pdfFontSize / 2);
                pdfTable.TotalWidth = tableWidth;
                float[] widths = { tableWidth / 2, tableWidth / 2 };
                pdfTable.SetWidths(widths);
                bool drawBorder = false;
                //pdfTable.AddCell(new it.Phrase("Parcela", pdfFontBold));
                //pdfTable.AddCell(new it.Phrase("Superficie", pdfFontBold));
                PdfPCell pdfCell = new PdfPCell();
                int alignmentCol1 = it.Element.ALIGN_LEFT;
                int alignmentCol2 = it.Element.ALIGN_LEFT;
                //pdfCell = PDFUtilities.GetCellForTable("Parcela", pdfFontBold, alignmentCol1, espaciado, drawBorder);
                //pdfTable.AddCell(pdfCell);
                //pdfCell = new PdfPCell();
                //pdfCell = PDFUtilities.GetCellForTable("Superficie", pdfFontBold, alignmentCol2, espaciado, drawBorder);
                //pdfTable.AddCell(pdfCell);
                float sizeTexto = pdfbaseFont.GetWidthPoint("99999.00", pdfFontSize);
                //x1c = x1c + (pdfFontSize / 2);
                //x2c = xMinReferencia + (xMaxReferencia - xMinReferencia) / cols + (pdfFontSize / 2);

                //Tabla para mensaje Cant expedientes no impresos
                PdfPTable pdfTableNoImpresos = new PdfPTable(1);
                pdfTableNoImpresos.TotalWidth = tableWidth;
                bool drawBorderNoImpresos = false;
                PdfPCell pdfCellNoImpresos = new PdfPCell();


                //List<ParcelaPlot> lstParcelaPlot = aParcelaPlot.ToList().OrderBy(p => Convert.ToInt64(p.Expediente)).ToList();
                //List<ParcelaPlot> lstParcelaPlot = aParcelaPlot.ToList();
                //var myComparer = new CustomComparer();
                //lstParcelaPlot.Sort(myComparer);
                List<ParcelaPlot> lstParcelaPlot = aParcelaPlot.ToList().OrderBy(x => x, new SemiNumericComparer()).ToList();

                int cantRows = maxParcelas / 2;
                int rowsColumn = Convert.ToInt32(Math.Ceiling(aParcelaPlot.Length / 2.0));
                if (rowsColumn <= maxLimite1)
                {
                    espaciado = espaciado * 2;
                }
                else if (rowsColumn > maxLimite2)
                {
                    rowsColumn = maxLimite2;
                }
                int cantExpNoImpresos = 0;
                if (lstParcelaPlot.Count() > maxParcelas)
                {
                    cantExpNoImpresos = lstParcelaPlot.Count() - maxParcelas;
                    lstParcelaPlot.RemoveRange(maxParcelas, cantExpNoImpresos);
                }

                List<ParcelaPlot> lstParcelaPlotSecuencial = new List<ParcelaPlot>();
                int iRowCol2 = rowsColumn;
                double sumSuperficieParc = 0;
                for (int i = 0; i < rowsColumn && i < lstParcelaPlot.Count(); i++)
                {
                    sumSuperficieParc += lstParcelaPlot[i].Superficie;
                    lstParcelaPlotSecuencial.Add(lstParcelaPlot[i]);
                    if (iRowCol2 < lstParcelaPlot.Count)
                    {
                        sumSuperficieParc += lstParcelaPlot[iRowCol2].Superficie;
                        lstParcelaPlotSecuencial.Add(lstParcelaPlot[iRowCol2]);
                        iRowCol2++;
                    }
                }
                y2c = y1c - (espaciado * 2) - (rowsColumn * espaciado * 2);
                y2c = yMinReferencia + espaciado;
                string texto = string.Empty;
                if (manzanaPlot != null)
                {
                    texto = "SUP MANZANA: " + Math.Round(manzanaPlot.Superficie, 0).ToString() + " / " + Math.Round(sumSuperficieParc, 0).ToString();
                    pdfCellManz = PDFUtilities.GetCellForTable(texto, pdfFont, alignmentCol, espaciado, drawBorderManz);
                    pdfTableManz.AddCell(pdfCellManz);
                    pdfCellManz = PDFUtilities.GetCellForTable(" ", pdfFont, alignmentCol, espaciado, drawBorderManz);
                    pdfTableManz.AddCell(pdfCellManz);
                    pdfTableManz.WriteSelectedRows(0, -1, x1c, y1c, pdfContentByte);
                }
                int cantParc = 1;
                foreach (var parcelaPlot in lstParcelaPlotSecuencial)
                {
                    //Rectangle rectTextoCol1 = new Rectangle(x1c, y1c, (int)Math.Ceiling(sizeTexto.Width), (int)Math.Ceiling(sizeTexto.Height));
                    //Rectangle rectTextoCol2 = new Rectangle(x2c, y1c, (int)sizeTexto.Width, (int)sizeTexto.Height);
                    pdfCell = new PdfPCell();
                    if (cantParc <= maxParcelas)
                    {
                        bool parcelaEnEspacioVerde = false;
                        if (existeEspacioVerde)
                        {
                            parcelaEnEspacioVerde = _parcelaPlotRepository.GetSuperposicionOCObjetoByIdParcela(esquemaParcela, tablaParcela, parcelaPlot.FeatId, idOCTipo, porcentajeAreaIncluida);
                            parcelaPlot.EsEspacioVerde = parcelaEnEspacioVerde;
                        }
                        texto = (parcelaPlot.Expediente.Contains("#") ? parcelaPlot.Expediente : parcelaPlot.Expediente.Trim().PadLeft(6, '0'));
                        texto = texto + (parcelaPlot.IdClienteTipo == 1 ? "-GU" : string.Empty);
                        texto = texto + (parcelaPlot.EsBarrioCarenciado ? "-BC" : string.Empty);
                        texto = texto + (parcelaPlot.EsEspacioVerde ? "-EV" : string.Empty);
                        //texto = texto + " (" + parcelaPlot.NomCatastral + ")";
                        texto = texto + (parcelaPlot.NomCatastral != string.Empty ? " (" + parcelaPlot.NomCatastral + ")" : "");

                        switch (infoLeyenda)
                        {
                            case 1: //Superficie terreno
                                string superficie = Math.Round(parcelaPlot.Superficie, 0).ToString();
                                texto += ": " + superficie;
                                break;
                            case 2: //Partida ARBA
                                //texto += ": part. arba xxxx"; //TODO: PARTIDA ARBA
                                break;
                            case 3: //Sup M2 Construido
                                //texto += ": xx sup constr"; //TODO: SUP M2 CONSTRUIDO
                                break;
                        }
                        if (leyenda != null)
                        {
                            var textoInformacionComercial = GetStringInformacionComercial(parcelaPlot.InformacionComercial, leyenda);
                            if (textoInformacionComercial != String.Empty)
                            {
                                texto += " - " + textoInformacionComercial;
                            }

                        }

                        //string superficie = Math.Round(parcelaPlot.Superficie, 0).ToString();
                        //texto = texto + ": " + superficie;

                        //DrawPDFText(pdfContentByte, texto, x1c, y1c, pdfFontText, pdfFontSize, alignment);
                        pdfCell = PDFUtilities.GetCellForTable(texto, pdfFont, alignmentCol1, espaciado, drawBorder);
                        pdfTable.AddCell(pdfCell);
                        //texto = (parcelaPlot.Expediente.Contains("#") ? parcelaPlot.Expediente : parcelaPlot.Expediente.Trim().PadLeft(6, '0'));
                        //texto = texto + (parcelaPlot.IdClienteTipo == 1 ? " - GU" : string.Empty);
                        //texto = texto + " ( " + parcelaPlot.NomCatastral + ")";
                        //string superficie = Math.Round(parcelaPlot.Superficie, 0).ToString();
                        //texto = texto + ": " + superficie;
                        ////DrawPDFText(pdfContentByte, superficie, x2c, y1c, pdfFontText, pdfFontSize, alignment);
                        //pdfCell = PDFUtilities.GetCellForTable(superficie, pdfFont, alignmentCol2, espaciado, drawBorder);
                        //pdfTable.AddCell(pdfCell);

                    }
                    //y1c = y1c - pdfFontSize - espaciado;
                    //if ((iRow % 2) == 0)
                    //{
                    //    iRow++;
                    //}
                    cantParc++;
                }
                if (lstParcelaPlotSecuencial.Count() % 2 != 0)
                {
                    pdfCell = PDFUtilities.GetCellForTable(" ", pdfFont, alignmentCol2, espaciado, drawBorder);
                    pdfTable.AddCell(pdfCell);
                }
                if (cantExpNoImpresos > 0)
                {
                    //int cantExpNoImpresos = aParcelaPlot.Length - (rowsColumn * 2);
                    texto = "Cantidad de Exp. No impresos: " + cantExpNoImpresos.ToString();
                    pdfCellNoImpresos = PDFUtilities.GetCellForTable(texto, pdfFont, alignmentCol, espaciado, drawBorderNoImpresos);
                    pdfTableNoImpresos.AddCell(pdfCellNoImpresos);
                    pdfCellNoImpresos = PDFUtilities.GetCellForTable(" ", pdfFont, alignmentCol, espaciado, drawBorderNoImpresos);
                    pdfTableNoImpresos.AddCell(pdfCellNoImpresos);
                    pdfTableNoImpresos.WriteSelectedRows(0, -1, x1c, y2c, pdfContentByte);
                }
                pdfTable.WriteSelectedRows(0, -1, x1c, y1c - (espaciado * 2), pdfContentByte);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void DibujarParcelaEnGraf(PdfContentByte pdfContentByte, ParcelaPlot[] aParcelaPlot, double xCentroidBase, double yCentroidBase, double factorEscala, Plantilla plantilla, int anguloTolerancia, double distanciaTolerancia, bool etiquetaMantieneOrientacion, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color colorTexto, Color colorTextoGranUsuario, Color colorTextoBarrioCarenciado)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                it.BaseColor pdfColorTextoGranUsuario = new it.BaseColor(colorTextoGranUsuario.R, colorTextoGranUsuario.G, colorTextoGranUsuario.B);
                it.BaseColor pdfColorTextoBarrioCarenciado = new it.BaseColor(colorTextoBarrioCarenciado.R, colorTextoBarrioCarenciado.G, colorTextoBarrioCarenciado.B);
                it.BaseColor pdfColorTextoParc = pdfColorTexto;
                Color colorTextoParc = colorTexto;

                if ((1 / factorEscala) >= 5000)
                {
                    textoFuenteTamanio = textoFuenteTamanio * 4.0;
                }
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();

                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                BaseFont pdfbaseFontTexto = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFontTexto = new it.Font(pdfbaseFontTexto, pdfFontSize, pdfFontStyle, pdfColorTexto);

                BaseFont pdfbaseFontGranUsuario = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTextoGranUsuario).BaseFont;
                it.Font pdfFontGranUsuario = new it.Font(pdfbaseFontGranUsuario, pdfFontSize, pdfFontStyle, pdfColorTextoGranUsuario);
                BaseFont pdfbaseFontBarrioCarenciado = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTextoBarrioCarenciado).BaseFont;
                it.Font pdfFontGranBarrioCarenciado = new it.Font(pdfbaseFontBarrioCarenciado, pdfFontSize, pdfFontStyle, pdfColorTextoBarrioCarenciado);

                int alignment = it.Element.ALIGN_CENTER + it.Element.ALIGN_MIDDLE;

                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                float x1Pdf = 0;
                float y1Pdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                string textoParc = string.Empty;
                foreach (var parcelaPlot in aParcelaPlot)
                {
                    textoParc = parcelaPlot.Expediente;
                    //bool parcelaEnEspacioVerde = false;
                    //if (existeEspacioVerde)
                    //{
                    //    parcelaEnEspacioVerde = _parcelaPlotRepository.GetSuperposicionOCObjetoByIdParcela(esquemaParcela, tablaParcela, parcelaPlot.FeatId, idOCTipo, porcentajeAreaIncluida);
                    //}
                    colorTextoParc = colorTexto;
                    pdfColorTextoParc = pdfColorTexto;
                    pdfbaseFont = pdfbaseFontTexto;
                    pdfFont = pdfFontTexto;
                    if (parcelaPlot.IdClienteTipo != null && parcelaPlot.IdClienteTipo > 0)
                    {
                        colorTextoParc = colorTextoGranUsuario;
                        pdfColorTextoParc = pdfColorTextoGranUsuario;
                        pdfbaseFont = pdfbaseFontGranUsuario;
                        pdfFont = pdfFontGranUsuario;
                    }
                    else
                    {
                        if (parcelaPlot.EsBarrioCarenciado)
                        {
                            colorTextoParc = colorTextoBarrioCarenciado;
                            pdfColorTextoParc = pdfColorTextoBarrioCarenciado;
                            pdfbaseFont = pdfbaseFontBarrioCarenciado;
                            pdfFont = pdfFontGranBarrioCarenciado;
                        }
                    }
                    PointF puntoMedio = new PointF();
                    List<Lado> lados = new List<Lado>();
                    Lado ladoMayor = new Lado();
                    double anguloRotacionParcela = GetAnguloRotacion(parcelaPlot.Geom, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);
                    //Paso a radianes
                    anguloRotacionParcela = anguloRotacionParcela * Math.PI / 180;

                    DbGeometry geometryParcelaPlot = parcelaPlot.Geom;
                    #region Codigo Comentado
                    //string wkt = geometryParcelaPlot.AsText();
                    //if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                    //{
                    //    int cantCoords = (int)geometryParcelaPlot.PointCount;
                    //    for (int i = 1; i <= cantCoords; i++)
                    //    {
                    //        x = (double)geometryParcelaPlot.PointAt(i).XCoordinate;
                    //        y = (double)geometryParcelaPlot.PointAt(i).YCoordinate;
                    //        if (i > 1)
                    //        {
                    //            x1 = x2;
                    //            y1 = y2;
                    //            x2 = x;
                    //            y2 = y;
                    //        }
                    //        else
                    //        {
                    //            x1 = x;
                    //            y1 = y;
                    //            x2 = x;
                    //            y2 = y;
                    //        }
                    //        if (i > 1)
                    //        {
                    //            //dibujar
                    //            x1Pdf = GetXPDFCanvas(x1, xCentroidBase, factorEscala, plantilla);
                    //            y1Pdf = GetYPDFCanvas(y1, yCentroidBase, factorEscala, plantilla);
                    //            x2Pdf = GetXPDFCanvas(x2, xCentroidBase, factorEscala, plantilla);
                    //            y2Pdf = GetYPDFCanvas(y2, yCentroidBase, factorEscala, plantilla);
                    //        }
                    //    }
                    //    x2 = x;
                    //    y2 = y;
                    //}
                    #endregion
                    double xCentroidLayerGraf = 0;
                    double yCentroidLayerGraf = 0;
                    xCentroidLayerGraf = (double)geometryParcelaPlot.Centroid.XCoordinate;
                    yCentroidLayerGraf = (double)geometryParcelaPlot.Centroid.YCoordinate;
                    if (!etiquetaMantieneOrientacion)
                    {
                        float sizeText = pdfbaseFont.GetWidthPoint(textoParc, pdfFontSize);
                        x1Pdf = GetXPDFCanvas(xCentroidLayerGraf, xCentroidBase, factorEscala, plantilla) - sizeText / 2;
                        //y1Pdf = GetYPDFCanvas(yCentroidLayerGraf, yCentroidBase, escala, plantilla) - pdfFontSize;
                        y1Pdf = GetYPDFCanvas(yCentroidLayerGraf, yCentroidBase, factorEscala, plantilla);

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
                            PDFUtilities.DrawPDFText(pdfContentByte, textoParc, xDrawPoint, yDrawPoint, pdfFont, pdfFontSize, alignment);
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

                        x1Pdf = GetXPDFCanvas(xCalc, xCentroidBase, factorEscala, plantilla);
                        y1Pdf = GetYPDFCanvas(yCalc, yCentroidBase, factorEscala, plantilla);
                        if (plantilla.OptimizarTamanioHoja)
                        {
                            PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                            double anguloPdf = ladoMayor.Angulo - (anguloRotacionFiltro * 180 / Math.PI);
                            if (anguloPdf < -85)
                                anguloPdf = anguloPdf + 180;
                            PDFUtilities.DrawPDFText(pdfContentByte, textoParc, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTextoParc, anguloPdf);
                        }
                        else
                        {
                            double anguloPdf = Math.Atan((y2 - y1) / (x2 - x1));
                            //anguloPdf = anguloPdf * 180 / Math.PI;
                            anguloPdf = ladoMayor.Angulo;
                            if (anguloPdf < -85)
                                anguloPdf = anguloPdf + 180;
                            //ColumnText.ShowTextAligned(pdfContentByte, it.Element.ALIGN_CENTER, new it.Phrase(layerGrafNombre), x1c, y1Pdf, (float)angulo);
                            PDFUtilities.DrawPDFText(pdfContentByte, textoParc, x1Pdf, y1Pdf, pdfbaseFont, pdfFontSize, colorTextoParc, anguloPdf);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void DibujarParcelaEnGrafComercial(PdfContentByte pdfContentByte, ParcelaPlot[] aParcelaPlot, double xCentroidBase, double yCentroidBase, double factorEscala, Plantilla plantilla, int anguloTolerancia, double distanciaTolerancia, bool etiquetaMantieneOrientacion, double anguloRotacion, bool sentidoHorario, double anguloRotacionFiltro, string textoFuenteNombre, double textoFuenteTamanio, string textoFuenteEstilo, Color colorTexto, Color colorTextoGranUsuario, Color colorTextoBarrioCarenciado, InformacionComercial grafico)
        {
            try
            {
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);
                it.BaseColor pdfColorTextoGranUsuario = new it.BaseColor(colorTextoGranUsuario.R, colorTextoGranUsuario.G, colorTextoGranUsuario.B);
                it.BaseColor pdfColorTextoBarrioCarenciado = new it.BaseColor(colorTextoBarrioCarenciado.R, colorTextoBarrioCarenciado.G, colorTextoBarrioCarenciado.B);
                it.BaseColor pdfColorTextoParc = pdfColorTexto;
                Color colorTextoParc = colorTexto;
                float pdfFontSize = it.Utilities.MillimetersToPoints((float)textoFuenteTamanio);

                PDFUtilities.RegisterBaseFont(textoFuenteNombre, pdfFontSize);
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                string[] aFontStylePdf = textoFuenteEstilo.Split(',');
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();

                BaseFont pdfbaseFont = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                BaseFont pdfbaseFontTexto = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFontTexto = new it.Font(pdfbaseFontTexto, pdfFontSize, pdfFontStyle, pdfColorTexto);

                BaseFont pdfbaseFontGranUsuario = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTextoGranUsuario).BaseFont;
                it.Font pdfFontGranUsuario = new it.Font(pdfbaseFontGranUsuario, pdfFontSize, pdfFontStyle, pdfColorTextoGranUsuario);
                BaseFont pdfbaseFontBarrioCarenciado = it.FontFactory.GetFont(textoFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTextoBarrioCarenciado).BaseFont;
                it.Font pdfFontGranBarrioCarenciado = new it.Font(pdfbaseFontBarrioCarenciado, pdfFontSize, pdfFontStyle, pdfColorTextoBarrioCarenciado);

                int alignment = it.Element.ALIGN_CENTER + it.Element.ALIGN_MIDDLE;

                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                float x1Pdf = 0;
                float y1Pdf = 0;
                float xCentro = it.Utilities.MillimetersToPoints((float)plantilla.X_Centro);
                float yCentro = it.Utilities.MillimetersToPoints((float)plantilla.Y_Centro);
                string textoParc = string.Empty;
                foreach (var parcelaPlot in aParcelaPlot)
                {
                    textoParc = parcelaPlot.Expediente;

                    if (parcelaPlot.InformacionComercial != null && grafico != null)
                    {
                        var textoInformacionComercial = GetStringInformacionComercial(parcelaPlot.InformacionComercial, grafico);
                        if (textoInformacionComercial != String.Empty)
                        {
                            textoParc += " - " + textoInformacionComercial;
                        }
                    }

                    //bool parcelaEnEspacioVerde = false;
                    //if (existeEspacioVerde)
                    //{
                    //    parcelaEnEspacioVerde = _parcelaPlotRepository.GetSuperposicionOCObjetoByIdParcela(esquemaParcela, tablaParcela, parcelaPlot.FeatId, idOCTipo, porcentajeAreaIncluida);
                    //}
                    colorTextoParc = colorTexto;
                    pdfColorTextoParc = pdfColorTexto;
                    pdfbaseFont = pdfbaseFontTexto;
                    pdfFont = pdfFontTexto;
                    if (parcelaPlot.IdClienteTipo != null && parcelaPlot.IdClienteTipo > 0)
                    {
                        colorTextoParc = colorTextoGranUsuario;
                        pdfColorTextoParc = pdfColorTextoGranUsuario;
                        pdfbaseFont = pdfbaseFontGranUsuario;
                        pdfFont = pdfFontGranUsuario;
                    }
                    else
                    {
                        if (parcelaPlot.EsBarrioCarenciado)
                        {
                            colorTextoParc = colorTextoBarrioCarenciado;
                            pdfColorTextoParc = pdfColorTextoBarrioCarenciado;
                            pdfbaseFont = pdfbaseFontBarrioCarenciado;
                            pdfFont = pdfFontGranBarrioCarenciado;
                        }
                    }
                    PointF puntoMedio = new PointF();
                    List<Lado> lados = new List<Lado>();
                    Lado ladoMayor = new Lado();
                    double anguloRotacionParcela = GetAnguloRotacion(parcelaPlot.Geom, anguloTolerancia, distanciaTolerancia, ref puntoMedio, ref lados, ref ladoMayor);
                    //Paso a radianes
                    anguloRotacionParcela = anguloRotacionParcela * Math.PI / 180;

                    DbGeometry geometryParcelaPlot = parcelaPlot.Geom;
                    #region Codigo Comentado
                    //string wkt = geometryParcelaPlot.AsText();
                    //if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                    //{
                    //    int cantCoords = (int)geometryParcelaPlot.PointCount;
                    //    for (int i = 1; i <= cantCoords; i++)
                    //    {
                    //        x = (double)geometryParcelaPlot.PointAt(i).XCoordinate;
                    //        y = (double)geometryParcelaPlot.PointAt(i).YCoordinate;
                    //        if (i > 1)
                    //        {
                    //            x1 = x2;
                    //            y1 = y2;
                    //            x2 = x;
                    //            y2 = y;
                    //        }
                    //        else
                    //        {
                    //            x1 = x;
                    //            y1 = y;
                    //            x2 = x;
                    //            y2 = y;
                    //        }
                    //        if (i > 1)
                    //        {
                    //            //dibujar
                    //            x1Pdf = GetXPDFCanvas(x1, xCentroidBase, factorEscala, plantilla);
                    //            y1Pdf = GetYPDFCanvas(y1, yCentroidBase, factorEscala, plantilla);
                    //            x2Pdf = GetXPDFCanvas(x2, xCentroidBase, factorEscala, plantilla);
                    //            y2Pdf = GetYPDFCanvas(y2, yCentroidBase, factorEscala, plantilla);
                    //        }
                    //    }
                    //    x2 = x;
                    //    y2 = y;
                    //}
                    #endregion
                    double xCentroidLayerGraf = 0;
                    double yCentroidLayerGraf = 0;
                    xCentroidLayerGraf = (double)geometryParcelaPlot.Centroid.XCoordinate;
                    yCentroidLayerGraf = (double)geometryParcelaPlot.Centroid.YCoordinate;
                    if (!etiquetaMantieneOrientacion)
                    {
                        float sizeText = pdfbaseFont.GetWidthPoint(textoParc, pdfFontSize);
                        x1Pdf = GetXPDFCanvas(xCentroidLayerGraf, xCentroidBase, factorEscala, plantilla) - sizeText / 2;
                        //y1Pdf = GetYPDFCanvas(yCentroidLayerGraf, yCentroidBase, escala, plantilla) - pdfFontSize;
                        y1Pdf = GetYPDFCanvas(yCentroidLayerGraf, yCentroidBase, factorEscala, plantilla);

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
                            PDFUtilities.DrawPDFText(pdfContentByte, textoParc, xDrawPoint, yDrawPoint, pdfFont, pdfFontSize, alignment);
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

                        x1Pdf = GetXPDFCanvas(xCalc, xCentroidBase, factorEscala, plantilla);
                        y1Pdf = GetYPDFCanvas(yCalc, yCentroidBase, factorEscala, plantilla);
                        if (plantilla.OptimizarTamanioHoja)
                        {
                            PointF ptRotado = Rotate(x1Pdf, y1Pdf, xCentro, yCentro, anguloRotacion, sentidoHorario);
                            double anguloPdf = ladoMayor.Angulo - (anguloRotacionFiltro * 180 / Math.PI);
                            if (anguloPdf < -85)
                                anguloPdf = anguloPdf + 180;
                            PDFUtilities.DrawPDFText(pdfContentByte, textoParc, ptRotado.X, ptRotado.Y, pdfbaseFont, pdfFontSize, colorTextoParc, anguloPdf);
                        }
                        else
                        {
                            double anguloPdf = Math.Atan((y2 - y1) / (x2 - x1));
                            //anguloPdf = anguloPdf * 180 / Math.PI;
                            anguloPdf = ladoMayor.Angulo;
                            if (anguloPdf < -85)
                                anguloPdf = anguloPdf + 180;
                            //ColumnText.ShowTextAligned(pdfContentByte, it.Element.ALIGN_CENTER, new it.Phrase(layerGrafNombre), x1c, y1Pdf, (float)angulo);
                            PDFUtilities.DrawPDFText(pdfContentByte, textoParc, x1Pdf, y1Pdf, pdfbaseFont, pdfFontSize, colorTextoParc, anguloPdf);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private string GetStringInformacionComercial(InformacionComercial informacionComercial, InformacionComercial informacionSeleccionada)
        {

            List<string> propiedadesComerciales = new List<string>();

            if (informacionSeleccionada.BA && informacionComercial.BA)
            {
                propiedadesComerciales.Add("BA");
            }
            else if (informacionSeleccionada.PV && informacionComercial.PV)
            {
                propiedadesComerciales.Add("PV");
            }
            else if (informacionSeleccionada.PH && informacionComercial.PH)
            {
                propiedadesComerciales.Add("PH");
            }

            if (informacionSeleccionada.AO && informacionComercial.AO)
            {
                propiedadesComerciales.Add("AO");
            }

            if (informacionSeleccionada.CC && informacionComercial.CC)
            {
                propiedadesComerciales.Add("CC");
            }

            if (informacionSeleccionada.BC && informacionComercial.BC)
            {
                propiedadesComerciales.Add("BC");
            }

            if (informacionSeleccionada.DH && informacionComercial.DH)
            {
                propiedadesComerciales.Add("DH");
            }

            if (informacionSeleccionada.OL && informacionComercial.OL)
            {
                propiedadesComerciales.Add("OL");
            }

            if (informacionSeleccionada.TI && informacionComercial.TI_Value != null)
            {
                propiedadesComerciales.Add(informacionComercial.TI_Value);
            }

            if (informacionSeleccionada.DI && informacionComercial.DI)
            {
                propiedadesComerciales.Add("DI");
            }

            if (informacionSeleccionada.ARBA && informacionComercial.ARBA)
            {
                propiedadesComerciales.Add("ARBA");
            }

            string texto = "";
            if (propiedadesComerciales.Count > 0)
            {
                texto = String.Join(" ", propiedadesComerciales);
            }

            return texto;

        }

        public static double DistToSegment(double pX, double pY, double x1, double y1, double x2, double y2, out double near_x, out double near_y, out bool bIsLock)
        {
            double dx;
            double dy;
            double t;
            dx = x2 - x1;
            dy = y2 - y1;
            if (dx == 0 && dy == 0)
            {
                dx = pX - x1;
                dy = pY - y1;
                near_x = x1;
                near_y = y1;
                bIsLock = false;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            t = ((pX - x1) * dx + (pY - y1) * dy) / (dx * dx + dy * dy);
            if (t < 0)
            {
                dx = pX - x1;
                dy = pY - y1;
                near_x = x1;
                near_y = y1;
                bIsLock = false;
            }
            else if (t > 1)
            {
                dx = pX - x2;
                dy = pY - y2;
                near_x = x2;
                near_y = y2;
                bIsLock = false;
            }
            else
            {
                near_x = x1 + t * dx;
                near_y = y1 + t * dy;
                dx = pX - near_x;
                dy = pY - near_y;
                bIsLock = true;
            }
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private double GetAnguloRotacion(DbGeometry geometryLayerGraf, int anguloTolerancia, double distanciaTolerancia, ref PointF ptoMedio, ref List<Lado> lados, ref Lado ladoMayor)
        {
            double angulo = 0;
            //Angulo de tolerancia utilizado para determinar el cambio de lado. Esta determinado por la diferencia de angulo >= anguloTolerancia (10)
            //int anguloTolerancia = 10;
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
                                //Cambio de lado. Esta determinado por la diferencia de angulo >= 10 y distancia >= 5m
                                //if (Math.Abs(Math.Abs(Segmentos[i - 1].Angulo) - Math.Abs(Segmentos[i].Angulo)) < anguloTolerancia || Segmentos[i].Distancia < distanciaTolerancia)
                                if (Math.Abs(ladoAng - Segmentos[i].Angulo) < anguloTolerancia || Segmentos[i].Distancia < distanciaTolerancia)
                                {
                                    if (Segmentos[i].Distancia < distanciaTolerancia)
                                        ladoAng = Segmentos[i - 1].Angulo;
                                    else
                                        ladoAng = Segmentos[i].Angulo;
                                    ladoDist += Segmentos[i].Distancia;
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
                                    //lado.Angulo = ladoAng;
                                    //lados.Add(lado);
                                    lstLadosElem.Add(lado);
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
                            //lado.Angulo = ladoAng;
                            //lados.Add(lado);
                            lstLadosElem.Add(lado);
                        }
                        //Verificar si el primer lado y el ultimo pertenecen al mismo lado
                        if (lstLadosElem.Count > 0 && lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count > 0)
                        {
                            int cantSegmentosUltLado = lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count - 1;
                            //if (Math.Abs(Math.Abs(lstLadosElem[0].Segmentos[0].Angulo) - Math.Abs(lstLadosElem[lstLadosElem.Count - 1].Segmentos[cantSegmentosUltLado].Angulo)) < anguloTolerancia)
                            if (Math.Abs(lstLadosElem[0].Segmentos[0].Angulo - lstLadosElem[lstLadosElem.Count - 1].Segmentos[cantSegmentosUltLado].Angulo) < anguloTolerancia)
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
                                //lados = ladosTemp;
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
                                lado = new Lado();
                                lado.Segmentos = new List<Segmento>();
                                ladoDist = (Segmentos.Count > 0 ? Segmentos[0].Distancia : 0);
                                ladoAng = (Segmentos.Count > 0 ? Segmentos[0].Angulo : 0);
                                if (Segmentos.Count > 0)
                                {
                                    lado.Segmentos.Add(Segmentos[0]);
                                }
                                for (int i = 0; i < Segmentos.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        //Cambio de lado. Esta determinado por la diferencia de angulo >= 10 y distancia >= 5m
                                        //if (Math.Abs(Segmentos[i - 1].Angulo - Segmentos[i].Angulo) < anguloTolerancia || Segmentos[i].Distancia < distanciaTolerancia)
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
                                            //lado.Angulo = ladoAng;
                                            //lados.Add(lado);
                                            lstLadosElem.Add(lado);
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
                                    //lado.Angulo = ladoAng;
                                    //lados.Add(lado);
                                    lstLadosElem.Add(lado);
                                }
                                //Verificar si el primer lado y el ultimo pertenecen al mismo lado
                                if (lstLadosElem.Count > 0 && lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count > 0)
                                {
                                    int cantSegmentosUltLado = lstLadosElem[lstLadosElem.Count - 1].Segmentos.Count - 1;
                                    //if (Math.Abs(lstLadosElem[0].Segmentos[0].Angulo - lstLadosElem[lstLadosElem.Count - 1].Segmentos[cantSegmentosUltLado].Angulo) < anguloTolerancia)
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
                                        //lados = ladosTemp;
                                        lstLadosElem = ladosTemp;
                                    }
                                    lados.AddRange(lstLadosElem);
                                }
                            }

                        }
                        //Fin Interior Ring
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
                //angulo = ladoMayor.Angulo;
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

        private List<double> GetLimite(DbGeometry geometry, double toleranciaCambioVertice)
        {
            List<double> lstLimite = new List<double>();
            try
            {
                double x = 0, y = 0;
                double x1 = 0, y1 = 0;
                double x2 = 0, y2 = 0;
                string wkt = geometry.AsText();
                List<double> lstLimiteOrig = new List<double>();
                if (wkt.Contains("LINE") || wkt.Contains("POLYG"))
                {
                    int cantCoords = (int)geometry.PointCount;
                    for (int iCoord = 1; iCoord <= cantCoords; iCoord++)
                    {
                        x = (double)geometry.PointAt(iCoord).XCoordinate;
                        y = (double)geometry.PointAt(iCoord).YCoordinate;
                        lstLimiteOrig.Add(x);
                        lstLimiteOrig.Add(y);
                    }
                    double[] aLimiteOrig = lstLimiteOrig.ToArray();
                    //Obtengo el los lados (limite) empezando desde el primer corte
                    double x0 = 0, y0 = 0;
                    double angAnt = 99;
                    double ang = 0;
                    double angRad = 0;
                    double angRadAnt = 0;
                    //double vertices = 0;
                    int i = 0;
                    int iCorte = 0;
                    bool salir = false;
                    while (i < aLimiteOrig.Length && !salir)
                    {
                        x1 = aLimiteOrig[i];
                        i = i + 1;
                        y1 = aLimiteOrig[i];
                        i = i + 1;
                        x2 = aLimiteOrig[i];
                        i = i + 1;
                        y2 = aLimiteOrig[i];
                        if ((x2 - x1) == 0)
                        {
                            ang = 90;
                        }
                        else
                        {
                            ang = Math.Atan((y2 - y1) / (x2 - x1));
                            ang = ang * 180 / Math.PI;
                        }
                        if (i == 3)
                        {
                            angAnt = ang;
                            angRadAnt = Math.Atan((y2 - y1) / (x2 - x1));
                        }
                        if (Math.Abs(ang - angAnt) > toleranciaCambioVertice)
                        {
                            salir = true;
                            i = i - 3;
                            iCorte = i;
                            angAnt = ang;
                        }
                        x0 = x1;
                        y0 = y1;
                        if (i > aLimiteOrig.Length - 1)
                        {
                            salir = true;
                        }
                        i = i - 1;
                    }
                    i = iCorte;
                    int j = 1;
                    while (i < aLimiteOrig.Length)
                    {
                        //lstLimite[j] = aLimiteOrig[i];
                        lstLimite.Add(aLimiteOrig[i]);
                        i = i + 1;
                        j = j + 1;
                    }
                    i = 2;
                    while (i < iCorte + 2)
                    {
                        //a_Limite(j) = aLimiteOrig[i];
                        lstLimite.Add(aLimiteOrig[i]);
                        i = i + 1;
                        j = j + 1;
                    }

                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                lstLimite = null;
            }
            return lstLimite;
        }

        private void DibujarTextos(PdfContentByte pdfContentByte, string texto, double x, double y, double rotation)
        {
            if (texto != string.Empty)
            {
                int alignment = it.Element.ALIGN_CENTER + it.Element.ALIGN_MIDDLE;

                string etiquetaFuenteNombre = "Arial";
                double etiquetaFuenteTamanio = 0.5;
                string etiquetaFuenteEstilo = "0,0,0,0";
                int etiquetaMantieneOrientacion = 0;

                Color colorTexto = Color.Black;
                it.BaseColor pdfColorTexto = new it.BaseColor(colorTexto.R, colorTexto.G, colorTexto.B);

                float pdfFontSize = it.Utilities.MillimetersToPoints((float)etiquetaFuenteTamanio);

                PDFUtilities.RegisterBaseFont(etiquetaFuenteNombre, pdfFontSize);
                string[] aFontStylePdf = etiquetaFuenteEstilo.Split(',');
                //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
                int pdfFontStyle = aFontStylePdf.Select(a => Convert.ToInt32(a)).Sum();
                BaseFont pdfbaseFont = it.FontFactory.GetFont(etiquetaFuenteNombre, pdfFontSize, pdfFontStyle, pdfColorTexto).BaseFont;
                it.Font pdfFont = new it.Font(pdfbaseFont, pdfFontSize, pdfFontStyle, pdfColorTexto);

                float sizeText = pdfbaseFont.GetWidthPoint(texto, pdfFontSize);
                float xPDF = (float)it.Utilities.MillimetersToPoints((float)x);
                //if (plantillaTexto.FuenteAlineacion == 2)
                //{
                xPDF = xPDF - sizeText / 2;
                //}
                //else if (plantillaTexto.FuenteAlineacion == 3)
                //{
                //    xPDF = xPDF - sizeText;
                //}
                float yPDF = it.Utilities.MillimetersToPoints((float)(y));
                //PDFUtilities.DrawPDFText(pdfContentByte, layerGrafNombre, x1Pdf, y1Pdf, pdfbaseFont, pdfFontSize, etiquetaColor, anguloPdf);
                PDFUtilities.DrawPDFText(pdfContentByte, texto, xPDF, yPDF, pdfbaseFont, pdfFontSize, colorTexto, rotation);
            }

        }


        private int GetPixels(double x, int resolucionPPP)
        {
            int px = (int)(x * (resolucionPPP / 25.4));
            return px;
        }

        private float GetXPDFCanvas(double x, double xCentroidBase, double escala, Plantilla plantilla)
        {
            float xc = it.Utilities.MillimetersToPoints((float)(((x - xCentroidBase) * escala) * 1000 + plantilla.X_Centro));
            return xc;
        }

        private float GetYPDFCanvas(double y, double yCentroidBase, double escala, Plantilla plantilla)
        {
            //int yc = GetPixels(((((y - yCentroidBase) * escala * 1000) + plantilla.Y_Centro)), plantilla.PPP);
            float yc = it.Utilities.MillimetersToPoints((float)(((y - yCentroidBase) * escala * 1000) + plantilla.Y_Centro));
            return yc;
        }

        private float GetYPDFCanvas(double y, Plantilla plantilla)
        {
            float yc = (float)(plantilla.HeigthPts - it.Utilities.MillimetersToPoints((float)(y)));
            //float yc = (float)(it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMax) + it.Utilities.MillimetersToPoints((float)plantilla.ImpresionYMin) - it.Utilities.MillimetersToPoints((float)y));
            return yc;
        }

        public PointF Rotate(float xToRotate, float yToRotate, float xOrigen, float yOrigen, double angulo)
        {
            return Rotate(xToRotate, yToRotate, xOrigen, yOrigen, angulo, false);
        }
        public PointF Rotate(float xToRotate, float yToRotate, float xOrigen, float yOrigen, double angulo, bool sentidoHorario)
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
        public PointF Rotate(PointF toRotate, PointF origin, double angulo)
        {
            return Rotate(toRotate, origin, angulo, false);
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
                //slope = Math.Round(slope, 2);
                //trunco a dos decimale sin redondear
                double stepper = Math.Pow(10.0, (double)2);
                int temp = (int)(stepper * slope);
                slope = (double)(temp / stepper);

                i++;
            }
            //Para Graphics
            //if (longSidePoint1.Y < origin.Y && longSidePoint2.Y < origin.Y)
            //    i += 180;
            if (longSidePoint1.Y > origin.Y && longSidePoint2.Y > origin.Y)
                i += 180;

            return i * Math.PI / 180;
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

        public string prefijoId { get; set; }
    }

    public class PointD
    {
        public double X;
        public double Y;
        public PointD(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        //public PoinD()
        //{
        //}
    }
    public class Segmento
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
        public Segmento()
        {
        }
    }
    public class Lado
    {
        public List<Segmento> Segmentos;
        public double Distancia;
        public double Angulo;
        public long IdCuadra;
        public Lado(List<Segmento> Segmentos, double distancia, double angulo, long idCuadra)
        {
            this.Segmentos = Segmentos;
            this.Distancia = distancia;
            this.Angulo = angulo;
            this.IdCuadra = idCuadra;
        }
        public Lado()
        {
        }
    }
    public class CustomComparer : IComparer<ParcelaPlot>
    {
        public int Compare(ParcelaPlot x, ParcelaPlot y)
        {
            var regex = new Regex("^(d+)");

            // run the regex on both strings
            var xRegexResult = regex.Match(x.Expediente);
            var yRegexResult = regex.Match(y.Expediente);

            // check if they are both numbers
            if (xRegexResult.Success && yRegexResult.Success)
            {
                return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
            }

            // otherwise return as string comparison
            return x.Expediente.CompareTo(y.Expediente);
        }
    }
    public class SemiNumericComparer : IComparer<ParcelaPlot>
    {
        public int Compare(ParcelaPlot s1, ParcelaPlot s2)
        {
            if (IsNumeric(s1.Expediente) && IsNumeric(s2.Expediente))
            {
                if (Convert.ToInt32(s1.Expediente) > Convert.ToInt32(s2.Expediente)) return 1;
                if (Convert.ToInt32(s1.Expediente) < Convert.ToInt32(s2.Expediente)) return -1;
                if (Convert.ToInt32(s1.Expediente) == Convert.ToInt32(s2.Expediente)) return 0;
            }

            if (IsNumeric(s1.Expediente) && !IsNumeric(s2.Expediente))
            {
                if (s2.Expediente.Contains('#'))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            if (!IsNumeric(s1.Expediente) && IsNumeric(s2.Expediente))
            {
                if (s1.Expediente.Contains('#'))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            return string.Compare(s1.Expediente, s2.Expediente, true);
        }

        public static bool IsNumeric(object value)
        {
            try
            {
                int i = Convert.ToInt32(value.ToString());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    public class SemiNumericComparerExpansionPlot : IComparer<ExpansionPlot>
    {
        public int Compare(ExpansionPlot s1, ExpansionPlot s2)
        {
            if (s1.Id > s2.Id) return 1;
            if (s1.Id < s2.Id) return -1;
            if (s1.Id == s2.Id) return 0;
            return 1;
        }

    }

}