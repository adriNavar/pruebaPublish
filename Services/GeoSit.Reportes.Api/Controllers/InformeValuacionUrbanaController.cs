using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using System.Diagnostics;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Reportes.Api.Reportes;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Designaciones;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeValuacionUrbanaController : ApiController
    {
        public IHttpActionResult Get(long idUnidadTributaria, long? idTramite, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                try
                {

                    var result = client.GetAsync("api/UnidadTributaria/Get/" + idUnidadTributaria).Result;
                    result.EnsureSuccessStatusCode();
                    var unidadTributaria = result.Content.ReadAsAsync<UnidadTributaria>().Result;

                    /*----------------------------------------------------------------------------*/
                    result = client.GetAsync("api/Parcela/Get/" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela = result.Content.ReadAsAsync<Parcela>().Result;

                    //Superficies
                    result = client.GetAsync($"api/parcela/{unidadTributaria.ParcelaID}/superficies/").Result;
                    result.EnsureSuccessStatusCode();
                    var parcelaSuperficies = result.Content.ReadAsAsync<ParcelaSuperficies>().Result;

                    result = client.GetAsync("api/parcela/getparcelasorigen?idparceladestino=" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.ParcelaOrigenes = result.Content.ReadAsAsync<IEnumerable<ParcelaOrigen>>().Result;

                    //valuaciones
                    result = client.GetAsync("api/DeclaracionJurada/GetValuacionVigente?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.UTValuaciones = result.Content.ReadAsAsync<VALValuacion>().Result;

                    //DDJJs
                    result = client.GetAsync("api/DeclaracionJurada/GetDeclaracionJuradaVigenteU?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.DeclaracionJ = result.Content.ReadAsAsync<DDJJ>().Result;

                    //parte tramite para que se muestre el numero y no el id en el reporte
                    METramite tramite = new METramite();
                    if (idTramite != null && idTramite > 0)
                    { 
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = (METramite)result.Content.ReadAsAsync<METramite>().Result;
                    }

                    //Designacion
                    result = client.GetAsync("api/Designacion/GetDesignacion?idParcela=" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Designacion = result.Content.ReadAsAsync<Designacion>().Result;

                    
                    byte[] bytes = ReporteHelper.GenerarReporte(new InformeValuacionUrbana(), unidadTributaria, parcelaSuperficies, usuario, tramite.Numero);
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
