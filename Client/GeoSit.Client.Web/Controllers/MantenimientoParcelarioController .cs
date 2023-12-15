using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Web.Mvc;
using GeoSit.Client.Web.Models.Dominio;
using GeoSit.Client.Web.Models.DominioTitular;
using GeoSit.Client.Web.Models.ResponsableFiscal;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.ValidationRules.MantenedorParcelario;
using Newtonsoft.Json;
using Resources;
using Model = GeoSit.Client.Web.Models;
using GeoSit.Client.Web.Helpers;
using System.Xml.Linq;
using System.Net;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.GlobalResources;
using Newtonsoft.Json.Linq;

namespace GeoSit.Client.Web.Controllers
{
    public class MantenimientoParcelarioController : Controller
    {
        private readonly HttpClient cliente = new HttpClient();
        private readonly HttpClient clienteInformes = new HttpClient();
        private static string _modelSession = "Parcela";

        private Model.ArchivoDescarga ArchivoInforme
        {
            get { return Session["ArchivoDescarga"] as Models.ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }
        private UnidadMantenimientoParcelario UnidadMantenimientoParcelario
        {
            get { return Session["UnidadMantenimientoParcelario"] as UnidadMantenimientoParcelario; }
            set { Session["UnidadMantenimientoParcelario"] = value; }
        }
        private Model.UsuariosModel Usuario
        {
            get { return Session["usuarioPortal"] as Model.UsuariosModel; }
        }
        private Dictionary<long, List<TitularViewModel>> TitularesDominio
        {
            get { return Session["titularesDominio"] as Dictionary<long, List<TitularViewModel>>; }
            set { Session["titularesDominio"] = value; }
        }


        public MantenimientoParcelarioController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            clienteInformes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        public ActionResult Index()
        {
            return this.Get(45);
        }

        public JsonResult CancelAll()
        {
            UnidadMantenimientoParcelario = new UnidadMantenimientoParcelario();
            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Get(long id)
        {
            UnidadMantenimientoParcelario = new UnidadMantenimientoParcelario();
            TitularesDominio = new Dictionary<long, List<TitularViewModel>>();

            var parametrosMantenedorParcelario = new SeguridadController().GetParametrosGenerales().Where(pg => pg.Agrupador == "MANTENEDOR_PARCELARIO");
            bool activaNomenclatura = parametrosMantenedorParcelario.Any(pmt => pmt.Clave == Recursos.ActivarNomenclaturas && pmt.Valor == "1");
            bool activaPartidas = parametrosMantenedorParcelario.Any(pmt => pmt.Clave == Recursos.ActivarPartidas && pmt.Valor == "1");
            bool activaZonificacion = parametrosMantenedorParcelario.Any(pmt => pmt.Clave == Recursos.ActivarZonificacion && pmt.Valor == "1");
            bool activaDesignaciones = parametrosMantenedorParcelario.Any(pmt => pmt.Clave == Recursos.ActivarDesignaciones && pmt.Valor == "1") && SeguridadController.ExisteFuncion(Seguridad.VisualizarDesignaciones);
            bool activaValuaciones = parametrosMantenedorParcelario.Any(pmt => pmt.Clave == Recursos.ActivarValuaciones && pmt.Valor == "1") && SeguridadController.ExisteFuncion(Seguridad.VisualizarValuacion);

            var model = GetParcela(id);

            #region Designaciones
            if (activaDesignaciones)
            {
                model.Designaciones = GetParcelaDesignaciones(id);
            }
            #endregion

            #region Zona Tributaria
            if (!string.IsNullOrEmpty(model.Atributos))
            {
                var xdoc = XDocument.Parse(model.Atributos);
                var node = xdoc.Descendants("ZonaTributaria").FirstOrDefault();
                model.ZonaTributaria = node != null ? node.Value : string.Empty;
            }
            #endregion

            #region Zonificacion
            if (activaZonificacion)
            {
                using (var result = cliente.GetAsync("api/parcela/getzonificacion?idparcela=" + id).Result)
                {
                    result.EnsureSuccessStatusCode();
                    model.Zonificacion = result.Content.ReadAsAsync<Zonificacion>().Result;
                }
            }
            #endregion

            #region Superficies
            using (var result = cliente.GetAsync($"api/parcela/{id}/superficies/").Result)
            {
                try
                {
                    result.EnsureSuccessStatusCode();
                    ViewData["Superficies"] = result.Content.ReadAsAsync<ParcelaSuperficies>().Result;
                }
                catch (HttpRequestException ex)
                {
                    MvcApplication.GetLogger().LogError($"MantenimientoParcelario({id}) - Superficies", ex);
                }
            }
            #endregion

            Session[_modelSession] = model;
            /*Cargo Tipo clase y estados de la parcela*/
            ViewData["TiposParcela"] = GetTiposParcelas();
            ViewData["ClasesParcela"] = GetClasesParcelas();
            ViewData["EstadosParcela"] = GetEstadosParcelas();

            ViewData["PuedeModificarDatos"] = SeguridadController.ExisteFuncion(Seguridad.ModificarMantenedorParcelario) && model.FechaBaja == null;
            ViewData["PuedeImprimirInformeParcelario"] = SeguridadController.ExisteFuncion(Seguridad.InformeParcelario);
            ViewData["PuedeVerVIR"] = SeguridadController.ExisteFuncion(Seguridad.VIR);
            ViewData["VisualizarNomenclaturas"] = activaNomenclatura;
            ViewData["TieneNomenclaturas"] = model.Nomenclaturas?.Any() ?? false;
            ViewData["VisualizarPartidas"] = activaPartidas;
            ViewData["TienePartidas"] = model.UnidadesTributarias?.Any(ut => ut.TipoUnidadTributariaID == 1 || ut.TipoUnidadTributariaID == 2) ?? false;
            ViewData["VisualizarZonificacion"] = activaZonificacion;
            ViewData["VisualizarDesignaciones"] = activaDesignaciones;
            ViewData["VisualizarValuaciones"] = activaValuaciones;

            using (var resp = cliente.GetAsync($"api/Parametro/GetValor?id={Recursos.ActivarInterfaz}").Result)
            {
                resp.EnsureSuccessStatusCode();
                ViewData["ActivaInterfaz"] = resp.Content.ReadAsStringAsync().Result;
            }
            if (activaValuaciones)
            {
                ViewData["ZonasValuaciones"] = GetZonasValuaciones();
            }

            ViewData["ZonasTributarias"] = GetZonasTributarias();

            ViewData["IdInmueble"] = model.UnidadesTributarias.FirstOrDefault().UnidadTributariaId;
            return PartialView("Index", model);
        }

        private Designacion[] GetParcelaDesignaciones(long id)
        {
            using (var result = cliente.GetAsync($"api/designacion/GetDesignacionesParcela?idParcela={id}").Result)
            {
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsAsync<Designacion[]>().Result;
            }
        }

        [HttpGet]
        public ActionResult Reload()
        {
            return RedirectToAction("Get", new { id = (Session[_modelSession] as Parcela).ParcelaID });
        }
        public ActionResult Reset()
        {
            bool activaDesignaciones = new SeguridadController().GetParametrosGenerales().Any(pmt => pmt.Clave == Recursos.ActivarDesignaciones && pmt.Valor == "1") &&
                                       SeguridadController.ExisteFuncion(Seguridad.VisualizarDesignaciones);
            UnidadMantenimientoParcelario = new UnidadMantenimientoParcelario();
            TitularesDominio.Clear();
            var model = GetParcela((Session[_modelSession] as Parcela).ParcelaID);
            if (activaDesignaciones)
            {
                model.Designaciones = GetParcelaDesignaciones(model.ParcelaID);
            }

            Session[_modelSession] = model;
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public string GetAtributosZonificacion()
        {
            return JsonConvert.SerializeObject(new { data = ((Parcela)Session[_modelSession]).Zonificacion?.AtributosZonificacion ?? new AtributosZonificacion[0] });
        }

        private Parcela GetParcela(long id)
        {
            using (var resp = cliente.GetAsync("api/Parcela/Get/" + id).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<Parcela>().Result;
            }
        }

        public string GetUnidadesTributarias()
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];
                if (parcela.ClaseParcelaID == Convert.ToInt64(ClasesParcelas.ConjuntoInmobiliario))
                {
                    long UT_TIPO_COMUN = 1;
                    long UT_TIPO_PH = 2;
                    foreach (var ut in parcela.UnidadesTributarias.Where(ut=>ut.TipoUnidadTributariaID != UT_TIPO_COMUN)) //
                    {
                        if(ut.TipoUnidadTributariaID == UT_TIPO_PH)
                        {
                            ut.TipoUnidadTributaria.Descripcion = "CI";
                        }
                        else
                        {
                            ut.TipoUnidadTributaria.Descripcion = "UP de CI";
                        }
                    }
                }
                return $"{{\"data\":{JsonConvert.SerializeObject(parcela.UnidadesTributarias, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}}}";
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Parcela Documentos
        public string GetParcelaDocumentos()
        {
            Parcela model = (Parcela)Session[_modelSession];
            string data = "";
            if (model.ParcelaDocumentos != null)
            {
                List<Documento> documentos = model.ParcelaDocumentos.Select(pd => pd.Documento).Where(d => d != null).ToList();
                data = JsonConvert.SerializeObject(new { data = documentos });
            }
            else
            {
                data = JsonConvert.SerializeObject(new { data = "" });
            }
            return data;
        }

        [HttpPost]
        public JsonResult AddParcelaDocumento(long idDocumento)
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];
                var fechaHora = DateTime.Now;
                var parcelaDocumento = new ParcelaDocumento
                {
                    DocumentoID = idDocumento,
                    ParcelaID = parcela.ParcelaID,
                    UsuarioAltaID = Usuario.Id_Usuario,
                    FechaAlta = fechaHora,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                    FechaModificacion = fechaHora
                };
                if (parcela.ParcelaDocumentos == null) parcela.ParcelaDocumentos = new List<ParcelaDocumento>();
                parcela.ParcelaDocumentos.Add(parcelaDocumento);

                UnidadMantenimientoParcelario.OperacionesParcelaDocumento.Add(new OperationItem<ParcelaDocumento>
                {
                    Operation = Operation.Add,
                    Item = parcelaDocumento
                });

                return Json(new { OK = true });
            }
            catch (Exception)
            {
                return Json(new { OK = false });
            }
        }

