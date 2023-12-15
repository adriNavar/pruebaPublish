using System;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Configuration;
using GeoSit.Data.BusinessEntities.Documentos;

namespace GeoSit.Client.Web.Controllers
{
    public class DocumentoController : Controller
    {
        private bool EsSoloLectura
        {
            set { Session["SoloLectura"] = value; }
            get { return Session["SoloLectura"] != null && (bool)Session["SoloLectura"]; }
        }
        private HttpClient cliente = new HttpClient();

        public DocumentoController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET: Documento
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: /Documento/DatosDocumento
        public ActionResult DatosDocumento(string id, bool esMantenedorParcelario = false)
        {
            bool nuevo = !long.TryParse(id, out long idDocumento) || idDocumento == 0;

            var documento = GetDatosDocumentoById(idDocumento);

            var tiposDocumento = GetTipoDocumento();

            if (esMantenedorParcelario)
            {
                tiposDocumento = tiposDocumento.Where(x => x.Value != "7" && x.Value != "15").ToList();
            }

            var tipoDocumento = GetTipoDocumentoById(nuevo ? tiposDocumento.First().Value : documento.id_tipo_documento.ToString());


            ViewBag.Editable = tipoDocumento.EsEditable;
            ViewBag.Eliminable = tipoDocumento.EsEliminable;

            ViewData["tiposdoc"] = new SelectList(tiposDocumento, "Value", "Text");

            ViewBag.DatosDocumento = documento;
            ViewBag.SoloLectura = EsSoloLectura;
            ViewBag.Nuevo = nuevo;

            ViewBag.MensajeSalida = "";

            ViewBag.archivo2 = new byte[] { 0 };
            ViewBag.BytesArray = null;
            if (!nuevo)
            {
                try
                {
                    var archivo = getArchivo(idDocumento);
                    if (archivo != null)
                    {
                        ViewBag.archivo2 = archivo.Contenido;
                        ViewBag.BytesArray = $"data:{archivo.ContentType};base64,{Convert.ToBase64String(archivo.Contenido)}";
                    }
                }
                catch (Exception ex)
                {
                    MvcApplication.GetLogger().LogError($"DatosDocumento/{id}", ex);
                }
            }
            EsSoloLectura = false;
            return PartialView();
        }

        public DocumentoModel GetDatosDocumentoById(long id)
        {
            try
            {
                using (var resp = cliente.GetAsync("api/DocumentoService/GetDocumentoById/" + id).Result.EnsureSuccessStatusCode())
                {
                    return resp.Content.ReadAsAsync<DocumentoModel>().Result;
                }
            }
            catch
            {
                return new DocumentoModel();
            }
        }

