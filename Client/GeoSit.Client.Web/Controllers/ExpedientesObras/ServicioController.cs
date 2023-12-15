using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Client.Web.Models.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class ServicioController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/servicioexpedienteobra/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }

        public ServicioController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public void ClearId()
        {
        }

        public string List(int id)
        {
            ExpedienteObraId = id;
            var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var servicioExpedienteObras = result.Content.ReadAsAsync<IEnumerable<ServicioExpedienteObra>>().Result;

            return JsonConvert.SerializeObject(servicioExpedienteObras);
        }

        public ActionResult FormContent()
        {
            var servicio = new ServicioViewModel();

            var result = _cliente.GetAsync("api/servicio/get").Result;
            result.EnsureSuccessStatusCode();
            var servicios = result.Content.ReadAsAsync<IEnumerable<Servicio>>().Result;

            servicio.ServicioList = new SelectList(servicios, "ServicioId", "Nombre");

            return PartialView("~/Views/ExpedientesObras/Partial/_Servicio.cshtml", servicio);
        }

        public JsonResult Save(long id)
        {
            var servicioExpedienteObra = new ServicioExpedienteObra
            {
                ExpedienteObraId = ExpedienteObraId,
                ServicioId = id,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now
            };

            UnidadExpedienteObra.OperacionesServicios.Add(new OperationItem<ServicioExpedienteObra> { Operation = Operation.Add, Item = servicioExpedienteObra });
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(long id)
        {
            var servicioDefault = new ServicioExpedienteObra { ExpedienteObraId = ExpedienteObraId, ServicioId = id };
            var uri = string.Format(ApiUri + "getserviciobyexpedienteobra?idExpedienteObra={0}&idServicioExpediente={1}", ExpedienteObraId, id);
            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var existente = result.Content.ReadAsAsync<ServicioExpedienteObra>().Result;
            UnidadExpedienteObra.OperacionesServicios.Add(new OperationItem<ServicioExpedienteObra> { Operation = Operation.Remove, Item = existente ?? servicioDefault });
            return new JsonResult();
        }
    }
}

