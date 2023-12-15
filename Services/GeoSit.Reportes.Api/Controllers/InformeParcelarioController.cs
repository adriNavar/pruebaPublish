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
    public class InformeParcelarioController : ApiController
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

                    var reporte = new InformeParcelario();

                    var detailReportValuaciones = reporte.FindControl("DetailReportValuaciones", true);
                    detailReportValuaciones.Visible = activaValuaciones;

                    VALValuacion valValuacion = null;
                    if (activaValuaciones)
                    {
                        //Valuacion
                        result = _cliente.GetAsync("api/ValuacionService/GetValuacionParcela?id=" + model.ParcelaID).Result;
                        result.EnsureSuccessStatusCode();
                        valValuacion = result.Content.ReadAsAsync<VALValuacion>().Result;
                    }

                    //Superficies
                    result = _cliente.GetAsync($"api/parcela/{id}/superficies/").Result;
                    result.EnsureSuccessStatusCode();
                    var parcelaSuperficies = result.Content.ReadAsAsync<ParcelaSuperficies>().Result;

                    //Unidades Tributarias
                    result = _cliente.GetAsync("api/UnidadTributaria/GetUnidadesTributariasByParcela?idParcela=" + model.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    model.UnidadesTributarias = result.Content.ReadAsAsync<List<UnidadTributaria>>().Result/*.OrderBy(x => x.TipoUnidadTributariaID).ThenBy(x => x.CodigoMunicipal).ToList()*/;

                    //Dominios
                    result = _cliente.GetAsync("api/Dominio/GetDominiosUFByParcela?idParcela=" + model.ParcelaID).Result;
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
                        /*LabelNomenclatura.Visible = false;
                        LabelNomenclatura.CanShrink = true;
                        lblNomenclatura.Visible = false;
                        lblNomenclatura.CanShrink = false;*/
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

                    /*if (activaNomenclatura == true)
                     {
                            foreach (var nomenc in model.Nomenclaturas)
                            {
                                nomenc.FechaAlta = model.FechaAlta;
                                nomenc.FechaModificacion = model.FechaModificacion;
                            }
                     }
                     else
                     {
                            var DetailSubReporteNomenclatura = reporte.FindControl("DetailSubReporteNomenclatura", true);
                            DetailSubReporteNomenclatura.Visible = false;
                     }*/

                    //Designacion
                    if (activaDesignaciones == true)
                    {
                        result = _cliente.GetAsync("api/Designacion/GetDesignacionesParcela?idParcela=" + model.ParcelaID).Result;
                        result.EnsureSuccessStatusCode();
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
                        /*node = xmlDoc.SelectSingleNode("//datos/SuperfecieTitulo/text()");
                        model.SuperfecieTitulo = node != null ? node.Value : string.Empty;
                        node = xmlDoc.SelectSingleNode("//datos/SuperfecieMensura/text()");
                        model.SuperfecieMensura = node != null ? node.Value : string.Empty;*/
                        node = xmlDoc.SelectSingleNode("//datos/Observacion/text()");
                        model.Observaciones = node != null ? node.Value : string.Empty;
                    }

                    //Zonificacion
                    result = _cliente.GetAsync("api/parcela/GetZonificacion?idparcela=" + id).Result;
                    result.EnsureSuccessStatusCode();
                    var zonificacion = result.Content.ReadAsAsync<Zonificacion>().Result;

                    if (activaZonificacion == false)
                    {
                        var detailReportZonificacion = reporte.FindControl("DetailReportZonificacion", true);
                        detailReportZonificacion.Visible = false;
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
                    result = _cliente.GetAsync("api/Parametro/GetValor?id=" + padronPartidaId).Result;
                    result.EnsureSuccessStatusCode();
                    xrTableCellPadronPartida.Text = result.Content.ReadAsStringAsync().Result;

                    /*if (activaValuacion == "1")
                    {
                        if (model.AtributoZonaID != null)
                        {
                            result = _cliente
                                .GetAsync("api/parcela/getparcelavaluacionzona?idAtributoZona=" + model.AtributoZonaID)
                                .Result;
                            result.EnsureSuccessStatusCode();
                            var objeto = result.Content.ReadAsAsync<Objeto>().Result;
                            model.ZonaValuacion = objeto.Nombre;

                            result = _cliente
                                .GetAsync("api/parcelaValuacion/GetByIdParcela?idParcela=" + model.ParcelaID)
                                .Result;
                            result.EnsureSuccessStatusCode();
                            var objeto2 = result.Content.ReadAsAsync<ParcelaValuacion>().Result;
                            model.ValorTierra = objeto2.ValorTierra;
                            model.ValorMejora = objeto2.ValorMejora;
                            model.ValorInmueble = objeto2.ValorMejora + objeto2.ValorTierra;
                            model.FechaVigencia = objeto2.VigenciaDesde;

                            //model.FechaVigencia = DateTime.ParseExact(time, "dd/MM/yyyy", null);
                        }
                    } else 
                    {
                        var detailReportValuaciones = reporte.FindControl("DetailReportValuaciones", true);
                        detailReportValuaciones.Visible = false;
                    }*/

                    var bytes = ReporteHelper.GenerarReporte(reporte, model, valValuacion, parcelaSuperficies, zonificacion, usuario);
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
