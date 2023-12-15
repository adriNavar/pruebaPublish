using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Reportes.Api.Reportes;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeUnidadTributariaController : ApiController
    {
        private readonly HttpClient _cliente = new HttpClient();

        public InformeUnidadTributariaController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET api/informeunidadtributaria
        public IHttpActionResult GetInforme(long idParcela, long idUnidadTributaria, string usuario)
        {
            try
            {
                using (_cliente)
                {
                    var informeUnidadTributaria = new InformeUnidadTributaria();

                    var result = _cliente.GetAsync($"api/Parcela/Get/{idParcela}").Result;
                    result.EnsureSuccessStatusCode();
                    var parcelaModel = result.Content.ReadAsAsync<Parcela>().Result;

                    var unidadTributaria = parcelaModel.UnidadesTributarias
                                                       .Single(x => x.UnidadTributariaId == idUnidadTributaria);
                    parcelaModel.UnidadesTributarias.Clear();
                    parcelaModel.UnidadesTributarias.Add(unidadTributaria);

                    //Responsables Fiscales
                    var res = _cliente.GetAsync($"api/Parcela/Get/{idParcela}").Result;
                    res.EnsureSuccessStatusCode();
                    var parcela = res.Content.ReadAsAsync<Parcela>().Result;

                    var ut = _cliente.GetAsync($"api/UnidadTributaria/Get?id={idUnidadTributaria}&incluirDominios=true").Result;
                    ut.EnsureSuccessStatusCode();
                    var unidad = ut.Content.ReadAsAsync<UnidadTributaria>().Result;

                    var lblCi = informeUnidadTributaria.FindControl("lblCi", true);
                    if (!unidad.Dominios.Any() && parcela.ClaseParcelaID == 6)
                    {
                        var idUt = parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 2).FirstOrDefault().UnidadTributariaId;
                        result = _cliente.GetAsync($"api/Dominio/Get?idUnidadTributaria={idUt}").Result;
                        result.EnsureSuccessStatusCode();
                        var domi = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;
                        parcelaModel.Dominios = domi;

                        lblCi.Text = "Unidad Parcelaria sin inscripción registrada, se informa Dominio y Titular de inmueble Origen";

                    }
                    else
                    {
                        result = _cliente.GetAsync($"api/Dominio/Get?idUnidadTributaria={idUnidadTributaria}").Result;
                        result.EnsureSuccessStatusCode();
                        parcelaModel.Dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;

                        lblCi.Visible = false;

                    }

                    //Parametros Generales
                    result = _cliente.GetAsync("api/Parametro/GetParametroByClave?clave=ACTIVA_VALUACIONES").Result;
                    result.EnsureSuccessStatusCode();
                    var parametro = result.Content.ReadAsAsync<ParametrosGenerales>().Result;
                    bool activaValuaciones = parametro?.Valor == "1";
                    var detailReportValuaciones = informeUnidadTributaria.FindControl("DetailReportValuaciones", true);
                    detailReportValuaciones.Visible = activaValuaciones;
                    
                    VALValuacion valValuacion = null;
                    if (activaValuaciones)
                    {
                        //Valuacion
                        result = _cliente.GetAsync($"api/valuacionservice/GetValuacionUnidadTributaria/{idUnidadTributaria}").Result;
                        result.EnsureSuccessStatusCode();
                        valValuacion = result.Content.ReadAsAsync<VALValuacion>().Result;
                    }

                    var bytes = ReporteHelper.GenerarReporte(informeUnidadTributaria, parcelaModel, valValuacion, usuario);
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError("InformeUnidadTributariaController-GetInforme", ex);
                return NotFound();
            }
        }
        /*[HttpPost]
        public IHttpActionResult GetInformeDGCeIT(Parcela parcelamodel)
        {
            try
            {

                var informeUnidadTributaria = new InformeUnidadTributaria();

                var band = informeUnidadTributaria.Bands.Single(x => x.Name.Equals("DetailReportValuacion"));
                band.Visible = false;



                var bytes = new byte[0]; // ReporteHelper.GenerarReporte(informeUnidadTributaria, parcelamodel);
                return Ok(Convert.ToBase64String(bytes));
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                return NotFound();
            }
        }*/
    }
}
