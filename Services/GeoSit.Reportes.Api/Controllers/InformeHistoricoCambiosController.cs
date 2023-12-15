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
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeHistoricoCambiosController : ApiController
    {
        public IHttpActionResult GetCambiosParcela(long id, string identificador, long? idTramite, string usuario)
        {
            return Ok(GetCambios(id, "Parcela", identificador, idTramite, usuario));
        }
        public IHttpActionResult GetCambiosUnidadTributaria(long id, string identificador, long? idTramite, string usuario)
        {
            return Ok(GetCambios(id, "UnidadTributaria", identificador, idTramite, usuario));
        }
        private string GetCambios(long id, string Objeto, string identificador, long? idTramite, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                try
                {

                    var reporte = new InformeHistoricoCambios();

                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);

                    var result = client.GetAsync("api/Auditoria/GetAuditoriaByObjeto?idObjeto=" + id + "&Objeto=" + Objeto).Result;
                    result.EnsureSuccessStatusCode();
                    var auditorias = result.Content.ReadAsAsync<List<Auditoria>>().Result;

                    METramite tramite = new METramite();
                    if (idTramite != null && idTramite > 0)
                    {
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = result.Content.ReadAsAsync<METramite>().Result;
                    }

                    byte[] bytes = ReporteHelper.GenerarReporte(new Reportes.InformeHistoricoCambios(), auditorias, "Partida Inmobiliaria", identificador, usuario, tramite.Numero);
                    return Convert.ToBase64String(bytes);
                }
                catch (Exception ex)
                {
                    WebApiApplication.GetLogger().LogError($"InformeHistoricoCambios/GetCambios({id},{Objeto})", ex);
                    throw;
                }
            }
        }
    }
}
