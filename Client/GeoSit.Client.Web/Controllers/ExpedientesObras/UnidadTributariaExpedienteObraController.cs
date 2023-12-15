using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using Newtonsoft.Json;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class UnidadTributariaExpedienteObraController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/unidadtributariaexpedienteobra/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }

        public UnidadTributariaExpedienteObraController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public JsonResult GetUnidadTributaria(long id)
        {
            var result = _cliente.GetAsync("api/unidadtributaria/get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var unidad = result.Content.ReadAsAsync<UnidadTributaria>().Result;
            unidad.Parcela.UnidadesTributarias.Clear();
            return Json(unidad);
        }
        public void ClearId()
        {
        }
        public ActionResult List(long id)
        {
            ExpedienteObraId = id;
            var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var unidadesTributarias = result.Content.ReadAsAsync<IEnumerable<UnidadTributaria>>().Result;

            return PartialView("~/Views/ExpedientesObras/Partial/_ListaUnidadesTributarias.cshtml", unidadesTributarias);
        }

        public JsonResult Save(long idUnidadTributaria)
        {
            var unidadTributariaExpedienteObra = new UnidadTributariaExpedienteObra
            {
                ExpedienteObraId = ExpedienteObraId,
                UnidadTributariaId = idUnidadTributaria,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now
            };

            #region Validaciones
            var result = _cliente.PostAsync(ApiUri + string.Format("validate?idExpedienteObra={0}&idUnidadTributaria={1}",
                                                                    unidadTributariaExpedienteObra.ExpedienteObraId,
                                                                    unidadTributariaExpedienteObra.UnidadTributariaId),
                                            new ObjectContent<Operaciones<UnidadTributariaExpedienteObra>>(UnidadExpedienteObra.OperacionesUnidadesTributarias, new JsonMediaTypeFormatter())).Result;
            result.EnsureSuccessStatusCode();
            var response = result.Content.ReadAsStringAsync().Result.ToStringOrDefault();
            #endregion
            if (string.IsNullOrEmpty(response))
            {
                UnidadExpedienteObra.OperacionesUnidadesTributarias.Add(new OperationItem<UnidadTributariaExpedienteObra> { Operation = Operation.Add, Item = unidadTributariaExpedienteObra });
                var uri = "api/domicilio/getbyunidadtributaria/" + idUnidadTributaria;
                result = _cliente.GetAsync(uri).Result;
                result.EnsureSuccessStatusCode();
                var domicilio = result.Content.ReadAsAsync<Domicilio>().Result;

                var domicilioExpedienteObra = new DomicilioExpedienteObra
                {
                    DomicilioInmuebleId = domicilio.DomicilioId,
                    ExpedienteObraId = ExpedienteObraId,
                    Primario = false,
                    UsuarioAltaId = 1,
                    FechaAlta = DateTime.Now,
                    UsuarioModificacionId = 1,
                    FechaModificacion = DateTime.Now
                };

                UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra> { Operation = Operation.Add, Item = domicilioExpedienteObra });
                return Json(domicilio);
            }
            else
            {
                return new JsonResult { Data = response };
            }
        }

        public JsonResult Delete(long idUnidadTributaria)
        {
            var result = _cliente.GetAsync(ApiUri + string.Format("getbyexpedienteobra?idExpedienteObra={0}&idUnidadTributaria={1}",
                                                                    ExpedienteObraId, idUnidadTributaria)).Result;
            result.EnsureSuccessStatusCode();
            var unidadTributaria = result.Content.ReadAsAsync<UnidadTributariaExpedienteObra>().Result;

            if (unidadTributaria == null)
                unidadTributaria = new UnidadTributariaExpedienteObra { ExpedienteObraId = ExpedienteObraId, UnidadTributariaId = idUnidadTributaria };

            UnidadExpedienteObra.OperacionesUnidadesTributarias.Add(new OperationItem<UnidadTributariaExpedienteObra> { Operation = Operation.Remove, Item = unidadTributaria });

            return new JsonResult { Data = "Ok" };
        }

        public JsonResult DeleteDomicilio(long idUnidadTributaria)
        {
            var uri = string.Format("api/domicilio/getbyunidadtributaria/{0}", idUnidadTributaria);

            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var domicilio = result.Content.ReadAsAsync<Domicilio>().Result;

            if (domicilio == null)
            {
                return new JsonResult { Data = string.Empty };
            }
            else
            {
                uri = string.Format("api/domicilioexpedienteobra/getbyexpedienteobra?idExpedienteObra={0}&idDomicilio={1}", ExpedienteObraId, domicilio.DomicilioId);

                result = _cliente.GetAsync(uri).Result;
                result.EnsureSuccessStatusCode();
                var domicilioExpedienteObra = result.Content.ReadAsAsync<DomicilioExpedienteObra>().Result;

                if (domicilioExpedienteObra == null)
                    domicilioExpedienteObra = new DomicilioExpedienteObra { ExpedienteObraId = ExpedienteObraId, DomicilioInmuebleId = domicilio.DomicilioId };

                UnidadExpedienteObra.OperacionesDomicilios.Add(new OperationItem<DomicilioExpedienteObra> { Operation = Operation.Remove, Item = domicilioExpedienteObra });

                return new JsonResult { Data = domicilio.DomicilioId.ToString() };
            }

        }

        public JsonResult GetUnidadTributariaDomicilio(long id)
        {
            var result = _cliente.GetAsync("api/unidadtributaria/UnidadTributariaDom?idUT=" + id).Result;
            result.EnsureSuccessStatusCode();
            var unidad = result.Content.ReadAsAsync<List<UnidadTributariaDomicilio>>().Result;
            return Json(JsonConvert.SerializeObject(unidad), JsonRequestBehavior.AllowGet);
            //return new JsonResult { Data = unidad, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}