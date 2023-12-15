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
    public class InformeValuacionPhController : ApiController
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

                    //valuaciones
                    result = client.GetAsync("api/DeclaracionJurada/GetValuacionVigente?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.UTValuaciones = result.Content.ReadAsAsync<VALValuacion>().Result;

                    //DDJJs
                    result = client.GetAsync("api/DeclaracionJurada/GetDeclaracionJuradaVigenteU?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.DeclaracionJ = result.Content.ReadAsAsync<DDJJ>().Result;

                    //UF
                    result = client.GetAsync("api/UnidadTributaria/GetUF?idParcela=" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.UnidadesTributarias = result.Content.ReadAsAsync<List<UnidadTributaria>>().Result.OrderBy(x => x.Piso).ThenBy(x => x.Unidad).ToList();

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

                    
                    byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeValuacionPh(), unidadTributaria, usuario, tramite.Numero);
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
