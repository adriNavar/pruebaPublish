using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Client.Web.Helpers.ExtensionMethods;
using GeoSit.Client.Web.Models.DominioTitular;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Client.Web.Controllers
{
    public class DominioTitularController : Controller
    {
        private readonly HttpClient _client = new HttpClient();

        public DominioTitularController()
        {
            _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
        }

        public ActionResult FormContent(TitularViewModel dominioTitular, long? idClaseParcela)
        {
            using (_client)
            using (var result = _client.GetAsync("api/declaracionjurada/gettipostitularidad").Result)
            {
                result.EnsureSuccessStatusCode();
                var tipos = result.Content.ReadAsAsync<IEnumerable<TipoTitularidad>>().Result;
                bool esParcelaPrescripcion = new Parcela { ClaseParcelaID = idClaseParcela ?? (Session["Parcela"] as Parcela).ClaseParcelaID }.IsPrescripcion();
                var tiposFiltrados = tipos.FilterTiposTitularidad(esParcelaPrescripcion);

                if (tiposFiltrados.All(t => t.IdTipoTitularidad != dominioTitular.TipoTitularidadId))
                {
                    tiposFiltrados = tiposFiltrados.Concat(tipos.Where(t => t.IdTipoTitularidad == dominioTitular.TipoTitularidadId));
                }
                ViewData["EsPrescripcion"] = esParcelaPrescripcion;
                ViewData["TiposTitularidad"] = new SelectList(tiposFiltrados, "IdTipoTitularidad", "Descripcion");

                return PartialView("DominioTitular", dominioTitular);
            }
        }
    }
}