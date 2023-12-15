using GeoSit.Data.BusinessEntities.Documentos;
using it = iTextSharp.text;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using System.Drawing;
using System.Net;

namespace GeoSit.Client.Web.Controllers
{
    public class PdfInternalViewerController : Controller
    {
        private readonly string sessionKey = "_docVisualizado_";
        // GET: PdfInternalViewer
        private DocumentoArchivo DocumentoVisualizado
        {
            get
            {
                return Session[sessionKey] as DocumentoArchivo;
            }
            set
            {
                Session[sessionKey] = value;
            }
        }
        public ActionResult View(long id, bool esDocProvincial = false)
        {
            string apiDocURL = ConfigurationManager.AppSettings["webApiUrl"];
            if (esDocProvincial)
            {
                using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
                using (var resp = cliente.GetAsync($"api/GetParametroByClave?clave=ID_MUNICIPIO").Result)
                {
                    if (resp.IsSuccessStatusCode && resp.Content.ReadAsAsync<Data.BusinessEntities.Seguridad.ParametrosGenerales>().Result?.Valor != "XX")
                    {
                        apiDocURL = ConfigurationManager.AppSettings["webApiProvincialUrl"];
                    }
                }
            }
            using (var cliente = new HttpClient() { BaseAddress = new Uri(apiDocURL) })
            using (var resp = cliente.GetAsync($"api/DocumentoService/{id}/File").Result)
            {
                DocumentoVisualizado = resp.Content.ReadAsAsync<DocumentoArchivo>().Result;
                var contenido = DocumentoVisualizado.Contenido;
                if (DocumentoVisualizado.ContentType.ToLower().StartsWith("image/"))
                {
                    using (var img = Image.FromStream(new System.IO.MemoryStream(contenido)))
                    using (var pdfdoc = new it.Document(new it.Rectangle(img.Width, img.Height), 0, 0, 0, 0))
                    using (var outputStream = new System.IO.MemoryStream())
                    {
                        it.pdf.PdfWriter.GetInstance(pdfdoc, outputStream);
                        pdfdoc.Open();
                        it.Image itImage = it.Image.GetInstance(img, new it.BaseColor(Color.Transparent));
                        pdfdoc.Add(itImage);
                        pdfdoc.Close();
                        contenido = outputStream.ToArray();
                    }
                }

                ViewData["filename"] = DocumentoVisualizado.NombreArchivo;
                return PartialView("View", Convert.ToBase64String(contenido));
            }
        }

        public FileResult Download()
        {
            return File(DocumentoVisualizado.Contenido, DocumentoVisualizado.ContentType, DocumentoVisualizado.NombreArchivo);
        }

        public HttpStatusCodeResult CloseViewer()
        {
            DocumentoVisualizado.Contenido = null;
            DocumentoVisualizado = null;
            Session.Remove(sessionKey);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}