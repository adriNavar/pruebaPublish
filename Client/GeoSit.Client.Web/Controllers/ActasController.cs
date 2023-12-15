using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Client.Web.Models;
using GeoSit.Client.Web.Models.Inspecciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using GeoSit.Client.Web.Helpers;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Client.Web.Controllers
{
    public class ActasController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private readonly HttpClient _clienteReportes = new HttpClient();
        private List<DomicilioModel> Domicilios { get { return Session["Actas_Domicilios"] as List<DomicilioModel>; } set { Session["Actas_Domicilios"] = value; } }

        public ActasController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            _clienteReportes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        // GET: Actas
        public ActionResult Index()
        {
            ActaModel actaModel = new ActaModel();
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;
            resp.EnsureSuccessStatusCode();
            actaModel.Inspectores = (List<InspectorModel>)resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result;

            resp = cliente.GetAsync("api/ActaService/GetActasEstados").Result;
            resp.EnsureSuccessStatusCode();
            List<EstadoActa> ActasEstados = resp.Content.ReadAsAsync<IEnumerable<EstadoActa>>().Result.ToList();

            actaModel.CmbEstadosActas = new List<SelectListItem>();
            //actaModel.CmbEstadosActas.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
            foreach (EstadoActa estado in ActasEstados)
            {
                actaModel.CmbEstadosActas.Add(new SelectListItem() { Text = estado.Descripcion, Value = estado.EstadoActaId.ToString() });
            }

            resp = cliente.GetAsync("api/ActaService/GetActasTipos").Result;
            resp.EnsureSuccessStatusCode();
            List<ActaTipo> ActasTipos = (List<ActaTipo>)resp.Content.ReadAsAsync<IEnumerable<ActaTipo>>().Result;

            actaModel.CmbActasTipos = new List<SelectListItem>();
            //actaModel.CmbActasTipos.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
            foreach (ActaTipo actaTipo in ActasTipos)
            {
                actaModel.CmbActasTipos.Add(new SelectListItem() { Text = actaTipo.Descripcion, Value = actaTipo.ActaTipoId.ToString() });
            }

            actaModel.CmbInspectores = new List<SelectListItem>();
            //actaModel.CmbInspectores.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
            foreach (InspectorModel inspector in actaModel.Inspectores)
            {
                if (inspector.Usuario.Fecha_baja == null)
                    actaModel.CmbInspectores.Add(new SelectListItem() { Text = inspector.Usuario.Apellido + " " + inspector.Usuario.Nombre, Value = inspector.InspectorID.ToString() });
            }

            return PartialView(actaModel);
        }

        public ActionResult LoadActa(long id)
        {
            ActaModel actaModel = new ActaModel();
            HttpResponseMessage resp = cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;
            resp.EnsureSuccessStatusCode();
            actaModel.Inspectores = (List<InspectorModel>)resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result;

            resp = cliente.GetAsync("api/ActaService/GetActasEstados").Result;
            resp.EnsureSuccessStatusCode();
            List<EstadoActa> ActasEstados = resp.Content.ReadAsAsync<IEnumerable<EstadoActa>>().Result.ToList();

            actaModel.CmbEstadosActas = new List<SelectListItem>();
            actaModel.CmbEstadosActas.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
            foreach (EstadoActa estado in ActasEstados)
            {
                actaModel.CmbEstadosActas.Add(new SelectListItem() { Text = estado.Descripcion, Value = estado.EstadoActaId.ToString() });
            }

            resp = cliente.GetAsync("api/ActaService/GetActasTipos").Result;
            resp.EnsureSuccessStatusCode();
            List<ActaTipo> ActasTipos = (List<ActaTipo>)resp.Content.ReadAsAsync<IEnumerable<ActaTipo>>().Result;

            actaModel.CmbActasTipos = new List<SelectListItem>();
            actaModel.CmbActasTipos.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
            foreach (ActaTipo actaTipo in ActasTipos)
            {
                actaModel.CmbActasTipos.Add(new SelectListItem() { Text = actaTipo.Descripcion, Value = actaTipo.ActaTipoId.ToString() });
            }

            actaModel.CmbInspectores = new List<SelectListItem>();
            actaModel.CmbInspectores.Add(new SelectListItem() { Text = "Seleccione", Value = "0" });
            foreach (InspectorModel inspector in actaModel.Inspectores)
            {
                actaModel.CmbInspectores.Add(new SelectListItem() { Text = inspector.Usuario.Apellido + " " + inspector.Usuario.Nombre, Value = inspector.InspectorID.ToString() });
            }

            ViewBag.ActaId = id;
            ViewBag.buscaId = 1;

            return PartialView("~/Views/Actas/Index.cshtml", actaModel);
        }

        public ActionResult GetActaRolPersona()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ActaService/GetActaRolPersona").Result;
            resp.EnsureSuccessStatusCode();
            List<ActaRolPersona> actas = (List<ActaRolPersona>)resp.Content.ReadAsAsync<IEnumerable<ActaRolPersona>>().Result;
            return Json(actas.OrderBy(o => o.Descripcion));
        }

        public ActionResult GetActaBaja(int actaID)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            long usuarioConectado = usuario == null ? 1 : usuario.Id_Usuario;

            HttpResponseMessage resp = cliente.GetAsync("api/ActaService/GetActaBaja/?actaID=" + actaID + "&usuarioConectado=" + usuarioConectado).Result;
            resp.EnsureSuccessStatusCode();
            StringObject result = (StringObject)resp.Content.ReadAsAsync<StringObject>().Result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostActaGuardar(Acta parametros)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            parametros.UsuarioModificacionId = usuario == null ? 1 : usuario.Id_Usuario;

            //GUARDAR DATOS DOMICILIO Y LUEGO LOS DE ACTAS

            HttpResponseMessage resp;
            var selectedDomicilios = new HashSet<string>((parametros.SelectedDomicilio ?? string.Empty)
                                                                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                                                .Distinct());

            foreach (var domicilio in (Domicilios ?? new List<DomicilioModel>()).Where(d => d.DomicilioId <= 0))
            {
                domicilio.TipoDomicilio = null;
                resp = cliente.PostAsJsonAsync("api/DomicilioService/SetDomicilio_Save", domicilio).Result;
                resp.EnsureSuccessStatusCode();
                var domiGrabado = resp.Content.ReadAsAsync<DomicilioModel>().Result;
                selectedDomicilios.Add(domiGrabado.DomicilioId.ToString());
            }

            parametros.SelectedDomicilio = string.Join(",", selectedDomicilios.Where(sd => sd != "0"));
            resp = cliente.PostAsJsonAsync("api/ActaService/PostActaGuardar/", parametros).Result;
            string result = resp.StatusCode.ToString();
            if (resp.IsSuccessStatusCode)
            {
                Domicilios = null;
            }
            else
            {
                result = resp.Content.ReadAsAsync<string>().Result;
            }
            return Json(result);
        }

        public ActionResult PostActaBuscar(ActaBusqueda parametros)
        {
            using (var resp = cliente.PostAsJsonAsync("api/ActaService/PostActaBuscar/", parametros).Result)
            {
                var result = resp.Content.ReadAsAsync<object[][]>().Result;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetActaById(long idActa)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ActaService/GetActaById?id=" + idActa).Result;
            var result = resp.Content.ReadAsAsync<Acta>().Result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string GetActaBuscar()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ActaService/GetActaBuscarTable/").Result;
            var result = resp.Content.ReadAsAsync<List<string[]>>().Result.ToList();
            string jsn = "{\"data\":" + JsonConvert.SerializeObject(result) + "}";
            System.Diagnostics.Debug.WriteLine(jsn);
            return jsn;
        }

        public ActionResult InformeActasVencidas()
        {
            return PartialView();
        }

        public ActionResult GenerarInformeActasVencidas(Acta model)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var resp = _clienteReportes.GetAsync($"api/Inspecciones/GetInformeActaVencidas?fecha={model.Fecha}&usuario={usuario}").Result;
            //var resp = _clienteReportes.GetAsync("api/Inspecciones/GetInformeActaVencidas?fecha=" + model.Fecha).Result;

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var msg = resp.Content.ReadAsStringAsync().Result;
                return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
            }

            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);

            Session["InformeActasVencida.pdf"] = bytes;
            return Json(new { success = true, file = "InformeActasVencida.pdf" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFileInformeActasVencida(string file)
        {
            byte[] bytes = Session[file] as byte[];
            if (bytes == null)
                return new EmptyResult();
            Session[file] = null;

            var cd = new System.Net.Mime.ContentDisposition
            {
                Size = bytes.Length,
                FileName = file,
                Inline = true,
            };
            Response.Clear();
            Response.AppendHeader("Content-Disposition", cd.ToString());
            Response.ContentType = "application/pdf";
            Response.Buffer = true;
            Response.BinaryWrite(bytes);

            return null;
        }

        public JsonResult InicializarDomicilios()
        {
            Domicilios = new List<DomicilioModel>();
            return Json(JsonConvert.SerializeObject("Ok"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostGuardarDomicilio(DomicilioModel domicilio)
        {
            domicilio.UsuarioModifId = domicilio.UsuarioAltaId;
            domicilio.FechaModif = domicilio.FechaAlta;
            if (Session["Actas_Domicilios"] == null)
            {
                HttpContext.Session["Actas_Domicilios"] = new List<DomicilioModel> { domicilio };
            }
            else
            {
                Domicilios.Add(domicilio);
            }

            if (domicilio.DomicilioId <= 0)
            {
                return new JsonResult { Data = "Ok" };
            }
            else
            {
                return Json(JsonConvert.SerializeObject(domicilio), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemDomicilio(int id)
        {
            var dom = Domicilios.FirstOrDefault(d => d.DomicilioId == id);
            if (dom != null)
            {
                Domicilios.Remove(dom);
            }
            return new JsonResult { Data = "Ok" };
        }

        [HttpGet]
        public ActionResult _ObjetosActaH(Acta ActaTipo)
        {
            List<string> mActas = new List<string>();

            HttpResponseMessage resp = cliente.GetAsync("api/ActasService/PostActaBuscar?actaPost =" + ActaTipo/*?ID_Subtipo_Objeto=" + pIdSubTipo*/).Result;
            resp.EnsureSuccessStatusCode();

            DataSet dataSet = new DataSet("Actas");

            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Columns.Count > 0)
            {
                foreach (DataColumn mColumn in dataSet.Tables[0].Columns)
                {
                    mActas.Add(mColumn.ColumnName);
                }
            }
            Session["actas"] = mActas;
            return Json(mActas, JsonRequestBehavior.AllowGet);
            //return PartialView("_ObjetosInfraestructuraH");
        }

        public JsonResult AddUnidadTributariaDomicilios(long id)
        {
            var result = cliente.GetAsync("api/unidadtributaria/UnidadTributariaDom?idUT=" + id).Result;
            result.EnsureSuccessStatusCode();
            var listadoUT = result.Content.ReadAsAsync<List<UnidadTributariaDomicilio>>().Result;

            var domicilios = listadoUT.Select(ut =>
            {
                result = cliente.GetAsync("api/Domicilio/Get?id=" + ut.DomicilioID).Result;
                if (result.IsSuccessStatusCode)
                {
                    return result.Content.ReadAsAsync<DomicilioModel>().Result;
                }
                return null;
            }).Where(dom => dom != null);

            return Json(JsonConvert.SerializeObject(domicilios), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDomiciliosUTbyId(long id)
        {
            UnidadTributariaDomicilio utd = new UnidadTributariaDomicilio();
            try
            {
                var result = cliente.GetAsync("api/UnidadTributaria/GetActaDomiciliosUTbyId?idDom=" + id).Result;
                result.EnsureSuccessStatusCode();
                utd = result.Content.ReadAsAsync<UnidadTributariaDomicilio>().Result;
            }
            catch (Exception)
            {

            }

            return Json(JsonConvert.SerializeObject(utd), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActaTipos()
        {
            var resp = cliente.GetAsync("api/ActaService/GetActasTipos").Result;
            resp.EnsureSuccessStatusCode();
            var ActasTipos = (List<ActaTipo>)resp.Content.ReadAsAsync<IEnumerable<ActaTipo>>().Result;

            return Json(JsonConvert.SerializeObject(ActasTipos), JsonRequestBehavior.AllowGet);
        }
    }
}