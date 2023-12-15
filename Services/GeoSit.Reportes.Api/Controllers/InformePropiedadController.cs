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
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using GeoSit.Data.BusinessEntities.Designaciones;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformePropiedadController : ApiController
    {
        public IHttpActionResult Get(long idUnidadTributaria, /*long idTramite,*/ string usuario)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                try
                {
                    //cuando se realice la llamada desde bandeja de tramites que llegue el id tramite
                    int? idTramite = null;

                    var result = client.GetAsync($"api/UnidadTributaria/Get?id={idUnidadTributaria}&incluirDominios=true").Result;
                    result.EnsureSuccessStatusCode();
                    var unidadTributaria = result.Content.ReadAsAsync<UnidadTributaria>().Result;

                    /*----------------------------------------------------------------------------*/
                    result = client.GetAsync($"api/Parcela/Get/{unidadTributaria.ParcelaID}").Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela = result.Content.ReadAsAsync<Parcela>().Result;

                    using (var resp = client.GetAsync($"api/parcela/{unidadTributaria.ParcelaID}/estampilla").Result)
                    {
                        resp.EnsureSuccessStatusCode();
                        string dataURI = resp.Content.ReadAsAsync<string>().Result;
                        if (!string.IsNullOrEmpty(dataURI))
                        {
                            var regex = new Regex(@"data:(?<mime>[\w/\-\.]+);(?<encoding>\w+),(?<data>.*)", RegexOptions.IgnoreCase);
                            if (regex.IsMatch(dataURI))
                            {
                                unidadTributaria.Parcela.Ubicacion = Image.FromStream(new MemoryStream(Convert.FromBase64String(regex.Match(dataURI).Groups["data"].Value)));
                            }
                        }
                    }

                    

                    //Designaciones
                    result = client.GetAsync("api/Designacion/GetDesignacionesParcela?idParcela=" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.Designaciones = result.Content.ReadAsAsync<List<Designacion>>().Result;

                    // domicilios
                    /*result = client.GetAsync("api/Domicilio/GetDomiciliosByUnidadTributariaId?idUnidadTributaria=" + idUnidadTributaria).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.Domicilios = result.Content.ReadAsAsync<IEnumerable<Domicilio>>().Result;

                    // responsable fiscal
                    result = client.GetAsync("api/UnidadTributariaPersona/Get?idUnidadTributaria=" + idUnidadTributaria).Result;
                    result.EnsureSuccessStatusCode();
                    unidadTributaria.Parcela.ResponsablesFiscales = result.Content.ReadAsAsync<IEnumerable<ResponsableFiscal>>().Result;*/

                    var reporte = new InformePropiedad();

                    var lblCi = reporte.FindControl("lblCi", true);
                    if (!unidadTributaria.Dominios.Any() && unidadTributaria.Parcela.ClaseParcelaID == 6)
                    {
                        var idUt = unidadTributaria.Parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 2).FirstOrDefault().UnidadTributariaId;
                        result = client.GetAsync("api/Dominio/Get?idUnidadTributaria=" + idUt).Result;
                        result.EnsureSuccessStatusCode();
                        unidadTributaria.Parcela.Dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;

                        lblCi.Text = "Unidad Parcelaria sin inscripción registrada, se informa Dominio y Titular de inmueble Origen";

                    }
                    else
                    {
                        result = client.GetAsync("api/Dominio/Get?idUnidadTributaria=" + idUnidadTributaria).Result;
                        result.EnsureSuccessStatusCode();
                        unidadTributaria.Parcela.Dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result;

                        lblCi.Visible = false;

                    }

                    //Superficies
                    result = client.GetAsync($"api/parcela/{unidadTributaria.ParcelaID}/superficies/").Result;
                    result.EnsureSuccessStatusCode();
                    var parcelaSuperficies = result.Content.ReadAsAsync<ParcelaSuperficies>().Result;

                    //------
                    result = client.GetAsync("api/Parcela/GetPlanosMensuraByIdParcela?id=" + unidadTributaria.ParcelaID).Result;
                    result.EnsureSuccessStatusCode();
                    var atributoDocumento = result.Content.ReadAsAsync<IEnumerable<AtributosDocumento>>().Result;
                    var documentoActual = atributoDocumento.OrderBy(ad => ad.fecha_vigencia_actual).LastOrDefault();

                    string numMensura = string.Empty;
                    string vigenciaMensura = string.Empty;

                    if (documentoActual != null)
                    {
                        numMensura = $"{documentoActual.numero_plano }-{ documentoActual.letra_plano }";
                        vigenciaMensura = documentoActual.fecha_vigencia_actual.ToString("dd/MM/yyyy");
                    }


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
                    //unidadTributaria.UTValuaciones.ValorTotal = unidadTributaria.UTValuaciones.ValorMejoras + unidadTributaria.UTValuaciones.ValorTierra ?? 0;

                    if (unidadTributaria.Parcela.AtributoZonaID != null)
                    {
                        result = client
                            .GetAsync("api/parcela/getparcelavaluacionzona?idAtributoZona=" + unidadTributaria.Parcela.AtributoZonaID)
                            .Result;
                        result.EnsureSuccessStatusCode();
                        var objeto = result.Content.ReadAsAsync<Objeto>().Result;
                        unidadTributaria.Parcela.ZonaValuacion = objeto.Nombre;

                        result = client
                            .GetAsync("api/parcelaValuacion/GetByIdParcela?idParcela=" + unidadTributaria.Parcela.ParcelaID)
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
                    if (idTramite != null)
                    {
                        result = client.GetAsync($"api/MesaEntradas/Tramites/{idTramite}").Result;
                        result.EnsureSuccessStatusCode();
                        tramite = result.Content.ReadAsAsync<METramite>().Result;
                    }

                    byte[] bytes = ReporteHelper.GenerarReporte(reporte, unidadTributaria, parcelaSuperficies, usuario, numMensura, vigenciaMensura, tramite.Numero);
                    return Ok(Convert.ToBase64String(bytes));
                }
                catch (Exception ex)
                {
                    WebApiApplication.GetLogger().LogError("InformePropiedadController-Get", ex);
                    return NotFound();
                }
            }
        }
    }
}
