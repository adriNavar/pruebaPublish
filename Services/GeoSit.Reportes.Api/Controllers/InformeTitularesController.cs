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
using GeoSit.Data.BusinessEntities.Valuaciones;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Reportes.Api.Reportes;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeTitularesController : ApiController
    {
        public IHttpActionResult Get(long idUnidadTributaria, /*long idTramite,*/ string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                try
                {
                    int? idTramite = null;

                    var result = client.GetAsync($"api/UnidadTributaria/Get/{idUnidadTributaria}").Result;
                    result.EnsureSuccessStatusCode();
                    var unidadTributaria = result.Content.ReadAsAsync<UnidadTributaria>().Result;

                    result = client.GetAsync($"api/Parcela/Get/{unidadTributaria.ParcelaID}").Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela = result.Content.ReadAsAsync<Parcela>().Result;

                    result = client.GetAsync($"api/Dominio/GetHistorico?idUnidadTributaria={idUnidadTributaria}").Result;
                    result.EnsureSuccessStatusCode();
                    var dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;
                    unidadTributaria.Parcela.Dominios = dominios;

                    METramite tramite = new METramite();
                    if (idTramite != null)
                    {
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = (METramite)result.Content.ReadAsAsync<METramite>().Result;
                    }

                    byte[] bytes = ReporteHelper.GenerarReporte(new InformeHistoricoTitulares(), unidadTributaria, usuario, tramite.Numero);
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
