using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Net;

namespace GeoSit.Client.Web.Controllers
{
    public class AnalisisPredefinidoController : Controller
    {
        private const string ApiUri = "api/analisispredefinido/";
        // GET: AnalisisPredefinido
        public ActionResult Index()
        {
            return PartialView("~/Views/AnalisisPredefinido/AnalisisPredefinido.cshtml");
        }

        public ActionResult GetFiltros()
        {
            long componenteParcela = Convert.ToInt64(new SeguridadController().GetParametrosGenerales().Single(p => p.Descripcion == "ID_COMPONENTE_PARCELA").Valor);

            ViewBag.Colecciones = GetDistritos().OrderBy(m => m.Nombre).ToList();

            ViewBag.Categorias = GetColeccionesByUsuarioLogueadoColeccionA4(((UsuariosModel)Session["usuarioPortal"]).Id_Usuario)
                                    .Where(col => col.FechaBaja == null &&
                                                  col.UsuarioBaja == null &&
                                                  col.Componentes.Any(cmp => cmp.ComponenteId == componenteParcela))
                                    .ToList();
            return PartialView("~/Views/AnalisisPredefinido/Partial/_Filtros.cshtml");
        }

        public ActionResult GetCargas()
        {
            ViewBag.Colecciones = GetDistritos().OrderBy(m => m.Nombre).ToList();
            ViewBag.Categorias = GetColeccionesByUsuarioLogueadoColeccionA4(((UsuariosModel)Session["usuarioPortal"]).Id_Usuario)
                                    .Where(p => p.FechaBaja == null && p.UsuarioBaja == null)
                                    .ToList();
            return PartialView("~/Views/AnalisisPredefinido/Partial/_Cargas.cshtml");
        }

        public ActionResult GetReporteSistematico()
        {
            return PartialView("~/Views/AnalisisPredefinido/Partial/_ReporteSistematico.cshtml");
        }

        public ActionResult CargarUltimaCarga(int id)
        {
            return new JsonResult { Data = "Ok" };
        }

        public ActionResult GetReporteZonasLavado()
        {
            return PartialView("~/Views/AnalisisPredefinido/Partial/_ReporteZonasLavado.cshtml");
        }

        public ActionResult GetReporteFaltaAguaPresion()
        {
            return PartialView("~/Views/AnalisisPredefinido/Partial/_ReporteFaltaAguaPresion.cshtml");
        }

        public ActionResult GetReporteAnalisisLibre()
        {
            return PartialView("~/Views/AnalisisPredefinido/Partial/_ReporteAnalisisLibre.cshtml");
        }

        private List<Coleccion> GetColeccionesByUsuarioLogueadoColeccionA4(long UserId)
        {
            using (var _cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials })
            { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                HttpResponseMessage respColecciones = _cliente.GetAsync($"api/PloteoService/GetColeccionByCurrentUserColeccionA4?UsuarioId={UserId}").Result;
                respColecciones.EnsureSuccessStatusCode();
                return respColecciones.Content.ReadAsAsync<List<Coleccion>>().Result;
            }
        }

        public ActionResult CargaDeReclamos(long analisisID, string distritoElegidoID, long cargaTipo, string fechaDesdeInput, string fechaHastaInput, long usuarioID, string fechaDeAlta)
        {
            // DateTime?  fechaDInput = string.IsNullOrEmpty(fechaDesdeInput) ? (DateTime?)null : DateTime.Parse(fechaDesdeInput);
            //  DateTime? fechaHInput = string.IsNullOrEmpty(fechaHastaInput) ? (DateTime?)null : DateTime.Parse(fechaHastaInput);
            //  DateTime? fechAlt = string.IsNullOrEmpty(fechaDeAlta) ? (DateTime?)null : DateTime.Parse(fechaDeAlta);

            //var analisisTecnico = new AnalisisTecnicos();
            //var reclamoTecnico = new CargasTecnicas();
            //reclamoTecnico.Id_Carga_Tecnica = 7;
            //reclamoTecnico.Id_Analisis=analisisID;       
            //reclamoTecnico.Id_Distrito=distritoElegidoID;
            //reclamoTecnico.Tipo_Carga=cargaTipo;
            //reclamoTecnico.Fecha_Desde=fechaDInput;
            //reclamoTecnico.Fecha_Hasta=fechaHInput;
            //reclamoTecnico.Usuario_Alta=usuarioID;
            //reclamoTecnico.Fecha_Alta = fechAlt;

            //_cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            //HttpResponseMessage resp = _cliente.PostAsync<CargasTecnicas>("api/AnalisisPredefinidoService/CargarReclamos", reclamoTecnico, new JsonMediaTypeFormatter()).Result;
            //resp.EnsureSuccessStatusCode();

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult ProcesarTipoReporte(int id)
        {
            return new JsonResult { Data = "Ok" };
        }

        public List<Distritos> GetDistritos()
        {
            using (var _cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials })
                                    { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                HttpResponseMessage resp = _cliente.GetAsync("api/AnalisisPredefinidoService/GetDistritos").Result;
                resp.EnsureSuccessStatusCode();
                return (List<Distritos>)resp.Content.ReadAsAsync<IEnumerable<Distritos>>().Result;
            }
        }
    }
}