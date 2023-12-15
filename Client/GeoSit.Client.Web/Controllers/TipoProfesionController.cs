using System;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Configuration;

namespace GeoSit.Client.Web.Controllers
{
    public class TipoProfesionController : Controller
    {
        private HttpClient cliente = new HttpClient();
        
        public TipoProfesionController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET: /TipoPrefesion/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: /TipoProfesion/TiposProfesion
        public ActionResult TiposProfesion()
        {
            ViewBag.listaTiposProfesion = GetTiposProfesion();
            return View();
        }

        public List<TiposProfesionesModel> GetTiposProfesion()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoProfesion/GetTiposProfesion").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TiposProfesionesModel>)resp.Content.ReadAsAsync<IEnumerable<TiposProfesionesModel>>().Result;
        }

    }
}
