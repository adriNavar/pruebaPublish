using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Net.Http;
using GeoSit.Client.Web.Solr;
using System.Net;

namespace GeoSit.Client.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly HttpClient cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials });

        public SearchController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return PartialView("SearchForm");
        }

        [HttpPost]
        public string ByFeatures(string features, int filterType, string newFeature)
        {
            string filtro = "capa";
            string campo = "featids";
            if (filterType != 1)
            {
                filtro = "tipo";
                campo = "id";
            }

            var busqueda = JsonConvert.DeserializeAnonymousType(features, new { seleccion = new[] { new[] { 0L } }.ToList(), capas = new string[] { string.Empty }.ToList() });

            //Remueve el primer id del array en caso de que la cantidad de elementos sea mayor a la que se muestra en pantalla
            /*
             * Cambio la logica porque si el tipo de objeto a agregar no tenia al menos un resultado en el buscador, 
             * no lo agregaba.
             * 
             * tambien hice que la cantidad maxima se tome del parametro que se usa para la busqueda. de esta forma, se mantiene 
             * en un solo lugar
             */
            if (!string.IsNullOrEmpty(newFeature))
            {
                var feature = JsonConvert.DeserializeAnonymousType(newFeature, new { id = 0L, doctype = string.Empty });

                var group_limit = ConfigurationManager.AppSettings["solrGroupParams"].Split('&')
                                    .Select(x => new { key = x.Split('=')[0], value = x.Split('=')[1] })
                                    .Single(k => k.key == "group.limit");

                int group_idx = busqueda.capas.IndexOf(feature.doctype);
                if (group_idx == -1 || !busqueda.seleccion[group_idx].Any(x => x == feature.id))
                { //solo agrego el nuevo objeto si no está entre los resultados actuales
                    var items = new long[0];
                    if (group_idx != -1)
                    {
                        items = busqueda.seleccion[group_idx];
                        if (items.Length >= Convert.ToInt32(group_limit.value))
                        {
                            items = new ArraySegment<long>(items, 0, Convert.ToInt32(group_limit.value) - 1).ToArray();
                        }
                    }
                    else
                    {
                        busqueda.capas.Add(feature.doctype);
                        busqueda.seleccion.Add(items);
                        group_idx = busqueda.seleccion.Count - 1;
                    }
                    busqueda.seleccion[group_idx] = items.Prepend(feature.id).ToArray();
                }
            }
            var terms = new List<KeyValuePair<int, string[]>>();
            for (int i = 0; i < busqueda.capas.Count; i++)
            {
                terms.Add(new KeyValuePair<int, string[]>(i, new string[] { filtro, busqueda.capas[i] }));
                terms.Add(new KeyValuePair<int, string[]>(i, new string[] { campo, string.Join(",", busqueda.seleccion[i]) }));
            }

            return new SolrServer()
            {
                UseDefaultBaseParams = true,
                UseDefaultFacetParams = true,
                UseDefaultGroupParams = true
            }.Terms(terms.ToList());
        }
        //private string GetLayerName(string layer)
        //{
        //    Regex regexLayerName = new Regex(@"^(?<tabla>[a-zA-Z0-9_\- ]+?)(?<conector>(_)+?)(?<fecha>[0-9\- ]+)\*?$", RegexOptions.IgnoreCase);
        //    if (regexLayerName.IsMatch(layer))
        //    {
        //        return regexLayerName.Match(layer).Groups["tabla"].Value;
        //    }
        //    return layer;
        //}

        [HttpPost]
        public string ByText(string text)
        {
            string pattern = $"*{SolrServer.EscapeSpecialChars(text)}*";
            int idx = text.IndexOf(":");
            if (idx != -1)
            {
                pattern = $"{text.Split(':').First()}:*{SolrServer.EscapeSpecialChars(Url.Encode(text.Substring(idx + 1)))}*";
            }
            return new SolrServer()
            {
                UseDefaultBaseParams = true,
                UseDefaultFacetParams = true,
                UseDefaultGroupParams = true
            }.Search(pattern);
        }

        [HttpPost]
        public string ByType(string filtro, string tipo, int idComponente)
        {

            var maximoNumeroObjetosABuscarPlotPredf = GetmaximoNumeroObjetosABuscarPlotPredf();

            var solr = new SolrServer() { UseDefaultBaseParams = true };
            solr.AddParam(new SolrParam("fq", $"tipo:{tipo}"));
            solr.AddParam(new SolrParam("lf", "nombre"));
            solr.AddParam(new SolrParam("rows", maximoNumeroObjetosABuscarPlotPredf.ToString()));
            return solr.Search($"*{filtro}*");
        }

        public int GetmaximoNumeroObjetosABuscarPlotPredf()
        {
            List<ParametrosGeneralesModel> lstParametrosGenerales = GetParametrosGenerales();
            var maximoNumeroObjetosABuscarPlotPredf = 0;
            ParametrosGeneralesModel parammaximoNumeroObjetosABuscarPlotPredf = lstParametrosGenerales.FirstOrDefault(p => p.Descripcion == "PloteoPredefMaxObjetoBusqueda");
            if (parammaximoNumeroObjetosABuscarPlotPredf != null)
            {
                maximoNumeroObjetosABuscarPlotPredf = Convert.ToInt32(parammaximoNumeroObjetosABuscarPlotPredf.Valor);
            }

            return maximoNumeroObjetosABuscarPlotPredf;
        }

        private List<ParametrosGeneralesModel> GetParametrosGenerales()
        {
            using (var cliente2 = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                var resp2 = cliente2.GetAsync("api/MapasTematicosService/GetParametrosGenerales").Result;
                resp2.EnsureSuccessStatusCode();
                return resp2.Content.ReadAsAsync<IEnumerable<ParametrosGeneralesModel>>().Result.ToList();
            }
        }

        [HttpGet]
        public string Suggest(string text)
        {
            return new SolrServer()
            {
                UseDefaultBaseParams = true,
                UseDefaultSuggestParams = true
            }.Suggest(Url.Encode(text));
        }

        [HttpGet]
        public ActionResult ObtenerLayer(long id)
        {
            try
            {
                return Json(new { Data = getComponenteById(id).DocType }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("SearchController-ObtenerLayer", ex);
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
        }
        private Componente getComponenteById(long id)
        {
            var result = cliente.GetAsync(string.Format("api/Componente/Get?id=" + id)).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<Componente>().Result;
        }
    }

    public static class XMethods
    {
        public static string Highlight(this string texto, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return texto;
            return Regex.Replace(texto, pattern, string.Format("<span class='highlight'>{0}</span>", "$0"), RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
        }
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
        public static object GetValue(this JToken obj, string path)
        {
            return obj.SelectToken(path);
        }
        public static string ToStringOrDefault(this object obj)
        {
            return obj == null ? "N/D" : obj.ToString();
        }
        public static string Capitalize(this string value)
        {
            return value.First().ToString().ToUpper() + string.Join("", value.Skip(1));
        }
    }
}