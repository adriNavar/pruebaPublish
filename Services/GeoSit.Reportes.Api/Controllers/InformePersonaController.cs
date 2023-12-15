using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Reportes.Api.Helpers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformePersonaController : ApiController
    {
        public IHttpActionResult Get(long idPersona, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                var response = client.GetAsync($"api/Persona/GetDatos?id={idPersona}").Result;
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                return Ok(ReporteHelper.GenerarReporte(new Reportes.InformePersona(), response.Content.ReadAsAsync<Persona>().Result, usuario));
            }
        }
    }
}
