using GeoSit.Client.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Http;

namespace GeoSit.Client.Web.Controllers
{
    public class InterfaseRentasController : Controller
    {
        private HttpClient cliente;

        public InterfaseRentasController()
        {
            cliente = new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) };
        }

        public ActionResult Index()
        {
            var resp = cliente.GetAsync("api/InterfaseRentas/GetLogs").Result;
            resp.EnsureSuccessStatusCode();
            var logs = resp.Content.ReadAsAsync<ICollection<InterfaseRentasLogModel>>().Result.ToArray();

            return PartialView(logs);
        }

        public ActionResult Reprocesar(long logId)
        {
            var resp = cliente.PostAsync("api/InterfaseRentas/Reprocesar?logId=" + logId, new StringContent(string.Empty)).Result;
            resp.EnsureSuccessStatusCode();

            return Json(resp.Content.ReadAsAsync<InterfaseRentasLogModel>().Result);
        }

        public ActionResult BuscarPersonas(string nombre, int doc, string cuit)
        {            
            string url = string.Format("api/InterfaseRentas/BuscarPersonas?nombre={0}&doc={1}&cuit={2}", nombre, doc, cuit);
            var resp = cliente.PostAsync(url, new StringContent(string.Empty)).Result;
            resp.EnsureSuccessStatusCode();

            return Json(resp.Content.ReadAsAsync<InterfaseRentasPersonaModel[]>().Result);
        }

        public ActionResult SeleccionarPersonas([FromBody] InterfaseRentasSeleccionPersonasModel[] seleccion)
        {
            return PartialView(seleccion);
        }
    }
}