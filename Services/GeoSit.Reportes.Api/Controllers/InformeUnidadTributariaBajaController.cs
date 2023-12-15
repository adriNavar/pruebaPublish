using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Reportes.Api.Reportes;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeUnidadTributariaBajaController : ApiController
    {
        private readonly HttpClient _cliente = new HttpClient();

        public InformeUnidadTributariaBajaController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET api/informeunidadtributariaBaja
        public IHttpActionResult GetInforme(long idParcela, long idUnidadTributaria, string usuario)
        {
            try
            {
                using (_cliente)
                {
                    var informeUnidadTributariaBaja = new InformeUnidadTributariaBaja();

                    var result = _cliente.GetAsync($"api/parcela/{idParcela}/utshistoricas").Result.EnsureSuccessStatusCode();
                    var parcelaModel = result.Content.ReadAsAsync<Parcela>().Result;

                    var unidadTributaria = parcelaModel.UnidadesTributarias
                                                       .Single(x => x.UnidadTributariaId == idUnidadTributaria);
                    parcelaModel.UnidadesTributarias.Clear();
                    parcelaModel.UnidadesTributarias.Add(unidadTributaria);

                    //Responsables Fiscales
                    result = _cliente.GetAsync($"api/Dominio/Get?idUnidadTributaria={idUnidadTributaria}&esHistorico=true").Result.EnsureSuccessStatusCode();
                    parcelaModel.Dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;

                    //Usuario de Baja
                    DateTime fechaBaja = unidadTributaria.FechaBaja.GetValueOrDefault().Date;
                    result = _cliente.GetAsync($"api/SeguridadService/Usuarios/{unidadTributaria.UsuarioBajaID}/Fecha/{fechaBaja.Ticks}").Result.EnsureSuccessStatusCode();
                    var usuarioBaja = result.Content.ReadAsAsync<Usuarios>().Result;
                    

                    //Parametros Generales
                    result = _cliente.GetAsync("api/Parametro/GetParametroByClave?clave=ACTIVA_VALUACIONES").Result.EnsureSuccessStatusCode();
                    var parametro = result.Content.ReadAsAsync<ParametrosGenerales>().Result;
                    bool activaValuaciones = parametro?.Valor == "1";
                    var detailReportValuaciones = informeUnidadTributariaBaja.FindControl("DetailReportValuaciones", true);
                    detailReportValuaciones.Visible = activaValuaciones;
                    
                    VALValuacion valValuacion = null;
                    if (activaValuaciones)
                    {
                        //Valuacion
                        result = _cliente.GetAsync($"api/valuacionservice/GetValuacionUnidadTributaria/{idUnidadTributaria}").Result.EnsureSuccessStatusCode();
                        valValuacion = result.Content.ReadAsAsync<VALValuacion>().Result;
                    }

                    var bytes = ReporteHelper.GenerarReporte(informeUnidadTributariaBaja, parcelaModel, valValuacion, usuarioBaja, usuario);
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError("InformeUnidadTributariaBajaController-GetInforme", ex);
                return NotFound();
            }
        }
    }
}
