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
    public class InformeParcelarioBajaController : ApiController
    {
        // GET api/informeparcelarioBaja
        private readonly HttpClient _cliente = new HttpClient();

        public IHttpActionResult GetInforme(int id, int padronPartidaId, string usuario)
        {
            try
            {
                _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                using (_cliente)
                {
                    var result = _cliente.GetAsync("api/Parcela/Get/" + id).Result.EnsureSuccessStatusCode();
                    var model = result.Content.ReadAsAsync<Parcela>().Result;

                    using (var resp = _cliente.GetAsync($"api/parcela/{id}/estampilla").Result.EnsureSuccessStatusCode())
                    {
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

                    var reporte = new InformeParcelarioBaja();

                    var detailReportValuaciones = reporte.FindControl("DetailReportValuaciones", true);
                    detailReportValuaciones.Visible = activaValuaciones;

                    VALValuacion valValuacion = null;
                    if (activaValuaciones)
                    {
                        //Valuacion
                        result = _cliente.GetAsync("api/ValuacionService/GetValuacionParcela?id=" + model.ParcelaID + "&esHistorico=true").Result.EnsureSuccessStatusCode();
                        valValuacion = result.Content.ReadAsAsync<VALValuacion>().Result;
                    }

                    //Superficies
                    result = _cliente.GetAsync($"api/parcela/{id}/superficies/?esHistorico=true").Result.EnsureSuccessStatusCode();
                    var parcelaSuperficies = result.Content.ReadAsAsync<ParcelaSuperficies>().Result;

                    //Unidades Tributarias
                    result = _cliente.GetAsync("api/UnidadTributaria/GetUnidadesTributariasByParcela?idParcela=" + model.ParcelaID + "&esHistorico=true").Result.EnsureSuccessStatusCode();
                    model.UnidadesTributarias = result.Content.ReadAsAsync<List<UnidadTributaria>>().Result;

                    //Usuario de Baja
                    DateTime fechaBaja = model.FechaBaja.GetValueOrDefault().Date;
                    result = _cliente.GetAsync($"api/SeguridadService/Usuarios/{model.UsuarioBajaID}/Fecha/{fechaBaja.Ticks}").Result.EnsureSuccessStatusCode();
                    var usuarioBaja = result.Content.ReadAsAsync<Usuarios>().Result;

                    //Dominios
                    result = _cliente.GetAsync("api/Dominio/GetDominiosUFByParcela?idParcela=" + model.ParcelaID + "&esHistorico=true").Result.EnsureSuccessStatusCode();
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

                    //Designacion
                    if (activaDesignaciones == true)
                    {
                        result = _cliente.GetAsync("api/Designacion/GetDesignacionesParcela?idParcela=" + model.ParcelaID).Result.EnsureSuccessStatusCode();
                        model.Designaciones = result.Content.ReadAsAsync<List<Designacion>>().Result;
                    }
                    else
                    {
                        var detailReportDesignaciones = reporte.FindControl("DetailReportDesignacion", true);
                        detailReportDesignaciones.Visible = false;
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
                    result = _cliente.GetAsync("api/parcela/GetZonificacion?idparcela=" + id + "&esHistorico=true").Result.EnsureSuccessStatusCode();
                    var zonificacion = result.Content.ReadAsAsync<Zonificacion>().Result;

                    if (activaZonificacion == false)
                    {
                        var detailReportZonificacion = reporte.FindControl("DetailReportZonificacion", true);
                        detailReportZonificacion.Visible = false;
                    }

                    //Parcelas Origen
                    result = _cliente.GetAsync("api/parcela/getparcelasorigen?idparceladestino=" + id).Result.EnsureSuccessStatusCode();
                    model.ParcelaOrigenes = result.Content.ReadAsAsync<IEnumerable<ParcelaOrigen>>().Result;


                    if (!model.ParcelaOrigenes.Any())
                    {
                        var detailReportParcelaOrigenes = reporte.FindControl("DetailReportParcelaOrigenes", true);
                        detailReportParcelaOrigenes.Visible = false;
                    }

                    var xrTableCellPadronPartida = reporte.FindControl("xrTableCellPadronPartida", true);
                    result = _cliente.GetAsync("api/Parametro/GetValor?id=" + padronPartidaId).Result.EnsureSuccessStatusCode();
                    xrTableCellPadronPartida.Text = result.Content.ReadAsStringAsync().Result;

                    var bytes = ReporteHelper.GenerarReporte(reporte, model, valValuacion, parcelaSuperficies, zonificacion, usuarioBaja, usuario);
                    return Ok(Convert.ToBase64String(bytes));
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"InformeParcelarioBaja/GetInforme({id},{padronPartidaId},{usuario})", ex);
                return NotFound();
            }
        }
    }
}
