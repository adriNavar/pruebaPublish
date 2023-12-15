using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Client.Web.Models.ResponsableFiscal;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Client.Web.Controllers
{
    public class ResponsableFiscalController : Controller
    {
        private readonly HttpClient _client = new HttpClient();

        public ResponsableFiscalController()
        {
            _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
        }

        public ActionResult FormContent(ResponsableFiscalViewModel responsableFiscalViewModel)
        {
            using (var result = _client.GetAsync("api/tipopersona/get").Result)
            {
                result.EnsureSuccessStatusCode();
                var tipoPersonas = result.Content.ReadAsAsync<IEnumerable<TipoPersona>>().Result;

                ViewData["TipoPersonaList"] = new SelectList(tipoPersonas, "TipoPersonaId", "Descripcion");

                return PartialView("ResponsableFiscal", responsableFiscalViewModel);
            }
        }
    }
}