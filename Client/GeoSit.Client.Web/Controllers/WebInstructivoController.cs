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
    public class WebInstructivoController : Controller
    {
        private HttpClient cliente = new HttpClient();
        public WebInstructivoController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET: WebInstructivo
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: /WebInstructivo/DatosWebInstructivo
        public ActionResult DatosWebInstructivo()
        {
            return PartialView(GetWebInstructivo());
        }

        public List<WebInstructivoModel> GetWebInstructivo()
        {
            var usuario = Session["UsuarioPortal"] as UsuariosModel;
            HttpResponseMessage resp = cliente.GetAsync("api/WebInstructivoService/GetWebInstructivo?idUsuario=" + usuario.Id_Usuario).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<WebInstructivoModel>>().Result;
        }
    }
}