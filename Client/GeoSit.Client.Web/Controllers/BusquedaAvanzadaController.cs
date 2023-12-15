using System;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Net.Http.Formatting;
using MT = GeoSit.Data.BusinessEntities.MapasTematicos;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using GeoSit.Client.Web.Helpers.ExtensionMethods;
using GeoSit.Client.Web.Helpers;
using System.Net;
using Resources;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Runtime.Remoting.Messaging;
using Elmah.ContentSyndication;

namespace GeoSit.Client.Web.Controllers
{
    public class BusquedaAvanzadaController : Controller
    {
        // GET: BusquedaAvanzada
        private UsuariosModel UsuarioConectado { get { return (UsuariosModel)Session["usuarioPortal"]; } }

        private HttpClient cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials });

        private readonly string UploadPath = ConfigurationManager.AppSettings["UploadPath"];//No se usa, borrar

        public BusquedaAvanzadaController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            cliente.Timeout = new TimeSpan(0, 10, 0);
        }

        // GET: /MapasTematicos/Index
        public ActionResult Index()
        {
            BusquedaAvanzadaModel model = new BusquedaAvanzadaModel();
            Session["BusquedaAvanzada"] = model;

            ViewBag.listaComponenetes = GetComponentes();
            //ViewBag.listaComponenetes = GetComponentesById(56);
            return PartialView("Componentes", model);
        }

        public ActionResult GetIndexView(BusquedaAvanzadaModel model)
        {
            if (Session["BusquedaAvanzada"] == null)
            {
                Session["BusquedaAvanzada"] = new BusquedaAvanzadaModel();
            }
            else
            {
                model = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];
            }
            ViewBag.listaComponenetes = GetListaPrimeroSeleccionado(model);
            return PartialView("Componentes", model);
        }

        public ActionResult GetAgrupamientoView(string ids)
        {
            BusquedaAvanzadaModel model = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];

            if (!string.IsNullOrEmpty(ids)) //no viene de "volver"
            {
                model.Componentelist.Clear();
                model.Componentelist.AddRange(ids.Split(',').Select(id => GetComponentesById(Convert.ToInt64(id))));
                model.ComponentesAgrupadores = GetListaPrimeroSeleccionadoAgrupadores(model);
            }
            else
            {
                var elementoSeleccionado = model.ComponentesAgrupadores.Where(c => c.ComponenteId == model.AgrupIdComponente).FirstOrDefault();
                model.ComponentesAgrupadores = model.ComponentesAgrupadores.OrderBy(c => c.Nombre).ToList().MoveElementToFirstPosition(elementoSeleccionado);
            }

            ViewBag.ops = new List<string>(new string[] { "Dentro", "Tocando" });
            ViewBag.Agrupamientos = new string[2] { "Sin Agrupamiento", "Con Agrupamiento" };

            return PartialView("Agrupamiento", model);
        }

        public ActionResult GetFiltrosView(string agrupamiento, string attrAgrup, string operaciones)
        {
            var model = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];
            if (!string.IsNullOrEmpty(agrupamiento))
            {
                model.OperacionAgrup = string.Empty;
                model.AgrupIdComponente = 0;
                model.Agrupamiento = Convert.ToInt32(agrupamiento);
                if (model.Agrupamiento != 0)
                {
                    model.OperacionAgrup = operaciones;
                    model.AgrupIdComponente = Convert.ToInt32(attrAgrup);
                }
            }
            if (model.Filtros != null)
            {
                foreach (var item in model.Filtros)
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
                    //Creo no hace falta agregar esta parte para filtros de condicion(and,or...)
                }
            }

            if (model.Componentelist.Count == 1)
            {
                ViewBag.Componentes = GetComponentesRelacionados(model.Componentelist.Single().ComponenteId);
            }
            else
            {
                ViewBag.Componentes = model.Componentelist;
            }

            ViewBag.ComponentesGraficos = GetComponentes();
            ViewBag.Colecciones = GetColeccionesSoloUnComponente().ToList();
            Session["BusquedaAvanzada"] = model;
            return PartialView("Filtros", model);
        }

        [ValidateAntiForgeryToken]
        public ActionResult GetResultadoView(BusquedaAvanzadaModel model)
        {
            var modelsession = new BusquedaAvanzadaModel();
            modelsession = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];

            var busquedaavanzada = ((BusquedaAvanzadaModel)Session["BusquedaAvanzada"]);
            var generarResultado = (int)Session["GenerarResultado"];
            if (generarResultado == 1)
            {
                if (busquedaavanzada != null)
                {
                    if (busquedaavanzada.Componente.ComponenteId > 0)
                    {
                        busquedaavanzada.Componente = GetComponentesById(busquedaavanzada.Componente.ComponenteId);
                    }

                    if (model.Filtros == null)
                        model.Filtros = new List<FiltroModel>();
                    if (busquedaavanzada.Filtros == null)
                        busquedaavanzada.Filtros = new List<FiltroModel>();

                    try
                    {
                        if (model.Filtros.Count > 0)
                        {
                            busquedaavanzada.cantFiltrosAtributo = model.cantFiltrosAtributo;
                            busquedaavanzada.cantFiltrosColeccion = model.cantFiltrosColeccion;
                            busquedaavanzada.cantFiltrosGeografico = model.cantFiltrosGeografico;
                            busquedaavanzada.Filtros = model.Filtros;

                        }
                    }
                    catch (Exception ex)
                    {
                        //Posible error. Se soluciona con try - cath (?
                    }

                    if (busquedaavanzada.Filtros != null)
                    {
                        foreach (var filtroModel in busquedaavanzada.Filtros)
                        {
                            if (filtroModel.FiltroTipo == 2 && filtroModel.Coordenadas != null && filtroModel.Coordenadas != string.Empty && filtroModel.Coordenadas.Substring(0, 1) == "G")
                            {
                                string filtroGrafico = filtroModel.Coordenadas.Substring(1, filtroModel.Coordenadas.Length - 1);
                                string[] aObjetoSeleccionado = filtroGrafico.Split('+');
                                if (aObjetoSeleccionado != null && aObjetoSeleccionado.Length > 0)
                                {
                                    List<string> lstObjetoSeleccionadoDatos = aObjetoSeleccionado[0].Split(';').ToList();

                                    ComponenteModel componenteGraf = GetComponenteByCapa(lstObjetoSeleccionadoDatos[0]);
                                    filtroModel.FiltroComponente = componenteGraf.ComponenteId;
                                    AtributoModel atributoGraf = GetAtributoFEATIDByComponente(componenteGraf.ComponenteId);
                                    filtroModel.FiltroAtributo = atributoGraf.AtributoId;
                                    TipoOperacionModel tipoOperacionGraf = GetOperacionesById(1);
                                    filtroModel.FiltroOperacion = tipoOperacionGraf.TipoOperacionId;
                                    filtroModel.FiltroOperacionDesc = tipoOperacionGraf.Nombre;
                                    filtroModel.Valor1 = lstObjetoSeleccionadoDatos[1];
                                }
                                filtroModel.Coordenadas = null;
                            }
                        }
                    }
                }
            }
            busquedaavanzada.lstConfiguracionFiltro = busquedaavanzada.Filtros?
                                            .Select(filtro => new BusquedaAvanzadaModel.ConfiguracionFiltro()
                                            {
                                                FiltroComponente = filtro.FiltroComponente,
                                                FiltroAtributo = filtro.FiltroAtributo,
                                                FiltroColeccion = filtro.FiltroColeccion,
                                                FiltroOperacion = filtro.FiltroOperacion ?? 16,
                                                FiltroTipo = filtro.FiltroTipo,
                                                Valor1 = filtro.Valor1,
                                                Valor2 = filtro.Valor2,
                                                Ampliar = filtro.Ampliar,
                                                PorcentajeInterseccion = filtro.PorcentajeInterseccion,
                                                Dentro = filtro.Dentro,
                                                Tocando = filtro.Tocando,
                                                Fuera = filtro.Fuera,
                                                Habilitado = filtro.Habilitado.GetValueOrDefault(),
                                                ConfiguracionesFiltroGrafico = (filtro.Coordenadas ?? string.Empty)
                                                                                            .Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                                                                                            .Select(coord => new BusquedaAvanzadaModel.ConfiguracionFiltroGrafico()
                                                                                            {
                                                                                                Coordenadas = coord,
                                                                                                sGeometry = coord
                                                                                            })
                                                                                            .ToList()
                                            }
                                            ).ToList()
                                            ?? new List<BusquedaAvanzadaModel.ConfiguracionFiltro>();
            if (busquedaavanzada.AgrupIdComponente != 0)
            {
                busquedaavanzada.Componente = GetComponentesById(busquedaavanzada.AgrupIdComponente);
            }

            try
            {
                busquedaavanzada.ListaResultados = getResultadoBusquedaAvanzada(busquedaavanzada);
                //AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Consulta, Autorizado.Si, Eventos.GeneracionResultadosEnBA);
                Session["BusquedaAvanzada"] = busquedaavanzada;
                int cantResultado = busquedaavanzada.ListaResultados.Sum(lista => lista.Count);
                if (cantResultado > 0)
                {
                    ViewBag.ItemsCount = cantResultado;
                    return PartialView("Visualizacion", busquedaavanzada);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        private AtributoModel GetAtributoFEATIDByComponente(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetAtributoFEATIDByComponente/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (AtributoModel)resp.Content.ReadAsAsync<AtributoModel>().Result;
        }

        public ComponenteModel GetComponenteByCapa(string layer)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponenteByCapa/?layer=" + layer).Result;
            resp.EnsureSuccessStatusCode();
            return (ComponenteModel)resp.Content.ReadAsAsync<ComponenteModel>().Result;
        }

        [HttpGet]
        public ActionResult GetResumenView_Guardado(long ConfiguracionId)
        {
            var modelo = this.GetMapaTematicoById(ConfiguracionId);
            var componentes = this.GetComponentesByBiblioteca(ConfiguracionId);
            // si es mapa tematico siempre tendra un solo componente , pero se obtiene a traves de una tabla de relacion
            // por lo cual viene en una lista de un elemento.
            var model = new BusquedaAvanzadaModel()
            {
                Componente = componentes[0],
                Componentelist = componentes,
                AgrupIdComponente = modelo.Rangos,
                Agrupamiento = modelo.Agrupacion > 0 ? 1 : 0,
                Nombre = modelo.Nombre,
                Descripcion = modelo.Descripcion,
                BusquedaObjetoId = modelo.ConfiguracionId,
                Visibilidad = modelo.Visibilidad,
                FechaCreacion = modelo.FechaCreacion,
                Visualizacion = new VisualizacionModel()
                {
                    Distribucion = modelo.Distribucion,
                    Rangos = int.Parse(modelo.Rangos.ToString()),
                    Coloreado = long.Parse(modelo.Color),
                    Transparencia = modelo.Transparencia,
                    Items = new List<VisualizacionItemModel>()
                }
            };
            model.Filtros = this.ConfiguracionesFiltroByMT(modelo.ConfiguracionId)
                                .Select(item =>
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
                                })
                                .ToList();
            switch (modelo.Agrupacion)
            {
                case 1: model.OperacionAgrup = "Dentro"; break;
                case 2: model.OperacionAgrup = "Tocando"; break;
                default: break;
            }
            model.cantFiltrosAtributo = model.Filtros.Count(f => f.FiltroTipo == 1);
            model.cantFiltrosGeografico = model.Filtros.Count(f => f.FiltroTipo == 2);
            model.cantFiltrosAtributo = model.Filtros.Count() - (model.cantFiltrosAtributo + model.cantFiltrosGeografico);

            Session["GUID"] = Guid.NewGuid().ToString();
            Session["BusquedaAvanzada"] = model;
            Session["MT_Predefinido"] = 0;
            return GetResultadoView(model);
        }

        public JsonResult RemoveFilter(long idFilter)
        {
            var model = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];

            model.Filtros.RemoveAll(x => x.FiltroId == idFilter);

            Session["BusquedaAvanzada"] = model;

            return Json(new { state = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        public ActionResult GetVisualizacionViewVolver(MapaTematicoModel model, string returnUrl, string siguente)
        {
            Session["GenerarResultado"] = 0;
            return null;
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

        [HttpGet]
        public ActionResult CargarUltimaBusquedaAvanzada()
        {
            var model = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];
            if (model != null)
            {
                return PartialView("Visualizacion", model);
            }
            else
            {
                return Json(new { msg = "No tiene mapa cargado" }, JsonRequestBehavior.AllowGet);
            }
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
            ViewBag.PublicarDespublicarEnabled = habilitado && SeguridadController.ExisteFuncion(Seguridad.PublicarDespublicarBibliotecaBA);
            ViewBag.ElimnarPublicaEnabled = SeguridadController.ExisteFuncion(Seguridad.EliminarBibliotecaBA);

            return PartialView("Bibliotecas", BiblioModel);
        }

        public FileResult ExportarAExcelBiblioteca(long id)
        {
            using (var memStreamTemp = new MemoryStream())
            using (var package = new ExcelPackage(memStreamTemp))
            {
                MapaTematicoConfiguracionModelo modelo = this.GetMapaTematicoById(id);
                List<MT.ComponenteConfiguracion> cc = this.GetComponenteConfiguracionByMTId(id);
                ExcelWorksheet hojaAtributos = package.Workbook.Worksheets.Add(modelo.Nombre);
                hojaAtributos.Cells[1, 1].Value = "Configuración ID";
                hojaAtributos.Cells[1, 2].Value = "Componente ID";
                int fila = 2;

                foreach (var item in cc)
                {
                    hojaAtributos.Cells[fila, 1].Value = modelo.ConfiguracionId;
                    hojaAtributos.Cells[fila, 2].Value = item.ComponenteId;
                    fila++;
                }

                using (var range = hojaAtributos.Cells[1, 1, 1, 2])
                {
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                hojaAtributos.Cells.AutoFitColumns(0);

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BúsquedaAvanzada " + modelo.Nombre + ".xlsx"); ;
            }
        }

        public ActionResult ExportarAExcel()
        {
            BusquedaAvanzadaModel busquedaAvanzada = ((BusquedaAvanzadaModel)Session["BusquedaAvanzada"]);
            var agrupamiento = busquedaAvanzada.Agrupamiento;
            var agrupador = busquedaAvanzada.Componente.Descripcion;
            var resultados = busquedaAvanzada.ListaResultados;

            if (resultados != null)
            {
                ObjetoExcel objetoAgrupador = null;
                var exportador = new ExportadorObjetosExcel(resultados.SelectMany(x => x)
                                                                      .OrderBy(r => r.Descripcion)
                                                                      .Select(o => new ObjetoExcel
                                                                      {
                                                                          componente = o.CompAgrupador.DocType,
                                                                          id = o.ObjetoId,
                                                                          agrupador = o.CompAgrupador.Nombre
                                                                      }));
                if (agrupamiento != 0)
                {
                    objetoAgrupador = new ObjetoExcel { componente = agrupador };
                }

                AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.ExportarExcelBA);

                return File(exportador.Exportar("Búsqueda Avanzada", objetoAgrupador), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportacionBusquedaAvanzada.xlsx");
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
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

        public int CallCopyMapaTematico(long id, long componenteId, int existe)//Revisar esta funcionalidad en BA
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

        public MT.Predefinido GetPredefinidoById(long idPredefinido)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetPredefinidoById/" + idPredefinido).Result;
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
        public JsonResult Upload()//No se usa
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
        public JsonResult GuardarColeccion(string nombre)
        {
            BusquedaAvanzadaModel busquedaAvanzada = ((BusquedaAvanzadaModel)Session["BusquedaAvanzada"]);

            try
            {
                if (busquedaAvanzada.Agrupamiento == 0)
                {
                    return Json(GrabarColeccion(busquedaAvanzada.ListaResultados, nombre));
                }
                else
                {
                    List<long> idsAgrupadores = new List<long>();
                    foreach (var item in busquedaAvanzada.ListaResultados)
                    {
                        foreach (var objeto in item)
                        {
                            idsAgrupadores.Add(objeto.CompAgrupador.ComponenteId);
                        }
                    }
                    idsAgrupadores = idsAgrupadores.Distinct().ToList();


                    List<ObjetoModel> all = new List<ObjetoModel>();

                    foreach (var lista in busquedaAvanzada.ListaResultados)
                    { all.AddRange(lista); }


                    foreach (var grupo in idsAgrupadores)
                    {
                        string nombreColeccion = nombre + "-" + busquedaAvanzada.Componente.Descripcion + " Nro:" + grupo.ToString();
                        var tanto = all.Where(a => a.CompAgrupador.ComponenteId == grupo).ToList();
                        List<List<ObjetoModel>> nuevaColeccion = new List<List<ObjetoModel>>();
                        nuevaColeccion.Add(tanto);
                        GrabarColeccion(nuevaColeccion, nombreColeccion);
                    }

                    var resultados = busquedaAvanzada.ListaResultados;//lista de listas de objetos model para coleccion

                    //string res = GrabarColeccion(resultados, nombre);
                    string ress = string.Empty;
                    return Json(ress);
                }
            }
            finally
            {
                AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Alta, Autorizado.Si, Eventos.GrabarColeccionBA);
            }

        }

        [HttpPost]
        public JsonResult GuardarBiblioteca(string nombre, string descripcion)//Viene de Visualizacion
        {
            int guardoOK = 0;

            var mapatematico = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];
            string guid = Session["GUID"].ToStringOrDefault();
            long idUsuario = this.UsuarioConectado.Id_Usuario;

            mapatematico.Nombre = nombre;
            mapatematico.Descripcion = descripcion;

            if (!string.IsNullOrEmpty(mapatematico.Nombre))
            {
                #region GrabarBiblioteca
                var mtc = new MapaTematicoConfiguracionModelo()
                {
                    ConfiguracionId = 0,
                    idConfigCategoria = 1,
                    Nombre = mapatematico.Nombre,
                    Descripcion = mapatematico.Descripcion,
                    Rangos = mapatematico.AgrupIdComponente,
                    Distribucion = mapatematico.Visualizacion.Distribucion,
                    Color = mapatematico.Visualizacion.Coloreado.ToString(), // contingencia como no se guarda el tipo de coloreado se utiliza el campo color en des uso para guardar ese dato.
                    Transparencia = mapatematico.Visualizacion.Transparencia,
                    Visibilidad = mapatematico.Visibilidad,
                    Usuario = idUsuario,
                    FechaCreacion = mapatematico.FechaCreacion
                };
                if (mapatematico.ComponenteAtributo.Atributo.EsImportado)
                {
                    mtc.Externo = 1;
                }
                else
                {
                    mtc.Atributo = mapatematico.ComponenteAtributo.Atributo.AtributoId ?? 0;
                }
                switch (mapatematico.OperacionAgrup)
                {
                    case "Dentro": mtc.Agrupacion = 1; break;
                    case "Tocando": mtc.Agrupacion = 2; break;
                    default: mtc.Agrupacion = 0; break;
                }

                mtc.ComponenteConfiguracion = mapatematico.Componentelist
                                                          .Select(cmp => new ComponenteConfiguracionModel
                                                          {
                                                              ComponenteId = cmp.ComponenteId,
                                                              ConfiguracionId = mtc.ConfiguracionId
                                                          }).ToList();

                mtc.ConfiguracionFiltro = mapatematico.Filtros
                                                      .Select((filtro, idx) => new MT.ConfiguracionFiltro
                                                      {
                                                          ConfiguracionId = mtc.ConfiguracionId,
                                                          FiltroId = idx,
                                                          FiltroTipo = filtro.FiltroTipo,
                                                          FiltroComponente = filtro.FiltroComponente,
                                                          FiltroAtributo = filtro.FiltroAtributo,
                                                          FiltroOperacion = filtro.FiltroOperacion ?? 0,
                                                          FiltroColeccion = filtro.FiltroColeccion,
                                                          Valor1 = filtro.Valor1,
                                                          Valor2 = filtro.Valor2,
                                                          Ampliar = filtro.Ampliar,
                                                          Dentro = filtro.Dentro,
                                                          Tocando = filtro.Tocando,
                                                          Fuera = filtro.Fuera,
                                                          Habilitado = filtro.Habilitado ?? 0,
                                                          PorcentajeInterseccion = filtro.PorcentajeInterseccion,
                                                          ConfiguracionesFiltroGrafico = filtro.FiltroTipo == 2
                                                                                                ? (filtro.Coordenadas ?? string.Empty)
                                                                                                        .Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                                                                                                        .Select(coord => new MT.ConfiguracionFiltroGrafico()
                                                                                                        {
                                                                                                            FiltroId = idx,
                                                                                                            Coordenadas = coord,
                                                                                                            sGeometry = coord
                                                                                                        }).ToList()
                                                                                                : null
                                                      }).ToList();

                mtc.ConfiguracionRango = mapatematico.Visualizacion.Items.Select((item, idx) =>
                {
                    return new MT.ConfiguracionRango()
                    {
                        Orden = idx,
                        Desde = ((object)item.Desde ?? (string.IsNullOrEmpty(item.Valor) ? string.Empty : item.Valor)).ToString(),
                        Hasta = ((object)item.Desde ?? string.Empty).ToString(),
                        Cantidad = item.Casos,
                        Leyenda = item.Leyenda ?? string.Empty,
                        ColorRelleno = item.Color,
                        ColorLinea = item.ColorBorde,
                        EspesorLinea = item.AnchoBorde
                    };
                }).ToList();

                this.GrabarMapaTematico(mtc);
                #endregion
                guardoOK = 1;
            }
            return Json(guardoOK);
        }

        private string GetMapaTemanticoNombre(string nombre, long idUsuario, long? configuracionId)
        {
            string retNombre = nombre;
            if (nombre != null && nombre != string.Empty)
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
                            //mapatematico.mensaje = "Ya existe una definición con dicho nombre. Cambie el nombre para continuar.";
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

        public JsonResult GetAtributosCombo(string id)
        {
            return Json(new SelectList(GetAtributosByComponente(long.Parse(id)), "AtributoId", "Nombre"));
        }

        public JsonResult GetAtributosByComp(string id)
        {
            return Json(GetAtributosByComponente(long.Parse(id)));
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

        public HttpStatusCodeResult EliminarBA(string id)
        {
            this.DeleteBusquedaAvanzadaById(long.Parse(id));
            //RegisterAuditoria(Request, UsuarioConectado.Id_Usuario, Seguridad.EliminarBibliotecaBA, Eventos.EliminarBibliotecaBA, "Busqueda avanzada eliminada");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        public JsonResult CambiarVisibilidad(string id)
        {
            return Json(this.CambiarVisibilidadMapaTematicoById(long.Parse(id)));
        }

        public List<MT.ComponenteConfiguracion> GetComponenteConfiguracionByMTId(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponenteConfiguracionByMTId/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<MT.ComponenteConfiguracion>)resp.Content.ReadAsAsync<List<MT.ComponenteConfiguracion>>().Result;
        }
        public List<ComponenteModel> GetComponentesByBiblioteca(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponentesByBiblioteca/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<List<ComponenteModel>>().Result;
        }
        public string DeleteBusquedaAvanzadaById(long id)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/BusquedaAvanzada/DeleteMapaTematicoById?id=" + id, id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<String>().Result;
        }
        public string CambiarVisibilidadMapaTematicoById(long id)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/BusquedaAvanzada/CambiarVisibilidadMapaTematicoById?id=" + id, id).Result;
            resp.EnsureSuccessStatusCode();
            var result = resp.Content.ReadAsAsync<string>().Result;
            //string funcion = result == "0" ? Seguridad.DespublicarBibliotecaBA : Seguridad.PublicarBibliotecaBA;
            //string evento = result == "0" ? Eventos.DespublicarBibliotecaBA : Eventos.PublicarBibliotecaBA;
            //RegisterAuditoria(Request, UsuarioConectado.Id_Usuario, funcion, evento, "Cambiar Visibilidad Busqueda Avanzada");
            return result;
        }

        public List<MT.ConfiguracionFiltro> ConfiguracionesFiltroByMT(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/ConfiguracionFiltroByMT/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<MT.ConfiguracionFiltro>>().Result;
        }

        public List<MT.ConfiguracionRango> ConfiguracionesRangoByMT(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/ConfiguracionesRangoByMT/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<MT.ConfiguracionRango>>().Result;
        }
        public MapaTematicoConfiguracionModelo GetMapaTematicoByName(string nombre)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetMapaTematicoByName/?nombre=" + HttpUtility.UrlEncode(nombre)).Result;
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
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetMapaTematicoById/" + id).Result;
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
            HttpResponseMessage resp = cliente.PostAsJsonAsync<MapaTematicoConfiguracionModelo>("api/BusquedaAvanzada/CopyMapaTematicoById", mtm).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                //return (MapaTematicoConfiguracionModelo)resp.Content.ReadAsAsync<MapaTematicoConfiguracionModelo>().Result;
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
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetMapaTematicosPublicos").Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<ComponenteConfiguracionModel>>().Result;
            }
            catch
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }

        public List<ComponenteConfiguracionModel> GetBibliotecasPrivadasByUsuario(long IdUsuario)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetBibliotecasPrivadasByUsuario/" + IdUsuario).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<ComponenteConfiguracionModel>>().Result;
            }
            catch
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }
        public List<string[]> calcularCoincidencia(long componenteId, long fileId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/calcularCoincidencia/?componenteId=" + componenteId + "&fileId=" + fileId).Result;
            resp.EnsureSuccessStatusCode();
            return (List<string[]>)resp.Content.ReadAsAsync<List<string[]>>().Result;
        }
        public long ProcesarArchivo(string fullPath)
        {
            try
            {
                //HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/procesarArchivo/?fullpath=" + fullPath).Result;
                var waiter = cliente.GetAsync("api/BusquedaAvanzada/procesarArchivo/?fullpath=" + fullPath);
                waiter.Wait(System.Threading.Timeout.Infinite, CancellationToken.None);
                var resp = waiter.Result;
                try
                {
                    resp.EnsureSuccessStatusCode();
                    return (long)resp.Content.ReadAsAsync<long>().Result;
                }
                catch
                {
                    string msgErr = resp.ReasonPhrase;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                string msgErr = ex.Message;
                return 0;
            }
        }
        public List<ComponenteModel> GetComponentes()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponentes").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }
        public List<ComponenteModel> GetComponentesGeograficos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponentesGeograficos").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }
        public ComponenteModel GetComponentesById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponentesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ComponenteModel)resp.Content.ReadAsAsync<ComponenteModel>().Result;
        }
        public AtributoModel GetAtributosById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetAtributosById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (AtributoModel)resp.Content.ReadAsAsync<AtributoModel>().Result;
        }
        public List<ComponenteModel> SearchComponente(string texto)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponentesBusqueda/" + texto).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }
        public List<ComponenteModel> GetComponentesRelacionados(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponentesRelacionados/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ComponenteModel>)resp.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;
        }
        public List<AtributoModel> GetAtributosByComponente(long id)
        {

            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetAtributosByComponente/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<AtributoModel>)resp.Content.ReadAsAsync<IEnumerable<AtributoModel>>().Result;
        }
        public List<TipoOperacionModel> GetOperaciones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetOperaciones").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoOperacionModel>)resp.Content.ReadAsAsync<IEnumerable<TipoOperacionModel>>().Result;
        }
        public List<TipoOperacionModel> GetOperacionesEspeciales(long? id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetOperacionesEspeciales/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoOperacionModel>)resp.Content.ReadAsAsync<IEnumerable<TipoOperacionModel>>().Result;
        }
        public TipoOperacionModel GetOperacionesById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetOperacionesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (TipoOperacionModel)resp.Content.ReadAsAsync<TipoOperacionModel>().Result;
        }
        public List<AgrupacionModel> GetAgrupaciones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetAgrupaciones").Result;
            resp.EnsureSuccessStatusCode();
            return (List<AgrupacionModel>)resp.Content.ReadAsAsync<IEnumerable<AgrupacionModel>>().Result;
        }
        public ColeccionModel GetColeccionesById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetColeccionesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ColeccionModel)resp.Content.ReadAsAsync<ColeccionModel>().Result;
        }
        public List<ColeccionModel> GetColecciones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetColeccionesCombo").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ColeccionModel>)resp.Content.ReadAsAsync<IEnumerable<ColeccionModel>>().Result;
        }
        public List<ColeccionModel> GetColeccionesSoloUnComponente()
        {
            using (var resp = cliente.GetAsync(string.Format("api/BusquedaAvanzada/GetColeccionesSoloUnComponenteByUsuario?usuario={0}", UsuarioConectado.Id_Usuario)).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<ColeccionModel>>().Result;
            }
        }

        public MT.MapaTematicoConfiguracion GrabarMapaTematico(MapaTematicoConfiguracionModelo mtc)
        {
            using (var resp = cliente.PostAsJsonAsync("api/BusquedaAvanzada/GrabaBusquedaAvanzada", mtc).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<MT.MapaTematicoConfiguracion>().Result;
            }
        }

        public string GrabarColeccion(List<List<ObjetoModel>> objetos, string nombre)
        {
            ColeccionModel coleccionModel = new ColeccionModel();

            coleccionModel.Nombre = nombre;
            coleccionModel.UsuarioId = UsuarioConectado.Id_Usuario;
            coleccionModel.Objetos = new List<ObjetoModel>();

            foreach (var comp in objetos)
            {
                coleccionModel.Objetos.AddRange(comp);
            }
            HttpResponseMessage resp = cliente.PostAsync("api/BusquedaAvanzada/GrabarColeccion", new ObjectContent<ColeccionModel>(coleccionModel, new JsonMediaTypeFormatter())).Result;

            try
            {
                resp.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "OK";
        }

        public MT.ObjetoResultadoDetalle GetObjetoResultadoDetalle(string guid, long distribucion, long cantRangos)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetObjetoResultadoDetalle/?guid=" + guid + "&distribucion=" + distribucion + "&cantRangos=" + cantRangos).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<MT.ObjetoResultadoDetalle>().Result;
        }

        public int ActualizarResultadoMapaTematico(MT.ObjetoResultadoDetalle objetoResultadoDetalle)//No se usa en BA
        {
            HttpResponseMessage resp = cliente.PostAsync<MT.ObjetoResultadoDetalle>("api/BusquedaAvanzada/ActualizarResultadoMapaTematico", objetoResultadoDetalle, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return (int)resp.Content.ReadAsAsync<int>().Result;
        }

        [HttpPost]
        public JsonResult GetMapaTematicoExtents()
        {
            string guid = Session["GUID"].ToStringOrDefault();
            string parametros = string.Format("guid={0}", guid);
            HttpResponseMessage resp = cliente.PostAsync("api/BusquedaAvanzada/GetMapaTematicoExtents?" + parametros, null).Result;
            resp.EnsureSuccessStatusCode();
            JsonResult jsonResult = new JsonResult
            {
                Data = (string)resp.Content.ReadAsAsync<string>().Result
            };
            return jsonResult;
        }

        [HttpGet]
        public FileResult BuscarAyuda()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/BuscarAyuda").Result;
            resp.EnsureSuccessStatusCode();
            return File(Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result), "application/pdf", "BusquedasAvanzadasAyuda.pdf");
        }

        public List<ParametrosGeneralesModel> GetParametrosGenerales()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetParametrosGenerales").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<ParametrosGeneralesModel>>().Result;
        }

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

        private List<ComponenteModel> getComponentesPadresAgrupamiento(IEnumerable<long> ids)
        {
            //GetComponentesAgrupadores
            HttpResponseMessage respJerarquia = cliente.GetAsync("api/BusquedaAvanzada/GetComponentesPadres?idComponentes=" + string.Join(",", ids)).Result;
            HttpResponseMessage resp = cliente.GetAsync("api/BusquedaAvanzada/GetComponentesAgrupadores").Result;
            respJerarquia.EnsureSuccessStatusCode();
            resp.EnsureSuccessStatusCode();
            var model = (BusquedaAvanzadaModel)Session["BusquedaAvanzada"];
            model.ComponentesJerarquicosAgrupamiento = (List<ComponenteModel>)respJerarquia.Content.ReadAsAsync<List<ComponenteModel>>().Result;
            Session["BusquedaAvanzada"] = model;
            return resp.Content.ReadAsAsync<List<ComponenteModel>>().Result;
        }

        private List<List<ObjetoModel>> getResultadoBusquedaAvanzada(BusquedaAvanzadaModel modelo)
        {
            foreach (var filtro in (modelo?.Filtros ?? new List<FiltroModel>()))
            {
                filtro.FiltroOperacion = filtro.FiltroOperacion ?? 16;
            }
            var resp = cliente.PostAsJsonAsync("api/BusquedaAvanzada/ResultadoBusquedaAvanzada", modelo, new CancellationTokenSource(TimeSpan.FromMinutes(30)).Token).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                var componentes = resp.Content.ReadAsAsync<List<ObjetoModel>>().Result;
                modelo.ComponentesAgrupadoresParaVista = componentes
                                                            .OrderBy(c => c.CompAgrupador.Nombre)
                                                            .ThenBy(c => c.CompAgrupador.Descripcion)
                                                            .GroupBy(c => c.CompAgrupador.ComponenteId)
                                                            .Select(g => g.GroupBy(sg => sg.ComponenteID)
                                                                          .Select(sg => sg.ToArray())
                                                                          .ToList())
                                                            .ToList();
                return (new[] { componentes }).ToList();
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("getResultadoBusquedaAvanzada", new Exception(resp.ReasonPhrase, ex));
                throw;
            }
        }

        public JsonResult PasarGrillaResultados(string seleccionados)
        {
            var grupos = JsonConvert.DeserializeAnonymousType(seleccionados, new[] { new { objeto = 0L, componente = 0L } })
                                    .GroupBy(x => x.componente, (k, v) => new { componente = k, ids = v.Select(o => o.objeto.ToString()) });

            var resultados = new PasarGrilla[0];
            foreach (var grupo in grupos)
            {
                string doctype = this.ObtenerComponente(grupo.componente).DocType;
                resultados = resultados.Concat(grupo.ids.Select(id => new PasarGrilla() { layer = doctype, id = id })).ToArray();
            }
            return Json(new { success = true, data = resultados });
        }
        public JsonResult VerResultadosEnMapa(string seleccionados)
        {
            var grupos = JsonConvert.DeserializeAnonymousType(seleccionados, new[] { new { objeto = 0L, componente = 0L } })
                                    .GroupBy(x => x.componente, (k, v) => new { componente = k, ids = v.Select(o => o.objeto.ToString()) });

            var ubicar = new List<object>();
            foreach (var grupo in grupos)
            {
                string capa = this.ObtenerComponente(grupo.componente).Capa;
                foreach (string c in capa.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var objetosPorCapa = new
                    {
                        capa = c,
                        objetos = grupo.ids.ToArray()
                    };
                    ubicar.Add(objetosPorCapa);
                }
            }
            AuditoriaHelper.Register(UsuarioConectado.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Consulta, Autorizado.Si, Eventos.VerMapaBA);
            return Json(new { data = ubicar });
        }
        public Componente ObtenerComponente(long idComponente)
        {
            var result = cliente.GetAsync(string.Format("api/Componente/Get/{0}", idComponente)).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<Componente>().Result;
        }

        public IEnumerable<MT.Atributo> ObtenerAtributosAll(long idComponente)
        {
            var result = cliente.GetAsync(string.Format("api/Atributo/GetByComponente/{0}", idComponente)).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<IEnumerable<GeoSit.Data.BusinessEntities.MapasTematicos.Atributo>>().Result;
        }

        private void RegisterAuditoria(HttpRequestBase request, long idUsuario, string funcionStr, string eventoNombre, string desc)
        {
            SeguridadController sc = new SeguridadController();
            sc.RegisterAuditoria(request, idUsuario, funcionStr, eventoNombre, desc);
        }

        private List<ComponenteModel> GetListaPrimeroSeleccionado(BusquedaAvanzadaModel model)
        {
            var listaComponentes = GetComponentes();
            foreach (var seleccionado in listaComponentes
                                            .Where(c => model.Componentelist.Any(x => x.ComponenteId == c.ComponenteId))
                                            .OrderByDescending(x => x.Nombre))
            {
                listaComponentes.MoveElementToFirstPosition(seleccionado);
            }
            return listaComponentes;
        }

        private List<ComponenteModel> GetListaPrimeroSeleccionadoAgrupadores(BusquedaAvanzadaModel model)
        {
            List<ComponenteModel> componentesAgrupadores = getComponentesPadresAgrupamiento(model.Componentelist.Select(c => c.ComponenteId));
            ComponenteModel seleccionado = componentesAgrupadores.Where(c => c.ComponenteId == model.AgrupIdComponente).FirstOrDefault();
            return componentesAgrupadores.MoveElementToFirstPosition<ComponenteModel>(seleccionado);
        }
    }
}