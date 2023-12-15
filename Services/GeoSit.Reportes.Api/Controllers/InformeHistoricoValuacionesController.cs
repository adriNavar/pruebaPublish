using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Reportes.Api.Reportes;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Designaciones;


namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeHistoricoValuacionesController : ApiController
    {
        public IHttpActionResult Get(long idUnidadTributaria, string usuario, long? idTramite)
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

                    result = client.GetAsync("api/parcela/getparcelasorigen?idparceladestino=" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.ParcelaOrigenes = result.Content.ReadAsAsync<IEnumerable<ParcelaOrigen>>().Result;

                    //valuacionesHistoricas
                    result = client.GetAsync("api/DeclaracionJurada/GetValuacionesHistoricas?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.UTValuacionesHistoricas = result.Content.ReadAsAsync<List<VALValuacion>>().Result;

                    //ValuacionVigente
                    result = client.GetAsync("api/DeclaracionJurada/GetValuacionVigente?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.UTValuaciones = result.Content.ReadAsAsync<VALValuacion>().Result;

                    //Expedientes
                    result = client.GetAsync("api/MesaEntradas/GetExpedientesDDJJByIdUnidadTributaria?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    var expedientesDdjj = result.Content.ReadAsAsync<Dictionary<long, string>>().Result;
                    

                    //parte tramite para que se muestre el numero y no el id en el reporte
                    METramite tramite = new METramite();
                    if (idTramite != null && idTramite > 0)
                    { 
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = (METramite)result.Content.ReadAsAsync<METramite>().Result;
                    }

                    //var bytes = ReporteHelper.GenerarReporte(reporte, unidadTributaria);
                    long? idValuacion = unidadTributaria.UTValuaciones?.IdValuacion; //si la ut no tiene valuacion vigente no da error
                    string expediente = null;
                    if(idValuacion.HasValue && expedientesDdjj.ContainsKey(idValuacion.Value))
                    {
                        expediente = expedientesDdjj[idValuacion.Value];
                    }
                    byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeHistoricoValuaciones(expedientesDdjj), unidadTributaria, usuario, tramite.Numero, expediente);
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
