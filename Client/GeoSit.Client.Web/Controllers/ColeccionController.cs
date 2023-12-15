using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using GeoSit.Client.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using Resources;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Net;
using Newtonsoft.Json;
using GeoSit.Client.Web.Helpers;

namespace GeoSit.Client.Web.Controllers
{
    public class ColeccionController : Controller
    {
        private FileResult FileToDownload { get { return Session["file_to_download"] as FileResult; } set { Session["file_to_download"] = value; } }
        private UsuariosModel Usuario { get { return (UsuariosModel)Session["usuarioPortal"]; } }
        private readonly HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });
        public ColeccionController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            _cliente.Timeout = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["httpClientIncreasedTimeout"]));
        }

        public ActionResult GestionarColecciones()
        {
            var coleccionesUsuario = GetAllColeccionesByUsuario(Usuario.Id_Usuario);
            return PartialView(coleccionesUsuario);
        }

        public ActionResult RefrescarColecciones()
        {
            var coleccionesUsuario = GetAllColeccionesByUsuario(Usuario.Id_Usuario);
            return PartialView("~/Views/Coleccion/Colecciones.cshtml", coleccionesUsuario);
        }

        public ActionResult ComponentesColeccion(long coleccionId)
        {
            var model = GetComponentesByColeccionId(coleccionId);
            return PartialView(model);
        }

        //public ActionResult CrearColeccionInsertarObjetos(string nombreColeccion, string[] objetos)
        //{
        //    List<ObjetoComponente> lista = new List<ObjetoComponente>();
        //    var res = NuevaColeccion(nombreColeccion);

        //    if (res.GetType() == typeof(JsonResult))
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed, "La colección no ha sido generada");
        //    }
        //    else
        //    {
        //        long idColeccion = -1;
        //        ColeccionListaModels coleccionesUsuario = GetAllColeccionesByUsuario(Usuario.Id_Usuario);

        //        var coleccion = coleccionesUsuario.Colecciones.Single(c => c.Nombre == nombreColeccion);

        //        idColeccion = coleccion.ColeccionId;
        //        for (int i = 0; i < objetos.Length; i = i + 2)
        //        {
        //            var oCompObj = new ObjetoComponente();
        //            oCompObj.ObjetoId = Convert.ToInt32(objetos[i]);
        //            oCompObj.ComponenteDocType = objetos[i + 1];
        //            lista.Add(oCompObj);
        //        }
        //        var r = ColeccionMedianteSeleccionObjetos(idColeccion, lista);
        //        AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Alta, Autorizado.Si, Eventos.AltaColeccion);
        //        return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = "ok" };
        //    }
        //}

        public ActionResult NuevaColeccion(string nombre, ObjetoComponente[] objetos)
        {
            var resp = _cliente
                            .PostAsync($"api/Coleccion/NuevaColeccion?usuarioId={Usuario.Id_Usuario}&nombre={nombre}",
                                       new ObjectContent<ObjetoComponente[]>(objetos, new JsonMediaTypeFormatter()))
                            .Result;
            if (resp.IsSuccessStatusCode)
            {
                Session["coleccionModificada"] = nombre;
                AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, TipoOperacion.Alta, Autorizado.Si, Eventos.AltaColeccion);
                return RedirectToAction("GestionarColecciones");
            }
            else
            {
                if(resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    var obj = JsonConvert.DeserializeAnonymousType(resp.Content.ReadAsStringAsync().Result, new { Message = string.Empty });
                    if (obj.Message == "nombre")
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.LengthRequired);
                }
                return new HttpStatusCodeResult(resp.StatusCode);
            }
        }

        private ColeccionListaModels GetAllColeccionesByUsuario(long usuarioId)
        {
            var result = _cliente.GetAsync("api/Coleccion/GetColeccionesByUserId?usuarioId=" + usuarioId).Result;
            result.EnsureSuccessStatusCode();

            var coleccionesUsuario = result.Content.ReadAsAsync<IEnumerable<ColeccionModel>>().Result;
            long idColeccion = -1;
            string nombreColeccion = Session["coleccionModificada"] != null ? Session["coleccionModificada"].ToString() : string.Empty;
            long.TryParse(nombreColeccion, out idColeccion);

            foreach (var coleccion in coleccionesUsuario)
            {
                coleccion.Modificada = coleccion.ColeccionId.Equals(idColeccion) || coleccion.Nombre.Equals(nombreColeccion);
                if (coleccion.Cantidad == 0 && coleccion.Componentes.Any())
                {
                    var totalObjetos = coleccion.Componentes.Sum(componente => componente.Objetos.Count());
                    coleccion.Cantidad = totalObjetos;
                }
            }
            var listaColeccionesModels = new ColeccionListaModels { Colecciones = new List<ColeccionModel>(coleccionesUsuario.OrderBy(x => x.Nombre)) };
            return listaColeccionesModels;
        }

        private bool ExisteNombreColeccion(string nombreColeccion)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/ValidarNombreColeccion?usuarioId={0}&nombreColeccion={1}", Usuario.Id_Usuario, nombreColeccion)).Result;
            result.EnsureSuccessStatusCode();

            return result.Content.ReadAsAsync<bool>().Result;
        }

        //SF01 - Copiar coleccion
        public ActionResult CopiarColeccion(long coleccionId, string nombreColeccion)
        {
            if (ExisteNombreColeccion(nombreColeccion))
                return Json(new { success = false, msg = string.Format(Recursos.ColeccionController_NombreColeccionExistente, nombreColeccion) }, JsonRequestBehavior.AllowGet);

            var result = _cliente.GetAsync(string.Format("api/Coleccion/CopiarColeccion?usuarioId={0}&coleccionId={1}&nombreColeccion={2}", Usuario.Id_Usuario, coleccionId, nombreColeccion)).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = nombreColeccion;
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Alta, Autorizado.Si, Eventos.AltaColeccion);
            return RedirectToAction("RefrescarColecciones");
        }

        //SF002 - Unir colecciones
        public ActionResult UnirColecciones(long coleccionId1, long coleccionId2, string nombreColeccion)
        {
            if (ExisteNombreColeccion(nombreColeccion))
                return Json(new { success = false, msg = string.Format(Recursos.ColeccionController_NombreColeccionExistente, nombreColeccion) }, JsonRequestBehavior.AllowGet);

            var result = _cliente.GetAsync(string.Format("api/Coleccion/UnirColecciones?usuarioId={0}&coleccionId1={1}&coleccionId2={2}&nombreColeccion={3}", Usuario.Id_Usuario, coleccionId1, coleccionId2, nombreColeccion)).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = nombreColeccion;
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Alta, Autorizado.Si, Eventos.AltaColeccion);
            return RedirectToAction("RefrescarColecciones");
        }

        //SF003 - Renombrar colección
        public ActionResult RenombrarColeccion(long coleccionId, string nombreColeccion)
        {
            if (ExisteNombreColeccion(nombreColeccion))
                return Json(new { success = false, msg = string.Format(Recursos.ColeccionController_NombreColeccionExistente, nombreColeccion) }, JsonRequestBehavior.AllowGet);

            var result = _cliente.GetAsync(string.Format("api/Coleccion/RenombrarColeccion?usuarioId={0}&coleccionId={1}&nombreColeccion={2}", Usuario.Id_Usuario, coleccionId, nombreColeccion)).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = nombreColeccion;
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Modificacion, Autorizado.Si, Eventos.ModificarColeccion);
            return RedirectToAction("RefrescarColecciones");
        }

        //SF004 - Exportar colección a archivo
        public ActionResult ExportarColeccionArchivo(long coleccionId)
        {
            try
            {
                var result = _cliente.GetAsync("api/Coleccion/GetColeccionById?id=" + coleccionId).Result;
                result.EnsureSuccessStatusCode();

                var coleccion = result.Content.ReadAsAsync<ColeccionModel>().Result;

                var sb = new StringBuilder();
                var headerColumnas = new[] { "ComponenteId", "ObjetoId" };
                sb.AppendLine(string.Join(",", headerColumnas));

                foreach (var componente in coleccion.Componentes)
                {
                    foreach (var objeto in componente.Objetos)
                    {
                        sb.AppendLine(string.Join(",", new[] { componente.ComponenteId.ToString(), objeto.ObjetoId.ToString() }));
                    }
                }
                AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.ExportarColeccion);
                FileToDownload = File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", coleccion.Nombre + ".csv");
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("ExportarColeccion", ex);
                return Json(new { success = false, message = "Sucedió un error al intentar exportar la colección a csv." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, message = "Exportado correctamente" }, JsonRequestBehavior.AllowGet);
        }

        //SF006 - Dar de baja una colección
        public ActionResult BajaColeccion(long coleccionId)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/BajaColeccion?usuarioId={0}&coleccionId={1}", Usuario.Id_Usuario, coleccionId)).Result;
            result.EnsureSuccessStatusCode();
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Baja, Autorizado.Si, Eventos.BajaColeccion);
            Session["coleccionModificada"] = 0; //limpio
            return RedirectToAction("RefrescarColecciones");
        }
        public ActionResult BajaColecciones(string idcolecciones)
        {

            //var result = _cliente.GetAsync(string.Format("api/Coleccion/BajaColecciones?usuarioId={0}&coleccionId={1}", Usuario.Id_Usuario, idcolecciones)).Result;
            //var result = _cliente.GetAsync(string.Format("api/Coleccion/BajaColeccion?usuarioId={0}&coleccionId={1}", Usuario.Id_Usuario, coleccionId)).Result;
            var result2 = _cliente.GetAsync(string.Format("api/Coleccion/BajaColecciones?usuarioId={0}&coleccionId={1}", Usuario.Id_Usuario, idcolecciones)).Result;
            result2.EnsureSuccessStatusCode();
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Baja, Autorizado.Si, Eventos.BajaColeccion);
            Session["coleccionModificada"] = 0; //limpio
            return RedirectToAction("RefrescarColecciones");
        }
        //SF007 - Colección mediante selección de objetos 
        public ActionResult ColeccionMedianteSeleccionObjetos(long coleccionId, List<ObjetoComponente> objetoComponente)
        {
            HttpResponseMessage result = _cliente.PostAsync(string.Format("api/Coleccion/ColeccionMedianteSeleccionObjetos?usuarioId={0}&coleccionId={1}", Usuario.Id_Usuario, coleccionId), objetoComponente, new JsonMediaTypeFormatter()).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = coleccionId;
            return RedirectToAction("RefrescarColecciones");
        }

        //SF008 - Colección mediante archivo externo 
        [HttpPost]
        public ActionResult ImportarArchivoColeccion(string nombreColeccion)
        {
            var file = Request.Files[0];
            ColeccionModel coleccion = null;
            try
            {
                var reader = new StreamReader(file.InputStream);

                if (ExisteNombreColeccion(nombreColeccion))
                {
                    return Json(new { success = false, msg = "El nombre especificado ya existe." }, JsonRequestBehavior.AllowGet);
                }

                string[] lineaHeader = reader.ReadLine().Split(',');

                if (lineaHeader.Count() != 2 || lineaHeader[0].ToUpper() != "COMPONENTEID" || lineaHeader[1].ToUpper() != "OBJETOID")
                {
                    return Json(new { success = false, msg = Recursos.ColeccionController_ImportarArchivoColeccion_FormatoInvalido }, JsonRequestBehavior.AllowGet);
                }

                coleccion = new ColeccionModel();
                coleccion.Nombre = nombreColeccion;
                coleccion.UsuarioId = Usuario.Id_Usuario;

                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(',');

                    var componente = coleccion.Componentes.FirstOrDefault(c => c.ComponenteId == Convert.ToInt32(line[0]));
                    if (componente == null)
                    {
                        componente = new Componente() { ComponenteId = Convert.ToInt32(line[0]) };
                        componente.Objetos.Add(new Objeto() { ObjetoId = Convert.ToInt32(line[1]) });
                        coleccion.Componentes.Add(componente);
                    }
                    else
                    {
                        if (!componente.Objetos.Any(o => o.ObjetoId == Convert.ToInt32(line[1])))
                        {
                            componente.Objetos.Add(new Objeto() { ObjetoId = Convert.ToInt32(line[1]) });
                        }
                    }

                }
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = Recursos.ColeccionController_ImportarArchivoColeccion_FormatoInvalido }, JsonRequestBehavior.AllowGet);
            }

            HttpResponseMessage result = _cliente.PostAsync("api/Coleccion/ImportarNuevaColeccion?usuarioId=" + Usuario.Id_Usuario, coleccion, new JsonMediaTypeFormatter()).Result;
            result.EnsureSuccessStatusCode();
            int cantidad = result.Content.ReadAsAsync<int>().Result;
            Session["coleccionModificada"] = nombreColeccion;
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Alta, Autorizado.Si, Eventos.ImportarColeccion);
            return Json(new { success = true, msg = string.Format("La Colección fue importada exitosamente y [{0}] objetos válidos fueron importados.", cantidad) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //ordenar colecciones
        public ActionResult Ordenar(int orden, string filtro = null)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/Ordenar?usuarioId={0}&orden={1}&filtro={2}", Usuario.Id_Usuario, orden, filtro)).Result;
            result.EnsureSuccessStatusCode();
            var colecciones = result.Content.ReadAsAsync<List<ColeccionModel>>().Result;
            return PartialView("~/Views/Coleccion/Colecciones.cshtml", new ColeccionListaModels { Colecciones = colecciones });
        }

        //SF010 - Quitar objeto
        public ActionResult QuitarObjetoColeccion(int objetoId, int componenteId, long coleccionId)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/QuitarObjetoColeccion?usuarioId={0}&objetoId={1}&componenteId={2}&coleccionId={3}", Usuario.Id_Usuario, objetoId, componenteId, coleccionId)).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = coleccionId;
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Modificacion, Autorizado.Si, Eventos.EliminarObjetosColeccion);
            return RedirectToAction("RefrescarColecciones");
        }
        public ActionResult QuitarMultiplesObjetoColeccion(string objetoId, int componenteId, long coleccionId)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/QuitarMultiplesObjetos?usuarioId={0}&objetoId={1}&componenteId={2}&coleccionId={3}", Usuario.Id_Usuario, objetoId, componenteId, coleccionId)).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = coleccionId;
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Modificacion, Autorizado.Si, Eventos.EliminarObjetosColeccion);
            return RedirectToAction("RefrescarColecciones");
        }
        public ActionResult AgregarObjetoColeccion(int objetoId, int componenteId, long coleccionId)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/AgregarObjetoColeccion?usuarioId={0}&objetoId={1}&componenteId={2}&coleccionId={3}", Usuario.Id_Usuario, objetoId, componenteId, coleccionId)).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = coleccionId;
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Modificacion, Autorizado.Si, Eventos.AgregarObjetosColeccion);
            return RedirectToAction("RefrescarColecciones");
        }

        //SF011 - Limpiar colección
        public ActionResult LimpiarColeccion(long coleccionId)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/LimpiarColeccion?usuarioId={0}&coleccionId={1}", Usuario.Id_Usuario, coleccionId)).Result;
            result.EnsureSuccessStatusCode();
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Modificacion, Autorizado.Si, Eventos.EliminarObjetosColeccion);
            Session["coleccionModificada"] = coleccionId;
            return RedirectToAction("RefrescarColecciones");
        }

        ////SF012 - Colección Nueva
        //public ActionResult NuevaColeccion(string nombreColeccion)
        //{
        //    if (ExisteNombreColeccion(nombreColeccion))
        //        return Json(new { success = false, msg = string.Format(Recursos.ColeccionController_NombreColeccionExistente, nombreColeccion) }, JsonRequestBehavior.AllowGet);

        //    var result = _cliente.GetAsync(string.Format("api/Coleccion/NuevaColeccion?usuarioId={0}&nombreColeccion={1}", Usuario.Id_Usuario, nombreColeccion)).Result;
        //    result.EnsureSuccessStatusCode();
        //    Session["coleccionModificada"] = nombreColeccion;
        //    return RedirectToAction("RefrescarColecciones");
        //}

        //SF013 - Exportación a Excel
        public JsonResult ExportarColeccionExcel(long coleccionId)
        {
            JsonResult resultado = null;
            try
            {
                HttpResponseMessage resp = _cliente.GetAsync("api/Coleccion/GetColeccionById/" + coleccionId).Result;
                resp.EnsureSuccessStatusCode();
                var coleccion = resp.Content.ReadAsAsync<ColeccionModel>().Result;

                var objetos = coleccion.Componentes.SelectMany(x => x.Objetos, (comp, obj) => new ObjetoExcel { componente = comp.DocType, id = obj.ObjetoId });
                var exportador = new ExportadorObjetosExcel(objetos);
                AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.ExportarColeccion);
                FileToDownload = File(exportador.Exportar("Colección"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Coleccion.xlsx");
                resultado = Json(new { success = true, message = "Exportado correctamente" }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Web.Http.HttpResponseException httpEx)
            {
                resultado = Json(new { success = false, message = httpEx.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                resultado = Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return resultado;
        }

        public JsonResult ExportarObjetoExcel(string objetos)
        {
            var exportador = new ExportadorObjetosExcel(JsonConvert.DeserializeObject<ObjetoExcel[]>(objetos));
            FileToDownload = File(exportador.Exportar("Objetos"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportacionObjetos.xlsx");
            return Json(new { Data = "ok" });
        }

        public FileResult DownloadFile()
        {
            return FileToDownload;
        }

        private ColeccionModel GetComponentesByColeccionId(long coleccionId)
        {
            var result = _cliente.GetAsync(string.Format("api/Coleccion/GetComponentesByColeccionId?coleccionId={0}", coleccionId)).Result;
            result.EnsureSuccessStatusCode();
            Session["coleccionModificada"] = coleccionId;
            return result.Content.ReadAsAsync<ColeccionModel>().Result;
        }

        public JsonResult GetFiltros(string seleccionados, string[] layers)
        {
            //esto queda provisoriamente asi, pero habria que ver si se tiene que armar basado en los metadatos
            var filtrosParaCapasFiltradas = layers != null ? layers.Select(l => new { layer = l, filtro = string.Format("USUARIO={0}", Usuario.Id_Usuario) }).ToArray() : new object[0];

            var grupos = JsonConvert.DeserializeAnonymousType(seleccionados, new[] { new { capa = string.Empty, componente = 0L, objetos = new string[0] } });

            var ubicar = new List<object>();
            foreach (var grupo in grupos)
            {
                using (var resp = _cliente.GetAsync(string.Format("api/Atributo/GetByComponente/{0}", grupo.componente)).Result)
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        var clave = resp.Content.ReadAsAsync<IEnumerable<GeoSit.Data.BusinessEntities.MapasTematicos.Atributo>>().Result.FirstOrDefault(a => a.EsClave == 1) ?? new GeoSit.Data.BusinessEntities.MapasTematicos.Atributo();
                        foreach (string c in grupo.capa.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var objetosPorCapa = new
                            {
                                capa = c,
                                campo = clave.Campo,
                                objetos = grupo.objetos
                            };
                            ubicar.Add(objetosPorCapa);
                        }
                    }
                }
            }
            return Json(new { success = true, data = ubicar, filtros = filtrosParaCapasFiltradas }, JsonRequestBehavior.AllowGet);
        }
    }
}