        [HttpPost]
        public JsonResult EditParcelaDocumento(long idDocumento)
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];
                var fechaHora = DateTime.Now;
                var parcelaDocumento = new ParcelaDocumento
                {
                    DocumentoID = idDocumento,
                    ParcelaID = parcela.ParcelaID,
                    UsuarioAltaID = Usuario.Id_Usuario,
                    FechaAlta = fechaHora,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                    FechaModificacion = fechaHora
                };

                UnidadMantenimientoParcelario.OperacionesParcelaDocumento.Add(new OperationItem<ParcelaDocumento>
                {
                    Operation = Operation.Update,
                    Item = parcelaDocumento
                });

                return Json(new { OK = true });
            }
            catch (Exception)
            {
                return Json(new { OK = false });
            }
        }

        [HttpPost]
        public ActionResult DeleteParcelaDocumento(long idDocumento)
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];
                var parcelaDocumento = parcela.ParcelaDocumentos.First(pd => pd.DocumentoID == idDocumento && pd.ParcelaID == parcela.ParcelaID);
                var idTipoDocumento = parcelaDocumento.Documento.id_tipo_documento;
                if (idTipoDocumento == 7)
                {

                    var idParcelaMensura = parcela.ParcelaMensuras.Where(pm => pm.IdParcela == parcelaDocumento.Parcela.ParcelaID).FirstOrDefault().IdParcelaMensura;

                    var resp = cliente.PostAsync($"api/MensuraService/SetParcelaMensuraMantenedorDelete_Save?idParcelaMensura={idParcelaMensura}&idUsuario={Usuario.Id_Usuario}", new StringContent(string.Empty)).Result;
                    resp.EnsureSuccessStatusCode();

                }
                parcelaDocumento.Parcela = null;

                UnidadMantenimientoParcelario.OperacionesParcelaDocumento.Add(new OperationItem<ParcelaDocumento>
                {
                    Operation = Operation.Remove,
                    Item = parcelaDocumento
                });
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/DeleteParcelaDocumento", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Unidades Tributarias Documentos
        public string GetUTDocumentos(long idUT)
        {
            var uts = ((Parcela)Session[_modelSession]).UnidadesTributarias?.ToList() ?? new List<UnidadTributaria>();

            var documentosUT = uts.Where(ut => ut.UnidadTributariaId == idUT && ut.UTDocumentos != null)
                                  .SelectMany(u => u.UTDocumentos
                                                    .Where(d => !d.FechaBaja.HasValue)
                                                    .Select(d => d.Documento));

            return JsonConvert.SerializeObject(new { data = documentosUT.ToList() });
        }

        [HttpPost]
        public JsonResult AddUnidadTributariaDocumento(long idUnidadTributaria, long idDocumento, string descripcion,
            long idTipoDocumento, string tipoDocumentoDescripcion, string nombreArchivo)
        {
            try
            {
                var fechaHora = DateTime.Now;
                UnidadTributariaDocumento utDocumento = new UnidadTributariaDocumento
                {
                    DocumentoID = idDocumento,
                    UnidadTributariaID = idUnidadTributaria,
                    UsuarioAltaID = Usuario.Id_Usuario,
                    FechaAlta = fechaHora,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                    FechaModificacion = fechaHora
                };

                Parcela parcela = (Parcela)Session[_modelSession];
                UnidadTributaria unidadTributaria = parcela.UnidadesTributarias.FirstOrDefault(ut => ut.UnidadTributariaId == idUnidadTributaria);
                unidadTributaria.UTDocumentos = unidadTributaria.UTDocumentos ?? new List<UnidadTributariaDocumento>();
                var utDoc = CloneUnidadTributariaDocumento(utDocumento);
                utDoc.Documento = new Documento
                {
                    nombre_archivo = nombreArchivo,
                    descripcion = descripcion,
                    id_tipo_documento = idTipoDocumento,
                    Tipo = new TipoDocumento
                    {
                        TipoDocumentoId = idTipoDocumento,
                        Descripcion = tipoDocumentoDescripcion
                    }
                };
                unidadTributaria.UTDocumentos.Add(utDoc);

                UnidadMantenimientoParcelario.OperacionesUnidadTributariaDocumento.Add(new OperationItem<UnidadTributariaDocumento>
                {
                    Item = utDocumento,
                    Operation = Operation.Add
                });

                return Json(new { OK = true });
            }
            catch (Exception)
            {
                return Json(new { OK = false });
            }
        }

        [HttpPost]
        public ActionResult EditUnidadTributariaDocumento(long idUnidadTributaria, long idDocumento, string descripcion,
            long idTipoDocumento, string tipoDocumentoDescripcion)
        {
            try
            {
                var fechaHora = DateTime.Now;
                var utDocumento = new UnidadTributariaDocumento
                {
                    DocumentoID = idDocumento,
                    UnidadTributariaID = idUnidadTributaria,
                    UsuarioAltaID = Usuario.Id_Usuario,
                    FechaAlta = fechaHora,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                    FechaModificacion = fechaHora
                };

                UnidadMantenimientoParcelario.OperacionesUnidadTributariaDocumento.Add(new OperationItem<UnidadTributariaDocumento>
                {
                    Item = utDocumento,
                    Operation = Operation.Update
                });

                //Actualiza el documento en sesión
                var model = (Parcela)Session[_modelSession];
                var ut = model.UnidadesTributarias.Single(u => u.UnidadTributariaId == idUnidadTributaria);
                if (ut == null || ut.UTDocumentos == null) return Json(new { OK = true });
                var utDoc = ut.UTDocumentos.Single(d => d.DocumentoID == idDocumento);
                utDoc.Documento.descripcion = descripcion;
                utDoc.Documento.id_tipo_documento = idTipoDocumento;
                utDoc.Documento.Tipo.TipoDocumentoId = idTipoDocumento;
                utDoc.Documento.Tipo.Descripcion = tipoDocumentoDescripcion;
                Session[_modelSession] = model;

                return Json(new { OK = true });
            }
            catch (Exception)
            {
                return Json(new { OK = false });
            }
        }

        [HttpPost]
        public ActionResult DeleteUnidadTributariaDocumento(long idUnidadTributaria, long idDocumento)
        {
            try
            {
                Parcela parcela = (Parcela)Session[_modelSession];
                UnidadTributaria unidadTributaria = parcela.UnidadesTributarias.Single(ut => ut.UnidadTributariaId == idUnidadTributaria);
                UnidadTributariaDocumento unidadTributariaDocumento = unidadTributaria.UTDocumentos.Single(utd => utd.DocumentoID == idDocumento);
                unidadTributariaDocumento.UnidadTributaria = null;

                UnidadMantenimientoParcelario.OperacionesUnidadTributariaDocumento.Add(new OperationItem<UnidadTributariaDocumento>
                {
                    Item = unidadTributariaDocumento,
                    Operation = Operation.Remove,
                });

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/DeleteUnidadTributariaDocumento", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Unidades Tributarias Domicilios
        public string GetUTDomicilios(long idUT)
        {
            List<Domicilio> domicilios = new List<Domicilio>();
            try
            {
                var ut = ((Parcela)Session[_modelSession]).UnidadesTributarias.FirstOrDefault(u => u.UnidadTributariaId == idUT);
                if (ut != null && ut.UTDomicilios != null)
                {
                    domicilios = ut.UTDomicilios.Select(utd => utd.Domicilio).ToList();
                }

                return JsonConvert.SerializeObject(new { data = domicilios }, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch (Exception)
            {
                return "";
            }
        }

        [HttpPost]
        public ActionResult AddUnidadTributariaDomicilio(Domicilio domicilio, long idUT)
        {
            try
            {
                var utDomicilio = CreateUnidadTributariaDomicilio(domicilio, idUT);
                UnidadMantenimientoParcelario.OperacionesUnidadTributariaDomicilio.Add(new OperationItem<UnidadTributariaDomicilio>
                {
                    Item = utDomicilio,
                    Operation = Operation.Add
                });
                var ut = ((Parcela)Session[_modelSession]).UnidadesTributarias.SingleOrDefault(u => u.UnidadTributariaId == idUT);
                if (ut == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ut.UTDomicilios = ut.UTDomicilios ?? new List<UnidadTributariaDomicilio>();
                ut.UTDomicilios.Add(utDomicilio);
                return Json(utDomicilio.Domicilio);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/AddUnidadTributariaDomicilio", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        private T Clone<T>(T objeto)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(objeto, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        [HttpPost]
        public ActionResult EditUnidadTributariaDomicilio(Domicilio domicilio, long idUT)
        {
            try
            {
                if (domicilio.DomicilioId > 0)
                {
                    UnidadMantenimientoParcelario.OperacionesDomicilio.Add(new OperationItem<Domicilio>
                    {
                        Item = domicilio,
                        Operation = Operation.Update
                    });

                    var ut = ((Parcela)Session[_modelSession]).UnidadesTributarias.SingleOrDefault(u => u.UnidadTributariaId == idUT);
                    if (ut == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    if (ut.UTDomicilios == null || !ut.UTDomicilios.Any(d => d.DomicilioID == domicilio.DomicilioId))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        ut.UTDomicilios.Single(d => d.DomicilioID == domicilio.DomicilioId).Domicilio = domicilio;
                    }
                    return Json(domicilio);
                }
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/EditUnidadTributariaDomicilio", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        private UnidadTributariaDomicilio CreateUnidadTributariaDomicilio(Domicilio domicilio, long idUt)
        {
            var fechaHora = DateTime.Now;
            var itemDomicilio = new Domicilio
            {
                DomicilioId = domicilio.DomicilioId,
                ViaNombre = domicilio.ViaNombre,
                numero_puerta = domicilio.numero_puerta,
                piso = domicilio.piso,
                unidad = domicilio.unidad,
                barrio = domicilio.barrio,
                localidad = domicilio.localidad,
                municipio = domicilio.municipio,
                provincia = domicilio.provincia,
                pais = domicilio.pais,
                ubicacion = domicilio.ubicacion,
                codigo_postal = domicilio.codigo_postal,
                UsuarioAltaId = Usuario.Id_Usuario,
                FechaAlta = fechaHora,
                UsuarioModifId = Usuario.Id_Usuario,
                FechaModif = fechaHora,
                ViaId = domicilio.ViaId,
                TipoDomicilioId = domicilio.TipoDomicilioId,
                TipoDomicilio = Clone(domicilio.TipoDomicilio)
            };

            var unidadTributariaDomicilio = new UnidadTributariaDomicilio
            {
                DomicilioID = domicilio.DomicilioId,
                Domicilio = itemDomicilio,
                TipoDomicilioID = itemDomicilio.TipoDomicilioId,
                UsuarioAltaID = Usuario.Id_Usuario,
                FechaAlta = fechaHora,
                UsuarioModificacionID = Usuario.Id_Usuario,
                FechaModificacion = fechaHora
            };

            unidadTributariaDomicilio.UnidadTributariaID = idUt;

            return unidadTributariaDomicilio;
        }

        [HttpPost]
        public ActionResult DeleteUnidadTributariaDomicilio(long idUT, long idDomicilio)
        {
            var parcela = (Parcela)Session[_modelSession];
            if (idDomicilio < 0)
            {
                var operacionesUnidadTributariaDomicilio = UnidadMantenimientoParcelario.OperacionesUnidadTributariaDomicilio
                    .Single(x => x.Item.Domicilio.DomicilioId == idDomicilio);
                operacionesUnidadTributariaDomicilio.Operation = Operation.None;
            }
            else
            {
                var unidadTributariaDomicilio = new UnidadTributariaDomicilio
                {
                    DomicilioID = idDomicilio,
                    UnidadTributariaID = idUT,

                };

                UnidadMantenimientoParcelario.OperacionesUnidadTributariaDomicilio.Add(new OperationItem<UnidadTributariaDomicilio>
                {
                    Item = unidadTributariaDomicilio,
                    Operation = Operation.Remove,
                });
            }
            var domicilioModel = parcela.UnidadesTributarias.FirstOrDefault(ut => ut.UnidadTributariaId == idUT).UTDomicilios.FirstOrDefault(d => d.DomicilioID == idDomicilio);
            parcela.UnidadesTributarias.FirstOrDefault(ut => ut.UnidadTributariaId == idUT).UTDomicilios.Remove(domicilioModel);
            return Json(new { OK = true });
        }

        #endregion

        #region Nomenclaturas
        public string GetNomenclaturas()
        {
            var nomenclaturas = ((Parcela)Session[_modelSession]).Nomenclaturas.Where(n => n.FechaBaja == null);
            return JsonConvert.SerializeObject(new { data = nomenclaturas.ToList() }, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public ActionResult AddNomenclatura(Nomenclatura nomenclatura)
        {
            try
            {
                var fechaHora = DateTime.Now;
                var itemNomenclatura = new Nomenclatura
                {
                    Nombre = nomenclatura.Nombre,
                    NomenclaturaID = nomenclatura.NomenclaturaID,
                    ParcelaID = ((Parcela)Session[_modelSession]).ParcelaID,
                    TipoNomenclaturaID = nomenclatura.TipoNomenclaturaID,
                    UsuarioAltaID = Usuario.Id_Usuario,
                    FechaAlta = fechaHora,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                    FechaModificacion = fechaHora
                };

                UnidadMantenimientoParcelario.OperacionesNomenclatura.Add(new OperationItem<Nomenclatura>
                {
                    Item = itemNomenclatura,
                    Operation = Operation.Add
                });
                return Json(nomenclatura);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/AddNomenclatura", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult EditNomenclatura(Nomenclatura nomenclatura)
        {
            try
            {
                var nomenclaturaItem = new Nomenclatura
                {
                    FechaAlta = nomenclatura.FechaAlta,
                    UsuarioAltaID = nomenclatura.UsuarioAltaID,
                    FechaModificacion = DateTime.Now,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                    Nombre = nomenclatura.Nombre,
                    NomenclaturaID = nomenclatura.NomenclaturaID,
                    ParcelaID = ((Parcela)Session[_modelSession]).ParcelaID,
                    TipoNomenclaturaID = nomenclatura.TipoNomenclaturaID,
                };
                UnidadMantenimientoParcelario.OperacionesNomenclatura.Add(new OperationItem<Nomenclatura>
                {
                    Item = nomenclaturaItem,
                    Operation = Operation.Update,
                });

                return Json(nomenclatura);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/AddNomenclatura", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult DeleteNomenclatura(Nomenclatura nomenclatura)
        {
            try
            {
                var fechaHora = DateTime.Now;
                nomenclatura.UsuarioBajaID = Usuario.Id_Usuario;
                nomenclatura.FechaBaja = fechaHora;
                nomenclatura.UsuarioModificacionID = Usuario.Id_Usuario;
                nomenclatura.FechaModificacion = fechaHora;

                UnidadMantenimientoParcelario.OperacionesNomenclatura.Add(new OperationItem<Nomenclatura>
                {
                    Item = nomenclatura,
                    Operation = Operation.Remove,
                });
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/DeleteNomenclatura", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Unidades Tributarias
        public ActionResult AddUnidadTributaria(UnidadTributaria unidadTributaria)
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];

                var unidadtributariaItem = new UnidadTributaria
                {
                    UnidadTributariaId = unidadTributaria.UnidadTributariaId,
                    CodigoMunicipal = unidadTributaria.CodigoMunicipal,
                    CodigoProvincial = unidadTributaria.CodigoProvincial,
                    UnidadFuncional = unidadTributaria.UnidadFuncional,
                    Observaciones = unidadTributaria.Observaciones,
                    PorcentajeCopropiedad = unidadTributaria.PorcentajeCopropiedad,
                    Piso = unidadTributaria.Piso,
                    Unidad = unidadTributaria.Unidad,
                    FechaAlta = unidadTributaria.FechaAlta,
                    TipoUnidadTributariaID = unidadTributaria.TipoUnidadTributariaID,
                    TipoUnidadTributaria = unidadTributaria.TipoUnidadTributaria,
                    Vigencia = unidadTributaria.Vigencia,
                    PlanoId = unidadTributaria.PlanoId,
                    Superficie = unidadTributaria.Superficie,
                    FechaVigenciaDesde = unidadTributaria.FechaVigenciaDesde,
                    FechaVigenciaHasta = unidadTributaria.FechaVigenciaHasta,
                    JurisdiccionID = unidadTributaria.JurisdiccionID,
                };
                parcela.UnidadesTributarias.Add(unidadtributariaItem);
                UnidadMantenimientoParcelario.OperacionesUnidadTributaria.Add(new OperationItem<UnidadTributaria>
                {
                    Item = Clone(unidadtributariaItem),
                    Operation = Operation.Add
                });

                return Json(unidadtributariaItem);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/AddUnidadTributaria", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult EditUnidadTributaria(UnidadTributaria unidadTributaria)
        {
            try
            {
                unidadTributaria.ParcelaID = ((Parcela)Session[_modelSession]).ParcelaID;

                var ut = ((Parcela)Session[_modelSession]).UnidadesTributarias.Single(x => x.UnidadTributariaId == unidadTributaria.UnidadTributariaId);

                ut.CodigoMunicipal = unidadTributaria.CodigoMunicipal;
                ut.CodigoProvincial = unidadTributaria.CodigoProvincial;
                ut.UnidadFuncional = unidadTributaria.UnidadFuncional;
                ut.Observaciones = unidadTributaria.Observaciones;
                ut.PorcentajeCopropiedad = unidadTributaria.PorcentajeCopropiedad;
                ut.Piso = unidadTributaria.Piso;
                ut.Unidad = unidadTributaria.Unidad;
                ut.JurisdiccionID = unidadTributaria.JurisdiccionID;
                ut.PlanoId = unidadTributaria.PlanoId;
                ut.Superficie = unidadTributaria.Superficie;
                ut.FechaVigenciaDesde = unidadTributaria.FechaVigenciaDesde;
                ut.FechaVigenciaHasta = unidadTributaria.FechaVigenciaHasta;
                ut.Vigencia = unidadTributaria.Vigencia;

                UnidadMantenimientoParcelario.OperacionesUnidadTributaria.Add(new OperationItem<UnidadTributaria>
                {
                    Item = unidadTributaria,
                    Operation = Operation.Update
                });

                return Json(unidadTributaria);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/EditUnidadTributaria", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult DeleteUnidadTributaria(UnidadTributaria unidadTributaria)
        {
            try
            {
                //Marco los domicilios agregados como borrados
                var operacionesUTDomicilio = UnidadMantenimientoParcelario
                                                .OperacionesUnidadTributariaDomicilio
                                                .Where(i => i.Item.UnidadTributariaID == unidadTributaria.UnidadTributariaId ||
                                                            i.Item.UnidadTributaria.UnidadTributariaId == unidadTributaria.UnidadTributariaId);

                foreach (var op in operacionesUTDomicilio)
                {
                    op.Operation = Operation.Remove;
                }
                var parcela = (Parcela)Session[_modelSession];
                var ut = parcela.UnidadesTributarias.Single(x => x.UnidadTributariaId == unidadTributaria.UnidadTributariaId);
                ut.FechaBaja = ut.FechaModificacion = DateTime.Now;
                ut.UsuarioBajaID = ut.UsuarioModificacionID = Usuario.Id_Usuario;
                //parcela.UnidadesTributarias.Remove(ut);
                UnidadMantenimientoParcelario.OperacionesUnidadTributaria.Add(new OperationItem<UnidadTributaria>
                {
                    Item = new UnidadTributaria() { UnidadTributariaId = ut.UnidadTributariaId, CodigoProvincial = ut.CodigoProvincial },
                    Operation = Operation.Remove,
                });
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/DeleteUnidadTributaria", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        public ActionResult Test(long id)
        {
            using (var resp = cliente.GetAsync($"api/Parcela/{id}/esVigente").Result)
            {
                resp.EnsureSuccessStatusCode();
                bool esVigente = resp.Content.ReadAsAsync<bool>().Result;

                if (esVigente)
                {
                    return RedirectToAction("Get", new { id });
                }

                return PartialView("ParcelaNoVigente");
            }
        }

        [HttpPost]
        public ActionResult Save(decimal Superficie, long TipoParcelaID, long ClaseParcelaID, long EstadoParcelaID, string PlanoId, string ExpedienteAlta, DateTime FechaAltaExpediente, string ExpedienteBaja,
            DateTime? FechaBajaExpediente, string Observaciones, bool AfectaPH, long? AtributoZonaID)
        {
            //_WEBSERVICE_ INMBAJA VERIFICAR FECHA BAJA IS_CHANGE - LISTO -
            //IF(FECHA_BAJA != NULL) PROCESO_MODIF : (PROCESO_BAJA)
            try
            {
                var parcelaAnterior = (Parcela)Session[_modelSession];
                var parcela = new Parcela
                {
                    AtributoZonaID = AtributoZonaID,
                    ClaseParcelaID = ClaseParcelaID,
                    EstadoParcelaID = EstadoParcelaID,
                    ExpedienteAlta = ExpedienteAlta,
                    ExpedienteBaja = ExpedienteBaja,
                    FechaAltaExpediente = FechaAltaExpediente,
                    FechaBajaExpediente = FechaBajaExpediente,
                    OrigenParcelaID = parcelaAnterior.OrigenParcelaID,
                    ParcelaID = parcelaAnterior.ParcelaID,
                    Superficie = Superficie,
                    PlanoId = PlanoId,
                    TipoParcelaID = TipoParcelaID,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                    Atributos = parcelaAnterior.Atributos,
                    _Ip = Request.UserHostAddress,
                    _Machine_Name = AuditoriaHelper.ReverseLookup(Request.UserHostAddress).ToLower(),
                };
                parcela.AtributosCrear(Observaciones, AfectaPH);

                UnidadMantenimientoParcelario.OperacionesParcela.Clear();
                UnidadMantenimientoParcelario.OperacionesParcela.Add(new OperationItem<Parcela>
                {
                    Operation = Operation.Update,
                    Item = parcela,
                });

                var content = new ObjectContent<UnidadMantenimientoParcelario>(UnidadMantenimientoParcelario, new JsonMediaTypeFormatter());
                var response = cliente.PostAsync("api/Parcela/Post", content).Result;
                response.EnsureSuccessStatusCode();
                if (!response.Content.ReadAsAsync<bool>().Result)
                {
                    return Json(new { eliminada = true });
                }
                UnidadMantenimientoParcelario = new UnidadMantenimientoParcelario();
                Get(parcela.ParcelaID);
                #region ListadoComarcal
                //if (UnidadMantenimientoParcelario.OperacionesUnidadTributaria == null)
                //{
                //    List<webserviceInmModificar> webList = new List<webserviceInmModificar>();//DATOS UNIDAD TRIBUTARIAS


                //}
                #endregion

                //if (parcela.FechaBaja.HasValue)
                //{
                //    //GET PARTIDAS.
                //    var resp = cliente.GetAsync("api/parcela/GetPartidasbyParcela?idParcela=" + parcela.ParcelaID).Result;
                //    resp.EnsureSuccessStatusCode();
                //    var set = string.Join(",", resp.Content.ReadAsAsync<List<string>>().Result.ToArray());
                //    BajaComarcal(parcela.FechaBaja.Value, parcela.ParcelaID, set); // SE DA DE BAJA TODAS LAS UT DE LA PARCELA                    
                //}
                //else
                //{
                //    //IF PARTIDAS_BAJA != NULL
                //}

                //AuditoriaHelper.Register(Usuario.Id_Usuario, "Se grabo la Parcela", Request, TiposOperacion.Modificacion, Autorizado.Si, Eventos.ModificarParcela);
                //TODO: por operacion informar las altas , las bajas y/o modificaciones en unidades tributarias

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenedorParcelario-Save", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #region Informes
        public ActionResult GetInformeParcelario(long? id)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/informeParcelario/GetInforme/{id ?? ((Parcela)Session[_modelSession]).ParcelaID}/?padronPartidaId={@Recursos.MostrarPadrónPartida}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                AuditoriaHelper.Register(((Model.UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformeParcelario);
                ArchivoInforme = new Model.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteParcelario {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetInformeParcelarioVIR()
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/informeParcelarioVIR/GetInforme/{((Parcela)Session[_modelSession]).ParcelaID}/?padronPartidaId={@Recursos.MostrarPadrónPartida}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                ArchivoInforme = new Model.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteParcelarioVIR {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetInformeParcelarioBaja(long id, string partida)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/informeParcelarioBaja/GetInforme/{id}/?padronPartidaId={@Recursos.MostrarPadrónPartida}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                ArchivoInforme = new Model.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteParcelarioBaja {partida} {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetInformeHistoricoCambiosParcela(long? idTramite)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var parcela = Session[_modelSession] as Parcela;
            string identificador = parcela.UnidadesTributarias.Single(ut => (ut.TipoUnidadTributariaID == 2) || (ut.TipoUnidadTributariaID == 1)).CodigoProvincial;
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/InformeHistoricoCambios/GetCambiosParcela?id={parcela.ParcelaID}&identificador={identificador}&idTramite={idTramite}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                ArchivoInforme = new Models.ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformeHistoricoCambiosParcela {identificador} {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetInformeHistoricoCambiosUnidadTributaria(long idUnidadTributaria, long? idTramite)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var parcela = Session[_modelSession] as Parcela;
            string identificador = parcela.UnidadesTributarias.Single(ut => ut.UnidadTributariaId == idUnidadTributaria).CodigoProvincial;
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/InformeHistoricoCambios/GetCambiosUnidadTributaria?id={idUnidadTributaria}&identificador={identificador}&idTramite={idTramite}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                ArchivoInforme = new Models.ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformeHistoricoCambiosUnidadTributaria {identificador} {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public FileResult Abrir()
        {
            Response.AppendHeader("Content-Disposition", new ContentDisposition { FileName = ArchivoInforme.NombreArchivo, Inline = true }.ToString());
            return File(ArchivoInforme.Contenido, ArchivoInforme.MimeType);
        }
        public ActionResult GetInformeUT(long idUnidadTributaria)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var parcela = Session[_modelSession] as Parcela;
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/informeUnidadTributaria/GetInforme?idParcela={parcela.ParcelaID}&idUnidadTributaria={idUnidadTributaria}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                AuditoriaHelper.Register(((Model.UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformeUT);
                string codigoProvincial = parcela.UnidadesTributarias.Single(ut => ut.UnidadTributariaId == idUnidadTributaria).CodigoProvincial;
                ArchivoInforme = new Model.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteUnidadTributaria {codigoProvincial} {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetInformeUTBaja(long idUnidadTributaria, long idParcela, string partida)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/informeUnidadTributariaBaja/GetInforme?idParcela={idParcela}&idUnidadTributaria={idUnidadTributaria}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                ArchivoInforme = new Model.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteUnidadTributariaBaja {partida} {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }
        public ActionResult GetInformeUTFromSearch(long idUnidadTributaria, long idParcela)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/informeUnidadTributaria/GetInforme?idParcela={idParcela}&idUnidadTributaria={idUnidadTributaria}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                AuditoriaHelper.Register(((Model.UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformeUT);
                var parcela = GetParcela(idParcela);
                string codigoProvincial = parcela.UnidadesTributarias.Single(ut => ut.UnidadTributariaId == idUnidadTributaria).CodigoProvincial;
                ArchivoInforme = new Model.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteUnidadTributaria {codigoProvincial} {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }
        public ActionResult GetInformeUTProvincialFromSearch(long idUnidadTributaria, long idParcela, string partida)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesProvincialURL"]) })
            using (var resp = cliente.GetAsync($"api/informeUnidadTributaria/GetInforme?idParcela={idParcela}&idUnidadTributaria={idUnidadTributaria}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                ArchivoInforme = new Models.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteUnidadTributaria {partida} {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }
        public ActionResult GetInformeCatastral(int id)
        {
            var resp = clienteInformes.GetAsync("api/informeCatastral/GetInforme?id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            AuditoriaHelper.Register(((Model.UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformeCatastral);
            byte[] bytes = Convert.FromBase64String(bytes64);
            var cd = new ContentDisposition
            {
                FileName = "InformeCatastral.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }
        public ActionResult VerificarDeuda(long idParcela, string partidas, char tipo)
        {
            //p es verificar todas las partidas por parcela / i verifica la partida individual

            if (tipo == 'p')
            {
                var resp = cliente.GetAsync("api/parcela/GetPartidasbyParcela?idParcela=" + idParcela).Result;
                resp.EnsureSuccessStatusCode();

                var partidasParcela = resp.Content.ReadAsAsync<List<string>>().Result;
                if (partidasParcela.Count() != 0)
                {
                    var set = string.Join(",", partidasParcela.ToArray());
                    string serviceUrl = string.Format("api/parcela/GetVerificarDeuda?parcelaId={0}&partidas={1}", idParcela, set);
                    var resp2 = cliente.GetAsync(serviceUrl).Result;
                    resp2.EnsureSuccessStatusCode();
                    return Json(JsonConvert.SerializeObject(resp2.Content.ReadAsAsync<IEnumerable<EstadoPartida>>().Result), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new List<EstadoPartida>()
                        {
                            new EstadoPartida {partidaID = "-13", Estado = "NOTHING"},
                        });

                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string serviceUrl = string.Format("api/parcela/GetVerificarDeuda?parcelaId={0}&partidas={1}", idParcela, partidas);
                var resp3 = cliente.GetAsync(serviceUrl).Result;
                resp3.EnsureSuccessStatusCode();
                return Json(JsonConvert.SerializeObject(resp3.Content.ReadAsAsync<IEnumerable<EstadoPartida>>().Result), JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetInformeSituacionPartidaInmobiliaria(long Id)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var parcela = Session[_modelSession] as Parcela;
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/InformeSituacionPartidaInmobiliaria/GetInformeSituacionPartidaInmobiliaria?IdParcelaOperacion={Id}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                AuditoriaHelper.Register(((Model.UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformeSituacion);
                ArchivoInforme = new Model.ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformeSituacionPartidaInmobiliaria.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Parcela Valuaciones
        public ActionResult GetValuacionParcela()
        {
            try
            {
                using (cliente)
                {
                    var parcela = (Parcela)Session[_modelSession];
                    var resp = cliente.GetAsync($"api/valuacionservice/GetValuacionParcela/{parcela.ParcelaID}").Result;
                    resp.EnsureSuccessStatusCode();
                    return Json(new Model.MantenedorParcelarioValuacionModel(resp.Content.ReadAsAsync<VALValuacion>().Result), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/GetValuacionParcela", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Unidad Triburaria Valuaciones
        public ActionResult GetValuacionUnidadTributaria(long id)
        {
            try
            {
                using (cliente)
                {
                    var resp = cliente.GetAsync($"api/valuacionservice/GetValuacionUnidadTributaria/{id}").Result;
                    resp.EnsureSuccessStatusCode();
                    return Json(new Model.MantenedorParcelarioValuacionModel(resp.Content.ReadAsAsync<VALValuacion>().Result), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/GetValuacionUnidadTributaria", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Parcelas Origen
        public string GetParcelasOrigen()
        {
            var parcelaDestino = Session[_modelSession] as Parcela;
            using (var result = cliente.GetAsync($"api/parcela/getparcelasorigen?idparceladestino={parcelaDestino.ParcelaID}").Result)
            {
                result.EnsureSuccessStatusCode();
                var parcelas = result.Content.ReadAsAsync<IEnumerable<ParcelaOrigen>>().Result;

                //Negrada #1
                parcelaDestino.ParcelasOrigen = parcelas.Select(p => new ParcelaOperacion()
                {
                    ParcelaOperacionID = p.IdOperacion,
                    ParcelaOrigenID = p.IdParcela,
                    ParcelaOrigen = new Parcela() { UnidadesTributarias = new[] { new UnidadTributaria() { CodigoProvincial = p.CodigoProvincial } } },
                    TipoOperacionID = p.IdTipoOperacion,
                    FechaOperacion = p.FechaAlta
                }).ToList();
                return $"{{\"data\":{JsonConvert.SerializeObject(parcelas)}}}";
            }
        }
        public ActionResult GetParcelaOrigen(long? id = null)
        {
            ParcelaOperacion parcelaOperacion;
            var origenes = GetTiposParcelaOrigen();
            if (id == null)
            {
                parcelaOperacion = new ParcelaOperacion()
                {
                    ParcelaOrigenID = 0,
                    ParcelaOrigen = new Parcela() { UnidadesTributarias = new List<UnidadTributaria>() },
                    FechaOperacion = DateTime.Today
                };
            }
            else
            {
                parcelaOperacion = (Session[_modelSession] as Parcela).ParcelasOrigen.SingleOrDefault(po => po.ParcelaOperacionID == id);
            }
            ViewData["tiposOperacion"] = origenes;
            //Negrada #2
            return PartialView("ParcelaOrigen", new ParcelaOrigen()
            {
                CodigoProvincial = parcelaOperacion.ParcelaOrigen.UnidadesTributarias.SingleOrDefault()?.CodigoProvincial,
                IdOperacion = parcelaOperacion.ParcelaOperacionID,
                IdTipoOperacion = parcelaOperacion.TipoOperacionID,
                IdParcela = parcelaOperacion.ParcelaOrigenID.Value,
                FechaAlta = parcelaOperacion.FechaOperacion.Value,
            });
        }
        public ActionResult SaveParcelaOrigen(ParcelaOrigen relacionOrigen)
        {
            try
            {
                var tiposParcela = GetTiposParcelas();
                using (var resp = cliente.GetAsync($"api/parcela/{relacionOrigen.IdParcela}/simple").Result.EnsureSuccessStatusCode())
                {
                    var parcelaOrigen = resp.Content.ReadAsAsync<Parcela>().Result;

                    relacionOrigen.TipoParcela = tiposParcela.Single(tp => tp.Value == parcelaOrigen.TipoParcelaID.ToString()).Text;

                    var parcela = Session[_modelSession] as Parcela;
                    var operacion = Operation.Update;
                    var existente = parcela.ParcelasOrigen.SingleOrDefault(po => po.ParcelaOperacionID == relacionOrigen.IdOperacion);
                    if (existente == null)
                    {
                        existente = new ParcelaOperacion()
                        {
                            ParcelaOperacionID = relacionOrigen.IdOperacion,
                            ParcelaDestinoID = parcela.ParcelaID
                        };
                        parcela.ParcelasOrigen.Add(existente);
                        operacion = Operation.Add;
                    }
                    existente.FechaOperacion = relacionOrigen.FechaAlta;
                    existente.ParcelaOrigenID = relacionOrigen.IdParcela;
                    existente.ParcelaOrigen = new Parcela() { UnidadesTributarias = new[] { new UnidadTributaria() { CodigoProvincial = relacionOrigen.CodigoProvincial } } };
                    existente.TipoOperacionID = relacionOrigen.IdTipoOperacion;

                    UnidadMantenimientoParcelario.OperacionesParcelaOrigen.Add(new OperationItem<ParcelaOperacion>
                    {
                        Item = Clone(existente),
                        Operation = operacion
                    });
                    return Json(relacionOrigen);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController->SaveParcelaOrigen", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult DeleteParcelaOrigen(long id)
        {
            var parcela = Session[_modelSession] as Parcela;
            var parcelaOrigen = parcela.ParcelasOrigen.SingleOrDefault(po => po.ParcelaOperacionID == id);
            UnidadMantenimientoParcelario.OperacionesParcelaOrigen.Add(new OperationItem<ParcelaOperacion>
            {
                Item = parcelaOrigen,
                Operation = Operation.Remove
            });
            parcela.ParcelasOrigen.Remove(parcelaOrigen);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        private List<TipoParcelaOperacion> GetTiposParcelaOrigen()
        {
            try
            {
                using (var resp = cliente.GetAsync($"api/TipoParcelaOperacion/Get").Result.EnsureSuccessStatusCode())
                {
                    return resp.Content.ReadAsAsync<List<TipoParcelaOperacion>>().Result;
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController->GetTiposParcelaOrigen", ex);
                throw;
            }

        }
        #endregion

        private List<SelectListItem> GetTiposParcelas()
        {
            List<SelectListItem> lista = null;
            HttpResponseMessage response = cliente.GetAsync("api/TipoParcela/Get").Result;
            response.EnsureSuccessStatusCode();
            ICollection<TipoParcela> tiposParcelas = response.Content.ReadAsAsync<ICollection<TipoParcela>>().Result;
            lista = tiposParcelas.Select(tp => new SelectListItem { Text = tp.Descripcion, Value = tp.TipoParcelaID.ToString() }).ToList();

            return lista;
        }

        private List<SelectListItem> GetClasesParcelas()
        {
            List<SelectListItem> lista = null;
            HttpResponseMessage response = cliente.GetAsync("api/ClaseParcela/Get").Result;
            response.EnsureSuccessStatusCode();
            ICollection<ClaseParcela> clasesParcela = response.Content.ReadAsAsync<ICollection<ClaseParcela>>().Result;
            lista = clasesParcela.Select(cp => new SelectListItem { Text = cp.Descripcion, Value = cp.ClaseParcelaID.ToString() }).ToList();

            return lista;
        }

        private List<SelectListItem> GetEstadosParcelas()
        {
            List<SelectListItem> lista = null;
            HttpResponseMessage response = cliente.GetAsync("api/EstadoParcela/Get").Result;
            response.EnsureSuccessStatusCode();
            ICollection<EstadoParcela> estadosParcelas = response.Content.ReadAsAsync<ICollection<EstadoParcela>>().Result;
            lista = estadosParcelas.Select(ep => new SelectListItem { Text = ep.Descripcion, Value = ep.EstadoParcelaID.ToString() }).ToList();

            return lista;
        }

        private SelectList GetZonasValuaciones()
        {
            var response = cliente.GetAsync("api/Parcela/GetParcelaValuacionZonas").Result;
            response.EnsureSuccessStatusCode();
            var objetos = response.Content.ReadAsAsync<ICollection<Models.Objeto>>().Result;
            return new SelectList(objetos, "FeatId", "Nombre");
        }

        private SelectList GetZonasTributarias()
        {
            var response = cliente.GetAsync("api/InterfaseRentas/GetZonasTributarias").Result;
            response.EnsureSuccessStatusCode();
            var objetos = response.Content.ReadAsAsync<ICollection<Objeto>>().Result;
            return new SelectList(objetos, "Codigo", "Nombre");
        }

        //private TipoNomenclatura GetNomenclaturaById(long tipoNomenclaturaID)
        //{
        //    var resp = cliente.GetAsync("api/TipoNomenclatura/GetById?Id=" + tipoNomenclaturaID).Result;
        //    resp.EnsureSuccessStatusCode();
        //    return resp.Content.ReadAsAsync<TipoNomenclatura>().Result;
        //}

        #region Responsables Fiscales

        public string GetUtResponsablesFiscal(long idUnidadTributaria)
        {
            var result = cliente.GetAsync("api/unidadtributariapersona/get?idunidadtributaria=" + idUnidadTributaria).Result;
            result.EnsureSuccessStatusCode();
            var responsablesFiscales = result.Content.ReadAsAsync<IEnumerable<ResponsableFiscal>>().Result;
            if (responsablesFiscales != null)
            {
                foreach (var responsableFiscal in responsablesFiscales)
                {
                    responsableFiscal.UnidadTributariaId = idUnidadTributaria;
                }
            }
            return "{\"data\":" + JsonConvert.SerializeObject(responsablesFiscales) + "}";
        }

        public string GetCambioMunicipio()
        {
            var result = cliente.GetAsync("api/parametro/getvalor/" + @Recursos.CambioMunicipio).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsStringAsync().Result;
        }

        public ActionResult SaveResponsableFiscal(ResponsableFiscalViewModel responsableFiscalViewModel)
        {
            var utPersona = new UtPersona
            {
                OperacionesUnidadTributariaPersona = UnidadMantenimientoParcelario.OperacionesUnidadTributariaPersona,
                UnidadTributariaId = responsableFiscalViewModel.UnidadTributariaId,
                PersonaId = responsableFiscalViewModel.PersonaId,
                SavedPersonaId = responsableFiscalViewModel.SavedPersonaId,
                Operacion = responsableFiscalViewModel.Operacion
            };

            using (var result = cliente.PostAsync("api/unidadtributariapersona/validate", new ObjectContent<UtPersona>(utPersona, new JsonMediaTypeFormatter())).Result)
            {
                result.EnsureSuccessStatusCode();
                string response = result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(response))
                {
                    return Json(new { error = true, mensaje = response });
                }
                responsableFiscalViewModel.SavedPersonaId = utPersona.PersonaId;
            }
            UnidadMantenimientoParcelario.OperacionesUnidadTributariaPersona.Add(new OperationItem<UnidadTributariaPersona>
            {
                Item = new UnidadTributariaPersona
                {
                    UnidadTributariaID = responsableFiscalViewModel.UnidadTributariaId,
                    PersonaID = responsableFiscalViewModel.PersonaId,
                    PersonaSavedId = responsableFiscalViewModel.SavedPersonaId,
                    TipoPersonaID = responsableFiscalViewModel.TipoPersonaId,
                    CodSistemaTributario = responsableFiscalViewModel.CodSistemaTributario,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                },
                Operation = responsableFiscalViewModel.Operacion
            });

            return Json(responsableFiscalViewModel);
        }

        public ActionResult DeleteResponsableFiscal(ResponsableFiscalViewModel responsableFiscalViewModel)
        {
            try
            {
                var unidadTributariaPersona = new UnidadTributariaPersona
                {
                    UnidadTributariaID = responsableFiscalViewModel.UnidadTributariaId,
                    PersonaID = responsableFiscalViewModel.PersonaId,
                    UsuarioModificacionID = Usuario.Id_Usuario,
                };

                UnidadMantenimientoParcelario.OperacionesUnidadTributariaPersona.Add(new OperationItem<UnidadTributariaPersona>
                {
                    Item = unidadTributariaPersona,
                    Operation = Operation.Remove
                });

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("MantenimientoParcelarioController/DeleteResponsableFiscal", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Dominios

        public string GetUtDominios(long idUnidadTributaria)
        {
            var result = cliente.GetAsync("api/dominio/get?idunidadtributaria=" + idUnidadTributaria).Result;
            result.EnsureSuccessStatusCode();
            var dominios = result.Content.ReadAsAsync<IEnumerable<DominioUT>>().Result.ToList();
            var dominiosOper = UnidadMantenimientoParcelario.OperacionesDominio
                .Where(dominioOper => dominioOper.Item.UnidadTributariaID == idUnidadTributaria).ToList();

            dominios.AddRange(dominiosOper
                .Where(dominioOper => dominioOper.Operation != Operation.Remove)
                .Select(dominioOper => new DominioUT
                {
                    DominioID = dominioOper.Item.DominioID,
                    Fecha = dominioOper.Item.Fecha,
                    Inscripcion = dominioOper.Item.Inscripcion,
                    TipoInscripcion = dominioOper.Item.TipoInscripcionDescripcion,
                    TipoInscripcionID = dominioOper.Item.TipoInscripcionID,
                    // Titulares = (IEnumerable<Titular>)dominioOper.Item.Titulares
                }));

            foreach (var dominioOper in dominiosOper)
            {
                dominios.RemoveAll(x => x.DominioID == dominioOper.Item.DominioID);
            }
            var datos = dominios.Select(d => new DominioViewModel()
            {
                DominioID = d.DominioID,
                UnidadTributariaID = idUnidadTributaria,
                Inscripcion = d.Inscripcion,
                Fecha = d.Fecha,
                FechaHora = d.Fecha.ToShortDateString(),
                TipoInscripcionID = d.TipoInscripcionID,
                TipoInscripcionDescripcion = d.TipoInscripcion,
                // Titulares = d.Titulares
            });
            return "{\"data\":" + JsonConvert.SerializeObject(datos) + "}";
        }

        public ActionResult SaveDominio(DominioViewModel dominioViewModel)
        {
            var utDominio = new UtDominio
            {
                UnidadTributariaId = dominioViewModel.UnidadTributariaID,
                DominioId = dominioViewModel.DominioID,
                Inscripcion = dominioViewModel.Inscripcion,
                OperacionesDominio = UnidadMantenimientoParcelario.OperacionesDominio
            };

            using (var result = cliente.PostAsync("api/dominio/validate", new ObjectContent<UtDominio>(utDominio, new JsonMediaTypeFormatter())).Result)
            {
                result.EnsureSuccessStatusCode();
                string response = result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(response))
                {
                    return Json(new { error = true, mensaje = response });
                }
            }
            dominioViewModel.Fecha = DateTime.Parse(dominioViewModel.FechaHora, System.Globalization.CultureInfo.CurrentUICulture);
            UnidadMantenimientoParcelario.OperacionesDominio.Add(new OperationItem<Dominio>
            {
                Item = new Dominio
                {
                    DominioID = dominioViewModel.DominioID,
                    UnidadTributariaID = dominioViewModel.UnidadTributariaID,
                    TipoInscripcionID = dominioViewModel.TipoInscripcionID,
                    TipoInscripcionDescripcion = dominioViewModel.TipoInscripcionDescripcion,
                    Inscripcion = dominioViewModel.Inscripcion,
                    Fecha = dominioViewModel.Fecha,
                    IdUsuarioModif = Usuario.Id_Usuario,
                },
                Operation = dominioViewModel.Operacion
            });

            return Json(dominioViewModel);
        }

        public ActionResult DeleteDominio(DominioViewModel dominio)
        {
            UnidadMantenimientoParcelario.OperacionesDominio.Add(new OperationItem<Dominio>
            {
                Item = new Dominio
                {
                    DominioID = dominio.DominioID
                },
                Operation = Operation.Remove
            });

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion

        #region Titulares

        public JsonResult GetPersonaDatos(long id)
        {
            var result = cliente.GetAsync("api/persona/getdatos/" + id).Result;
            result.EnsureSuccessStatusCode();
            var persona = result.Content.ReadAsAsync<Persona>().Result;

            return Json(persona);
        }

        public string GetUtTitulares(long idDominio)
        {
            if (!TitularesDominio.ContainsKey(idDominio))
            {
                var result = cliente.GetAsync("api/dominiotitular/get?iddominio=" + idDominio).Result;
                result.EnsureSuccessStatusCode();
                TitularesDominio.Add(idDominio, result.Content
                                                      .ReadAsAsync<IEnumerable<Titular>>()
                                                      .Result.Select(x => new TitularViewModel
                                                      {
                                                          PersonaId = x.PersonaId,
                                                          DominioId = idDominio,
                                                          NombreCompleto = x.NombreCompleto,
                                                          PorcientoCopropiedad = x.PorcientoCopropiedad,
                                                          TipoNoDocumento = x.TipoNoDocumento,
                                                          TipoTitularidadId = x.TipoTitularidadId
                                                      }).ToList());
            }

            return "{\"data\":" + JsonConvert.SerializeObject(TitularesDominio[idDominio]) + "}";
        }

        public ActionResult SaveTitular(TitularViewModel titularViewModel)
        {
            var dominioTitular = new DominioTitular
            {
                DominioID = titularViewModel.DominioId,
                PersonaID = titularViewModel.PersonaId,
                TipoTitularidadID = titularViewModel.TipoTitularidadId,
                PorcientoCopropiedad = titularViewModel.PorcientoCopropiedad
            };

            if (titularViewModel.Operacion == Operation.Add)
            {
                var domTitular = new DomTitular
                {
                    OperacionesDominioTitular = UnidadMantenimientoParcelario.OperacionesDominioTitular,
                    DominioId = titularViewModel.DominioId,
                    PersonaId = titularViewModel.PersonaId
                };

                using (var result = cliente.PostAsync("api/dominiotitular/validate", new ObjectContent<DomTitular>(domTitular, new JsonMediaTypeFormatter())).Result)
                {
                    result.EnsureSuccessStatusCode();
                    string response = result.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(response))
                    {
                        return Json(new { error = true, mensaje = response });
                    }
                }
                (TitularesDominio[titularViewModel.DominioId] = TitularesDominio[titularViewModel.DominioId] ?? new List<TitularViewModel>()).Add(titularViewModel);
            }
            else
            {
                TitularesDominio[titularViewModel.DominioId][TitularesDominio[titularViewModel.DominioId].FindIndex(tv => tv.PersonaId == titularViewModel.PersonaId)] = titularViewModel;
            }

            UnidadMantenimientoParcelario.OperacionesDominioTitular.Add(new OperationItem<DominioTitular>
            {
                Item = dominioTitular,
                Operation = titularViewModel.Operacion
            });

            return Json(titularViewModel);
        }

        public ActionResult DeleteTitular(TitularViewModel titularViewModel)
        {
            UnidadMantenimientoParcelario.OperacionesDominioTitular.Add(new OperationItem<DominioTitular>
            {
                Item = new DominioTitular()
                {
                    DominioID = titularViewModel.DominioId,
                    PersonaID = titularViewModel.PersonaId
                },
                Operation = Operation.Remove
            });
            TitularesDominio[titularViewModel.DominioId].RemoveAll(tv => tv.PersonaId == titularViewModel.PersonaId);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion

        #region Estado de Deudas

        public JsonResult GetYears()
        {
            var result = cliente.GetAsync("api/estadodeuda/getyears").Result;
            result.EnsureSuccessStatusCode();
            var rentas = result.Content.ReadAsAsync<IEnumerable<int>>().Result;

            return Json(rentas);
        }

        public string GetServiciosGenerales(string padron)
        {
            //var result = cliente.GetAsync("api/estadodeuda/getserviciosgenerales?padron=" + padron).Result;
            //result.EnsureSuccessStatusCode();
            //var serviciosGenerales = result.Content.ReadAsAsync<IEnumerable<EstadoDeudaServicioGeneral>>().Result;

            //return "{\"data\":" + JsonConvert.SerializeObject(serviciosGenerales) + "}";
            return "{\"data\":[]}";
        }

        public string GetRentas(int year)
        {
            var result = cliente.GetAsync("api/estadodeuda/getrentas?year=" + year).Result;
            result.EnsureSuccessStatusCode();
            var rentas = result.Content.ReadAsAsync<IEnumerable<EstadoDeudaRenta>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(rentas) + "}";
        }

        #endregion

        public T CloneUnidadTributariaDocumento<T>(T unidadTributariaDocumento)
        {
            var json = JsonConvert.SerializeObject(unidadTributariaDocumento);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public ActionResult BajaComarcal(DateTime fechaBaja, long parcelaId, string partidas)
        {
            string url = string.Format("api/Parcela/BajaComarcal?fechaBaja={0}&parcelaId={1}&partidas={2}",
                fechaBaja.ToString("yyyy-MM-dd HH:mm:ss"), parcelaId, partidas);
            var result = cliente.PostAsync(url, new StringContent(string.Empty)).Result;
            result.EnsureSuccessStatusCode();

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult GetParcelaDesignaciones()
        {
            var parcela = Session[_modelSession] as Parcela;
            var designaciones = parcela.Designaciones?.Select(d => new Models.DesignacionParcelaModel(d, parcela)).ToArray();
            parcela = null;
            return Json(new { data = designaciones }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddParcelaDesignacion(Designacion designacion)
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];
                parcela.Designaciones = new List<Designacion>((parcela.Designaciones ?? new Designacion[0]).Concat(new[] { designacion }));

                UnidadMantenimientoParcelario.OperacionesDesignaciones.Add(new OperationItem<Designacion>
                {
                    Operation = Operation.Add,
                    Item = designacion
                });

                return Json(new Models.DesignacionParcelaModel(designacion, parcela));
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("AddParcelaDesignacion", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult EditParcelaDesignacion(Designacion designacion)
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];
                parcela.Designaciones = new List<Designacion>(parcela.Designaciones.Where(d => d.IdDesignacion != designacion.IdDesignacion).Concat(new[] { designacion }));

                UnidadMantenimientoParcelario.OperacionesDesignaciones.Add(new OperationItem<Designacion>
                {
                    Operation = Operation.Update,
                    Item = designacion
                });

                return Json(new Models.DesignacionParcelaModel(designacion, parcela));
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("EditParcelaDesignacion", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult DeleteParcelaDesignacion(long idDesignacion)
        {
            try
            {
                var parcela = (Parcela)Session[_modelSession];
                var current = parcela.Designaciones.SingleOrDefault(d => d.IdDesignacion == idDesignacion);
                if (current == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
                parcela.Designaciones = new List<Designacion>(parcela.Designaciones.Except(new[] { current }));

                UnidadMantenimientoParcelario.OperacionesDesignaciones.Add(new OperationItem<Designacion>
                {
                    Operation = Operation.Remove,
                    Item = current
                });
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("DeleteParcelaDesignacion", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public FileContentResult DownloadDocumento(long id)
        {
            using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            using (var resp = cliente.GetAsync($"api/DocumentoService/{id}/File").Result)
            {
                var doc = resp.Content.ReadAsAsync<DocumentoArchivo>().Result;
                return File(doc.Contenido, doc.ContentType, doc.NombreArchivo);
            }
        }
        public ActionResult GetInformeParcelarioProvincial(long id)
        {
            string usuario = $"{((Model.UsuariosModel)Session["usuarioPortal"]).Nombre} {((Model.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesProvincialURL"]) })
            using (var resp = cliente.GetAsync($"api/informeParcelario/GetInforme/{id}/?padronPartidaId={@Recursos.MostrarPadrónPartida}&usuario={usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                ArchivoInforme = new Models.ArchivoDescarga(Convert.FromBase64String(bytes64), $"ReporteParcelario {DateTime.Now:dd-MM-yyyy HH:mm:ss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult PuedeAgregarDesignacion(long idClaseParcela)
        {
            using (var result = cliente.GetAsync($"api/designacion/GetTiposDesignador").Result)
            {
                var parcela = Session["Parcela"] as Parcela;
                result.EnsureSuccessStatusCode();
                var tipos = result.Content.ReadAsAsync<IEnumerable<TipoDesignador>>().Result;
                var disponibles = tipos.Where(t => parcela.Designaciones.All(d => t.IdTipoDesignador != d.IdTipoDesignador));

                if (!disponibles.Any() || disponibles.Count() == 1 && disponibles.SingleOrDefault().Nombre == "TITULO" && idClaseParcela == 2)
                {
                    return Content(string.Empty);
                }
                else
                {
                    return Content("Ok");
                }

            }
        }

        public static string GetFormatedSuperficie(decimal superficie)
        {
            string[] partes = superficie.ToString().Split('.');
            int decimales = (System.Web.HttpContext.Current.Session[_modelSession] as Parcela).TipoParcelaID != 1 ? 4 : 2;
            return $"{partes[0]}.{partes[1].PadRight(decimales, '0')}";
        }
    }
}