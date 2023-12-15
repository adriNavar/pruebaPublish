using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using System.Net.Mime;
using GeoSit.Client.Web.Models;
using GeoSit.Client.Web.Models.ObrasPublicas;
using System.Net.Http.Formatting;
using System.Data;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Client.Web.Helpers;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Client.Web.Controllers.ObrasPublicas
{
    public class TramitesCertificadosController : Controller
    {
        private List<PerfilFuncion> FuncionesHabilitadas { get { return Session["FuncionesHabilitadas"] as List<PerfilFuncion>; } }
        private UsuariosModel Usuario { get { return Session["usuarioPortal"] as UsuariosModel; } }
        private int? Permisos_Tramite_Activado { get { return (int?)Session["PermisosTramite"]; } set { Session["PermisosTramite"] = value; } }

        private bool? PermisoTramiteFinalizado { get { return (bool?)Session["PermisoTramiteFinalizado"]; } set { Session["PermisoTramiteFinalizado"] = value; } }

        private HttpClient clienteInformes = new HttpClient();
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/TramitesCertificadosService/";
        private OperationItem<Documento> InformeImpreso
        {
            get { return Session["InformeImpreso"] as OperationItem<Documento>; }
            set { Session["InformeImpreso"] = value; }
        }

        private UnidadGeneracionCertificados UnidadGeneracionCertificados
        {
            get { return Session["UnidadGeneracionCertificados"] as UnidadGeneracionCertificados; }
            set { Session["UnidadGeneracionCertificados"] = value; }
        }
        private long CertificadoId { get { return Convert.ToInt64(Session["CertificadoId"]); } set { Session["CertificadoId"] = value; } }

        TramiteModel mTramite_Cabecera = new TramiteModel();

        public TramitesCertificadosController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            clienteInformes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        // GET: TramitesCertificados
        public ActionResult Index()
        {
            return PartialView();
        }

        public string _ObjetosTramitesCertificados(long? pTipoId, long? pNumDesde, long? pNumHasta, string pFechaDesde, string pFechaHasta, int? pEstadoId, int? pUnidadT, int? pIdTramite, string pIdentificador)
        {

            List<Tramite> resultadoBusqueda = GetObjetosTramitesCertificadosByCriteria(pTipoId, pNumDesde, pNumHasta, pFechaDesde, pFechaHasta, pEstadoId, pUnidadT, pIdTramite, pIdentificador);


            return "{\"data\":" + JsonConvert.SerializeObject(resultadoBusqueda) + "}";
        }

        public ActionResult _InitForm()
        {
            ViewBag.TipoTramiteList = Session["TipoTramite"] as IEnumerable<SelectListItem>;
            ViewBag.TramiteEstados = mTramite_Cabecera.TramiteEstados.Select(i => new SelectListItem { Text = i.Descripcion.TrimEnd(), Value = i.Id_Estado.ToString() });
            return PartialView("_ObjetoTramiteCertificadoABM", new Tramite());
        }
        public ActionResult _ObjetoTramiteCertificadoABM(long idTramite, long idTipoTramite)
        {
            var tramite = new Tramite();
            var tipos = Session["TipoTramite"] as IEnumerable<SelectListItem>;

            this.UnidadGeneracionCertificados = new UnidadGeneracionCertificados();

            if (tipos != null)
            {
                tramite.Fecha = DateTime.Today;

                ViewBag.TipoTramiteList = tipos;
            }

            if (idTramite > 0)
            {
                tramite = GetObjetosTramitesCertificados(idTramite);
                ViewBag.TramiteEstados = mTramite_Cabecera.TramiteEstados.Where(e => e.Id_Estado == 2 || e.Id_Estado == 3).Select(i => new SelectListItem { Text = i.Descripcion.TrimEnd(), Value = i.Id_Estado.ToString() });
            }
            else
            {
                var tipo = GetTipos().Single(t => t.Id_Tipo_Tramite == idTipoTramite);
                if (tipo.Autonumerico) tramite.Cod_Tramite = (tipo.Numerador.GetValueOrDefault() + 1).ToString();
                tramite.Id_Tipo_Tramite = tipo.Id_Tipo_Tramite;
                ViewBag.TramiteEstados = mTramite_Cabecera.TramiteEstados.Where(e => e.Id_Estado == 1).Select(i => new SelectListItem { Text = i.Descripcion.TrimEnd(), Value = i.Id_Estado.ToString() });
            }

            return PartialView("_ObjetoTramiteCertificadoABM", tramite);
        }

        public JsonResult ValidateIdentificador(string identificador)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/ValidateIdentificador?identificador=" + identificador).Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = (TramiteModel)resp.Content.ReadAsAsync<TramiteModel>().Result;

            return Json(lstObjeto, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BusquedaTramitesCertificados(long? id)
        {
            //
            this.UnidadGeneracionCertificados = new UnidadGeneracionCertificados();

            Session["TipoTramite"] = GetTipos().Select(i => new SelectListItem() { Text = i.Nombre.TrimEnd(), Value = i.Id_Tipo_Tramite.ToString(), Disabled = i.Fecha_Baja == null ? false : true });
            Session["Roles"] = GetRoles().Select(i => new SelectListItem() { Text = i.Descripcion.TrimEnd(), Value = i.Id_Rol.ToString() });
            if (Session["TipoTramite"] != null)
            {
                ViewBag.TipoTramiteList = Session["TipoTramite"];
            }

            if (id != null)
            {
                ViewBag.BuscaId = 1;
            }
            else
            {
                ViewBag.BuscaId = 0;
            }
            ViewBag.TramiteEstados = mTramite_Cabecera.TramiteEstados.Select(i => new SelectListItem() { Text = i.Descripcion.TrimEnd(), Value = i.Id_Estado.ToString() });
            ViewBag.TramiteId = id;

            return PartialView();

        }

        public JsonResult UpdateTipoTramite(long id)
        {
            var tipo = GetTipos().Single(t => t.Id_Tipo_Tramite == id);
            return Json(tipo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTramiteCertificado(long Id_Tramite)
        {
            HttpResponseMessage resp = _cliente.PostAsync(ApiUri + "/DeleteTramiteCertificado", new Tramite { Id_Tramite = Id_Tramite, Id_Usu_Baja = Usuario.Id_Usuario }, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();


            AuditoriaHelper.Register(Usuario.Id_Usuario, "Se ha dado de baja el Tramite.",
                          Request, TiposOperacion.Baja, Autorizado.Si, Eventos.BajadeTramite);

            return Json(new { success = true });

        }

        public string GetTiposJson()
        {
            var tipo = JsonConvert.SerializeObject(GetTipos());
            return tipo;
        }
        public List<TipoTramite> GetTipos()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetTipos").Result;
            resp.EnsureSuccessStatusCode();
            var lstTipoObjeto = (List<TipoTramite>)resp.Content.ReadAsAsync<IEnumerable<TipoTramite>>().Result;
            return lstTipoObjeto;
        }

        public List<Tramite> GetObjetosTramitesCertificados()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesCertificados").Result;
            resp.EnsureSuccessStatusCode();
            List<Tramite> lstObjeto = resp.Content.ReadAsAsync<IEnumerable<Tramite>>().Result.ToList();

            return lstObjeto;
        }
        public List<Tramite> GetObjetosTramitesCertificadosByCriteria(long? pTipoId, long? pNumDesde, long? pNumHasta, string pFechaDesde, string pFechaHasta, int? pEstadoId, int? pUnidadT, int? pIdTramite, string pIndetificador)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesCertificadosByCriteria?pTipoId=" + pTipoId + "&pNumDesde=" + pNumDesde + "&pNumHasta=" + pNumHasta + "&pFechaDesde=" + pFechaDesde + "&pFechaHasta=" + pFechaHasta + "&pEstadoId=" + pEstadoId + "&pUnidadT=" + pUnidadT + "&pIdTramite=" + pIdTramite + "&pIdentificador=" + pIndetificador).Result;
            resp.EnsureSuccessStatusCode();
            List<Tramite> lstObjeto = resp.Content.ReadAsAsync<IEnumerable<Tramite>>().Result.ToList();

            return lstObjeto;
        }

        public Tramite GetObjetosTramitesCertificados(long Id_Tramite)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesCertificados?idTramite=" + Id_Tramite).Result;
            resp.EnsureSuccessStatusCode();
            Tramite lstObjeto = resp.Content.ReadAsAsync<Tramite>().Result;

            return lstObjeto;
        }

        public ActionResult SaveAll(TramiteSaveModel model)
        {
            var ret = new JsonResult();
            var operacion = CertificadoId == 0 ? Operation.Add : Operation.Update;

            var tramite = new Tramite
            {
                Id_Tramite = CertificadoId,
                Id_Tipo_Tramite = model.Tipo,
                Cod_Tramite = model.Identificador,
                Nro_Tramite = model.NumeroTramite,
                Imprime_Doc = model.ImprimeDocumentos,
                Imprime_Per = model.ImprimePersonas,
                Imprime_UTS = model.ImprimeUTS,
                Informe_Final = model.InformeFinal,
                Imprime_Final = model.ImprimeInformeFinal,
                Estado = model.Estado,
                Fecha = DateTime.Parse(model.FechaInicio),
                Fecha_Modif = DateTime.Now
            };
            tramite.Id_Usu_Modif = Usuario.Id_Usuario;
            //Si es un alta configura fecha alta y usuario alta.
            if (CertificadoId == 0)
            {
                tramite.Fecha_Alta = tramite.Fecha_Modif;
                tramite.Id_Usu_Alta = tramite.Id_Usu_Modif;
            }
            else
            {
                var tramiteOld = new Tramite();
                tramiteOld = GetObjetosTramitesCertificados(CertificadoId);
                tramite.Fecha_Alta = tramiteOld.Fecha_Modif;
                tramite.Id_Usu_Alta = tramiteOld.Id_Usu_Modif;
            }

            UnidadGeneracionCertificados.OperacionTramite = new OperationItem<Tramite> { Item = tramite, Operation = operacion };

            var result = _cliente.PostAsync(ApiUri + "/validate", new ObjectContent<UnidadGeneracionCertificados>(UnidadGeneracionCertificados, new JsonMediaTypeFormatter())).Result;
            result.EnsureSuccessStatusCode();
            var response = result.Content.ReadAsStringAsync().Result;
            if (response != string.Empty) return new JsonResult { Data = response };

            if (InformeImpreso != null)
            {
                UnidadGeneracionCertificados.InformeImpreso = InformeImpreso;
            }

            result = _cliente.PostAsync(ApiUri + "/saveall", new ObjectContent<UnidadGeneracionCertificados>(UnidadGeneracionCertificados, new JsonMediaTypeFormatter())).Result;
            result.EnsureSuccessStatusCode();

            var tramiteGuardado = result.Content.ReadAsAsync<Tramite>().Result;
            UnidadGeneracionCertificados = new UnidadGeneracionCertificados();
            InformeImpreso = null;

            if (operacion == Operation.Add)//
            {
                AuditoriaHelper.Register(tramite.Id_Usu_Alta, "Se registro un nuevo Tramite.",
                              Request, TiposOperacion.Alta, Autorizado.Si, Eventos.AltadeTramites);
            }
            else
            {
                AuditoriaHelper.Register(tramite.Id_Usu_Modif, "Se modifico el Tramite.",
                              Request, TiposOperacion.Modificacion, Autorizado.Si, Eventos.ModificarTramites);
            }
            return new JsonResult { Data = tramiteGuardado.Id_Tramite };
        }

        public ActionResult GeneralFormContent()
        {
            var tramite = new Tramite();
            tramite.Id_Tipo_Tramite = 1;


            var list = Session["TipoTramite"] as IEnumerable<SelectListItem>;
            ViewBag.TipoTramiteList = list;

            SeguridadController sc = new SeguridadController();
            List<ParametrosGeneralesModel> pgm = sc.GetParametrosGenerales();
            ViewBag.TramiteVerificar = pgm.Where(x => x.Clave == "VISTA_TRAMITE").Select(x => x.Valor).FirstOrDefault();
            return PartialView("_ObjetosTramiteDatosGenerales", tramite);
        }

        public ActionResult InformeFinalFormContent()
        {
            var tramite = new Tramite();
            tramite.Id_Tipo_Tramite = 1;
            ViewBag.TramiteEstados = mTramite_Cabecera.TramiteEstados.Select(i => new SelectListItem { Text = i.Descripcion.TrimEnd(), Value = i.Id_Estado.ToString() });
            return PartialView("_ObjetosTramiteInformeFinal", tramite);
        }

        #region Personas
        public ActionResult ImpresionPersonas()
        {
            var tramite = new Tramite();
            return PartialView("_ObjetosTramitePersonasImpresion", tramite);
        }

        [HttpGet]
        public string _ObjetosTramitePersonas(long id)
        {
            CertificadoId = id;
            var personas = GetObjetosTramitesPersonas(id);

            return "{\"data\":" + JsonConvert.SerializeObject(personas) + "}";
        }

        public ActionResult _LoadPersonaForm()
        {
            var roles = Session["Roles"] as IEnumerable<SelectListItem>;
            var mTrtTramitePersona = new TramitePersona();
            mTrtTramitePersona.Id_Rol = Convert.ToInt64(roles.First().Value);
            ViewBag.RolList = roles;
            return PartialView("_PersonaAjaxForm", mTrtTramitePersona);
        }

        public JsonResult SavePersona(TramitePersona personaTramite)
        {
            personaTramite.Id_Tramite = CertificadoId;

            #region Validaciones
            var result = _cliente.PostAsync(ApiUri + string.Format("ValidatePersona?idTramite={0}&idPersona={1}&idRol={2}",
                                                                    personaTramite.Id_Tramite,
                                                                    personaTramite.Id_Persona,
                                                                    personaTramite.Id_Rol),
                                            new ObjectContent<Operaciones<TramitePersona>>(UnidadGeneracionCertificados.OperacionesPersonas, new JsonMediaTypeFormatter())).Result;
            result.EnsureSuccessStatusCode();
            var response = result.Content.ReadAsStringAsync().Result.ToStringOrDefault();
            #endregion
            if (string.IsNullOrEmpty(response))
            {
                UnidadGeneracionCertificados.OperacionesPersonas.Add(new OperationItem<TramitePersona> { Operation = Operation.Add, Item = personaTramite });
                return new JsonResult { Data = "Ok" };
            }
            else
            {
                return new JsonResult { Data = response };
            }
        }

        public JsonResult DeletePersona(long idPersona, long idRol)
        {
            var personaDefault = new TramitePersona { Id_Tramite = CertificadoId, Id_Persona = idPersona, Id_Rol = idRol };
            var uri = string.Format(ApiUri + "GetTramitePersonaByIdTramiteIdPersona?idTramite={0}&idPersona={1}&idRol={2}", CertificadoId, idPersona, idRol);
            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var existente = result.Content.ReadAsAsync<TramitePersona>().Result;
            UnidadGeneracionCertificados.OperacionesPersonas.Add(new OperationItem<TramitePersona> { Operation = Operation.Remove, Item = existente ?? personaDefault });
            return new JsonResult { Data = "Ok" };
        }

        public List<TramitePersona> GetObjetosTramitesPersonas(long Id_Tramite)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesPersonas?idTramite=" + Id_Tramite).Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = (List<TramitePersona>)resp.Content.ReadAsAsync<IEnumerable<TramitePersona>>().Result;

            return lstObjeto;
        }

        public TramitePersona GetObjetoTramitePersona(long Id_Tramite_Persona)
        {
            var resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetoTramitePersona?idTramitePersona=" + Id_Tramite_Persona).Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = (TramitePersona)resp.Content.ReadAsAsync<TramitePersona>().Result;

            return lstObjeto;
        }

        public List<Persona> GetPersonas()
        {
            var resp = _cliente.GetAsync("api/PersonaService/GetPersonas").Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<Persona>>().Result.ToList();
            return lstObjeto;
        }

        public List<TramiteRol> GetRoles()
        {
            var resp = _cliente.GetAsync("api/TramitesCertificadosService/GetRoles").Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<TramiteRol>>().Result.ToList();

            return lstObjeto;
        }

        public JsonResult LoadPersonas()
        {
            var jsonResult = Json(GetPersonas(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult LoadRoles()
        {
            return Json(GetRoles(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Unidades Tributarias

        public ActionResult ImpresionUnidadesTributarias()
        {
            var tramite = new Tramite();
            return PartialView("_ObjetosTramiteUnidadesTributariasImpresion", tramite);
        }

        [HttpGet]
        public string _ObjetosTramiteUTS(long id)
        {
            CertificadoId = id;
            var unidadesTributarias = GetObjetosTramitesUTS(id);

            return "{\"data\":" + JsonConvert.SerializeObject(unidadesTributarias) + "}";
        }

        public ActionResult _LoadUtsForm(long idTramite)
        {
            var mTrtTramiteUts = new TramiteUnidadTributaria { Id_Tramite = idTramite, Id_Unidad_Tributaria = 0 };

            return PartialView("_UTSAjaxForm", mTrtTramiteUts);
        }

        public JsonResult GetUts(long id)
        {
            using (var result = _cliente.GetAsync("api/unidadtributaria/get/" + id).Result.EnsureSuccessStatusCode())
            {
                var unidad = result.Content.ReadAsAsync<UnidadTributaria>().Result;
                unidad.Parcela.UnidadesTributarias.Clear();
                return Json(unidad);
            }
        }

        public JsonResult SaveUts(long id, long id_tramite)
        {
            var trtTramiteUts = new TramiteUnidadTributaria
            {
                Id_Tramite = id_tramite, //CertificadoId,
                Id_Unidad_Tributaria = id
            };
            #region Validaciones
            var result = _cliente.PostAsync(ApiUri + string.Format("ValidateUnidadTributaria?idTramite={0}&idUnidadTributaria={1}",
                                                                    trtTramiteUts.Id_Tramite,
                                                                    trtTramiteUts.Id_Unidad_Tributaria),
                                            new ObjectContent<Operaciones<TramiteUnidadTributaria>>(UnidadGeneracionCertificados.OperacionesUnidadesTributarias, new JsonMediaTypeFormatter())).Result;
            result.EnsureSuccessStatusCode();
            var response = result.Content.ReadAsStringAsync().Result.ToStringOrDefault();
            #endregion
            if (string.IsNullOrEmpty(response))
            {
                UnidadGeneracionCertificados.OperacionesUnidadesTributarias.Add(new OperationItem<TramiteUnidadTributaria> { Operation = Operation.Add, Item = trtTramiteUts });
                return new JsonResult { Data = "Ok" };
            }
            else
            {
                return new JsonResult { Data = response };
            }
        }

        public JsonResult DeleteUts(long id)
        {
            var utDefault = new TramiteUnidadTributaria { Id_Tramite = CertificadoId, Id_Unidad_Tributaria = id };
            var uri = string.Format(ApiUri + "GetTramiteUnidadTributariaByIdTramiteIdUnidadTributaria?idTramite={0}&idUnidadTributaria={1}", CertificadoId, id);
            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var existente = result.Content.ReadAsAsync<TramiteUnidadTributaria>().Result;
            UnidadGeneracionCertificados.OperacionesUnidadesTributarias.Add(new OperationItem<TramiteUnidadTributaria> { Operation = Operation.Remove, Item = existente ?? utDefault });
            return new JsonResult { Data = "Ok" };
        }

        public List<TramiteUnidadTributaria> GetObjetosTramitesUTS(long Id_Tramite)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesUTS?idTramite=" + Id_Tramite).Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<TramiteUnidadTributaria>>().Result.ToList();

            return lstObjeto;
        }

        public List<UnidadTributaria> GetUnidadesTributarias()
        {
            using (var resp = _cliente.GetAsync("api/UnidadTributaria/Get").Result.EnsureSuccessStatusCode())
            {
                var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<UnidadTributaria>>().Result.ToList();
                return lstObjeto;
            }
        }

        public UnidadTributaria GetUnidadTributaria(long idUnidadTributaria)
        {
            using (var resp = _cliente.GetAsync($"api/UnidadTributaria/Get/{idUnidadTributaria}").Result.EnsureSuccessStatusCode())
            {
                var ut= resp.Content.ReadAsAsync<UnidadTributaria>().Result;
                ut.Parcela = null;
                return ut;
            }
        }

        public JsonResult LoadUnidadesTributarias()
        {
            var jsonResult = Json(GetUnidadesTributarias(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult LoadUnidadTributaria(long idUnidadTributaria)
        {
            var jsonResult = Json(GetUnidadTributaria(idUnidadTributaria), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Documentos
        public ActionResult ImpresionDocumentos()
        {
            var tramite = new Tramite();
            return PartialView("_ObjetosTramiteDocumentosImpresion", tramite);
        }

        [HttpGet]
        public string _ObjetosTramiteDocumentos(long id)
        {
            CertificadoId = id;
            var documentos = GetObjetosTramitesDocumentos(id);
            return "{\"data\":" + JsonConvert.SerializeObject(documentos) + "}";
        }

        public JsonResult GetDocumento(long id)
        {
            var result = _cliente.GetAsync("api/documento/get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var unidad = result.Content.ReadAsAsync<Documento>().Result;
            return Json(unidad);
        }

        public ActionResult _LoadDocForm(long Id_Tramite)
        {
            var mTrtTramiteDoc = new TramiteDocumento { Id_Tramite = Id_Tramite, Id_Documento = 0 };

            return PartialView("_DocumentoAjaxForm", mTrtTramiteDoc);
        }

        [HttpPost]
        public ActionResult SaveDocumento(long id, bool modificar)
        {
            var trtTramiteDoc = new TramiteDocumento
            {
                Id_Tramite = CertificadoId,
                Id_Documento = id,
            };


            if (modificar == false)
            {
                #region Validaciones
                var result = _cliente.PostAsync(ApiUri + string.Format("ValidateDocumento?idTramite={0}&idDocumento={1}",
                                                                    trtTramiteDoc.Id_Tramite,
                                                                    trtTramiteDoc.Id_Documento),
                                            new ObjectContent<Operaciones<TramiteDocumento>>(UnidadGeneracionCertificados.OperacionesDocumentos, new JsonMediaTypeFormatter())).Result;
                result.EnsureSuccessStatusCode();
                var response = result.Content.ReadAsStringAsync().Result.ToStringOrDefault();

                if (string.IsNullOrEmpty(response))
                {
                    UnidadGeneracionCertificados.OperacionesDocumentos.Add(new OperationItem<TramiteDocumento> { Operation = Operation.Add, Item = trtTramiteDoc });
                    return new JsonResult { Data = "Ok" };

                }
                else
                {
                    return new JsonResult { Data = response };
                }
                #endregion
            }

            UnidadGeneracionCertificados.OperacionesDocumentos.Add(new OperationItem<TramiteDocumento> { Operation = Operation.Update, Item = trtTramiteDoc });
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult DeleteDocumento(long id)
        {
            var docDefault = new TramiteDocumento { Id_Tramite = CertificadoId, Id_Documento = id };
            var uri = string.Format(ApiUri + "GetTramiteDocumentoByIdTramiteIdDocumento?idTramite={0}&idDocumento={1}", CertificadoId, id);
            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var existente = result.Content.ReadAsAsync<TramiteDocumento>().Result;
            UnidadGeneracionCertificados.OperacionesDocumentos.Add(new OperationItem<TramiteDocumento> { Operation = Operation.Remove, Item = existente ?? docDefault });
            return new JsonResult { Data = "Ok" };
        }

        public List<TramiteDocumento> GetObjetosTramitesDocumentos(long Id_Tramite)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesDocumentos?idTramite=" + Id_Tramite).Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<TramiteDocumento>>().Result.ToList();

            return lstObjeto;
        }

        //public JsonResult LoadTiposDocumentos()
        //{
        //    var jsonResult = Json(GetTiposDocumentos(), JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;
        //}

        //public JsonResult LoadDocumentos(long tipoDocId)
        //{
        //    var jsonResult = Json(GetDocumentos(tipoDocId), JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;
        //}

        //public JsonResult LoadDocumento(long DocId)
        //{
        //    var jsonResult = Json(GetDocumento(DocId), JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;
        //}

        //public List<TipoDocumento> GetTiposDocumentos()
        //{
        //    var resp = _cliente.GetAsync("api/TipoDocumentoService/GetTiposDocumentos").Result;
        //    resp.EnsureSuccessStatusCode();
        //    var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<TipoDocumento>>().Result.ToList();

        //    return lstObjeto;
        //}

        //public List<Documento> GetDocumentos(long tipoDocId)
        //{
        //    var resp = _cliente.GetAsync("api/Documento/GetByIdTipo/" + tipoDocId).Result;
        //    resp.EnsureSuccessStatusCode();
        //    var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<Documento>>().Result.ToList();

        //    return lstObjeto;
        //}

        //public Documento GetDocumento(long DocId)
        //{
        //    var resp = _cliente.GetAsync("api/Documento/Get/" + DocId).Result;
        //    resp.EnsureSuccessStatusCode();
        //    var documento = resp.Content.ReadAsAsync<Documento>().Result;

        //    return documento;
        //} 
        #endregion

        #region Secciones
        [HttpGet]
        public ActionResult _ObjetosTramiteSecciones(long id)
        {
            var secciones = GetObjetosTramitesSecciones(id);
            foreach (var seccion in secciones)
            {
                seccion.Visualizar = VerificarFunciones(seccion.Id_Tipo_Seccion, seccion.TipoSeccion.Id_Tipo_Tramite, "Visualizar");
                seccion.Editar = VerificarFunciones(seccion.Id_Tipo_Seccion, seccion.TipoSeccion.Id_Tipo_Tramite, "Editar");
            }
            return PartialView("_ObjetosTramiteSecciones", secciones);
        }

        public ActionResult _ObjetosTramiteSeccionesByTipoTramite(long id)
        {
            CertificadoId = 0;
            var tipos = GetTiposSecciones(id);
            var secciones = tipos.Select(t => new TramiteSeccion { Id_Tipo_Seccion = t.Id_Tipo_Seccion, Imprime = t.Imprime, Detalle = t.Plantilla, Visualizar = VerificarFunciones(t.Id_Tipo_Seccion, id, "Visualizar"), Editar = VerificarFunciones(t.Id_Tipo_Seccion, id, "Editar"), TipoSeccion = new TramiteTipoSeccion { Id_Tipo_Seccion = t.Id_Tipo_Seccion, Nombre = t.Nombre } });
            return PartialView("_ObjetosNuevoTramiteSecciones", secciones);
        }

        public List<TramiteSeccion> GetObjetosTramitesSecciones(long Id_Tramite)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetObjetosTramitesSecciones?idTramite=" + Id_Tramite).Result;
            resp.EnsureSuccessStatusCode();
            List<TramiteSeccion> lstObjeto = resp.Content.ReadAsAsync<IEnumerable<TramiteSeccion>>().Result.ToList();

            return lstObjeto;
        }

        public List<TramiteTipoSeccion> GetTiposSecciones(long tipoTraId)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/TramitesCertificadosService/GetTiposSecciones?tipoTraId=" + tipoTraId).Result;
            resp.EnsureSuccessStatusCode();
            var lstObjeto = resp.Content.ReadAsAsync<IEnumerable<TramiteTipoSeccion>>().Result.ToList();

            return lstObjeto;
        }

        public JsonResult PostSecciones(List<TramiteSeccion> secciones)
        {
            UnidadGeneracionCertificados.OperacionesSecciones.Clear();

            if (secciones != null)
            {
                foreach (var trtTramiteSec in secciones)
                {
                    bool esNuevo = trtTramiteSec.Id_Tramite_Seccion <= 0;
                    var operacion = esNuevo ? Operation.Add : Operation.Update;
                    trtTramiteSec.TipoSeccion = null;

                    UnidadGeneracionCertificados.OperacionesSecciones.Add(new OperationItem<TramiteSeccion> { Operation = operacion, Item = trtTramiteSec });
                }
            }

            return Json(new { success = true });
        }
        #endregion

        #region VALOR VALIDACION UNIDADES TRIBUTARIAS
        public string getValidateUT()
        {
            SeguridadController sc = new SeguridadController();
            List<ParametrosGeneralesModel> pgm = sc.GetParametrosGenerales();
            string valUT = pgm.Where(x => x.Clave == "UT_VALIDAR").Select(x => x.Valor).FirstOrDefault();

            return valUT;
        }
        #endregion

        [HttpPost]
        public ActionResult CancelAll()
        {
            this.UnidadGeneracionCertificados = new UnidadGeneracionCertificados();
            return Json(new { Data = "Ok" });
        }

        #region Informe Generado A Partir del Formulario de Generacion de Certificados
        public ActionResult GetInformeTramite(int id)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var resp = clienteInformes.GetAsync("api/InformeTramite/GetInforme?id=" + id + "&usuario=" + usuario).Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            InformeImpreso = new OperationItem<Documento>
            {
                Item = new Documento
                {
                    contenido = bytes,
                    extension_archivo = "pdf",
                    descripcion = "Informe de Trámite",
                    id_tipo_documento = 3,
                    fecha = DateTime.Today,
                    nombre_archivo = "InformeTramite.pdf",
                    id_usu_alta = Usuario.Id_Usuario,
                    id_usu_modif = Usuario.Id_Usuario
                }
            };
            var cd = new ContentDisposition
            {
                FileName = "InformeTramite.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }
        #endregion

        #region Informe Genrado A Partir de Opción del Menú
        public ActionResult ConsultaTramites()
        {
            TramiteModels model = new TramiteModels()
            {
                TramitesList = this.GetTipos().Where(x => x.Fecha_Baja == null).ToList()
            };
            return PartialView(model);
        }
        public ActionResult GenerarInformeEstadoTramite(TramiteModels model)
        {
            //preguntar si es null identificador o nro de tramite para poder optar por cual hacer la consulta
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
            var resp = _cliente.GetAsync("api/ReporteEstadoTramite/GetInformeEstadoTramite?Id=" + model.Identificador + "&Numero=" + model.Numero + "&Operacion=" + model.Operacion + "&usuario=" + usuario).Result;

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var msg = resp.Content.ReadAsStringAsync().Result;
                return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
            }

            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);

            Session["InformeEstadoTramite.pdf"] = bytes;
            return Json(new { success = true, file = "InformeEstadoTramite.pdf" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFileInformeEstadoTramite(string file)
        {
            byte[] bytes = Session[file] as byte[];
            if (bytes == null)
                return new EmptyResult();
            Session[file] = null;

            var cd = new System.Net.Mime.ContentDisposition
            {
                Size = bytes.Length,
                FileName = file,
                Inline = true,
            };
            Response.Clear();
            Response.AppendHeader("Content-Disposition", cd.ToString());
            Response.ContentType = "application/pdf";
            Response.Buffer = true;
            Response.BinaryWrite(bytes);

            return null;
        }
        #endregion

        public void LoadTramite(long? idTramite)
        {
            //var resp = _cliente.GetAsync("api/TramitesCertificados/GetObjetosTramitesCertificados?IdTramite=" + idTramite).Result;
            //resp.EnsureSuccessStatusCode();
            ////if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            ////{ 

            ////}
            //Tramite tramite = new Tramite();
            //tramite = resp.Content.ReadAsAsync<Tramite>().Result;
            //Session["TipoTramite"] = GetTipos().Select(i => new SelectListItem() { Text = i.Nombre.TrimEnd(), Value = i.Id_Tipo_Tramite.ToString() });
            //Session["Roles"] = GetRoles().Select(i => new SelectListItem() { Text = i.Descripcion.TrimEnd(), Value = i.Id_Rol.ToString() });
            //if (Session["TipoTramite"] != null)
            //{
            //    ViewBag.TipoTramiteList = Session["TipoTramite"];
            //}

            //ViewBag.TramiteEstados = mTramite_Cabecera.TramiteEstados.Select(i => new SelectListItem() { Text = i.Descripcion.TrimEnd(), Value = i.Id_Estado.ToString() });
            //return PartialView("busqudaTramitesCertificados.cshtml", tramite);

            ViewBag.TramiteId = idTramite;
            //BusquedaTramitesCertificados();
        }

        public void ParametroTramitePermiso()
        {
            if (Permisos_Tramite_Activado == null)
            {
                HttpResponseMessage respParametro = _cliente.GetAsync("api/SeguridadService/GetParametrosGenerales").Result;
                respParametro.EnsureSuccessStatusCode();
                var resultParametro = respParametro.Content.ReadAsAsync<IEnumerable<ParametrosGeneralesModel>>().Result;

                //Listado de Permisos
                Permisos_Tramite_Activado = Convert.ToInt32(resultParametro.Where(x => x.Clave.Contains("TRAMITE_SECCION_FUNCION")).Select(x => x.Valor).FirstOrDefault());
            }
        }

        public JsonResult GetParametroTramitePermiso()
        {
            ParametroTramitePermiso();
            return Json(Permisos_Tramite_Activado, JsonRequestBehavior.AllowGet);
        }


        public bool VerificarFunciones(long idSeccion, long idTipoTramite, string text)
        {
            ParametroTramitePermiso();
            if (Permisos_Tramite_Activado == 1)
            {
                HttpResponseMessage resppermiso = _cliente.GetAsync("api/TramitesCertificadosService/GetPermisosTramiteSaved?idSeccion=" + idSeccion + "&idTipoTramite=" + idTipoTramite).Result;
                resppermiso.EnsureSuccessStatusCode();

                var permisos = JsonConvert.DeserializeObject<List<GeoSit.Data.BusinessEntities.ObrasPublicas.TramitePermisos>>(resppermiso.Content.ReadAsAsync<string>().Result);
                bool PostResult = false;
                if (permisos != null)
                {
                    foreach (var permiso in permisos)
                    {
                        var result = FuncionesHabilitadas.Where(x => x.Id_Funcion == permiso.ID_FUNCION && x.Funcion_Nombre.StartsWith(text));
                        if (result.Count() > 0)
                        {
                            PostResult = true;
                            return PostResult;
                        }
                    }
                }
                return PostResult;
            }
            else
            {
                return true;
            }
        }

        public JsonResult VerificarFuncionesJson(long idSeccion, long idTipoTramite, string text)
        {
            return Json(VerificarFunciones(idSeccion, idTipoTramite, text), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PermisosTramiteFinalizado()
        {
            if (PermisoTramiteFinalizado == null)
            {
                if (FuncionesHabilitadas.Where(x => x.Funcion_Nombre == "Edicion Tramite Finalizado").Count() > 0)
                {
                    PermisoTramiteFinalizado = true;
                }
                else
                {
                    PermisoTramiteFinalizado = false;
                }
            }

            return Json(PermisoTramiteFinalizado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDatosTramiteKontaktar(string numero)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/InterfacesService/GetDatosTramiteKontaktar?numero=" + numero).Result;
            resp.EnsureSuccessStatusCode();
            List<VistaKontaktar> lstObjeto = resp.Content.ReadAsAsync<List<VistaKontaktar>>().Result;

            return Json(lstObjeto, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExpresionRegularTramite()
        {
            SeguridadController sc = new SeguridadController();
            List<ParametrosGeneralesModel> pgm = sc.GetParametrosGenerales();
            var expresion = pgm.Where(x => x.Clave == "EXP_NUMERO_TRAMITE").Select(x => x.Valor).FirstOrDefault();
            var patron = pgm.Where(x => x.Clave == "PATRON_NUMERO_TRAMITE").Select(x => x.Valor).FirstOrDefault();

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("expresion", expresion);
            dict.Add("patron", patron);

            return new JsonResult { Data = dict };
        }
    }
}