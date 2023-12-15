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
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Text;
using System.Linq;
using GeoSit.Client.Web.Models.Inspecciones;
using GeoSit.Client.Web.Helpers;
using GeoSit.Data.BusinessEntities.GlobalResources;


namespace GeoSit.Client.Web.Controllers
{
    public class ObrasParticularesController : Controller
    {
        private HttpClient cliente = new HttpClient();

        private string UploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public ObrasParticularesController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        #region Gestion de Inspecciones
        // GET: ObrasParticulares
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: ObrasParticulares/GestionInspecciones
        public ActionResult GetIiposInspeccionesPorInspector(int idInspector, bool limitarTipos)
        {
            List<InspectorTipoInspeccion> inspectorSeleccionadoTipoInspeccionList = new List<InspectorTipoInspeccion>();
            List<InspectorTipoInspeccion> inspectorResultTipoInspeccionList = new List<InspectorTipoInspeccion>();
            List<InspectorTipoInspeccion> inspectorConectadoTipoInspeccionList = new List<InspectorTipoInspeccion>();
            List<TipoInspeccionModel> tiposInspecciones;
            InspectorModel inspectorConectado = null;
            var usuario = (UsuariosModel)Session["usuarioPortal"];


            if (limitarTipos)
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectorByIdUsuario/?usuario=" + usuario.Id_Usuario).Result;
                resp.EnsureSuccessStatusCode();
                inspectorConectado = (InspectorModel)resp.Content.ReadAsAsync<InspectorModel>().Result;

                resp = cliente.GetAsync("api/ObrasParticularesService/GetIiposInspeccionesPorInspector/?inspector=" + inspectorConectado.InspectorID).Result;
                resp.EnsureSuccessStatusCode();
                inspectorConectadoTipoInspeccionList = (List<InspectorTipoInspeccion>)resp.Content.ReadAsAsync<List<InspectorTipoInspeccion>>().Result;
            }
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetTiposInspecciones").Result;
                resp.EnsureSuccessStatusCode();
                tiposInspecciones = (List<TipoInspeccionModel>)resp.Content.ReadAsAsync<IEnumerable<TipoInspeccionModel>>().Result;

            }

            if (idInspector != 0)
            {

                {
                    HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetIiposInspeccionesPorInspector/?inspector=" + idInspector).Result;
                    resp.EnsureSuccessStatusCode();
                    inspectorSeleccionadoTipoInspeccionList = resp.Content.ReadAsAsync<List<InspectorTipoInspeccion>>().Result;
                }

                foreach (InspectorTipoInspeccion iti in inspectorSeleccionadoTipoInspeccionList)
                {
                    if (!limitarTipos || inspectorConectadoTipoInspeccionList.Find(a => a.TipoInspeccionID == iti.TipoInspeccionID) != null)
                    {
                        inspectorResultTipoInspeccionList.Add(iti);
                    }
                }
            }
            else
            {
                inspectorResultTipoInspeccionList = inspectorConectadoTipoInspeccionList;
            }

            foreach (InspectorTipoInspeccion iti in inspectorResultTipoInspeccionList)
            {
                iti.Descripcion = tiposInspecciones.Find(a => a.TipoInspeccionID == iti.TipoInspeccionID)?.Descripcion;
            }

