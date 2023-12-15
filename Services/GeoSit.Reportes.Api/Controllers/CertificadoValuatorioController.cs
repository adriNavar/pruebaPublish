using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Reportes.Api.Reportes;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Reportes.Api.Controllers
{
    public class CertificadoValuatorioController : ApiController
    {
        public IHttpActionResult Get(long idUnidadTributaria, long? idTramite, string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                try
                {
                    //cuando se realice la llamada desde bandeja de tramites que llegue el id tramite
                    //int? idTramite = null; 

                    var result = client.GetAsync($"api/UnidadTributaria/Get?id={idUnidadTributaria}&incluirDominios=true").Result;
                    result.EnsureSuccessStatusCode();
                    var unidadTributaria = result.Content.ReadAsAsync<UnidadTributaria>().Result;

                    /*----------------------------------------------------------------------------*/
                    result = client.GetAsync("api/Parcela/Get/" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela = result.Content.ReadAsAsync<Parcela>().Result;

                    // domicilios
                    result = client.GetAsync("api/Domicilio/GetDomiciliosByUnidadTributariaId?idUnidadTributaria=" + idUnidadTributaria).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.Domicilios = result.Content.ReadAsAsync<IEnumerable<Domicilio>>().Result;

                    var reporte = new CertificadoValuatorio();
                    var lblCi = reporte.FindControl("lblCi", true);
                    // responsable fiscal
                    if (!unidadTributaria.Dominios.Any() && unidadTributaria.Parcela.ClaseParcelaID == 6)
                    {
                        var idUt = unidadTributaria.Parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 2).FirstOrDefault().UnidadTributariaId;
                        result = client.GetAsync("api/UnidadTributariaPersona/Get?idUnidadTributaria=" + idUt).Result;
                        result.EnsureSuccessStatusCode();
                        unidadTributaria.Parcela.ResponsablesFiscales = result.Content.ReadAsAsync<IEnumerable<ResponsableFiscal>>().Result;

                        result = client.GetAsync("api/Dominio/Get?idUnidadTributaria=" + idUt).Result;
                        result.EnsureSuccessStatusCode();
                        unidadTributaria.Parcela.Dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;

                        lblCi.Text = "Unidad Parcelaria sin inscripción registrada, se informa Dominio y Titular de inmueble Origen";

                    }
                    else
                    {
                        result = client.GetAsync("api/UnidadTributariaPersona/Get?idUnidadTributaria=" + idUnidadTributaria).Result;
                        result.EnsureSuccessStatusCode();
                        unidadTributaria.Parcela.ResponsablesFiscales = result.Content.ReadAsAsync<IEnumerable<ResponsableFiscal>>().Result;

                        result = client.GetAsync("api/Dominio/Get?idUnidadTributaria=" + idUnidadTributaria).Result;
                        result.EnsureSuccessStatusCode();
                        unidadTributaria.Parcela.Dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;

                        lblCi.Visible = false;
                    }
                    
                    //Parametros Generales
                    result = client.GetAsync("api/SeguridadService/GetParametrosGenerales").Result;
                    result.EnsureSuccessStatusCode();
                    var parametros = result.Content.ReadAsAsync<List<ParametrosGenerales>>().Result;

                    bool activaNomenclatura = parametros.Any(pmt => pmt.Clave == "ACTIVA_NOMENCLATURAS" && pmt.Valor == "1");
                    bool activaPartidas = parametros.Any(pmt => pmt.Clave == "ACTIVA_PARTIDAS" && pmt.Valor == "1");

                    //Partida y Nomenclatura
                    var SubBandPartidaNomenclatura = reporte.FindControl("SubBandPartidaNomenclatura", true);
                    var SubBandNomenclatura = reporte.FindControl("SubBandNomenclatura", true);
                    var SubBandTitulo = reporte.FindControl("SubBandTitulo", true);
                    var SubBandPartida = reporte.FindControl("SubBandPartida", true);
                    var LabelNomenclatura = reporte.FindControl("lblLabelNomenclatura", true);
                    var lblNomenclatura = reporte.FindControl("lblNomenclatura", true);

                    if (activaPartidas == true && activaNomenclatura == true)
                    {
                        SubBandNomenclatura.Visible = false;
                        SubBandNomenclatura.CanShrink = true;
                        SubBandPartida.Visible = false;
                        SubBandPartida.CanShrink = true;
                        SubBandTitulo.Visible = false;
                        SubBandTitulo.CanShrink = true;
                    }
                    else if (activaPartidas == true && activaNomenclatura == false)
                    {
                        SubBandNomenclatura.Visible = false;
                        SubBandNomenclatura.CanShrink = true;
                        SubBandPartidaNomenclatura.Visible = false;
                        SubBandPartidaNomenclatura.CanShrink = true;
                        SubBandTitulo.Visible = false;
                        SubBandTitulo.CanShrink = true;
                    }
                    else if (activaPartidas == false && activaNomenclatura == true)
                    {
                        SubBandNomenclatura.Visible = true;
                        SubBandPartidaNomenclatura.Visible = false;
                        SubBandPartidaNomenclatura.CanShrink = true;
                        SubBandPartida.Visible = false;
                        SubBandPartida.CanShrink = true;
                        SubBandTitulo.Visible = false;
                        SubBandTitulo.CanShrink = true;
                    }
                    else
                    {
                        SubBandNomenclatura.Visible = false;
                        SubBandNomenclatura.CanShrink = true;
                        SubBandPartidaNomenclatura.Visible = false;
                        SubBandPartidaNomenclatura.CanShrink = true;
                        SubBandPartida.Visible = false;
                        SubBandPartida.CanShrink = true;
                        SubBandTitulo.Visible = true;
                    }


                    //------
                    foreach (var nomenc in unidadTributaria.Parcela.Nomenclaturas)
                    {
                        nomenc.FechaAlta = unidadTributaria.Parcela.FechaAlta;
                        nomenc.FechaModificacion = unidadTributaria.Parcela.FechaModificacion;
                    }

                    if (!string.IsNullOrEmpty(unidadTributaria.Parcela.Atributos))
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(unidadTributaria.Parcela.Atributos);
                        var node = xmlDoc.SelectSingleNode("//datos/AfectaPH/text()");
                        unidadTributaria.Parcela.AfectaPh = node != null ? (node.Value == "true" ? "SI" : "NO") : string.Empty;
                        node = xmlDoc.SelectSingleNode("//datos/SuperfecieTitulo/text()");
                        unidadTributaria.Parcela.SuperfecieTitulo = node != null ? node.Value : string.Empty;
                        node = xmlDoc.SelectSingleNode("//datos/SuperfecieMensura/text()");
                        unidadTributaria.Parcela.SuperfecieMensura = node != null ? node.Value : string.Empty;
                    }



                    result = client.GetAsync("api/parcela/getparcelasorigen?idparceladestino=" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.ParcelaOrigenes = result.Content.ReadAsAsync<IEnumerable<ParcelaOrigen>>().Result;

                    //valuaciones
                    result = client.GetAsync("api/DeclaracionJurada/GetValuacionVigente?idUnidadTributaria=" + unidadTributaria.UnidadTributariaId).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.UTValuaciones = result.Content.ReadAsAsync<VALValuacion>().Result;
                    
                    if (unidadTributaria.Parcela.AtributoZonaID != null)
                    {
                        result = client
                            .GetAsync("api/parcela/getparcelavaluacionzona?idAtributoZona=" + unidadTributaria.Parcela.AtributoZonaID)
                            .Result;
                        result.EnsureSuccessStatusCode();
                        var objeto = result.Content.ReadAsAsync<Objeto>().Result;
                        unidadTributaria.Parcela.ZonaValuacion = objeto.Nombre;

                        result = client
                            .GetAsync($"api/parcelaValuacion/GetByIdParcela?idParcela={unidadTributaria.Parcela.ParcelaID}")
                            .Result;
                        result.EnsureSuccessStatusCode();
                        var objeto2 = result.Content.ReadAsAsync<ParcelaValuacion>().Result;
                        unidadTributaria.Parcela.ValorTierra = objeto2.ValorTierra;
                        unidadTributaria.Parcela.ValorMejora = objeto2.ValorMejora;
                        unidadTributaria.Parcela.ValorInmueble = objeto2.ValorMejora + objeto2.ValorTierra;
                        unidadTributaria.Parcela.FechaVigencia = objeto2.VigenciaDesde;
                    }

                    //parte tramite para que se muestre el numero y no el id en el reporte
                    METramite tramite = new METramite();
                    if (idTramite != null && idTramite > 0)
                    {
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = result.Content.ReadAsAsync<METramite>().Result;
                    }

                    //Designacion
                    result = client.GetAsync($"api/Designacion/GetDesignacionesParcela?idParcela={unidadTributaria.ParcelaID}").Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Designacion = result.Content.ReadAsAsync<List<Designacion>>().Result.FirstOrDefault(d => d.IdTipoDesignador == 0);

                    //Jurisdiccion
                    if (unidadTributaria.JurisdiccionID != null)
                    {
                        result = client.GetAsync("api/ObjetoAdministrativoService/GetObjetoById?id=" + unidadTributaria.JurisdiccionID).Result;
                        result.EnsureSuccessStatusCode();
                        unidadTributaria.Objeto = result.Content.ReadAsAsync<Objeto>().Result;
                    }
                    byte[] bytes = ReporteHelper.GenerarReporte(reporte, unidadTributaria, usuario, tramite.Numero);
                    return Ok(Convert.ToBase64String(bytes));
                }
                catch (Exception ex)
                {
                    WebApiApplication.GetLogger().LogError("CertificadoValuatorioController", ex);
                    return NotFound();
                }
            }
        }
    }
}
