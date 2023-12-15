using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Xml;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Reportes.Api.Helpers;
using GeoSit.Reportes.Api.Reportes;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeParcelarioVIRController : ApiController
    {
        // GET api/informeparcelario
        private readonly HttpClient _cliente = new HttpClient();

        public IHttpActionResult GetInforme(int id, int padronPartidaId, string usuario)
        {
            try
            {
                _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                using (_cliente)
                {
                    var result = _cliente.GetAsync("api/Parcela/Get/" + id).Result;
                    result.EnsureSuccessStatusCode();
                    var model = result.Content.ReadAsAsync<Parcela>().Result;

                    using (var resp = _cliente.GetAsync($"api/parcela/{id}/estampilla").Result)
                    {
                        resp.EnsureSuccessStatusCode();
                        string dataURI = resp.Content.ReadAsAsync<string>().Result;
                        if (!string.IsNullOrEmpty(dataURI))
                        {
                            var regex = new Regex(@"data:(?<mime>[\w/\-\.]+);(?<encoding>\w+),(?<data>.*)", RegexOptions.IgnoreCase);
                            if (regex.IsMatch(dataURI))
                            {
                                model.Ubicacion = Image.FromStream(new MemoryStream(Convert.FromBase64String(regex.Match(dataURI).Groups["data"].Value)));
                            }
                        }
                    }

                    //Parametros Generales
                    result = _cliente.GetAsync("api/SeguridadService/GetParametrosGenerales").Result;
                    result.EnsureSuccessStatusCode();
                    var parametros = result.Content.ReadAsAsync<List<ParametrosGenerales>>().Result;

                    bool activaDesignaciones = parametros.Any(pmt => pmt.Clave == "ACTIVA_DESIGNACIONES" && pmt.Valor == "1");
                    bool activaZonificacion = parametros.Any(pmt => pmt.Clave == "ACTIVA_ZONIFICACION" && pmt.Valor == "1");
                    bool activaValuaciones = parametros.Any(pmt => pmt.Clave == "ACTIVA_VALUACIONES" && pmt.Valor == "1");
                    bool activaNomenclatura = parametros.Any(pmt => pmt.Clave == "ACTIVA_NOMENCLATURAS" && pmt.Valor == "1");
                    bool activaPartidas = parametros.Any(pmt => pmt.Clave == "ACTIVA_PARTIDAS" && pmt.Valor == "1");

                    var reporte = new InformeParcelarioVIR();
                    var detailReportValuaciones = reporte.FindControl("DetailReportValuaciones", true);
                    detailReportValuaciones.Visible = activaValuaciones;

                    VALValuacion valValuacion = null;
                    if (activaValuaciones)
                    {
                        //Valuacion
                        result = _cliente.GetAsync($"api/ValuacionService/GetValuacionParcela/{model.ParcelaID}").Result;
                        result.EnsureSuccessStatusCode();
                        valValuacion = result.Content.ReadAsAsync<VALValuacion>().Result;
                    }

                    //ValuacionVIR
                    result = _cliente.GetAsync($"api/VIR/GetValuacionVIRInmueble/{model.UnidadesTributarias.FirstOrDefault(x => x.TipoUnidadTributariaID != 3).UnidadTributariaId}").Result;
                    result.EnsureSuccessStatusCode();
                    var valuacionVIR = result.Content.ReadAsAsync<VIRValuacion>().Result;

                    //Superficies
                    result = _cliente.GetAsync($"api/parcela/{id}/superficies/").Result;
                    result.EnsureSuccessStatusCode();
                    var parcelaSuperficies = result.Content.ReadAsAsync<ParcelaSuperficies>().Result;

                    //Unidades Tributarias
                    result = _cliente.GetAsync($"api/UnidadTributaria/GetUnidadesTributariasByParcela?idParcela={model.ParcelaID}").Result;
                    result.EnsureSuccessStatusCode();
                    model.UnidadesTributarias = result.Content.ReadAsAsync<List<UnidadTributaria>>().Result;

                    //Dominios
                    result = _cliente.GetAsync($"api/Dominio/GetDominiosUFByParcela?idParcela={model.ParcelaID}").Result;
                    result.EnsureSuccessStatusCode();
                    var dominios = result.Content.ReadAsAsync<List<Dominio>>().Result;

                    foreach (var ut in model.UnidadesTributarias)
                    {
                        ut.Dominios = dominios.Where(t => t.UnidadTributariaID == ut.UnidadTributariaId).ToList();
                    }

                    //Partida y Nomenclatura
                    var SubBandPartidaNomenclatura = reporte.FindControl("SubBandPartidaNomenclatura", true);
                    var SubBandNomenclatura = reporte.FindControl("SubBandNomenclatura", true);
                    var SubBandTitulo = reporte.FindControl("SubBandTitulo", true);
                    var SubBandPartida = reporte.FindControl("SubBandPartida", true);
                    var LabelNomenclatura = reporte.FindControl("lblLabelNomenclatura", true);
                    var lblNomenclatura = reporte.FindControl("lblNomenclatura", true);


                    SubBandNomenclatura.Visible = activaNomenclatura && !activaPartidas;
                    SubBandNomenclatura.CanShrink = activaPartidas || !activaNomenclatura;
                    SubBandPartida.Visible = activaPartidas && !activaNomenclatura;
                    SubBandPartida.CanShrink = activaNomenclatura || !activaPartidas;
                    SubBandPartidaNomenclatura.Visible = activaPartidas && activaNomenclatura;
                    SubBandPartidaNomenclatura.CanShrink = !activaPartidas || !activaNomenclatura;
                    SubBandTitulo.Visible = !activaNomenclatura && !activaNomenclatura;
                    SubBandTitulo.CanShrink = activaPartidas || activaNomenclatura;

                    //Designacion
                    var detailReportDesignaciones = reporte.FindControl("DetailReportDesignacion", true);
                    detailReportDesignaciones.Visible = activaDesignaciones;
                    if (activaDesignaciones)
                    {
                        result = _cliente.GetAsync("api/Designacion/GetDesignacionesParcela?idParcela=" + model.ParcelaID).Result;
                        result.EnsureSuccessStatusCode();
                        model.Designaciones = result.Content.ReadAsAsync<List<Designacion>>().Result;
                    }


                    //Observaciones y Afecta PH
                    if (!string.IsNullOrEmpty(model.Atributos))
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(model.Atributos);
                        var node = xmlDoc.SelectSingleNode("//datos/AfectaPH/text()");
                        model.AfectaPh = node != null ? (node.Value == "true" ? "SI" : "NO") : string.Empty;
                        node = xmlDoc.SelectSingleNode("//datos/Observacion/text()");
                        model.Observaciones = node != null ? node.Value : string.Empty;
                    }

                    //Zonificacion
                    var detailReportZonificacion = reporte.FindControl("DetailReportZonificacion", true);
                    detailReportZonificacion.Visible = activaZonificacion;

                    Zonificacion zonificacion = null;
                    if (activaZonificacion)
                    {
                        result = _cliente.GetAsync("api/parcela/GetZonificacion?idparcela=" + id).Result;
                        result.EnsureSuccessStatusCode();
                        zonificacion = result.Content.ReadAsAsync<Zonificacion>().Result;
                    }

                    //Parcelas Origen
                    result = _cliente.GetAsync("api/parcela/getparcelasorigen?idparceladestino=" + id).Result;
                    result.EnsureSuccessStatusCode();
                    model.ParcelaOrigenes = result.Content.ReadAsAsync<IEnumerable<ParcelaOrigen>>().Result;


                    if (!model.ParcelaOrigenes.Any())
                    {
                        var detailReportParcelaOrigenes = reporte.FindControl("DetailReportParcelaOrigenes", true);
                        detailReportParcelaOrigenes.Visible = false;
                    }

                    var xrTableCellPadronPartida = reporte.FindControl("xrTableCellPadronPartida", true);
                    result = _cliente.GetAsync($"api/Parametro/GetValor?id={padronPartidaId}").Result;
                    result.EnsureSuccessStatusCode();
                    xrTableCellPadronPartida.Text = result.Content.ReadAsStringAsync().Result;

                    var bytes = ReporteHelper.GenerarReporte(reporte, model, valValuacion, valuacionVIR, parcelaSuperficies, zonificacion, usuario);
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"InformeParcelario/GetInforme({id},{padronPartidaId},{usuario})", ex);
                return NotFound();
            }
        }
    }
}
