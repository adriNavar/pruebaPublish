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
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class LiquidacionController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/liquidacion/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }
        public LiquidacionController()
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
            var liquidaciones = result.Content.ReadAsAsync<IEnumerable<Liquidacion>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(liquidaciones) + "}";
        }

        public ActionResult FormContent()
        {
            ParametrosGenerales pg = GetParametrosGeneralesByNombre("LiquidacionesExternas");
            if (pg != null && pg.Valor == "1")
            {
                
                //List<SelectListItem> items = new List<SelectListItem>();
                //items.Add(new SelectListItem { Text = "PAGO", Value = "PAGO" });

                //items.Add(new SelectListItem { Text = "CANC", Value = "CANC" });

                //items.Add(new SelectListItem { Text = "IMP", Value = "IMP"});

                //items.Add(new SelectListItem { Text = "CONV", Value = "CONV" });
                //SelectList Estados = new SelectList(items, "value", "text");
                
                //ViewBag.Estado = Estados ;
                    
                return PartialView("~/Views/ExpedientesObras/Partial/_LiquidacionExterno.cshtml");
            }
            else
            {
                return PartialView("~/Views/ExpedientesObras/Partial/_Liquidacion.cshtml");
            }
        }

        public JsonResult Save(LiquidacionViewModel liquidacionViewModel)
        {
            var liquidacion = new Liquidacion
            {
                LiquidacionId = liquidacionViewModel.LiquidacionId,
                ExpedienteObraId = ExpedienteObraId,
                Fecha = DateTime.Parse(liquidacionViewModel.Fecha),
                Numero = liquidacionViewModel.Numero,
                Importe = liquidacionViewModel.Importe,
                Observaciones = liquidacionViewModel.Observaciones,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now
            };

            var exists = liquidacion.LiquidacionId > 0 ||
                         UnidadExpedienteObra.OperacionesLiquidaciones.Exists(o => o.Item.LiquidacionId == liquidacion.LiquidacionId);
            UnidadExpedienteObra.OperacionesLiquidaciones.Add(new OperationItem<Liquidacion> { Operation = (exists ? Operation.Update : Operation.Add), Item = liquidacion });


            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(long idLiquidacion)
        {
            var liquidacion = new Liquidacion { LiquidacionId = idLiquidacion };
            var isnew = idLiquidacion < 0;
            if (!isnew)
            {
                var result = _cliente.GetAsync(ApiUri + "getliquidacionbyid/" + idLiquidacion).Result;
                result.EnsureSuccessStatusCode();
                liquidacion = result.Content.ReadAsAsync<Liquidacion>().Result;
            }

            UnidadExpedienteObra.OperacionesLiquidaciones.Add(new OperationItem<Liquidacion> { Operation = Operation.Remove, Item = liquidacion });

            return new JsonResult { Data = "Ok" };
        }
        public ParametrosGenerales GetParametrosGeneralesByNombre(string nombre)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Liquidacion/GetParametrosGeneralesByNombre/" + nombre).Result;
            resp.EnsureSuccessStatusCode();
            return (ParametrosGenerales)resp.Content.ReadAsAsync<ParametrosGenerales>().Result;
        }
    }
}