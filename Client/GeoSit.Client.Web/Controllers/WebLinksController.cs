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
using System.Net.Http.Formatting;
using System.Xml;

namespace GeoSit.Client.Web.Controllers
{
    public class WebLinksController : Controller
    {
        private HttpClient cliente = new HttpClient();

        public WebLinksController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET: WebLinks
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: /WebLinks/DatosWebLinks
        public ActionResult DatosWebLinks()
        {
            ViewBag.DatosWebLinks = GetWebLinks();

            var model = new WebLinksModels();
            return PartialView(model);
        }

        public List<WebLinksModel> GetWebLinks()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/WebLinksService/GetWebLinks").Result;
            resp.EnsureSuccessStatusCode();
            return (List<WebLinksModel>)resp.Content.ReadAsAsync<IEnumerable<WebLinksModel>>().Result;
        }

    }
}