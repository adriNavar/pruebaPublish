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
    public class TipoTramiteController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();

        private const string ApiUri = "api/tipoexpedienteobra/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }
        public TipoTramiteController()
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
            var tipoExpedienteObras = result.Content.ReadAsAsync<IEnumerable<TipoExpedienteObra>>().Result;

            return JsonConvert.SerializeObject(tipoExpedienteObras);
        }

        public ActionResult FormContent()
        {
            var tipoTramiteDto = new TipoTramiteViewModel();

            var result = _cliente.GetAsync("api/tipoexpediente/get").Result;
            result.EnsureSuccessStatusCode();
            var tipoTramites = result.Content.ReadAsAsync<IEnumerable<TipoExpediente>>().Result;

            tipoTramiteDto.TipoList = new SelectList(tipoTramites, "TipoExpedienteId", "Descripcion");

            return PartialView("~/Views/ExpedientesObras/Partial/_TipoTramite.cshtml", tipoTramiteDto);
        }

        public JsonResult Save(long id)
        {
            var tipoExpedienteObra = new TipoExpedienteObra
            {
                ExpedienteObraId = ExpedienteObraId,
                TipoExpedienteId = id,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now
            };
            UnidadExpedienteObra.OperacionesTipos.Add(new OperationItem<TipoExpedienteObra> { Operation = Operation.Add, Item = tipoExpedienteObra });
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(long id)
        {
            var uri = string.Format(ApiUri + "getbyexpedienteobra?idExpedienteObra={0}&idTipoExpediente={1}", ExpedienteObraId, id);
            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var defaultTEO = new TipoExpedienteObra
            {
                ExpedienteObraId = ExpedienteObraId,
                TipoExpedienteId = id,
                FechaBaja = DateTime.Now
            };
            var existente = result.Content.ReadAsAsync<TipoExpedienteObra>().Result;
            UnidadExpedienteObra.OperacionesTipos.Add(new OperationItem<TipoExpedienteObra> { Operation = Operation.Remove, Item = existente ?? defaultTEO });
            return new JsonResult();
        }
    }
}