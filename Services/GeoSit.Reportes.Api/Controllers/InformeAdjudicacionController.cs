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
using GeoSit.Data.BusinessEntities.Temporal;



namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeAdjudicacionController : ApiController
    {
        public IHttpActionResult Get(long idTramite, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                try
                {
                    var result = client.GetAsync($"api/TramitesCertificadosService/GetObjetoMensuraTramite?idTramite={idTramite}").Result;
                    result.EnsureSuccessStatusCode();
                    var tramiteMensura = result.Content.ReadAsAsync<MensuraTemporal>().Result;

                    result = client.GetAsync($"api/TramitesCertificadosService/GetObjetoTramiteUt?idTramite={idTramite}").Result;
                    result.EnsureSuccessStatusCode();
                    var tramiteUt = result.Content.ReadAsAsync<List<UnidadTributariaTramiteTemporal>>().Result;

                    METramite tramite = new METramite();
                    if (idTramite > 0)
                    {
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = result.Content.ReadAsAsync<METramite>().Result;
                    }

                    List<int> idObjetoPorcen = new List<int> { 57, 58, 61, 62 };//Código de DGCyC corrientes

                    if (idObjetoPorcen.Any(x => x == tramite.IdObjetoTramite))//Código de DGCyC corrientes
                    {
                        byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeAdjudicacionPorcen(), tramiteMensura, tramiteUt, tramite.Numero, usuario);//Código de DGCyC corrientes
                        return Ok(Convert.ToBase64String(bytes));//Código de DGCyC corrientes
                    }
                    else
                    {
                        byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeAdjudicacion(), tramiteMensura, tramiteUt, tramite.Numero, usuario);
                        return Ok(Convert.ToBase64String(bytes));
                    }

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
