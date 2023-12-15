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
    public class SuperficieController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }
        public SuperficieController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public void ClearId()
        {
        }

        public string List(long id)
        {
            ExpedienteObraId = id;
            var result = _cliente.GetAsync("api/superficieexpedienteobra/get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var tipoSuperficieExpedienteObras = result.Content.ReadAsAsync<IEnumerable<TipoSuperficieExpedienteObra>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(tipoSuperficieExpedienteObras) + "}";
        }

        public ActionResult FormContent()
        {
            var result = _cliente.GetAsync("api/tiposuperficie/get").Result;
            result.EnsureSuccessStatusCode();
            var tipos = result.Content.ReadAsAsync<IEnumerable<TipoSuperficie>>().Result;

            result = _cliente.GetAsync("api/destino/get").Result;
            result.EnsureSuccessStatusCode();
            var destinos = result.Content.ReadAsAsync<IEnumerable<Destino>>().Result;

            var superficie = new SuperficieViewModel
            {
                TipoList = new SelectList(tipos, "TipoSuperficieId", "Descripcion"),
                DestinoList = new SelectList(destinos, "DestinoId", "Descripcion")
            };

            return PartialView("~/Views/ExpedientesObras/Partial/_Superficie.cshtml", superficie);
        }

        public JsonResult Save(SuperficieViewModel superficieViewModel)
        {
            var tipoSuperficieExpedienteObra = new TipoSuperficieExpedienteObra
            {
                ExpedienteObraSuperficieId = superficieViewModel.ExpedienteObraSuperficieId,
                ExpedienteObraId = ExpedienteObraId,
                TipoSuperficieId = superficieViewModel.TipoSuperficieId,
                DestinoId = superficieViewModel.DestinoId,
                Fecha = DateTime.Parse(superficieViewModel.Fecha),
                Superficie = superficieViewModel.Superficie,
                CantidadPlantas = superficieViewModel.CantidadPlantas,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now
            };

            var exists = tipoSuperficieExpedienteObra.ExpedienteObraSuperficieId > 0 || 
                         UnidadExpedienteObra.OperacionesSuperficies.Exists(o => o.Item.ExpedienteObraSuperficieId == tipoSuperficieExpedienteObra.ExpedienteObraSuperficieId);
            UnidadExpedienteObra.OperacionesSuperficies.Add(new OperationItem<TipoSuperficieExpedienteObra> { Operation = (exists ? Operation.Update : Operation.Add), Item = tipoSuperficieExpedienteObra });

            return new JsonResult { Data = "Ok" };

        }

        public JsonResult Delete(long idExpedienteObraSuperficie)
        {
            var tipoSuperficie = new TipoSuperficieExpedienteObra { ExpedienteObraSuperficieId = idExpedienteObraSuperficie };
            var isnew = idExpedienteObraSuperficie < 0;
            if (!isnew)
            {

                var result = _cliente.GetAsync("api/SuperficieExpedienteObra/GetById/" + idExpedienteObraSuperficie).Result;
                result.EnsureSuccessStatusCode();
                tipoSuperficie = result.Content.ReadAsAsync<TipoSuperficieExpedienteObra>().Result;
            }

            UnidadExpedienteObra.OperacionesSuperficies.Add(new OperationItem<TipoSuperficieExpedienteObra> { Operation = Operation.Remove, Item = tipoSuperficie });

            return new JsonResult { Data = "Ok" };

        }
    }
}