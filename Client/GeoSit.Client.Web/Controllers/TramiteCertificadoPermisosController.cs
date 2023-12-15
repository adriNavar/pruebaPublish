using GeoSit.Client.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Controllers
{
    public class tableData
    {
        public long id_funcion { get; set; }
        public string descripcion { get; set; }
        public bool estado { get; set; }
    }
    public class TramiteCertificadoPermisosController : Controller
    {

        private UsuariosModel Usuario { get { return Session["usuarioPortal"] as UsuariosModel; } }
        //
        // GET: /TramiteCertificadoPermisos/
        private readonly HttpClient cliente = new HttpClient();
        private List<tableData> tableData { get { return Session["tableData"] as List<tableData>; } set { Session["tableData"] = value; } }
        public TramiteCertificadoPermisosController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPermisosForm(long idSeccion, long idTipoTramite)
        {
            Session["Id_Seccion"] = idSeccion;
            Session["Id_Tipo_Tramite"] = idTipoTramite;
            tableData = new List<tableData>();

            HttpResponseMessage resp = cliente.GetAsync("api/SeguridadService/GetFunciones").Result;
            resp.EnsureSuccessStatusCode();
            var results = resp.Content.ReadAsAsync<IEnumerable<FuncionesModel>>().Result;

            HttpResponseMessage respParametro = cliente.GetAsync("api/SeguridadService/GetParametrosGenerales").Result;
            respParametro.EnsureSuccessStatusCode();
            var resultParametro = respParametro.Content.ReadAsAsync<IEnumerable<ParametrosGeneralesModel>>().Result;

            //Listado de Permisos
            var parametro = resultParametro.Where(x => x.Clave.Contains("TRAMITE_FUNCION_PADRE")).FirstOrDefault();

            results = results.Where(x => x.Id_Funcion_Padre == Convert.ToInt32(parametro.Valor));
            //----


            List<tableData> TableResults = new List<tableData>();
            //Decode Tramite Seccion
            if (idSeccion >= 0)
            {
                //BUSCAR DATOS GUARDADOS PARA PODER EDITARLOS
                HttpResponseMessage resppermiso = cliente.GetAsync("api/TramitesCertificadosService/GetPermisosTramiteSaved?idSeccion=" + idSeccion + "&idTipoTramite=" + idTipoTramite).Result;
                resppermiso.EnsureSuccessStatusCode();

                var Permisos = JsonConvert.DeserializeObject<List<TramitePermisos>>(resppermiso.Content.ReadAsAsync<string>().Result);
                if (Permisos != null)
                {
                    if (Permisos.Count() != 0)
                    {
                        foreach (var row in results)
                        {
                            var data = Permisos.Where(x => x.ID_FUNCION == row.Id_Funcion).Count();
                            if (data > 0)
                            {
                                TableResults.Add(new tableData() { id_funcion = row.Id_Funcion, descripcion = row.Nombre, estado = true });
                            }
                            else
                            {
                                TableResults.Add(new tableData() { id_funcion = row.Id_Funcion, descripcion = row.Nombre, estado = false });
                            }
                        }
                    }
                    else
                    {
                        foreach (var row in results)
                        {
                            TableResults.Add(new tableData() { id_funcion = row.Id_Funcion, descripcion = row.Nombre, estado = false });
                        }
                    }
                }
                else
                {
                    foreach (var row in results)
                    {
                        TableResults.Add(new tableData() { id_funcion = row.Id_Funcion, descripcion = row.Nombre, estado = false });
                    }
                }

            }
            else
            {
                foreach (var row in results)
                {
                    TableResults.Add(new tableData() { id_funcion = row.Id_Funcion, descripcion = row.Nombre, estado = false });
                }
            }

            tableData = TableResults;

            return PartialView("~/Views/TramiteCertificadoPermisos/Index.cshtml");
        }

        public JsonResult GetResults()
        {
            return Json(JsonConvert.SerializeObject(tableData), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save()
        {
            long idSeccion = Convert.ToInt32(Session["Id_Seccion"]);
            long idTipoTramite = Convert.ToInt32(Session["Id_Tipo_Tramite"]);
            var jsonPost = JsonConvert.SerializeObject(tableData);
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/TramitesCertificadosService/SaveSeccionPermiso?jsonPost=" + jsonPost + "&idSeccion=" + idSeccion + "&usuarioId=" + Usuario.Id_Usuario + "&idTipoTramite=" + idTipoTramite).Result;
                resp.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return Json("ERROR");
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        public JsonResult Change(bool estado, long idFuncion)
        {
            foreach (var item in tableData.Where(x => x.id_funcion == idFuncion))
            {
                item.estado = estado;
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

    }
}