using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Reportes.Api.Reportes;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.Certificados;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeCertificadoCatastralController : ApiController
    {
        // GET api/ExpedienteObra/
        private HttpClient cliente = new HttpClient();

        public IHttpActionResult GetInforme(int id, string usuario, long? tramite)
        {
            try
            {
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                HttpResponseMessage resp = new HttpResponseMessage();

                resp = cliente.GetAsync("api/expedienteobra/GetCertificadoCatastral/" + id).Result;
                resp.EnsureSuccessStatusCode();
                var certCat = resp.Content.ReadAsAsync<INMCertificadoCatastral>().Result;

                //filtrar domicilio real de la parsela -> tipo domicilio = 1
                certCat.UnidadTributaria.UTDomicilios = certCat.UnidadTributaria.UTDomicilios.Where(x => x.TipoDomicilioID == 1).ToList();


                //obtener los dominios y dominio vigente
                resp = cliente.GetAsync("api/Dominio/Get?idUnidadTributaria=" + certCat.UnidadTributaria.UnidadTributariaId).Result;
                resp.EnsureSuccessStatusCode();
                certCat.UnidadTributaria.Parcela.Dominios = resp.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result.ToList();


                //certificados emitidos
                resp = cliente.GetAsync("api/expedienteobra/GetCertificadosCatastral").Result;
                resp.EnsureSuccessStatusCode();
                var certificadosEmitidos = resp.Content.ReadAsAsync<IEnumerable<INMCertificadoCatastral>>().Result
                .Where(b => b.UnidadTributariaId == certCat.UnidadTributariaId).OrderBy(c => c.UnidadTributariaId).Count();

                //obtener dominio vigente
                //certCat.Dominio = dominios.OrderByDescending(x => x.Fecha).ToList();

                //valuaciones
                resp = cliente.GetAsync("api/DeclaracionJurada/GetValuacionVigente?idUnidadTributaria=" + certCat.UnidadTributaria.UnidadTributariaId).Result;
                resp.EnsureSuccessStatusCode();
                certCat.UnidadTributaria.UTValuaciones = resp.Content.ReadAsAsync<VALValuacion>().Result;
                //certCat.UnidadTributaria.UTValuaciones = valuacionList.Where(a => a.FechaHasta == null).FirstOrDefault() ?? new VALValuacion();
                certCat.UnidadTributaria.UTValuaciones.ValorTotal = certCat.UnidadTributaria.UTValuaciones.ValorMejoras + certCat.UnidadTributaria.UTValuaciones.ValorTierra ?? 0;

                //dominio titular
                //resp = cliente.GetAsync("api/dominiotitular/Get/" + certCat.Dominio.DominioID).Result;
                //resp.EnsureSuccessStatusCode();
                //certCat.DominioTitular = resp.Content.ReadAsAsync<IEnumerable<DominioTitular>>().Result.ToList();


                //MENSURA - > crear mapper mensura ¿??
                //DeclaracionJurada/GetMensura
                resp = cliente.GetAsync("api/DeclaracionJurada/GetMensura?idMensura=" + certCat.MensuraId).Result;
                resp.EnsureSuccessStatusCode();
                certCat.MensuraDesc = resp.Content.ReadAsAsync<Mensura>().Result.Descripcion;

                //DeclaracionJurada/GetMensura
                /*if(certCat.MensuraVepId.HasValue)
                {
                    resp = cliente.GetAsync("api/DeclaracionJurada/GetMensura?idMensura=" + certCat.MensuraVepId).Result;
                    resp.EnsureSuccessStatusCode();
                    certCat.MensuraVepDesc = resp.Content.ReadAsAsync<Mensura>().Result.Descripcion;
                }*/
                
                
                //Designacion
                resp = cliente.GetAsync("api/Designacion/GetDesignacion?idParcela=" + certCat.UnidadTributaria.ParcelaID).Result;
                resp.EnsureSuccessStatusCode();
                certCat.Designacion = resp.Content.ReadAsAsync<Designacion>().Result;

                byte[] bytes = ReporteHelper.GenerarReporte(new InformeCertificadoCatastral(), certCat, certificadosEmitidos, usuario, tramite);
                return Ok(Convert.ToBase64String(bytes));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
