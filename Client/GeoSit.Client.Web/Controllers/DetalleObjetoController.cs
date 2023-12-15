using GeoSit.Client.Web.Helpers;
using GeoSit.Client.Web.Models;
//using GeoSit.Data.BusinessEntities.Validaciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Controllers
{
    public class DetalleObjetoController : Controller
    {
        private readonly int TIPO_PUNTO = 3;
        private FileResult FileToDownload { get { return Session["file_to_download"] as FileResult; } set { Session["file_to_download"] = value; } }
        private readonly HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials });
        public DetalleObjetoController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }
        public ActionResult GetDetalleObjeto(string objetoId, long componenteId)
        {
            return Json(new { data = this.getObjeto(objetoId, componenteId), imgData = this.GetImagenObjeto(objetoId, componenteId) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDetalleObjetoByDocType(long objetoId, string docType)
        {
            var comp = GetComponenteByDocType(docType);
            if (comp == null)
            {
                MvcApplication.GetLogger().LogInfo(string.Format("no existe ningun componente con el docType -> {0}", docType));
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
            return RedirectToAction("GetDetalleObjeto", new { objetoId, componenteId = comp.ComponenteId });
        }
        public ActionResult GetDetalleObjetoByTable(string objetoId, string table)
        {
            var comp = GetComponenteByTable(table);
            if (comp == null)
            {
                MvcApplication.GetLogger().LogInfo(string.Format("no existe ningun componente con la tabla -> {0}", table));
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
            return RedirectToAction("GetDetalleObjeto", new { objetoId, componenteId = comp.ComponenteId });
        }
        public ActionResult EditarParametros(string idObjeto, string docType)
        {
            var obj = getObjetoEdicion(idObjeto, docType);
            bloquearObjeto(idObjeto, docType);
            ViewBag.DocType = docType;
            ViewBag.ObjetoId = idObjeto;
            ViewBag.Atributos = obj.Atributos.Where(a => a.EsEditable).ToList();
            ViewBag.CambioDireccion = docType == "ccaneris";
            return PartialView();
        }
        [HttpPost]
        public ActionResult ExportarObjetoExcel(ObjetoExcel[] objetos)
        {
            FileToDownload = File(new ExportadorObjetosExcel(objetos).Exportar("Objetos"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportacionObjetos.xlsx");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [HttpGet]
        public FileResult DownloadFile()
        {
            return FileToDownload;
        }

        /*public ActionResult ValidarObjeto()
        {
            string objetoId = Request.Form["objetoId"];
            string docType = Request.Form["docType"];
            desbloquearObjeto(objetoId, docType);

            Dictionary<long, string> atributos = new Dictionary<long, string>();
            foreach (var key in Request.Form.Keys)
            {
                if (!key.Equals("objetoId") && !key.Equals("docType"))
                {
                    var val = Request.Form[key.ToString()];

                    atributos.Add(long.Parse(key.ToString()), val);
                }
            }
            ValidacionResult validRetorno = null;
            try
            {
                HttpResponseMessage result = _cliente.PostAsync(
                                            string.Format(
                                                "api/DetalleObjeto/ValidarObjeto?objetoId={0}&docType={1}&usuarioAuditoria={2}"
                                                , objetoId
                                                , docType
                                                , ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario)
                                                , atributos
                                               , new JsonMediaTypeFormatter()).Result;
                result.EnsureSuccessStatusCode();
                validRetorno = (ValidacionResult)result.Content.ReadAsAsync<ValidacionResult>().Result;
            }
            catch (Exception e)
            {
                MvcApplication.GetLogger().LogInfo(string.Format("no existe ningun componente con el docType -> {0}", docType));
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed, "El componente no está bien configurado.");
            }


            return Json(new { data = validRetorno }, JsonRequestBehavior.AllowGet);
        }*/

        public ActionResult SaveEditarParametros()
        {
            string objetoId = Request.Form["objetoId"];
            string docType = Request.Form["docType"];

            desbloquearObjeto(objetoId, docType);

            Dictionary<long, string> atributos = new Dictionary<long, string>();
            foreach (var key in Request.Form.Keys)
            {
                if (!key.Equals("objetoId") && !key.Equals("docType"))
                {
                    var val = Request.Form[key.ToString()];

                    atributos.Add(long.Parse(key.ToString()), val);
                }
            }

            saveParametros(objetoId, docType, atributos);

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LiberarObjeto(string idObjeto, string docType)
        {
            var comp = GetComponenteByDocType(docType);
            desbloquearObjeto(idObjeto, docType);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CambiarSentido(long objetoId, string docType)
        {
            var comp = GetComponenteByDocType(docType);
            var user = ((UsuariosModel)Session["usuarioPortal"]);
            var result = _cliente.GetAsync(string.Format("api/DetalleObjeto/CambiarSentido?objetoId={0}&componenteId={1}&usuarioAuditoria={2}", objetoId, comp.ComponenteId, user.Id_Usuario)).Result;
            result.EnsureSuccessStatusCode();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        private void bloquearObjeto(string objetoId, string docType)
        {
            var result = _cliente.GetAsync(string.Format("api/DetalleObjeto/Bloquear?objetoId={0}&docType={1}&usuarioAuditoria={2}", objetoId, docType, ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario)).Result;
            result.EnsureSuccessStatusCode();
        }

        private void saveParametros(string objetoId, string docType, Dictionary<long, string> atributos)
        {
            try
            {
                var result = _cliente.PostAsync(string.Format("api/DetalleObjeto/SetAtributos?objetoId={0}&docType={1}&usuarioAuditoria={2}", objetoId, docType, ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario), atributos, new JsonMediaTypeFormatter()).Result;
                result.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
            }
        }

        private void desbloquearObjeto(string objetoId, string docType)
        {
            var result = _cliente.GetAsync(string.Format("api/DetalleObjeto/Desbloquear?objetoId={0}&docType={1}&usuarioAuditoria={2}", objetoId, docType, ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario)).Result;
            result.EnsureSuccessStatusCode();
        }

        private Objeto getObjetoEdicion(string objetoId, string docType)
        {
            var result = _cliente.GetAsync(string.Format("api/DetalleObjeto/GetEdicion?objetoId={0}&docType={1}", objetoId, docType)).Result;
            result.EnsureSuccessStatusCode();

            return result.Content.ReadAsAsync<Objeto>().Result;
        }

        private Objeto getObjeto(string objetoId, long componenteId)
        {
            var result = _cliente.GetAsync(string.Format("api/DetalleObjeto/Get?objetoId={0}&componenteId={1}", objetoId, componenteId)).Result;
            result.EnsureSuccessStatusCode();

            var model = result.Content.ReadAsAsync<Objeto>().Result;

            var incluido = new Func<Grafico, bool>(g => g.TipoGrafico != TIPO_PUNTO || g.Nombre != "Cant. Gráficos");
            if (model.Graficos.Count(k => k.TipoGrafico == TIPO_PUNTO) > 3)
            {
                incluido = new Func<Grafico, bool>(g => g.Nombre != "X" && g.Nombre != "Y");
            }

            var lista = model.Graficos
                             .Where(k => incluido(k))
                             .GroupBy(g => new { Tipo = g.TipoGrafico, g.Nombre })
                             .Select(g => new Grafico { TipoGrafico = g.Key.Tipo, Nombre = g.Key.Nombre, Valor = g.Sum(v => v.Valor) }).ToList();


            /*
            * Para determinar la cantidad de graficos que tiene cada tipo (Poligono, Linea, Punto), agrupo por Tipo y 
            * determino la cantidad dividiendo el total de registros del grupo por la cantidad de "NOMBRES" diferentes para el grupo
            * 
            * Para terminar, por definicion, evito que se agregue la cantidad cuando hay solo 1 grafico de tipo PUNTO
            *          
            */
            foreach (var grupo in model.Graficos.GroupBy(g => new { g.TipoGrafico }, (k, v) => new { tipo = k.TipoGrafico, cantidad = v.Count() / v.Select(x => x.Nombre).Distinct().Count() }).Where(g => g.tipo != TIPO_PUNTO || g.cantidad > 1))
            {
                lista.Add(new Grafico() { TipoGrafico = grupo.tipo, Nombre = "Cant. Gráficos", Valor = grupo.cantidad });
            }


            model.Graficos = lista.OrderBy(g => g.TipoGrafico).ThenBy(g => g.Nombre).ToList();
            return model;
        }

        private List<ImagenGrafico> GetImagenObjeto(string objetoId, long componenteId)
        {
            try
            {
                var result = _cliente.GetAsync(string.Format("api/DetalleObjeto/GetImagen?objetoId={0}&componenteId={1}", objetoId, componenteId)).Result;
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsAsync<List<ImagenGrafico>>().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Componente GetComponenteByDocType(string docType)
        {
            var result = _cliente.GetAsync(string.Format("api/Componente/Get?docType=" + docType)).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<Componente>().Result;
        }

        private Componente GetComponenteByTable(string table)
        {
            var result = _cliente.GetAsync(string.Format("api/Componente/GetByTable?table=" + table)).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<Componente>().Result;
        }
    }
}