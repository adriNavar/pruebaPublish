using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Client.Web.Models.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class ObservacionController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/observacion/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }
        public ObservacionController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public void ClearId()
        {
        }

        public string List(long id)
        {
            ExpedienteObraId = id;
            var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var controles = result.Content.ReadAsAsync<IEnumerable<ObservacionExpediente>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(controles) + "}";
        }

        public ActionResult FormContent()
        {
            return PartialView("~/Views/ExpedientesObras/Partial/_Observacion.cshtml");
        }

        public JsonResult Save(ObservacionViewModel observacionViewModel)
        {
            var observacionExpediente = new ObservacionExpediente
            {
                ObservacionExpedienteId = observacionViewModel.ObservacionExpedienteId,
                ExpedienteObraId = ExpedienteObraId,
                Fecha = DateTime.Parse(observacionViewModel.Fecha),
                Observaciones = observacionViewModel.Observaciones,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now

            };
            var exists = observacionExpediente.ObservacionExpedienteId > 0 ||
                         UnidadExpedienteObra.OperacionesObservaciones.Exists(o => o.Item.ObservacionExpedienteId == observacionExpediente.ObservacionExpedienteId);
            UnidadExpedienteObra.OperacionesObservaciones.Add(new OperationItem<ObservacionExpediente> { Operation = (exists ? Operation.Update : Operation.Add), Item = observacionExpediente });

            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(long idObservacionExpediente)
        {
            var observacion = new ObservacionExpediente { ObservacionExpedienteId = idObservacionExpediente };
            var isnew = idObservacionExpediente < 0;
            if (!isnew)
            {
                var result = _cliente.GetAsync(ApiUri + "getobservacionbyid/" + idObservacionExpediente).Result;
                result.EnsureSuccessStatusCode();
                observacion = result.Content.ReadAsAsync<ObservacionExpediente>().Result;
            }

            UnidadExpedienteObra.OperacionesObservaciones.Add(new OperationItem<ObservacionExpediente> { Operation = Operation.Remove, Item = observacion });

            return new JsonResult { Data = "Ok" };
        }
    }
}