using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Mapa;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Controllers
{
    public class MapaController : Controller
    {
        private short ID_CAPA_TEMP_SEQ
        {
            get { return Convert.ToInt16(Session["ID_CAPA_TEMP_SEQ"]); }
            set { Session["ID_CAPA_TEMP_SEQ"] = value; }
        }

        [HttpGet]
        public JsonResult GetConfiguracionByIdMapa(short id)
        {
            using (var cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials }) { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                HttpResponseMessage resp = cliente.GetAsync($"api/Layers/GetMapLayersByIdMapa/{id}").Result;
                resp.EnsureSuccessStatusCode();
                var config = new
                {
                    capas = resp.Content.ReadAsAsync<MapLayer[]>().Result,
                    centro = new[] { Convert.ToDouble(ConfigurationManager.AppSettings["longitudInicial"]), Convert.ToDouble(ConfigurationManager.AppSettings["latitudInicial"]) },
                    zoom = Convert.ToInt32(ConfigurationManager.AppSettings["zoomInicial"])
                };
                return Json(config, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GenerarLayerMapaTematico(string extent)
        {
            using (var cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials }) { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            using (var resp = cliente.GetAsync($"api/Layers/GetMapLayerConfigMapaTematico").Result)
            {
                resp.EnsureSuccessStatusCode();
                var objetoResultadoDetalle = Session["objetoResultadoDetalle"] as ObjetoResultadoDetalle;
                var mapatematico = Session["MapaTematico"] as MapaTematicoModel;
                bool verLabels = mapatematico.Visualizacion.VerLabels;
                int opacidad = (int)Math.Round(255 - (int)objetoResultadoDetalle.Transparencia / 100m * 255, 0);

                Color colorFromHtml(string html) => Color.FromArgb(opacidad, ColorTranslator.FromHtml($"#{html}"));
                string rgbaFromColor(Color color) => $"rgba({color.R},{color.G},{color.B},{color.A / 255.0})";
                bool esIcono(Rango rango) => (mapatematico.Componente.Graficos == 3 || rango.Icono != null) && rango.sIcono != "glyphicon-one-fine-dot";
                bool esPuntoDefault(Rango rango) => mapatematico.Componente.Graficos == 3 && rango.sIcono == "glyphicon-one-fine-dot";

                var config = resp.Content.ReadAsAsync<MapLayer>().Result;
                config.IdCapa += ID_CAPA_TEMP_SEQ++;
                config.ZIndex = config.IdCapa;
                config.VisibleDefault = true;
                config.FiltroPredefinido = config.FiltroPredefinido.Replace("GUID_VALUE", objetoResultadoDetalle.GUID);
                config.NombreCapa = mapatematico.Nombre ?? $"Mapa Temático de {mapatematico.Componente.Nombre} ({DateTime.Now.ToString("dd/MM/yyyy HH:mm")})";
                var parametros = JsonConvert.DeserializeAnonymousType(config.ConfiguracionEstilo, new { value = string.Empty, text = string.Empty, font = string.Empty, icon_scale = 0m });
                config.ConfiguracionEstilo =
                    JsonConvert.SerializeObject(objetoResultadoDetalle.Rangos.Select((item, idx) => new
                    {
                        type = "field",
                        fields = new[] { parametros.value },
                        filters = new[] { (idx + 1).ToString() },
                        styles = new[] {
                                    new {
                                            icon = esIcono(item) ? new
                                            {
                                                embedded = $"data:image/png;base64,{Convert.ToBase64String(item.Icono)}",
                                                size = new[] { 32, 32 },
                                                anchor = new[] { 0.5m, 2m },
                                                scale = parametros.icon_scale,
                                                xunits = "fraction",
                                                yunits = "pixels"
                                            } : null,
                                            circle = esPuntoDefault(item) ? new
                                            {
                                                fill = new { color = rgbaFromColor(colorFromHtml(item.Color)) },
                                                stroke = new { color = rgbaFromColor(colorFromHtml(item.ColorBorde)), width = item.AnchoBorde },
                                                radius = 7m
                                            } : null,
                                            fill = esPuntoDefault(item) || esIcono(item) ? null : new { color = rgbaFromColor(colorFromHtml(item.Color)) },
                                            stroke = esPuntoDefault(item) || esIcono(item) ? null : new { color = rgbaFromColor(colorFromHtml(item.ColorBorde)), width = item.AnchoBorde }
                                        }
                        },
                        text = new { field = parametros.text, font = parametros.font, color = rgbaFromColor(Color.FromArgb(colorFromHtml(item.Color).ToArgb() ^ 0xFFFFFF)) },
                    }), new JsonSerializerSettings() { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore });

                config.ConfiguracionTematico = JsonConvert.SerializeObject(mapatematico.Visualizacion.Items.Select((item) => new
                {
                    image = $"data:image/png;base64,{Convert.ToBase64String(item.GetSVGIcon(objetoResultadoDetalle.GeometryType.GetValueOrDefault(), true))}",
                    text = item.Leyenda,
                }), new JsonSerializerSettings() { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore });

                return Json(new { config, extent }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DescargarGeometria(long idObjeto, string capa)
        {
            using (var cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials }) { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            using (var resp = cliente.GetAsync($"api/Layers/GetGeometriaObjeto?idObjeto={idObjeto}&capa={capa}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                byte[] bytes = Convert.FromBase64String(bytes64);
                string fileName = "Geometria_" + capa + "_" + idObjeto.ToString() + ".json";
                return File(bytes, "application/geo+json", fileName);
            }
        }

        public ActionResult DescargarGeometrias(string capa, string features)
        {
            //var features = JsonConvert.DeserializeAnonymousType(seleccion, new { seleccion = new[] { new[] { 0L } }.ToList(), capas = new string[] { string.Empty }.ToList() });
  
            using (var cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials }) { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            using (var resp = cliente.GetAsync($"api/Layers/GetGeometriasObjetos?capa={capa}&features={features}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                byte[] bytes = Convert.FromBase64String(bytes64);
                string fechaNombre = DateTime.Now.ToString("yyyyMMdd_HH-mm-ss");
                string fileName = "Geometria_" + capa + "_" + fechaNombre + ".geojson";
                return File(bytes, "application/geo+json", fileName);
            }
        }

    }
}