using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Mantenimiento;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Data;
using GeoSit.Client.Web.Helpers;
using Resources;
using System.Web.Script.Serialization;

namespace GeoSit.Client.Web.Controllers
{
    public class MantenimientoController : Controller
    {
        private HttpClient cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });

        public MantenimientoController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TablasAuxiliares(MantenimientoModel model)
        {
            //var model = new MantenimientoModel();
            List<ComponenteTA> Lista = GetListaComponenteTA();
            model.ListaComponente = Lista;
            return PartialView("TablasAuxiliares", model);
        }

        public ActionResult TablasAuxiliaresData(long id)
        {

            List<ComponenteTA> ListaComponentes = GetListaComponenteTA();
            ComponenteTA componente = ListaComponentes.Where(c => c.Id_Compoente == id).FirstOrDefault();

            List<AtributoTA> Lista = GetListaAtributoTAById(id);
            DataTable data = GetContenidoTabla(id);
            ViewBag.Data = data;
            ViewBag.Componente = componente;
            return PartialView("TablasAuxiliaresData", Lista);
        }

        public ActionResult EditarRow(long idComponente, long? id)
        {
            List<AtributoTA> Lista = GetListaAtributoTAById(idComponente);
            if (id != null)
            {
                Dictionary<string, string> datos = GetContenidoTablaAsignacion(idComponente, (long)id);
                foreach (AtributoTA elemento in Lista)
                {
                    KeyValuePair<String, String> dato = datos.Where(x => x.Key.ToLower().Equals(elemento.Campo.ToLower())).FirstOrDefault();
                    if (dato.Key != null)
                    {
                        elemento.Valor = dato.Value;
                    }
                }

            }
            ViewBag.IdComponente = idComponente;
            ViewBag.Id = id;
            return PartialView("EditarRow", Lista);
        }

        public JsonResult GetAtributosByComponente(long IdComponente)
        {
            List<AtributoTA> listaAtributos = GetListaAtributoTAById(IdComponente);
            return Json(listaAtributos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAtributoValores(long IdComponente, long IdTabla)
        {
            Dictionary<string, string> listaValores = GetContenidoTablaAsignacion(IdComponente, IdTabla);
            return Json(listaValores, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAtributoRelacionado(string tabla_relacion, string esquema_relacion, string campo_relacion, string descripcion_relacion)
        {
            List<AtributoRelacionado> listaAtributoRelacionado = GetListaAtributoRelacionado(tabla_relacion, esquema_relacion, campo_relacion, descripcion_relacion);
            return Json(listaAtributoRelacionado, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GuardarDatos(string IdComponente, string IdTabla, Dictionary<string, string> Parametros)
        {

            TablasAuxiliares model = new TablasAuxiliares();
            model.ComponentesId = IdComponente;
            model.TablaID = IdTabla != "" ? IdTabla : null;
            model.CamposTablas = new List<string>();
            model.ValoresTablas = new List<string>();
            foreach (var parametro in Parametros)
            {
                if (parametro.Value != null)
                {
                    model.CamposTablas.Add(parametro.Key);
                    model.ValoresTablas.Add(parametro.Value);
                }
            }


            TablasAuxiliares modelOriginal = null;
            if (model.TablaID != null)
            {
                modelOriginal = new TablasAuxiliares();
                modelOriginal.TablaID = model.TablaID;
                modelOriginal.ComponentesId = model.ComponentesId;
                modelOriginal.CamposTablas = new List<string>();
                modelOriginal.ValoresTablas = new List<string>();
                var ParametrosOriginales = GetContenidoTablaAsignacion(long.Parse(IdComponente), long.Parse(IdTabla));
                foreach (var parametro in ParametrosOriginales)
                {
                    if (parametro.Value != null)
                    {
                        modelOriginal.CamposTablas.Add(parametro.Key);
                        modelOriginal.ValoresTablas.Add(parametro.Value);
                    }
                }

            }

            var usuario = ((UsuariosModel)Session["usuarioPortal"]) ?? new UsuariosModel();
            model.Id_Usuario = usuario.Id_Usuario;

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/MantenimientoService/SetAgregarRegistro/", model).Result;
            resp.EnsureSuccessStatusCode();
            var success = resp.Content.ReadAsAsync<bool>().Result;

            var serializer = new JavaScriptSerializer();


            List<ComponenteTA> ListaComponentes = GetListaComponenteTA();
            var componente = ListaComponentes.FirstOrDefault(l => l.Id_Compoente.ToString() == IdComponente);

            if (model.TablaID != null)
            {
                AuditoriaHelper.Register(usuario.Id_Usuario, "Se modificó un registro", Request,
                    TipoOperacion.Modificacion, Autorizado.Si, Eventos.ModificarRegistro, componente != null ? componente.Descripcion : "", serializer.Serialize(modelOriginal), serializer.Serialize(model));
            }
            else
            {
                AuditoriaHelper.Register(usuario.Id_Usuario, "Se agregó un registro", Request,
                    TipoOperacion.Alta, Autorizado.Si, Eventos.AgregarRegistro, componente != null ? componente.Descripcion : "", serializer.Serialize(model));
            }

            return Json(new { success = success });

        }

        public List<ComponenteTA> GetListaComponenteTA()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MantenimientoService/GetListaComponenteTA").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteTA>)resp.Content.ReadAsAsync<List<ComponenteTA>>().Result;

        }

        public List<AtributoTA> GetListaAtributoTA()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MantenimientoService/GetListaAtributoTA").Result;
            resp.EnsureSuccessStatusCode();
            return (List<AtributoTA>)resp.Content.ReadAsAsync<List<AtributoTA>>().Result;

        }
        public List<AtributoTA> GetListaAtributoTAById(long IdComponente)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MantenimientoService/GetListaAtributoTAById/" + IdComponente).Result;
            resp.EnsureSuccessStatusCode();
            return (List<AtributoTA>)resp.Content.ReadAsAsync<List<AtributoTA>>().Result;

        }

        public DataTable GetContenidoTabla(long IdComponente)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MantenimientoService/GetContenidoTabla/" + IdComponente).Result;
            resp.EnsureSuccessStatusCode();
            return (DataTable)resp.Content.ReadAsAsync<DataTable>().Result;

        }

        public Dictionary<string, string> GetContenidoTablaAsignacion(long IdComponente, long IdTabla)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MantenimientoService/GetContenidoTablaAsignacion/?Id=" + IdComponente + "&IdTabla=" + IdTabla).Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, string>)resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;

        }

        [HttpPost]
        public JsonResult GetEliminaRegistroAsignacion(string IdComponente, string IdTabla)
        {
            TablasAuxiliares model = new TablasAuxiliares();
            model.ComponentesId = IdComponente;
            model.TablaID = IdTabla != "" ? IdTabla : null;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]) ?? new UsuariosModel();
            model.Id_Usuario = usuario.Id_Usuario;

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/MantenimientoService/GetEliminaRegistroAsignacion/", model).Result;
            resp.EnsureSuccessStatusCode();
            var success = resp.Content.ReadAsAsync<bool>().Result;

            List<ComponenteTA> ListaComponentes = GetListaComponenteTA();
            var componente = ListaComponentes.FirstOrDefault(l => l.Id_Compoente.ToString() == IdComponente);

            AuditoriaHelper.Register(usuario.Id_Usuario, "Se eliminó un registro", Request,
                TipoOperacion.Baja, Autorizado.Si, Eventos.EliminarRegistro, componente != null ? componente.Descripcion : "");

            return Json(new { success = success });
        }

        //[ValidateInput(false)]
        //public ActionResult SetAgregarRegistro(MantenimientoModel model)
        //{
        //    //Lstring Respuesta_guardar = "";
        //    HttpResponseMessage resp;
        //    string TipoABM = "";
        //    if (model.TablasAuxiliares.TablaID == null)
        //    {
        //        if (model.TablasAuxiliares.CamposTablasAgregar != null)
        //        {
        //            for (int i = 0; i < model.TablasAuxiliares.CamposTablasAgregar.Count(); i++)
        //            {
        //                TablasAuxiliares ta = new TablasAuxiliares();
        //                ta.ComponentesId = model.TablasAuxiliares.ComponentesId;
        //                ta.CamposTablas = model.TablasAuxiliares.CamposTablasAgregar[i];
        //                ta.ValoresTablas = model.TablasAuxiliares.ValoresTablasAgregar[i];
        //                resp = cliente.PostAsJsonAsync("api/MantenimientoService/SetAgregarRegistro/", ta).Result;
        //                resp.EnsureSuccessStatusCode();
        //                TipoABM = resp.Content.ReadAsAsync<string>().Result;
        //            }
        //        }
        //        else
        //        {
        //            resp = cliente.PostAsJsonAsync("api/MantenimientoService/SetAgregarRegistro/", model.TablasAuxiliares).Result;
        //            resp.EnsureSuccessStatusCode();
        //            TipoABM = resp.Content.ReadAsAsync<string>().Result;
        //        }
        //    }
        //    else
        //    {
        //        resp = cliente.PostAsJsonAsync("api/MantenimientoService/SetAgregarRegistro/", model.TablasAuxiliares).Result;
        //        resp.EnsureSuccessStatusCode();
        //        TipoABM = resp.Content.ReadAsAsync<string>().Result;
        //    }
        //    //HttpResponseMessage resp = cliente.GetAsync("api/MantenimientoService/SetAgregarRegistro/", model.TablassAuxiliares).Result;


        //    if (TipoABM == "A")
        //    {
        //        model.Mensaje = "AltaOK";
        //    }
        //    else if (TipoABM == "M")
        //    {
        //        model.Mensaje = "ModificacionOK";
        //    }
        //    else
        //    {
        //        model.Mensaje = "Error: " + TipoABM;
        //    }

        //    //return View("Perfiles", model);
        //    return RedirectToAction("TablasAuxiliares", new { Mensaje = model.Mensaje });
        //    //return View("TablasAuxiliares", model); 
        //}

        public string GetGrillaAtributos(long IdComponente)
        {
            //var listadoAtributos = GetListaAtributoTAById(IdComponente);
            List<AtributoTA> listaAtributos = GetListaAtributoTAById(IdComponente);

            List<string> campos_xml = new List<string>();

            var campo_clave = "";

            for (int i = 0; i < listaAtributos.Count; i++)
            {
                if (listaAtributos[i].Es_Clave == 1)
                {
                    campo_clave = listaAtributos[i].Campo;

                }
                if (listaAtributos[i].Id_Tipo_Dato == 9 || listaAtributos[i].Id_Tipo_Dato == 10)
                {
                    campos_xml.Add(listaAtributos[i].Campo);
                }

            }

            var dt = GetContenidoTabla(IdComponente);
            //html = "<table>";
            //add header row
            string html = "";
            //for (int i = 0; i < dt.Columns.Count; i++)
            //    html += "<td>" + dt.Columns[i].ColumnName + "</td>";
            //html += "</tr>";
            //add rows

            var columna_clave = "";
            var columnas_no_clave = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                columna_clave = "";
                columnas_no_clave = "";

                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    if (dt.Columns[j].ToString() == campo_clave)
                    {
                        columna_clave = "<td>";
                        columna_clave += "<input type=\"radio\" aria-label=\"...\" id=\"IdResultado\" name=\"model.TablasAuxiliares.TablaID\" class=\"id_resultado\" value=\"" + dt.Rows[i][j].ToString() + "\">";
                        columna_clave += "</td>";
                        columnas_no_clave += "<td>";
                        columnas_no_clave += dt.Rows[i][j].ToString();
                        columnas_no_clave += "</td>";
                    }
                    else
                    {
                        columnas_no_clave += "<td>";

                        if (campos_xml.Count > 0)
                        {
                            for (int h = 0; h < campos_xml.Count; h++)
                            {
                                if (campos_xml[h].ToString() == dt.Columns[j].ToString())
                                {
                                    if (dt.Rows[i][j].ToString() != "")
                                    {
                                        columnas_no_clave += "Datos XML";
                                    }
                                }
                                else
                                {
                                    columnas_no_clave += dt.Rows[i][j].ToString();
                                }
                            }
                        }
                        else
                        {
                            columnas_no_clave += dt.Rows[i][j].ToString();
                            columnas_no_clave += "</td>";
                        }
                    }
                }

                html += columna_clave;
                html += columnas_no_clave;
                html += "</tr>";

                //html += "<tr>";
                //for (int j = 0; j < dt.Columns.Count; j++)
                //{



                //    if (j == 0)
                //    {
                //        html += "<td>";
                //        html += "<input type=\"radio\" aria-label=\"...\" id=\"IdResultado\" name=\"model.TablasAuxiliares.TablaID\" class=\"id_resultado\" value=\"" + dt.Rows[i][j].ToString() + "\">";
                //        html += "</td>";
                //    }
                //    html += "<td>";
                //    html += dt.Rows[i][j].ToString() + "</td>";
                //}
                //html += "</tr>";
            }

            //html += "<tr>";
            //html += columna_clave;
            //html += columnas_no_clave;
            //html += "</tr>";

            return html;

        }

        public List<AtributoRelacionado> GetListaAtributoRelacionado(string tabla_relacion, string esquema_relacion, string campo_relacion, string descripcion_relacion)
        {

            HttpResponseMessage resp = cliente.GetAsync("api/MantenimientoService/GetListaAtributoRelacionado/?tabla_relacion=" + tabla_relacion + "&esquema_relacion=" + esquema_relacion + "&campo_relacion=" + campo_relacion + "&descripcion_relacion=" + descripcion_relacion).Result;
            resp.EnsureSuccessStatusCode();
            return (List<AtributoRelacionado>)resp.Content.ReadAsAsync<List<AtributoRelacionado>>().Result;


        }
    }
}