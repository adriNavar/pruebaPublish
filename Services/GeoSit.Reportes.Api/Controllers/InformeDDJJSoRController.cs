using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeDDJJSoRController : ApiController
    {
        public IHttpActionResult Get(long idDeclaracionJurada, long? idTramite, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                try
                {
                    var result = client.GetAsync("api/UnidadTributaria/GetByIdDeclaracionJurada/" + idDeclaracionJurada).Result;
                    result.EnsureSuccessStatusCode();
                    var unidadTributaria = result.Content.ReadAsAsync<UnidadTributaria>().Result;

                    /*----------------------------------------------------------------------------*/
                    result = client.GetAsync("api/Parcela/Get/" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela = result.Content.ReadAsAsync<Parcela>().Result;

                    //valuaciones
                    /*result = client.GetAsync("api/DeclaracionJurada/GetValuacionVigente?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.UTValuaciones = result.Content.ReadAsAsync<VALValuacion>().Result;*/

                    //parte tramite para que se muestre el numero y no el id en el reporte
                    METramite tramite = new METramite();
                    if (idTramite != null && idTramite > 0)
                    {
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = result.Content.ReadAsAsync<METramite>().Result;
                    }


                    byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeDDJJSoR(), unidadTributaria, usuario, tramite.Numero);
                    return Ok(Convert.ToBase64String(bytes));
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                    return NotFound();
                }
            }
        }
    }
}
