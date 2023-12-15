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
    public class InformeSituacionPartidaInmobiliariaController : ApiController
    {
        public IHttpActionResult GetInformeSituacionPartidaInmobiliaria(long idParcelaOperacion, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                var response = client.GetAsync($"api/Parcela/GetParcelaDatos?idParcelaOperacion={idParcelaOperacion}").Result;
                response.EnsureSuccessStatusCode();
                Parcela parcela = response.Content.ReadAsAsync<Parcela>().Result;

                response = client.GetAsync($"api/Parcela/GetParcelaOperacionesOrigen?idParcelaOperacion={idParcelaOperacion}").Result;
                response.EnsureSuccessStatusCode();
                List<ParcelaOperacion> parcelasOrigen = response.Content.ReadAsAsync<List<ParcelaOperacion>>().Result;

                response = client.GetAsync($"api/Parcela/GetParcelaOperacionesDestino?idParcelaOperacion={idParcelaOperacion}").Result;
                response.EnsureSuccessStatusCode();
                List<ParcelaOperacion> parcelasDestino = response.Content.ReadAsAsync<List<ParcelaOperacion>>().Result;

                long? idTramite = null;
                METramite tramite = new METramite();
                if (idTramite != null)
                {
                    response = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                    response.EnsureSuccessStatusCode();
                    tramite = (METramite)response.Content.ReadAsAsync<METramite>().Result;
                }

                byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeSituacionPartidaInmobiliaria(), parcela, parcelasOrigen, parcelasDestino, tramite.Numero, usuario);
                return Ok(Convert.ToBase64String(bytes));
            }
        }
    }
}
