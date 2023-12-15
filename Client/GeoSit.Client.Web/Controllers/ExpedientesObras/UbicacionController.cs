using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class UbicacionController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();

        private const string ApiUri = "api/domicilioexpedienteobra/";

        private long ExpedienteObraId
        {
            get { return Convert.ToInt64(Session["ExpedienteObraId"]); } 
            set { Session["ExpedienteObraId"] = value; }
        }

        private UnidadExpedienteObra UnidadExpedienteObra
        {
            get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; }
        }

        public UbicacionController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public void ClearId()
        {
        }

        public ActionResult List(long id)
        {
            ExpedienteObraId = id;
            var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var ubicaciones = result.Content.ReadAsAsync<IEnumerable<UbicacionExpedienteObra>>().Result;

            return PartialView("~/Views/ExpedientesObras/Partial/_ListaUbicaciones.cshtml", ubicaciones);
        }

        public ActionResult FormContent()
        {
            return PartialView("~/Views/ExpedientesObras/Partial/_Ubicacion.cshtml");
        }

        public ActionResult Save(long idDomicilioInmueble)
        {
            var domicilioInmuebleExpedienteObra = new DomicilioExpedienteObra
            {
                DomicilioInmuebleId = idDomicilioInmueble,
                ExpedienteObraId = ExpedienteObraId,
                Primario = false,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now
            };
            UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra> { Operation = Operation.Add, Item = domicilioInmuebleExpedienteObra });
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(int id)
        {
            //var uri = string.Format(ApiUri + "validate?idDomicilio={0}", id);
            //var result = _cliente.PostAsync(uri, new ObjectContent<Operaciones<UnidadTributariaExpedienteObra>>(UnidadExpedienteObra.OperacionesUnidadesTributarias, new JsonMediaTypeFormatter()))
            //                     .Result;
            //result.EnsureSuccessStatusCode();
            //var response = result.Content.ReadAsStringAsync().Result;
            //if (string.IsNullOrEmpty(response))
            //{
            //    var defaultDEO = new DomicilioExpedienteObra { ExpedienteObraId = ExpedienteObraId, DomicilioInmuebleId = id };
            //    uri = string.Format(ApiUri + "getbyexpedienteobra?idDomicilio={0}&idExpedienteObra={1}", id, ExpedienteObraId);
            //    result.EnsureSuccessStatusCode();
            //    var existente = result.Content.ReadAsAsync<DomicilioExpedienteObra>().Result;
            //    UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra> { Operation = Operation.Remove, Item = existente ?? defaultDEO });
            //    return new JsonResult { Data = "Ok" };
            //}
            //else
            //{
            //    return new JsonResult { Data = response };
            //}

            var defaultDEO = new DomicilioExpedienteObra { ExpedienteObraId = ExpedienteObraId, DomicilioInmuebleId = id };
            UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra> { Operation = Operation.Remove, Item = defaultDEO });
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Primary(long id)
        {
            #region Degradar Primario Actual
            var currentMemoryPrimario = UnidadExpedienteObra.OperacionesDomicilios.Find(dom => dom.Item.Primario);
            if (currentMemoryPrimario != null)
            {
                currentMemoryPrimario.Item.Primario = false;
                UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra>
                {
                    Operation = Operation.Update,
                    Item = currentMemoryPrimario.Item
                });
            }
            else
            {
                if (ExpedienteObraId > 0)
                {
                    var uri = string.Format(ApiUri + "getbyexpedienteobraprimario?idExpedienteObra={0}",
                        ExpedienteObraId);
                    var result = _cliente.GetAsync(uri).Result;
                    result.EnsureSuccessStatusCode();
                    var currentDbPrimario = result.Content.ReadAsAsync<DomicilioExpedienteObra>().Result;
                    if (currentDbPrimario != null)
                    {
                        currentDbPrimario.Primario = false;
                        UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra>
                        {
                            Operation = Operation.Update,
                            Item = currentDbPrimario
                        });
                    }
                }
            }
            #endregion

            
            #region Promover Nuevo Primario
            var newMemoryPrimario = UnidadExpedienteObra.OperacionesDomicilios.Find(dom => dom.Item.DomicilioInmuebleId == id);
            if (newMemoryPrimario != null)
            {
                newMemoryPrimario.Item.Primario = true;
                UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra>
                {
                    Operation = Operation.Update,
                    Item = newMemoryPrimario.Item
                });
            }
            else
            {
                if (ExpedienteObraId <= 0) return new JsonResult { Data = "Ok" };
                var uri = string.Format(ApiUri + "getbyexpedienteobra?idExpedienteObra={0}&idDomicilio={1}",
                    ExpedienteObraId, id);
                var result = _cliente.GetAsync(uri).Result;
                result.EnsureSuccessStatusCode();
                var newDbPrimario = result.Content.ReadAsAsync<DomicilioExpedienteObra>().Result;
                if (newDbPrimario == null) return new JsonResult { Data = "Ok" };
                newDbPrimario.Primario = true;
                UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra>
                {
                    Operation = Operation.Update,
                    Item = newDbPrimario
                });
            }
            #endregion
            return new JsonResult { Data = "Ok" };
        }
    }
}