            return Json(inspectorResultTipoInspeccionList, JsonRequestBehavior.AllowGet);
        }

        // GET: ObrasParticulares/GestionInspecciones
        public ActionResult GetInspectoresPorTipoInspeccion(int tipoInspeccion, bool incluyeBaja)
        {
            List<InspectorModel> inspectoresResult = new List<InspectorModel>();


            var usuario = (UsuariosModel)Session["usuarioPortal"];

            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectorByIdUsuario/?usuario=" + usuario.Id_Usuario).Result;
            resp.EnsureSuccessStatusCode();
            InspectorModel inspectorConectado = (InspectorModel)resp.Content.ReadAsAsync<InspectorModel>().Result;
            if (inspectorConectado.EsPlanificador == "S")
            {
                if (tipoInspeccion != 0)
                {
                    StringObject so = new StringObject() { Response = tipoInspeccion.ToString() };
                    resp = cliente.PostAsJsonAsync("api/ObrasParticularesService/PostInspectoresByTiposInspeccion", so).Result;
                }
                else
                {
                    resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;
                }

                foreach (InspectorModel inspector in resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result)
                {
                    if (incluyeBaja)
                    {
                        inspectoresResult.Add(inspector);
                    }
                    else if (
                            (inspector.Usuario.Fecha_baja == null ||
                            inspector.Usuario.Fecha_baja < new DateTime(10)) &&
                            (inspector.FechaBaja == null ||
                            inspector.FechaBaja < new DateTime(10))
                        )
                    {
                        inspectoresResult.Add(inspector);
                    }
                }
            }
            else
            {
                inspectoresResult.Add(inspectorConectado);
            }


            return Json(inspectoresResult, JsonRequestBehavior.AllowGet);
        }

        // GET: ObrasParticulares/GestionInspecciones
        public ActionResult GestionInspecciones(bool agregaExpedienteObra = false)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            if (usuario == null)
            {
                return View("~/Views/Account/Login.cshtml");
            }

            var model = new GestionInspeccionesModel()
            {
                CmbTipoInspectores = new List<SelectListItem>(),
                CmbInspectores = new List<SelectListItem>(),
                CmbInspectoresActivos = new List<SelectListItem>(),
                CmbTipos = new List<SelectListItem>(),
                CmbObjetos = new List<SelectListItem>(),
                InspeccionUnidadeTributarias = new List<InspeccionUnidadesTributarias>(),
                InspeccionId = "0"
            };

            //Cargo datos del inspector actualmente conectado
            InspectorModel inspectorConectado = null;
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectorByIdUsuario/?usuario=" + usuario.Id_Usuario).Result;
                resp.EnsureSuccessStatusCode();
                inspectorConectado = resp.Content.ReadAsAsync<InspectorModel>().Result;

                if (inspectorConectado == null)
                {
                    ViewBag.Title = "Inspecciones";
                    ViewBag.Description = "El usuario no se encuentra habilitado como inspector.";
                    ViewBag.ReturnUrl = Url.Action("Index", "Home");
                    return PartialView("InformationMessageView");
                }

                resp = cliente.GetAsync("api/ObrasParticularesService/GetIiposInspeccionesPorInspector/?inspector=" + inspectorConectado.InspectorID).Result;
                resp.EnsureSuccessStatusCode();
                var inspectorTipoInspeccionList = resp.Content.ReadAsAsync<List<InspectorTipoInspeccion>>().Result;

                resp = cliente.GetAsync("api/ObrasParticularesService/GetTiposInspecciones").Result;
                resp.EnsureSuccessStatusCode();
                var tiposInspecciones = resp.Content.ReadAsAsync<List<TipoInspeccionModel>>().Result;

                model.CmbTipoInspectores.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
                foreach (var iti in inspectorTipoInspeccionList)
                {
                    var tipoInspeccion = tiposInspecciones.Find(a => a.TipoInspeccionID == iti.TipoInspeccionID);
                    inspectorConectado.TiposInspeccion.Add(tipoInspeccion);
                    model.CmbTipoInspectores.Add(new SelectListItem() { Text = tipoInspeccion.Descripcion, Value = tipoInspeccion.TipoInspeccionID.ToString() });
                }
                model.Planificador = inspectorConectado.EsPlanificador;
            }

            {

                if (model.Planificador == "S")
                {
                    model.CmbInspectores.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
                    model.CmbInspectoresActivos.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });

                    var so = new StringObject() { Response = string.Join(",", inspectorConectado.TiposInspeccion.Select(ti => ti.TipoInspeccionID.ToString())) };
                    HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ObrasParticularesService/PostInspectoresByTiposInspeccion", so).Result;
                    var inspectores = resp.Content.ReadAsAsync<List<InspectorModel>>().Result;

                    foreach (InspectorModel inspector in inspectores)
                    {
                        var item = new SelectListItem() { Text = $"{inspector.Usuario.Apellido} {inspector.Usuario.Nombre}", Value = inspector.InspectorID.ToString() };
                        model.CmbInspectores.Add(item);

                        if (
                            (inspector.Usuario.Fecha_baja == null ||
                            inspector.Usuario.Fecha_baja < new DateTime(10)) &&
                            (inspector.FechaBaja == null ||
                            inspector.FechaBaja < new DateTime(10))
                            )
                        {
                            model.CmbInspectoresActivos.Add(item);
                        }
                    }
                }
                else
                {
                    model.CmbInspectores.Add(new SelectListItem() { Text = $"{inspectorConectado.Usuario.Apellido} {inspectorConectado.Usuario.Nombre}", Value = inspectorConectado.InspectorID.ToString(), Selected = true });
                }
            }

            {
                model.CmbObjetos.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
                model.CmbObjetos.Add(new SelectListItem() { Text = "Expedientes", Value = "1" });
                model.CmbObjetos.Add(new SelectListItem() { Text = "Actas", Value = "2" });
                model.CmbObjetos.Add(new SelectListItem() { Text = "Trámites", Value = "3" });
            }

            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetEstadosInspecciones").Result;
                var inspectores = resp.Content.ReadAsAsync<List<EstadoInspeccionModel>>().Result;
                inspectores = inspectores.Where(x => x.EstadoInspeccionID < 3).ToList();
                model.CmbEstados = new SelectList(inspectores, "EstadoInspeccionID", "Descripcion");
            }
            ViewData["agregaExpedienteObra"] = agregaExpedienteObra;
            return PartialView(model);
        }

        // GET: ObrasParticulares/GestionInspeccion/1
        public ActionResult GetInspeccion(int inspeccion)
        {
            InspeccionModel detalleInspeccion = null;
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspeccion/?inspeccion=" + inspeccion).Result;
                resp.EnsureSuccessStatusCode();
                detalleInspeccion = (InspeccionModel)resp.Content.ReadAsAsync<InspeccionModel>().Result;
            }

            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspeccionUnidadesTributariasByInspeccion/?inspeccion=" + inspeccion).Result;
                resp.EnsureSuccessStatusCode();
                detalleInspeccion.InspeccionUnidadeTributarias = (List<InspeccionUnidadesTributarias>)resp.Content.ReadAsAsync<List<InspeccionUnidadesTributarias>>().Result;
            }

            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspeccionDocumentosByInspeccion/?inspeccion=" + inspeccion).Result;
                resp.EnsureSuccessStatusCode();
                detalleInspeccion.InspeccionDocumento = (List<InspeccionDocumento>)resp.Content.ReadAsAsync<List<InspeccionDocumento>>().Result;
            }

            HttpResponseMessage respx = cliente.GetAsync("api/ObrasParticularesService/GetEstadosInspecciones").Result;
            //resp.EnsureSuccessStatusCode();
            List<EstadoInspeccionModel> estadoInspeccionList = (List<EstadoInspeccionModel>)respx.Content.ReadAsAsync<IEnumerable<EstadoInspeccionModel>>().Result;
            detalleInspeccion.EstadosInspeccion = new List<EstadoInspeccionModel>();
            foreach (EstadoInspeccionModel estadoInspeccion in estadoInspeccionList)
            {
                if (
                    //Planificada
                    detalleInspeccion.SelectedEstado == 1 && estadoInspeccion.EstadoInspeccionID == 1 ||
                    detalleInspeccion.SelectedEstado == 1 && estadoInspeccion.EstadoInspeccionID == 2 ||
                    detalleInspeccion.SelectedEstado == 1 && estadoInspeccion.EstadoInspeccionID == 4 ||
                    //Abierta
                    detalleInspeccion.SelectedEstado == 2 && estadoInspeccion.EstadoInspeccionID == 2 ||
                    detalleInspeccion.SelectedEstado == 2 && estadoInspeccion.EstadoInspeccionID == 4 ||
                    //Vencida
                    detalleInspeccion.SelectedEstado == 3 && estadoInspeccion.EstadoInspeccionID == 3 ||
                    detalleInspeccion.SelectedEstado == 3 && estadoInspeccion.EstadoInspeccionID == 1 ||
                    detalleInspeccion.SelectedEstado == 3 && estadoInspeccion.EstadoInspeccionID == 4 ||

                    //Finalizada
                    detalleInspeccion.SelectedEstado == 4 && estadoInspeccion.EstadoInspeccionID == 4
                    )
                    detalleInspeccion.EstadosInspeccion.Add(estadoInspeccion);
            }

            {

                var usuario = (UsuariosModel)Session["usuarioPortal"];
                if (usuario == null)
                {
                    return View("~/Views/Account/Login.cshtml");
                }

                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectorByIdUsuario/?usuario=" + usuario.Id_Usuario).Result;
                resp.EnsureSuccessStatusCode();
                InspectorModel inspectorConectado = (InspectorModel)resp.Content.ReadAsAsync<InspectorModel>().Result;

                detalleInspeccion.UsuarioUpdate = inspectorConectado.InspectorID;
                detalleInspeccion.EsPlanificador = inspectorConectado.EsPlanificador;

            }


            return Json(detalleInspeccion, JsonRequestBehavior.AllowGet);
        }

        //GET: PuedeProgramar
        public ActionResult PuedeProgramar(StringObject inspector)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspector/?inspector=" + inspector.Response).Result;
            resp.EnsureSuccessStatusCode();
            InspectorModel detalleInspector = (InspectorModel)resp.Content.ReadAsAsync<InspectorModel>().Result;

            StringObject response = new StringObject();
            response.Response = "true";
            if (detalleInspector != null &&
                    (detalleInspector.Usuario.Fecha_baja != null ||
                    detalleInspector.Usuario.Fecha_baja > new DateTime(10)) &&
                    (detalleInspector.FechaBaja != null ||
                    detalleInspector.FechaBaja > new DateTime(10))

                )
                response.Response = "false";

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // GET: ObrasParticulares/Inspecciones/1/1/1/1/1
        public ActionResult Inspecciones(int tipoInspeccion, int Inspector, long from, long to, long utc_offset)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspecciones/?from=" + from + "&to=" + to + "&utc_offset=" + utc_offset + "&tipoInspeccion=" + tipoInspeccion + "&idInspector=" + Inspector + "&resource=" + Resources.Recursos.InspeccionDe).Result;
            resp.EnsureSuccessStatusCode();
            CalendarioModel model = (CalendarioModel)resp.Content.ReadAsAsync<CalendarioModel>().Result;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExisteObjeto(int objeto, int tipo, string identificador)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetExisteObjeto/?objeto=" + objeto + "&tipo=" + tipo + "&identificador=" + identificador).Result;
            resp.EnsureSuccessStatusCode();
            long model = (long)resp.Content.ReadAsAsync<long>().Result;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOtrosObjetos(int tipo, string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetOtrosObjetos/?&tipo=" + tipo + "&id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            string model = (string)resp.Content.ReadAsAsync<string>().Result;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]*/
        public ActionResult PostInspeccionProgramar(InspeccionModel parametros, string returnUrl)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            parametros.UsuarioUpdate = usuario == null ? 1 : usuario.Id_Usuario;
            if (!String.IsNullOrEmpty(parametros.selectedUT))
                parametros.selectedUT = parametros.selectedUT.Replace("unidad_tributaria-", "");

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ObrasParticularesService/PostInspeccion/", parametros).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                var result = resp.Content.ReadAsAsync<GeoSit.Data.BusinessEntities.ObrasParticulares.Inspeccion>().Result;
                AuditoriaHelper.Register(usuario.Id_Usuario, "Se programo la inspeccion correctamente.",
                          Request, TiposOperacion.Alta, Autorizado.No, Eventos.AltaInspeccion);
                return Json(new { inspeccion = result.InspeccionID, Response = "OK" });
            }
            catch (Exception)
            {
                var error = resp.Content.ReadAsAsync<Exception>().Result;
                return Json(new { error = error.Message, Response = "ERROR" });
            }
        }

        public ActionResult GetInspeccionBaja(int inspeccionID)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            long usuarioConectado = usuario == null ? 1 : usuario.Id_Usuario;

            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspeccionBaja/?&inspeccionID=" + inspeccionID + "&usuarioConectado=" + usuarioConectado).Result;
            resp.EnsureSuccessStatusCode();
            StringObject result = (StringObject)resp.Content.ReadAsAsync<StringObject>().Result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Registro de Inspectores
        public ActionResult GetInspectores()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;
            //resp.EnsureSuccessStatusCode();
            List<InspectorModel> inspectores = (List<InspectorModel>)resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result;
            List<InspectorModel> inspectoresResult = new List<InspectorModel>();
            foreach (InspectorModel inspector in inspectores)
            {
                if (
                    (inspector.Usuario.Fecha_baja == null ||
                    inspector.Usuario.Fecha_baja < new DateTime(10)) &&
                    (inspector.FechaBaja == null ||
                    inspector.FechaBaja < new DateTime(10))
                    )
                {
                    inspectoresResult.Add(inspector);
                }
            }
            return Json(inspectoresResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllInspectores()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;
            //resp.EnsureSuccessStatusCode();
            List<InspectorModel> inspectores = (List<InspectorModel>)resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result;
            return Json(inspectores, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegistroInspectores()
        {
            AdministracionInspectoresModel model = new AdministracionInspectoresModel();
            model.TiposInspeccionesSelected = "";
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;
                //resp.EnsureSuccessStatusCode();
                List<InspectorModel> inspectores = (List<InspectorModel>)resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result;
                List<InspectorModel> inspectoresResult = new List<InspectorModel>();
                foreach (InspectorModel inspector in inspectores)
                {
                    if (
                    (inspector.Usuario.Fecha_baja == null ||
                    inspector.Usuario.Fecha_baja < new DateTime(10)) &&
                    (inspector.FechaBaja == null ||
                    inspector.FechaBaja < new DateTime(10))
                    )
                    {
                        inspectoresResult.Add(inspector);
                    }
                }
                model.Inspectores = inspectoresResult.OrderBy(a => a.Usuario.Apellido).ThenBy(a => a.Usuario.Nombre).ToList<InspectorModel>();
            }
            {
                model.cmbUsuarios = new List<SelectListItem>();
                model.cmbUsuarios.Add(new SelectListItem() { Value = "0", Text = "Seleccione" });

                HttpResponseMessage resp = cliente.GetAsync("api/SeguridadService/GetUsuarios").Result;
                List<UsuariosModel> usuarios = (List<UsuariosModel>)resp.Content.ReadAsAsync<IEnumerable<UsuariosModel>>().Result;
                List<UsuariosModel> usuariosResult = new List<UsuariosModel>();
                foreach (UsuariosModel usuario in usuarios)
                {
                    if (usuario.Fecha_baja == null || usuario.Fecha_baja < new DateTime(10))
                    {
                        model.cmbUsuarios.Add(new SelectListItem() { Value = usuario.Id_Usuario.ToString(), Text = usuario.Apellido + ' ' + usuario.Nombre });
                    }
                }
            }
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetTiposInspecciones").Result;
                resp.EnsureSuccessStatusCode();
                model.TiposInspecciones = (List<TipoInspeccionModel>)resp.Content.ReadAsAsync<IEnumerable<TipoInspeccionModel>>().Result;

            }
            model.idInspector = "0";

            return PartialView(model);
        }

        public ActionResult GetUsuarios(int idUsuario)
        {
            List<InspectorModel> inspectoresResult = new List<InspectorModel>();
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;
                //resp.EnsureSuccessStatusCode();
                List<InspectorModel> inspectores = (List<InspectorModel>)resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result;
                inspectoresResult = new List<InspectorModel>();
                foreach (InspectorModel inspector in inspectores)
                {
                    if (
                    (inspector.Usuario.Fecha_baja == null ||
                    inspector.Usuario.Fecha_baja < new DateTime(10)) &&
                    (inspector.FechaBaja == null ||
                    inspector.FechaBaja < new DateTime(10))
                    )
                    {
                        inspectoresResult.Add(inspector);
                    }
                }
            }
            List<SelectListItem> cmbUsuarios = new List<SelectListItem>();
            {

                cmbUsuarios.Add(new SelectListItem() { Value = "0", Text = "Seleccione" });

                HttpResponseMessage resp = cliente.GetAsync("api/SeguridadService/GetUsuarios").Result;
                List<UsuariosModel> usuarios = (List<UsuariosModel>)resp.Content.ReadAsAsync<IEnumerable<UsuariosModel>>().Result;
                List<UsuariosModel> usuariosResult = new List<UsuariosModel>();
                foreach (UsuariosModel usuario in usuarios)
                {
                    if (idUsuario == 0)
                    {
                        if (
                            (usuario.Fecha_baja == null ||
                            usuario.Fecha_baja < new DateTime(10))
                            && inspectoresResult.Find(a => a.UsuarioID == usuario.Id_Usuario) == null
                            )
                        {
                            cmbUsuarios.Add(new SelectListItem() { Value = usuario.Id_Usuario.ToString(), Text = usuario.Apellido + ' ' + usuario.Nombre });
                        }
                    }
                    else
                    {
                        if (usuario.Id_Usuario == idUsuario)
                            cmbUsuarios.Add(new SelectListItem() { Value = usuario.Id_Usuario.ToString(), Text = usuario.Apellido + ' ' + usuario.Nombre });
                    }

                }
            }
            cmbUsuarios = cmbUsuarios.OrderBy(o => o.Text).ToList<SelectListItem>();
            return Json(cmbUsuarios, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostInspectorUpdate(InspectorModel inspector)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            inspector.UsuarioUpdate = usuario == null ? 1 : usuario.Id_Usuario;

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ObrasParticularesService/PostInspectorUpdate/", inspector).Result;
            AuditoriaHelper.Register(usuario.Id_Usuario, "Se actualizo el inspector correctamente.",
                          Request, TiposOperacion.Modificacion, Autorizado.Si, Eventos.ModificarInspeccion);

            return Json(resp.Content.ReadAsStringAsync(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInspectorRemove(int inspectorID)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectorRemove/?inspectorID=" + inspectorID + "&usuarioConectado=" + usuario.Id_Usuario).Result;

            return Json(resp.Content.ReadAsStringAsync(), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }

    public class StringObject
    {
        public string Response { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }

}