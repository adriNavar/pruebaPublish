using GeoSit.Reportes.Api.Helpers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeBienesRegistradosController : ApiController
    {
        public IHttpActionResult GetInformeBienesRegistrados(long idPersona, long? idTramite, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                var response = client.GetAsync($"api/DominioTitular/GetDominioTitularByTitularId?idPersona={idPersona}").Result;
                response.EnsureSuccessStatusCode();
                List<DominioTitular> domTitular = response.Content.ReadAsAsync<List<DominioTitular>>().Result;

                METramite tramite = new METramite();
                if (idTramite != null)
                {
                    response = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                    response.EnsureSuccessStatusCode();
                    tramite = (METramite)response.Content.ReadAsAsync<METramite>().Result;
                }

                byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeBienesRegistrados(), domTitular, tramite.Numero, usuario);
                return Ok(Convert.ToBase64String(bytes));
            }
        }
    }
}
