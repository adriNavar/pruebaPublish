using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Client.Web.Models.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class IdentificacionController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();

        public IdentificacionController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult FormContent()
        {
            var identificacion = new IdentificacionViewModel();
            
            var result = _cliente.GetAsync("api/plan/get").Result;
            result.EnsureSuccessStatusCode();
            var planes = (List<Plan>) result.Content.ReadAsAsync<IEnumerable<Plan>>().Result;

            planes.Insert(0, new Plan { PlanId = 0, Descripcion = "(Ninguno)" });
            identificacion.PlanList = new SelectList(planes, "PlanId", "Descripcion");

            return PartialView("~/Views/ExpedientesObras/Partial/_Identificacion.cshtml", identificacion);
        }        
    }
}