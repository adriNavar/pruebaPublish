using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.Inmuebles;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class DomicilioExpedienteObraController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();

        public DomicilioExpedienteObraController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public string Search(string nombreVia)
        {
            var result = _cliente.GetAsync("api/domicilio/get?nombreVia=" + nombreVia).Result;
            result.EnsureSuccessStatusCode();
            var domicilios = result.Content.ReadAsAsync<IEnumerable<Domicilio>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(domicilios) + "}";
        }

        public JsonResult GetDomicilio(long id)
        {
            var result = _cliente.GetAsync("api/domicilio/get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var domicilio = result.Content.ReadAsAsync<Domicilio>().Result;

            return Json(domicilio);
        }
    }
}