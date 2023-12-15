using System;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using MT = GeoSit.Data.BusinessEntities.MapasTematicos;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Client.Web.Helpers.ExtensionMethods;
using GeoSit.Client.Web.Helpers;
using Resources;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Web.Mvc.Filters;
using System.Windows.Media.Converters;

namespace GeoSit.Client.Web.Controllers
{
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class MapasTematicosController : Controller
    {
        private UsuariosModel UsuarioConectado { get { return (UsuariosModel)Session["usuarioPortal"]; } }

        private HttpClient cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });

        private readonly string UploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public MapasTematicosController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            cliente.Timeout = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["httpClientIncreasedTimeout"]));
        }

        // GET: /MapasTematicos/Index
        public ActionResult Index()
        {
            MapaTematicoModel model = new MapaTematicoModel();
            Session["MapaTematico"] = model;

            ViewBag.listaComponentes = GetComponentesGeograficos();
            return PartialView("Componentes", model);
        }
        public ActionResult GetIndexView()
        {
            var model = (MapaTematicoModel)(Session["MapaTematico"] = (MapaTematicoModel)Session["MapaTematico"] ?? new MapaTematicoModel());
            ViewBag.listaComponentes = GetListaPrimeroSeleccionado(model);
            return PartialView("Componentes", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAtributosView(MapaTematicoModel model)
        {
            if (model == null)
            {
                return RedirectToAction("Index", "MapasTematicos");
            }

            var mapatematico = (MapaTematicoModel)Session["MapaTematico"];

            if (mapatematico.Componente.ComponenteId != model.Componente.ComponenteId)
            {
                model.ComponenteAtributo = new ComponenteModel();
                model.Filtros = new List<FiltroModel>();
                model.Componente = GetComponentesById(model.Componente.ComponenteId);
            }
            if (mapatematico.Componente.ComponenteId != model.Componente.ComponenteId)
            {
                mapatematico.Componente.ComponenteId = model.Componente.ComponenteId;
                if (model.ComponenteAtributo.ComponenteId > 0)
                {
                    mapatematico.ComponenteAtributo = GetComponentesById(model.ComponenteAtributo.ComponenteId);
                }
                else
                {
                    mapatematico.ComponenteAtributo = mapatematico.Componente;
                }
            }
            if (mapatematico.ComponenteAtributo.Atributo.AtributoId != model.ComponenteAtributo.Atributo.AtributoId)
            {
                mapatematico.ComponenteAtributo.Atributo.AtributoId = model.ComponenteAtributo.Atributo.AtributoId;
                if (model.ComponenteAtributo.Atributo.AtributoId.HasValue)
                {
                    long atributo = model.ComponenteAtributo.Atributo.AtributoId.Value;
                    if (model.ComponenteAtributo.Atributo.EsImportado)
                    {
                        var dec = GetDatoExternoConfiguracionById(atributo);
                        mapatematico.ComponenteAtributo.Atributo = new AtributoModel()
                        {
                            EsImportado = true,
                            Nombre = dec.Nombre,
                            AtributoId = dec.DatoExternoConfiguracionId
                        };
                    }
                    else
                    {
                        mapatematico.ComponenteAtributo.Atributo = GetAtributosById(atributo);
                    }
                }
            }
            if (mapatematico.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId != model.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId)
            {
                mapatematico.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId = model.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId;
                if (model.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId > 0)
                {
                    mapatematico.ComponenteAtributo.Atributo.Agrupacion = GetAgrupacionesById(model.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId);
                }
            }
            ViewBag.Componentes = new SelectList(GetComponentesByPadre(model.Componente.ComponenteId), "ComponenteId", "Nombre");

            Session["MapaTematico"] = mapatematico;
            return PartialView("Atributos", mapatematico);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAtributosViewVolver()
        {
            return GetAtributosView((MapaTematicoModel)Session["MapaTematico"]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetFiltrosView(MapaTematicoModel model)
        {
            var mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);
            if (mapatematico != null && model != mapatematico)
            {
                if (model.ComponenteAtributo.ComponenteId > 0)
                {
                    mapatematico.ComponenteAtributo = GetComponentesById(model.ComponenteAtributo.ComponenteId);
                }
                if (model.ComponenteAtributo.Atributo.AtributoId != null && !model.ComponenteAtributo.Atributo.EsImportado)
                {
                    long atributo = model.ComponenteAtributo.Atributo.AtributoId ?? 0;
                    mapatematico.ComponenteAtributo.Atributo = GetAtributosById(atributo);
                }
                else if (model.ComponenteAtributo.Atributo.AtributoId != null && model.ComponenteAtributo.Atributo.EsImportado)
                {
                    mapatematico.ComponenteAtributo.Atributo = model.ComponenteAtributo.Atributo;
                }
                if (model.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId > 0)
                {
                    mapatematico.ComponenteAtributo.Atributo.Agrupacion = GetAgrupacionesById(model.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId);
                }
                if (model.Filtros.Count > 0 && model.Filtros.Count < 2)
                {
                    mapatematico.Filtros.AddRange(model.Filtros);
                }
            }
            else
            {
                mapatematico = model;
            }
            foreach (var item in mapatematico.Filtros)
            {
                if (item.FiltroAtributo != null)
                {
                    item.FiltroAtributoDesc = GetAtributosById(item.FiltroAtributo ?? 0).Nombre;
                }
                if (item.FiltroComponente != null)
                {
                    item.FiltroComponenteDesc = GetComponentesById(item.FiltroComponente ?? 0).Nombre;
                }
                if (item.FiltroOperacion != null)
                {
                    if (item.FiltroOperacion > 0)
                    {
                        item.FiltroOperacionDesc = GetOperacionesById(item.FiltroOperacion ?? 0).Nombre;
                    }
                }
                if (item.FiltroColeccion != null)
                {
                    item.FiltroColeccionDesc = GetColeccionesById(item.FiltroColeccion ?? 0).Nombre;
                }
            }
            ViewBag.Componentes = new SelectList(GetComponentesRelacionados(mapatematico.Componente.ComponenteId), "ComponenteId", "Nombre");
            ViewBag.Colecciones = GetColeccionesByComponente(mapatematico.Componente.ComponenteId);
            ViewBag.ComponentesGraficos = GetComponentes();

            Session["MapaTematico"] = mapatematico;
            return PartialView("Filtros", mapatematico);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetFiltrosViewVolver()
        {
            return GetFiltrosView((MapaTematicoModel)Session["MapaTematico"]);
        }

        public ActionResult GetVisualizacionViewFromUbicaciones()
        {
            Session["FromUbicaciones"] = true;
            ViewBag.FromUbicaciones = true;
            MapaTematicoModel mapaTematico = (MapaTematicoModel)Session["MapaTematico"];
            ViewBag.IdConfigCategoria = 2;
            ViewBag.NombreColeccion = "Ubicaciones";
            ViewBag.Plantillas = new SelectList(GetPlantillasForMapaTematico(), "IdPlantilla", "Nombre");
            ViewBag.ImagenesSatelitales = GetAllImagenSatelital();
            Session["MT_Volver_Predefinido"] = 0;
            Session["GUID"] = System.Guid.NewGuid().ToString();
            MT.ObjetoResultadoDetalle objetoResultadoDetalle = GetObjetoResultadoDetalle(mapaTematico.Visualizacion);
            Session["Geometry_Type"] = 1;
            objetoResultadoDetalle = GetObjetoDetallesConRangos(objetoResultadoDetalle);
            ActualizarResultadoMapaTematico(objetoResultadoDetalle);
            mapaTematico.Visualizacion = GetVisualizacionModel(objetoResultadoDetalle, mapaTematico.Visualizacion);
            Session["MapaTematico"] = mapaTematico;
            return PartialView("Visualizacion", mapaTematico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetVisualizacionView(MapaTematicoModel model)
        {
            var mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);

            var generarResultado = (int)Session["GenerarResultado"];
            if (generarResultado == 1)
            {
                if (mapatematico != null)
                {
                    if (mapatematico.Componente.ComponenteId > 0)
                    {
                        //mapatematico.Componente = GetComponentesById(mapatematico.Componente.ComponenteId);
                        Session["CompenenteId"] = mapatematico.Componente.ComponenteId;
                    }
                    mapatematico.cantFiltrosAtributo = model.cantFiltrosAtributo;
                    mapatematico.cantFiltrosColeccion = model.cantFiltrosColeccion;
                    mapatematico.cantFiltrosGeografico = model.cantFiltrosGeografico;
                    mapatematico.Filtros = model.Filtros;
                }
                else
                {
                    mapatematico = model;
                }

                //Determinar si se llama a GenerarResultado
                var mapaTematicoGenerado = (MapaTematicoModel)Session["MapaTematicoGenerado"];
                if (CambioMapaTematico(mapaTematicoGenerado, mapatematico))
                {
                    string guid = Guid.NewGuid().ToString();
                    Session["GUID"] = guid;

                    if (mapatematico.Filtros != null)
                    {
                        foreach (var filtroModel in mapatematico.Filtros)
                        {
                            if (filtroModel.FiltroTipo == 1 && filtroModel.FiltroOperacion.GetValueOrDefault() == 0)
                            {
                                //Vacio
                                //filtroModel.FiltroOperacion = 1;
                                filtroModel.FiltroOperacion = 16; //DENTRO DE
                            }
                            else if (filtroModel.FiltroTipo == 2 && !string.IsNullOrEmpty(filtroModel.Coordenadas) && filtroModel.Coordenadas.Substring(0, 1) == "G")
                            {
                                string filtroGrafico = filtroModel.Coordenadas.Substring(1);
                                var aObjetoSeleccionado = filtroGrafico.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                                if (aObjetoSeleccionado.Any())
                                {
                                    var lstObjetoSeleccionadoDatos = aObjetoSeleccionado[0].Split(';').ToList();
                                    string layer = lstObjetoSeleccionadoDatos[0];
                                    string key = lstObjetoSeleccionadoDatos[1];
                                    ComponenteModel componenteGraf = null;
                                    try
                                    {
                                        componenteGraf = GetComponenteByCapa(layer);
                                    }
                                    catch (Exception)
                                    {
                                        return Json(new { error = true, msg = string.Format("Error en el filtro {0}", layer) }, JsonRequestBehavior.AllowGet);
                                    }
                                    filtroModel.FiltroComponente = componenteGraf.ComponenteId;
                                    filtroModel.FiltroAtributo = GetAtributoFEATIDByComponente(componenteGraf.ComponenteId).AtributoId;
                                    filtroModel.FiltroOperacion = 1; //operador "=" (igual)
                                    filtroModel.FiltroOperacionDesc = GetOperacionesById(filtroModel.FiltroOperacion.Value).Nombre;
                                    filtroModel.Valor1 = key;
                                }
                                filtroModel.Coordenadas = null;
                            }
                        }
                    }
                    MT.ObjetoResultadoDetalle objetoResultadoDetalle = null;
                    try
                    {
                        objetoResultadoDetalle = GenerarResultadoMapaTematico(guid, mapatematico, 0);
                    }
                    catch (HttpRequestException ex)
                    {
                        MvcApplication.GetLogger().LogError("MapasTemasticosController/GetVisualizacionView(httpEx)", ex);
                        return new HttpStatusCodeResult(System.Net.HttpStatusCode.ExpectationFailed);
                    }
                    catch (TimeoutException ex)
                    {
                        MvcApplication.GetLogger().LogError("MapasTemasticosController/GetVisualizacionView(timeout)", ex);
                        return new HttpStatusCodeResult(System.Net.HttpStatusCode.RequestTimeout);
                    }
                    if (objetoResultadoDetalle != null)
                    {
                        long idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;
                        AuditoriaHelper.Register(idUsuario, string.Empty, Request, TipoOperacion.Consulta, Autorizado.Si, Eventos.GeneracionResultadosEnMT);
                        Session["Geometry_Type"] = objetoResultadoDetalle.GeometryType;
                        mapatematico.Visualizacion = GetVisualizacionModel(objetoResultadoDetalle, mapatematico.Visualizacion);
                    }
                    else
                    {
                        mapatematico.Visualizacion = new VisualizacionModel();
                    }
                    Session["MapaTematicoGenerado"] = GetMapaTematicoCopy(mapatematico);
                }
                else
                {
                    mapatematico.Visualizacion = GetVisualizacionModel(GetObjetoResultadoDetalle(mapatematico.Visualizacion), mapatematico.Visualizacion);
                }
            }
            else
            {
                mapatematico.Visualizacion = GetVisualizacionModel(GetObjetoResultadoDetalle(mapatematico.Visualizacion), mapatematico.Visualizacion);
            }

            int.TryParse(GetParametrosGenerales().FirstOrDefault(p => p.Descripcion == "Rangos_Maximos")?.Valor, out int rangosMaximos);
            ViewBag.RangosMaximos = rangosMaximos;

            Session["GenerarResultado"] = 1;
            Session["MapaTematico"] = mapatematico;
            ViewBag.Plantillas = new SelectList(GetPlantillasForMapaTematico(), "IdPlantilla", "Nombre");
            ViewBag.ImagenesSatelitales = GetAllImagenSatelital();
            ViewBag.IdConfigCategoria = 1;

            ViewBag.Distribuciones = new SelectList(new[]
                                     {
                                        new SelectListItem(){ Value= "1",Text="Uniforme"},
                                        new SelectListItem() { Value = "2", Text = "Cuantiles" },
                                        new SelectListItem() { Value = "3", Text = "Valores Individuales"}
                                     }, "Value", "Text", mapatematico.Visualizacion.Distribucion.ToString());

            ViewBag.AnchosContorno = new SelectList(Enumerable.Range(0, 6)
                                                              .Select(val => new SelectListItem()
                                                              {
                                                                  Value = val.ToString(),
                                                                  Text = val.ToString()
                                                              }), "Value", "Text", mapatematico.Visualizacion.CantidadContorno);

            ViewBag.VerIdentificante = 1;

            Session["MT_Predefinido"] = 0;
            ViewBag.PredefinidoId = 0;
            Session["PredefinidoId"] = 0;
            ViewBag.IdPlantilla = 0;
            Session["IdPlantilla"] = 0;
            Session["IdDistrito"] = "";

            return PartialView("Visualizacion", mapatematico);
        }

        private AtributoModel GetAtributoFEATIDByComponente(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetAtributoFEATIDByComponente/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (AtributoModel)resp.Content.ReadAsAsync<AtributoModel>().Result;
        }

        private MapaTematicoModel GetMapaTematicoCopy(MapaTematicoModel mapatematico)
        {
            MapaTematicoModel mapaTematicoCopy = new MapaTematicoModel();
            mapaTematicoCopy.cantFiltrosAtributo = mapatematico.cantFiltrosAtributo;
            mapaTematicoCopy.cantFiltrosColeccion = mapatematico.cantFiltrosColeccion;
            mapaTematicoCopy.cantFiltrosGeografico = mapatematico.cantFiltrosGeografico;
            mapaTematicoCopy.Descripcion = mapatematico.Descripcion;
            mapaTematicoCopy.Externo = mapatematico.Externo;
            mapaTematicoCopy.FechaBaja = mapatematico.FechaBaja;
            mapaTematicoCopy.FechaCreacion = mapatematico.FechaCreacion;
            mapaTematicoCopy.GrabaBiblioteca = mapatematico.GrabaBiblioteca;
            mapaTematicoCopy.GrabaColeccion = mapatematico.GrabaColeccion;
            mapaTematicoCopy.MapaTematicoId = mapatematico.MapaTematicoId;
            mapaTematicoCopy.mensaje = mapatematico.mensaje;
            mapaTematicoCopy.Nombre = mapatematico.Nombre;
            mapaTematicoCopy.Visibilidad = mapatematico.Visibilidad;

            ComponenteModel componente = new ComponenteModel();
            componente.ComponenteId = mapatematico.Componente.ComponenteId;
            componente.Descripcion = mapatematico.Componente.Descripcion;
            componente.EntornoId = mapatematico.Componente.EntornoId;
            componente.Esquema = mapatematico.Componente.Esquema;
            componente.Graficos = mapatematico.Componente.Graficos;
            componente.Nombre = mapatematico.Componente.Nombre;
            componente.Tabla = mapatematico.Componente.Tabla;
            AtributoModel atributo = new AtributoModel();
            atributo.AtributoId = mapatematico.Componente.Atributo.AtributoId;
            //...falta copiar las otras propiedades pero para la comparacion que se utiliza no es necesario
            componente.Atributo = atributo;
            mapaTematicoCopy.Componente = componente;

            ComponenteModel componenteAtributo = new ComponenteModel();
            componenteAtributo.ComponenteId = mapatematico.ComponenteAtributo.ComponenteId;
            componenteAtributo.Descripcion = mapatematico.ComponenteAtributo.Descripcion;
            componenteAtributo.EntornoId = mapatematico.ComponenteAtributo.EntornoId;
            componenteAtributo.Esquema = mapatematico.ComponenteAtributo.Esquema;
            componenteAtributo.Graficos = mapatematico.ComponenteAtributo.Graficos;
            componenteAtributo.Nombre = mapatematico.ComponenteAtributo.Nombre;
            componenteAtributo.Tabla = mapatematico.ComponenteAtributo.Tabla;

            AtributoModel atributo2 = new AtributoModel();
            atributo2.AtributoId = mapatematico.ComponenteAtributo.Atributo.AtributoId;

            AgrupacionModel agrupacion = new AgrupacionModel();
            agrupacion.AgrupacionId = mapatematico.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId;
            agrupacion.Nombre = mapatematico.ComponenteAtributo.Atributo.Agrupacion.Nombre;
            atributo2.Agrupacion = agrupacion;
            //...falta copiar las otras propiedades pero para la comparacion que se utiliza no es necesario
            componenteAtributo.Atributo = atributo2;
            mapaTematicoCopy.ComponenteAtributo = componenteAtributo;

            List<FiltroModel> lstFiltros = new List<FiltroModel>();
            foreach (var filtro in mapatematico.Filtros)
            {
                FiltroModel filtroCopy = new FiltroModel();
                filtroCopy.Ampliar = filtro.Ampliar;
                filtroCopy.Coordenadas = filtro.Coordenadas;
                filtroCopy.Dentro = filtro.Dentro;
                filtroCopy.FiltroAtributo = filtro.FiltroAtributo;
                filtroCopy.FiltroAtributoDesc = filtro.FiltroAtributoDesc;
                filtroCopy.FiltroColeccion = filtro.FiltroColeccion;
                filtroCopy.FiltroColeccionDesc = filtro.FiltroColeccionDesc;
                filtroCopy.FiltroComponente = filtro.FiltroComponente;
                filtroCopy.FiltroComponenteDesc = filtro.FiltroComponenteDesc;
                filtroCopy.FiltroId = filtro.FiltroId;
                filtroCopy.FiltroOperacion = filtro.FiltroOperacion;
                filtroCopy.FiltroOperacionDesc = filtro.FiltroOperacionDesc;
                filtroCopy.FiltroTipo = filtro.FiltroTipo;
                filtroCopy.Fuera = filtro.Fuera;
                filtroCopy.Habilitado = filtro.Habilitado;
                filtroCopy.PorcentajeInterseccion = filtro.PorcentajeInterseccion;
                filtroCopy.Tocando = filtro.Tocando;
                filtroCopy.Valor1 = filtro.Valor1;
                filtroCopy.Valor2 = filtro.Valor2;
                lstFiltros.Add(filtroCopy);
            }
            mapaTematicoCopy.Filtros = lstFiltros;

            return mapaTematicoCopy;
        }

        private bool CambioMapaTematico(MapaTematicoModel mapaTematicoGenerado, MapaTematicoModel mapatematico)
        {
            bool cambio = false;
            if (mapatematico.Componente.ComponenteId != mapaTematicoGenerado.Componente.ComponenteId
                || mapatematico.ComponenteAtributo.Atributo.AtributoId != mapaTematicoGenerado.ComponenteAtributo.Atributo.AtributoId)
            {
                cambio = true;
            }
            else
            {
                if (mapatematico.ComponenteAtributo.Atributo.Agrupacion != null && mapaTematicoGenerado.ComponenteAtributo.Atributo.Agrupacion == null
                    || mapatematico.ComponenteAtributo.Atributo.Agrupacion == null && mapaTematicoGenerado.ComponenteAtributo.Atributo.Agrupacion != null)
                {
                    cambio = true;
                }
                else
                {
                    if (mapatematico.ComponenteAtributo.Atributo.Agrupacion != null && mapaTematicoGenerado.ComponenteAtributo.Atributo.Agrupacion != null)
                    {
                        if (mapatematico.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId != mapaTematicoGenerado.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId)
                        {
                            cambio = true;
                        }
                    }
                }
                if (!cambio)
                {
                    if (mapatematico.Filtros.Count() != mapaTematicoGenerado.Filtros.Count())
                    {
                        cambio = true;
                    }
                    else
                    {
                        for (int i = 0; i < mapatematico.Filtros.Count(); i++)
                        {
                            if (mapatematico.Filtros[i].Ampliar != mapaTematicoGenerado.Filtros[i].Ampliar ||
                                mapatematico.Filtros[i].Coordenadas != mapaTematicoGenerado.Filtros[i].Coordenadas ||
                                mapatematico.Filtros[i].Dentro != mapaTematicoGenerado.Filtros[i].Dentro ||
                                mapatematico.Filtros[i].FiltroAtributo != mapaTematicoGenerado.Filtros[i].FiltroAtributo ||
                                mapatematico.Filtros[i].FiltroColeccion != mapaTematicoGenerado.Filtros[i].FiltroColeccion ||
                                mapatematico.Filtros[i].FiltroComponente != mapaTematicoGenerado.Filtros[i].FiltroComponente ||
                                mapatematico.Filtros[i].FiltroId != mapaTematicoGenerado.Filtros[i].FiltroId ||
                                mapatematico.Filtros[i].FiltroOperacion != mapaTematicoGenerado.Filtros[i].FiltroOperacion ||
                                mapatematico.Filtros[i].FiltroTipo != mapaTematicoGenerado.Filtros[i].FiltroTipo ||
                                mapatematico.Filtros[i].Fuera != mapaTematicoGenerado.Filtros[i].Fuera ||
                                mapatematico.Filtros[i].Habilitado != mapaTematicoGenerado.Filtros[i].Habilitado ||
                                mapatematico.Filtros[i].PorcentajeInterseccion != mapaTematicoGenerado.Filtros[i].PorcentajeInterseccion ||
                                mapatematico.Filtros[i].Tocando != mapaTematicoGenerado.Filtros[i].Tocando ||
                                mapatematico.Filtros[i].Valor1 != mapaTematicoGenerado.Filtros[i].Valor1 ||
                                mapatematico.Filtros[i].Valor2 != mapaTematicoGenerado.Filtros[i].Valor2)
                            {
                                cambio = true;
                                break;
                            }
                        }
                    }
                }
            }
            return cambio;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetVisualizacionViewVolver()
        {
            var mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);
            Session["GenerarResultado"] = 0;
            return GetVisualizacionView(mapatematico);
        }

        private VisualizacionModel GetVisualizacionModel(MT.ObjetoResultadoDetalle objetoResultadoDetalle, VisualizacionModel visualizacion)
        {
            return GetVisualizacionModel(objetoResultadoDetalle, visualizacion, false);
        }
        private VisualizacionModel GetVisualizacionModel(MT.ObjetoResultadoDetalle objetoResultadoDetalle, VisualizacionModel visualizacion, bool coloresConfig)
        {
            VisualizacionModel visualizacionModel = new VisualizacionModel { VerLabels = false };
            if (objetoResultadoDetalle != null)
            {
                visualizacionModel.Coloreado = visualizacion.Coloreado;
                visualizacionModel.ColoreadoDesc = visualizacion.ColoreadoDesc;
                visualizacionModel.Transparencia = visualizacion.Transparencia;
                visualizacionModel.ColorPrincipal = visualizacion.ColorPrincipal;
                visualizacionModel.ColorSecundario = visualizacion.ColorSecundario;
                visualizacionModel.ColorContorno = visualizacion.ColorContorno;
                visualizacionModel.CantidadContorno = visualizacion.CantidadContorno;
                visualizacionModel.Rangos = objetoResultadoDetalle.Rangos?.Count ?? 0;
                visualizacionModel.Distribucion = objetoResultadoDetalle.Distribucion;
                visualizacionModel.DistribucionDesc = GetDistribucion(objetoResultadoDetalle.Distribucion);
                visualizacionModel.Items = (objetoResultadoDetalle.Rangos ?? new List<MT.Rango>())
                    .Select((rango, idx) =>
                    {
                        var rangoAux = coloresConfig && idx < visualizacion.Items?.Count
                            ? new MT.Rango()
                            {
                                Leyenda = visualizacion.Items[idx].Leyenda,
                                Color = visualizacion.Items[idx].Color,
                                ColorBorde = visualizacion.Items[idx].ColorBorde,
                                AnchoBorde = visualizacion.Items[idx].AnchoBorde,
                                sIcono = visualizacion.Items[idx].sIcono
                            } : rango;

                        return new VisualizacionItemModel()
                        {
                            Casos = rangoAux.Casos,
                            Desde = rangoAux.Desde,
                            Hasta = rangoAux.Hasta,
                            Valor = rangoAux.Valor ?? string.Empty,
                            Leyenda = rangoAux.Leyenda,
                            Color = rangoAux.Color,
                            ColorBorde = rangoAux.ColorBorde,
                            AnchoBorde = rangoAux.AnchoBorde,
                            sIcono = rangoAux.sIcono
                        };
                    }).ToList();
            }
            return visualizacionModel;
        }

        [HttpGet]
        public ActionResult GetVisualizacionPartialView(int Rangos, int Distribucion)
        {
            var mapaTematicoModel = ((MapaTematicoModel)Session["MapaTematico"]);
            var objetoResultadoDetalle = GetObjetoResultadoDetalle(Session["GUID"].ToStringOrDefault(), Distribucion, Rangos);
            mapaTematicoModel.Visualizacion.Rangos = 0;
            mapaTematicoModel.Visualizacion = GetVisualizacionModel(objetoResultadoDetalle, mapaTematicoModel.Visualizacion); ;

            Session["MapaTematico"] = mapaTematicoModel;
            return PartialView("Partial/_VisualizacionPartialView", mapaTematicoModel);
        }

        private string GetDistribucion(long codDistribucion)
        {
            string distribucion = "UNIFORME";
            switch (codDistribucion)
            {
                case 1:
                    distribucion = "Uniforme";
                    break;
                case 2:
                    distribucion = "Cuantiles";
                    break;
                case 3:
                    distribucion = "Valores Individuales";
                    break;
            }
            return distribucion;
        }

        public ActionResult GetResumenView_Guardado(long ConfiguracionId)
        {
            var modelo = this.GetMapaTematicoById(ConfiguracionId);
            // si es mapa tematico siempre tendra un solo componente , pero se obtiene a traves de una tabla de relacion
            // por lo cual viene en una lista de un elemento.
            var model = new MapaTematicoModel()
            {
                Componente = this.GetComponentesByBiblioteca(ConfiguracionId).First(),
                Nombre = modelo.Nombre,
                Descripcion = modelo.Descripcion,
                MapaTematicoId = modelo.ConfiguracionId,
                Visibilidad = modelo.Visibilidad,
                Visualizacion = new VisualizacionModel
                {
                    Distribucion = modelo.Distribucion,
                    ColorPrincipal = modelo.ColorPrincipal,
                    ColorSecundario = modelo.ColorSecundario,
                    ColorContorno = modelo.ColorContorno,
                    CantidadContorno = modelo.CantidadContorno,
                    Rangos = int.Parse(modelo.Rangos.ToString()),
                    Coloreado = long.Parse(modelo.Color),
                    Transparencia = modelo.Transparencia
                }
            };

            if (modelo.Externo < 1)
            {
                var attr = GetAtributosById(modelo.Atributo);
                model.ComponenteAtributo = GetComponentesById(attr.ComponenteId ?? 0);
                model.ComponenteAtributo.Atributo = attr;
                if (modelo.Agrupacion > 0)
                {
                    model.ComponenteAtributo.Atributo.Agrupacion = GetAgrupacionesById(modelo.Agrupacion);
                }
            }
            else
            {
                model.Externo = 1;
                model.ComponenteAtributo.Atributo = new AtributoModel()
                {
                    AtributoId = modelo.Atributo,
                    EsImportado = true
                };
            }
            switch (modelo.Color)
            {
                case "2":
                    model.Visualizacion.ColoreadoDesc = "gradiente";
                    break;
                case "3":
                    model.Visualizacion.ColoreadoDesc = "manual";
                    break;
                default:
                    model.Visualizacion.ColoreadoDesc = "monocromático";
                    break;
            }
            model.FechaCreacion = modelo.FechaCreacion;
            modelo.ConfiguracionFiltro = this.ConfiguracionesFiltroByMT(modelo.ConfiguracionId);
            model.Filtros = modelo.ConfiguracionFiltro.Select(item =>
            {
                return new FiltroModel()
                {
                    FiltroId = item.FiltroId,
                    FiltroTipo = item.FiltroTipo,
                    FiltroComponente = item.FiltroComponente,
                    FiltroAtributo = item.FiltroAtributo,
                    FiltroOperacion = item.FiltroOperacion,
                    FiltroColeccion = item.FiltroColeccion,
                    Valor1 = item.Valor1,
                    Valor2 = item.Valor2,
                    Ampliar = item.Ampliar,
                    Dentro = item.Dentro,
                    Tocando = item.Tocando,
                    Fuera = item.Fuera,
                    Habilitado = item.Habilitado,
                    PorcentajeInterseccion = item.PorcentajeInterseccion,
                    Coordenadas = string.Join("|", item.ConfiguracionesFiltroGrafico.Select(cfg => cfg.sGeometry))
                };
            }).ToList();
            model.cantFiltrosAtributo = modelo.ConfiguracionFiltro.Count(cf => cf.FiltroTipo == 1);
            model.cantFiltrosGeografico = modelo.ConfiguracionFiltro.Count(cf => cf.FiltroTipo == 2);
            model.cantFiltrosColeccion = modelo.ConfiguracionFiltro.Count - model.cantFiltrosAtributo - model.cantFiltrosGeografico;
            modelo.ConfiguracionRango = this.ConfiguracionesRangoByMT(modelo.ConfiguracionId);
            model.Visualizacion.Items = modelo.ConfiguracionRango.Select(item =>
            {
                string valor = null;
                double? desde = null, hasta = null;
                if (modelo.Distribucion == 3)
                {
                    valor = item.Desde;
                }
                else
                {
                    if (string.IsNullOrEmpty((item.Desde ?? string.Empty).Trim()) || !double.TryParse(item.Desde, out double val))
                    {
                        valor = item.Desde;
                    }
                    else
                    {
                        desde = val;
                    }
                    if (!string.IsNullOrEmpty((item.Desde ?? string.Empty).Trim()))
                    {
                        hasta = double.Parse(item.Hasta);
                    }
                    else
                    {
                        hasta = 0d;
                    }
                }
                return new VisualizacionItemModel()
                {
                    Casos = item.Cantidad,
                    Leyenda = item.Leyenda,
                    Color = item.ColorRelleno,
                    ColorBorde = item.ColorLinea,
                    AnchoBorde = int.Parse(item.EspesorLinea.ToString()),
                    Valor = valor,
                    Desde = desde,
                    Hasta = hasta
                };
            }).ToList();

            string guid = Guid.NewGuid().ToString();
            Session["GUID"] = guid;
            var usuarioConectado = (UsuariosModel)Session["usuarioPortal"];

            var objetoResultadoDetalle = GenerarResultadoMapaTematico(guid, model, modelo.ConfiguracionId);

            if (objetoResultadoDetalle != null)
            {
                objetoResultadoDetalle.Rangos = GetObjetoResultadoDetalle(model.Visualizacion).Rangos;
                AuditoriaHelper.Register(usuarioConectado.Id_Usuario, string.Empty, Request, TipoOperacion.Consulta, Autorizado.Si, Eventos.GeneracionResultadosEnMT);
                Session["Geometry_Type"] = objetoResultadoDetalle.GeometryType;
            }
            Session["MapaTematico"] = model;
            Session["MT_Predefinido"] = 0;

            ViewBag.Plantillas = new SelectList(GetPlantillasForMapaTematico(), "IdPlantilla", "Nombre");
            ViewBag.ImagenesSatelitales = GetAllImagenSatelital();

            int verIdentificante = 1;
            string idComponenteCTParcela = GetParametrosGenerales().FirstOrDefault(p => p.Descripcion == "ID_COMPONENTE_CT_PARCELA")?.Valor;
            if (!string.IsNullOrEmpty(idComponenteCTParcela) && model.Componente.ComponenteId == Convert.ToInt64(idComponenteCTParcela))
            {
                verIdentificante = 0;
            }
            ViewBag.VerIdentificante = verIdentificante;
            return PartialView("Resumen", model);
        }

        public JsonResult RemoveFilter(long idFilter)
        {
            var model = (MapaTematicoModel)Session["MapaTematico"];

            model.Filtros.RemoveAll(x => x.FiltroId == idFilter);

            Session["MapaTematico"] = model;

            return Json(new { state = "OK" }, JsonRequestBehavior.AllowGet);
        }

        private List<ImagenSatelital> GetAllImagenSatelital()
        {
            //HttpResponseMessage respImagenSatelital = cliente.GetAsync("api/PloteoService/GetAllImagenSatelital").Result;
            //respImagenSatelital.EnsureSuccessStatusCode();
            //var lstImagenSatelital = (List<ImagenSatelital>)respImagenSatelital.Content.ReadAsAsync<IEnumerable<ImagenSatelital>>().Result;
            //return lstImagenSatelital;
            return new List<ImagenSatelital>();
        }

        //public ActionResult GetVisualizacionView_Guardada(string parametros)
        //{
        //    var paramType = new
        //    {
        //        ConfiguracionId = 0L,
        //        CategoriaId = 0,
        //        idDistrito = string.Empty,
        //        idColeccion = 0L,
        //        fechaDesde = new DateTime(),
        //        fechaHasta = new DateTime(),
        //        idCargaTecnica = 0L
        //    };
        //    var config = JsonConvert.DeserializeAnonymousType(parametros, paramType);
        //    MvcApplication.GetLogger().LogInfo("Carga Visualizacion Guardada App Comerciales Inicio");
        //    MapaTematicoConfiguracionModelo modelo = this.GetMapaTematicoById(config.ConfiguracionId);
        //    List<ComponenteModel> componentes = this.GetComponentesByBiblioteca(config.ConfiguracionId);
        //    MapaTematicoModel model = new MapaTematicoModel();

        //    // si es mapa tematico siempre tendra un solo componente , pero se obtiene a traves de una tabla de relacion por lo cual viene en una lista de un elemento.
        //    model.Componente = componentes[0];

        //    if (modelo.Externo < 1)
        //    {
        //        AtributoModel atributo = GetAtributosById(modelo.Atributo);
        //        model.ComponenteAtributo = GetComponentesById(atributo.ComponenteId ?? 0);
        //        model.ComponenteAtributo.Atributo = atributo;

        //        if (modelo.Agrupacion > 0)
        //        {
        //            model.ComponenteAtributo.Atributo.Agrupacion = GetAgrupacionesById(modelo.Agrupacion);
        //        }
        //    }

        //    model.Nombre = modelo.Nombre;
        //    model.Descripcion = modelo.Descripcion;
        //    model.MapaTematicoId = modelo.ConfiguracionId;

        //    model.Visualizacion.Distribucion = modelo.Distribucion;
        //    if (modelo.Distribucion == 1)
        //    {
        //        model.Visualizacion.DistribucionDesc = "Uniforme";
        //    }
        //    else if (modelo.Distribucion == 2)
        //    {
        //        model.Visualizacion.DistribucionDesc = "Cuantiles";
        //    }
        //    else if (modelo.Distribucion == 3)
        //    {
        //        model.Visualizacion.DistribucionDesc = "Valores individuales";
        //    }
        //    model.Visualizacion.Rangos = int.Parse(modelo.Rangos.ToString());
        //    model.Visualizacion.Coloreado = long.Parse(modelo.Color);
        //    model.Visualizacion.Transparencia = modelo.Transparencia;
        //    model.Visibilidad = modelo.Visibilidad;
        //    model.Visualizacion.ColorPrincipal = modelo.ColorPrincipal;
        //    model.Visualizacion.ColorSecundario = modelo.ColorSecundario;
        //    model.Visualizacion.ColorContorno = modelo.ColorContorno;
        //    model.Visualizacion.CantidadContorno = modelo.CantidadContorno;
        //    var coloreado = "";
        //    if (modelo.Color == "1")
        //    {
        //        coloreado = "monocromático";
        //    }
        //    else if (modelo.Color == "2")
        //    {
        //        coloreado = "gradiente";
        //    }
        //    else if (modelo.Color == "3")
        //    {
        //        coloreado = "manual";
        //    }
        //    else
        //    {
        //        coloreado = "monocromático";
        //    }
        //    model.Visualizacion.ColoreadoDesc = coloreado;
        //    model.FechaCreacion = modelo.FechaCreacion;

        //    #region Filtro
        //    //Leer la configuracion de mt_config_filtro. 
        //    //id_filtro_tipo=3 => coleccion, 
        //    //id_filtro_tipo=1 y id_operacion_tipo=1 => distrito
        //    //id_filtro_tipo=1 y id_operacion_tipo=7 => fecha

        //    modelo.ConfiguracionFiltro = this.ConfiguracionesFiltroByMT(modelo.ConfiguracionId);

        //    model.Filtros = new List<FiltroModel>();
        //    FiltroModel filtro = new FiltroModel();
        //    ////filtro.FiltroId = item.FiltroId;
        //    //filtro.FiltroId = 0;
        //    List<MT.ConfiguracionFiltro> lstConfiguracionFiltro = new List<MT.ConfiguracionFiltro>();
        //    MT.ConfiguracionFiltro configuracionFiltro = new MT.ConfiguracionFiltro();
        //    int cantFiltrosAtributo = 0;
        //    int cantFiltrosColeccion = 0;
        //    if (config.idDistrito != string.Empty && config.idCargaTecnica == 0)
        //    {
        //        //Filtro por id_distrito
        //        lstConfiguracionFiltro = modelo.ConfiguracionFiltro.Where(f => f.ConfiguracionId == config.ConfiguracionId && f.FiltroTipo == 1 && f.FiltroOperacion == 1 && f.Habilitado == 1).ToList();
        //        if (lstConfiguracionFiltro.Count > 0)
        //        {
        //            configuracionFiltro = lstConfiguracionFiltro[0];
        //            filtro = new FiltroModel();
        //            filtro.FiltroTipo = configuracionFiltro.FiltroTipo;
        //            //Parcelas
        //            filtro.FiltroComponente = configuracionFiltro.FiltroComponente;
        //            //idDistrito
        //            filtro.FiltroAtributo = configuracionFiltro.FiltroAtributo;
        //            //Igual a
        //            filtro.FiltroOperacion = configuracionFiltro.FiltroOperacion;
        //            filtro.Valor1 = config.idDistrito.ToString();

        //            filtro.Habilitado = 1;

        //            model.Filtros.Add(filtro);

        //            cantFiltrosAtributo++;
        //        }
        //    }
        //    else if (config.idColeccion > 0)
        //    {
        //        //Filtro por id_distrito
        //        lstConfiguracionFiltro = modelo.ConfiguracionFiltro.Where(f => f.ConfiguracionId == config.ConfiguracionId && f.FiltroTipo == 3 && f.Habilitado == 1).ToList();
        //        if (lstConfiguracionFiltro.Count > 0)
        //        {
        //            configuracionFiltro = lstConfiguracionFiltro[0];
        //            filtro = new FiltroModel();
        //            filtro.FiltroTipo = configuracionFiltro.FiltroTipo;
        //            filtro.FiltroOperacion = configuracionFiltro.FiltroOperacion;
        //            filtro.FiltroColeccion = config.idColeccion;

        //            filtro.Habilitado = 1;

        //            model.Filtros.Add(filtro);

        //            cantFiltrosColeccion++;
        //        }
        //    }
        //    else if (config.idCargaTecnica > 0)
        //    {
        //        //Filtro por idCargaTecnica
        //        lstConfiguracionFiltro = modelo.ConfiguracionFiltro.Where(f => f.ConfiguracionId == config.ConfiguracionId && f.FiltroTipo == 1 && f.FiltroOperacion == 1 && f.Habilitado == 1).ToList();
        //        if (lstConfiguracionFiltro.Count > 0)
        //        {
        //            configuracionFiltro = lstConfiguracionFiltro[0];
        //            filtro = new FiltroModel();
        //            filtro.FiltroTipo = configuracionFiltro.FiltroTipo;
        //            //Parcelas
        //            filtro.FiltroComponente = configuracionFiltro.FiltroComponente;
        //            //idCargaTecnica
        //            filtro.FiltroAtributo = configuracionFiltro.FiltroAtributo;
        //            //Igual a
        //            filtro.FiltroOperacion = configuracionFiltro.FiltroOperacion;
        //            filtro.Valor1 = config.idCargaTecnica.ToString();

        //            filtro.Habilitado = 1;

        //            model.Filtros.Add(filtro);

        //            cantFiltrosAtributo++;
        //        }
        //    }
        //    if (config.idCargaTecnica <= 0)
        //    {
        //        //Filtro por fechaDesde fechaHasta
        //        lstConfiguracionFiltro = modelo.ConfiguracionFiltro.Where(f => f.ConfiguracionId == config.ConfiguracionId && f.FiltroTipo == 1 && f.FiltroOperacion == 7 && f.Habilitado == 1).ToList();
        //        if (lstConfiguracionFiltro.Count > 0)
        //        {
        //            configuracionFiltro = lstConfiguracionFiltro[0];
        //            filtro = new FiltroModel();
        //            filtro.FiltroTipo = configuracionFiltro.FiltroTipo;
        //            filtro.FiltroComponente = configuracionFiltro.FiltroComponente;
        //            filtro.FiltroAtributo = configuracionFiltro.FiltroAtributo;
        //            filtro.FiltroOperacion = configuracionFiltro.FiltroOperacion;
        //            filtro.Valor1 = config.fechaDesde.ToString("dd'/'MM'/'yyyy");
        //            filtro.Valor2 = config.fechaHasta.ToString("dd'/'MM'/'yyyy");

        //            filtro.Habilitado = 1;

        //            model.Filtros.Add(filtro);

        //            cantFiltrosAtributo++;
        //        }
        //    }
        //    model.cantFiltrosAtributo = cantFiltrosAtributo;
        //    model.cantFiltrosColeccion = cantFiltrosColeccion;
        //    #endregion

        //    #region Visualizacion Rangos
        //    modelo.ConfiguracionRango = this.ConfiguracionesRangoByMT(modelo.ConfiguracionId);
        //    model.Visualizacion.Items = new List<VisualizacionItemModel>();
        //    VisualizacionItemModel cr;

        //    foreach (var item in modelo.ConfiguracionRango.OrderBy(o => o.Orden))
        //    {
        //        cr = new VisualizacionItemModel();

        //        if (item.Desde.Trim() != string.Empty)
        //        {
        //            try
        //            {
        //                cr.Desde = long.Parse(item.Desde);
        //                //EAQ 20190924 TFS 10993  no se porque estaba comentado 
        //                //pero mejora los mapas de analisis predefinido
        //                //cr.Valor = item.Desde;
        //                cr.Valor = item.Desde;
        //                //EAQ 20190924
        //            }
        //            catch
        //            {
        //                cr.Desde = 0;
        //                cr.Valor = item.Desde;
        //            }
        //        }
        //        else
        //        {
        //            cr.Desde = 0;
        //            cr.Valor = item.Desde;
        //        }
        //        if (item.Hasta.Trim() != string.Empty)
        //        {
        //            cr.Hasta = long.Parse(item.Hasta);
        //        }
        //        else
        //        {
        //            cr.Hasta = 0;
        //        }
        //        cr.Casos = item.Cantidad;
        //        cr.Leyenda = item.Leyenda;
        //        cr.Color = item.ColorRelleno;
        //        cr.ColorBorde = item.ColorLinea;
        //        cr.AnchoBorde = int.Parse(item.EspesorLinea.ToString());
        //        //cr.Icono = item.Icono;

        //        model.Visualizacion.Items.Add(cr);
        //    }
        //    #endregion

        //    //cargar Predefinido en base a config.ConfiguracionId
        //    //se agrega filtro por categoria ya que puede haber varias plantillas con una misma configuración pero distinta categoria. mantis 1386: Bug 6127
        //    MT.Predefinido predefinido = GetPredefinidoByConfiguracionId(config.ConfiguracionId, config.CategoriaId);
        //    Plantilla plantilla = null;
        //    if (predefinido != null)
        //    {
        //        //plantilla = GetPlantillaByIdPlantillaCategoria(predefinido.IdPlantillaCategoria);
        //        plantilla = GetPlantillaByIdPlantillaCategoria(config.CategoriaId);
        //    }

        //    string guid = System.Guid.NewGuid().ToString();
        //    Session["GUID"] = guid;
        //    if (predefinido != null && (predefinido.IdPredefinido == 4 || predefinido.IdPredefinido == 5))
        //    {
        //        //Hace que habilite el boton de iconos en visualizacion
        //        model.Componente.Graficos = 3;
        //    }
        //    UsuariosModel usuarioConectado = (UsuariosModel)Session["usuarioPortal"];
        //    MT.ObjetoResultadoDetalle objetoResultadoDetalle = GenerarResultadoMapaTematico(guid, model, usuarioConectado.Id_Usuario, config.ConfiguracionId);
        //    if (objetoResultadoDetalle != null)
        //    {
        //        Session["Geometry_Type"] = objetoResultadoDetalle.GeometryType;
        //        bool leyendaConfig = (predefinido != null && predefinido.LeyendaConfig == 1 ? true : false);
        //        model.Visualizacion = GetVisualizacionModel(objetoResultadoDetalle, model.Visualizacion, true, leyendaConfig);
        //    }
        //    Session["MapaTematico"] = model;
        //    if (modelo.Externo > 0)
        //    {
        //        MvcApplication.GetLogger().LogInfo("Carga Resumen Guardado Fin");
        //        return PartialView("AvisoAtributoImportado", model);
        //    }
        //    //Session["ConfiguracionId"] = config.ConfiguracionId;
        //    Session["MT_Predefinido"] = 1;

        //    List<ParametrosGeneralesModel> lstParametrosGenerales = GetParametrosGenerales();
        //    int rangosMaximos = 0;
        //    ParametrosGeneralesModel paramRangosMaximos = lstParametrosGenerales.FirstOrDefault(p => p.Id_Parametro == 29);
        //    if (paramRangosMaximos != null)
        //    {
        //        rangosMaximos = Convert.ToInt32(paramRangosMaximos.Valor);
        //    }
        //    ViewBag.RangosMaximos = rangosMaximos;

        //    if (predefinido != null)
        //    {
        //        AuditoriaHelper.Register(usuarioConectado.Id_Usuario, string.Empty, Request, TipoOperacion.EstudiosComerciales, Autorizado.Si, this.getEvento(predefinido.IdPredefinido));
        //    }

        //    ViewBag.ConfiguracionId = config.ConfiguracionId;
        //    Session["ConfiguracionId"] = config.ConfiguracionId;
        //    ViewBag.IdConfigCategoria = modelo.idConfigCategoria;
        //    Session["IdConfigCategoria"] = modelo.idConfigCategoria;
        //    ViewBag.NombreColeccion = (predefinido != null ? predefinido.NombreColeccion : string.Empty);
        //    Session["NombreColeccion"] = (predefinido != null ? predefinido.NombreColeccion : string.Empty);
        //    ViewBag.PredefinidoId = (predefinido != null ? predefinido.IdPredefinido : 0);
        //    Session["PredefinidoId"] = (predefinido != null ? predefinido.IdPredefinido : 0);
        //    ViewBag.IdPlantilla = (plantilla != null ? plantilla.IdPlantilla : 0);
        //    Session["IdPlantilla"] = (plantilla != null ? plantilla.IdPlantilla : 0);
        //    ViewBag.IdDistrito = config.idDistrito;
        //    Session["IdDistrito"] = config.idDistrito;


        //    Session["GenerarResultado"] = 1;

        //    Session["MapaTematico"] = model;
        //    ViewBag.Plantillas = new SelectList(GetPlantillasForMapaTematico(), "IdPlantilla", "Nombre");
        //    ViewBag.ImagenesSatelitales = GetAllImagenSatelital();

        //    ViewBag.VerIdentificante = 1;

        //    return PartialView("Visualizacion", model);
        //}

        [HttpGet]
        public ActionResult CargarUltimoMapaTematico()
        {
            var model = (MapaTematicoModel)Session["MapaTematico"];
            if (model != null)
            {
                ViewBag.Plantillas = new SelectList(GetPlantillasForMapaTematico(), "IdPlantilla", "Nombre");
                ViewBag.ImagenesSatelitales = GetAllImagenSatelital();

                ViewBag.VerIdentificante = 1;

                //Si viene de un predefinido y llamar a visualizacion, sino a resumen
                int mtVolverPredefinido = (Session["MT_Volver_Predefinido"] == null ? 0 : (int)Session["MT_Volver_Predefinido"]);
                if (mtVolverPredefinido == 1)
                {
                    Session["MT_Predefinido"] = 1;

                    List<ParametrosGeneralesModel> lstParametrosGenerales = GetParametrosGenerales();
                    int rangosMaximos = 0;
                    ParametrosGeneralesModel paramRangosMaximos = lstParametrosGenerales.FirstOrDefault(p => p.Id_Parametro == 29);
                    if (paramRangosMaximos != null)
                    {
                        rangosMaximos = Convert.ToInt32(paramRangosMaximos.Valor);
                    }
                    ViewBag.RangosMaximos = rangosMaximos;
                    ViewBag.PredefinidoId = (Session["PredefinidoId"] == null ? 0 : Convert.ToInt64(Session["PredefinidoId"]));
                    ViewBag.ConfiguracionId = (Session["ConfiguracionId"] == null ? 0 : Convert.ToInt32(Session["ConfiguracionId"]));
                    ViewBag.IdConfigCategoria = (Session["IdConfigCategoria"] == null ? 0 : Convert.ToInt32(Session["IdConfigCategoria"]));
                    ViewBag.NombreColeccion = (Session["NombreColeccion"] == null ? string.Empty : Session["NombreColeccion"].ToString());
                    ViewBag.IdPlantilla = (Session["IdPlantilla"] == null ? 0 : Convert.ToInt32(Session["IdPlantilla"]));
                    ViewBag.IdDistrito = Session["IdDistrito"].ToString();
                    return PartialView("Visualizacion", model);
                }
                else
                {
                    return PartialView("Resumen", model);
                }
            }
            else
            {
                return Json(new { msg = "No tiene mapa cargado" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetResumenView(MapaTematicoModel model)
        {
            var mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);
            mapatematico.Visualizacion = model.Visualizacion;
            if (mapatematico != null)
            {
                mapatematico.Visualizacion = model.Visualizacion;
                model = mapatematico;
            }

            //bool? fromUbicaciones = Session["FromUbicaciones"] == null ? false : (bool)Session["FromUbicaciones"];
            //if (fromUbicaciones != null && fromUbicaciones == true)
            //{
            //    Session["MT_Volver_Predefinido"] = 0;
            //    Session["FromUbicaciones"] = false;
            //    MT.ObjetoResultadoDetalle objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            //    ActualizarResultadoMapaTematico(objetoResultadoDetalle);
            //    Session["MapaTematico"] = mapatematico;
            //    Session["Geometry_Type"] = 1;
            //    Session["objetoResultadoDetalle"] = objetoResultadoDetalle;
            //    return Json(new { CallMT = true, LayerName = ConfigurationManager.AppSettings[string.Format("layerCargaTecnica_{0}", Session["TIPO_RED"])] });
            //}

            long atributoId = long.Parse(mapatematico.ComponenteAtributo.Atributo.AtributoId.ToString());
            long agrupacionId = long.Parse(mapatematico.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId.ToString());
            mapatematico.Componente = this.GetComponentesById(mapatematico.Componente.ComponenteId);

            if (mapatematico.ComponenteAtributo.Atributo.EsImportado)
            {
                DatoExternoConfiguracionModel dec = GetDatoExternoConfiguracionById(atributoId);
                AtributoModel atributoimportado = new AtributoModel();
                atributoimportado.EsImportado = true;
                atributoimportado.Nombre = dec.Nombre;
                atributoimportado.AtributoId = dec.DatoExternoConfiguracionId;
                //atributoimportado.TipoDatoId = dec.TipoDato;
                mapatematico.ComponenteAtributo = this.GetComponentesById(mapatematico.ComponenteAtributo.ComponenteId);
                mapatematico.ComponenteAtributo.Atributo = atributoimportado;

            }
            else
            {
                mapatematico.ComponenteAtributo = this.GetComponentesById(mapatematico.ComponenteAtributo.ComponenteId);
                mapatematico.ComponenteAtributo.Atributo = this.GetAtributosById(atributoId);
            }

            if (agrupacionId != 0)
            {
                mapatematico.ComponenteAtributo.Atributo.Agrupacion = this.GetAgrupacionesById(agrupacionId);
            }

            Session["MapaTematico"] = mapatematico;
            ViewBag.Plantillas = new SelectList(GetPlantillasForMapaTematico(), "IdPlantilla", "Nombre");
            ViewBag.ImagenesSatelitales = GetAllImagenSatelital();

            ViewBag.VerIdentificante = 1;

            string guid = Session["GUID"].ToStringOrDefault();

            if (!model.Visualizacion.VerLabels)
            {
                var response = cliente.GetAsync("api/MapasTematicosService/BorrarDescriptionGuid?idGuid=" + guid).Result;
                response.EnsureSuccessStatusCode();
            }

            var objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            UpdateRangosIcono(objetoResultadoDetalle);
            Session["MT_Volver_Predefinido"] = 0;
            Session["objetoResultadoDetalle"] = objetoResultadoDetalle;
            return PartialView("Resumen", mapatematico);

            ////long configuracionId = (Session["ConfiguracionId"] == null ? 0 : (long)Session["ConfiguracionId"]);
            //int mtPredefinido = (Session["MT_Predefinido"] == null ? 0 : (int)Session["MT_Predefinido"]);
            ////Si es 1 viene de app Comercial
            //if (mtPredefinido == 1)
            //{
            //    //Actualizar Resultado Mapa Tematico - campo Rango
            //    MT.ObjetoResultadoDetalle objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            //    objetoResultadoDetalle.GUID = guid;
            //    int cantUpd = ActualizarResultadoMapaTematico(objetoResultadoDetalle);
            //    int idPredefinido = (Session["PredefinidoId"] == null ? 0 : Convert.ToInt32(Session["PredefinidoId"]));
            //    //Todos los Medidores y Medidores diferenciados por marca
            //    if (idPredefinido == 4 || idPredefinido == 5)
            //    {
            //        if (mapatematico.Visualizacion.Items != null && mapatematico.Visualizacion.Items.Count > 0 && !string.IsNullOrEmpty(mapatematico.Visualizacion.Items[0].sIcono))
            //        {
            //            //Si eligio icono paso geometry de poligono de parc a geometry punto y cambio geometry_type de 3 a 1
            //            int cantUpdPasarAPunto = ActualizarResultadoMapaTematicoPasarAPunto(guid);
            //            Session["Geometry_Type"] = 1;
            //        }
            //    }
            //    if (mapatematico.Nombre == string.Empty)
            //    {
            //        mapatematico.Nombre = "Sin Titulo " + System.DateTime.Now;
            //        mapatematico.Descripcion = "";
            //    }
            //    Session["MapaTematico"] = mapatematico;
            //    Session["MT_Predefinido"] = 0;
            //    Session["MT_Volver_Predefinido"] = 1;
            //    Session["objetoResultadoDetalle"] = objetoResultadoDetalle;
            //    return Json(new { CallMT = true, LayerName = ConfigurationManager.AppSettings[string.Format("layerCargaTecnica_{0}", Session["TIPO_RED"])] });
            //}
            //else
            //{
            //    MT.ObjetoResultadoDetalle objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            //    UpdateRangosIcono(objetoResultadoDetalle);
            //    Session["MT_Volver_Predefinido"] = 0;
            //    Session["objetoResultadoDetalle"] = objetoResultadoDetalle;
            //    return PartialView("Resumen", mapatematico);
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generar(MapaTematicoModel model)
        {
            var mapatematico = Session["MapaTematico"] as MapaTematicoModel;
            mapatematico.Nombre = model.Nombre;
            mapatematico.Descripcion = model.Descripcion;
            var objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            objetoResultadoDetalle.GUID = Session["GUID"].ToStringOrDefault();
            ActualizarResultadoMapaTematico(objetoResultadoDetalle);
            var resp = cliente.PostAsync($"api/MapasTematicosService/GetMapaTematicoExtents?guid={objetoResultadoDetalle.GUID}", null).Result;
            resp.EnsureSuccessStatusCode();
            string extent = resp.Content.ReadAsAsync<string>().Result;
            if (mapatematico.ComponenteAtributo.Atributo.EsImportado)
            {
                this.BorrarDatoExternoByUsuario(UsuarioConectado.Id_Usuario);
            }

            AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, TipoOperacion.Consulta, Autorizado.Si, Eventos.VerMapaMT);
            Session["MapaTematico"] = mapatematico;
            Session["objetoResultadoDetalle"] = objetoResultadoDetalle;
            return RedirectToAction("GenerarLayerMapaTematico", "Mapa", new { extent });

            //if (model.Nombre != null)
            //{
            //    MapaTematicoConfiguracionModelo mapatematicoEncontrado = this.GetMapaTematicoByName(model.Nombre);

            //    if (mapatematicoEncontrado != null)
            //    {
            //        if (mapatematicoEncontrado.Usuario == usuarioPortal.Id)
            //        {
            //            if (mapatematicoEncontrado.ConfiguracionId == mapatematico.MapaTematicoId)
            //            {
            //                model.Nombre = model.Nombre;
            //                mapatematico.Nombre = model.Nombre;
            //            }
            //            else
            //            {
            //                //Mapa tematico nuevo

            //                //model.Nombre = model.Nombre + " Copia";
            //                //mapatematico.Nombre = model.Nombre;
            //                mapatematico.mensaje = "Ya existe una definición con dicho nombre. Cambie el nombre para continuar.";
            //                mapatematico.Nombre = model.Nombre;
            //                mapatematico.Descripcion = model.Descripcion;
            //                ViewBag.ImagenesSatelitales = GetAllImagenSatelital();
            //                return PartialView("Resumen", mapatematico);
            //            }
            //            mapatematico.Descripcion = model.Descripcion;
            //        }
            //    }
            //    else
            //    {
            //        mapatematico.Nombre = model.Nombre;
            //        mapatematico.Descripcion = model.Descripcion;
            //    }
            //}
            //else
            //{
            //    mapatematico.Nombre = "Sin Titulo " + DateTime.Now;
            //    mapatematico.Descripcion = "";
            //}

            //if (mapatematico.ComponenteAtributo.Atributo.EsImportado)
            //{
            //    this.BorrarDatoExternoByUsuario(usuarioPortal.Id);
            //}

            //AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, TipoOperacion.Consulta, Autorizado.Si, Eventos.VerMapaMT);
            //Session["MapaTematico"] = mapatematico;
            //Session["objetoResultadoDetalle"] = objetoResultadoDetalle;
            //return Json(new { Ok = true });
        }
        public FileResult ExportarAExcelBiblioteca(long id)
        {
            using (var memStreamTemp = new MemoryStream())
            using (var package = new ExcelPackage(memStreamTemp))
            {
                MapaTematicoConfiguracionModelo modelo = this.GetMapaTematicoById(id);
                MT.ComponenteConfiguracion cc = this.GetComponenteConfiguracionByMTId(id);
                ExcelWorksheet hojaAtributos = package.Workbook.Worksheets.Add(modelo.Nombre);
                hojaAtributos.Cells[1, 1].Value = "Configuración ID";
                hojaAtributos.Cells[1, 2].Value = "Componente ID";
                //int fila = 2;

                hojaAtributos.Cells[2, 1].Value = cc.ConfiguracionId;
                hojaAtributos.Cells[2, 2].Value = cc.ComponenteId;

                using (var range = hojaAtributos.Cells[1, 1, 1, 2])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                hojaAtributos.Cells.AutoFitColumns(0);

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mapa Temático " + modelo.Nombre + ".xlsx"); ;
            }
        }

        internal MT.ObjetoResultadoDetalle GetObjetoResultadoDetalle(VisualizacionModel visualizacionModel)
        {
            MT.ObjetoResultadoDetalle objetoResultadoDetalle = null;
            if (visualizacionModel != null)
            {
                List<MT.Rango> lstRangos = new List<MT.Rango>();
                objetoResultadoDetalle = new MT.ObjetoResultadoDetalle()
                {
                    Distribucion = visualizacionModel.Distribucion,
                    Transparencia = visualizacionModel.Transparencia,
                    GeometryType = Convert.ToInt32(Session["Geometry_Type"])
                };
                bool tieneIcono = visualizacionModel.Items.Any(i => !string.IsNullOrEmpty(i.sIcono)) || objetoResultadoDetalle.GeometryType == 3;
                foreach (var item in visualizacionModel.Items)
                {
                    MT.Rango rango = new MT.Rango()
                    {
                        Casos = Convert.ToInt32(item.Casos.ToString()),
                        Desde = item.Desde,
                        Hasta = item.Hasta
                    };
                    /* --------> ERNESTO.
                     * truchada que hago porque hay inconsistencia entre campos de la DB
                     * - MT_OBJETO_RESULTADO admite rangos y valores NULL
                     * - MT_CONFIG_RANGO no admite NULLs para campos DESDE Y HASTA.
                     * cuando el valor de rango es NULL, falla cuando se levanta la configuracion desde biblioteca
                     * porque los rangos se calculan teniendo en cuenta un " " (caracter de espacio)
                     * que es el que se graba en MT_CONFIG_RANGO 
                     * 
                     * PREGUNTAR A AGUSTIN SI SE AJUSTAN LOS CAMPOS O SE DEJA ESTA NEGRADA. 
                     * (puede darse el caso de que el " " sea un valor de rango valido, por lo que 
                     * puede ser que en esos casos, esta solucion rompa el funcionamiento                     
                     * 
                     */

                    if (item.Valor != null && item.Valor.Trim() != string.Empty)
                    {
                        rango.Valor = item.Valor;
                        rango.Desde = null;
                        rango.Hasta = null;
                    }
                    rango.Leyenda = item.Leyenda;
                    rango.Color = item.Color;
                    if (item.AnchoBorde == 0)
                    {
                        rango.ColorBorde = item.Color;
                    }
                    else
                    {
                        rango.ColorBorde = item.ColorBorde;
                    }
                    rango.AnchoBorde = item.AnchoBorde;

                    if (tieneIcono)
                    {
                        if (string.IsNullOrEmpty(item.sIcono))
                        {
                            item.sIcono = "glyphicon-one-fine-dot";
                        }
                        rango.Icono = item.GetSVGIcon(objetoResultadoDetalle.GeometryType.GetValueOrDefault());
                        rango.sIcono = item.sIcono;
                    }
                    lstRangos.Add(rango);
                }
                objetoResultadoDetalle.Rangos = lstRangos;
            }
            return objetoResultadoDetalle;
        }

        public bool UpdateRangosIcono(MT.ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            HttpResponseMessage resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/MapasTematicosService/UpdateRangosIcono", objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<bool>().Result;
        }

        public ActionResult GetBibliotecas()
        {
            List<BibliotecasItemModel> func(List<ComponenteConfiguracionModel> lista)
            {
                return (lista ?? new List<ComponenteConfiguracionModel>()).Where(b => b.Configuracion.idConfigCategoria != 2)
                                    .GroupBy(elem => elem.Componente.Nombre)
                                    .Select(grp => new BibliotecasItemModel
                                    {
                                        ComponenteNombre = grp.Key,
                                        bibliotecas = grp.Select(c => c.Configuracion).ToList()
                                    }).ToList();
            }

            var BiblioModel = new BibliotecasViewModel()
            {
                bibliotecasPublicas = func(GetMapaTematicosPublicos()),
                bibliotecasPrivadas = func(GetBibliotecasPrivadasByUsuario(this.UsuarioConectado.Id_Usuario))
            };

            bool habilitado = GetParametrosGenerales().FirstOrDefault(p => p.Id_Parametro == 30)?.Valor == "1";
            ViewBag.PublicarDespublicarEnabled = habilitado && SeguridadController.ExisteFuncion(Seguridad.PublicarDespublicarBibliotecaMT);
            ViewBag.ElimnarPublicaEnabled = SeguridadController.ExisteFuncion(Seguridad.EliminarBibliotecaMT);

            this.BorrarDatoExternoByUsuario(this.UsuarioConectado.Id_Usuario);

            return PartialView("Bibliotecas", BiblioModel);
        }

        public FileResult ExportarAExcel(long id)
        {
            using (var memStreamTemp = new MemoryStream())
            using (var package = new ExcelPackage(memStreamTemp))
            {
                MapaTematicoConfiguracionModelo modelo = this.GetMapaTematicoById(id);
                MT.ComponenteConfiguracion cc = this.GetComponenteConfiguracionByMTId(id);
                ExcelWorksheet hojaAtributos = package.Workbook.Worksheets.Add(modelo.Nombre);
                hojaAtributos.Cells[1, 1].Value = "Configuración ID";
                hojaAtributos.Cells[1, 2].Value = "Componente ID";

                hojaAtributos.Cells[2, 1].Value = modelo.ConfiguracionId;
                hojaAtributos.Cells[2, 2].Value = cc.ComponenteId;

                using (var range = hojaAtributos.Cells[1, 1, 1, 2])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                hojaAtributos.Cells.AutoFitColumns(0);

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MapasTematicos " + modelo.Nombre + ".xlsx"); ;
            }
        }
        [HttpPost]
        public JsonResult ImportarAExcel()
        {
            long id = 0;
            long ComponenteId = 0;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file == null)
                {
                    var ex = new Exception("El fichero esta Nulo");
                    MvcApplication.GetLogger().LogError("Importar Biblioteca", ex);
                    throw ex;
                }
                using (var package = new ExcelPackage(Request.Files[i].InputStream))
                {
                    var hoja = package.Workbook.Worksheets[0];
                    id = long.Parse(hoja.Cells[2, 1].Text);
                    ComponenteId = long.Parse(hoja.Cells[2, 2].Text);
                }

            }
            var listaPrivados = GetBibliotecasPrivadasByUsuario(UsuarioConectado.Id_Usuario);
            var componenteConfiguracionModel = listaPrivados.FirstOrDefault(p => p.ConfiguracionId == id);

            int existe = 0;
            if (listaPrivados.Exists(p => p.ConfiguracionId == id))
            {
                existe = 1;
                if (componenteConfiguracionModel != null && UsuarioConectado.Id_Usuario == componenteConfiguracionModel.Configuracion.Usuario)
                {
                    existe = 2;
                }
            }
            else
            {
                existe = CallCopyMapaTematico(id, ComponenteId, existe);
            }
            return Json(new { Existe = existe, Id = id, ComponenteId = ComponenteId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CopyMapaTematicoDeExcel(long id, long componenteId, int existe)
        {
            int existeOrig = CallCopyMapaTematico(id, componenteId, existe);
            return Json(existeOrig);
        }

        public int CallCopyMapaTematico(long id, long componenteId, int existe)
        {
            //TODO RA - devolver si se importa o no porq si no existe el id no deberia hacer nada
            int existeOrig = 0;
            MapaTematicoConfiguracionModelo modelo = this.GetMapaTematicoById(id);
            if (modelo != null)
            {
                existeOrig = 1;
                modelo.ComponenteConfiguracion = new List<ComponenteConfiguracionModel>();
                var componente = new ComponenteConfiguracionModel();
                componente.ComponenteId = componenteId;
                componente.ConfiguracionId = modelo.ConfiguracionId;
                modelo.ComponenteConfiguracion.Add(componente);
                if (modelo.FechaBaja != null)
                {
                    modelo.FechaBaja = null;
                }

                UsuariosModel usuarioPortal = (UsuariosModel)Session["usuarioPortal"];
                //Usuario usuarioPortal = new Usuario();
                //usuarioPortal.Id = 0;
                modelo.Usuario = usuarioPortal.Id_Usuario;
                if (existe == 2)
                {
                    modelo.Nombre = modelo.Nombre + " " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                this.CopyMapaTematico(modelo);
            }
            return existeOrig;
        }

        [HttpPost]
        public JsonResult CalcularCasos(long distribucion, double desde1, double hasta1, double desde2, double hasta2)
        {
            string guid = Session["GUID"].ToStringOrDefault();
            int casos1 = 0;
            int casos2 = 0;
            if (desde1 != 0 || hasta1 != 0)
            {
                casos1 = GetNroCasos(guid, distribucion, desde1, hasta1);
            }
            if (desde2 != 0 || hasta2 != 0)
            {
                casos2 = GetNroCasos(guid, distribucion, desde2, hasta2);
            }
            return Json(new { Casos1 = casos1, Casos2 = casos2 }, JsonRequestBehavior.AllowGet);
        }

        public int GetNroCasos(string guid, long distribucion, double desde, double hasta)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/CalcularCasos/?guid=" + guid + "&distribucion=" + distribucion + "&desde=" + desde + "&hasta=" + hasta).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return (int)resp.Content.ReadAsAsync<int>().Result;
            }
            catch (Exception ex)
            {
                string msgErr = resp.ReasonPhrase;
                return 0;
            }
        }

        [HttpPost]
        public JsonResult GetIdConfigByIdPredefinido(long idPredefinido)
        {
            long? idConfig = 0;
            int? idCateg = 0;
            MT.Predefinido predefinido = GetPredefinidoById(idPredefinido);
            if (predefinido != null)
            {
                if (predefinido.ConfiguracionId != null)
                {
                    idConfig = predefinido.ConfiguracionId;
                    idCateg = predefinido.IdPlantillaCategoria;
                }
            }
            //return Json(idConfig, idCateg);
            return Json(new { idConfig = idConfig, idCateg = idCateg }, JsonRequestBehavior.AllowGet);
        }

        public MT.Predefinido GetPredefinidoById(long idPredefinido)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetPredefinidoById/" + idPredefinido).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<MT.Predefinido>().Result;
            }
            catch (Exception ex)
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }

        //Metodos para AJAX
        [HttpPost]
        public JsonResult Upload()
        {
            FicheroModel fichero = new FicheroModel();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    fichero.Nombre = fileName;

                    //var serverPath = Path.CombineServer.MapPath("~/files/" + item.FileName);
                    var path = Path.Combine(this.UploadPath, fileName);
                    fichero.Path = path;

                    //si existe en la carpeta destino lo borro.
                    if (new FileInfo(path).Exists)
                    {
                        new FileInfo(path).Delete();
                    }
                    //Grabo
                    file.SaveAs(path);
                    fichero.Tamanio = file.ContentLength;
                }
                else
                {
                    MvcApplication.GetLogger().LogError("El fichero esta Nulo", new Exception());
                }
            }
            return Json(fichero);
        }

        [HttpPost]
        public JsonResult GuardarColeccion(string nombre, MapaTematicoModel mapatematico)
        {
            if (!mapatematico.Visualizacion.Items.Any()) mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);

            string guid = Session["GUID"].ToStringOrDefault();
            MT.ObjetoResultadoDetalle objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            objetoResultadoDetalle.GUID = guid;
            objetoResultadoDetalle.IdUsuario = this.UsuarioConectado.Id_Usuario;
            objetoResultadoDetalle.ComponenteId = mapatematico.Componente.ComponenteId;
            mapatematico.Nombre = nombre;
            if (mapatematico.Nombre == null || mapatematico.Nombre == string.Empty)
            {
                mapatematico.Nombre = "Sin Titulo " + System.DateTime.Now;
            }
            objetoResultadoDetalle.NombreMT = mapatematico.Nombre;
            int cantUpd = ActualizarResultadoMapaTematico(objetoResultadoDetalle);

            int cantObj = GrabarColeccion(objetoResultadoDetalle);
            AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Alta, Autorizado.Si, Eventos.GrabarColeccionMT);
            return Json(cantObj);
        }

        [HttpPost]
        public JsonResult ActualizarResultadoMapaTematico()
        {
            MapaTematicoModel mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);

            string guid = Session["GUID"].ToStringOrDefault();
            MT.ObjetoResultadoDetalle objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            objetoResultadoDetalle.GUID = guid;
            objetoResultadoDetalle.IdUsuario = this.UsuarioConectado.Id_Usuario;
            objetoResultadoDetalle.ComponenteId = mapatematico.Componente.ComponenteId;
            if (mapatematico.Nombre == null || mapatematico.Nombre == string.Empty)
            {
                mapatematico.Nombre = "Sin Titulo " + System.DateTime.Now;
            }
            objetoResultadoDetalle.NombreMT = mapatematico.Nombre;
            int cantUpd = ActualizarResultadoMapaTematico(objetoResultadoDetalle);

            return Json(cantUpd);
        }

        [HttpPost]
        public JsonResult GuardarBiblioteca(string nombre, string descripcion)
        {
            int guardoOK = 0;
            MapaTematicoModel mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);
            string guid = Session["GUID"].ToStringOrDefault();
            var componente = GetComponentesById(mapatematico.Componente.ComponenteId);
            long id = mapatematico.MapaTematicoId ?? 0;
            mapatematico.Nombre = GetMapaTemanticoNombre(nombre, UsuarioConectado.Id_Usuario, id);
            mapatematico.Descripcion = descripcion;

            if (!string.IsNullOrEmpty(mapatematico.Nombre))
            {
                #region GrabarBiblioteca
                var mtc = new MapaTematicoConfiguracionModelo()
                {
                    ConfiguracionId = id,
                    Nombre = mapatematico.Nombre,
                    Descripcion = mapatematico.Descripcion,
                    idConfigCategoria = 1,
                    Rangos = mapatematico.Visualizacion.Rangos,
                    Atributo = mapatematico.ComponenteAtributo.Atributo.AtributoId ?? 0,
                    Externo = mapatematico.ComponenteAtributo.Atributo.EsImportado ? 1 : 0,
                    Agrupacion = mapatematico.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId,
                    Distribucion = mapatematico.Visualizacion.Distribucion,
                    Color = mapatematico.Visualizacion.Coloreado.ToString(), // contingencia como no se guarda el tipo de coloreado se utiliza el campo color en des uso para guardar ese dato.
                    Transparencia = mapatematico.Visualizacion.Transparencia,
                    ColorPrincipal = mapatematico.Visualizacion.ColorPrincipal,
                    ColorSecundario = mapatematico.Visualizacion.ColorSecundario,
                    ColorContorno = mapatematico.Visualizacion.ColorContorno,
                    CantidadContorno = mapatematico.Visualizacion.CantidadContorno,
                    Visibilidad = mapatematico.Visibilidad,
                    Usuario = UsuarioConectado.Id_Usuario,
                    FechaCreacion = mapatematico.FechaCreacion,
                    ComponenteConfiguracion = new[]
                                                {
                                                    new ComponenteConfiguracionModel()
                                                    {
                                                        ConfiguracionId = id,
                                                        ComponenteId = componente.ComponenteId
                                                    }
                                                }.ToList()
                };

                mtc.ConfiguracionFiltro = mapatematico.Filtros
                                            .Select(item => new MT.ConfiguracionFiltro()
                                            {
                                                ConfiguracionId = id,
                                                FiltroId = item.FiltroId,
                                                FiltroTipo = item.FiltroTipo,
                                                FiltroComponente = item.FiltroComponente,
                                                FiltroAtributo = item.FiltroAtributo,
                                                FiltroOperacion = item.FiltroOperacion,
                                                FiltroColeccion = item.FiltroColeccion,
                                                Valor1 = item.Valor1,
                                                Valor2 = item.Valor2,
                                                Ampliar = item.Ampliar,
                                                Dentro = item.Dentro,
                                                Tocando = item.Tocando,
                                                Fuera = item.Fuera,
                                                Habilitado = item.Habilitado ?? 0,
                                                PorcentajeInterseccion = item.PorcentajeInterseccion,
                                                ConfiguracionesFiltroGrafico = item.FiltroTipo == 2
                                                                                    ? (item.Coordenadas ?? string.Empty)
                                                                                            .Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                                                                                            .Select(coord => new MT.ConfiguracionFiltroGrafico()
                                                                                            {
                                                                                                FiltroId = item.FiltroId,
                                                                                                Coordenadas = coord,
                                                                                                sGeometry = coord
                                                                                            })
                                                                                            .ToList()
                                                                                    : new List<MT.ConfiguracionFiltroGrafico>()
                                            }).ToList();

                mtc.ConfiguracionRango = mapatematico.Visualizacion.Items
                                            .Select((item, idx) => new MT.ConfiguracionRango()
                                            {
                                                ConfiguracionId = id,
                                                Orden = idx,
                                                Desde = ((object)item.Desde ?? item.Valor ?? string.Empty).ToString(),
                                                Hasta = ((object)item.Hasta ?? string.Empty).ToString(),
                                                Leyenda = item.Leyenda ?? string.Empty,
                                                Cantidad = item.Casos,
                                                ColorRelleno = item.Color,
                                                ColorLinea = item.ColorBorde,
                                                EspesorLinea = item.AnchoBorde,
                                                Icono = item.Icono ?? item.GetSVGIcon((int)componente.Graficos)
                                            }).ToList();
                this.GrabarMapaTematico(mtc);
                #endregion
                guardoOK = 1;
            }
            AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, TipoOperacion.Alta, Autorizado.Si, Eventos.GrabarBibliotecaMT);
            return Json(guardoOK);
        }

        private string GetMapaTemanticoNombre(string nombre, long idUsuario, long? configuracionId)
        {
            string retNombre = nombre;
            if (!string.IsNullOrEmpty(nombre))
            {
                MapaTematicoConfiguracionModelo mapatematicoEncontrado = this.GetMapaTematicoByName(nombre);
                if (mapatematicoEncontrado != null)
                {
                    if (mapatematicoEncontrado.Usuario == idUsuario)
                    {
                        if (mapatematicoEncontrado.ConfiguracionId == configuracionId)
                        {
                            retNombre = nombre;
                        }
                        else
                        {
                            retNombre = string.Empty;
                        }
                    }
                }
                else
                {
                    retNombre = nombre;
                }
            }
            else
            {
                retNombre = "Sin Titulo " + System.DateTime.Now;
            }
            return retNombre;
        }

        public JsonResult leerFichero(string fileName, string componente)
        {
            FicheroModel fichero = new FicheroModel();
            string path = Path.Combine(UploadPath, fileName);
            if (new FileInfo(path).Exists)
            {
                string[] resultados = ProcesarArchivo(HttpUtility.UrlEncode(fileName), System.IO.File.ReadAllBytes(path)).Split('@');
                if (resultados.Length != 0)
                {
                    fichero.fileId = long.Parse(resultados.First());
                    if (fichero.fileId > 0)
                    {
                        var lineas = JsonConvert.DeserializeObject<List<string[]>>(resultados.Last());
                        List<string[]> coincidencias = calcularCoincidencia(long.Parse(componente), fichero.fileId);
                        fichero.Cabeceras = new List<string>(lineas[0]);
                        var filaUno = new List<string>(lineas[1]);
                        fichero.cantLineas = lineas.Count;
                        for (int i = 0; i < fichero.Cabeceras.Count; i++)
                        {
                            if (!comparaTipos(fichero.Cabeceras[i], filaUno[i]))
                            {
                                fichero.tieneCabecera = true;
                            }
                            fichero.TipoDato.Add(ParseString(filaUno[i]));
                            foreach (string[] coincidencia in coincidencias)
                            {
                                if (int.Parse(coincidencia[0]) == i)
                                {
                                    fichero.Coincidencia.Add(coincidencia[1]);
                                    fichero.Macheo.Add(coincidencia[2]);
                                }
                            }
                        }
                        AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, TipoOperacion.Alta, Autorizado.Si, Eventos.ImportarArchivo);
                    }
                    else
                    {
                        MvcApplication.GetLogger().LogError("Fichero tiene un formato incorrecto. Error al leer el fichero.", new Exception());
                    }
                }
                else
                {
                    MvcApplication.GetLogger().LogError("Fichero tiene un formato incorrecto.", new Exception());
                }
            }

            return Json(fichero);

        }

        public JsonResult GetAtributosCombo(string id)
        {
            return Json(new SelectList(GetAtributosByComponente(long.Parse(id)), "AtributoId", "Nombre"));
        }

        public JsonResult GetAtributosComboImport(string id, Usuario usuarioPortal)
        {
            MapaTematicoModel model = (MapaTematicoModel)Session["MapaTematico"];
            var usuarioId = 0;
            AtributoModel atributoSeleccionado = null;
            //if (usuarioPortal != null)
            //{
            //    usuarioId = usuarioPortal.Id;
            //}
            //TODO revisar nuevamente cuando el CU de usuarios este integrado
            List<AtributoModel> atributos = GetAtributosByComponente(long.Parse(id));
            List<DatoExternoConfiguracionModel> listaDEC = GetDatoExternoConfiguracionByUsuario(usuarioId);
            AtributoModel atributoimportado;
            foreach (var item in listaDEC)
            {
                atributoimportado = new AtributoModel();
                atributoimportado.EsImportado = true;
                atributoimportado.Nombre = item.Nombre;
                atributoimportado.AtributoId = item.DatoExternoConfiguracionId;

                atributos.Add(atributoimportado);
            }

            if (model.ComponenteAtributo != null && model.ComponenteAtributo.Atributo != null)
                atributoSeleccionado = atributos.Where(a => a.AtributoId == model.ComponenteAtributo.Atributo.AtributoId).FirstOrDefault();

            return Json(atributos.OrderBy(a => a.Nombre).ToList().MoveElementToFirstPosition(atributoSeleccionado));
        }

        public JsonResult GrabarAtributoExterno(long macheo, long columna, string descripcion, long fileid, long componenteId, string columnaMacheo, Usuario usuarioPortal)
        {
            //var idusuario = 0;
            //if (usuarioPortal != null)
            //{
            //    idusuario = usuarioPortal.Id;
            //}
            //TODO revisar nuevamente cuando el CU de usuarios este integrado
            MT.DatoExternoConfiguracion dec = GrabarDatoExterno(macheo, columna, descripcion, fileid, componenteId, columnaMacheo, this.UsuarioConectado.Id_Usuario);
            return Json(dec);
        }

        public JsonResult GetAtributosByComp(string id)
        {
            return Json(GetAtributosByComponente(long.Parse(id)).OrderBy(a => a.Nombre));
        }
        public JsonResult SearchComponentes(string texto)//No se usa
        {
            return Json(SearchComponente(texto));
        }
        public JsonResult GetOperacionesCombo(string id)
        {
            AtributoModel atributoModel = GetAtributosById(long.Parse(id));
            if (atributoModel.EsValorFijo == true)
            {
                return Json(new
                {
                    habilitaSeleccionMultiple = true,
                    lista = GetOperacionesEspeciales(atributoModel.AtributoParentId ?? atributoModel.AtributoId)
                });
            }
            else
            {
                List<TipoOperacionModel> lstTipoOperacionModel = GetOperaciones();
                if (atributoModel.EsObligatorio)
                {
                    //Sacar de la lista vacio y no vacio
                    //14 - Vacio
                    //15 - No Vacio
                    TipoOperacionModel tpVacio = lstTipoOperacionModel.Where(p => p.TipoOperacionId == 14).ToList()[0];
                    TipoOperacionModel tpNoVacio = lstTipoOperacionModel.Where(p => p.TipoOperacionId == 15).ToList()[0];
                    lstTipoOperacionModel.Remove(tpVacio);
                    lstTipoOperacionModel.Remove(tpNoVacio);
                }
                return Json(new { lista = lstTipoOperacionModel });
            }
        }
        public JsonResult GetAgrupacionesCombo(string id)
        {
            return Json(new SelectList(GetAgrupaciones(id), "AgrupacionId", "Nombre"));
        }

        public JsonResult EliminarMT(string id)
        {
            this.DeleteMapaTematicoById(long.Parse(id));
            //RegisterAuditoria(Request, UsuarioConectado.Id_Usuario, Seguridad.EliminarBibliotecaMT, Eventos.EliminarBibliotecaMT, "Mapa Temático eliminado");
            return Json(id);

        }
        public JsonResult CambiarVisibilidad(string id)
        {
            return Json(this.CambiarVisibilidadMapaTematicoById(long.Parse(id)));
        }

        public JsonResult GetPlantillaById(long idPlantilla)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/Plantilla/Get/" + idPlantilla).Result;
            resp.EnsureSuccessStatusCode();
            Plantilla plantilla = resp.Content.ReadAsAsync<Plantilla>().Result;

            return Json(new { Transparencia = plantilla.PPP }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComponentesPrincipalesByPlantilla(long idPlantilla)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/Plantilla/Get/" + idPlantilla).Result;
            resp.EnsureSuccessStatusCode();
            Plantilla plantilla = resp.Content.ReadAsAsync<Plantilla>().Result;

            List<Layer> lstLayerPrincipal = plantilla.Layers.Where(l => l.Categoria == 1).ToList();
            SelectList layersPcipales = new SelectList(lstLayerPrincipal, "ComponenteId", "Nombre");

            string orientacion = (plantilla.Orientacion == 0 ? "Horizontal" : "Vertical");
            string imagenNombre = plantilla.PlantillaFondos.First().ImagenNombre;

            string componentePrincipal = String.Join(", ", lstLayerPrincipal.Select(x => x.Nombre));
            return Json(new { Transparencia = plantilla.PPP, layers = layersPcipales, HojaNombre = plantilla.Hoja.Nombre, Orientacion = orientacion, ComponentePrincipal = componentePrincipal, ImagenNombre = imagenNombre }, JsonRequestBehavior.AllowGet);
        }

        //METODOS DE ACCESO A LA API ↓
        public MT.DatoExternoConfiguracion GrabarDatoExterno(long macheo, long columna, string descripcion, long fileid, long componenteId, string columnaMacheo, long usuarioId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GrabarDatoExterno/?macheo=" + macheo + "&columna=" + columna + "&descripcion=" + descripcion + "&fileid=" + fileid + "&componenteId=" + componenteId + "&columnaMacheo=" + columnaMacheo + "&usuarioId=" + usuarioId).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<MT.DatoExternoConfiguracion>().Result;
        }
        public void BorrarDatoExterno(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/BorrarDatoExterno/" + id).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void BorrarDatoExternoByUsuario(long idUsuario)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/BorrarDatoExternoByUsuario/" + idUsuario).Result;
            resp.EnsureSuccessStatusCode();
        }
        public DatoExternoConfiguracionModel GetDatoExternoConfiguracionById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetDatoExternoConfiguracionById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (DatoExternoConfiguracionModel)resp.Content.ReadAsAsync<DatoExternoConfiguracionModel>().Result;
        }
        public List<DatoExternoConfiguracionModel> GetDatoExternoConfiguracionByUsuario(long idUsuario)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetDatoExternoConfiguracionByUsuario/" + idUsuario).Result;
            resp.EnsureSuccessStatusCode();
            return (List<DatoExternoConfiguracionModel>)resp.Content.ReadAsAsync<List<DatoExternoConfiguracionModel>>().Result;
        }
        public MT.ComponenteConfiguracion GetComponenteConfiguracionByMTId(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponenteConfiguracionByMTId/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<MT.ComponenteConfiguracion>().Result;
        }
        public List<ComponenteModel> GetComponentesByBiblioteca(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponentesByBiblioteca/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<ComponenteModel>>().Result;
        }
        public string DeleteMapaTematicoById(long id)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/MapasTematicosService/DeleteMapaTematicoById?id=" + id, id).Result;
            resp.EnsureSuccessStatusCode();
            return (String)resp.Content.ReadAsAsync<String>().Result;
        }

        public int CambiarVisibilidadMapaTematicoById(long id)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/MapasTematicosService/CambiarVisibilidadMapaTematicoById?id=" + id, id).Result;
            resp.EnsureSuccessStatusCode();
            //var result = resp.Content.ReadAsAsync<int>().Result;
            //string funcion = result == "0" ? Seguridad.DespublicarBibliotecaMT : Seguridad.PublicarBibliotecaMT;
            //string evento = result == "0" ? Eventos.DespublicarBibliotecaMT : Eventos.PublicarBibliotecaMT;
            //RegisterAuditoria(Request, UsuarioConectado.Id_Usuario, funcion, evento, "Cambiar Visibilidad Mapa Temático");
            return resp.Content.ReadAsAsync<int>().Result;
        }

        public List<MT.ConfiguracionFiltro> ConfiguracionesFiltroByMT(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/ConfiguracionFiltroByMT/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<MT.ConfiguracionFiltro>>().Result;
        }

        public List<MT.ConfiguracionRango> ConfiguracionesRangoByMT(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/ConfiguracionesRangoByMT/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<MT.ConfiguracionRango>>().Result;
        }
        public MapaTematicoConfiguracionModelo GetMapaTematicoByName(string nombre)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetMapaTematicoByName/?nombre=" + HttpUtility.UrlEncode(nombre)).Result;
                try
                {
                    resp.EnsureSuccessStatusCode();
                    return (MapaTematicoConfiguracionModelo)resp.Content.ReadAsAsync<MapaTematicoConfiguracionModelo>().Result;
                }
                catch
                {
                    string msgErr = resp.ReasonPhrase;
                    return null;
                }
            }
            catch (Exception ex)
            {
                string msgErr = ex.Message;
                return null;
            }
        }
        public MapaTematicoConfiguracionModelo GetMapaTematicoById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetMapaTematicoById/" + id).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return (MapaTematicoConfiguracionModelo)resp.Content.ReadAsAsync<MapaTematicoConfiguracionModelo>().Result;
            }
            catch (Exception ex)
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }
        public bool CopyMapaTematico(MapaTematicoConfiguracionModelo mtm)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync<MapaTematicoConfiguracionModelo>("api/MapasTematicosService/CopyMapaTematicoById", mtm).Result;
            try
            {
                resp.EnsureSuccessStatusCode();

                return true;
            }
            catch
            {
                string msgErr = resp.ReasonPhrase;
                return false;
            }
        }
        public List<ComponenteConfiguracionModel> GetMapaTematicosPublicos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetMapaTematicosPublicos").Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return (List<ComponenteConfiguracionModel>)resp.Content.ReadAsAsync<List<ComponenteConfiguracionModel>>().Result;
            }
            catch
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }

        public List<ComponenteConfiguracionModel> GetBibliotecasPrivadasByUsuario(long IdUsuario)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetBibliotecasPrivadasByUsuario/" + IdUsuario).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<ComponenteConfiguracionModel>>().Result;
            }
            catch
            {
                return null;
            }
        }
        public List<string[]> calcularCoincidencia(long componenteId, long fileId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/calcularCoincidencia/?componenteId=" + componenteId + "&fileId=" + fileId).Result;
            resp.EnsureSuccessStatusCode();
            return (List<string[]>)resp.Content.ReadAsAsync<List<string[]>>().Result;
        }
        public string ProcesarArchivo(string fileName, byte[] fileContent)
        {
            try
            {
                var content = new ByteArrayContent(fileContent);
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                var waiter = cliente.PostAsync("api/MapasTematicosService/procesarArchivo?fileName=" + fileName, content);
                waiter.Wait(System.Threading.Timeout.Infinite, CancellationToken.None);
                var resp = waiter.Result;
                try
                {
                    resp.EnsureSuccessStatusCode();
                    return resp.Content.ReadAsAsync<string>().Result;
                }
                catch (Exception ex)
                {
                    MvcApplication.GetLogger().LogError("ProcesarArchivo", ex);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("ProcesarArchivo", ex);
            }
            return string.Empty;
        }
        public List<ComponenteModel> GetComponentes()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponentes").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }
        public List<ComponenteModel> GetComponentesGeograficos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponentesGeograficos").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }

        public ComponenteModel GetComponentesById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponentesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ComponenteModel)resp.Content.ReadAsAsync<ComponenteModel>().Result;
        }
        public ComponenteModel GetComponenteByCapa(string layer)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponenteByCapa/?layer=" + layer).Result;
            resp.EnsureSuccessStatusCode();
            return (ComponenteModel)resp.Content.ReadAsAsync<ComponenteModel>().Result;
        }
        public AtributoModel GetAtributosById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetAtributosById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (AtributoModel)resp.Content.ReadAsAsync<AtributoModel>().Result;
        }
        public AgrupacionModel GetAgrupacionesById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetAgrupacionesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (AgrupacionModel)resp.Content.ReadAsAsync<AgrupacionModel>().Result;
        }
        public List<ComponenteModel> SearchComponente(string texto)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponentesBusqueda/" + texto).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }

        public List<ComponenteModel> GetComponentesByPadre(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponentesByPadre/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }

        public List<ComponenteModel> GetComponentesRelacionados(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetComponentesRelacionados/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }
        public List<AtributoModel> GetAtributosByComponente(long id)
        {

            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetAtributosByComponente/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<AtributoModel>)resp.Content.ReadAsAsync<IEnumerable<AtributoModel>>().Result;
        }

        public List<TipoOperacionModel> GetOperaciones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetOperaciones").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoOperacionModel>)resp.Content.ReadAsAsync<IEnumerable<TipoOperacionModel>>().Result;
        }
        public List<TipoOperacionModel> GetOperacionesEspeciales(long? id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetOperacionesEspeciales/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoOperacionModel>)resp.Content.ReadAsAsync<IEnumerable<TipoOperacionModel>>().Result;
        }
        public TipoOperacionModel GetOperacionesById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetOperacionesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (TipoOperacionModel)resp.Content.ReadAsAsync<TipoOperacionModel>().Result;
        }
        public List<AgrupacionModel> GetAgrupaciones(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetAgrupacionesByAtributos/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<AgrupacionModel>)resp.Content.ReadAsAsync<IEnumerable<AgrupacionModel>>().Result;
        }
        public ColeccionModel GetColeccionesById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetColeccionesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ColeccionModel)resp.Content.ReadAsAsync<ColeccionModel>().Result;
        }
        public List<ColeccionModel> GetColecciones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetColeccionesCombo").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ColeccionModel>)resp.Content.ReadAsAsync<IEnumerable<ColeccionModel>>().Result;
        }
        public List<ColeccionModel> GetColeccionesByComponente(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync(string.Format("api/MapasTematicosService/GetColeccionesByComponenteUsuario?componente={0}&usuario={1}", id, UsuarioConectado.Id_Usuario)).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ColeccionModel>)resp.Content.ReadAsAsync<IEnumerable<ColeccionModel>>().Result;
        }

        public List<Plantilla> GetPlantillasForMapaTematico()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetPlantillasForMapaTematico/").Result;
            resp.EnsureSuccessStatusCode();
            return (List<Plantilla>)resp.Content.ReadAsAsync<IEnumerable<Plantilla>>().Result;
        }

        public void GrabarMapaTematico(MapaTematicoConfiguracionModelo mtc)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/MapasTematicosService/GrabaMapaTematico", mtc).Result;
            resp.EnsureSuccessStatusCode();
            mtc.ConfiguracionId = resp.Content.ReadAsAsync<long>().Result;
            return;
        }

        public int GrabarColeccion(MT.ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            using (var resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/MapasTematicosService/GrabarColeccion", objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result)
            {
                try
                {
                    resp.EnsureSuccessStatusCode();
                    return resp.Content.ReadAsAsync<int>().Result;
                }
                catch (HttpRequestException)
                {
                    MvcApplication.GetLogger().LogInfo($"MapasTematicos/GrabarColeccion => {resp.ReasonPhrase}");
                    throw;
                }
                catch (Exception ex)
                {
                    MvcApplication.GetLogger().LogError("MapasTematicos/GrabarColeccion", ex);
                    return 0;
                }
            }
        }

        public MT.ObjetoResultadoDetalle GenerarResultadoMapaTematico(string guid, MapaTematicoModel mapaTematicoModel, long configuracionId)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            var requestObjetoResultado = new MT.RequestObjetoResultado()
            {
                IdUsuario = usuario.Id_Usuario,
                TokenSesion = usuario.Token,
                GUID = guid,
                Componente = new MT.Componente()
                {
                    ComponenteId = mapaTematicoModel.Componente.ComponenteId,
                    Esquema = mapaTematicoModel.Componente.Esquema,
                    Tabla = mapaTematicoModel.Componente.Tabla
                },
                Atributo = new MT.Atributo()
                {
                    ComponenteId = mapaTematicoModel.ComponenteAtributo.ComponenteId,
                    AtributoId = (mapaTematicoModel.ComponenteAtributo.Atributo.AtributoId != null ? (long)mapaTematicoModel.ComponenteAtributo.Atributo.AtributoId : 0)
                },
                MapaTematicoConfiguracion = new MT.MapaTematicoConfiguracion()
                {
                    ConfiguracionId = Math.Max(configuracionId, 0),
                    Agrupacion = mapaTematicoModel.ComponenteAtributo.Atributo.Agrupacion.AgrupacionId,
                    Distribucion = mapaTematicoModel.Visualizacion.Distribucion,
                    Rangos = mapaTematicoModel.Visualizacion.Rangos
                }
            };

            if (mapaTematicoModel.Externo == 1)
            {
                requestObjetoResultado.EsImportado = true;
            }
            else
            {
                requestObjetoResultado.EsImportado = mapaTematicoModel.ComponenteAtributo.Atributo.EsImportado;
            }

            requestObjetoResultado.ConfiguracionFiltros = (mapaTematicoModel.Filtros ?? new List<FiltroModel>())
                                                                .Select(item =>
                                                                {
                                                                    return new MT.ConfiguracionFiltro()
                                                                    {
                                                                        FiltroId = item.FiltroId,
                                                                        FiltroTipo = item.FiltroTipo,
                                                                        FiltroComponente = item.FiltroComponente,
                                                                        FiltroAtributo = item.FiltroAtributo,
                                                                        FiltroOperacion = item.FiltroOperacion,
                                                                        FiltroColeccion = item.FiltroColeccion,
                                                                        Valor1 = item.Valor1,
                                                                        Valor2 = item.Valor2,
                                                                        Ampliar = item.Ampliar,
                                                                        Dentro = item.Dentro,
                                                                        Tocando = item.Tocando,
                                                                        Fuera = item.Fuera,
                                                                        Habilitado = item.Habilitado.GetValueOrDefault() == 0 ? 0 : 1,
                                                                        PorcentajeInterseccion = item.PorcentajeInterseccion,
                                                                        ConfiguracionesFiltroGrafico = (item.Coordenadas ?? string.Empty).Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                                                                                                           .Select(coord => new MT.ConfiguracionFiltroGrafico()
                                                                                                           {
                                                                                                               Coordenadas = coord,
                                                                                                               sGeometry = coord
                                                                                                           })
                                                                                                           .ToList()
                                                                    };
                                                                }).ToList();
            try
            {
                var resp = cliente.PostAsync("api/MapasTematicosService/GenerarResultadoMapaTematico", requestObjetoResultado, new JsonMediaTypeFormatter()).Result;
                try
                {
                    resp.EnsureSuccessStatusCode();
                    return resp.Content.ReadAsAsync<MT.ObjetoResultadoDetalle>().Result;
                }
                catch (HttpRequestException ex)
                {
                    MvcApplication.GetLogger().LogError("MapasTematicos - GenerarResultadoMapaTematico", ex);
                    MvcApplication.GetLogger().LogError("MapasTematicos - GenerarResultadoMapaTematico", new Exception(resp.ReasonPhrase));
                    throw;
                }
            }
            catch (AggregateException toEx)
            {
                MvcApplication.GetLogger().LogError("MapasTematicos - GenerarResultadoMapaTematico TimeOut",
                    new Exception(string.Format("Timeout en mapaTematico\nGUID: {0}\nUsuario: {1}\n", guid, UsuarioConectado.Login), toEx));
                MvcApplication.GetLogger().LogInfo(string.Format("Timeout en mapaTematico\nGUID: {5}\nComponente: {0}-{1}\nAtributo: {2}-{3}\nFiltro: {4}",
                    mapaTematicoModel.Componente.ComponenteId, mapaTematicoModel.Componente.Nombre,
                    mapaTematicoModel.ComponenteAtributo.Atributo.AtributoId, mapaTematicoModel.ComponenteAtributo.Atributo.Nombre,
                    JsonConvert.SerializeObject(requestObjetoResultado.ConfiguracionFiltros, Newtonsoft.Json.Formatting.Indented), guid));
                throw new TimeoutException("La consulta ha tardado más de lo permitido.");
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MapasTematicos - GenerarResultadoMapaTematico", ex);
                throw;
            }
        }

        public MT.ObjetoResultadoDetalle GetObjetoResultadoDetalle(string guid, long distribucion, long cantRangos)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetObjetoResultadoDetalle/?guid=" + guid + "&distribucion=" + distribucion + "&cantRangos=" + cantRangos).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<MT.ObjetoResultadoDetalle>().Result;
        }

        public int ActualizarResultadoMapaTematico(MT.ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            HttpResponseMessage resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/MapasTematicosService/ActualizarResultadoMapaTematico", objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<int>().Result;
        }

        public int ActualizarResultadoMapaTematicoPasarAPunto(string guid)
        {
            HttpResponseMessage resp = cliente.PostAsync("api/MapasTematicosService/ActualizarResultadoMapaTematicoPasarAPunto?guid=" + guid, null).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<int>().Result;
        }

        [HttpPost]
        public JsonResult GetMapaTematicoExtents()
        {
            string guid = Session["GUID"].ToStringOrDefault();
            string parametros = string.Format("guid={0}", guid);
            HttpResponseMessage resp = cliente.PostAsync("api/MapasTematicosService/GetMapaTematicoExtents?" + parametros, null).Result;
            resp.EnsureSuccessStatusCode();
            JsonResult jsonResult = new JsonResult
            {
                Data = (string)resp.Content.ReadAsAsync<string>().Result
            };
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetMapaTematicoExtentsForModPlot()
        {
            string guid = Session["GUID"].ToStringOrDefault();
            string extent = GetMapaTematicoExtentsForModPlot(guid);
            JsonResult jsonResult = new JsonResult
            {
                Data = extent
            };
            return jsonResult;
        }

        internal string GetMapaTematicoExtentsForModPlot(string guid)
        {
            string parametros = string.Format("guid={0}", guid);
            HttpResponseMessage resp = cliente.PostAsync("api/MapasTematicosService/GetMapaTematicoExtentsForModPlot?" + parametros, null).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<string>().Result;
        }

        [HttpGet]
        public FileResult BuscarAyuda()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/BuscarAyuda").Result;
            resp.EnsureSuccessStatusCode();
            return File(Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result), "application/pdf", "MapasTematicosAyuda.pdf");
        }

        [HttpGet]
        public FileResult ExportarExcelResMT(string parametros)
        {
            string idPredefinido = string.Empty;
            string nombreReporte = string.Empty;
            MapaTematicoModel mapatematico = ((MapaTematicoModel)Session["MapaTematico"]);
            if (parametros != null)
            {
                var obj = new { Colores = new string[0], ColoresBorde = new string[0], AnchosBorde = new string[0], IdPredefinido = string.Empty, NombreReporte = string.Empty };
                var objParams = JsonConvert.DeserializeAnonymousType(parametros, obj);

                idPredefinido = objParams.IdPredefinido;
                nombreReporte = objParams.NombreReporte;

                int iColor = 0;
                foreach (var visualiz in mapatematico.Visualizacion.Items)
                {
                    visualiz.Color = objParams.Colores[iColor].Replace('-', '0');
                    visualiz.ColorBorde = objParams.ColoresBorde[iColor];
                    visualiz.AnchoBorde = (objParams.AnchosBorde[iColor] != string.Empty ? Convert.ToInt32(objParams.AnchosBorde[iColor]) : 0);
                    iColor++;
                }
            }
            string guid = Session["GUID"].ToStringOrDefault();
            MT.ObjetoResultadoDetalle objetoResultadoDetalle = GetObjetoResultadoDetalle(mapatematico.Visualizacion);
            objetoResultadoDetalle.ComponenteId = mapatematico.Componente.ComponenteId;
            objetoResultadoDetalle.GUID = guid;
            HttpResponseMessage resp = null;
            if (idPredefinido == "4")
            {
                //Todos los medidores
                resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/MapasTematicosService/ExportarExcelResultadoMTPredefinido_4?idPredefinido=" + idPredefinido, objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            }
            else if (idPredefinido == "5")
            {
                //Medidores diferenciados por marca
                resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/MapasTematicosService/ExportarExcelResultadoMTPredefinido_5?idPredefinido=" + idPredefinido, objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            }
            else if (idPredefinido == "7")
            {
                //Medidores diferenciados por marca
                resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/MapasTematicosService/ExportarExcelResultadoMTPredefinido_7?idPredefinido=" + idPredefinido, objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            }
            else
            {
                resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/MapasTematicosService/ExportarExcelResultadoMapaTematico?idPredefinido=" + idPredefinido, objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            }
            try
            {
                resp.EnsureSuccessStatusCode();
                var base64 = resp.Content.ReadAsAsync<string>().Result;
                var bytes = Convert.FromBase64String(base64);
                if (nombreReporte == string.Empty)
                {
                    nombreReporte = "ResultadoMapasTematicos.xlsx";
                }
                else
                {
                    nombreReporte += ".xlsx";
                }
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);

            }
            catch
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }

        public List<ParametrosGeneralesModel> GetParametrosGenerales()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetParametrosGenerales").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ParametrosGeneralesModel>)resp.Content.ReadAsAsync<IEnumerable<ParametrosGeneralesModel>>().Result;
        }

        public MT.Predefinido GetPredefinidoByConfiguracionId(long configuracionId, int categoriaId)
        {
            //HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetPredefinidoByConfiguracionId?configuracionId=" + configuracionId).Result;
            HttpResponseMessage resp = cliente.GetAsync(string.Format("api/MapasTematicosService/GetPredefinidoByConfiguracionId?configuracionId={0}&categoriaId={1}", configuracionId, categoriaId)).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<MT.Predefinido>().Result;
            }
            catch (Exception ex)
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }

        public Plantilla GetPlantillaByIdPlantillaCategoria(int idPlantillaCategoria)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MapasTematicosService/GetPlantillaByIdPlantillaCategoria?idPlantillaCategoria=" + idPlantillaCategoria).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<Plantilla>().Result;
            }
            catch (Exception ex)
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }

        //METODOS DE ACCESO A LA API ↑


        // Metodos Helper 
        private string ParseString(string str)
        {
            if (bool.TryParse(str, out bool _))
                return "Binario";
            else if (int.TryParse(str, out int _) ||
                     long.TryParse(str, out long _) ||
                     double.TryParse(str, out double _))
                return "Numerico";
            else if (DateTime.TryParse(str, out DateTime _))
                return "Fecha";
            else return "Texto";

        }
        private bool comparaTipos(string str1, string str2)
        {
            return bool.TryParse(str1, out bool _) && bool.TryParse(str2, out bool _) ||
                   int.TryParse(str1, out int _) && int.TryParse(str2, out int _) ||
                   long.TryParse(str1, out long _) && long.TryParse(str2, out long _) ||
                   double.TryParse(str1, out double _) && double.TryParse(str2, out double _) ||
                   DateTime.TryParse(str1, out DateTime _) && DateTime.TryParse(str2, out DateTime _);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
        }

        private void RegisterAuditoria(HttpRequestBase request, long idUsuario, string funcionStr, string eventoNombre, string desc)
        {
            SeguridadController sc = new SeguridadController();
            sc.RegisterAuditoria(request, idUsuario, funcionStr, eventoNombre, desc);
        }

        private List<ComponenteModel> GetListaPrimeroSeleccionado(MapaTematicoModel model)
        {
            List<ComponenteModel> listaComponentes = GetComponentesGeograficos();
            if (model != null)
            {
                ComponenteModel seleccionado = listaComponentes.Where(c => c.ComponenteId == model.Componente.ComponenteId).FirstOrDefault();
                return listaComponentes.MoveElementToFirstPosition<ComponenteModel>(seleccionado);
            }

            return listaComponentes;
        }

        private MT.ObjetoResultadoDetalle GetObjetoDetallesConRangos(MT.ObjetoResultadoDetalle objetoResultadoDetalle)
        {
            HttpResponseMessage resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>(string.Format("api/MapasTematicosService/GenerarResultadoMapaTematicoUbicaciones?userId={0}&guid={1}", UsuarioConectado.Id_Usuario, Session["GUID"].ToString()), objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return (MT.ObjetoResultadoDetalle)resp.Content.ReadAsAsync<MT.ObjetoResultadoDetalle>().Result;
        }
    }
}