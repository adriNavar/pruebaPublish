using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Mapa;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Client.Web.Controllers
{
    public class HomeController : Controller
    {
        private List<PerfilFuncion> FuncionesHabilitadas { get { return Session["FuncionesHabilitadas"] as List<PerfilFuncion>; } }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Index()
        {
            if (this.FuncionesHabilitadas == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.GenerarMT = Session["generarMT"];
            ViewBag.Menu = GetMenu();
            ViewBag.SRID = GetMapSRID();
            Session["GenerarResultado"] = 1;
            Session["MapaTematicoGenerado"] = new MapaTematicoModel();

            // Configuración de Interfases
            using (var sc = new SeguridadController())
            {
                var param = sc.GetParametrosGenerales().FirstOrDefault(x => x.Clave == "IST_ENABLED")?.Valor;
                ViewBag.InterfaseRentasHabilitada = param ?? "0";

                param = sc.GetParametrosGenerales().FirstOrDefault(x => x.Clave == "IDGC_LAYERS_REPORTE_GENERAL_INMUEBLE")?.Valor ?? string.Empty;
                ViewBag.LayersReporteGeneralInmueble = param.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

                param = sc.GetParametrosGenerales().FirstOrDefault(x => x.Clave == "IDGC_LAYERS_COPIA_PARCELAS")?.Valor ?? string.Empty;
                ViewBag.LayersCopiaParcelas = param.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

                param = sc.GetParametrosGenerales().FirstOrDefault(x => x.Clave == "IDGC_LAYERS_DGC")?.Valor ?? string.Empty;
                ViewBag.LayersDGC = param.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

                param = sc.GetParametrosGenerales().FirstOrDefault(x => x.Id_Parametro == 12)?.Valor ?? "1";
                Session["timeout"] = int.Parse(param) * 60 * 1000;

                param = sc.GetParametrosGenerales().FirstOrDefault(x => x.Clave == "ID_MUNICIPIO")?.Valor ?? string.Empty;
                Session["cod_municipio"] = param;
            }

            //Limpieza de MT_OBJETO_RESULTADO 
            MvcApplication.GetInstance().BorrarMapasTematicos(Session["usuarioPortal"] as UsuariosModel);

            ViewBag.GoogleApiKey = ConfigurationManager.AppSettings["GoogleApiKey"];
            ViewBag.ShowTempDataWarning = Session["showafterlogin"] == null;
            Session["showafterlogin"] = false;
            return View();
        }

        private SRIDDefinition GetMapSRID()
        {
            using (var client = getHttpClient())
            using (var resp = client.GetAsync("api/genericoservice/getmapsrid").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<SRIDDefinition>().Result;
            }
        }

        private HttpClient getHttpClient()
        {
            return new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials })
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"])
            };
        }

        public JsonResult GetMapInitialCoords()
        {
            return Json(new { center = new { lat = Convert.ToDouble(ConfigurationManager.AppSettings["latitudInicial"]), lon = Convert.ToDouble(ConfigurationManager.AppSettings["longitudInicial"]) }, zoom = Convert.ToInt32(ConfigurationManager.AppSettings["zoomInicial"]) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPermisos(string[] permisos)
        {
            var permisosList = this.FuncionesHabilitadas.Select(m => m.Id_Funcion_Padre.ToString()).Intersect(permisos).ToList();
            permisosList.AddRange(this.FuncionesHabilitadas.Select(m => m.Id_Funcion.ToString()).Intersect(permisos));

            return Json(permisosList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        private List<MenuItemModels> GetMenu()
        {
            //Call HttpClient.GetAsync to send a GET request to the appropriate URI   
            var resp = getHttpClient().GetAsync("api/GenericoService/GetMenuItem").Result;
            //This method throws an exception if the HTTP response status is an error code.  
            resp.EnsureSuccessStatusCode();
            var listaMenu = (List<MenuItemModels>)resp.Content.ReadAsAsync<IEnumerable<MenuItemModels>>().Result;

            Session["Menu"] = listaMenu;//REVISAR

            /*
             * El objetivo de este codigo es agrupar los registros de la tabla menu en el arbol de menu correspondiente.
             * Agrupa por MenuItemIdPadre y luego se lo asigna a su padre correspondiente. se repite en cascada para todos 
             * haciendo que el resultado final sea el arbol armado para cada menu principal
            */
            List<MenuItemModels> menuFinal = new List<MenuItemModels>();

            var grupos = listaMenu
                            .GroupBy(m => m.MenuItemIdPadre, (k, v) => new { padre = k, hijos = v.OrderBy(h => h.MenuItemId) })
                            .Where(g => g.padre != null)
                            .OrderByDescending(g => g.padre);

            var funcionesIds = this.FuncionesHabilitadas.Select(x => x.Id_Funcion);
            var funcionesIdsParent = this.FuncionesHabilitadas.Select(x => x.Id_Funcion_Padre);

            foreach (var grupo in grupos)
            {
                var menuItem = listaMenu.Single(m => m.MenuItemId == grupo.padre);

                var hijos = grupo.hijos.Where(x => funcionesIds.Contains(x.IdFuncion)).ToList();

                if (hijos.Any())
                {
                    menuItem.SubMenuList = new List<MenuItemModels>(hijos);
                }
                else
                {
                    listaMenu.Remove(menuItem);
                }
            }

            menuFinal.AddRange(listaMenu.FindAll(a => a.MenuItemIdPadre == null));
            MenuItemModels menuRaiz = new MenuItemModels { Nombre = "Raiz", SubMenuList = menuFinal };
            menuRaiz.PrintPretty("", true);

            return menuFinal;

        }

        public void Heartbeat()
        {
            var usuario = Session["usuarioPortal"] as UsuariosModel;
            if (usuario == null)
            {
                FormsAuthentication.SignOut();
                RedirectToAction("Account/ExpiredSession");
            }
            else
            {
                using (var resp = getHttpClient().PostAsync("api/SeguridadService/Ping", new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("idUsuario", usuario.Id_Usuario.ToString()),
                    new KeyValuePair<string, string>("token", usuario.Token)
                })).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        MvcApplication.GetLogger().LogError("Home/Heartbeat", new Exception($"Error al hacer ping al servicio. IdUsuario={usuario.Id_Usuario} y Token={usuario.Token}"));
                    }
                }
            }
        }
    }
}