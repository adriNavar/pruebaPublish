using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
    public class EstadoController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/estadoexpedienteobra/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }
        public EstadoController()
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
            var estadoExpedienteObras = result.Content.ReadAsAsync<IEnumerable<EstadoExpedienteObra>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(estadoExpedienteObras) + "}";
        }

        public ActionResult FormContent()
        {
            var estado = new EstadoViewModel();

            var result = _cliente.GetAsync("api/estadoexpediente/get").Result;
            result.EnsureSuccessStatusCode();
            var estados = result.Content.ReadAsAsync<IEnumerable<EstadoExpediente>>().Result;

            estado.EstadoList = new SelectList(estados, "EstadoExpedienteId", "Descripcion");

            return PartialView("~/Views/ExpedientesObras/Partial/_Estado.cshtml", estado);
        }

        public ActionResult Save(EstadoViewModel estadoViewModel)
        {
            var fecha = DateTime.Now;

            var uri = string.Format("api/estadoexpediente/getestado?idExpedienteObra={0}&idEstado={1}&fechaEstado={2}",
                ExpedienteObraId, estadoViewModel.EstadoExpedienteId, estadoViewModel.Fecha);
            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var eeo = result.Content.ReadAsAsync<EstadoExpedienteObra>().Result;

            if (eeo != null)
            {
                eeo.Observaciones = estadoViewModel.Observaciones;
                eeo.UsuarioModificacionId = 1;
                eeo.FechaModificacion = fecha;

                UnidadExpedienteObra.OperacionesEstados.Add(new OperationItem<EstadoExpedienteObra> { Operation = Operation.Update, Item = eeo });

                return new JsonResult { Data = "Ok" };
            }

            UnidadExpedienteObra.OperacionesEstados.Add(new OperationItem<EstadoExpedienteObra>
            {
                Operation = Operation.Add,
                Item = new EstadoExpedienteObra
                {
                    ExpedienteObraId = ExpedienteObraId,
                    EstadoExpedienteId = estadoViewModel.EstadoExpedienteId,
                    Fecha = fecha,
                    Observaciones = estadoViewModel.Observaciones,
                    UsuarioAltaId = 1,
                    FechaAlta = fecha,
                    UsuarioModificacionId = 1,
                    FechaModificacion = fecha
                }
            });

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Delete(long idEstadoExpediente)
        {
            return new JsonResult();
        }
    }
}