        public ActionResult GetDatosDocumentoByIdJson(long id)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/DocumentoService/GetDocumentoById/" + id).Result;
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<DocumentoModel>().Result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new DocumentoModel(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_DatosDocumento(DocumentoModels Save_Datos, HttpPostedFileBase archivo2)
        {
            if (archivo2 != null)
            {
                var nombre = archivo2.FileName;
                byte[] result;
                using (var streamReader = new MemoryStream())
                {
                    archivo2.InputStream.CopyTo(streamReader);
                    result = streamReader.ToArray();
                };
                Save_Datos.DatosDocumento.contenido = result;
                var base64 = Convert.ToBase64String(result);
                var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
                ViewBag.BytesArray = imgSrc;
            }

            Save_Datos.DatosDocumento.extension_archivo = "txt";
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/DocumentoService/SetDocumento_Save", Save_Datos.DatosDocumento).Result;
            resp.EnsureSuccessStatusCode();
            ViewBag.MensajeSalida = resp.StatusCode.ToString();

            ViewBag.DatosDocumento = GetDatosDocumentoById(0);
            ViewData["tiposdoc"] = new SelectList(GetTipoDocumento(), "Value", "Text");
            return PartialView("DatosDocumento");
        }

        private DocumentoArchivo getArchivo(long id)
        {
            using (var resp = cliente.GetAsync($"api/DocumentoService/{id}/File").Result)
            {
                return resp.Content.ReadAsAsync<DocumentoArchivo>().Result;
            }
        }
        public ActionResult Download(long id)
        {
            var doc = getArchivo(id);
            string nombreArchivo = $"{doc.NombreArchivo.Split('.').First()}.{doc.Extension_archivo.Split('.').Last()}";
            return File(doc.Contenido, doc.ContentType, nombreArchivo);
        }

        public void DownloadServerFile(long id)
        {
            var docFile = getArchivo(id);
            Response.Clear();
            Response.AddHeader("Content-Disposition", $"attachment; filename={docFile.NombreArchivo}");
            Response.AddHeader("Content-Length", docFile.Contenido.Length.ToString());
            Response.ContentType = docFile.ContentType;
            Response.Write(docFile.Contenido);
            Response.End();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update_DatosDocumento(DocumentoModels Save_Datos)
        {
            Save_Datos.DatosDocumento.id_usu_modif = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;
            if (Save_Datos.archivo2 != null)
            {
                using (var memStr = new MemoryStream())
                {
                    Save_Datos.archivo2.InputStream.CopyTo(memStr);
                    Save_Datos.DatosDocumento.contenido = memStr.ToArray();
                }
            }

            if (!string.IsNullOrEmpty(Save_Datos.DatosDocumento.atributos))
            {
                Save_Datos.DatosDocumento.atributos = Save_Datos.DatosDocumento.atributos.Replace("!", "<").Replace("¡", ">");
            }

            using (var resp = cliente.PostAsJsonAsync("api/DocumentoService/SetDocumento_Save", Save_Datos.DatosDocumento).Result)
            {
                resp.EnsureSuccessStatusCode();
                var grabado = resp.Content.ReadAsAsync<DocumentoModel>().Result;
                var jsonResult = Json(grabado);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        public List<SelectListItem> GetTipoDocumento()
        {
            List<SelectListItem> itemsTiposDocumento = new List<SelectListItem>();
            foreach (var tipoDoc in GetTiposDocumentos())
            {
                itemsTiposDocumento.Add(new SelectListItem { Text = tipoDoc.Descripcion, Value = Convert.ToString(tipoDoc.TipoDocumentoId) });
            }
            return itemsTiposDocumento;
        }

        public List<TiposDocumentosModel> GetTiposDocumentos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoDocumentoService/GetTiposDocumento").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TiposDocumentosModel>)resp.Content.ReadAsAsync<IEnumerable<TiposDocumentosModel>>().Result;
        }

        public ActionResult GetTiposDocumentosJson()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoDocumentoService/GetTiposDocumento").Result;
            resp.EnsureSuccessStatusCode();
            return Json((List<TiposDocumentosModel>)resp.Content.ReadAsAsync<IEnumerable<TiposDocumentosModel>>().Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTamanioMaxArchivo()
        {
            string tamanio = ConfigurationManager.AppSettings["TamanioMaxArchivo"];
            return Json(tamanio);
        }

        public JsonResult GetTipoDocumentoByIdJson(string id)
        {
            return Json(GetTipoDocumentoById(id), JsonRequestBehavior.AllowGet);
        }

        public void SoloLectura()
        {
            EsSoloLectura = true;
        }
        public void Editable()
        {
            EsSoloLectura = false;
        }

        public TiposDocumentosModel GetTipoDocumentoById(string id)
        {
            using (var resp = cliente.GetAsync($"api/TipoDocumentoService/GetTipoDocumentoById/{id}").Result.EnsureSuccessStatusCode())
            {
                var result = resp.Content.ReadAsAsync<TiposDocumentosModel>().Result;
                if (result != null)
                {
                    result.EsEliminable &= !EsSoloLectura;
                    result.EsEditable &= !EsSoloLectura;
                }
                return result;
            }
        }

        public string GetDataBaseState()
        {
            HttpResponseMessage resp0 = cliente.GetAsync("api/DocumentoService/GetDataBaseState").Result;
            resp0.EnsureSuccessStatusCode();
            return resp0.Content.ReadAsAsync<string>().Result;
        }

        public int SearchSameFile(string nombreArchivo)
        {
            if (GetDataBaseState() == "1")
            {
                return 2;
            }
            else
            {
                HttpResponseMessage respUrl = cliente.GetAsync("api/DocumentoService/GetUrl").Result;
                respUrl.EnsureSuccessStatusCode();
                string ServerUrl = respUrl.Content.ReadAsAsync<string>().Result;

                if (System.IO.Directory.Exists(ServerUrl))
                {
                    string path = Path.Combine(ServerUrl, Path.GetFileName(nombreArchivo));
                    if (System.IO.File.Exists(path))
                        return 1;
                    else
                        return 2;
                }
                else
                    return 3;
            }

        }

        [HttpGet]
        public ActionResult _ObjetoDocumento(string TipoId, long Id)
        {
            TiposDocumentosModel Objeto = GetTipoDocumentoById(TipoId);
            Objeto.Esquema = Objeto.Esquema ?? "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                                                    "<xsd:schema xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                                                                                    "<xsd:element name=\"Datos\">" +
                                                                                    "<xsd:complexType>" +
                                                                                           "<xsd:sequence>" +
                                                                                           //"<xsd:element name=\"Descripcion\" type=\"xsd:string\"/>" +
                                                                                           "</xsd:sequence>" +
                                                                                       "</xsd:complexType>" +
                                                                                   "</xsd:element>" +
                                                                               "</xsd:schema>";
            ViewBag.Esquema = Objeto.Esquema;
            var datosDocumento = GetDatosDocumentoById(Id);
            ViewBag.atributos = datosDocumento._atributos;
            datosDocumento = null;
            return PartialView("_ObjetoDocumento");
        }
    }
}