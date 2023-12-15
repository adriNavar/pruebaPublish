using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Web.Api.Ploteo;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Linq;
using System;
using System.Net.Http;
using System.Net;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.Ploteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    [RoutePrefix("api/moduloploteo")]
    public class ModuloPloteoController : ApiController
    {
        private readonly ModPlot _modPlot;
        private readonly ModPlotGraphics _modPlotGraphics;
        private const string PLANTILLA_INCOMPLETA = "Error en plantilla.  Uno o más campos requeridos no contienen entradas.";
        private const string PLANTILLA_ERROR = "Ha ocurrido un error. Verifique el log de errores para más información.";

        public ModuloPloteoController(UnitOfWork unitOfWork)
        {
            _modPlot = new ModPlot(unitOfWork.PlantillaRepository, unitOfWork.LayerGrafRepository, unitOfWork.PlantillaFondoRepository, unitOfWork.HojaRepository, unitOfWork.NorteRepository, unitOfWork.ParcelaPlotRepository, unitOfWork.CuadraPlotRepository, unitOfWork.ManzanaPlotRepository, unitOfWork.CallePlotRepository, unitOfWork.ParametroRepository, unitOfWork.ImagenSatelitalRepository, unitOfWork.ExpansionPlotRepository, unitOfWork.TipoPlanoRepository, unitOfWork.PartidoRepository, unitOfWork.CensoRepository,
                unitOfWork.PloteoFrecuenteRepository, unitOfWork.PloteoFrecuenteEspecialRepository, unitOfWork.PlantillaViewportRepository, unitOfWork.TipoViewportRepository, unitOfWork.LayerViewportReposirory, unitOfWork.AtributoRepository, unitOfWork.ComponenteRepository);
            _modPlotGraphics = new ModPlotGraphics(unitOfWork.PlantillaRepository, unitOfWork.LayerGrafRepository, unitOfWork.PlantillaFondoRepository, unitOfWork.HojaRepository, unitOfWork.NorteRepository, unitOfWork.ParcelaPlotRepository);
        }

        // GET api/modplot/GetPlantilla
        [HttpGet]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GetPlantilla(int id, string idObjetoGraf, int idComponenteObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, bool verCotas, int idImagenSatelital, float imagenTransparencia, string grafico = null, string leyenda = null, int? infoLeyenda = null, /*string textosParametros = ""*/string numeroODT = "0", string tipo = null, string relacion = null, string direccionCompleta = null, string datosReferencia = null)//, string oDS = null, string expediente = null, string observaciones = null
        {//Sive para los casos que genera 1 solo archivo.
            try
            {

                InformacionComercial graficoObj = null;
                InformacionComercial leyendaObj = null;
                try
                {
                    graficoObj = JsonConvert.DeserializeObject<InformacionComercial>(grafico);
                    leyendaObj = JsonConvert.DeserializeObject<InformacionComercial>(leyenda);
                }
                catch (Exception) { }

                return _modPlot.GetPlantilla(id, idObjetoGraf, idComponenteObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, graficoObj, leyendaObj, infoLeyenda, /*textosParametros*/numeroODT, tipo, relacion, direccionCompleta, datosReferencia);
            }
            catch (ArgumentNullException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError, PLANTILLA_INCOMPLETA));
            }

        }



        [HttpPost]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GenerarPloteoPDFManzanasUOtros(int id, string[] idsObjetosGraf, int idComponenteObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, bool verCotas, int idImagenSatelital, float imagenTransparencia, string grafico = null, string leyenda = null, int? infoLeyenda = null)
        {//Una pagina por manzana
            try
            {
                InformacionComercial graficoObj = null;
                InformacionComercial leyendaObj = null;
                try
                {
                    graficoObj = JsonConvert.DeserializeObject<InformacionComercial>(grafico);
                    leyendaObj = JsonConvert.DeserializeObject<InformacionComercial>(leyenda);
                }
                catch (Exception) { }

                byte[] pdfResult = null;

                //
                string extent = string.Empty;
                string scale = string.Empty;
                string layersVisibles = string.Empty;
                bool verIdentificante = true;
                long? idComponentePrincipal = null;
                bool verContexto = false;
                bool esInformeAnual = false;
                string anio = string.Empty;

                string numeroODT = null;
                string tipo = null;
                string relacion = null;
                string direccionCompleta = null;
                string datosReferencia = null;
                //
                foreach (var idObjGraf in idsObjetosGraf)
                {
                    pdfResult = _modPlot.GetPlantilla(id, idObjGraf, idComponenteObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables, extent, scale, layersVisibles, verCotas, null, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, graficoObj, leyendaObj, infoLeyenda, esInformeAnual, anio, /*textosParametros */numeroODT, tipo, relacion, direccionCompleta, datosReferencia, null, pdfResult, null);//, oDS, expediente, observaciones

                }

                return pdfResult;
            }
            catch (ArgumentNullException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError, PLANTILLA_INCOMPLETA));
            }

        }

        //"id={0}&dicParcelasGraficar={1}&idComponenteObjetoGraf={2}&idPlantillaFondo={3}&textosVariables={5}&verCotas={6}&idImagenSatelital={7}&imagenTransparencia={8}&grafico={9}&leyenda={10}&infoLeyenda={11}",

        [HttpPost]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GenerarPloteoPDFParcelas(int id, List<KeyValuePair<long, string>> dicParcelasGraficar, int idComponenteObjetoGraf, int idPlantillaFondo, string textosVariables, bool verCotas, int idImagenSatelital, float imagenTransparencia, string grafico = null, string leyenda = null, int? infoLeyenda = null)
        {
            try
            {
                InformacionComercial graficoObj = null;
                InformacionComercial leyendaObj = null;
                try
                {
                    graficoObj = JsonConvert.DeserializeObject<InformacionComercial>(grafico);
                    leyendaObj = JsonConvert.DeserializeObject<InformacionComercial>(leyenda);
                }
                catch (Exception) { }

                byte[] pdfResult = null;

                //
                string extent = string.Empty;
                string scale = string.Empty;
                string layersVisibles = string.Empty;
                bool verIdentificante = true;
                long? idComponentePrincipal = null;
                bool verContexto = false;
                bool esInformeAnual = false;
                string anio = string.Empty;

                string numeroODT = null;
                string tipo = null;
                string relacion = null;
                string direccionCompleta = null;
                string datosReferencia = null;
                //
                foreach (var obj in dicParcelasGraficar)
                {
                    pdfResult = _modPlot.GetPlantilla(id, obj.Key.ToString(), idComponenteObjetoGraf, idPlantillaFondo, obj.Value.ToString(), textosVariables, extent, scale, layersVisibles, verCotas, null, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, graficoObj, leyendaObj, infoLeyenda, esInformeAnual, anio, /*textosParametros */numeroODT, tipo, relacion, direccionCompleta, datosReferencia, null, pdfResult, null);
                }

                return pdfResult;
            }
            catch (ArgumentNullException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError, PLANTILLA_INCOMPLETA));
            }
        }


        // GET api/modplot/GetPlantillaDoc
        [HttpPost]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GetPlantillaDoc(int id, int idComponenteObjetoGraf, int idPlantillaFondo, string textosVariables, bool verCotas, int idImagenSatelital, float imagenTransparencia, List<ObjetoPloteable> listaObjetosAplotear, string grafico = null, string leyenda = null, int? infoLeyenda = null /*string textosParametros = ""*/)//, string oDS = null, string expediente = null, string observaciones = null
        {
            try
            {
                string numeroODT = "0";
                string tipo = null;
                string relacion = null;
                string direccionCompleta = null;
                string datosReferencia = null;

                InformacionComercial graficoObj = null;
                InformacionComercial leyendaObj = null;
                try
                {
                    graficoObj = JsonConvert.DeserializeObject<InformacionComercial>(grafico);
                    leyendaObj = JsonConvert.DeserializeObject<InformacionComercial>(leyenda);
                }
                catch (Exception) { }
                byte[] bPdfCompleto = { };
                foreach (var pp in listaObjetosAplotear)
                {
                    string parce = String.IsNullOrEmpty(pp.idParcela) ? "" : pp.idParcela.ToString();
                    bPdfCompleto = _modPlot.GetPlantilla(id, pp.idManzana.ToString(), idComponenteObjetoGraf, idPlantillaFondo, parce, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, graficoObj, leyendaObj, infoLeyenda, /*textosParametros*/numeroODT, tipo, relacion, direccionCompleta, datosReferencia, bPdfCompleto);

                    /*if(pdfResult != null && pdfResult.Count() != 0)
                    {
                        if (bPdfCompleto == null || bPdfCompleto.Count() == 0)
                        {
                            bPdfCompleto = pdfResult;
                        }
                        else
                        {
                            using
                        }
                    }*/
                }
                return bPdfCompleto;
            }
            catch (ArgumentNullException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError, PLANTILLA_INCOMPLETA));
            }

        }



        // GET api/modplot/GetPlantillaObra
        [HttpPost]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GetPlantillaObra(int idPlantilla, int idPlantillaFondo, int idComponenteObjetoGraf, int[] lstIds, string idAtributosTexto)
        {
            try
            {
                string numeroODT = "0";
                string tipo = null;
                string relacion = null;
                string direccionCompleta = null;
                string datosReferencia = null;

                InformacionComercial graficoObj = null;
                InformacionComercial leyendaObj = null;


                int? infoLeyenda = null;
                bool verCotas = false;
                int idImagenSatelital = 0;
                float imagenTransparencia = 0;
                List<ObjetoPloteable> listaObjetosAplotear;
                string grafico = null;
                string leyenda = null;
                string textosVariables = null;
                string idsObjetosSecundarios = null;

                byte[] bPdfCompleto = { };
                foreach (var pp in lstIds)
                {
                    //bPdfCompleto = _modPlot.GetPlantilla(idPlantilla, pp.idManzana.ToString(), idComponenteObjetoGraf, idPlantillaFondo, parce, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, graficoObj, leyendaObj, infoLeyenda, /*textosParametros*/numeroODT, tipo, relacion, direccionCompleta, datosReferencia, bPdfCompleto);
                    bPdfCompleto = _modPlot.GetPlantilla(idPlantilla, pp.ToString(), idComponenteObjetoGraf, idPlantillaFondo, idsObjetosSecundarios, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, graficoObj, leyendaObj, infoLeyenda, /*textosParametros*/numeroODT, tipo, relacion, direccionCompleta, datosReferencia, bPdfCompleto, idAtributosTexto);
                }
                return bPdfCompleto;
            }
            catch (ArgumentNullException e)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError, PLANTILLA_INCOMPLETA));
            }

        }


        [HttpGet]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GetPlantillaVistaActual(int id, int idPlantillaFondo, string textosVariables, string extent, string scale, string layersVisibles, int idImagenSatelital, float imagenTransparencia)
        {
            try
            {
                return _modPlot.GetPlantilla(id, idPlantillaFondo, textosVariables, extent, scale, layersVisibles, idImagenSatelital, imagenTransparencia);
            }
            catch (NullReferenceException)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Conflict, PLANTILLA_INCOMPLETA));
            }
            catch (Exception)
            {
                throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError, PLANTILLA_ERROR));
            }

        }

        [HttpPost]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GetPlantillaMapaTematico(int idPlantilla, int idPlantillaFondo, string extent, string textosVariables, string idDistrito, int idImagenSatelital, float imagenTransparencia, bool verIdentificante, long? idComponentePrincipal, bool verContexto, ObjetoResultadoDetalle objetoResultadoDetalle, long? idComponenteTematico)
        {
            int idComponenteDistrito = 0;
            if (!string.IsNullOrEmpty(idDistrito))
            {
                ParametrosGenerales parametroGeneral = GeoSITMContext.CreateContext().ParametrosGenerales.FirstOrDefault(p => p.Descripcion.ToUpper() == "ID_COMPONENTE_DISTRITO");
                if (parametroGeneral != null)
                {
                    idComponenteDistrito = Convert.ToInt32(parametroGeneral.Valor);
                }
            }
            return _modPlot.GetPlantilla(idPlantilla, idPlantillaFondo, extent, textosVariables, idDistrito, idComponenteDistrito, objetoResultadoDetalle, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, idComponenteTematico);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GetPlantillaInformeAnual(int idPlantilla, int idPlantillaFondo, int idPartido, string anio, string textosVariables, int idImagenSatelital, float imagenTransparencia)
        {
            int idComponentePartido = 0;
            if (idPartido > 0)
            {
                ParametrosGenerales parametroGeneral = GeoSITMContext.CreateContext().ParametrosGenerales.FirstOrDefault(p => p.Descripcion.ToUpper() == "ID_COMPONENTE_PARTIDO");
                if (parametroGeneral != null)
                {
                    idComponentePartido = Convert.ToInt32(parametroGeneral.Valor);
                }
            }
            return _modPlot.GetPlantilla(idPlantilla, idPlantillaFondo, idPartido, anio, idComponentePartido, textosVariables, idImagenSatelital, imagenTransparencia);
        }

        [Route("plantillaimagen")]
        [ResponseType(typeof(ICollection<byte>))]
        public byte[] GetPlantillaImagen(int id, string idObjetoGraf, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables)
        {
            return _modPlotGraphics.GetPlantilla(id, idObjetoGraf, idPlantillaFondo, idsObjetoSecundario, textosVariables);
        }
    }
}
