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
using GeoSit.Data.BusinessEntities.Documentos;

namespace GeoSit.Client.Web.Controllers
{
    public class AyudaLineaController : Controller
    {
        private HttpClient cliente = new HttpClient();
        public AyudaLineaController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET: AyudaLinea
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: /AyudaLinea/DatosAyudaLinea
        public ActionResult DatosAyudaLinea()
        {
            return PartialView(GetAyudaLinea());
        }

        public List<AyudaLineaModel> GetAyudaLinea()
        {
            var usuario = Session["UsuarioPortal"] as UsuariosModel;
            HttpResponseMessage resp = cliente.GetAsync("api/AyudaLineaService/GetAyudaLinea?idUsuario=" + usuario.Id_Usuario).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<AyudaLineaModel>>().Result;
        }

        public ActionResult View(long id)
        {
            using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            using (var resp = cliente.GetAsync($"api/DocumentoService/{id}/FilePdf").Result)
            {
                var doc = resp.Content.ReadAsAsync<DocumentoArchivo>().Result;
                ViewData["filename"] = doc.NombreArchivo + ".Pdf";
                return PartialView("~/Views/PdfInternalViewer/View.cshtml", Convert.ToBase64String(doc.Contenido));
            }
        }
    }
}