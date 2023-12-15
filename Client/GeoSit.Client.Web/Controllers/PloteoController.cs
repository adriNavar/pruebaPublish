using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using it = iTextSharp.text;
using System.Text;
using Ionic.Zip;
using OfficeOpenXml;
using iTextSharp.text;
using GeoSit.Client.Web.Models;
using System.Net.Http.Formatting;
using System.Net;
using System.Net.Http;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Client.Web.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using GeoSit.Data.BusinessEntities.Ploteo;
using GeoSit.Data.BusinessEntities.GlobalResources;
using Newtonsoft.Json.Linq;

namespace GeoSit.Client.Web.Controllers
{
    public class PloteoController : Controller
    {
        private const string SUPERA_MAXIMO_PLOTEO = "Supera el máximo permitido de ploteos";
        private const string DOCUMENTO_SIN_PAGINAS = "El documento no contiene páginas";

        string responseError = string.Empty;

        private UsuariosModel Usuario { get { return Session != null ? (UsuariosModel)Session["usuarioPortal"] : null; } }

        private Plantilla PlantillaLocal { get; set; }
        private Plantilla Plantilla
        {
            get
            {
                return Session != null ? (Plantilla)Session["plantillaPloteo"] : PlantillaLocal;
            }
            set
            {
                if (Session != null)
                {
                    Session["plantillaPloteo"] = value;
                }
                else
                {
                    PlantillaLocal = value;
                }
            }
        }
        public HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });
        private const string ApiUri = "api/plantilla/";

        private FileContentResult zipFileLocal { get; set; }
        private FileContentResult zipFile
        {
            get
            {
                return Session != null ? (FileContentResult)Session["zip_file"] : zipFileLocal;
            }
            set
            {
                if (Session != null)
                {
                    Session["zip_file"] = value;
                }
                else
                {
                    zipFileLocal = value;
                }
            }
        }

        private List<Periodo> PeriodosLocal { get; set; }
        private List<Periodo> Periodos
        {
            get
            {
                return Session != null ? (List<Periodo>)Session["Periodos"] : PeriodosLocal;
            }
            set
            {
                if (Session != null)
                {
                    Session["Periodos"] = value;
                }
                else
                {
                    PeriodosLocal = value;
                }
            }
        }
        private List<Partido> PartidosLocal { get; set; }
        private List<Partido> Partidos
        {
            get
            {
                return Session != null ? (List<Partido>)Session["Partidos"] : PartidosLocal;
            }
            set
            {
                if (Session != null)
                {
                    Session["Partidos"] = value;
                }
                else
                {
                    PartidosLocal = value;
                }
            }
        }

        private List<TipoPlano> TipoPlanosLocal { get; set; }
        private List<TipoPlano> TipoPlanos
        {
            get
            {
                return Session != null ? (List<TipoPlano>)Session["TipoPlanos"] : TipoPlanosLocal;
            }
            set
            {
                if (Session != null)
                {
                    Session["TipoPlanos"] = value;
                }
                else
                {
                    TipoPlanosLocal = value;
                }
            }
        }

        private List<PartidoApicRedes> PartidosApicRedesLocal { get; set; }
        private List<PartidoApicRedes> PartidosApicRedes
        {
            get
            {
                return Session != null ? (List<PartidoApicRedes>)Session["PartidosApicRedes"] : PartidosApicRedesLocal;
            }
            set
            {
                if (Session != null)
                {
                    Session["PartidosApicRedes"] = value;
                }
                else
                {
                    PartidosApicRedesLocal = value;
                }
            }
        }
        public PloteoController()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["webApiUrl"]))
            {
                _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            }
            _cliente.Timeout = TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["httpClientIncreasedTimeout"]));
        }

        #region INDEX
        public ActionResult Index()
        {
            //bool existeFunPlotObras = SeguridadController.ExisteFuncion(Resources.Seguridad.PloteoObra);
            bool existeFunPlotObras = true;

            var idsCategorias = new List<short>() { 1, 2, 3 };

            if (existeFunPlotObras)
                idsCategorias.Add(13);

            long idUsuario = Usuario.Id_Usuario;
            var listaFiltrada = getPlantillasByIdsCategoria(idsCategorias.ToArray())
                                    .Where(p => p.Visibilidad == 0 || (p.Visibilidad == 1 && p.IdUsuarioModificacion == idUsuario)); //filtra las públicas y las privadas del usuario
            //agrupa las plantillas por categoria

            var listaTipos = new List<CategoriaModel>
                {
                    new CategoriaModel{ IdTipo = 1, Nombre= "Predefinidos", Action="GetPredefinido" },
                    new CategoriaModel{ IdTipo = 2, Nombre= "Plancheta A4", Action="GetPlanchetaA4" },
                    new CategoriaModel{ IdTipo = 3, Nombre= "Ploteo General", Action="GetPloteoGeneral" }
                };

            /*if (existeFunPlotObras)
                listaTipos.Add(new CategoriaModel { IdTipo = 13, Nombre = "Ploteo Obras", Action = "GetPloteoObras" });*/

            // var grupos = listaPlantillas.GroupBy(m => m.IdPlantillaCategoria, (k, v) => new { categoria = k, plantillas = v.OrderBy(h => h.IdPlantilla) });
            var grupos = listaFiltrada.GroupBy(m => m.IdPlantillaCategoria, (k, v) => new { categoria = k, plantillas = v.OrderBy(h => h.IdPlantilla) });

            foreach (var tipo in listaTipos)
            {
                var grupo = grupos.ToList().SingleOrDefault(g => g.categoria == tipo.IdTipo);
                tipo.Plantillas = grupo == null ? new List<Plantilla>() : grupo.plantillas.Where(n => n.FechaBaja == null && n.IdUsuarioBaja == null).ToList().OrderBy(h => h.Nombre).ToList();
            }
            ViewBag.Categorias = listaTipos.OrderBy(t => t.IdTipo);
            ViewBag.VieneDeColeccion = false;
            return PartialView("~/Views/Ploteo/Ploteo.cshtml");
        }
        #endregion

        public ActionResult PloteoGeneral(long id)
        {
            ActionResult result = Index();
            ViewBag.VieneDeColeccion = true;
            ViewBag.idColeccion = id;
            return result;
        }

        #region OBTENER PLANTILLAS
        public List<Plantilla> getPlantillas()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Plantilla/Get").Result;
            resp.EnsureSuccessStatusCode();

            return (List<Plantilla>)resp.Content.ReadAsAsync<IEnumerable<Plantilla>>().Result;
        }
        #endregion

        #region OBTENER PLANTILLAS BY CATEGORIA
        public List<Plantilla> getPlantillasByIdsCategoria(short[] ids)
        {
            HttpResponseMessage resp = _cliente.PostAsync("api/Plantilla/GetByCategorias", new ObjectContent(ids.GetType(), ids, new JsonMediaTypeFormatter())).Result;

            resp.EnsureSuccessStatusCode();

            return (List<Plantilla>)resp.Content.ReadAsAsync<IEnumerable<Plantilla>>().Result;
        }
        #endregion

        #region OBTENER PLANTILLAS BY ID
        private Plantilla GetPlantilla(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Plantilla/GetResumen/" + id).Result;
            resp.EnsureSuccessStatusCode();
            Plantilla plantilla = resp.Content.ReadAsAsync<Plantilla>().Result;
            PlantillaFondo fondo = plantilla.PlantillaFondos.First();
            if (fondo.IBytes != null)
            {
                Bitmap.FromStream(new MemoryStream(fondo.IBytes)).Save(Path.Combine(ConfigurationManager.AppSettings["UploadPath"], fondo.ImagenNombre + ".png"), ImageFormat.Png);
            }
            return plantilla;
        }
        #endregion

        private List<Periodo> GetAllPeriodo()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Periodo/Get").Result;
            resp.EnsureSuccessStatusCode();
            return (List<Periodo>)resp.Content.ReadAsAsync<IEnumerable<Periodo>>().Result;
        }

        private List<Partido> GetAllPartido()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Partido/Get").Result;
            resp.EnsureSuccessStatusCode();
            return (List<Partido>)resp.Content.ReadAsAsync<IEnumerable<Partido>>().Result;
        }

        private List<PartidoApicRedes> GetAllPartidoApicRedes()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/PartidoApicRedes/Get").Result;
            resp.EnsureSuccessStatusCode();
            return (List<PartidoApicRedes>)resp.Content.ReadAsAsync<IEnumerable<PartidoApicRedes>>().Result;
        }

        private List<TipoPlano> GetAllTipoPlano()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TipoPlano/Get").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoPlano>)resp.Content.ReadAsAsync<IEnumerable<TipoPlano>>().Result;
        }

        public JsonResult GetTipoPlanoByIdPeriodo(int idPeriodo)
        {
            //List<TipoPlano> lstTipoPlano = this.TipoPlanos;
            List<TipoPlano> lstTipoPlano = new List<TipoPlano>();
            Periodo periodo = this.Periodos.FirstOrDefault(p => p.IdPeriodo == idPeriodo);
            if (periodo != null)
            {
                if (!periodo.Activo)
                {
                    HttpResponseMessage resp = _cliente.GetAsync("api/TipoPlano/GetByIdPeriodo/" + idPeriodo).Result;
                    resp.EnsureSuccessStatusCode();
                    lstTipoPlano = (List<TipoPlano>)resp.Content.ReadAsAsync<IEnumerable<TipoPlano>>().Result;
                }
                else
                {
                    lstTipoPlano = this.TipoPlanos.Where(p => p.Activo).ToList();
                }
            }
            SelectList tiposPlanos = new SelectList(lstTipoPlano, "IdTipoPlano", "Nombre");
            return Json(new { slTipoPlanos = tiposPlanos }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPartidoByPeriodoTipoPlano(int idPeriodo, int idTipoPlano)
        {
            //List<Partido> lstPartido = this.Partidos;
            List<Partido> lstPartido = new List<Partido>();
            Periodo periodo = this.Periodos.FirstOrDefault(p => p.IdPeriodo == idPeriodo);
            TipoPlano tipoPlano = this.TipoPlanos.FirstOrDefault(p => p.IdTipoPlano == idTipoPlano);
            if (periodo != null && tipoPlano != null)
            {
                if (!periodo.Activo)
                {
                    //HttpResponseMessage resp = _cliente.GetAsync("api/PartidoApicRedes/GetByIdPeriodoTipoPlano/?idPeriodo=" + idPeriodo + "&idTipoPlano=" + idTipoPlano).Result;
                    HttpResponseMessage resp = _cliente.GetAsync("api/Partido/GetByIdPeriodoTipoPlano/?idPeriodo=" + idPeriodo + "&idTipoPlano=" + idTipoPlano).Result;
                    resp.EnsureSuccessStatusCode();
                    lstPartido = (List<Partido>)resp.Content.ReadAsAsync<IEnumerable<Partido>>().Result;
                }
                else
                {
                    lstPartido = this.Partidos;
                }
            }
            //SelectList partidos = new SelectList(lstPartido, "GId", "Nombre");
            SelectList partidos = new SelectList(lstPartido, "IdPartido", "Nombre");
            return Json(new { slPartidos = partidos }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetValidaciones(int idPeriodo, int idTipoPlano, int idPartido)
        {
            string valido = "0";
            string msgValidac = string.Empty;
            string nombreColeccion = string.Empty;
            string idComponente = string.Empty;
            string idObjetos = string.Empty;
            Periodo periodo = this.Periodos.FirstOrDefault(p => p.IdPeriodo == idPeriodo);
            TipoPlano tipoPlano = this.TipoPlanos.FirstOrDefault(p => p.IdTipoPlano == idTipoPlano);
            if (periodo != null && tipoPlano != null)
            {
                if (periodo.Activo)
                {
                    if (idTipoPlano == 1)
                    {
                        //Expansión del Servicio. Redes de Agua

                        HttpResponseMessage resp = _cliente.GetAsync("api/InformeAnual/GetValidaciones/?idPeriodo=" + idPeriodo + "&idTipoPlano=" + idTipoPlano + "&idPartido=" + idPartido).Result;
                        resp.EnsureSuccessStatusCode();
                        string retValidaciones = (string)resp.Content.ReadAsAsync<string>().Result;
                        //"2|" + idComponenteCanFala.ToString() + "|" + string.Join(",", lstObjetos);
                        string[] aValidaciones = retValidaciones.Split('|');
                        if (aValidaciones.Length > 0)
                        {
                            string sValido = aValidaciones[0];
                            if (aValidaciones.Length >= 2)
                            {
                                idComponente = aValidaciones[1];
                                idObjetos = aValidaciones[2];
                            }
                            if (sValido == "1")
                            {
                                valido = sValido;
                            }
                            else if (sValido == "2")
                            {
                                valido = "2";
                                msgValidac = "Se han encontrado cañerías faltantes terminadas, por obras de Instalación con área de Expansión, que no tienen asignada un área de expansión";
                                Partido partido = this.Partidos.FirstOrDefault(p => p.IdPartido == idPartido);
                                DateTime ahora = DateTime.Now;
                                string fecha = ahora.Year + "-" + ahora.Month.ToString("00") + "-" + ahora.Day.ToString("00") + "-" + ahora.Hour.ToString("00") + ahora.Minute.ToString("00") + ahora.Second.ToString("00");
                                int iAnio = Convert.ToInt32(periodo.AnioCalendario) - 5 - 2000;
                                string codigo = tipoPlano.CodigoPlano + partido.Abrev + iAnio.ToString();
                                nombreColeccion = string.Format("{0}_{1}", codigo, fecha);
                                //nombreColeccion = string.Format("{0}_{1}_{2}_{3}", tipoPlano.Nombre, partido.Nombre, periodo.Descripcion, fecha);
                            }
                            else
                            {
                                valido = "3";
                            }
                        }
                        else
                        {
                            //Hubo error
                            valido = "3";
                        }
                    }
                    else
                    {
                        valido = "1";
                    }
                }
                else
                {
                    valido = "0";
                }
            }
            return Json(new { Valido = valido, MsgValidac = msgValidac, NombreColeccion = nombreColeccion, IdComponente = idComponente, IdObjetos = idObjetos }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CrearColeccion(string nombreColeccionValidac, string idComponenteValidac, string idObjetosValidac)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/InformeAnual/CrearColeccion/?nombreColeccion=" + nombreColeccionValidac + "&idComponente=" + idComponenteValidac + "&idObjetos=" + idObjetosValidac + "&idUsuario=" + Usuario.Id_Usuario).Result;
            resp.EnsureSuccessStatusCode();
            string creoOK = (string)resp.Content.ReadAsAsync<string>().Result;

            return Json(new { Result = creoOK }, JsonRequestBehavior.AllowGet);
        }

        private Plantilla GetPlantillaById(long idPlantilla)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Plantilla/GetResumen/" + idPlantilla).Result;
            resp.EnsureSuccessStatusCode();
            Plantilla plantilla = resp.Content.ReadAsAsync<Plantilla>().Result;
            return plantilla;
        }

        #region VISTAS PARCIALES SEGUN TIPO DE PLANTILLA
        public ActionResult GetPredefinido(long id)
        {
            //PLANTILLAS
            this.Plantilla = GetPlantilla(id);

            //COMPONENTES
            var componentes = this.Plantilla.Layers
                                .Where(l => l.Categoria == 1 && l.FechaBaja == null && l.IdUsuarioBaja == null)
                                .Select(l => l.Componente).ToList();

            string[] componentesPrincipales = componentes.Select(c => c.Nombre).ToArray();
            ViewBag.ComponentePrincipal = componentesPrincipales.Any() ? string.Join(", ", componentesPrincipales) : "No definido";
            ViewBag.ListComponente = componentes;
            ViewBag.ImagenesSatelitales = GetAllImagenSatelital();
            return PartialView("~/Views/Ploteo/Partial/_Predefinidos.cshtml", this.Plantilla);
        }

        private List<Data.BusinessEntities.MapasTematicos.Componente> GetComponentesPloteables()
        {

            HttpResponseMessage respuesta = _cliente.GetAsync("api/Componente/GetPloteables").Result;
            respuesta.EnsureSuccessStatusCode();
            return respuesta.Content.ReadAsAsync<List<Data.BusinessEntities.MapasTematicos.Componente>>().Result;
        }

        public ActionResult GetPlanchetaA4(long id)
        {

            //************* REVISAR *************************************
            // CAMBIAR ANY POR WHERE.
            // ASEGURAR QUE EL COMBO DE COLECCIONES NO SE CARGUE DEPENDIENDO DEL
            // COMPONENTE PPAL DE LA PLANTILLA SELECCIONADA
            // ESO NO ES PARA PLANCHETA A4!!
            //**************************************************************

            long idComponentePrincipal = 0;
            this.Plantilla = GetPlantilla(id);

            //ID COMPONENTE PRINCIPAL DE LA PLANTILLA SELECCIONADA
            if (this.Plantilla.Layers.FirstOrDefault(l => l.Categoria == 1)?.ComponenteId != 0)
                idComponentePrincipal = this.Plantilla.Layers.FirstOrDefault(l => l.Categoria == 1).ComponenteId;
            else
                idComponentePrincipal = Convert.ToInt64(GetParametrosGenerales().SingleOrDefault(p => p.Descripcion == "ID_COMPONENTE_MANZANA")?.Valor);

            //OBTENER TODAS LAS COLECCIONES QUE CONTENGAN MANZANA/PARCELA ASIGNADAS AL USUARIO LOGUEADO
            var coleccionesByUser = GetColeccionesByUsuarioLogueadoColeccionA4(Usuario.Id_Usuario);

            //FILTRAR ADICIONAL, POR MANZANA/PARCELA
            //var coleccionesSoloManzanaParcela = GetColeccionesByManzanaParcela(coleccionesByUser);

            //FIN FILTRAR ADICIONAL

            //********** SOLO PARA PLANCHETA A4, AGREGAR AL CODIGO
            //ULTIMO ADEMAS, CONSIDERAR EN EL PROCESO PARA PLOTEO LO SIGUIENTE:
            //Si [la colección elegida por el usuario está compuesta solo por parcelas], El sistema genera el proceso del ploteo para las parcelas indicadas e informa la cantidad de parcelas a plotear
            //IMPLICA GETMANZANABYPARCELA o GETMANZANABYPARCELAAPICID

            //Si [la colección está compuesta solo por manzanas, el sistema genera el proceso del ploteo de las manzanas indicadas e informa la cantidad de manzanas a plotear
            //IMPLICA, SI SUPERA EL MAXIMO PERMITIDO, EMITIR UN MENSAJE ADVIRTIENDO Y NO GENERAR

            //Si [la colección está compuesta por manzanas y parcelas], El sistema genera el proceso solamente de las manzanas indicadas descartando las parcelas y el sistema presenta un mensaje con la cantidad de manzanas a generar el ploteo 
            //IMPLICA, SI SUPERA EL MAXIMO PERMITIDO, EMITIR UN MENSAJE ADVIRTIENDO Y NO GENERAR
            //********* SOLO PARA PLANCHETA A4, AGREGAR AL CODIGO

            ViewBag.Colecciones = coleccionesByUser;
            ViewBag.IdComponentePrincipal = idComponentePrincipal;
            ViewBag.ComponentesPloteables = GetComponentesPloteables();

            ViewBag.Comercial = false;
            Dictionary<string, string> opcionesComerciales = new Dictionary<string, string>();
            List<bool> opcionesComercialesHabilitadas = new List<bool>();
            if (Plantilla.IdFuncionAdicional == 7)
            {
                ViewBag.Comercial = true;

                opcionesComerciales.Add("BA", "Categoría  Baldío");
                opcionesComercialesHabilitadas.Add(true);
                opcionesComerciales.Add("PV", "Categoría Propiedad Vertical");
                opcionesComercialesHabilitadas.Add(true);
                opcionesComerciales.Add("PH", "Propiedad Horizontal");
                opcionesComercialesHabilitadas.Add(true);
                opcionesComerciales.Add("TI", "Tipo de Inmueble");
                opcionesComercialesHabilitadas.Add(true);
                opcionesComerciales.Add("AO", "Agua de Obra");
                opcionesComercialesHabilitadas.Add(true);
                opcionesComerciales.Add("CC", "Consumo Cero");
                opcionesComercialesHabilitadas.Add(false);
                opcionesComerciales.Add("BC", "Bajo Consumo");
                opcionesComercialesHabilitadas.Add(false);
                opcionesComerciales.Add("DH", "Destino Humedo");
                opcionesComercialesHabilitadas.Add(false);
                opcionesComerciales.Add("OL", "Obra Liquidada");
                opcionesComercialesHabilitadas.Add(false);
                opcionesComerciales.Add("ARBA", "ARBA");
                opcionesComercialesHabilitadas.Add(false);
                opcionesComerciales.Add("DI", "Detección de Indicios");
                opcionesComercialesHabilitadas.Add(false);

            }

            ViewBag.OpcionesComerciales = opcionesComerciales;
            ViewBag.opcionesComercialesHabilitadas = opcionesComercialesHabilitadas;
            ViewBag.ImagenesSatelitales = GetAllImagenSatelital();

            return PartialView("~/Views/Ploteo/Partial/_PlanchetaA4.cshtml", this.Plantilla);
        }

        public ActionResult GetPloteoGeneral(long id)
        {
            this.Plantilla = GetPlantilla(id);


            var layersPrincipales = this.Plantilla.Layers.Where(l => l.Categoria == 1).ToList();

            var colecciones = GetColeccionesUsuarioByComponentesPrincipales(layersPrincipales);

            ViewBag.Colecciones = colecciones;
            ViewBag.IdComponentePrincipal = Convert.ToInt64(GetParametrosGenerales().SingleOrDefault(p => p.Descripcion == "ID_COMPONENTE_MANZANA")?.Valor);

            var componentes = this.Plantilla.Layers
                                .Where(l => l.Categoria == 1 && l.FechaBaja == null && l.IdUsuarioBaja == null)
                                .Select(l => l.Componente).ToList();

            string[] componentesPrincipales = componentes.Select(c => c.Nombre).ToArray();
            ViewBag.ComponentePrincipal = componentesPrincipales.Any() ? string.Join(", ", componentesPrincipales) : "No definido";
            ViewBag.ImagenesSatelitales = GetAllImagenSatelital();

            return PartialView("~/Views/Ploteo/Partial/_PloteoGeneral.cshtml", this.Plantilla);
        }

        public ActionResult GetPloteoObras(long id)
        {

            long idComponentePrincipal = 0;
            this.Plantilla = GetPlantilla(id);

            //ID COMPONENTE PRINCIPAL DE LA PLANTILLA SELECCIONADA
            if ((this.Plantilla.Layers.FirstOrDefault(l => l.Categoria == 1)?.ComponenteId ?? 0) == 0)
                idComponentePrincipal = this.Plantilla.Layers.FirstOrDefault(l => l.Categoria == 1).ComponenteId;
            else
                throw new Exception("Plantilla sin componente principal");


            var colecciones = GetColeccionesUsuarioByComponentesPrincipales(new List<long>() { idComponentePrincipal });

            List<Data.BusinessEntities.MapasTematicos.Componente> lstComponentes = new List<Data.BusinessEntities.MapasTematicos.Componente>();
            lstComponentes.Add(GetComponenteById(idComponentePrincipal));

            ViewBag.Colecciones = colecciones;
            ViewBag.IdComponentePrincipal = idComponentePrincipal;
            ViewBag.ComponentesPloteables = lstComponentes;


            List<Atributo> lstAtributos = new List<Atributo>();

            lstAtributos = Plantilla.PlantillaTextos.Where(p => p.Atributo != null && p.Tipo == 4).Select(p => p.Atributo).ToList();

            ViewBag.Atributos = lstAtributos.OrderBy(o => o.Orden).ToList();

            return PartialView("~/Views/Ploteo/Partial/_PloteoObras.cshtml", this.Plantilla);
        }
        private Data.BusinessEntities.MapasTematicos.Componente GetComponenteById(long idComp)
        {
            Data.BusinessEntities.MapasTematicos.Componente lstComp = new Data.BusinessEntities.MapasTematicos.Componente();

            HttpResponseMessage respuesta = _cliente.GetAsync("api/Componente/Get?id=" + idComp).Result;
            respuesta.EnsureSuccessStatusCode();
            lstComp = respuesta.Content.ReadAsAsync<Data.BusinessEntities.MapasTematicos.Componente>().Result;

            return lstComp;
        }
        private IEnumerable<Coleccion> GetColeccionesUsuarioByComponentesPrincipales(List<Layer> layersPrincipales)
        {
            HttpResponseMessage respColecciones = _cliente.PostAsync<long[]>("api/Coleccion/ColeccionesUsuarioByComponentesPrincipales?idUsuario=" + Usuario.Id_Usuario, layersPrincipales.Select(l => l.ComponenteId).ToArray(), new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            return respColecciones.Content.ReadAsAsync<List<Coleccion>>().Result;
        }

        private IEnumerable<Coleccion> GetColeccionesUsuarioByComponentesPrincipales(List<long> ids)
        {
            HttpResponseMessage respColecciones = _cliente.PostAsync<long[]>("api/Coleccion/ColeccionesUsuarioByComponentesPrincipales?idUsuario=" + Usuario.Id_Usuario, ids.ToArray(), new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            return respColecciones.Content.ReadAsAsync<List<Coleccion>>().Result;
        }

        public List<Coleccion> GetcoleccionesByUserConComponentePpal(List<Coleccion> coleccionesByUserLogueado, Plantilla plantilla)
        {
            var coleccionesByUserConComponentePpal = new List<Coleccion>();
            var cantComponentesPrincipales = 0;
            var coleccionRedef = new Coleccion();
            var componentesRedef = new List<ColeccionComponente>();

            foreach (Coleccion col in coleccionesByUserLogueado)
            {
                cantComponentesPrincipales = 0;
                //SETEO EL IDCOMPONENTEPPAL EN SESSION
                cantComponentesPrincipales = GetCantComponentesPrincipales(col, plantilla);

                if (cantComponentesPrincipales > 0)
                    coleccionesByUserConComponentePpal.Add(col);
            }

            return coleccionesByUserConComponentePpal;
        }

        public int GetCantComponentesPrincipales(Coleccion componentesByColeccionId, Plantilla plant)
        {
            //ESTE METODO SIRVE SOLAMENTE PARA DETERMINAR QUE COLECCIONES SE MUESTRAN EN EL COMBO.
            var cantComponentesPrincipales = 0;
            var layersPrincipales = new List<Layer>();

            layersPrincipales = plant.Layers.Where(l => l.Categoria == 1).ToList();

            foreach (Layer lay in layersPrincipales)
            {
                var compPpales = componentesByColeccionId.Componentes.Where(x => x.ComponenteId == lay.ComponenteId).ToList();
                //LAS COLECCIONES QUE NO TIENEN COMPONENTE PPAL DEL LAYER
                //NO VAN A APARECER EN EL COMBO
                if (compPpales != null && compPpales.Count > 0)
                {
                    //AUN CUANDO EL COMPONENTE PPAL DE ESTE LAYER, APAREZCA MAS DE UNA VEZ EN LA COLECCION
                    //, SERA CONSIDERADO COMO SI EXISTIERA SOLO UNA UNICA VEZ EN ESTA COLECCION
                    //
                    Session["idCompPpalPlotGral"] = lay.ComponenteId;
                    cantComponentesPrincipales++;
                }

            }

            return cantComponentesPrincipales;
        }

        #endregion

        #region COMPONENTES PRINCIPALES (PLANTILLA PREDEFINIDA)
        public ActionResult GetComponentesFiltrados(string tipo, string filtro)
        {
            tipo = "manzanas";
            filtro = filtro.ToLower();
            var values = new object();

            KeyValuePair<int, string>[] Localidades = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(1, "quilmes"),
                new KeyValuePair<int, string>(2, "bernal"),
                new KeyValuePair<int, string>(3, "berazategui"),
                new KeyValuePair<int, string>(4, "dominico"),
                new KeyValuePair<int, string>(5, "quilmes")
            };

            KeyValuePair<int, string>[] Partidos = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(1, "quilmes"),
                new KeyValuePair<int, string>(2, "la plata"),
                new KeyValuePair<int, string>(3, "san isidro"),
                new KeyValuePair<int, string>(4, "tres de febrero"),
                new KeyValuePair<int, string>(5, "avellaneda")
            };

            //KeyValuePair<int, string>[] Manzanas = new KeyValuePair<int, string>[]
            //{
            //    new KeyValuePair<int, string>(1, "manzana 2321"),
            //    new KeyValuePair<int, string>(2, "manzana 5434"),
            //    new KeyValuePair<int, string>(3, "manzana 7665"),
            //    new KeyValuePair<int, string>(4, "manzana 3286"),
            //    new KeyValuePair<int, string>(5, "manzana 8464")
            //};

            KeyValuePair<int, string>[] Manzanas = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(958670, "manzana prueba ok"),
                new KeyValuePair<int, string>(958671, "manzana2 prueba2 ok"),
                new KeyValuePair<int, string>(958672, "manzana3 prueba3 ok"),
                new KeyValuePair<int, string>(958673, "manzana4 prueba4 ok")
            };

            KeyValuePair<int, string>[] Parcelas = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(279, "parcela 1"),
                new KeyValuePair<int, string>(280, "parcela 2"),
                new KeyValuePair<int, string>(281, "parcela 3"),
                new KeyValuePair<int, string>(282, "parcela 4"),
                new KeyValuePair<int, string>(283, "parcela 5")
            };

            //Parcelas.
            switch (tipo.ToLower())
            {
                case "partido":
                    values = Partidos.Where(element => element.Value.StartsWith(filtro, StringComparison.Ordinal)).Select(r => new { id = r.Key, descripcion = r.Value }).ToList();
                    break;
                case "localidad":
                    values = Localidades.Where(element => element.Value.StartsWith(filtro, StringComparison.Ordinal)).Select(r => new { id = r.Key, descripcion = r.Value }).ToList();
                    break;
                case "manzanas":
                    values = Manzanas.Where(element => element.Value.StartsWith(filtro, StringComparison.Ordinal)).Select(r => new { id = r.Key, descripcion = r.Value }).ToList();
                    break;
                case "parcelas":
                    values = Parcelas.Where(element => element.Value.StartsWith(filtro, StringComparison.Ordinal)).Select(r => new { id = r.Key, descripcion = r.Value }).ToList();
                    break;
            }


            return Json(values, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region TEXTOS VARIABLES (PLANTILLA GENERAL)
        public ActionResult GetTextosVariables() //long id
        {
            var plantillaTextos = this.Plantilla.PlantillaTextos ?? new PlantillaTexto[0];

            return PartialView("~/Views/Ploteo/Partial/_TextosVariablesGeneral.cshtml", plantillaTextos.Where(t => t.Tipo == 2));
        }
        #endregion

        #region GENERAR PLOTEO PREDEFINIDO
        [HttpPost]
        public ActionResult GenerarPloteoPredefinido(long idComponentePrincipal, string idsObjetoGraf, int idImagenSatelital, int transparenciaPorc)
        {
            //Array con ids de objetos a graficar

            string[] listaObjetosGraficar = JsonConvert.DeserializeObject<string[]>(idsObjetoGraf);
            try
            {
                var lisMaximonumPloteos = GetLisMaximoNumePloteos(listaObjetosGraficar);
                if (lisMaximonumPloteos != null)
                {
                    //GENERAR ARCHIVO PDF
                    var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);

                    int idPlantillaFondo = plantillaFondo != null ? plantillaFondo.IdPlantillaFondo : 0;

                    byte[] bytes = GenerarPloteoPDFManzanasUOtros(idComponentePrincipal, lisMaximonumPloteos, idPlantillaFondo, string.Empty, string.Empty, false, null, null, null, idImagenSatelital, transparenciaPorc);

                    if (bytes == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                    }
                    //AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionPloteoPredefinido, "", "", "", lisMaximonumPloteos.Length);
                    zipFile = File(bytes, "application/pdf", string.Format("PloteoPredefinido_{0}.pdf", DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")));
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoPredefinido", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

        }
        #endregion

        #region GENERAR PLOTEO VISTA ACTUAL
        [HttpPost]
        public ActionResult GenerarPloteoVistaActual(int idPlantillaFondo, string textosVariables, string extent, string scale, string layersVisibles, int idImagenSatelital, int transparenciaPorc)
        {
            try
            {
                idPlantillaFondo = 0;

                var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);

                if (plantillaFondo != null)
                {
                    idPlantillaFondo = plantillaFondo.IdPlantillaFondo;
                }
                float imagenTransparencia = (float)(transparenciaPorc / 100.0);
                string parametros = string.Format("id={0}&idPlantillaFondo={1}&textosVariables={2}&extent={3}&scale={4}&layersVisibles={5}&idImagenSatelital={6}&imagenTransparencia={7}",
                                                   this.Plantilla.IdPlantilla, idPlantillaFondo, textosVariables, extent, scale, layersVisibles, idImagenSatelital, imagenTransparencia);

                HttpResponseMessage resp = _cliente.GetAsync("api/ModuloPloteo/GetPlantillaVistaActual?" + parametros).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(resp.StatusCode);
                }
                byte[] bytes = resp.Content.ReadAsAsync<byte[]>().Result;

                if (bytes == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                }

                AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionPloteoGeneral);
                zipFile = File(bytes, "application/pdf", string.Format("PloteoGeneral_{0}.pdf", DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")));
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoVistaActual", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Informe Anual
        public ActionResult InformeAnual()
        {
            //var listaPlantillas = getPlantillas(); //trae toda la lista de plantillas

            this.Periodos = GetAllPeriodo();
            this.TipoPlanos = GetAllTipoPlano();
            //this.PartidosApicRedes = GetAllPartidoApicRedes();
            this.Partidos = this.GetAllPartido().Where(p => p.Prestac == 1).ToList();

            ViewBag.Periodos = this.Periodos;
            //ViewBag.TipoPlanos = this.TipoPlanos;
            ////ViewBag.Partidos = this.PartidosApicRedes;
            //ViewBag.Partidos = this.Partidos;
            ViewBag.TipoPlanos = new List<TipoPlano>();
            ViewBag.Partidos = new List<Partido>();

            return PartialView("~/Views/InformeAnual/InformeAnual.cshtml");
        }

        //public ActionResult GetAlertaValidac(string nombreColeccion, string idComponente, string idObjetos, string msgAlerta)
        public ActionResult GetAlertaValidac()
        {
            //ViewBag.nombreColeccion = nombreColeccion;
            //ViewBag.idComponente = idComponente;
            //ViewBag.idObjetos = idObjetos;
            //ViewBag.MsgAlerta = msgAlerta;
            //return PartialView("~/Views/InformeAnual/Partial/_AlertaValidacPartialView.cshtml", this.Plantilla);
            return PartialView("~/Views/InformeAnual/Partial/_AlertaValidacPartialView.cshtml");
        }

        public ActionResult GetNombreColeccionPartialView()
        {
            return PartialView("~/Views/InformeAnual/Partial/_NombreColeccionPartialView.cshtml");
        }

        #endregion

        [HttpPost]
        public string GenerarPloteoMapaTematico(int idPlantilla, string idDistrito, string extents, int idImagenSatelital, int transparenciaPorc, bool verIdentificante, long? idComponentePrincipal, long? idComponenteTematico, bool verContexto)
        {

            byte[] bytes;
            try
            {
                int idPlantillaFondo = 0;
                var plantilla = GetPlantilla(idPlantilla);
                var plantillaFondo = plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == idPlantilla && !pf.FechaBaja.HasValue);
                if (plantillaFondo != null)
                {
                    idPlantillaFondo = plantillaFondo.IdPlantillaFondo;
                }

                var doc = new it.Document();
                var memStream = new MemoryStream();
                var writer = new PdfCopy(doc, memStream);
                doc.Open();


                MapaTematicoModel mapaTematicoModel = ((MapaTematicoModel)Session["MapaTematico"]);
                string guid = Session["GUID"].ToString();
                var mtController = new MapasTematicosController();
                mtController.ControllerContext = new System.Web.Mvc.ControllerContext();
                mtController.ControllerContext.HttpContext = HttpContext;
                ObjetoResultadoDetalle objetoResultadoDetalle = (Session["objetoResultadoDetalle"] as ObjetoResultadoDetalle) ??
                                                                    mtController.GetObjetoResultadoDetalle(mapaTematicoModel.Visualizacion);
                objetoResultadoDetalle.GUID = guid;
                objetoResultadoDetalle.NombreMT = mapaTematicoModel.Nombre;
                objetoResultadoDetalle.GeometryType = (int?)Session["Geometry_Type"];

                extents = extents.Replace(";", ",");

                string textosVariables = string.Empty;
                int mtPredefinido = (Session["MT_Predefinido"] == null ? 0 : (int)Session["MT_Predefinido"]);
                if (mtPredefinido == 1)
                {
                    int idAnalisis = (Session["IdAnalisis"] == null ? 0 : Convert.ToInt32(Session["IdAnalisis"]));
                    AnalisisTecnicos analisisTecnico = GetAnalisisTecnicosById(idAnalisis);
                    int idPredefinido = (Session["PredefinidoId"] == null ? 0 : Convert.ToInt32(Session["PredefinidoId"]));
                    Predefinido predefinido = GetPredefinidoById(idPredefinido);
                    if (analisisTecnico != null)
                    {
                        textosVariables += "TITULO," + analisisTecnico.Nombre + ";";
                    }
                    if (predefinido != null)
                    {
                        if (idAnalisis == 0)
                        {
                            textosVariables += "TITULO," + predefinido.Descripcion + ";";
                        }
                        else
                        {
                            textosVariables += "SUBTITULO," + predefinido.Descripcion + ";";
                        }
                    }
                    UsuariosModel usuarioConectado = (UsuariosModel)Session["usuarioPortal"];
                    if (usuarioConectado != null)
                    {
                        textosVariables += "SECTOR," + usuarioConectado.Sector + ";";
                    }
                    if (textosVariables != string.Empty)
                    {
                        textosVariables = textosVariables.Substring(0, textosVariables.Length - 1);
                    }

                }

                float imagenTransparencia = (float)(transparenciaPorc / 100.0);
                string parametros = string.Format("idPlantilla={0}&idPlantillaFondo={1}&extent={2}&textosVariables={3}&idDistrito={4}&idImagenSatelital={5}&imagenTransparencia={6}&verIdentificante={7}&idComponentePrincipal={8}&verContexto={9}&idComponenteTematico={10}", idPlantilla, idPlantillaFondo, extents, textosVariables, idDistrito, idImagenSatelital, imagenTransparencia, verIdentificante, idComponentePrincipal, verContexto, idComponenteTematico);
                var content = new ObjectContent<ObjetoResultadoDetalle>(objetoResultadoDetalle, new JsonMediaTypeFormatter());
                var resp = _cliente.PostAsync("api/ModuloPloteo/GetPlantillaMapaTematico?" + parametros, content).Result;

                resp.EnsureSuccessStatusCode();
                bytes = resp.Content.ReadAsAsync<byte[]>().Result;

                if (bytes != null)
                {
                    var reader = new PdfReader(bytes);
                    writer.AddDocument(reader);
                    reader.Close();
                }
                else
                {
                    bytes = null;
                }

                writer.Close();
                bytes = memStream.ToArray();
                memStream.Close();
                doc.Close();
            }
            catch (Exception ex)
            {
                return "Ha ocurrido un problema: " + ex.Message;
            }

            if (bytes != null)
            {
                AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GenerarPdf);
                var fileName = "PloteoMapaTematico_{0}.pdf";
                var contentType = "application/pdf";
                zipFile = File(bytes, contentType, string.Format(fileName, DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")));
            }
            else
            {
                return "<script language='javascript' type='text/javascript'>alert('El documento no contiene paginas');</script>";
            }
            return "ok";
        }

        private AnalisisTecnicos GetAnalisisTecnicosById(long id)
        {
            HttpResponseMessage respColecciones = _cliente.GetAsync("api/CargaTecnica/GetAnalisisTecnicoPorId?analisisId=" + id).Result;
            respColecciones.EnsureSuccessStatusCode();
            return respColecciones.Content.ReadAsAsync<AnalisisTecnicos>().Result;
        }

        public Predefinido GetPredefinidoById(long idPredefinido)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/MapasTematicosService/GetPredefinidoById/" + idPredefinido).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                return (Predefinido)resp.Content.ReadAsAsync<Predefinido>().Result;
            }
            catch (Exception ex)
            {
                string msgErr = resp.ReasonPhrase;
                return null;
            }
        }

        #region GENERAR PLOTEO PLANCHETA A4 POR COLECCION
        [HttpPost]
        /*aysa string numeroODT*/
        public ActionResult GenerarPloteoPlanchetaA4ByColeccion(long idColeccion, bool ordenarPorExpediente, bool ordenarPorManzana, bool grabarListado, bool isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc, string numeroODT)
        {
            List<ComponenteModel> listComponentes = new List<ComponenteModel>();
            List<ObjetoPloteableGenerico> listObjetosPloteables = new List<ObjetoPloteableGenerico>();

            var coleccion = GetColeccionById(idColeccion);

            foreach (ColeccionComponente coleccionComponente in coleccion.Componentes)
            {
                var componente = GetComponenteIfNotExists(ref listComponentes, coleccionComponente.ComponenteId);
                if (componente != null)
                {
                    ObjetoPloteableGenerico objetoPloteable = new ObjetoPloteableGenerico();
                    objetoPloteable.Id = coleccionComponente.ObjetoId.ToString();
                    objetoPloteable.Componente = componente.DocType;

                    listObjetosPloteables.Add(objetoPloteable);
                }
            }
            /*AYSA , numeroODT */
            return GenerarPloteoPlanchetaA4(listObjetosPloteables, ordenarPorExpediente, ordenarPorManzana, grabarListado, isManzanaDuplicadaCheck, verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc, numeroODT);
        }

        private ComponenteModel GetComponenteIfNotExists(ref List<ComponenteModel> componentes, long idComponente)
        {
            List<ComponenteModel> listComponentes = componentes;

            ComponenteModel componente = componentes.FirstOrDefault(c => c.ComponenteId == idComponente);

            if (componente == null)
            {
                componente = GetComponentesById(idComponente);
                componentes.Add(componente);
            }

            return componente;
        }

        public byte[] ProcesarParcelasAgrupadasPorManzana(long idComponentePrincipal, List<KeyValuePair<long, long>> listaDeParesMaximoNumPloteos, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {
            //CREAR UNA NUEVA LISTA DE PARES(MANZANA,PARCELA), AGRUPADO POR MANZANAS
            //EL DICCIONARIO RESULTADO SERA MENOR O IGUAL EN CANT DE ELEMENTOS
            //QUE EL ARRAY ORIGINAL
            //SOBRESCRIBE Y REUTILIZA EL MISMO LISTADO

            //UTILIZA LA LISTA DE PARES CON LA NUEVA LISTA DE PARES, AGRUPADO POR MANZANAS
            //HEREDARA EL ORDEN ESTABLECIDO ANTERIORMENTE
            return PrepararPDFParcelasAgrupadasPorManzana(AgruparParcelasByManzana(listaDeParesMaximoNumPloteos), idComponentePrincipal, "", verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);

        }

        public List<KeyValuePair<long, long>> OrdenarPorParcelaOManzana(List<string> idObjetosGraficar, bool ordenarPorManzana, bool ordenarPorExpediente, bool IsColeccionA4)
        {
            List<KeyValuePair<long, long>> listadoManzanasByParcelaID = new List<KeyValuePair<long, long>>();

            if (IsColeccionA4)
            {
                #region EsColeccionA4
                if (ordenarPorManzana)
                {
                    //SE ASUME QUE idObjetosGraficar SON ID DE PARCELAS. 
                    //esApic es false, ENTONCES OBTENGO LA LISTA DE MANZANAS-PARCELAS, A PARTIR DE 
                    //UNA LISTA DE ID PARCELAS
                    listadoManzanasByParcelaID = GetManzanasByIds(idObjetosGraficar.ToArray(), false);

                    var manzList = from l in listadoManzanasByParcelaID select l.Value.ToString();
                    List<string> lstManzApicId = GetLisManzanasAPICIDsByManzanasID(manzList.ToArray());
                    List<string> lstManzApicIdOrder = lstManzApicId.OrderBy(o => o).ToList();

                    List<string> lstManzIdOrder = GetManzanasByLisManzanasAPICIDs(lstManzApicIdOrder.ToArray());
                    var orderedByIDList = from i in lstManzIdOrder.Distinct()
                                          join o in listadoManzanasByParcelaID
                                          on i equals o.Value.ToString()
                                          select o;

                    listadoManzanasByParcelaID = orderedByIDList.ToList();
                }
                else
                {
                    if (ordenarPorExpediente)
                    {
                        List<string> lsParcelasAPICByParcelasID = new List<string>();
                        //SE ASUME QUE idObjetosGraficar SON PARCELASID.
                        //LUEGO OBTENGO LA LISTA DE APIC_ID_PARCELAS CON GETPARCELA_APICID_BYPARCELAID
                        lsParcelasAPICByParcelasID = GetParcela_APICID_ByParcelaID(idObjetosGraficar);

                        //ORDENO  LISTA DE APIC_ID EN FORMA ASCENDENTE
                        lsParcelasAPICByParcelasID = lsParcelasAPICByParcelasID.OrderBy(o => o).ToList();

                        //LUEGO OBTENGO LA LISTA DE PARES(MANZANAID,PARCELAID) CON GetManzanasByIds
                        //LA LISTA QUEDA AUTOMATICAMENTE ORDENADA POR APIC ID DE PARCELAS
                        //EL VALOR esApic=true LLAMARA AL PROCEDURE QUE TRAE LISTA DE PARCELASID
                        //DADA UNA LISTA DE PARCELAS_APICID
                        //listadoManzanasByParcelaID = GetManzanasByIds(lsParcelasAPICByParcelasID.ToArray(), true);
                        listadoManzanasByParcelaID = GetManzanasByParcelasApicIds(lsParcelasAPICByParcelasID.ToArray());
                    }
                    else
                    {
                        //SE ASUME QUE SON PARCELAS ID
                        //SI NO PIDE ORDEN POR MANZANA O POR EXPEDIENTE, PIDE DEJAR EL ORDEN ORIGINAL, TAL
                        //COMO ESTA EN EL ARCHIVO EXCEL
                        listadoManzanasByParcelaID = GetManzanasByIds(idObjetosGraficar.ToArray(), true);
                    }

                }
                #endregion
            }
            else
            {
                #region EsListadoExpediente
                //SI NO ES COLECCION A4, ENTONCES ES LISTADO (EXPEDIENTE)
                if (ordenarPorManzana)
                {
                    //SE ASUME QUE idObjetosGraficar SON APIC_ID DE PARCELAS. 
                    //LUEGO OBTENGO LA LISTA DE PARES(MANZANAID,PARCELAID) CON GETMANZANASBYPARCELAID
                    listadoManzanasByParcelaID = GetManzanasByIds(idObjetosGraficar.ToArray(), true);

                    //ORDENAR LISTA DE PARES POR MANZANA(Value)
                    listadoManzanasByParcelaID = listadoManzanasByParcelaID.OrderBy(o => o.Value).ToList();
                }
                else
                {
                    if (ordenarPorExpediente)
                    {
                        //ORDENO  LISTA DE APIC_ID EN FORMA ASCENDENTE
                        idObjetosGraficar = idObjetosGraficar.OrderBy(o => o).ToList();

                        //LUEGO OBTENGO LA LISTA DE PARES(MANZANAID,PARCELAID) CON GetManzanasByIds
                        //LA LISTA QUEDA AUTOMATICAMENTE ORDENADA POR APIC ID DE PARCELAS
                        //EL VALOR esApic=true LLAMARA AL PROCEDURE QUE TRAE LISTA DE PARCELASID
                        //DADA UNA LISTA DE PARCELAS_APICID
                        listadoManzanasByParcelaID = GetManzanasByIds(idObjetosGraficar.ToArray(), true);
                    }
                    else
                    {
                        //SI NO PIDE ORDEN POR MANZANA O POR EXPEDIENTE, PIDE DEJAR EL ORDEN ORIGINAL, TAL
                        //COMO ESTA EN EL ARCHIVO EXCEL
                        listadoManzanasByParcelaID = GetManzanasByIds(idObjetosGraficar.ToArray(), true);
                    }

                }
                #endregion
            }

            return listadoManzanasByParcelaID;
        }

        private List<string> GetParcela_APICID_ByParcelaID(List<string> idObjetosGraficar)
        {
            //VIENE DE LINEA 572
            HttpResponseMessage respAPICID = _cliente.PostAsync<string[]>("api/PloteoService/GetParcela_APICID_ByParcelaID", idObjetosGraficar.ToArray(), new JsonMediaTypeFormatter()).Result;
            respAPICID.EnsureSuccessStatusCode();
            var listadoAPICParcelaByParcelaID = (List<string>)respAPICID.Content.ReadAsAsync<List<string>>().Result;

            return listadoAPICParcelaByParcelaID;
        }

        //private List<string> GetManzana_APICID_ByManzanaID(List<string> idsManzana)
        //{
        //    //VIENE DE LINEA 572
        //    HttpResponseMessage respAPICID = _cliente.PostAsync<string[]>("api/PloteoService/GetManzana_APICID_ByManzanaID", idsManzana.ToArray(), new JsonMediaTypeFormatter()).Result;
        //    respAPICID.EnsureSuccessStatusCode();
        //    var listadoAPICParcelaByParcelaID = (List<string>)respAPICID.Content.ReadAsAsync<List<string>>().Result;

        //    return listadoAPICParcelaByParcelaID;
        //}

        private byte[] ProcesarManzanas(long idComponentePrincipal, string[] idsObjetosAGraficar, bool IsColeccionA4, bool ordenarPorManzana, bool verCotas, ref List<string> lstManzPlotear, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {
            //VENGO DE LINEA 1035/ 456
            byte[] bytesPdf = null;
            var lisManzanasFromAPIC_ID = new List<string>();
            var lisManzanasFromAPIC_ID3 = new List<string>();
            var lisAPICIDManzanasFromLisManzanasID = new List<string>();
            var lisManzanasFromAPIC_ID2 = new List<string>();

            try
            {
                //DETERMINO SI LA LISTA TIENE UNA CANTIDAD DE OBJETOS PERMITIDA
                var lisMaximonumPloteos = GetLisMaximoNumePloteos(idsObjetosAGraficar);

                if (lisMaximonumPloteos != null && lisMaximonumPloteos.Count() > 0)
                {
                    if (!IsColeccionA4)
                    {
                        //PROCESANDO LISTADO MANZANAS(APIC_ID)
                        if (ordenarPorManzana)
                        {
                            //lisMaximonumPloteos ESTA FORMADO POR APIC_ID
                            //ORDENO POR APIC ID
                            lisManzanasFromAPIC_ID2 = lisMaximonumPloteos.OrderBy(i => i).ToList();

                            //OBTENGO LA LISTA DE MANZANAID A PARTIR DE APIC ID_MANZANAS
                            //(YA ORDENADOS POR APIC ID MANZANA)
                            lisManzanasFromAPIC_ID = GetManzanasByLisManzanasAPICIDs(lisManzanasFromAPIC_ID2.ToArray());
                        }
                        else
                        {
                            //CONSERVA EL ORDEN APIC ID ORIGINAL
                            lisManzanasFromAPIC_ID = GetManzanasByLisManzanasAPICIDs(lisMaximonumPloteos);

                            //OBTENGO LA LISTA DE MANZANAID A PARTIR DE APIC ID_MANZANAS
                            //(CON EL ORDEN ORIGINAL DEL LISTADO)
                            //lisManzanasFromAPIC_ID = GetManzanasByLisManzanasAPICIDs(lisManzanasFromAPIC_ID3.ToArray());
                        }

                    }
                    else
                    {
                        //PROCESANDO COLECCIONES A4 (MANZANAID)
                        if (ordenarPorManzana)
                        {
                            //lisMaximonumPloteos ESTA FORMADO POR MANZANAID
                            //POR SER UNA LISTA DE MANZANA ID, SE REALIZA EL PROCESO OPUESTO AL ANTERIOR
                            //OBTENGO LA LISTA DE APIC_ID_MANZANAS A PARTIR DE MANZANAID
                            lisAPICIDManzanasFromLisManzanasID = GetLisManzanasAPICIDsByManzanasID(lisMaximonumPloteos);

                            //ORDENO POR APIC ID DE MANZANA
                            //OBTENGO LA LISTA DE MANZANAID A PARTIR DE APIC ID_MANZANAS
                            //(YA ORDENADOS POR APIC ID MANZANA)
                            lisManzanasFromAPIC_ID = GetManzanasByLisManzanasAPICIDs(lisAPICIDManzanasFromLisManzanasID.OrderBy(t => t).ToArray());
                        }
                        //else: lisMaximonumPloteos ESTA FORMADO POR MANZANAID Y SE DEJA EL ORDEN COMO
                        //VINO DEL COMBO COLECCION
                    }

                    //PREPARO PLANTILLA FONDO PARA ENVIAR COMO PARAMETRO

                    var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);

                    int idPlantillaFondo = plantillaFondo != null ? plantillaFondo.IdPlantillaFondo : 0;

                    if (!IsColeccionA4)
                    {
                        //LISTADO MANZANA: CREO EL ARCHIVO PDF
                        bytesPdf = GenerarPloteoPDFManzanasUOtros(idComponentePrincipal, lisManzanasFromAPIC_ID.ToArray(), idPlantillaFondo, string.Empty, string.Empty, verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
                        lstManzPlotear = lisManzanasFromAPIC_ID;
                    }
                    else
                    {
                        //COLECCION A4: CREO EL ARCHIVO PDF
                        if (ordenarPorManzana)
                        {
                            bytesPdf = GenerarPloteoPDFManzanasUOtros(idComponentePrincipal, lisManzanasFromAPIC_ID.ToArray(), idPlantillaFondo, string.Empty, string.Empty, verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
                            lstManzPlotear = lisManzanasFromAPIC_ID;
                        }
                        else
                        {
                            bytesPdf = GenerarPloteoPDFManzanasUOtros(idComponentePrincipal, lisAPICIDManzanasFromLisManzanasID.ToArray(), idPlantillaFondo, string.Empty, string.Empty, verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
                            lstManzPlotear = lisAPICIDManzanasFromLisManzanasID;
                        }
                    }

                    return bytesPdf;
                }
                else
                {
                    //SUPERA EL MAXIMO CARTELITO O REDIRECT
                    throw new Exception("Supera el maximo permitido de objetos a plotear.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<string> GetLisManzanasAPICIDsByManzanasID(string[] lisMaximonumPloteos)
        {
            //VIENE DE LINEA 629
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetLisManzanasAPICIDsByManzanasID", lisMaximonumPloteos, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listadoAPICIDManzanasByIDOfManzana = (List<string>)respColecciones.Content.ReadAsAsync<List<string>>().Result;

            return listadoAPICIDManzanasByIDOfManzana;
        }

        public Dictionary<long, string> AgruparParcelasByManzana(List<KeyValuePair<long, long>> listadoManzanasByParcelaID)
        {
            //Ejemplo Lista input:
            //Key, Value
            //25,  1326587
            //5,   100205
            //36,  1326587
            //205, 100205
            //57,  1326587
            //190, 100205
            //14,  1326587

            //Ejmplo Lista output:
            //lisParcelasAgrupadasByManzana[0].Key= 1326587;
            //lisParcelasAgrupadasByManzana[0].Value= "25,36,14,57";

            Dictionary<long, List<long>> lisAgruparParcelasAsLong = new Dictionary<long, List<long>>();
            Dictionary<long, string> lisAgruparParcelasAsString = new Dictionary<long, string>();

            foreach (var u in listadoManzanasByParcelaID)
            {
                if (!lisAgruparParcelasAsLong.ContainsKey(u.Value))
                    lisAgruparParcelasAsLong.Add(u.Value, new List<long>());
                lisAgruparParcelasAsLong[u.Value].Add(u.Key);
            }

            foreach (var g in lisAgruparParcelasAsLong.Keys)
            {
                lisAgruparParcelasAsString.Add(g, ConvertirListaLongsAString(lisAgruparParcelasAsLong[g]));
            }

            return lisAgruparParcelasAsString;

        }

        public string ConvertirListaLongsAString(List<long> list)
        {
            String res = String.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i].ToString();
                if (i != list.Count - 1) res += ",";
            }

            return res;
        }

        public byte[] GenerarPDFParcelas(List<KeyValuePair<long, long>> listadoManzanasByParcelaID)
        {
            throw new NotImplementedException();
        }

        private string[] ObtenerManzanasParcelas(long idComponenteParcela, long idComponenteManzana, long idColeccion, out long tipo)
        {
            /* SI TIENE OBJETOS DE TIPO DE COMPONENTE MANZANA, SE GRAFICAN INDEPENDIENTEMENTE DE SI TIENEN PARCELAS O NO */
            tipo = idComponenteManzana;
            var coleccion = GetColeccionById(idColeccion);

            var idsObjetosAGraficar = coleccion.Componentes.Where(c => c.ComponenteId == idComponenteManzana).Select(c => c.ObjetoId.ToString()).ToArray();

            if (idsObjetosAGraficar.Any())
                tipo = idComponenteManzana;


            if (!idsObjetosAGraficar.Any())
            {
                idsObjetosAGraficar = coleccion.Componentes.Where(c => c.ComponenteId == idComponenteParcela).Select(c => c.ObjetoId.ToString()).ToArray();
                if (idsObjetosAGraficar.ToList().Count > 0)
                    tipo = idComponenteParcela;
            }

            return idsObjetosAGraficar;
        }

        public byte[] GenerarListadoTxt(string[] componentes)
        {
            //modificado por revision de codigo (borrar codigo comentado en un futur0)
            //byte[] streamTXT;
            //var memString = "";

            //for (int j = 0; j < componentes.Length; j++)
            //{
            //    var p = j + 1;
            //    if (p == componentes.Length)
            //        memString += componentes[j] + "\r\n";
            //    else
            //        memString += componentes[j] + "\r\n";
            //}

            //streamTXT = Encoding.ASCII.GetBytes(memString);
            //MemoryStream ms = new MemoryStream(streamTXT);

            //return streamTXT;
            return Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, componentes) + Environment.NewLine);
        }

        public byte[] GenerarArchivoZip(byte[] streamPDF, byte[] streamTXT)
        {
            using (var zipFile = new ZipFile())
            using (var memStr = new MemoryStream())
            {
                zipFile.AddEntry("PloteoPlanchetaA4.pdf", streamPDF);
                zipFile.AddEntry("ListadoA4PorColeccion.txt", streamTXT);
                zipFile.Save(memStr);
                return memStr.GetBuffer();
            }
        }

        #endregion POR COLECCION

        [HttpPost]
        /*AYSA , string numeroODT*/
        public ActionResult GenerarPloteoPlanchetaA4ByManzanaZonaResultado(string idsObjetoGraf, bool ordenarPorExpediente, bool ordenarPorManzana, bool grabarListado, bool isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc, string numeroODT)
        {
            List<ObjetoPloteableGenerico> listaObjetosPloteables = new List<ObjetoPloteableGenerico>();
            try
            {
                listaObjetosPloteables = JsonConvert.DeserializeObject<List<ObjetoPloteableGenerico>>(idsObjetoGraf);
            }
            catch (Exception) { }
            /*AYSA NUMEROODT*/
            return GenerarPloteoPlanchetaA4(listaObjetosPloteables, ordenarPorExpediente, ordenarPorManzana, grabarListado, isManzanaDuplicadaCheck, verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc, numeroODT);
        }

        [HttpGet]
        public JsonResult GetJsonPlanchetaA4(long idPlantilla, string manzana, string expedienteCatastro, string expedienteReferencial, string expedienteSAP, string cuentaContrato, string textos, string numeroODT)
        {
            PlanchetaA4Response planchetaA4 = new PlanchetaA4Response();

            var maximoObjetosPloteables = 1;

            try
            {
                this.Plantilla = GetPlantilla(idPlantilla);

                List<ObjetoPloteableGenerico> listaObjetos = new List<ObjetoPloteableGenerico>();

                //Proceso la manzana
                if (!string.IsNullOrEmpty(manzana))
                {
                    var idManzana = GetManzanasByLisManzanasAPICIDs(new string[] { manzana }).FirstOrDefault();
                    if (idManzana == null)
                    {
                        throw new Exception("No se reconoce la manzana seleccionada");
                    }

                    listaObjetos.Add(new ObjetoPloteableGenerico()
                    {
                        Componente = "manzanas",
                        Id = idManzana
                    });
                }

                //Proceso los expedienteCatastro
                if (!string.IsNullOrEmpty(expedienteCatastro))
                {
                    string[] expendientesCatastro = expedienteCatastro.Split(',');
                    List<string> parcelasIds = GetParcelasByParcelasApicIds(expendientesCatastro);
                    if (parcelasIds.Count != expendientesCatastro.Count())
                    {
                        throw new Exception("No se pudieron procesar todos los expedientes catastro");
                    }

                    listaObjetos.AddRange(parcelasIds.Select(c => new ObjetoPloteableGenerico()
                    {
                        Componente = "parcelas",
                        Id = c
                    }));

                }

                //Proceso los expedienteReferencial
                if (!string.IsNullOrEmpty(expedienteReferencial))
                {
                    string[] expedientesReferencial = expedienteReferencial.Split(',');
                    List<string> parcelasIds = GetParcelasByParcelasExpRef(expedientesReferencial);
                    if (parcelasIds.Count != expedientesReferencial.Count())
                    {
                        throw new Exception("No se pudieron procesar todos los expedientes referencial");
                    }

                    listaObjetos.AddRange(parcelasIds.Select(c => new ObjetoPloteableGenerico()
                    {
                        Componente = "parcelas",
                        Id = c
                    }));

                }

                //Proceso los expedienteSAP
                if (!string.IsNullOrEmpty(expedienteSAP))
                {
                    string[] expedientesSAP = expedienteSAP.Split(',');
                    List<string> parcelasIds = GetParcelasByParcelasSapID(expedientesSAP);
                    if (parcelasIds.Count != expedientesSAP.Count())
                    {
                        throw new Exception("No se pudieron procesar todos los expedientes sap");
                    }

                    listaObjetos.AddRange(parcelasIds.Select(c => new ObjetoPloteableGenerico()
                    {
                        Componente = "parcelas",
                        Id = c
                    }));

                }

                //Proceso los expedienteSAP
                if (!string.IsNullOrEmpty(cuentaContrato))
                {
                    string[] cuentasContrato = cuentaContrato.Split(',');
                    List<string> parcelasIds = GetParcelasByParcelasCC(cuentasContrato);
                    if (parcelasIds.Count != cuentasContrato.Count())
                    {
                        throw new Exception("No se pudieron procesar todas las cuentas contrato");
                    }

                    listaObjetos.AddRange(parcelasIds.Select(c => new ObjetoPloteableGenerico()
                    {
                        Componente = "parcelas",
                        Id = c
                    }));

                }

                string textosVariables = null;
                if (!string.IsNullOrEmpty(textos))
                {
                    try
                    {
                        List<JProperty> jsonTextos = ((JContainer)(JsonConvert.DeserializeObject(textos))).ToList().Cast<JProperty>().ToList();
                        textosVariables = string.Join(";", jsonTextos.Select(t => t.Name + "," + t.Value));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Se produjo un error al parsear los textos");
                    }
                }

                try
                {
                    // EEGR 20190503 - Cotas en plancheta

                    var result = GenerarPloteoPlanchetaA4(listaObjetos, false, true, false, true, false, null, null, null, maximoObjetosPloteables, textosVariables, 0, 0, numeroODT);

                    if (result is HttpStatusCodeResult)
                    {
                        planchetaA4.Result = false;
                        planchetaA4.Message = ((HttpStatusCodeResult)result).StatusDescription;

                    }
                    else
                    {
                        planchetaA4.PDF = zipFile.FileContents;
                        planchetaA4.Result = true;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Se produjo un error al generar la plancheta");
                }
            }
            catch (Exception e)
            {
                planchetaA4.Message = e.Message;
                planchetaA4.Result = false;
            }

            return Json(planchetaA4, JsonRequestBehavior.AllowGet);
        }

        #region GENERAR PLOTEO PLANCHETA A4 POR ARCHIVO

        [HttpPost]
        public ActionResult GenerarPloteoPlanchetaA4ByFile(string content, string fileExt, string tipoGrafico, string isManzanaDuplicada, long idComponentePrincipal, bool ordenarPorExpediente, bool ordenarPorManzana, bool grabarListado, bool isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {
            try
            {
                var groups = System.Text.RegularExpressions.Regex.Match(content, @"data:(?<type>.+?)/(?<mime>.+?),(?<data>.+)").Groups;
                var data = Convert.FromBase64String(groups["data"].Value);
                //IDENTIFICAR EL TIPO DE ARCHIVO QUE SE VA A PROCESAR:
                if (data.Length > 0)
                {
                    var bytes = GenerarPloteoFromFile(data, tipoGrafico, fileExt, idComponentePrincipal, ordenarPorExpediente, ordenarPorManzana, grabarListado, isManzanaDuplicadaCheck, verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
                    if (bytes == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                    }
                    var fileName = "PloteoPlanchetaA4_{0}.pdf";
                    var contentType = "application/pdf";
                    AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionPloteoPlanchetaA4);
                    zipFile = File(bytes, contentType, string.Format(fileName, DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")));
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoPlanchetaA4ByFile", ex);
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed, ex.Message);
            }
        }

        public ActionResult DownloadZip()
        {
            var result = zipFile;
            zipFile = null;
            return result;
        }

        #endregion

        #region GENERAR PLOTEO PLANCHETA A4 POR MANZANA ZONA RESULTADO
        /*AYSA NUMEROODT*/
        public ActionResult GenerarPloteoPlanchetaA4(List<ObjetoPloteableGenerico> listaObjetosPloteables, bool ordenarPorExpediente, bool ordenarPorManzana, bool grabarListado, bool isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, string numeroODT)
        {
            /*AYSA NUMEROODT*/
            return GenerarPloteoPlanchetaA4(listaObjetosPloteables, ordenarPorExpediente, ordenarPorManzana, grabarListado, isManzanaDuplicadaCheck, verCotas, grafico, leyenda, infoLeyenda, null, null, 0, 0, numeroODT);

        }
        /*AYSA NUMEROODT*/
        public ActionResult GenerarPloteoPlanchetaA4(List<ObjetoPloteableGenerico> listaObjetosPloteables, bool ordenarPorExpediente, bool ordenarPorManzana, bool grabarListado, bool isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc, string numeroODT)
        {
            return GenerarPloteoPlanchetaA4(listaObjetosPloteables, ordenarPorExpediente, ordenarPorManzana, grabarListado, isManzanaDuplicadaCheck, verCotas, grafico, leyenda, infoLeyenda, null, null, idImagenSatelital, transparenciaPorc, numeroODT);

        }
        /*AYSA NUMEROODT*/
        public ActionResult GenerarPloteoPlanchetaA4(List<ObjetoPloteableGenerico> listaObjetosPloteables, bool ordenarPorExpediente, bool ordenarPorManzana, bool grabarListado, bool isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int? maximoObjetosPloteables, string textosVariables, int idImagenSatelital, int transparenciaPorc, string numeroODT)
        {
            byte[] bytes = { };
            byte[] bytesTxt = { 0, 1 };
            var componentesGraficar = new List<string>();
            List<ObjetoPloteable> listaObjetosGraficar = new List<ObjetoPloteable>();
            string responseError = string.Empty;

            try
            {
                foreach (var objeto in listaObjetosPloteables)
                {

                    using (var respObjetoPloteable = _cliente.GetAsync("api/PloteoService/GetObjetoPloteable?idObjeto=" + objeto.Id + "&componente=" + objeto.Componente).Result)
                    {
                        respObjetoPloteable.EnsureSuccessStatusCode();
                        ObjetoPloteable objetoPloteable = respObjetoPloteable.Content.ReadAsAsync<ObjetoPloteable>().Result;

                        if (objetoPloteable != null)
                        {
                            listaObjetosGraficar.Add(objetoPloteable);
                        }
                    }
                }

                if (listaObjetosGraficar == null || listaObjetosGraficar.Count() == 0 || listaObjetosGraficar.Count() != listaObjetosPloteables.Count())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.LengthRequired);
                }

                if (listaObjetosGraficar != null && listaObjetosGraficar.Count() > 0)
                {

                    int idPlantillaFondo = 0;

                    var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);

                    if (plantillaFondo != null)
                    {
                        idPlantillaFondo = plantillaFondo.IdPlantillaFondo;
                    }

                    string idsObjetoSecundario = string.Empty;
                    textosVariables = textosVariables ?? string.Empty;

                    //Ya tengo la lista de objetos a graficar(Manzanas)

                    //Elimino las parcelas duplicadas
                    var Manzanas = listaObjetosGraficar.Where(a => a.idParcela == null).ToList();
                    var Parcelas = listaObjetosGraficar.Where(a => a.idParcela != null).GroupBy(o => o.idParcela).Select(group => group.First()).ToList();

                    listaObjetosGraficar = new List<ObjetoPloteable>();
                    listaObjetosGraficar.AddRange(Manzanas);
                    listaObjetosGraficar.AddRange(Parcelas);

                    if (ordenarPorExpediente)
                    {
                        listaObjetosGraficar = listaObjetosGraficar.OrderBy(o => o.apicIdParcela).ToList();//cambiar por id
                    }
                    else
                    {
                        listaObjetosGraficar = listaObjetosGraficar.OrderBy(o => o.apicIdManzana).ToList();//cambiar por id
                    }


                    List<ObjetoPloteable> listaObjetosAplotear = listaObjetosGraficar;
                    if (isManzanaDuplicadaCheck)
                    {
                        //Agrupo por idManzana
                        listaObjetosAplotear = listaObjetosAplotear.GroupBy(o => o.idManzana).Select(g => new ObjetoPloteable() { idManzana = g.First().idManzana, idParcela = String.Join(",", g.Where(s => !string.IsNullOrEmpty(s.idParcela)).Select(s => s.idParcela)) }).ToList();
                    }

                    var noExcede = GetLisMaximoNumePloteos(listaObjetosAplotear.Select(p => p.idManzana.ToString()).ToArray(), maximoObjetosPloteables) != null;

                    if (noExcede)
                    {
                        //listaObjetosAplotear = listaObjetosAplotear.OrderBy(p => p.idManzana).ToList();

                        bool hasPages = false;

                        byte[] bytesResult;

                        #region obtiene el pdf
                        long idComponente = Convert.ToInt64(GetParametrosGenerales().SingleOrDefault(p => p.Descripcion == "ID_COMPONENTE_MANZANA")?.Valor);

                        float imagenTransparencia = (float)(transparenciaPorc / 100.0);
                        string parametros = string.Format("id={0}&idComponenteObjetoGraf={1}&idPlantillaFondo={2}&textosVariables={3}&verCotas={4}&idImagenSatelital={5}&imagenTransparencia={6}&grafico={7}&leyenda={8}&infoLeyenda={9},&numeroODT={10}",
                                                               this.Plantilla.IdPlantilla, idComponente, idPlantillaFondo, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, JsonConvert.SerializeObject(grafico), JsonConvert.SerializeObject(leyenda), infoLeyenda, numeroODT);

                        HttpResponseMessage resp = GetPlantillaResponseDoc(parametros, listaObjetosAplotear);
                        if (!resp.IsSuccessStatusCode)
                        {
                            responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
                        }
                        resp.EnsureSuccessStatusCode();
                        bytesResult = resp.Content.ReadAsAsync<byte[]>().Result;
                        #endregion


                        if (bytesResult != null)
                        {
                            bytes = bytesResult;
                            hasPages = true;
                        }

                        if (!hasPages)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                        }

                        //bytes = memStream.GetBuffer();
                        //bytes = memStream.ToArray();
                        //Ya tengo el PDF. 
                    }
                    else
                    {
                        //Supera el maximo permitido de ploteos
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }
                }

                var fileName = "PloteoPlanchetaA4_{0}.pdf";
                var contentType = "application/pdf";

                if (grabarListado)
                {
                    //Si tengo al menos 1 objeto asociado a manzana (y no a parcela), armo la lista con las apicId de mazana. Sino, con las apicId de parcela
                    string[] apic_ids = listaObjetosGraficar.Any(l => l.idParcela == null) ? listaObjetosGraficar.Select(p => p.apicIdManzana).ToArray() : listaObjetosGraficar.Select(p => p.apicIdParcela).ToArray();
                    bytesTxt = GenerarListadoTxt(apic_ids);
                    bytes = GenerarArchivoZip(bytes, bytesTxt);
                    fileName = "PloteoPlanchetaA4_{0}.zip";
                    contentType = "application/zip";
                }

                //Puede no existir usuario logueado cuando proviene de los web service externos
                if (Usuario != null)
                {
                    AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionPloteoPlanchetaA4);
                }
                // return 
                zipFile = File(bytes, contentType, string.Format(fileName, DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")));
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (HttpRequestException ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoPlanchetaA4", new Exception(!string.IsNullOrEmpty(responseError) ? responseError : ex.Message));
                responseError = string.Empty;
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoPlanchetaA4", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        public ActionResult GenerarPloteoObrasByColeccion(long idColeccion, long idComponentePrincipal, string textosVariables)
        {
            Coleccion cole = GetColeccionById(idColeccion);

            string lstIds;

            lstIds = string.Join(",", cole.Componentes.Where(c => c.ComponenteId == idComponentePrincipal).Select(c => c.ObjetoId));

            return GenerarPloteoObrasByIds(lstIds, idComponentePrincipal, textosVariables);
        }

        public ActionResult GenerarPloteoObrasByIds(string idsObjetoGraf, long idComponentePrincipal, string textosVariables)
        {
            byte[] bytes = { };
            byte[] bytesTxt = { 0, 1 };
            string responseError = string.Empty;

            try
            {
                int[] ids = GetLisMaximoNumePloteos(idsObjetoGraf.Split(','))?.Select(elem => Convert.ToInt32(elem)).ToArray();

                if (ids != null)
                {
                    bool hasPages = false;

                    byte[] bytesResult;

                    #region obtiene el pdf
                    long idComponente = Convert.ToInt64(GetParametrosGenerales().SingleOrDefault(p => p.Descripcion == "ID_COMPONENTE_MANZANA")?.Valor);


                    int idPlantillaFondo = 0;

                    var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);

                    if (plantillaFondo != null)
                    {
                        idPlantillaFondo = plantillaFondo.IdPlantillaFondo;
                    }


                    string parametros = string.Format("idPlantilla={0}&idPlantillaFondo={1}&idComponenteObjetoGraf={2}&lstIds={3}&idAtributosTexto={4}",
                                                           this.Plantilla.IdPlantilla, idPlantillaFondo, idComponentePrincipal, ids, textosVariables);

                    HttpResponseMessage resp = _cliente.PostAsync("api/ModuloPloteo/GetPlantillaObra?" + parametros, new ObjectContent(ids.GetType(), ids, new JsonMediaTypeFormatter())).Result;

                    if (!resp.IsSuccessStatusCode)
                    {
                        responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    }
                    resp.EnsureSuccessStatusCode();
                    bytesResult = resp.Content.ReadAsAsync<byte[]>().Result;
                    #endregion


                    if (bytesResult != null)
                    {
                        bytes = bytesResult;
                        hasPages = true;
                    }

                    if (!hasPages)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                    }
                }
                else
                {
                    //Supera el maximo permitido de ploteos
                    return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                }

                //Puede no existir usuario logueado cuando proviene de los web service externos
                if (Usuario != null)
                {
                    AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionPloteoPlanchetaA4);
                }
                zipFile = File(bytes, "application/pdf", $"Ploteo{GetComponenteById(idComponentePrincipal).Nombre.Replace(" ", "")}_{DateTime.Now:dd/MM/yyyy HH:mm:ss}.pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (HttpRequestException ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoObrasByIds", new Exception(!string.IsNullOrEmpty(responseError) ? responseError : ex.Message));
                responseError = string.Empty;
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoObrasByIds", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult GenerarPloteoInformeAnual(int idPeriodo, int idTipoPlano, long idPartido)
        {
            byte[] bytes = { 0, 1 };
            byte[] bytesTxt = { 0, 1 };
            string responseError = string.Empty;
            try
            {
                //bool grabarListado = true;
                double transparenciaPorc = 0;

                //int anio = 2014;
                Periodo periodo = this.Periodos.FirstOrDefault(p => p.IdPeriodo == idPeriodo);
                int anio = periodo.AnioCalendario;
                //buscar plantilla por idTipoPlano
                TipoPlano tipoPlano = this.TipoPlanos.FirstOrDefault(p => p.IdTipoPlano == idTipoPlano);
                Plantilla plantilla = GetPlantillaById(tipoPlano.IdPlantilla);
                int idPlantilla = tipoPlano.IdPlantilla;
                int idPlantillaFondo = 87;
                if (plantilla.PlantillaFondos != null)
                {
                    var plantillaFondo = plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == idPlantilla && !pf.FechaBaja.HasValue);
                    if (plantillaFondo != null)
                    {
                        idPlantillaFondo = plantillaFondo.IdPlantillaFondo;
                    }
                }
                //PartidoApicRedes partido = this.PartidosApicRedes.FirstOrDefault(p => p.GId == idPartido);
                Partido partido = this.Partidos.FirstOrDefault(p => p.IdPartido == idPartido);

                //string textosVariables = "ANIO," + anio.ToString() + ";PARTIDO," + partido.Nombre + ";TIPO_PLANO," + tipoPlano.Nombre;
                string textosVariables = string.Empty;
                float imagenTransparencia = (float)(transparenciaPorc / 100.0);
                //http://localhost:37291/api/ModuloPloteo/GetPlantillaInformeAnual?idPlantilla=80&idPlantillaFondo=87&idPartido=57&anio=2014&textosVariables=ANIO,2014;PARTIDO,TIGRE&idImagenSatelital=0&imagenTransparencia=0
                string parametros = string.Format("idPlantilla={0}&idPlantillaFondo={1}&idPartido={2}&anio={3}&textosVariables={4}&idImagenSatelital={5}&imagenTransparencia={6}",
                                                   idPlantilla, idPlantillaFondo, idPartido, anio, textosVariables, 0, 0);
                HttpResponseMessage resp = _cliente.GetAsync("api/ModuloPloteo/GetPlantillaInformeAnual?" + parametros).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
                }
                resp.EnsureSuccessStatusCode();
                bytes = resp.Content.ReadAsAsync<byte[]>().Result;
                if (bytes == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                }
                //Puede no existir usuario logueado cuando proviene de los web service externos
                if (Usuario != null)
                {
                    //AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionInformeAnual);
                }

                int iAnio = Convert.ToInt32(anio) - 5 - 2000;
                zipFile = File(bytes, "application/pdf", $"{tipoPlano.Nombre}_{partido.Nombre}_{periodo.Descripcion} ({tipoPlano.CodigoPlano}{partido.Abrev}{iAnio})");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (HttpRequestException ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoInformeAnual", new Exception(!string.IsNullOrEmpty(responseError) ? responseError : ex.Message));
                responseError = string.Empty;
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoInformeAnual", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult BuscarInformeAnual(int idPeriodo, int idTipoPlano, long idPartido)
        {
            byte[] bytes = { 0, 1 };
            byte[] bytesTxt = { 0, 1 };
            string responseError = string.Empty;
            try
            {
                Periodo periodo = this.Periodos.FirstOrDefault(p => p.IdPeriodo == idPeriodo);
                int anio = periodo.AnioCalendario;

                TipoPlano tipoPlano = this.TipoPlanos.FirstOrDefault(p => p.IdTipoPlano == idTipoPlano);

                Partido partido = this.Partidos.FirstOrDefault(p => p.IdPartido == idPartido);

                using (var doc = new it.Document())
                using (var memStream = new MemoryStream())
                using (var writer = new PdfCopy(doc, memStream))
                {
                    bool hasPages = false;

                    //http://localhost:37291/api/InformeAnual/GetInformeAnualGuardado?idPeriodo=5&idTipoPlano=1&idPartido=63
                    string parametros = string.Format("idPeriodo={0}&idTipoPlano={1}&idPartido={2}",
                                                       idPeriodo, idTipoPlano, idPartido);
                    HttpResponseMessage resp = _cliente.GetAsync("api/InformeAnual/GetInformeAnualGuardado?" + parametros).Result;
                    if (!resp.IsSuccessStatusCode)
                    {
                        responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
                    }
                    resp.EnsureSuccessStatusCode();

                    bytes = resp.Content.ReadAsAsync<byte[]>().Result;
                    if (bytes != null)
                    {
                        using (var reader = new PdfReader(bytes))
                        {
                            doc.Open();
                            writer.AddDocument(reader);
                        }
                        hasPages = true;
                    }
                    if (!hasPages)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                    }
                    doc.Close();
                    bytes = memStream.GetBuffer();
                }
                //Puede no existir usuario logueado cuando proviene de los web service externos
                if (Usuario != null)
                {
                    //AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionInformeAnual);
                }
                var contentType = "application/pdf";
                DateTime ahora = DateTime.Now;
                string fecha = ahora.Year + "-" + ahora.Month.ToString("00") + "-" + ahora.Day.ToString("00") + "-" + ahora.Hour.ToString("00") + ahora.Minute.ToString("00") + ahora.Second.ToString("00");
                //var fileName = string.Format("{0}_{1}_{2}.pdf", tipoPlano.Nombre, partido.Nombre, periodo.Descripcion);
                int iAnio = Convert.ToInt32(anio) - 5 - 2000;
                string nombre = tipoPlano.CodigoPlano + partido.Abrev + iAnio.ToString();
                var fileName = string.Format("{0}_{1}_{2} ({3}).pdf", tipoPlano.Nombre, partido.Nombre, periodo.Descripcion, nombre);
                zipFile = File(bytes, contentType, fileName);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (HttpRequestException ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-BuscarInformeAnual", new Exception(!string.IsNullOrEmpty(responseError) ? responseError : ex.Message));
                responseError = string.Empty;
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-BuscarInformeAnual", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #region Procesar Excel
        public byte[] GenerarPloteoFromFile(byte[] file, string tipgrafico, string fileExtension, long idComponentePrincipal, bool ordenarPorExpediente, bool ordenarPorManzana, bool grabarListado, bool isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {
            //VIENE DE LINEA 804
            //Inicializar variables
            var lisObjetosAGraficar = new List<string>();
            var listParesManzanaParcela = new List<KeyValuePair<long, long>>();
            byte[] bytesPdf = null;
            var ordenarPorManzana2 = ordenarPorManzana;
            var ordenarPorExpediente2 = ordenarPorExpediente;

            //Cargar archivo en memoria con los ids del excel
            MemoryStream target = new MemoryStream(file);
            //file.InputStream.CopyTo(target);

            //lisObjetosAGraficar CONTENDRA UNA LISTA DE APIC_ID_MANZANAS
            //O DE PARCELAS(APIC_ID o CODIGOS DE DISTRITOS)
            //TODO LO QUE SEA EXPEDIENTE Y QUE NO EMPIECE CON CODIGO DE DISTRITOS(102,103,202,ETC), SERA TRATADO COMO APIC_ID(DIRECTO) DE PARCELA.
            if (fileExtension.ToUpper().Equals("XLSX"))
            {
                //DEVUELVE UNA LISTA DE APIC_ID DE PARCELAS o DE APIC ID MANZANAS
                lisObjetosAGraficar = GetListaObjetosFromExcel(target, tipgrafico);
            }
            else
            {
                if (fileExtension.ToUpper().Equals("CSV") || fileExtension.ToUpper().Equals("TXT"))
                    lisObjetosAGraficar = GetListaObjetosFromCSV_TXT(target, tipgrafico);
            }

            //DETERMINO SI LA LISTA DE APIC_ID DE PARCELAS, CONTIENE UNA CANTIDAD DE OBJETOS PERMITIDA O NO
            string[] lsStringObjetosAGraficar = GetLisMaximoNumePloteos(lisObjetosAGraficar.ToArray());

            if (lsStringObjetosAGraficar != null && lsStringObjetosAGraficar.Count() > 0)
            {
                //Generar Ploteo desde la lista de objetos
                if (tipgrafico.Equals("esExpediente"))
                {
                    //EN ESTE CASO lsStringObjetosAGraficar ES UNA LISTA DE APIC_ID DE PARCELA, 
                    //PORQUE tipgrafico ES esExpediente. 

                    //if (lsStringObjetosAGraficar.Count > 0)
                    //{
                    //3- DETERMINA SI ORDENA LA LISTA DE PARES POR APIC_ID_PARCELA O POR APIC_ID_MANZANA
                    listParesManzanaParcela = OrdenarPorParcelaOManzana(lsStringObjetosAGraficar.ToList(), ordenarPorExpediente2, ordenarPorManzana2, false);

                    //4-MANZANA DUPLICADA TILDADA
                    if (isManzanaDuplicadaCheck)
                    {
                        //DEJA LA LISTA DE PARES, SIN MODIFICAR
                        bytesPdf = PrepararPDFParcelas(listParesManzanaParcela, idComponentePrincipal, "", verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
                    }
                    else //5-MANZANA DUPLICADA DESTILDADA 
                    {
                        //AGRUPA LA LISTA DE PARES POR MANZANA
                        //A CADA MANZANA LE ASOCIA "N" PARCELAS, SEPARADAS POR COMA
                        bytesPdf = ProcesarParcelasAgrupadasPorManzana(idComponentePrincipal, listParesManzanaParcela, verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
                    }
                    //}
                    //else
                    //{
                    //    throw new Exception("Supera el maximo permitido de Ploteos.");
                    //}

                }
                else if (tipgrafico.Equals("esManzana"))
                {
                    //EN ESTE CASO lsStringObjetosAGraficar ES UNA LISTA DE APIC ID de la tabla CT_MANZANA
                    List<string> lstManzPlotear = new List<string>();
                    bytesPdf = ProcesarManzanas(idComponentePrincipal, lsStringObjetosAGraficar, false, ordenarPorManzana, verCotas, ref lstManzPlotear, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
                }

                return bytesPdf;
            }
            else
                throw new Exception("Supera el maximo permitido de objetos a plotear.");
        }

        public List<string> GetListaObjetosFromExcel(MemoryStream target, string graphType)
        {
            //VIENE DE LINEA 956
            List<string> listObjetos = new List<string>();
            var idDistrito = string.Empty;
            var idExpediente = string.Empty;

            var apicID = string.Empty;

            //Esta sera la lista final a procesar
            List<string> lisObjetosAGraficar = new List<string>();

            using (var package = new ExcelPackage(target))
            {
                // Get the work book in the file
                ExcelWorkbook workBook = package.Workbook;
                if (workBook != null)
                {
                    if (workBook.Worksheets.Count > 0)
                    {
                        // Get the first worksheet
                        ExcelWorksheet currentWorksheet = workBook.Worksheets.First();

                        //Obtengo la lista de objetos a procesar
                        foreach (ExcelRangeBase cell in currentWorksheet.Cells)
                        {
                            //Asigna el rango de celdas con valores a un array listObjetos
                            //Recorre todas las filas del excel
                            //Si la celda no está vacia la agrego al array de objetos
                            //listObjetos DESCONOCE LA NATURALEZA DE SU COLECCION DE ITEMS
                            listObjetos.Add(cell.Text.ToString().Trim());
                        }

                        //ESTA LISTA TENDRA APIC_ID DE PARCELAS o DE APIC_ID MANZANAS, LISTAS PARA SER PROCESADAS
                        lisObjetosAGraficar = GetObjetosAPlotearFromListado(listObjetos, graphType);
                    }
                }
            }

            return lisObjetosAGraficar;
        }
        #endregion

        #region Procesar CSV_TXT

        public List<string> GetListaObjetosFromCSV_TXT(MemoryStream target, string tgraf)
        {
            //VIENE DE 970
            var listObjetos = new List<string>();
            var lisObjetosAGraficar = new List<string>();

            //AQUI ASEGURAR EL PROCESAMIENTO DE ARCHIVO CSV/TXT
            //SE CONSIDERA QUE CADA RENGLON(LINEA) CONTIENE UN UNICO STRING, CORRESPONDIENTE A
            //ID_DISTRITO + ID_EXPEDIENTE
            var reader = new StreamReader(target);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var value = line.TrimEnd(',');

                listObjetos.Add(value);
            }

            lisObjetosAGraficar = GetObjetosAPlotearFromListado(listObjetos, tgraf);

            return lisObjetosAGraficar;
        }
        #endregion

        #region Funciones de Ploteo
        public string[] GetLisMaximoNumePloteos(string[] idsObjetosAGraficar, int? maximoNumeroPloteos = null)
        {
            if (!maximoNumeroPloteos.HasValue)
            {
                maximoNumeroPloteos = 0;
                var paramMaximoNumeroPloteos = GetParametrosGenerales().SingleOrDefault(p => p.Descripcion == "MaximoNumeroPloteos");
                if (paramMaximoNumeroPloteos != null)
                {
                    maximoNumeroPloteos = Convert.ToInt32(paramMaximoNumeroPloteos.Valor);
                }
            }
            if (idsObjetosAGraficar.Length <= maximoNumeroPloteos)
            {
                //PLOTEA
                idsObjetosAGraficar = idsObjetosAGraficar.Take(maximoNumeroPloteos.Value).ToArray();
            }
            else
            {
                idsObjetosAGraficar = null;
            }

            return idsObjetosAGraficar;
        }

        public List<KeyValuePair<long, long>> GetDicMaximoNumePloteos(List<KeyValuePair<long, long>> listadoManzanasByParcelaID)
        {
            List<ParametrosGeneralesModel> lstParametrosGenerales = GetParametrosGenerales();
            var maximoNumeroPloteos = 0;
            ParametrosGeneralesModel paramMaximoNumeroPloteos = lstParametrosGenerales.FirstOrDefault(p => p.Id_Parametro == 31);
            if (paramMaximoNumeroPloteos != null)
            {
                maximoNumeroPloteos = Convert.ToInt32(paramMaximoNumeroPloteos.Valor);
            }

            if (listadoManzanasByParcelaID.Count < maximoNumeroPloteos)
            {
                //PLOTEA
                listadoManzanasByParcelaID = listadoManzanasByParcelaID.Take(maximoNumeroPloteos).ToList();
            }
            else
            {
                listadoManzanasByParcelaID = null;
            }

            return listadoManzanasByParcelaID;
        }

        public List<string> GetObjetosAPlotearFromListado(List<string> listObjetos, string typegraph)
        {
            //VIENE DE LINEA 1064/ 1112
            var lisParcelasAGraficar = new List<string>();
            var lisAPIC_ID_Parcelas = new List<string>();
            //101,102,103,201,202,203
            var listIDDistritos = new List<string>(new string[] { "101", "102", "103", "201", "202", "203" });
            var idexpsinceros = 0;
            var apicID = string.Empty;
            var idDistrito = string.Empty;
            var idExpediente = string.Empty;
            var componente = new ComponenteModel();
            var idComponente = "0";

            //IDENTIFICO LA NATURALEZA DEL COMPONENTE PARA listObjetos
            if (typegraph == "esManzana")
                idComponente = GetParametrosGenerales().Single(p => p.Descripcion == "ID_COMPONENTE_MANZANA").Valor;
            else
                idComponente = GetParametrosGenerales().Single(p => p.Descripcion == "ID_COMPONENTE_PARCELA").Valor;

            //IDENTIFICO LA NATURALEZA DEL CONTENIDO PARA listObjetos
            if (typegraph == "esExpediente")
            {
                //SI ES EXPEDIENTE idComponente DEBERIA SER 9
                componente = GetComponentesById(Convert.ToInt64(idComponente));


                //DETERMINO SI PROCESARE LA LISTA(listObjetos) COMO CODIGOS DE DISTRITO O SI DEBERE 
                //RETORNAR LA LISTA AL PROGRAMA SIN MODIFICAR, PARA SER PROCESADA LUEGO.

                for (int r = 0; r < listObjetos.Count; r++)
                {
                    //Paso 1: Separo ID_DISTRITO (primeros 3) + EXPEDIENTE (resto) [202001642 -> 202 + 001642]
                    idDistrito = listObjetos[r].Substring(0, 3); // EJ. 202
                    idExpediente = listObjetos[r].Substring(3, (listObjetos[r].Length) - 3); //EJ. 001642

                    //Paso 2: para el ID_DISTRITO, reemplazo 101,102,103,201,202,203 por 990, sino el mismo [990]
                    if (listIDDistritos.Contains(idDistrito))
                    {
                        //reemplazo por 990
                        idDistrito = "990";
                    }
                    //sino el mismo [990]
                    if (idDistrito == "990")
                    {
                        //Paso 3: saco ceros a izq del EXPEDIENTE [1642]
                        idexpsinceros = Int32.Parse(idExpediente);
                        //Paso 4: concateno y busco por APIC_ID [9901642]
                        idExpediente = idexpsinceros.ToString();
                        apicID = idDistrito + idExpediente;

                        if (apicID != string.Empty)
                        {
                            lisAPIC_ID_Parcelas.Add(apicID);
                        }
                    }
                    else
                    {
                        //SI NO ES UN DISTRITO, ENTONCES SE TRATA DIRECTAMENTE COMO APIC_ID
                        //Y SE LO AGREGA A LA LISTA
                        lisAPIC_ID_Parcelas.Add(listObjetos[r]);
                    }
                }

                //UNA VEZ QUE SE TERMINA DE PROCESAR, SE SOBREESCRIBE
                //LA LISTA ORIGINAL, CON LA LISTA FINAL DE APIC_ID
                listObjetos = lisAPIC_ID_Parcelas;
            }
            //Si no es una lista de Distritos o de APIC_ID de Parcelas, entonces debe ser 
            //una lista de APIC_ID de Manzanas.
            //SE DEJA listObjetos COMO ESTA Y SE RETORNA PARA SER PROCESADO LUEGO.

            return listObjetos;
        }
        #endregion

        #region GENERAR PLOTEO GENERAL
        [HttpPost]
        public ActionResult GenerarPloteoGeneral(long idColeccion, long idComponentePrincipal, string textosVariables, int idImagenSatelital, int transparenciaPorc)
        {

            byte[] bytes = null;

            //INICIALIZAR ID's PARA PARCELA,MANZANA,DISTRITO,CALLE
            string[] idsObjetosAGraficar = new string[0];
            try
            {
                //OBTENGO ID COMPONENTE PRINCIPAL DESDE SESSION
                idComponentePrincipal = Convert.ToInt32(Session["idComponentePrincipal"]);

                //ESTO ME TRAE COMPONENTES REPETIDOS, PERO ME INTERESAN EN ESTE METODO LOS OBJETOS.
                //LOS CUALES SON UNICOS, NO IMPORTA QUE SE REPITAN LOS ID COMPONENTES
                var coleccion = GetColeccionById(idColeccion);

                //FILTRAR LA COLECCION PARA QUE SOLO ESTE COMPUESTA DE LOS COMPONENTES A GRAFICAR
                //ESTOS COMPONENTES SON LOS MISMOS QUE EL COMPONENTE PRINCIPAL, PERO CON DISTINTO
                //OBJETOID                      
                idsObjetosAGraficar = coleccion.Componentes.Select(c => c.ObjetoId.ToString()).ToArray();

                //AQUI, CONTROLA EL LIMITEMAXIMO A PLOTEAR Y GENERA O NO
                var idsMaximonumPloteos = GetLisMaximoNumePloteos(idsObjetosAGraficar);

                if (idsMaximonumPloteos.Count() > 0)
                {
                    bytes = PrepararPDFManzanasUOtros(idsMaximonumPloteos, idComponentePrincipal.ToString(), textosVariables, idImagenSatelital, transparenciaPorc);
                    if (bytes == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                    }
                    AuditoriaHelper.Register(Usuario.Id_Usuario, string.Empty, Request, Resources.TipoOperacion.Exportar, Autorizado.Si, Eventos.GeneracionPloteoGeneral, "", "", "", idsObjetosAGraficar.Length);
                    zipFile = File(bytes, "application/pdf", string.Format("PloteoGeneral_{0}.pdf", DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")));

                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("PloteoController-GenerarPloteoGeneral", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region GENERAR PLOTEO
        public byte[] PrepararPDFManzanasUOtros(string[] lisMaximoNumPloteos, string idComponentePpal, string textosVariables, int idImagenSatelital, int transparenciaPorc)
        {
            //OBTENER PLANTILLAFONDO
            var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);
            int idPlantillaFondo = plantillaFondo != null ? plantillaFondo.IdPlantillaFondo : 0;

            //RETORNA UN ARRAY DE BYTES
            return GenerarPloteoPDFManzanasUOtros(Convert.ToInt64(idComponentePpal), lisMaximoNumPloteos, idPlantillaFondo, string.Empty, textosVariables, false, null, null, null, idImagenSatelital, transparenciaPorc);
        }

        public byte[] PrepararPDFParcelasAgrupadasPorManzana(Dictionary<long, string> dictionaryMaximoNumPloteos, long idComponentePrincipal, string textosVariables, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {
            var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);

            int idPlantillaFondo = 0;

            if (plantillaFondo != null)
            {
                idPlantillaFondo = plantillaFondo.IdPlantillaFondo;
            }

            return GenerarPloteoPDFParcelasAgrupadasPorManzana(idComponentePrincipal, dictionaryMaximoNumPloteos, idPlantillaFondo, string.Empty, textosVariables, "", "", verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);
        }

        public byte[] PrepararPDFParcelas(List<KeyValuePair<long, long>> lisMaximoNumPloteos, long idComponentePrincipal, string textosVariables, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {
            byte[] bytes = { 0, 1 };

            var plantillaFondo = this.Plantilla.PlantillaFondos.SingleOrDefault(pf => pf.IdPlantilla == this.Plantilla.IdPlantilla && !pf.FechaBaja.HasValue);

            var idPlantillaFondo = plantillaFondo != null ? plantillaFondo.IdPlantillaFondo : 0;

            //RETORNA UN ARRAY DE BYTES
            bytes = GenerarPloteoPDFParcelas(idComponentePrincipal, lisMaximoNumPloteos, idPlantillaFondo, string.Empty, textosVariables, "", "", verCotas, grafico, leyenda, infoLeyenda, idImagenSatelital, transparenciaPorc);

            return bytes;
        }
        private List<KeyValuePair<long, long>> GetManzanasByIds(string[] idsObjetosAGraficar, bool esApic)
        {
            //VIENE DE LINEA 972/ 498
            var listaIdsApic = idsObjetosAGraficar;
            //esApic=true: idsObjetosAGraficar ES UNA LISTA DE APIC_ID DE PARCELAS
            //esApic=false: idsObjetosAGraficar ES UNA LISTA DE ID PARCELAS
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetManzanasByIds?esApic=" + esApic, listaIdsApic, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listadoManzanasByParcela = (List<KeyValuePair<long, long>>)respColecciones.Content.ReadAsAsync<List<KeyValuePair<long, long>>>().Result;

            return listadoManzanasByParcela;
        }

        private List<KeyValuePair<long, long>> GetManzanasByParcelasApicIds(string[] parcelasApicIds)
        {
            //VIENE DE LINEA 972/ 498
            var listaIdsApic = parcelasApicIds;
            //esApic=true: idsObjetosAGraficar ES UNA LISTA DE APIC_ID DE PARCELAS
            //esApic=false: idsObjetosAGraficar ES UNA LISTA DE ID PARCELAS
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetManzanasByParcelasAPIC_ID", listaIdsApic, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listadoManzanasByParcela = (List<KeyValuePair<long, long>>)respColecciones.Content.ReadAsAsync<List<KeyValuePair<long, long>>>().Result;

            return listadoManzanasByParcela;
        }

        private List<string> GetManzanasByLisManzanasAPICIDs(string[] idsObjetosAGraficar)
        {
            //VIENE DE LINEA 613
            //HttpResponseMessage respColecciones = _cliente.GetAsync("api/PloteoService/GetManzanasByIdsParcela?listaIdParcelas=" + listaIdParcelas).Result;
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetManzanasByLisManzanasAPICIDs", idsObjetosAGraficar, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listadoManzanasByApicIDOfManzana = (List<string>)respColecciones.Content.ReadAsAsync<List<string>>().Result;

            return listadoManzanasByApicIDOfManzana;
        }

        private List<string> GetParcelasByParcelasApicIds(string[] parcelasApicIds)
        {
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetIDParcelasByApicID", parcelasApicIds, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listado = respColecciones.Content.ReadAsAsync<List<string>>().Result;

            return listado;
        }

        private List<string> GetParcelasByParcelasExpRef(string[] expRef)
        {
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetIDParcelasByExpRef", expRef, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listado = respColecciones.Content.ReadAsAsync<List<string>>().Result;

            return listado;
        }

        private List<string> GetParcelasByParcelasSapID(string[] SAPIds)
        {
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetIDParcelasBySAPId", SAPIds, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listado = respColecciones.Content.ReadAsAsync<List<string>>().Result;

            return listado;
        }

        private List<string> GetParcelasByParcelasCC(string[] CCs)
        {
            HttpResponseMessage respColecciones = _cliente.PostAsync<string[]>("api/PloteoService/GetIDParcelasByCC", CCs, new JsonMediaTypeFormatter()).Result;
            respColecciones.EnsureSuccessStatusCode();
            var listado = respColecciones.Content.ReadAsAsync<List<string>>().Result;

            return listado;
        }


        public byte[] GenerarPloteoPDFManzanasUOtros(long idComponente, string[] idsObjetosGraficar, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {
            //Pdf de varias paginas => crea por la api.

            bool hasPages = true;
            byte[] bytesPdf = null;

            float imagenTransparencia = (float)(transparenciaPorc / 100.0);
            string parametros = string.Format("id={0}&idsObjetosGraf={1}&idComponenteObjetoGraf={2}&idPlantillaFondo={3}&idsObjetoSecundario={4}&textosVariables={5}&verCotas={6}&idImagenSatelital={7}&imagenTransparencia={8}&grafico={9}&leyenda={10}&infoLeyenda={11}",
                                                   this.Plantilla.IdPlantilla, idsObjetosGraficar, idComponente, idPlantillaFondo, idsObjetoSecundario, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, JsonConvert.SerializeObject(grafico), JsonConvert.SerializeObject(leyenda), infoLeyenda);

            HttpResponseMessage resp = _cliente.PostAsync("api/ModuloPloteo/GenerarPloteoPDFManzanasUOtros?" + parametros, new ObjectContent(idsObjetosGraficar.GetType(), idsObjetosGraficar, new JsonMediaTypeFormatter())).Result;
            if (!resp.IsSuccessStatusCode)
            {
                responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
            }
            resp.EnsureSuccessStatusCode();

            bytesPdf = resp.Content.ReadAsAsync<byte[]>().Result;

            return (hasPages ? bytesPdf : null);//Verificar, creo que si devuelve null, la pagina da error.
        }


        public byte[] GenerarPloteoPDFParcelas(long idComponente, List<KeyValuePair<long, long>> dicParcelasGraficar, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, string PloteoType, string isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {

            //************** AQUI SOLO TIENE QUE GENERAR EL PDF, 
            //ASEGURANDO DE LLAMAR AL GETPLANTILLA DE RAUL
            //POR PARES (OBJETO=MANZANA, COMPONENTESECUNDARIO=PARCELA)
            //UNA HOJA POR PAR DE LA LISTA

            byte[] bytes = null;

            float imagenTransparencia = (float)(transparenciaPorc / 100.0);
            //string parametros = string.Format("id={0}&{1}&idComponenteObjetoGraf={2}&idPlantillaFondo={3}&textosVariables={4}&verCotas={5}&idImagenSatelital={6}&imagenTransparencia={7}&grafico={8}&leyenda={9}&infoLeyenda={10}",
            //                                  this.Plantilla.IdPlantilla, idComponente, idPlantillaFondo, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, JsonConvert.SerializeObject(grafico), JsonConvert.SerializeObject(leyenda), infoLeyenda);


            string parametros = string.Format("id={0}&idComponenteObjetoGraf={1}&idPlantillaFondo={2}&textosVariables={3}&verCotas={4}&idImagenSatelital={5}&imagenTransparencia={6}&grafico={7}&leyenda={8}&infoLeyenda={9}",
                                               this.Plantilla.IdPlantilla, idComponente, idPlantillaFondo, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, JsonConvert.SerializeObject(grafico), JsonConvert.SerializeObject(leyenda), infoLeyenda);
            List<KeyValuePair<long, string>> asd = dicParcelasGraficar.Select(a => new KeyValuePair<long, string>(a.Value, a.Key.ToString())).ToList();
            HttpResponseMessage resp = _cliente.PostAsync("api/ModuloPloteo/GenerarPloteoPDFParcelas?" + parametros, new ObjectContent(asd.GetType(), asd, new JsonMediaTypeFormatter())).Result;
            if (!resp.IsSuccessStatusCode)
            {
                responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
            }
            resp.EnsureSuccessStatusCode();


            bytes = resp.Content.ReadAsAsync<byte[]>().Result;

            return bytes;

        }


        public byte[] GenerarPloteoPDFParcelasAgrupadasPorManzana(long idComponente, Dictionary<long, string> dicParcelasGraficar, int idPlantillaFondo, string idsObjetoSecundario, string textosVariables, string PloteoType, string isManzanaDuplicadaCheck, bool verCotas, InformacionComercial grafico, InformacionComercial leyenda, int? infoLeyenda, int idImagenSatelital, int transparenciaPorc)
        {

            //************** AQUI SOLO TIENE QUE GENERAR EL PDF, 
            //ASEGURANDO DE LLAMAR EL GETPLANTILLA DE RAUL
            //POR PARES OBJETO=MANZANA, COMPONENTESECUNDARIO=PARCELA
            //UNA HOJA POR PAR DE LA LISTA
            /*
            byte[] bytes = null;

            foreach (var objeto in dicParcelasGraficar)
            {
                float imagenTransparencia = (float)(transparenciaPorc / 100.0);
                string parametros = string.Format("id={0}&idObjetoGraf={1}&idComponenteObjetoGraf={2}&idPlantillaFondo={3}&idsObjetoSecundario={4}&textosVariables={5}&verCotas={6}&idImagenSatelital={7}&imagenTransparencia={8}&grafico={9}&leyenda={10}&infoLeyenda={11}",
                                                   this.Plantilla.IdPlantilla, objeto.Key, idComponente, idPlantillaFondo, objeto.Value, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, JsonConvert.SerializeObject(grafico), JsonConvert.SerializeObject(leyenda), infoLeyenda);

                HttpResponseMessage resp = GetPlantillaResponse(parametros);
                bytes = resp.Content.ReadAsAsync<byte[]>().Result;

            }

            return bytes;
            */
            byte[] bytes = null;

            float imagenTransparencia = (float)(transparenciaPorc / 100.0);
            string parametros = string.Format("id={0}&idComponenteObjetoGraf={1}&idPlantillaFondo={2}&textosVariables={3}&verCotas={4}&idImagenSatelital={5}&imagenTransparencia={6}&grafico={7}&leyenda={8}&infoLeyenda={9}",
                                               this.Plantilla.IdPlantilla, idComponente, idPlantillaFondo, textosVariables, verCotas, idImagenSatelital, imagenTransparencia, JsonConvert.SerializeObject(grafico), JsonConvert.SerializeObject(leyenda), infoLeyenda);
            List<KeyValuePair<long, string>> asd = dicParcelasGraficar.Select(a => new KeyValuePair<long, string>(a.Key, a.Value)).ToList();
            HttpResponseMessage resp = _cliente.PostAsync("api/ModuloPloteo/GenerarPloteoPDFParcelas?" + parametros, new ObjectContent(asd.GetType(), asd, new JsonMediaTypeFormatter())).Result;
            if (!resp.IsSuccessStatusCode)
            {
                responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
            }
            resp.EnsureSuccessStatusCode();


            bytes = resp.Content.ReadAsAsync<byte[]>().Result;

            return bytes;
        }

        public List<ParametrosGeneralesModel> GetParametrosGenerales()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/MapasTematicosService/GetParametrosGenerales").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ParametrosGeneralesModel>)resp.Content.ReadAsAsync<IEnumerable<ParametrosGeneralesModel>>().Result;
        }
        #endregion

        #region COLECCIONES SEGUN COMPONENTE PRINCIPAL
        private List<Coleccion> GetColeccionesByComponentePrincipal(long idComponentePrincipal)
        {
            HttpResponseMessage respColecciones = _cliente.GetAsync("api/PloteoService/GetColeccionesByComponentePrincipal?idComponentePrincipal=" + idComponentePrincipal).Result;
            respColecciones.EnsureSuccessStatusCode();
            var colecciones = (List<Coleccion>)respColecciones.Content.ReadAsAsync<IEnumerable<Coleccion>>().Result;

            return colecciones;
        }
        #endregion

        private List<ImagenSatelital> GetAllImagenSatelital()
        {
            HttpResponseMessage respImagenSatelital = _cliente.GetAsync("api/PloteoService/GetAllImagenSatelital").Result;
            respImagenSatelital.EnsureSuccessStatusCode();
            var lstImagenSatelital = (List<ImagenSatelital>)respImagenSatelital.Content.ReadAsAsync<IEnumerable<ImagenSatelital>>().Result;
            return lstImagenSatelital;
        }

        #region COLECCION POR ID
        private Coleccion GetColeccionById(long idColeccion)
        {
            //_cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            HttpResponseMessage respColecciones = _cliente.GetAsync("api/PloteoService/GetColeccionById?idColeccion=" + idColeccion).Result;
            respColecciones.EnsureSuccessStatusCode();
            var coleccion = respColecciones.Content.ReadAsAsync<Coleccion>().Result;
            return coleccion;
        }


        #endregion

        #region COLECCIONES POR MANZANA O PARCELA
        private List<Coleccion> GetColeccionesByUsuarioLogueadoColeccionA4(long UserId)
        {
            HttpResponseMessage respColecciones = _cliente.GetAsync("api/PloteoService/GetColeccionByCurrentUserColeccionA4?UsuarioId=" + UserId).Result;
            respColecciones.EnsureSuccessStatusCode();
            var colecciones = (List<Coleccion>)respColecciones.Content.ReadAsAsync<IEnumerable<Coleccion>>().Result;

            return colecciones;
        }

        private List<Coleccion> GetColeccionesByUsuarioLogueado(long UserId)
        {
            HttpResponseMessage respColecciones = _cliente.GetAsync("api/PloteoService/GetColeccionByUserComponentePrincipal?UsuarioId=" + UserId + "&ComponentePrincipalId").Result;
            respColecciones.EnsureSuccessStatusCode();
            var colecciones = (List<Coleccion>)respColecciones.Content.ReadAsAsync<IEnumerable<Coleccion>>().Result;

            return colecciones;
        }

        public ComponenteModel GetComponentesById(long id)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/MapasTematicosService/GetComponentesById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ComponenteModel)resp.Content.ReadAsAsync<ComponenteModel>().Result;
        }
        #endregion

        #region COMPOSICION COLECCION

        public JsonResult ComposicionColeccion(long idColeccion)
        {
            var coleccion = GetColeccionById(idColeccion);

            var componentesPloteables = GetComponentesPloteables();

            Dictionary<Data.BusinessEntities.MapasTematicos.Componente, int> composicion = new Dictionary<Data.BusinessEntities.MapasTematicos.Componente, int>();

            var componentesPloteablesEnColeccion = coleccion.Componentes.Where(c => componentesPloteables.Any(cp => cp.ComponenteId == c.ComponenteId));

            foreach (var group in componentesPloteablesEnColeccion.GroupBy(c => c.ComponenteId))
            {
                composicion.Add(componentesPloteables.First(cp => cp.ComponenteId == group.First().ComponenteId), group.Count());
            }


            return Json(composicion.Where(c => c.Key.DocType != null).ToDictionary(c => c.Key.Nombre, c => c.Value), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComposicionColeccionObra(long idColeccion, long idCompPrincipal)
        {
            var coleccion = GetColeccionById(idColeccion);

            Dictionary<Data.BusinessEntities.MapasTematicos.Componente, int> composicion = new Dictionary<Data.BusinessEntities.MapasTematicos.Componente, int>();

            var componentesPloteablesEnColeccion = coleccion.Componentes.Where(c => c.ComponenteId == idCompPrincipal);

            return Json(componentesPloteablesEnColeccion.Count(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region VALIDACION COLECCION

        public string ValidacionColeccion(long idPlantilla, long idColeccion)
        {
            var validacionColeccionComponente = "";

            /*Recupero los componentes de la coleccion y los componentes definidos como principales en la plantilla*/
            var componentesColeccion = GetColeccionById(idColeccion).Componentes.Select(c => c.ComponenteId);
            var componentesPrincipalesPlantilla = this.Plantilla.Layers.Where(l => l.Categoria == 1).Select(l => l.ComponenteId);


            /*Recupero los que son comunes a ambos (coleccion y plantilla)*/
            var componentesPosibles = (from idCompColeccion in componentesColeccion
                                       join idCompPlantilla in componentesPrincipalesPlantilla on idCompColeccion equals idCompPlantilla
                                       select idCompColeccion).Distinct();

            /* TODO: 
             *      REVISAR SI SE PUEDE SACAR ESTA VALIDACION Y PLOTEAR TODOS LOS OBJETOS 
             *      DE LA COLECCION DE TODOS LOS COMPONENTES PRINCIPALES DE LA PLANTILLA
             */

            /* Mientras se analiza lo de arriba, si la coleccion tiene objetos de mas de un tipo de componente definido 
             * como principal en la plantilla no se permite plotear
             */
            if (componentesPosibles.Count() == 1)
                Session["idComponentePrincipal"] = componentesPosibles.First();
            else
                validacionColeccionComponente = "La colección seleccionada contiene más de un componente principal de esta plantilla.";

            return validacionColeccionComponente;
        }
        #endregion

        [HttpPost]
        public ActionResult GenerarPloteoSubcuencas(string ids)
        {
            //Viene de RastreoProgramado\planificarRastreo.js
            short idCategoria = Convert.ToInt16(ConfigurationManager.AppSettings["CategoriaSubcuencas"]);
            long idComponente = Convert.ToInt64(ConfigurationManager.AppSettings["Subcuenca"]);//136

            this.Plantilla = GetPlantilla(getPlantillas().FirstOrDefault(l => l.IdPlantillaCategoria == idCategoria).IdPlantilla);
            return GenerarPloteoPredefinido(idComponente, ids, 0, 0);
        }

        [HttpPost]
        public ActionResult GenerarPloteoMallas(string ids)
        {
            //Viene de:
            //ZonasLavado\planificarRastreo.js
            //RehabilitacionServicio\planificarRehabilitacion.js
            //RANC\planificarLavado.js
            short idCategoria = Convert.ToInt16(ConfigurationManager.AppSettings["CategoriaMallas"]);
            long idComponente = Convert.ToInt32(ConfigurationManager.AppSettings["Malla"]);

            this.Plantilla = GetPlantilla(getPlantillas().FirstOrDefault(l => l.IdPlantillaCategoria == idCategoria).IdPlantilla);
            return GenerarPloteoPredefinido(idComponente, ids, 0, 0);
        }

        private HttpResponseMessage GetPlantillaResponseDoc(string parametros, List<ObjetoPloteable> listaObjetosAplotear)
        {
            HttpResponseMessage resp = _cliente.PostAsync("api/ModuloPloteo/GetPlantillaDoc?" + parametros, new ObjectContent(listaObjetosAplotear.GetType(), listaObjetosAplotear, new JsonMediaTypeFormatter())).Result;

            if (!resp.IsSuccessStatusCode)
            {
                responseError = resp.Content.ReadAsStringAsync().Result.Replace("\"", "");
            }
            resp.EnsureSuccessStatusCode();
            return resp;
        }
    }

    public class CategoriaModel
    {
        public int IdTipo { get; set; }
        public string Nombre { get; set; }
        public List<Plantilla> Plantillas { get; set; }
        public string Action { get; set; }
    }
}
