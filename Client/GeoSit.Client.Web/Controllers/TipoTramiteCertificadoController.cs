using System;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Xml;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using System.Linq;
using System.Net;
using System.Net.Mime;

namespace GeoSit.Client.Web.Controllers
{
    public class TipoTramiteCertificadoController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private UsuariosModel Usuario { get { return ((UsuariosModel)Session["usuarioPortal"]); } }
        public TipoTramiteCertificadoController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }


        // GET: TipoTramite
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: /TipoTramite/DatosTipoTramite
        [ValidateInput(false)]
        public ActionResult DatosTipoTramite()
        {

            //ViewBag.DatosTipoTramite = GetDatosTipoDocumento();

            return PartialView();
        }

        public JsonResult GetTiposTramites()
        {
            return Json(GetDatosTipoDocumento());
        }

        public List<TipoTramiteModel> GetDatosTipoDocumento()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoTramiteService/GetTiposTramite").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoTramiteModel>)resp.Content.ReadAsAsync<IEnumerable<TipoTramiteModel>>().Result;
        }

        public JsonResult GetTipoTramiteById(long id)
        {
            return Json(GetTipoTramite(id));
        }
        public TipoTramiteModel GetTipoTramite(long idTipoTramite)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoTramiteService/GetTipoTramiteById/" + idTipoTramite).Result;
            resp.EnsureSuccessStatusCode();
            return (TipoTramiteModel)resp.Content.ReadAsAsync<TipoTramiteModel>().Result;
        }

        public JsonResult GetTiposSeccionByIdTramite(long id)
        {
            return Json(GetTiposSeccion(id));
        }

        public List<TramiteTipoSeccion> GetTiposSeccion(long idTipoTramite)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoSeccionService/GetTipoSeccionByIdTramite/" + idTipoTramite).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<TramiteTipoSeccion>>().Result;
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Save_DatosTramite(TipoTramite Save_Datos, List<string> TxtIdTipoSeccion, List<string> TxtNombreInformeSeccion, List<string> TxtTextoInformeSeccion, List<string> TxtConfiguracionImpresionPorDefecto)
        {
            InitializeAuditData(ref Save_Datos);
            var tramite = Save_Datos.Id_Tipo_Tramite;
            Save_Datos.Fecha_Modif = DateTime.Now;
            Save_Datos.Id_Usu_Modif = Usuario.Id_Usuario;
            Save_Datos.Id_Usu_Alta = Usuario.Id_Usuario;
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/TipoTramiteService/SetTipoTramite_Save", Save_Datos).Result;
            resp.EnsureSuccessStatusCode();
            ViewBag.MensajeSalida = resp.StatusCode.ToString();
            if (ViewBag.MensajeSalida == "OK")
                if (Save_Datos.Id_Tipo_Tramite == 0)
                {
                    ViewBag.MensajeSalida = "AltaOK";
                    try
                    {
                        Save_Datos.Fecha_Alta = Save_Datos.Fecha_Modif;
                        Save_Datos.Id_Usu_Alta = Save_Datos.Id_Usu_Modif;
                        HttpResponseMessage respGet = cliente.GetAsync(string.Format("api/TipoTramiteService/GetTipoTramiteByNombre?id={0}", Save_Datos.Nombre)).Result;
                        respGet.EnsureSuccessStatusCode();
                        var tra = respGet.Content.ReadAsAsync<TipoTramite>().Result;
                        Save_Datos.Id_Tipo_Tramite = tra.Id_Tipo_Tramite;
                    }
                    catch
                    {
                        Save_Datos.Id_Tipo_Tramite = 0;
                    }
                }
                else
                    ViewBag.MensajeSalida = "ModificacionOK";
            else
                ViewBag.MensajeSalida = "Error";


            long[] vectorDom;
            if (TxtIdTipoSeccion != null)
            {
                vectorDom = new long[TxtIdTipoSeccion.Count];
                for (int i = 0; i < TxtIdTipoSeccion.Count; i++)
                {
                    vectorDom[i] = Convert.ToInt64(TxtIdTipoSeccion[i]);
                }
            }
            else
                vectorDom = new long[0];


            var listSecciones = new List<TramiteTipoSeccion>();
            listSecciones = GetTiposSeccion(Save_Datos.Id_Tipo_Tramite);
            foreach (var element in listSecciones)
            {
                if (Array.IndexOf(vectorDom, element.Id_Tipo_Seccion) == -1)
                {
                    var RelacionSeccion = new TramiteTipoSeccion();
                    InitializeAuditData(ref RelacionSeccion);
                    RelacionSeccion.Id_Tipo_Tramite = Save_Datos.Id_Tipo_Tramite;
                    RelacionSeccion.Id_Tipo_Seccion = element.Id_Tipo_Seccion;
                    RelacionSeccion.Fecha_Alta = element.Fecha_Alta;
                    RelacionSeccion.Fecha_Modif = element.Fecha_Modif;
                    RelacionSeccion.Id_Usu_Alta = element.Id_Usu_Alta;
                    RelacionSeccion.Id_Usu_Modif = element.Id_Usu_Modif;
                    RelacionSeccion.Imprime = element.Imprime;
                    RelacionSeccion.Nombre = element.Nombre;
                    RelacionSeccion.Plantilla = element.Plantilla;
                    // Información de baja.
                    RelacionSeccion.Id_Usu_Baja = Save_Datos.Id_Usu_Modif;
                    RelacionSeccion.Fecha_Baja = Save_Datos.Fecha_Modif;

                    resp = cliente.PostAsJsonAsync("api/TipoSeccionService/SetTipoSeccion_Save", RelacionSeccion).Result;
                    resp.EnsureSuccessStatusCode();
                }
            };

            //long[] vectorDom;
            if (TxtIdTipoSeccion != null)
            {
                //vectorDom = new long[TxtIdTipoSeccion.Count];
                for (int i = 0; i < TxtIdTipoSeccion.Count; i++)
                {
                    var seccion = new TramiteTipoSeccion();
                    InitializeAuditData(ref seccion);
                    seccion.Id_Tipo_Seccion = Convert.ToInt64(TxtIdTipoSeccion[i]);
                    seccion.Id_Tipo_Tramite = Save_Datos.Id_Tipo_Tramite;
                    if (TxtConfiguracionImpresionPorDefecto[i] != "null")
                        seccion.Imprime = Convert.ToBoolean(Convert.ToInt32(TxtConfiguracionImpresionPorDefecto[i]));

                    seccion.Nombre = TxtNombreInformeSeccion[i];
                    seccion.Plantilla = TxtTextoInformeSeccion[i];
                    seccion.Id_Usu_Alta = Save_Datos.Id_Usu_Alta;
                    seccion.Fecha_Alta = Save_Datos.Fecha_Alta;
                    seccion.Id_Usu_Baja = Save_Datos.Id_Usu_Baja;
                    seccion.Id_Usu_Modif = Save_Datos.Id_Usu_Modif;
                    seccion.Fecha_Baja = Save_Datos.Fecha_Baja;
                    seccion.Fecha_Modif = Save_Datos.Fecha_Modif;
                    resp = cliente.PostAsJsonAsync("api/TipoSeccionService/SetTipoSeccion_Save", seccion).Result;
                    resp.EnsureSuccessStatusCode();

                    //vectorDom[i] = seccion.Id_Tipo_Seccion;
                }
            }
            //else
            //    vectorDom = new long[0];

            return PartialView("DatosTipoTramite");
        }

        public JsonResult DeleteTipoTramite(long id)
        {
            return Json(DeleteTipoTramiteById(id, string.Empty, 0));
        }

        public string DeleteTipoTramiteById(long id, string fecha_baja, long usuario_baja)
        {
            TipoTramiteModel tra = new TipoTramiteModel();
            tra.Id_Tipo_Tramite = id;
            tra.Fecha_Modif = DateTime.Now;
            tra.Id_Usu_Modif = Usuario.Id_Usuario;
            tra.Fecha_Baja = tra.Fecha_Modif;
            tra.Id_Usu_Baja = tra.Id_Usu_Modif;
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/TipoTramiteService/DeleteTipoTramite_Save", tra).Result;
            resp.EnsureSuccessStatusCode();
            return resp.StatusCode.ToString();
        }

        public string GetCantSeccionesByIdTipoTramite(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoSeccionService/GetTipoSeccionByIdTramite/" + id).Result;
            resp.EnsureSuccessStatusCode();
            List<TramiteTipoSeccion> Secciones = resp.Content.ReadAsAsync<List<TramiteTipoSeccion>>().Result;
            var CantidadSecciones = Secciones.Count;
            return CantidadSecciones.ToString();
        }
        public List<Tramite> GetObjetosTramitesCertificadosByCriteria(long? pTipoId, long? pNumDesde, long? pNumHasta, string pFechaDesde, string pFechaHasta, int? pEstadoId, int? pUnidadT)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesCertificadosByCriteria?pTipoId=" + pTipoId + "&pNumDesde=" + pNumDesde + "&pNumHasta=" + pNumHasta + "&pFechaDesde=" + pFechaDesde + "&pFechaHasta=" + pFechaHasta + "&pEstadoId=" + pEstadoId + "&pUnidadT=" + pUnidadT + "&pIdTramite" + null + "&pIdentificador" + "").Result;
            resp.EnsureSuccessStatusCode();
            List<Tramite> lstObjeto = resp.Content.ReadAsAsync<IEnumerable<Tramite>>().Result.ToList();

            return lstObjeto;
        }
        public JsonResult ObjetosTramitesCertificadosDelete(long? pTipoId)
        {

            List<Tramite> resultadoBusqueda = GetObjetosTramitesCertificadosByCriteria(pTipoId, null, null, null, null, null, null);
            if(resultadoBusqueda != null && resultadoBusqueda.Count() > 0){
                
                resultadoBusqueda = resultadoBusqueda.Where( x => x.Estado != "4").ToList();

                return Json(resultadoBusqueda.Count() > 0,JsonRequestBehavior.AllowGet);
            }
            return Json(false,JsonRequestBehavior.AllowGet);
        }

        private void InitializeAuditData(ref TipoTramite datos)
        {
            datos._Ip = Request.UserHostAddress;
            try
            {
                datos._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                datos._Machine_Name = Request.UserHostName;
            }
        }

        private void InitializeAuditData(ref TramiteTipoSeccion datos)
        {
            datos._Ip = Request.UserHostAddress;
            try
            {
                datos._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                datos._Machine_Name = Request.UserHostName;
            }
        }

        private ArchivoDescarga ArchivoDescarga
        {
            get { return Session["ArchivoDescarga"] as ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }
        public FileResult AbrirReporte()
        {
            Response.AppendHeader("Content-Disposition", new ContentDisposition { FileName = ArchivoDescarga.NombreArchivo, Inline = true }.ToString());
            return File(ArchivoDescarga.Contenido, ArchivoDescarga.MimeType);
        }

        public ActionResult GenerarInformeAdjudicacion(long idTramite)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
            using (var resp = apiReportes.GetAsync($"api/InformeAdjudicacion/Get?idTramite={idTramite}&usuario={usuario}").Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;

                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformeAdjudicacion_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }
    }
}