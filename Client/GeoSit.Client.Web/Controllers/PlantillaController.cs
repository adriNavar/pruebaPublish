using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using System.Reflection;
using GeoSit.Client.Web.Helpers;
using System.Web.UI.WebControls;
using System.Web.UI;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Client.Web.Controllers
{
    public class PlantillaController : Controller
    {
        private UsuariosModel Usuario { get { return (UsuariosModel)Session["usuarioPortal"]; } }

        private readonly HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });
        private const string ApiUri = "api/plantilla/";
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public PlantillaController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult Index()
        {
            Session["UnidadPloteoPredefinido"] = new UnidadPloteoPredefinido();
            return PartialView("Plantilla", new List<Plantilla>());
        }

        public string List(int id = 0)
        {
            var result = _cliente.GetAsync(ApiUri + "get").Result;
            result.EnsureSuccessStatusCode();
            var plantillasAll = (List<Plantilla>)result.Content.ReadAsAsync<IEnumerable<Plantilla>>().Result;
            List<Plantilla> plantillas = plantillasAll;
            //id = MP_PLANTILLA.VISIBILIDAD 0: Publica / 1: Privada
            /*if (id == 1)
            {
                long idUsuario = Usuario.Id_Usuario;
                //plantillas = plantillasAll.Where(p => p.Visibilidad == 1 && p.IdUsuarioAlta == idUsuario).ToList();
                plantillas = plantillasAll.Where(p => (p.Visibilidad == 1 && p.IdUsuarioAlta == idUsuario) || (p.Visibilidad == 1 && p.IdUsuarioModificacion == idUsuario)).ToList();
            }
            else
            {
                plantillas = plantillasAll.Where(p => p.Visibilidad == 0).ToList();
            }*/
            var plantillasJson = new List<dynamic>();
            foreach (var p in plantillas)
            {
                string hoja = "";
                string componente = "";
                string visibilidad = "";
                if (p.Hoja != null)
                    hoja = p.Hoja.Nombre;
                if (p.Layers != null && p.Layers.Count > 0)
                {
                    var layer = p.Layers.FirstOrDefault(l => l.Categoria == 1);
                    if (layer != null)
                        componente = layer.Componente.Nombre;
                }
                string orientacion = "Horizontal";
                if (p.Orientacion == 1)
                {
                    orientacion = "Vertical";
                }
                string categoria = (p.PlantillaCategoria != null ? p.PlantillaCategoria.Nombre : string.Empty);
                if (p.Visibilidad == 1)
                {
                    visibilidad = "Privada";
                }
                else
                {
                    visibilidad = "Pública";
                }
                plantillasJson.Add(new
                {
                    p.IdPlantilla,
                    Categoria = categoria,
                    p.Nombre,
                    Hoja = hoja,
                    Orientacion = orientacion,
                    Componente = componente,
                    Visibilidad = visibilidad
                });
            }

            return "{\"data\":" + JsonConvert.SerializeObject(plantillasJson) + "}";
        }

        public ActionResult Partial()
        {
            return PartialView("~/Views/Plantilla/Partial/_ListaPloteo.cshtml", new List<Plantilla>());
        }

        public ActionResult FormContent()
        {
            var viewPlantilla = new ViewPlantilla();
            HttpResponseMessage response = null;
            try
            {
                response = _cliente.GetAsync("api/hoja/get").Result;
                response.EnsureSuccessStatusCode();
                var hojas = (List<Hoja>)response.Content.ReadAsAsync<IEnumerable<Hoja>>().Result;
                viewPlantilla.HojaList = new SelectList(hojas, "IdHoja", "Nombre");
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("hoja", ex);
            }
            try
            {
                var installedFontCollection = new InstalledFontCollection();
                var fuentes = installedFontCollection.Families;

                viewPlantilla.FuenteList = new SelectList(fuentes, "Name", "Name");
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("installedFontCollection", ex);
            }

            try
            {
                response = _cliente.GetAsync("api/norte/gethttp").Result;
                response.EnsureSuccessStatusCode();
                var nortes = (List<Norte>)response.Content.ReadAsAsync<IEnumerable<Norte>>().Result;

                nortes.ForEach(n =>
                {
                    n.IBytes = Encoding.Default.GetBytes(n.SBytes);
                    n.SBytes = null;
                    n.Imagen.Save(Path.Combine(_uploadPath, "norte" + n.IdNorte + "." + n.IType), n.ImagenFormat);
                });

                viewPlantilla.NorteList = new SelectList(nortes, "IdNorte", "Nombre");
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("nortes", ex);
            }

            try
            {
                response = _cliente.GetAsync("api/funcionadicional/get").Result;
                response.EnsureSuccessStatusCode();
                var funcionAdicionales =
                    (List<FuncionAdicional>)response.Content.ReadAsAsync<IEnumerable<FuncionAdicional>>().Result;
                funcionAdicionales.Insert(0, new FuncionAdicional { IdFuncionAdicional = 0, Nombre = "(Ninguna)" });

                viewPlantilla.FuncionAdicionalList = new SelectList(funcionAdicionales, "IdFuncionAdicional", "Nombre");
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("funcionadicional", ex);
            }
            try
            {
                response = _cliente.GetAsync("api/plantillacategoria/get").Result;
                response.EnsureSuccessStatusCode();
                var plantillaCategorias = response.Content.ReadAsAsync<IEnumerable<PlantillaCategoria>>().Result;

                viewPlantilla.PlantillaCategoriaList = new SelectList(plantillaCategorias, "IdPlantillaCategoria", "Nombre");
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("plantillacategoria", ex);
            }
            return PartialView("~/Views/Plantilla/Partial/_PlantillaAjaxForm.cshtml", viewPlantilla);
        }

        public ActionResult Save(ViewPlantilla viewPlantilla, HttpPostedFileBase pdf)
        {
            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;
            HttpResponseMessage response;

            var result = new JsonResult();

            var estilo = viewPlantilla.Negrita ? "1" : "0";
            estilo += viewPlantilla.Cursiva ? ",1" : ",0";
            estilo += viewPlantilla.Tachada ? ",1" : ",0";
            estilo += viewPlantilla.Subrayada ? ",1" : ",0";

            viewPlantilla.ReferenciaFuenteEstilo = estilo;
            viewPlantilla.ReferenciaColor = "#" + viewPlantilla.ReferenciaColor;
            viewPlantilla.IdUsuarioModificacion = Usuario.Id_Usuario;
            viewPlantilla.IdFuncionAdicional = viewPlantilla.IdFuncionAdicional == 0 ? null : viewPlantilla.IdFuncionAdicional;

            string tipoOperacionAuditoria = Resources.TipoOperacion.Alta;
            string eventoAuditoria = Eventos.AltaDePlantilla;

            Operation operacion = Operation.Add;
            if (viewPlantilla.IdPlantilla == 0)
            {
                result.Data = "Ok";
            }
            else
            {
                tipoOperacionAuditoria = Resources.TipoOperacion.Modificacion;
                eventoAuditoria = Eventos.ModificacionDePlantilla;
                operacion = Operation.Update;
                result.Data = "Ok_Edit";
            }

            if (pdf != null)
            {
                MemoryStream memstr = new MemoryStream();
                pdf.InputStream.CopyTo(memstr);
                admin.OperacionPlantillaFondo = new OperationItem<PlantillaFondo>
                {
                    Operation = Operation.Add,
                    Item = new PlantillaFondo { ImagenNombre = viewPlantilla.Pdf.Split('.')[0], IBytes = memstr.ToArray() }
                };
            }

            admin.OperacionPlantilla = new OperationItem<Plantilla> { Item = viewPlantilla.GetPlantilla(), Operation = operacion };

            response = _cliente.PostAsync("api/plantilla/post", new ObjectContent<UnidadPloteoPredefinido>(admin, new JsonMediaTypeFormatter())).Result;

            response.EnsureSuccessStatusCode();
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, tipoOperacionAuditoria, Autorizado.Si, eventoAuditoria);
            admin = new UnidadPloteoPredefinido();
            Session["UnidadPloteoPredefinido"] = admin;

            return result;
        }

        public ActionResult CancelAll()
        {

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Delete(int id)
        {
            var result = _cliente.GetAsync(ApiUri + "Borrar?id=" + id + "&idUsuario=" + Usuario.Id_Usuario).Result;
            result.EnsureSuccessStatusCode();
            AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Baja, Autorizado.Si, Eventos.BajaDePlantilla);
            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Copy(string id, string nombre = null)
        {
            var response = _cliente.PostAsync(ApiUri + string.Format("post?id={0}&idUsuario={1}&nombre={2}", id, Usuario.Id_Usuario, nombre), null).Result;
            response.EnsureSuccessStatusCode();
            //AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Alta, Autorizado.Si, Eventos.AltaDePlantilla);
            return new JsonResult { Data = "Ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult CambiarVisibilidad(int id)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/Plantilla/CambiarVisibilidad?id=" + id + "&idUsuario=" + Usuario.Id_Usuario, id).Result;
            resp.EnsureSuccessStatusCode();
            var result = resp.Content.ReadAsAsync<string>().Result;
            //string funcion = result == "0" ? Seguridad.DespublicarBibliotecaMT : Seguridad.PublicarBibliotecaMT;
            //string evento = result == "0" ? Eventos.DespublicarBibliotecaMT : Eventos.PublicarBibliotecaMT;
            //RegisterAuditoria(Request, UsuarioConectado.Id_Usuario, funcion, evento, "Cambiar Visibilidad Mapa Temático");
            return Json(result);
        }

        public ActionResult Plantilla(int id)
        {
            var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var plantilla = result.Content.ReadAsAsync<Plantilla>().Result;
            PlantillaFondo fondo = plantilla.PlantillaFondos != null ? plantilla.PlantillaFondos.FirstOrDefault() : null;
            plantilla.Hoja.Plantillas.Clear();
            if (plantilla.Layers != null) plantilla.Layers.Clear();
            if (plantilla.PlantillaCategoria != null) plantilla.PlantillaCategoria.Plantillas.Clear();
            if (plantilla.PlantillaEscalas != null) plantilla.PlantillaEscalas.Clear();
            if (plantilla.PlantillaTextos != null) plantilla.PlantillaTextos.Clear();
            if (plantilla.PlantillaFondos != null) plantilla.PlantillaFondos.Clear();
            if (plantilla.FuncionAdicional != null)
            {
                plantilla.FuncionAdicional.Plantillas.Clear();
                plantilla.FuncionAdicional.FuncAdicParametros.Clear();
            }

            Session["Plantilla"] = plantilla;

            if (fondo != null && fondo.IBytes != null)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        plantilla = plantilla,
                        fondo = new
                        {
                            nombre = fondo.ImagenNombre,
                            content = Convert.ToBase64String(fondo.IBytes)
                        }
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            return new JsonResult { Data = new { plantilla = plantilla }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public FileResult ExportarPlantilla(int id, string nombre)
        {
            return File(Encoding.UTF8.GetBytes(id.ToString()), "text/txt", nombre + ".txt");
        }

        [HttpPost]
        public ActionResult ImportarPlantilla(string nombrePlantilla)
        {
            var file = Request.Files[0];
            try
            {
                var reader = new StreamReader(file.InputStream);

                if (ValidarNombrePlantilla(nombrePlantilla))
                {
                    return Json(new { success = false, msg = "El nombre especificado ya existe." }, JsonRequestBehavior.AllowGet);
                }
                string idPlantilla = reader.ReadLine();
                int idPlantillaNro = 0;
                bool result = Int32.TryParse(idPlantilla, out idPlantillaNro);
                if (!result)
                {
                    return Json(new { success = false, msg = "El archivo no contiene un id de plantilla válido." }, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Copy", "Plantilla", new { id = idPlantilla, nombre = nombrePlantilla });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "Error al leer el archivo." }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool ValidarNombrePlantilla(string nombrePlantilla)
        {
            var result = _cliente.GetAsync(ApiUri + string.Format("ValidarNombre?usuarioId={0}&nombre={1}", Usuario.Id_Usuario.ToString(), nombrePlantilla)).Result;
            result.EnsureSuccessStatusCode();

            return result.Content.ReadAsAsync<bool>().Result;
        }
        public JsonResult GetUsuarioPerfil()
        {
            long idPerfil = 0;
            try
            {
                HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetUsuariosPerfiles/" + Usuario.Id_Usuario.ToString()).Result;
                resp.EnsureSuccessStatusCode();
                List<UsuariosPerfiles> lstUsuarioPerfiles = (List<UsuariosPerfiles>)resp.Content.ReadAsAsync<IEnumerable<UsuariosPerfiles>>().Result;
                if (lstUsuarioPerfiles != null && lstUsuarioPerfiles.Count > 0)
                {
                    if (lstUsuarioPerfiles.Any(p => p.Id_Perfil == 1))
                    {
                        idPerfil = 1;
                    }
                    else
                    {
                        idPerfil = lstUsuarioPerfiles[0].Id_Perfil;
                    }
                }
            }
            catch (Exception)
            {
            }
            //return Json(result);
            return Json(new { IdPerfil = idPerfil }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidarFiltro(int idComponente, string filtro)
        {
            bool valid = true;
            if (filtro != null && filtro.Length > 0) {
                try
                {
                    var result = _cliente.GetAsync(ApiUri + string.Format("ValidarFiltro?idComponente={0}&filtro={1}", idComponente.ToString(), filtro)).Result;
                    result.EnsureSuccessStatusCode();
                    valid = result.Content.ReadAsAsync<bool>().Result;
                }
                catch (Exception e) {
                    valid = false;
                }
            }
            return Json(new { valid = valid });
        }
    }
    
}
