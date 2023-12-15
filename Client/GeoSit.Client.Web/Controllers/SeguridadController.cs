using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.Seguridad;
using Newtonsoft.Json;
using GeoSit.Client.Web.Helpers;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Client.Web.Controllers
{
    public class SeguridadController : Controller
    {
        private readonly HttpClient _cliente = null;

        private const int ListadoTareasPorUsuarios = 1;
        private const int RankingTareasPorUsuarios = 2;
        private const int ListadoUsuariosPorTareas = 3;
        private const int RankingUsuariosPorTareas = 4;
        private const int RankingGeneralTareas = 5;
        private const int RankingGeneralTiposComponentesCantidad = 6;

        public SeguridadController()
        {
            _cliente = new HttpClient()
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]),
                Timeout = TimeSpan.FromMinutes(30)
            };
        }

        // GET: /Seguridad/Index
        public ActionResult Index()
        {
            //logger.Debug("Carga Componentes Inicio");
            //ViewBag.listaComponenetes = GetComponentes();    
            //logger.Debug("Carga Componentes Final");
            return PartialView();
        }
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        // GET: /Seguridad/ParametrosGenerales
        public ActionResult ParametrosGenerales()
        {
            return PartialView(GetParametrosGenerales().ToDictionary(p => p.Clave, p => p.Valor));
        }
        public List<ParametrosGeneralesModel> GetParametrosGenerales()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetParametrosGenerales").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<ParametrosGeneralesModel>>().Result;
        }
        public ActionResult ParametrosGeneralesDeep()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetParametrosGenerales").Result;
            resp.EnsureSuccessStatusCode();
            return View("ParametrosGeneralesDeep", (List<ParametrosGenerales>)resp.Content.ReadAsAsync<List<ParametrosGenerales>>().Result);
        }

        public ActionResult ParametrosGeneralesDeepEdit(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Parametro/GetParametro?id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            return View("ParametrosGeneralesDeepEdit", (ParametrosGenerales)resp.Content.ReadAsAsync<ParametrosGenerales>().Result);
        }
        public ActionResult GrabarPGDeep(ParametrosGenerales pg)
        {

            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/SaveParametroGeneral", pg).Result;
            resp.EnsureSuccessStatusCode();
            return RedirectToAction("ParametrosGeneralesDeep");
        }

        //public ActionResult SetParametrosGenerales_Save(SeguridadModel model, string returnUrl, string siguente)
        public ActionResult SetParametrosGenerales_Save(ParametrosGeneralesModel model)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/SetSeguridad_Save/", model).Result;
            if (!resp.IsSuccessStatusCode)
            {
                return Json(new { error = true, mensaje = resp.ReasonPhrase });
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Usuarios(string Mensaje)
        {
            //logger.Debug("Carga Componentes Inicio");
            ViewBag.listaParametrosGenerales = GetParametrosGenerales();
            ViewBag.listaTipoDoc = GetTipoDoc();
            ViewBag.listaUsuarios = GetUsuarios();
            var perfiles = GetPerfiles();
            perfiles.ForEach(p => p.Horario.Perfiles = null);
            ViewBag.listaPerfiles = perfiles.OrderBy(p => p.Nombre).ToList();
            ViewBag.listaSectores = GetSectores();
            ViewBag.listaHorarios = GetHorarios();

            return PartialView(new SeguridadModel() { Mensaje = Mensaje });
        }
        public JsonResult GetUsuarioInterno()
        {
            long idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;
            List<ParametrosGeneralesModel> parametros = GetParametrosGenerales().Where(x => x.Clave == "ID_SECTOR_EXTERNO").ToList();
            foreach (ParametrosGeneralesModel parametro in parametros)
            {
                var datosUsuario = GetUsuarios().Where(x => x.IdSector.ToString() != parametro.Valor).ToList();
                foreach (UsuariosModel user in datosUsuario)
                {
                    if (user.Id_Usuario == idUsuario)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDatosUsuario(long id)
        {
            return Json(GetUsuarioById(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRegistroUsuario(long id)
        {
            var registro = GetUsuariosRegistro(id);
            if (registro != null) registro.Usuarios = null;
            return Json(registro ?? new UsuariosRegistro(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerfilesUsuario(long id)
        {
            var perfiles = GetUsuariosPerfiles(id);
            perfiles.ForEach(p => p.Horarios.HorariosDetalle = null);
            return Json(perfiles ?? new List<UsuariosPerfiles>(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDistritosUsuario(long id)
        {
            return Json(GetUsuariosDistritos(id));
        }
        public ActionResult SetUsuarios_Save(SeguridadModel model, string returnUrl, List<long> PerfilUsuario, List<long> HorarioPerfilUsuario)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);

            model.Usuarios.Usuario_modificacion = usuario.Id_Usuario;
            model.Usuarios.Ip = usuario.Ip;
            model.Usuarios.Machine_Name = usuario.Machine_Name;

            HttpResponseMessage resp_Usuario = _cliente.PostAsJsonAsync("api/SeguridadService/SetUsuarios_Save/", model.Usuarios).Result;
            resp_Usuario.EnsureSuccessStatusCode();
            long idUsuarioGrabado = resp_Usuario.Content.ReadAsAsync<long>().Result;

            model.UsuariosRegistro.Id_Usuario = idUsuarioGrabado;
            model.UsuariosRegistro.Id_Usuario_Registro = usuario.Id_Usuario;
            HttpResponseMessage resp_Registro = _cliente.PostAsJsonAsync("api/SeguridadService/SetUsuariosRegistro_Save/", model.UsuariosRegistro).Result;
            resp_Registro.EnsureSuccessStatusCode();

            var PerfilesAsignados = PerfilUsuario.Select((p, idx) => new UsuariosPerfiles() { Id_Usuario = idUsuarioGrabado, Id_Perfil = p, Id_Horario = HorarioPerfilUsuario[idx], Usuario_Alta = usuario.Id_Usuario }).ToList();
            HttpResponseMessage resp_Perfiles = _cliente.PostAsJsonAsync("api/SeguridadService/SetUsuariosPerfiles_Save/", PerfilesAsignados).Result;
            resp_Perfiles.EnsureSuccessStatusCode();

            model.Mensaje = "ModificacionOK";
            if (model.Usuarios.Id_Usuario == 0)
            {
                model.Mensaje = "AltaOK";
            }
            return RedirectToAction("Usuarios", new { model.Mensaje });
        }

        public JsonResult GetUsuarioByLogin(string id)
        {

            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/GetUsuarioByLogin/?id=" + id, id).Result;
            var Usuario_ID = resp.Content.ReadAsAsync<long>().Result;
            return Json(Usuario_ID);
        }

        public JsonResult GetUsuarioByDocumento(string id, string NroDoc)
        {

            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/GetUsuarioByDocumento/?id=" + id + "&NroDoc=" + NroDoc, id).Result;
            var Usuario_ID = resp.Content.ReadAsAsync<long>().Result;
            return Json(Usuario_ID);
        }
        public UsuariosModel GetUsuarioPorLogin(string login)
        {
            /*
             * si uso <<"api/SeguridadService/GetUsuarioPorLogin/" + login>> haciendo uso del ruteo default definido en la webapi
             * (toma "id" como parametro por default) da error en el caso de que el login contenga un "."
             */
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetUsuarioPorLogin?id=" + login).Result;
            return (UsuariosModel)resp.Content.ReadAsAsync<UsuariosModel>().Result;

        }
        public ActionResult SetUsuarios_Save_AD(SeguridadModel model, string returnUrl, List<long> PerfilAsociado, List<string> DistritoAsociado)
        {
            string Respuesta_Usuario = "";
            string Respuesta_Perfiles = "";

            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            model.Usuarios.Usuario_alta = usuario.Id_Usuario;
            model.Usuarios.Usuario_modificacion = usuario.Id_Usuario;

            HttpResponseMessage resp_Usuario = _cliente.PostAsJsonAsync("api/SeguridadService/SetUsuarios_Save/", model.Usuarios).Result;
            var Usuario_ID = resp_Usuario.Content.ReadAsAsync<long>().Result;

            //MANEJO DE PERFILES ↓
            var PerfilesAsignados = new List<UsuariosPerfilesModel>();
            if (PerfilAsociado != null)
            {

                for (int i = 0; i < PerfilAsociado.Count; i++)
                {

                    var UsuarioPerfil = new UsuariosPerfilesModel();
                    UsuarioPerfil.Id_Usuario = Usuario_ID;
                    UsuarioPerfil.Id_Perfil = PerfilAsociado[i];
                    UsuarioPerfil.Id_Horario = 1;
                    UsuarioPerfil.Usuario_Alta = model.Usuarios.Usuario_modificacion;
                    UsuarioPerfil.Fecha_Alta = model.Usuarios.Fecha_modificacion;
                    PerfilesAsignados.Add(UsuarioPerfil);

                }
            }
            else
            {
                var UsuarioPerfil = new UsuariosPerfilesModel();
                UsuarioPerfil.Id_Usuario = Usuario_ID;
                UsuarioPerfil.Id_Perfil = 0;
                UsuarioPerfil.Id_Horario = 0;
                UsuarioPerfil.Usuario_Alta = model.Usuarios.Usuario_modificacion;
                UsuarioPerfil.Fecha_Alta = model.Usuarios.Fecha_modificacion;
                PerfilesAsignados.Add(UsuarioPerfil);
            }
            //MANEJO DE PERFILES ↑

            //MANEJO DE DISTRITOS ↓
            var DistritosAsignados = new List<UsuariosDistritosModel>();
            if (DistritoAsociado != null)
            {

                for (int i = 0; i < DistritoAsociado.Count; i++)
                {

                    var UsuarioDistrito = new UsuariosDistritosModel();
                    UsuarioDistrito.Id_Usuario = Usuario_ID;
                    UsuarioDistrito.Id_Distrito = DistritoAsociado[i];
                    //UsuarioPerfil.Id_Horario = HorarioPerfilUsuario[i];
                    UsuarioDistrito.Usuario_Alta = model.Usuarios.Usuario_modificacion;
                    UsuarioDistrito.Fecha_Alta = model.Usuarios.Fecha_modificacion;
                    DistritosAsignados.Add(UsuarioDistrito);

                }
            }
            else
            {
                var UsuarioDistrito = new UsuariosDistritosModel();
                UsuarioDistrito.Id_Usuario = Usuario_ID;
                UsuarioDistrito.Id_Distrito = "0";
                //UsuarioPerfil.Id_Horario = HorarioPerfilUsuario[i];
                UsuarioDistrito.Usuario_Alta = model.Usuarios.Usuario_modificacion;
                UsuarioDistrito.Fecha_Alta = model.Usuarios.Fecha_modificacion;
                DistritosAsignados.Add(UsuarioDistrito);
            }
            //MANEJO DE DISTRITOS ↑

            Respuesta_Usuario = resp_Usuario.StatusCode.ToString();
            if (resp_Usuario.StatusCode.ToString() == "OK")
            {
                HttpResponseMessage resp_Perfiles = _cliente.PostAsJsonAsync("api/SeguridadService/SetUsuariosPerfiles_Save/", PerfilesAsignados).Result;
                Respuesta_Perfiles = resp_Perfiles.StatusCode.ToString();
                resp_Perfiles.EnsureSuccessStatusCode();
                if (resp_Perfiles.StatusCode.ToString() == "OK")
                {
                    HttpResponseMessage resp_Distritos = _cliente.PostAsJsonAsync("api/SeguridadService/SetUsuariosDistritos_Save/", DistritosAsignados).Result;
                    resp_Distritos.EnsureSuccessStatusCode();
                }
                else
                {
                    resp_Perfiles.EnsureSuccessStatusCode();
                }

            }
            else
            {
                resp_Usuario.EnsureSuccessStatusCode();
            }

            ViewBag.listaParametrosGenerales = GetParametrosGenerales();
            ViewBag.listaUsuarios = GetUsuarios();
            ViewBag.listaTipoDoc = GetTipoDoc();
            ViewBag.listaPerfiles = GetPerfiles();
            ViewBag.listaDistritos = GetDistritos();
            ViewBag.listaHorarios = GetHorarios();

            if (Respuesta_Usuario == "OK")
            {
                if (model.Usuarios.Fecha_modificacion == null)
                {
                    model.Mensaje = "AltaOK";
                }
                else
                {
                    model.Mensaje = "ModificacionOK";
                }

            }
            else
            {
                model.Mensaje = "Error";
            }

            return PartialView("Usuarios", model);
        }
        public ActionResult SetUsuarios_Delete(SeguridadModel model, string returnUrl)
        {

            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            model.Usuarios.Usuario_baja = usuario.Id_Usuario;

            var FechaNow = DateTime.Now;
            model.Usuarios.Fecha_baja = FechaNow;

            HttpResponseMessage resp_Usuario = _cliente.PostAsJsonAsync("api/SeguridadService/SetUsuarios_Delete", model.Usuarios).Result;

            resp_Usuario.EnsureSuccessStatusCode();
            return RedirectToAction("Usuarios", new { Mensaje = "EliminacionOK" });
        }
        public JsonResult BuscaUsuarioAD(string id)
        {
            if (id == "")
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            //This info can become from web.config
            string userAD = ConfigurationManager.AppSettings["UserAD"]; //ConfigurationSettings.AppSettings["UserAD"];
            string passAD = ConfigurationManager.AppSettings["PassAD"]; //ConfigurationSettings.AppSettings["PassAD"];
            string ldap = "LDAP://" + ConfigurationManager.AppSettings["LDAP"]; //ConfigurationSettings.AppSettings["LDAP"];
            string strSearchAD = "";
            strSearchAD += "(sAMAccountName=" + id + ")";

            List<UsuariosModel> listUsuarios = new List<UsuariosModel>();
            //DirectoryEntry adEntry = new DirectoryEntry("LDAP://Domaincontroler", "user", "pass", AuthenticationTypes.Secure);
            DirectoryEntry adEntry = new DirectoryEntry(ldap, userAD, passAD, AuthenticationTypes.Secure);
            DirectorySearcher adSearch = new DirectorySearcher(adEntry);


            adSearch.Filter = "(&(objectClass=user)" + strSearchAD + ")";

            SearchResultCollection objResultados;
            //adResult = adSearch.FindOne(); // if only i need the first returned user 
            objResultados = adSearch.FindAll();

            foreach (SearchResult MiObjeto in objResultados)
            {
                UsuariosModel usarioToAdd = new UsuariosModel();
                if (MiObjeto.Properties["mail"].Count > 0)
                {
                    usarioToAdd.Mail = MiObjeto.Properties["mail"][0].ToString();
                }
                else
                {
                    usarioToAdd.Mail = "";
                }
                if (MiObjeto.Properties["sAMAccountName"].Count > 0)
                {
                    usarioToAdd.Login = MiObjeto.Properties["sAMAccountName"][0].ToString();
                }
                else
                {
                    usarioToAdd.Login = "";
                }
                if (MiObjeto.Properties["givenName"].Count > 0)
                {
                    usarioToAdd.Nombre = MiObjeto.Properties["givenName"][0].ToString();
                }
                else
                {
                    usarioToAdd.Nombre = "";
                }
                if (MiObjeto.Properties["sn"].Count > 0)
                {
                    usarioToAdd.Apellido = MiObjeto.Properties["sn"][0].ToString();
                }
                else
                {
                    usarioToAdd.Apellido = "";
                }
                if (MiObjeto.Properties["department"].Count > 0)
                {
                    usarioToAdd.Sector = MiObjeto.Properties["department"][0].ToString();
                }
                else
                {
                    usarioToAdd.Sector = "";
                }
                listUsuarios.Add(usarioToAdd);
            }

            return Json(listUsuarios, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Perfiles(string Mensaje)
        {
            ViewBag.listaPerfiles = GetPerfiles();
            ViewBag.listaHorarios = JsonConvert.SerializeObject(GetHorarios().ToArray());
            ViewBag.ArbolFunciones = GetFunciones().Select(f => new Nodo { id = f.Id_Funcion, padre = f.Id_Funcion_Padre, nombre = f.Nombre, esRoot = f.Id_Funcion == f.Id_Funcion_Padre }).ToList();
            ViewBag.ArbolComponentes = GetComponentes().Select(f => new Nodo { id = f.ComponenteId, nombre = f.Nombre, esRoot = true }).ToList();
            ViewBag.TrabajaConHorarios = GetParametrosGenerales().FirstOrDefault(pg => pg.Descripcion == "Horarios en Perfiles")?.Valor;

            var model = new SeguridadModel();
            if (Mensaje == null)
            {
                ViewBag.MensajeSalida = "";
            }
            else
            {
                ViewBag.MensajeSalida = Mensaje;
            }

            model.Mensaje = Mensaje;

            //logger.Debug("Carga Componentes Final");
            return PartialView(model);
        }

        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Consultas()
        {
            var listaReportes = GetTiposReporte();

            ViewBag.esAD = this.esActiveDirectory();
            ViewBag.listaUsuarios = GetUsuarios(true);
            ViewBag.listaFunciones = GetFunciones();

            return PartialView("Reportes", listaReportes);
        }
        private List<TipoReporte> GetTiposReporte()
        {
            return new List<TipoReporte>()
            {
                { new TipoReporte() { IdReporte = ListadoTareasPorUsuarios, Nombre = "Listado de Tareas por Usuarios" } },
                { new TipoReporte() { IdReporte = RankingTareasPorUsuarios, Nombre = "Ranking de Tareas por Usuarios" } },
                { new TipoReporte() { IdReporte = ListadoUsuariosPorTareas, Nombre = "Listado de Usuarios por Tareas" } },
                { new TipoReporte() { IdReporte = RankingUsuariosPorTareas, Nombre = "Ranking de Usuarios por Tareas" } },
                { new TipoReporte() { IdReporte = RankingGeneralTareas, Nombre = "Ranking General de Tareas", UtilizaUsuarios = false } },
                { new TipoReporte() { IdReporte = RankingGeneralTiposComponentesCantidad, Nombre = "Ranking General de Tipos de Componentes y Cantidad", UtilizaUsuarios = false, UtilizaTareas = false } }
            };
        }

        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Auditoria(string Mensaje)
        {
            ViewBag.esAD = this.esActiveDirectory();
            ViewBag.listaUsuarios = GetUsuarios(true);
            ViewBag.listaFunciones = GetFunciones();

            return PartialView();
        }


        public JsonResult GetDatosPerfil(long id)
        {
            return Json(GetPerfilById(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFuncionesPerfil(long id)
        {
            var nodos = GetFuncionesByPerfil(id).Select(f => new Nodo { id = f.Id_Funcion, padre = f.Id_Funcion_Padre, nombre = f.Funcion_Nombre, esRoot = f.Id_Funcion == f.Id_Funcion_Padre }).ToList();
            var grupos = nodos
                            .GroupBy(f => f.padre, (k, v) => new { padre = k, hijos = v.Where(h => !h.esRoot).OrderBy(h => h.id).ToList() })
                            .OrderByDescending(g => g.padre).ToList();

            /*
             * recupero solo los nodos hoja (nodos sin hijos) ya que la logica del armado del arbol se encarga de los nodos intermedios o roots
             * busco los que no tienen una funcion cuyo id sea id_padre en otra, o si lo tiene, que no tenga hijos (en caso de que sea una funcion 
             * suelta como nodo root)
             */
            var hojas = nodos.Where(f => !nodos.Any(c => c.padre == f.id) || !grupos.Any(g => g.padre == f.id && g.hijos.Any()));

            return Json(hojas.Select(f => f.id).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetComponentesPerfil(long id)
        {
            return Json(GetComponentesByPerfil(id).Select(c => c.Id_Componente).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUsuariosPerfil(long id)
        {
            return Json(GetUsuariosByPerfil(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetPerfiles_Save(SeguridadModel model, string returnUrl, List<long> FuncionAsociada, List<long> ComponenteAsociado, UsuariosModel usuarioPortal)
        {

            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            model.Perfiles.Usuario_Modificacion = usuario.Id_Usuario;

            HttpResponseMessage resp_Perfil = _cliente.PostAsJsonAsync("api/SeguridadService/SetPerfiles_Save/", model.Perfiles).Result;
            resp_Perfil.EnsureSuccessStatusCode();
            var grabado = resp_Perfil.Content.ReadAsAsync<Perfiles>().Result;

            //MANEJO DE FUNCIONES ↓
            var funciones = FuncionAsociada?.Select(f => new PerfilFuncion { Id_Perfil = grabado.Id_Perfil, Id_Funcion = f, Usuario_Alta = usuario.Id_Usuario }).ToList() ?? new List<PerfilFuncion>();
            var resp_Funciones = _cliente.PostAsJsonAsync("api/SeguridadService/SetPerfilesFunciones_Save/", funciones).Result;
            resp_Funciones.EnsureSuccessStatusCode();
            //MANEJO DE FUNCIONES ↑

            //MANEJO DE COMPONENTES ↓
            var componentes = ComponenteAsociado?.Select(c => new PerfilesComponentesModel { Id_Perfil = grabado.Id_Perfil, Id_Componente = c, Usuario_Alta = usuario.Id_Usuario }).ToList() ?? new List<PerfilesComponentesModel>();
            var resp_Componentes = _cliente.PostAsJsonAsync("api/SeguridadService/SetPerfilesComponentes_Save/", componentes).Result;
            resp_Componentes.EnsureSuccessStatusCode();
            //MANEJO DE COMPONENTES ↑

            model.Mensaje = "ModificacionOK";
            if (model.Perfiles.Id_Perfil == 0)
            {
                model.Mensaje = "AltaOK";
            }
            return RedirectToAction("Perfiles", new { model.Mensaje });
        }

        public JsonResult GetPerfilByName(string id)
        {
            HttpResponseMessage resp_Perfil = _cliente.PostAsJsonAsync("api/SeguridadService/GetPerfilByName/?id=" + id, id).Result;
            long Perfil_ID = resp_Perfil.Content.ReadAsAsync<long>().Result;
            return Json(Perfil_ID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetPerfiles_Delete(long id, UsuariosModel usuarioPortal)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/SetPerfiles_Delete?id=" + id + "&usuario=" + usuario.Id_Usuario, id).Result;
            resp.EnsureSuccessStatusCode();
            return RedirectToAction("Perfiles", new { Mensaje = "EliminacionOK" });
        }
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Ayuda()
        {


            return PartialView();
        }

        public static bool ExisteFuncion(string funcionNombre)
        {
            long funcionid = Convert.ToInt64(funcionNombre);
            var funcionesHabilitadas = (List<PerfilFuncion>)System.Web.HttpContext.Current.Session["FuncionesHabilitadas"];
            return funcionesHabilitadas.Exists(x => x.Id_Funcion.Equals(funcionid));
        }

        public static string GetUsuarioActivo()
        {
            var usuario = System.Web.HttpContext.Current.Session["usuarioPortal"] as UsuariosModel;
            return usuario != null ? usuario.Login : string.Empty;
        }

        //METODOS DE ACCESO A LA API ↓

        // GET: /Seguridad/Usuarios
        public List<UsuariosModel> GetUsuarios(bool soloInternos = false)
        {
            using (var resp = _cliente.GetAsync($"api/SeguridadService/GetUsuarios{(soloInternos ? $"?soloInternos={true}" : string.Empty)}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<UsuariosModel>>().Result;
            }
        }

        private List<Sector> GetSectores()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetSectores").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Sector>>().Result;
        }

        // GET: /Seguridad/TipoDoc
        public List<TipoDocModel> GetTipoDoc()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetTipoDoc").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<TipoDocModel>>().Result;
        }
        public Usuarios GetUsuarioById(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetUsuarioById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<Usuarios>().Result;
        }

        // GET: /Seguridad/UsuariosRegistro
        public UsuariosRegistro GetUsuariosRegistro(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetUsuariosRegistrobyUsuario/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<UsuariosRegistro>().Result;
        }

        // GET: /Seguridad/UsuariosPerfiles
        public List<UsuariosPerfiles> GetUsuariosPerfiles(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetUsuariosPerfiles/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<UsuariosPerfiles>>().Result;
        }

        private List<PerfilesModel> GetPerfiles()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetPerfiles").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<PerfilesModel>>().Result;
        }
        [HttpGet]
        public JsonResult GetPerfilesJson()
        {
            var perfiles = GetPerfiles();
            perfiles.ForEach(p => p.Horario = null);
            return Json(perfiles, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUsuariosJson()
        {
            return Json(GetUsuarios(), JsonRequestBehavior.AllowGet);
        }
        public PerfilesModel GetPerfilById(long id)
        {
            if (id > 0)
            {
                HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetPerfilById/" + id).Result;
                resp.EnsureSuccessStatusCode();
                var perfil = resp.Content.ReadAsAsync<PerfilesModel>().Result;
                perfil?.Horario?.Perfiles.Clear();
                return perfil;
            }
            return new PerfilesModel();
        }

        public List<PerfilFuncion> GetFuncionesByPerfil(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetFuncionesByPerfil/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PerfilFuncion>)resp.Content.ReadAsAsync<IEnumerable<PerfilFuncion>>().Result;
        }
        public List<PerfilesComponentesModel> GetComponentesByPerfil(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetComponentesByPerfil/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PerfilesComponentesModel>)resp.Content.ReadAsAsync<IEnumerable<PerfilesComponentesModel>>().Result;
        }
        public List<PerfilesUsuariosModel> GetUsuariosByPerfil(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetUsuariosByPerfil/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PerfilesUsuariosModel>)resp.Content.ReadAsAsync<IEnumerable<PerfilesUsuariosModel>>().Result;
        }


        private List<HorariosModel> GetHorarios()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetHorarios").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<HorariosModel>>().Result;
        }
        [HttpGet]
        public JsonResult GetHorariosJson()
        {
            return Json(GetHorarios(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHorariosDetalleJson(long id)
        {
            var detalle = GetHorariosDetalle(id).HorariosDetalle.Select(d => new HorariosDetalleModel { Dia = d.Dia, Hora_Fin = d.Hora_Fin, Hora_Inicio = d.Hora_Inicio, Id_Horario = d.Id_Horario, Id_Horario_Detalle = d.Id_Horario_Detalle });
            return Json(detalle, JsonRequestBehavior.AllowGet);
        }
        private HorariosModel GetHorariosDetalle(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetHorariosDetalle/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (HorariosModel)resp.Content.ReadAsAsync<HorariosModel>().Result;
        }

        public List<DistritosModel> GetDistritos()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetDistritos").Result;
            resp.EnsureSuccessStatusCode();
            return (List<DistritosModel>)resp.Content.ReadAsAsync<IEnumerable<DistritosModel>>().Result;
        }
        // GET: /Seguridad/UsuariosPerfiles
        public List<UsuariosDistritosModel> GetUsuariosDistritos(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetUsuarioDistritos/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<UsuariosDistritosModel>)resp.Content.ReadAsAsync<IEnumerable<UsuariosDistritosModel>>().Result;
        }
        public void BajaHorarios(long id, long usuario)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/BajaHorarios?id=" + id + "&usuario=" + usuario, id).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void GrabarHorarios(HorariosModel horario)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/GrabarHorarios", horario).Result;
            resp.EnsureSuccessStatusCode();
        }

        public void SolicitarCambioPass(UsuariosModel usuario)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/SolicitarCambioPass", usuario).Result;
            resp.EnsureSuccessStatusCode();
        }

        //METODOS DE ACCESO A LA API ↑

        public ActionResult Prueba()
        {
            var model = new SeguridadModel();
            return PartialView(model);
        }

        public JsonResult BajaHorario(long id)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long usuarioId = 0;
            if (usuario == null || usuario.Id_Usuario == 0)
            {
                usuarioId = 1;
            }
            else
            {
                usuarioId = usuario.Id_Usuario;
            }
            BajaHorarios(id, usuarioId);
            return Json("ok");
        }
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]

        public ActionResult Horarios()
        {
            return PartialView("Horarios", GetHorarios());
        }
        public ActionResult GuardarHorario(HorariosModel model, Usuario usuarioPortal)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            for (int i = 0; i < model.HorariosDetalle.Count; i++)
            {
                model.HorariosDetalle[i].Id_Horario = model.Id_Horario;
            }
            model.Usuario_Modificacion = usuario.Id_Usuario;
            GrabarHorarios(model);
            return Json(new object());
        }
        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Feriados()
        {
            ViewBag.Mensaje = "";
            List<FeriadosModel> list = GetFeriadosPorAnio(DateTime.Today.Year);
            return PartialView("Feriados", list);
        }


        public List<FuncionesModel> GetFunciones()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetFunciones").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<FuncionesModel>>().Result;
        }
        public List<EntornosModel> GetEntornos()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetEntornos").Result;
            resp.EnsureSuccessStatusCode();
            return (List<EntornosModel>)resp.Content.ReadAsAsync<IEnumerable<EntornosModel>>().Result;
        }


        public List<ComponenteModel> GetComponentes()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/MapasTematicosService/GetComponentesGeograficos").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;

        }

        public List<FeriadosModel> GetFeriadosPorAnio(long anio)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetFeriados/" + anio).Result;
            resp.EnsureSuccessStatusCode();
            return (List<FeriadosModel>)resp.Content.ReadAsAsync<IEnumerable<FeriadosModel>>().Result;

        }
        public FeriadosModel GetFeriadoById(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetFeriadoById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (FeriadosModel)resp.Content.ReadAsAsync<FeriadosModel>().Result;

        }

        public FeriadosModel GetFeriadoByFecha(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetFeriadoByFecha/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (FeriadosModel)resp.Content.ReadAsAsync<FeriadosModel>().Result;

        }
        public void BajaFeriados(long id, long usuario)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/BajaFeriados?id=" + id + "&usuario=" + usuario, id).Result;
            resp.EnsureSuccessStatusCode();
        }

        public JsonResult BajaFeriado(long id)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long usuarioId = 0;
            if (usuario == null || usuario.Id_Usuario == 0)
            {
                usuarioId = 1;
            }
            else
            {
                usuarioId = usuario.Id_Usuario;
            }
            BajaFeriados(id, usuarioId);
            return Json("ok");
        }

        [HttpGet]
        public JsonResult GetFeriados(long year)
        {
            return Json(GetFeriadosPorAnio(year), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFeriado(long id)
        {
            FeriadosModel list = GetFeriadoById(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFeriadoPorFecha(long id, string fecha)
        {
            DateTime f = DateTime.Parse(fecha);
            return Json(new { existe = GetFeriadosPorAnio(f.Year).Any(feriado => feriado.Id_Feriado != id && feriado.Fecha == f) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GuardarFeriado(Feriados model)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            model._Id_Usuario = usuario.Id_Usuario;
            model._Ip = usuario.Ip;
            model._Machine_Name = usuario.Machine_Name; ;
            var resp = _cliente.PostAsJsonAsync("api/SeguridadService/GrabarFeriados/", new[] { model }.ToList()).Result;
            if (!resp.IsSuccessStatusCode && resp.StatusCode != HttpStatusCode.NotModified)
            {
                return Json(new { error = true, mensaje = resp.ReasonPhrase });
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        public JsonResult UpdateTimeout()
        {
            Session["timeout"] = 0;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void Bloquear()
        {
            Session["blocked"] = true;
        }
        public JsonResult Login(string pass)
        {
            try
            {
                var usuarioMem = (UsuariosModel)Session["usuarioPortal"];
                if (usuarioMem == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var bodyParams = new[]
                {
                    new KeyValuePair<string,string>("user", usuarioMem.Login),
                    new KeyValuePair<string,string>("pass", CalculateMD5Hash(pass))
                };
                using (var resp = _cliente.PostAsync("api/SeguridadService/LoginUsuario/", new FormUrlEncodedContent(bodyParams)).Result)
                {
                    if (resp.IsSuccessStatusCode || resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        var sc = new SeguridadController();
                        var pgm = sc.GetParametrosGenerales();
                        string timeout = pgm.Where(x => x.Id_Parametro == 12).Select(x => x.Valor).FirstOrDefault();

                        Session["blocked"] = !resp.IsSuccessStatusCode;
                        Session["timeout"] = int.Parse(timeout) * 60 * 1000;
                        return Json(new { valido = resp.IsSuccessStatusCode });
                    }
                    else
                    {
                        throw new Exception(resp.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("Login", ex);
                throw;
            }
        }

        // GET: /Seguridad/Reportes
        public List<ReportesModel> GetReportes()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetPerfiles").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ReportesModel>)resp.Content.ReadAsAsync<IEnumerable<ReportesModel>>().Result;
        }


        /////Funciones Exportar excel

        public ActionResult ExportarAExcel()
        {
            long id = 1;

            MapaTematicoConfiguracionModelo modelo = this.GetMapaTematicoById(id);

            ComponenteConfiguracion cc = this.GetComponenteConfiguracionByMTId(id);
            var componenteId = cc.ComponenteId;

            var MT = new DataTable("Export");
            MT.Columns.Add("ConfiguracionId", typeof(long));
            MT.Columns.Add("ComponenteId", typeof(long));
            MT.Rows.Add(modelo.ConfiguracionId, componenteId);

            var vdd = new ViewDataDictionary<MapaTematicoConfiguracionModelo>(modelo);

            var grid = new GridView();
            grid.DataSource = MT;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + modelo.Nombre.Replace(" ", "-") + ".xlsx");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("GetResumenView");
        }
        public MapaTematicoConfiguracionModelo GetMapaTematicoById(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/MapasTematicosService/GetMapaTematicoById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (MapaTematicoConfiguracionModelo)resp.Content.ReadAsAsync<MapaTematicoConfiguracionModelo>().Result;
        }

        public ComponenteConfiguracion GetComponenteConfiguracionByMTId(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/MapasTematicosService/GetComponenteConfiguracionByMTId/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<ComponenteConfiguracion>().Result;
        }

        // GET: /Seguridad/GetConsultas
        public ActionResult GetConsultas(long[] idUsuarios, long[] FuncionAsociada, string fechaDesde, string fechaHasta, int tipo)
        {
            Consultas cons = new Consultas() { tipoInforme = tipo, fechaDesde = fechaDesde, fechaHasta = fechaHasta };

            if (idUsuarios != null)
                cons.idUsuarios = idUsuarios.ToList();

            if (FuncionAsociada != null)
                cons.FuncionAsociada = FuncionAsociada.ToList();

            Func<Consultas, List<ListadoDeTareas>> get = GetDatos;
            if (cons.tipoInforme == RankingGeneralTareas)
            {
                get = GetDatosGeneralTareas;
            }
            else if (cons.tipoInforme == RankingGeneralTiposComponentesCantidad)
            {
                get = GetConsultaTipoObjetoCantidad;
            }
            try
            {
                return GenerarReporte(cons.tipoInforme, get(cons));
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("SeguridadController-GetConsultas", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

        }

        private ActionResult GenerarReporte(int tipo, List<ListadoDeTareas> datos)
        {
            string view = string.Empty;
            switch (tipo)
            {
                case ListadoTareasPorUsuarios:
                    view = "ListadoTareasPorUsuario";
                    break;
                case RankingTareasPorUsuarios:
                    view = "RankingTareasPorUsuarios";
                    break;
                case ListadoUsuariosPorTareas:
                    view = "ListadoUsuariosPorTareas";
                    break;
                case RankingUsuariosPorTareas:
                    view = "RankingUsuariosPorTareas";
                    break;
                case RankingGeneralTareas:
                    view = "RankingGeneralTareas";
                    break;
                case RankingGeneralTiposComponentesCantidad:
                    view = "RankingGeneralTiposComponentesCantidad";
                    break;
            }

            return PartialView($"ResultadoReporte/{view}", datos);
        }

        private List<ListadoDeTareas> GetDatos(Consultas consulta)
        {
            _cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/GetConsultas/", consulta).Result;
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception(resp.Content.ReadAsAsync<string>().Result);
            }
            return resp.Content.ReadAsAsync<IEnumerable<ListadoDeTareas>>().Result.ToList();
        }

        private List<ListadoDeTareas> GetDatosGeneralTareas(Consultas consulta)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/GetConsultaGeneralTareas/", consulta).Result;
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception(resp.Content.ReadAsAsync<string>().Result);
            }
            return resp.Content.ReadAsAsync<IEnumerable<ListadoDeTareas>>().Result.ToList();
        }

        private List<ListadoDeTareas> GetConsultaTipoObjetoCantidad(Consultas consulta)
        {
            HttpResponseMessage resp = _cliente.PostAsJsonAsync("api/SeguridadService/GetConsultaTipoObjetoCantidad/", consulta).Result;
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception(resp.Content.ReadAsAsync<string>().Result);
            }
            return resp.Content.ReadAsAsync<IEnumerable<ListadoDeTareas>>().Result.ToList();
        }

        public ActionResult GetAuditoria(List<long> idUsuarios, List<long> FuncionAsociada, string fechaDesde, string fechaHasta, string contenido)
        {
            Consultas cons = new Consultas() { fechaDesde = fechaDesde, fechaHasta = fechaHasta, contenido = contenido};

            //Filtros de la pantalla
            if (idUsuarios != null)
                cons.idUsuarios = idUsuarios;

            if (FuncionAsociada != null)
                cons.FuncionAsociada = FuncionAsociada;
            try
            {
                return new JsonResult() { Data = GetConsultaAuditoria(cons), MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("SeguridadController-GetAuditoria", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public List<ListadoDeTareas> GetConsultaAuditoria(Consultas consulta)
        {
            _cliente.Timeout = new TimeSpan(1, 0, 0);
            using (var resp = _cliente.PostAsJsonAsync("api/SeguridadService/GetConsultaAuditoria/", consulta).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception(resp.Content.ReadAsAsync<string>().Result);
                }
                return resp.Content.ReadAsAsync<IEnumerable<ListadoDeTareas>>().Result.ToList();
            }
        }

        // POST: /Seguridad/MostrarObjetoActualizado
        public ActionResult MostrarObjetoActualizado(long id)
        {
            try
            {
                return Json(GetMostrarObjetoActualizado(id));
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("SeguridadController-MostrarObjetoActualizado", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: /Seguridad/GetConsultaTipoObjetoCantidad
        public ListadoDeTareas GetMostrarObjetoActualizado(long id)
        {
            using (var resp = _cliente.GetAsync("api/SeguridadService/GetMostrarObjetoActualizado/" + id).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception(resp.Content.ReadAsAsync<string>().Result);
                }
                return resp.Content.ReadAsAsync<ListadoDeTareas>().Result;
            }
        }

        // POST: /Seguridad/MostrarObjetoActualizado
        public ActionResult MostrarObjetoHistorico(long id)
        {
            try
            {
                return Json(GetMostrarObjetoHistorico(id));
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("SeguridadController-MostrarObjetoHistorico", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // GET: /Seguridad/GetConsultaTipoObjetoCantidad
        public ListadoDeTareas GetMostrarObjetoHistorico(long id)
        {
            using (var resp = _cliente.GetAsync("api/SeguridadService/GetMostrarObjetoHistorico/" + id).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception(resp.Content.ReadAsAsync<string>().Result);
                }
                return resp.Content.ReadAsAsync<ListadoDeTareas>().Result;
            }
        }

        // POST: /Seguridad/GetFuncionesArbol
        public JsonResult GetFuncionesArbol(long id, long idPadre)
        {

            return Json(FuncionesArbol(id, idPadre));
        }

        // GET: /Seguridad/FuncionesArbol
        public ListadoDeTareas FuncionesArbol(long id, long idPadre)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/FuncionesArbol?id=" + id + "&idPadre=" + idPadre).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<ListadoDeTareas>().Result;
        }
        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public void RegisterAuditoria(System.Web.HttpRequestBase request, long idUsuario, string funcion, string eventoNombre, string desc)
        {
            var funcionesHabilitadas = (List<PerfilFuncion>)System.Web.HttpContext.Current.Session["FuncionesHabilitadas"];
            PerfilFuncion pf = funcionesHabilitadas.Where(x => x.Id_Funcion == Convert.ToInt64(funcion)).FirstOrDefault();
            HttpResponseMessage resp = _cliente.GetAsync("api/SeguridadService/GetEventoIdsByIdFuncion?id=" + pf.Id_Funcion).Result;
            resp.EnsureSuccessStatusCode();
            Dictionary<long, string> eventos = (Dictionary<long, string>)resp.Content.ReadAsAsync<Dictionary<long, string>>().Result;
            KeyValuePair<long, string> evento = eventos.Where(x => x.Value.Trim().ToLower().Equals(eventoNombre.ToLower())).FirstOrDefault();
            AuditoriaHelper.Register(idUsuario, desc, request, pf.Id_Funcion.ToString(), Autorizado.Si, evento.Key.ToString());
        }

        public string GuidEncode(string guid, object data)
        {
            string output = string.Empty;
            var key = Encoding.UTF8.GetBytes(Regex.Replace(guid, @"\W+", string.Empty)).Reverse().ToArray();
            var iv = key.Where((x, i) => i % 2 == 0).ToArray();

            using (var aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(JsonConvert.SerializeObject(data));
                    }
                    output = Convert.ToBase64String(ms.ToArray());
                }
            }
            return output;
        }

        private bool esActiveDirectory()
        {
            var parametros = GetParametrosGenerales();
            return parametros.Any(x => x.Descripcion == "Active_Directory" && x.Valor == "1");
        }

        [HttpPost]
        public JsonResult ValidarPassword(Usuario model)
        {
            var errores = new AccountController().ValidarPassword(model, out UsuariosRegistroModel _);
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                errores.Add("Las contraseñas proporcionadas son diferentes");
            }
            return Json(errores);
        }
    }
}