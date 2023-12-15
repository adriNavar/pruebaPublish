using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GeoSit.Client.Web.Models.Search;
using GeoSit.Client.Web.Solr;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeoSit.Client.Web.Controllers.Search
{
    public class BuscadorGenericoController : Controller
    {
        private string[] CamposBusqueda
        {
            set
            {
                Session["CamposBusqueda"] = value;
            }
            get
            {
                return (string[])Session["CamposBusqueda"];
            }
        }
        private bool IgnoreText
        {
            set
            {
                Session["__IgnoreText__"] = value;
            }
            get
            {
                return (bool)Session["__IgnoreText__"];
            }
        }
        private bool RetrieveFeatids
        {
            set
            {
                Session["__RetrieveFeatids__"] = value;
            }
            get
            {
                return (bool)Session["__RetrieveFeatids__"];
            }
        }

        private Dictionary<string, string> Filters
        {
            set
            {
                Session["__Filters__"] = value;
            }
            get
            {
                return (Dictionary<string, string>)Session["__Filters__"];
            }
        }
        // GET: BuscadorGenerico
        public ActionResult Index(string titulo, bool multiSelect, bool verAgregar, string tipos, string[] campos, object[][] seleccionActual, string[] filters, bool readonlyText = false, bool retrieveFeatids = false, bool includeSearch = false)
        {
            CamposBusqueda = new[] { "nombre" }
                        .Concat(campos
                                .Where(c => c.Split(':').Length == 2)
                                .Select(c => $"dato_{c.Split(':')[0]}"))
                        .ToArray();
            IgnoreText = readonlyText;
            RetrieveFeatids = retrieveFeatids;
            Filters = (filters ?? new string[0]).ToDictionary(f => f.Split('=').First(), f => f.Split('=').Last());

            return PartialView(new BuscadorGenericoConfig()
            {
                Titulo = titulo,
                MultiSelect = multiSelect,
                Tipos = tipos,
                Campos = campos.Select(c => c.Split(':').Last()),
                VerAgregar = verAgregar,
                SeleccionActual = seleccionActual ?? new object[0][],
                IncluirTextoBusqueda = includeSearch
            });
        }

        // GET: BuscadorGenerico
        public ActionResult SuggestsEx(string text, string tipos)
        {
            var filters = Filters.ToDictionary(f => f.Key, f => f.Value);
            var camposSeleccionados = new[] { "id", "tipo" }.Concat(CamposBusqueda).Concat(RetrieveFeatids ? new[] { "featids", "capa" } : new string[0]).ToList();
            var busqueda = IgnoreText ? new[] { "*:*" } : CamposBusqueda.Select(c => $"{c}:*{Server.UrlEncode(text)}*");
            var result = JToken.Parse(GetResults(tipos, busqueda, filters, camposSeleccionados));
            var listado = (result["response"]["docs"] as JArray)
                                .Where(elem => !string.IsNullOrEmpty((string)elem["nombre"]))
                                .Select(doc => camposSeleccionados.Aggregate(new JObject(), (obj, campo) => { obj.Add($"{camposSeleccionados.IndexOf(campo)}", doc[campo]); return obj; }))
                                .ToList();

            return Json(JsonConvert.SerializeObject(listado), JsonRequestBehavior.AllowGet);
    }

    // GET: BuscadorGenerico
    public ActionResult Suggests(string text, string tipos, string listaCampos)
    {
        var camposSeleccionados = new[] { "id", "tipo", "nombre" }.Concat((listaCampos ?? string.Empty).Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries));
        return Json(new { Result = GetResults(tipos, new[] { $"*{Server.UrlEncode(text)}*" }, new Dictionary<string, string>(), camposSeleccionados) }, JsonRequestBehavior.AllowGet);
    }

    public ActionResult GetElements(string elements, string tipos)
    {
        return Json(GetResults(tipos, elements.Split(',').Select(elem => $"id:{elem}"), new Dictionary<string, string>(), new string[0]), JsonRequestBehavior.AllowGet);
    }

    private string GetResults(string tipos, IEnumerable<string> busqueda, Dictionary<string, string> filters, IEnumerable<string> fields)
    {
        var server = new SolrServer() { UseDefaultBaseParams = true };

        //para los tipos formo solo un fq con los tipos concatenados por el operador OR. el operador usado entre fq distintos es AND y como quiero
        //dar la posibilidad de buscar en varios tipos al mismo tiempo ese AND no me sirve
        server.AddParam(new SolrParam("fq", $"{string.Join(" OR ", tipos.Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).Select(tipo => $"tipo:{tipo}"))}"));
        foreach (var filter in filters)
        {
            server.AddParam(new SolrParam("fq", $"{filter.Key}:{filter.Value}"));
        }
        if (fields.Any())
        {
            server.AddParam(new SolrParam("fl", $"{string.Join(",", fields)}"));
        }
        return server.Search($"{string.Join(" OR ", busqueda)}");
    }
}
}