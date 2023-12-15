using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using Newtonsoft.Json;
using GeoSit.Client.Web.Models;
using System.Linq;
using GeoSit.Client.Web.Models.Inspecciones;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class InspeccionController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private readonly HttpClient _clienteReportes = new HttpClient();

        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }

        public InspeccionController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            _clienteReportes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        public void ClearId()
        {
        }

        public string List(long id)
        {
            ExpedienteObraId = id;

            var result = _cliente.GetAsync("api/inspeccion/get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var inspecciones = result.Content.ReadAsAsync<IEnumerable<InspeccionInspector>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(inspecciones) + "}";
            //return PartialView("~/Views/ExpedientesObras/Partial/_ListaInspecciones.cshtml", inspecciones);
        }

        public ActionResult Save(long id)
        {
            var inspeccionEO = new InspeccionExpedienteObra
            {
                ExpedienteObraId = ExpedienteObraId,
                InspeccionId = id,
                UsuarioAltaId = 1,
                UsuarioModificacionId = 1,
            };

            UnidadExpedienteObra.OperacionesInspecciones.Add(new OperationItem<InspeccionExpedienteObra> { Operation = Operation.Add, Item = inspeccionEO });

            var result = _cliente.GetAsync("api/obrasparticularesservice/getinspeccion?inspeccion=" + id).Result;
            result.EnsureSuccessStatusCode();
            var inspeccion = result.Content.ReadAsAsync<Inspeccion>().Result;

            result = _cliente.GetAsync("api/obrasparticularesservice/getinspector?inspector=" + inspeccion.InspectorID).Result;
            result.EnsureSuccessStatusCode();
            var inspector = result.Content.ReadAsAsync<Inspector>().Result;

            var inspeccionInspector = new InspeccionInspector
            {
                Tipo = inspeccion.TipoInspeccion.Descripcion,
                InspeccionID = inspeccion.InspeccionID,
                Fecha = inspeccion.FechaHoraInicio,
                Observaciones = inspeccion.ResultadoInspeccion,
                Inspector = string.Format("{0} {1}", inspector.Usuario.Nombre, inspector.Usuario.Apellido)
            };
            return new JsonResult { Data = new { inspeccion = JsonConvert.SerializeObject(inspeccionInspector), result = "Ok" } };
        }

        public ActionResult InformeInspeccionesPorPeriodo()
        {
            using (var resp = _cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result)
            {
                return PartialView(resp.Content.ReadAsAsync<IEnumerable<Inspector>>().Result);
            }
        }

        public ActionResult GenerateInformeInspeccionesPorPeriodo(InformeInspeccionModel model)
        {
            if (string.IsNullOrEmpty(model.FechaDesde))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "El parámetro [fechaDesde] es inválido.");
            }

            if (string.IsNullOrEmpty(model.FechaHasta))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "El parámetro [fechaHasta] es inválido.");
            }

            if (DateTime.TryParse(model.FechaDesde, out DateTime dtFrom) &&
                DateTime.TryParse(model.FechaHasta, out DateTime dtTo) &&
                dtTo < dtFrom)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "El parámetro [fechaDesde] debe ser menor al parámetro [fechaHasta].");
            }

            if (model.Inspectores == null || !model.Inspectores.Any())
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Debe seleccionar al menos un inspector.");
            }

            var data = new List<string>
            {
                model.FechaDesde,
                model.FechaHasta
            };
            data.AddRange(model.Inspectores.Select(i => i.ToString()));

            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var resp = _clienteReportes.PostAsJsonAsync($"api/Inspecciones/GetInformePorPeriodo/?usuario={usuario}", data.ToArray()).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(resp.StatusCode, resp.Content.ReadAsStringAsync().Result);
                }
                Session["InspeccionesPorPeriodo.pdf"] = Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result);
                return Json(new { file = "InspeccionesPorPeriodo.pdf" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFileInformeInspeccionesPorPeriodo(string file)
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

        public ActionResult InformeInspeccionesPorTipo()
        {
            using (var resp = _cliente.GetAsync("api/ObrasParticularesService/GetTiposInspecciones").Result)
            {
                return PartialView(resp.Content.ReadAsAsync<IEnumerable<TipoInspeccion>>().Result);
            }
        }

        public ActionResult GenerateInformeInspeccionesPorTipo(InformeInspeccionModel model)
        {
            var errores = new List<string>();
            if (string.IsNullOrEmpty(model.FechaDesde))
            {
                errores.Add("El parámetro [fechaDesde] es inválido.");
            }

            if (string.IsNullOrEmpty(model.FechaHasta))
            {
                errores.Add("El parámetro [fechaHasta] es inválido.");
            }

            if (DateTime.TryParse(model.FechaDesde, out DateTime dtFrom) &&
                DateTime.TryParse(model.FechaHasta, out DateTime dtTo) &&
                dtTo < dtFrom)
            {
                errores.Add("El parámetro [fechaDesde] debe ser menor al parámetro [fechaHasta].");
            }

            if (model.Tipos == null || !model.Tipos.Any())
            {
                errores.Add("Debe seleccionar al menos un tipo de inspección.");
            }
            if (errores.Any())
            {
                return Json(new { error = true, mensajes = errores }, JsonRequestBehavior.AllowGet);
            }

            var data = new List<string>
            {
                model.FechaDesde,
                model.FechaHasta
            };
            data.AddRange(model.Tipos.Select(i => i.ToString()));

            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var resp = _clienteReportes.PostAsJsonAsync($"api/Inspecciones/GetInformePorTipo/?usuario={usuario}", data.ToArray()).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(resp.StatusCode);
                }
                Session["InspeccionesPorTipo.pdf"] = Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result);
                return Json(new { file = "InspeccionesPorTipo.pdf" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFileInformeInspeccionesPorTipo(string file)
        {
            byte[] bytes = Session[file] as byte[];
            if (!(bytes ?? new byte[0]).Any())
            {
                return new EmptyResult();
            }
            Session.Remove(file);

            var cd = new System.Net.Mime.ContentDisposition()
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
    }
}