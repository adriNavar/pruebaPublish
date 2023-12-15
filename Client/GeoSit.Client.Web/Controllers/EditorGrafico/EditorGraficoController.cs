using WebModels = GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.EditorGrafico.DTO;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos.DTO;
using GeoSit.Data.BusinessEntities.Via.DTO;
using VIA = GeoSit.Data.BusinessEntities.Via;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Client.Web.Controllers.EditorGrafico
{
    public class EditorGraficoController : Controller
    {
        WebModels.UsuariosModel Usuario { get { return Session["UsuarioPortal"] as WebModels.UsuariosModel; } }

        readonly HttpClient httpClient;
        const string API_URL = "api/EditorGrafico";
        const string EMPTY_ID = "-1";
        const short ID_MAPA_EDITOR_JURISDICCIONES = 6;
        const short ID_MAPA_EDITOR_SECCIONES = 7;
        const short ID_MAPA_EDITOR_MUNICIPIOS = 8;
        const short ID_MAPA_EDITOR_BARRIOS = 9;
        const short ID_MAPA_EDITOR_MANZANAS = 10;
        const short ID_MAPA_EDITOR_TRAMOS_VIAS = 11;

        public EditorGraficoController()
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) };
        }
        ~EditorGraficoController()
        {
            httpClient.Dispose();
        }

        #region Editor de Manzanas
        public ActionResult EditorManzanas()
        {
            var tiposDivision = GetTiposDivision().Select(t => new SelectListItem() { Value = $"{t.TipoDivisionId}", Text = t.Nombre });
            var localidades = GetLocalidades().Select(loc => new SelectListItem() { Value = $"{loc.FeatId}", Text = loc.Nombre });

            ViewData["TipoDivisionDefault"] = EMPTY_ID;
            ViewData["FiltroTiposDivision"] = tiposDivision
                                                    .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODAS -" })
                                                    .OrderBy(x => x.Text);

            ViewData["TiposDivision"] = tiposDivision
                                            .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                            .OrderBy(x => x.Text);

            ViewData["LocalidadDefault"] = EMPTY_ID;
            ViewData["FiltroLocalidades"] = localidades
                                                .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODAS -" })
                                                .OrderBy(x => x.Text);

            ViewData["Localidades"] = localidades
                                            .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                            .OrderBy(x => x.Text);

            ViewData["DataURL"] = "ListaManzanas";
            ViewData["IdMapa"] = ID_MAPA_EDITOR_MANZANAS;
            ViewData["EsPoligono"] = true;
            ViewData["EsLinea"] = false;
            ViewData["Layer"] = GetLayer(GetIdComponente("ID_COMPONENTE_MANZANA"));
            ViewData["LayerLocalidad"] = GetLayer(GetIdComponente("ID_COMPONENTE_LOCALIDAD"));
            return PartialView();
        }
        [HttpPost]
        public JsonResult ListaManzanas(DataTableParameters parametros)
        {
            return Json(GetManzanas(parametros));
        }
        [HttpGet]
        public JsonResult Manzanas(long id)
        {
            using (var resp = httpClient.GetAsync($"{API_URL}/Manzanas/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<Division>().Result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult Manzanas(Division division)
        {
            division._Id_Usuario = Usuario.Id_Usuario;
            using (var resp = httpClient.PostAsJsonAsync($"{API_URL}/Manzanas", division).Result)
            {
                resp.EnsureSuccessStatusCode();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult BorrarManzana(long id)
        {
            using (var resp = httpClient.DeleteAsync($"{API_URL}/Manzanas/{id}?usuario={Usuario.Id_Usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        private DataTableResult<GrillaManzana> GetManzanas(DataTableParameters parametros)
        {
            using (var resp = httpClient.PostAsync($"{API_URL}/Search/Manzanas", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaManzana>>().Result;
            }
        }
        private IEnumerable<TipoDivision> GetTiposDivision()
        {
            using (var resp = httpClient.GetAsync("api/TiposDivisionesAdministrativas/GetTiposDivision").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<TipoDivision>>().Result;
            }
        }
        #endregion

        #region Editor de Jurisdicciones
        public ActionResult EditorJurisdicciones()
        {
            var departamentos = GetDepartamentos().Select(loc => new SelectListItem() { Value = $"{loc.FeatId}", Text = loc.Nombre });

            ViewData["DepartamentosDefault"] = EMPTY_ID;
            ViewData["FiltroDepartamentos"] = departamentos
                                                .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODAS -" })
                                                .OrderBy(x => x.Text);

            ViewData["Departamentos"] = departamentos
                                            .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                            .OrderBy(x => x.Text);

            ViewData["DataURL"] = "ListaJurisdicciones";
            ViewData["IdMapa"] = ID_MAPA_EDITOR_JURISDICCIONES;
            ViewData["EsPoligono"] = true;
            ViewData["EsLinea"] = false;
            ViewData["Layer"] = GetLayer(GetIdComponente("ID_COMPONENTE_JURISDICCION"));
            ViewData["LayerDepartamento"] = GetLayer(GetIdComponente("ID_COMPONENTE_DEPARTAMENTO"));

            return PartialView();
        }
        [HttpPost]
        public JsonResult ListaJurisdicciones(DataTableParameters parametros)
        {
            return Json(GetJurisdicciones(parametros));
        }
        [HttpGet]
        public JsonResult Jurisdicciones(long id)
        {
            using (var resp = httpClient.GetAsync($"{API_URL}/Jurisdicciones/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<JurisdiccionDTO>().Result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult Jurisdicciones(JurisdiccionDTO objeto)
        {
            objeto.UsuarioId = Usuario.Id_Usuario;
            using (var resp = httpClient.PostAsJsonAsync($"{API_URL}/Jurisdicciones", objeto).Result)
            {
                resp.EnsureSuccessStatusCode();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult BorrarJurisdiccion(long id)
        {
            using (var resp = httpClient.DeleteAsync($"{API_URL}/Jurisdicciones/{id}?usuario={Usuario.Id_Usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        private DataTableResult<GrillaJurisdiccion> GetJurisdicciones(DataTableParameters parametros)
        {
            using (var resp = httpClient.PostAsync($"{API_URL}/Search/Jurisdicciones", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaJurisdiccion>>().Result;
            }
        }
        #endregion

        #region Editor de Secciones
        public ActionResult EditorSecciones()
        {
            var departamentos = GetDepartamentos().Select(loc => new SelectListItem() { Value = $"{loc.FeatId}", Text = loc.Nombre });

            ViewData["DepartamentoDefault"] = EMPTY_ID;
            ViewData["FiltroDepartamentos"] = departamentos
                                                .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODAS -" })
                                                .OrderBy(x => x.Text);

            ViewData["Departamentos"] = departamentos
                                            .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                            .OrderBy(x => x.Text);

            ViewData["DataURL"] = "ListaSecciones";
            ViewData["IdMapa"] = ID_MAPA_EDITOR_SECCIONES;
            ViewData["EsPoligono"] = true;
            ViewData["EsLinea"] = false;
            ViewData["Layer"] = GetLayer(GetIdComponente("ID_COMPONENTE_SECCION"));
            ViewData["LayerDepartamento"] = GetLayer(GetIdComponente("ID_COMPONENTE_DEPARTAMENTO"));

            return PartialView();
        }
        [HttpPost]
        public JsonResult ListaSecciones(DataTableParameters parametros)
        {
            return Json(GetSecciones(parametros));
        }
        [HttpGet]
        public JsonResult Secciones(long id)
        {
            using (var resp = httpClient.GetAsync($"{API_URL}/Secciones/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<SeccionDTO>().Result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult Secciones(SeccionDTO objeto)
        {
            objeto.UsuarioId = Usuario.Id_Usuario;
            using (var resp = httpClient.PostAsJsonAsync($"{API_URL}/Secciones", objeto).Result)
            {
                resp.EnsureSuccessStatusCode();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult BorrarSeccion(long id)
        {
            using (var resp = httpClient.DeleteAsync($"{API_URL}/Secciones/{id}?usuario={Usuario.Id_Usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        private DataTableResult<GrillaSeccion> GetSecciones(DataTableParameters parametros)
        {
            using (var resp = httpClient.PostAsync($"{API_URL}/Search/Secciones", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaSeccion>>().Result;
            }
        }
        #endregion

        #region Editor de Municipios
        public ActionResult EditorMunicipios()
        {
            var departamentos = GetDepartamentos().Select(loc => new SelectListItem() { Value = $"{loc.FeatId}", Text = loc.Nombre });

            ViewData["DepartamentosDefault"] = EMPTY_ID;
            ViewData["FiltroDepartamentos"] = departamentos
                                                .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODAS -" })
                                                .OrderBy(x => x.Text);

            ViewData["Departamentos"] = departamentos
                                            .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                            .OrderBy(x => x.Text);

            ViewData["DataURL"] = "ListaMunicipios";
            ViewData["IdMapa"] = ID_MAPA_EDITOR_MUNICIPIOS;
            ViewData["EsPoligono"] = true;
            ViewData["EsLinea"] = false;
            ViewData["Layer"] = GetLayer(GetIdComponente("ID_COMPONENTE_MUNICIPIO"));
            ViewData["LayerDepartamento"] = GetLayer(GetIdComponente("ID_COMPONENTE_DEPARTAMENTO"));

            return PartialView();
        }
        [HttpPost]
        public JsonResult ListaMunicipios(DataTableParameters parametros)
        {
            return Json(GetMunicipios(parametros));
        }
        [HttpGet]
        public JsonResult Municipios(long id)
        {
            using (var resp = httpClient.GetAsync($"{API_URL}/Municipios/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<MunicipioDTO>().Result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult Municipios(MunicipioDTO objeto)
        {
            objeto.UsuarioId = Usuario.Id_Usuario;
            using (var resp = httpClient.PostAsJsonAsync($"{API_URL}/Municipios", objeto).Result)
            {
                resp.EnsureSuccessStatusCode();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult BorrarMunicipio(long id)
        {
            using (var resp = httpClient.DeleteAsync($"{API_URL}/Municipios/{id}?usuario={Usuario.Id_Usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        private DataTableResult<GrillaMunicipio> GetMunicipios(DataTableParameters parametros)
        {
            using (var resp = httpClient.PostAsync($"{API_URL}/Search/Municipios", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaMunicipio>>().Result;
            }
        }
        #endregion

        #region Editor de Barrios
        public ActionResult EditorBarrios()
        {
            var localidades = GetLocalidades().Select(loc => new SelectListItem() { Value = $"{loc.FeatId}", Text = loc.Nombre });

            ViewData["LocalidadDefault"] = EMPTY_ID;
            ViewData["FiltroLocalidades"] = localidades
                                                    .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODAS -" })
                                                    .OrderBy(x => x.Text);

            ViewData["Localidades"] = localidades
                                            .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                            .OrderBy(x => x.Text);

            ViewData["DataURL"] = "ListaBarrios";
            ViewData["IdMapa"] = ID_MAPA_EDITOR_BARRIOS;
            ViewData["EsPoligono"] = true;
            ViewData["EsLinea"] = false;
            ViewData["Layer"] = GetLayer(GetIdComponente("ID_COMPONENTE_BARRIO"));
            ViewData["LayerLocalidad"] = GetLayer(GetIdComponente("ID_COMPONENTE_LOCALIDAD"));

            return PartialView();
        }
        [HttpPost]
        public JsonResult ListaBarrios(DataTableParameters parametros)
        {
            return Json(GetBarrios(parametros));
        }
        [HttpGet]
        public JsonResult Barrios(long id)
        {
            using (var resp = httpClient.GetAsync($"{API_URL}/Barrios/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<BarrioDTO>().Result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult Barrios(BarrioDTO objeto)
        {
            objeto.UsuarioId = Usuario.Id_Usuario;
            using (var resp = httpClient.PostAsJsonAsync($"{API_URL}/Barrios", objeto).Result)
            {
                resp.EnsureSuccessStatusCode();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult BorrarBarrio(long id)
        {
            using (var resp = httpClient.DeleteAsync($"{API_URL}/Barrios/{id}?usuario={Usuario.Id_Usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        private DataTableResult<GrillaBarrio> GetBarrios(DataTableParameters parametros)
        {
            using (var resp = httpClient.PostAsync($"{API_URL}/Search/Barrios", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaBarrio>>().Result;
            }
        }
        #endregion

        #region Editor de Tramos Vías
        public ActionResult EditorTramosVias()
        {
            var localidades = GetLocalidades().Select(loc => new SelectListItem() { Value = $"{loc.FeatId}", Text = loc.Nombre });
            var paridades = GetParidades().Select(e => new SelectListItem() { Value = e.Key, Text = e.Value });
            var tiposVia = GetTiposVias().Select(tpv => new SelectListItem() { Value = $"{tpv.TipoViaId}", Text = tpv.Nombre });

            ViewData["LocalidadDefault"] = EMPTY_ID;
            ViewData["FiltroLocalidades"] = localidades
                                                 .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODAS -" })
                                                 .OrderBy(x => x.Text);
            ViewData["Localidades"] = localidades
                                            .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                            .OrderBy(x => x.Text);

            ViewData["ParidadDefault"] = EMPTY_ID;
            ViewData["FiltroParidad"] = paridades
                                            .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- AMBAS -" })
                                            .OrderBy(x => x.Text);
            ViewData["Paridad"] = paridades
                                        .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                        .OrderBy(x => x.Text);

            ViewData["TipoViaDefault"] = EMPTY_ID;
            ViewData["FiltroTiposVias"] = tiposVia
                                            .Append(new SelectListItem() { Value = EMPTY_ID, Text = "- TODOS -" })
                                            .OrderBy(x => x.Text);
            ViewData["TiposVias"] = tiposVia
                                        .Append(new SelectListItem() { Value = string.Empty, Text = "- SELECCIONE -" })
                                        .OrderBy(x => x.Text);

            ViewData["DataURL"] = "ListaTramosVias";
            ViewData["IdMapa"] = ID_MAPA_EDITOR_TRAMOS_VIAS;
            ViewData["EsPoligono"] = false;
            ViewData["EsLinea"] = true;
            ViewData["Layer"] = GetLayer(GetIdComponente("ID_COMPONENTE_TRAMO_VIA"));
            ViewData["LayerLocalidad"] = GetLayer(GetIdComponente("ID_COMPONENTE_LOCALIDAD"));

            return PartialView();
        }
        [HttpPost]
        public JsonResult ListaTramosVias(DataTableParameters parametros)
        {
            return Json(GetTramosVias(parametros));
        }
        [HttpGet]
        public JsonResult TramosVias(long id)
        {
            using (var resp = httpClient.GetAsync($"{API_URL}/TramosVias/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<TramoViaDTO>().Result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult ViasByLocalidad(long id)
        {
            return Json(GetViasByLocalidad(id)
                            .Select(via => new { id = via.ViaId.ToString(), text = via.Nombre })
                            .ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public HttpStatusCodeResult TramosVias(TramoViaDTO objeto)
        {
            objeto.UsuarioId = Usuario.Id_Usuario;
            using (var resp = httpClient.PostAsJsonAsync($"{API_URL}/TramosVias", objeto).Result)
            {
                resp.EnsureSuccessStatusCode();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        [HttpPost]
        public HttpStatusCodeResult BorrarTramoVia(long id)
        {
            using (var resp = httpClient.DeleteAsync($"{API_URL}/TramosVias/{id}?usuario={Usuario.Id_Usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }
        private DataTableResult<GrillaTramoVia> GetTramosVias(DataTableParameters parametros)
        {
            using (var resp = httpClient.PostAsync($"{API_URL}/Search/TramosVias", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaTramoVia>>().Result;
            }
        }
        private IEnumerable<VIA.Via> GetViasByLocalidad(long localidad)
        {
            using (var resp = httpClient.GetAsync($"api/ViaService/GetViaByNombreYLocalidad/?nombre=&idLocalidad={localidad}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<VIA.Via>>().Result;
            }
        }
        private Dictionary<string, string> GetParidades()
        {
            using (var resp = httpClient.GetAsync("api/EditorGrafico/TramosVias/Paridades").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;
            }
        }
        private IEnumerable<VIA.TipoVia> GetTiposVias()
        {
            using (var resp = httpClient.GetAsync("api/ViaService/TiposVias").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<VIA.TipoVia>>().Result;
            }
        }
        #endregion

        #region Comunes
        private IEnumerable<Objeto> GetDepartamentos()
        {
            return GetObjetosByTipo(TipoObjetoAdministrativoEnum.Departamento);
        }
        private IEnumerable<Objeto> GetLocalidades()
        {
            return GetObjetosByTipo(TipoObjetoAdministrativoEnum.Localidad);
        }
        private IEnumerable<Objeto> GetObjetosByTipo(TipoObjetoAdministrativoEnum tipo)
        {
            using (var resp = httpClient.GetAsync($"api/ObjetoAdministrativoService/GetObjetoByTipo/{(long)tipo}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<Objeto>>().Result;
            }
        }
        private string GetLayer(long id)
        {
            using (var resp = httpClient.GetAsync($"api/Componente/Get/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<Componente>().Result.Capa;
            }
        }
        private long GetIdComponente(string key)
        {
            using (var resp = httpClient.GetAsync($"api/Parametro/GetParametroByClave?clave={key}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return long.Parse(resp.Content.ReadAsAsync<ParametrosGenerales>().Result.Valor);
            }
        }
        #endregion
    }
}