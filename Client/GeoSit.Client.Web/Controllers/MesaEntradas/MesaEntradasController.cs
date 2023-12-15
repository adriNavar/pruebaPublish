using System;
using System.Web.Mvc;
using System.Net.Http;
using System.Configuration;
using System.Net.Http.Formatting;
using GeoSit.Data.BusinessEntities.Common;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.MesaEntradas.DTO;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Linq;
using System.Net;
using static GeoSit.Data.BusinessEntities.Common.Enumerators;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.Documentos;
using System.Net.Mime;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Client.Web.ViewModels;
using GeoSit.Data.BusinessEntities.Inmuebles;
using Parcela = GeoSit.Client.Web.ViewModels.Parcela;
using Newtonsoft.Json;
using GeoSit.Client.Web.Solr;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.ValidacionesDB;
using System.Net.Mail;
using GeoSit.Data.BusinessEntities.Personas;
using System.Threading;
using System.Net.Sockets;
using GeoSit.Data.BusinessEntities.Temporal;
using Newtonsoft.Json.Linq;
using GeoSit.Data.BusinessEntities.Via;
using GeoSit.Client.Web.Helpers.ExtensionMethods.DeclaracionesJuradasTemporal;
using GeoSit.Web.Api.Models;

using System.Globalization;
namespace GeoSit.Client.Web.Controllers
{
    public class MesaEntradasController : Controller
    {
        private HttpClient cliente = null;
        private HttpClient clienteInformes = null;
        private string uploadTempFolder = string.Empty;
        private ArchivoDescarga ArchivoDescarga
        {
            get { return Session["ArchivoDescarga"] as ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }
        private UsuariosModel Usuario { get { return (UsuariosModel)Session["usuarioPortal"]; } }
        private OperationItem<Documento> InformeImpreso
        {
            get { return Session["InformeImpreso"] as OperationItem<Documento>; }
            set { Session["InformeImpreso"] = value; }
        }
        public MesaEntradasController()
        {
            cliente = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan, BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) };
            clienteInformes = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan, BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]) };
            uploadTempFolder = GetParamtrosGeneralesByClave("RUTA_DOCUMENTOS_TEMPORAL");
        }

        // GET: MesaEntradas
        public ActionResult Index()
        {
            ViewData["tiposTramites"] = new SelectList(GetTiposTramitesItems("- Todos -"), "Value", "Text");
            ViewData["objetosTramites"] = GetObjetosTramites();
            ViewData["estadosTramites"] = new SelectList(GetEstadosTramites("- Todos -"), "Value", "Text");
            ViewData["prioridadesTramites"] = new SelectList(GetPrioridadesTramites("- Todos -"), "Value", "Text");
            ViewData["sectores"] = new SelectList(GetSectores("- Todos -"), "Value", "Text");
            ViewData["usuariosMismoSector"] = new SelectList(GetUsuariosMismoSector((long)Usuario.IdSector, "- Todos -"), "Value", "Text");

            ViewBag.sector = (Usuario.SectorUsuario != null ? Usuario.SectorUsuario.Nombre.Trim() : string.Empty);
            ViewBag.usuarioNombre = Usuario.Apellido + ", " + Usuario.Nombre;

            int esProfesional = 0;
            var sIdSectorProfesional = GetParamtrosGeneralesByClave("ID_SECTOR_EXTERNO");
            if (sIdSectorProfesional != string.Empty)
            {
                if (Usuario.IdSector.GetValueOrDefault() == Convert.ToInt32(sIdSectorProfesional))
                {
                    esProfesional = 1;
                }
            }
            ViewBag.esProfesional = esProfesional;

            return PartialView(new METramite());
        }

        public ActionResult EdicionTramite(int idTramite, string operacion)
        {
            string directorio = Path.Combine(uploadTempFolder, idTramite.ToString(), "temporales");
            if (Directory.Exists(directorio))
            {
                Directory.Delete(directorio, true);
            }

            var selLstItemsEstadosTramites = GetEstadosTramites();

            ViewData["tiposTramites"] = new SelectList(GetTiposTramitesItems(), "Value", "Text");
            ViewData["objetosTramites"] = GetObjetosTramites();
            ViewData["estadosTramites"] = new SelectList(selLstItemsEstadosTramites, "Value", "Text");
            ViewData["prioridadesTramites"] = new SelectList(GetPrioridadesTramites(), "Value", "Text");
            ViewData["jurisdicciones"] = new SelectList(GetJurisdicciones(), "Value", "Text");
            ViewData["localidades"] = GetObjetosByTipo(TipoObjetoAdministrativoEnum.Localidad);
            ViewData["desglosesDestinos"] = new SelectList(GetDesglosesDestinos(), "Value", "Text");
            ViewData["tiposDocumentos"] = new SelectList(GetTiposDocumentos(), "Value", "Text");

            ViewData["accionesGenerales"] = GetAccionesGenerales(idTramite);

            ViewBag.sector = (Usuario.SectorUsuario != null ? Usuario.SectorUsuario.Nombre.Trim() : string.Empty);
            ViewBag.usuarioNombre = Usuario.Nombre + ", " + Usuario.Apellido;
            ViewBag.usuarioLogin = Usuario.Login;

            ViewBag.iniciador = string.Empty;
            ViewBag.idPersona = string.Empty;
            ViewBag.confirmarHabilitado = false;
            ViewBag.tipoTramite = null;
            ViewBag.titular = string.Empty;

            bool esProfesional = false;
            var sIdSectorProfesional = GetParamtrosGeneralesByClave("ID_SECTOR_EXTERNO");
            if (sIdSectorProfesional != string.Empty)
            {
                if (Usuario.IdSector.GetValueOrDefault() == Convert.ToInt32(sIdSectorProfesional))
                {
                    esProfesional = true;
                }
            }
            ViewBag.esProfesional = esProfesional;
            int puedeReingresar = 0;

            var vigenciaReingreso = "-";

            string titulo = "Edición de Trámites";
            ModelState.Remove("IdTramite");
            METramite model = GetTramiteById(idTramite) ?? new METramite { IdTramite = idTramite };
            if (model.IdTramite > 0)
            {
                if (esProfesional)
                {
                    if (model.IdEstado != (int)EnumEstadoTramite.Provisorio && model.IdEstado != (int)EnumEstadoTramite.Despachado && model.IdEstado != (int)EnumEstadoTramite.None)
                    {
                        operacion = "Consulta";
                    }
                }
                else
                {
                    if (model.IdEstado == (int)EnumEstadoTramite.Provisorio || model.IdEstado == (int)EnumEstadoTramite.Presentado)
                    {
                        ViewBag.confirmarHabilitado = true;
                    }
                }
                ViewBag.idPersona = model.IdIniciador?.ToString();
                if (model.Iniciador != null)
                {
                    //ViewBag.iniciador = model.Iniciador.Apellido + "," + model.Iniciador.Nombre;
                    ViewBag.iniciador = model.Iniciador.NombreCompleto;
                }

                if (model.IdUnidadTributaria != null)
                {
                    ViewBag.titular = GetPersonaByIdUnidadTributaria(model.IdUnidadTributaria.Value);
                }

                if (operacion == "Consulta")
                {
                    titulo = "Consulta de Trámites";
                }
                ViewBag.estado = selLstItemsEstadosTramites.Single(e => e.Value == model.IdEstado.ToString()).Text;

                if (esProfesional)
                {
                    ViewBag.confirmarHabilitado = false;
                    var fechaUltDespacho = (model.Movimientos != null && model.Movimientos.Count > 0 ? model.Movimientos.SingleOrDefault(m => m.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar && m.IdMovimiento == model.Movimientos.Where(p => p.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar).Max(f => f.IdMovimiento))?.FechaAlta : null);
                    if (fechaUltDespacho.HasValue)
                    {
                        if (model.IdEstado == (int)EnumEstadoTramite.Despachado)
                        {
                            vigenciaReingreso = fechaUltDespacho.Value.AddDays(365).ToShortDateString();
                        }
                        if (fechaUltDespacho.Value.AddDays(365) >= DateTime.Today)
                        {
                            puedeReingresar = 1;
                        }
                    }
                }
            }
            else
            {
                model.IdTramite = new Random().Next(int.MinValue, -1);
                model.IdEstado = (int)EnumEstadoTramite.None;
                model.FechaIngreso = DateTime.Today;
                titulo += " - Crear";
                ViewBag.confirmarHabilitado = !esProfesional;
            }
            ViewBag.operacion = Session["UltimaOperacionTramite"] = operacion;
            ViewBag.VigenciaReingreso = vigenciaReingreso;
            ViewBag.puedeReingresar = puedeReingresar;
            ViewBag.habilitaDatosEspecificos = HabilitaDatosEspecificos(model);

            ViewBag.Title = titulo;
            ViewBag.libroDiarioAbierto = GetParamtrosGeneralesByClave("LIBRO_DIARIO_ABIERTO");

            Session["ModelTramite"] = model;

            return PartialView(model);
        }

        private bool HabilitaDatosEspecificos(METramite model)

        {
            return !model.Movimientos?.Any(m => m.IdTipoMovimiento == (int)EnumTipoMovimiento.Anular || m.IdTipoMovimiento == (int)EnumTipoMovimiento.Finalizar) ?? true;
        }

        private List<METipoMovimiento> GetAccionesGenerales(int idTramite)
        {
            using (var resp = cliente.GetAsync($"api/mesaentradas/tramites/{idTramite}/usuario/{Usuario.Id_Usuario}/acciones").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<METipoMovimiento>>().Result;
            }
        }

        public async Task<ActionResult> GetDatosEspecificos()
        {
            var tramite = Session["ModelTramite"] as METramite;
            if (!HabilitaDatosEspecificos(tramite))
            {
                return Json(new MEDatosEspecificos[0], JsonRequestBehavior.AllowGet); //Se trae primero los que no tengan hijos
            }
            using (var resp = await cliente.GetAsync($"api/mesaentradas/tramites/{tramite.IdTramite}/entradas"))
            {
                resp.EnsureSuccessStatusCode();
                var tramiteEntradas = await resp.Content.ReadAsAsync<IEnumerable<METramiteEntrada>>();

                using (var cts = new CancellationTokenSource())
                {
                    CancellationToken token = cts.Token;

                    var tasks = new List<Task<List<MEDatosEspecificos>>>();
                    var grupos = tramiteEntradas.GroupBy(d => d.ObjetoEntrada.IdEntrada).ToDictionary(x => x.Key, x => x.ToList());
                    foreach (var grp in grupos)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            Task<List<MEDatosEspecificos>> entradasComponentes;
                            try
                            {
                                entradasComponentes = ProcesarComponente(tramite.IdTramite, grp, token);
                            }
                            catch (OperationCanceledException)
                            {
                                entradasComponentes = Task.FromResult(new List<MEDatosEspecificos>());
                            }
                            catch (Exception)
                            {
                                cts.Cancel();
                                entradasComponentes = Task.FromResult(new List<MEDatosEspecificos>());
                            }
                            return entradasComponentes;
                        }, token));
                    }
                    var datosEspecificosByEntrada = await Task.WhenAll(tasks);
                    if (cts.IsCancellationRequested)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var datosEspecificos = datosEspecificosByEntrada
                                                .SelectMany(x => x)
                                                .OrderByDescending(t => !t.ParentIdTramiteEntradas.Any())
                                                .ThenBy(t => t.IdTramiteEntrada)
                                                .ToList();

                    var jsonSettings = new JsonSerializerSettings
                    {
                        Culture = new CultureInfo("en-US")
                    };

                    string jsonData = JsonConvert.SerializeObject(datosEspecificos, Formatting.None, jsonSettings);
                    return Content(jsonData, "application/json");
                }
            }
        }

        private Task<List<MEDatosEspecificos>> RunTaskComponente<T>(string dataRetrievalURL, Func<T, IEnumerable<Task<MEDatosEspecificos>>> execute)
        {
            using (var response = cliente.GetAsync(dataRetrievalURL).Result)
            {
                response.EnsureSuccessStatusCode();
                var data = response.Content.ReadAsAsync<T>().Result;
                var tasks = new List<Task<MEDatosEspecificos>>();
                var datosEspecificos = Task.WhenAll(execute(data)).Result;
                return Task.FromResult(datosEspecificos.ToList());
            }
        }

        private Task<List<MEDatosEspecificos>> ProcesarComponente(int idTramite, KeyValuePair<int, List<METramiteEntrada>> entradasTramiteByComponente, CancellationToken token)
        {
            MEDatosEspecificos getNewDatoEspecifico(METramiteEntrada tramiteEntrada, MEEntrada entrada, IEnumerable<Propiedad> propiedades)
            {
                return new MEDatosEspecificos
                {
                    Guid = tramiteEntrada.IdTramiteEntrada.ToGuid(), //random guid solo a fines de mapear la relacion arbol - storage
                    ParentGuids = tramiteEntrada.TramiteEntradaRelaciones.Select(t => t.IdTramiteEntradaPadre.ToGuid()),
                    IdTramiteEntrada = tramiteEntrada.IdTramiteEntrada,
                    ParentIdTramiteEntradas = tramiteEntrada.TramiteEntradaRelaciones.Select(t => t.IdTramiteEntradaPadre),
                    Propiedades = propiedades.ToList(),
                    TipoObjeto = new TipoDatoEspecifico { Id = entrada.IdEntrada, Text = entrada.Descripcion },
                };
            }
            using (var respEntrada = cliente.GetAsync($"api/MesaEntradas/entrada/{entradasTramiteByComponente.Key}/", token).Result)
            {
                respEntrada.EnsureSuccessStatusCode();

                var entrada = respEntrada.Content.ReadAsAsync<MEEntrada>().Result;

                #region Procesamiento de Parcelas
                if (entrada.IdEntrada == Convert.ToInt32(Entradas.Parcela))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<ParcelaTemporal> parcelas)
                    {
                        var clases = GetClasesParcela();
                        var tipos = GetTiposParcela();
                        foreach (var parcela in parcelas)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.Single(x => x.IdObjeto == parcela.ParcelaID);
                            var copy = parcela;
                            yield return Task.Run(async () =>
                            {
                                bool isDestino = tramiteEntrada.TramiteEntradaRelaciones.Any();
                                var propiedades = await ProcesarParcela(copy, isDestino, clases, tipos, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<ParcelaTemporal>>($"api/ParcelaTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Unidades Tributarias
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.UnidadTributaria))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<UnidadTributariaTemporal> uts)
                    {
                        var tiposUT = GetTiposUF();
                        foreach (var ut in uts)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == ut.UnidadTributariaId);
                            var copy = ut;
                            yield return Task.Run(async () =>
                            {
                                bool isDestino = tramiteEntrada.TramiteEntradaRelaciones.Any();
                                var propiedades = await ProcesarUnidadTributaria(copy, isDestino, tiposUT, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<UnidadTributariaTemporal>>($"api/UnidadTributariaTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Mensura Registrada
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.MensuraRegistrada))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<MensuraTemporal> mensuras)
                    {
                        foreach (var mensura in mensuras)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == mensura.IdMensura);
                            var copy = mensura;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarMensura(copy, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<MensuraTemporal>>($"api/MensuraTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Comprobantes de Pago
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.ComprobantePago))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<MEComprobantePago> comprobantes)
                    {
                        var tasas = GetTiposTasa();
                        foreach (var comprobante in comprobantes)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == comprobante.IdComprobantePago);
                            var copy = comprobante;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarComprobantePago(copy, tasas, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<MEComprobantePago>>($"api/ComprobantePagoService/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Libres Deuda
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.LibreDeuda))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<INMLibreDeDeudaTemporal> libresDeuda)
                    {
                        var entes = GetTiposEnteEmisor();
                        foreach (var libreDeuda in libresDeuda)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == libreDeuda.IdLibreDeuda);
                            var copy = libreDeuda;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarLibreDeuda(copy, entes, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<INMLibreDeDeudaTemporal>>($"api/LibreDeDeudaTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Descripciones del Inmueble
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.DescripcionInmueble))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<INMCertificadoCatastralTemporal> certificados)
                    {
                        foreach (var certificadoCatastral in certificados)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == certificadoCatastral.CertificadoCatastralId);
                            var copy = certificadoCatastral;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarDescripcionInmueble(copy, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<INMCertificadoCatastralTemporal>>($"api/CertificadoCatastralTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Manzanas
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.Manzana))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<DivisionTemporal> manzanas)
                    {
                        foreach (var manzana in manzanas)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == manzana.FeatId);
                            var copy = manzana;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarManzana(copy, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<DivisionTemporal>>($"api/DivisionTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Espacios Públicos
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.EspacioPublico))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<EspacioPublicoTemporal> espaciosPublicos)
                    {
                        foreach (var espacioPublico in espaciosPublicos)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == espacioPublico.EspacioPublicoID);
                            var copy = espacioPublico;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarEspacioPublico(copy, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<EspacioPublicoTemporal>>($"api/EspacioPublicoTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Vías
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.Via))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<Via> vias)
                    {
                        var tiposVias = GetTiposVia();
                        foreach (var via in vias)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == via.ViaId);
                            var copy = via;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarVia(copy, tiposVias, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<Via>>($"api/ViaService/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Títulos
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.Titulo))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<DominioTemporal> dominios)
                    {
                        var tiposTitulo = GetTiposTitulo();
                        foreach (var dominio in dominios)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == dominio.DominioID);
                            var copy = dominio;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarTitulo(copy, tiposTitulo, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<DominioTemporal>>($"api/DominioTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de Personas
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.Persona))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<Persona> personas)
                    {
                        var tiposTitularidad = GetTiposTitularidad();
                        foreach (var persona in personas)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == persona.PersonaId);
                            var copy = persona;
                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarPersona(copy, tramiteEntrada.TramiteEntradaRelaciones, tiposTitularidad, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    }
                    return RunTaskComponente<List<Persona>>($"api/Persona/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion
                #region Procesamiento de DDJJ
                else if (entrada.IdEntrada == Convert.ToInt32(Entradas.DDJJ))
                {
                    IEnumerable<Task<MEDatosEspecificos>> execute(List<Tuple<long, DDJJTemporal>> resultados)
                    {
                        var titulares = resultados.Where(r => r.Item2.Dominios?.Any() ?? false).SelectMany(r => r.Item2.Dominios.SelectMany(d => d.Titulares.Select(t => t.IdPersona)));

                        var vias = resultados.Where(r => r.Item2.U != null)
                                             .SelectMany(r => r.Item2.U.Fracciones.SelectMany(f => f.MedidasLineales.Select(ml => Tuple.Create(ml.IdTramoVia, ml.IdVia))))
                                             .Distinct()
                                             .ToList();

                        var versionesDDJJ = GetVersionesDDJJ();

                        var resp = cliente.PostAsJsonAsync($"api/Persona/Buscar/Completa", titulares).Result.EnsureSuccessStatusCode();
                        var personas = resp.Content.ReadAsAsync<IEnumerable<Persona>>().Result;

                        resp = cliente.GetAsync($"api/DeclaracionJurada/Aptitudes").Result.EnsureSuccessStatusCode();
                        var aptitudes = resp.Content.ReadAsAsync<IEnumerable<Data.BusinessEntities.DeclaracionesJuradas.VALAptitudes>>().Result;

                        resp = cliente.PostAsJsonAsync($"api/DeclaracionJurada/Aforos/Vias", vias).Result.EnsureSuccessStatusCode();
                        var aforos = resp.Content.ReadAsAsync<IEnumerable<Aforo>>().Result;

                        foreach (var resultado in resultados)
                        {
                            token.ThrowIfCancellationRequested();
                            var tramiteEntrada = entradasTramiteByComponente.Value.SingleOrDefault(x => x.IdObjeto == resultado.Item2.IdDeclaracionJurada);
                            var copy = resultado;

                            yield return Task.Run(async () =>
                            {
                                var propiedades = await ProcesarDDJJ(copy, versionesDDJJ, personas, aptitudes, aforos, token);
                                return getNewDatoEspecifico(tramiteEntrada, entrada, propiedades);
                            }, token);
                        }
                    };
                    return RunTaskComponente<List<Tuple<long, DDJJTemporal>>>($"api/DeclaracionJuradaTemporal/Tramite/{idTramite}/Entradas", execute);
                }
                #endregion

                throw new NotImplementedException($"El procesamiento para la entrada {entrada.Descripcion}, no se ha implementado.");
            }

        }

        private Task<IEnumerable<Propiedad>> RunTaskPropiedades(Func<IEnumerable<Propiedad>> collect, CancellationToken token)
        {
            return Task.Run(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarParcela(ParcelaTemporal parcela, bool isDestino, IEnumerable<Data.BusinessEntities.Inmuebles.ClaseParcela> clasesParcela, IEnumerable<TipoParcela> tiposParcela, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "hdnOperacionObjetoEspecifico",
                    Value = isDestino ? "Destino" : "Origen",
                    Text = "",
                    Visible = false,
                    Label = "Tipo"
                };
                yield return new Propiedad
                {
                    Id = "hdnIdPartidaPersona",
                    Value = parcela.ParcelaID.ToString(),
                    Text = "",
                    Visible = false,
                    Label = ""
                };
                #region Parcela Destino
                if (isDestino)
                {
                    yield return new Propiedad
                    {
                        Id = "Plano",
                        Value = parcela.PlanoId,
                        Visible = true,
                        Label = "ID s/Plano"
                    };
                    yield return new Propiedad
                    {
                        Id = "Zona",
                        Value = parcela.TipoParcelaID.ToString(),
                        Text = tiposParcela.Single(tp => tp.TipoParcelaID == parcela.TipoParcelaID).Descripcion,
                        Visible = false,
                        Label = "Zona"
                    };
                    yield return new Propiedad
                    {
                        Id = "Tipo",
                        Value = parcela.ClaseParcelaID.ToString(),
                        Text = clasesParcela.Single(cp => cp.ClaseParcelaID == parcela.ClaseParcelaID).Descripcion,
                        Visible = true,
                        Label = "Clase"
                    };
                }
                #endregion
                #region Parcela Origen
                else
                {
                    string tipos = "parcelas";
                    var fields = new[] { "nombre", "dato_nomenclatura" };
                    var filters = new Dictionary<string, string> { { "id", parcela.ParcelaID.ToString() } };
                    var busqueda = new[] { "*:*" };
                    var result = JToken.Parse(GetResults(tipos, fields, filters, busqueda));

                    var textos = new List<string>();
                    foreach (string field in fields)
                    {
                        string dato = result["response"]["docs"].FirstOrDefault().GetValue(field)?.ToString();
                        if (!string.IsNullOrEmpty(dato))
                        {
                            textos.Add(dato);
                        }
                    }
                    yield return new Propiedad
                    {
                        Id = "Partida",
                        Value = string.Join("-", textos),
                        Text = "",
                        Visible = true,
                        Label = "Partida/Nomenclatura"
                    };
                }
                #endregion
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarUnidadTributaria(UnidadTributariaTemporal ut, bool isDestino, ICollection<SelectListItem> tiposUT, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "hdnOperacionObjetoEspecifico",
                    Value = isDestino ? "Destino" : "Origen",
                    Text = "",
                    Visible = false,
                    Label = "Tipo"
                };
                yield return new Propiedad
                {
                    Id = "hdnIdUnidadFuncional",
                    Value = ut.UnidadTributariaId.ToString(),
                    Text = "",
                    Visible = false,
                    Label = ""
                };
                #region Unidad Tributaria Destino
                if (isDestino)
                {
                    yield return new Propiedad
                    {
                        Id = "Plano",
                        Value = ut.PlanoId,
                        Text = "",
                        Visible = true,
                        Label = "ID s/Plano"
                    };
                    yield return new Propiedad
                    {
                        Id = "Piso",
                        Value = ut.Piso,
                        Text = "",
                        Visible = false,
                        Label = "Piso"
                    };
                    yield return new Propiedad
                    {
                        Id = "Unidad",
                        Value = ut.Unidad,
                        Text = "",
                        Visible = false,
                        Label = "Unidad"
                    };
                    yield return new Propiedad
                    {
                        Id = "Tipo",
                        Value = ut.TipoUnidadTributariaID.ToString(),
                        Text = tiposUT.Single(x => x.Value == ut.TipoUnidadTributariaID.ToString()).Text,
                        Visible = true,
                        Label = "Tipo UF"
                    };
                    yield return new Propiedad
                    {
                        Id = "Superficie",
                        Value = ut.Superficie.ToString(),
                        Text = "",
                        Visible = false,
                        Label = "Superficie"
                    };
                    yield return new Propiedad
                    {
                        Id = "vigencia",
                        Value = ut.Vigencia.ToString(),
                        Text = "",
                        Visible = false,
                        Label = "Vigencia"
                    };
                }
                #endregion
                #region Unidad Tributaria Origen
                else
                {
                    yield return new Propiedad
                    {
                        Id = "UnidadFuncionalCodigo",
                        Value = ut.CodigoProvincial,
                        Text = "",
                        Visible = true,
                        Label = "Partida inmobiliaria"
                    };
                }
                #endregion
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarMensura(MensuraTemporal mensura, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "Mensura",
                    Value = mensura.Descripcion,
                    Text = "",
                    Visible = true,
                    Label = "Nº de mensura"
                };
                yield return new Propiedad
                {
                    Id = "TipoMensura",
                    Value = mensura.IdTipoMensura.ToString(),
                    Text = GetTipoMensura(mensura.IdMensura),
                    Visible = false,
                    Label = "Objeto"
                };
                yield return new Propiedad
                {
                    Id = "hdnIdMensura",
                    Value = mensura.IdMensura.ToString(),
                    Text = "",
                    Visible = false,
                    Label = ""
                };
                yield return new Propiedad
                {
                    Id = "IdTipoMensura",
                    Value = mensura.IdTipoMensura.ToString(),
                    Text = "",
                    Visible = false,
                    Label = ""
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarComprobantePago(MEComprobantePago comprobantePago, ICollection<SelectListItem> tasas, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "TipoTasa",
                    Value = comprobantePago.IdTipoTasa.ToString(),
                    Text = tasas.Single(x => x.Value == comprobantePago.IdTipoTasa.ToString()).Text,
                    Visible = true,
                    Label = "Tipo de tasa"
                };
                yield return new Propiedad
                {
                    Id = "Identificacion",
                    Value = comprobantePago.IdTramite.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Identificación Trámite"
                };
                yield return new Propiedad
                {
                    Id = "TipoTramite",
                    Value = comprobantePago.TipoTramiteDgr,
                    Text = "",
                    Visible = false,
                    Label = "Tipo de trámite"
                };
                yield return new Propiedad
                {
                    Id = "fecha-liquidacion",
                    Value = comprobantePago.FechaLiquidacion.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Fecha Liquidación"
                };
                yield return new Propiedad
                {
                    Id = "fecha-pago",
                    Value = comprobantePago.FechaPago.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Fecha de Pago"
                };
                yield return new Propiedad
                {
                    Id = "fecha-vencimiento",
                    Value = comprobantePago.FechaVencimiento.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Fecha de Vencimiento"
                };
                yield return new Propiedad
                {
                    Id = "MedioPago",
                    Value = comprobantePago.MedioPago,
                    Text = "",
                    Visible = false,
                    Label = "Medio de Pago"
                };
                yield return new Propiedad
                {
                    Id = "Importe",
                    Value = comprobantePago.Importe.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Importe"
                };
                yield return new Propiedad
                {
                    Id = "EstadoPago",
                    Value = comprobantePago.EstadoPago,
                    Text = "",
                    Visible = false,
                    Label = "Estado de Pago"
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarLibreDeuda(INMLibreDeDeudaTemporal libreDeuda, ICollection<SelectListItem> entes, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "EnteEmisor",
                    Value = libreDeuda.IdEnteEmisor.ToString(),
                    Text = entes.Single(x => x.Value == libreDeuda.IdEnteEmisor.ToString()).Text,
                    Visible = true,
                    Label = "Ente Emisor"
                };
                yield return new Propiedad
                {
                    Id = "NroCertificado",
                    Value = libreDeuda.NroCertificado.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "N° de certificado"
                };
                yield return new Propiedad
                {
                    Id = "Superficie",
                    Value = libreDeuda.Superficie.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Superficie"
                };
                yield return new Propiedad
                {
                    Id = "Valuacion",
                    Value = libreDeuda.Valuacion.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Valuacion"
                };
                yield return new Propiedad
                {
                    Id = "Deuda",
                    Value = libreDeuda.Deuda.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Deuda"
                };
                yield return new Propiedad
                {
                    Id = "fecha-vigencia",
                    Value = libreDeuda.FechaVigencia.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Fecha de Vigencia"
                };
                yield return new Propiedad
                {
                    Id = "fecha-emision",
                    Value = libreDeuda.FechaEmision.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Fecha de Emision"
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarDescripcionInmueble(INMCertificadoCatastralTemporal certificado, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "Descripcion",
                    Value = certificado.Descripcion,
                    Text = "...",
                    Visible = true,
                    Label = ""
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarManzana(DivisionTemporal manzana, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "Plano",
                    Value = manzana.PlanoId,
                    Text = string.Empty,
                    Visible = true,
                    Label = "ID s/Plano"
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarEspacioPublico(EspacioPublicoTemporal espacioPublico, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "IdEspacioPublico",
                    Value = espacioPublico.EspacioPublicoID.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "IdEspacioPublico"
                };
                yield return new Propiedad
                {
                    Id = "Superficie",
                    Value = espacioPublico.Superficie.ToString(),
                    Text = "",
                    Visible = true,
                    Label = "Superficie"
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarVia(Via via, ICollection<SelectListItem> tiposVias, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "Plano",
                    Value = via.PlanoId,
                    Text = "",
                    Visible = true,
                    Label = "ID s/Plano"
                };
                yield return new Propiedad
                {
                    Id = "Tipo",
                    Value = via.TipoViaId.ToString(),
                    Text = tiposVias.Single(x => x.Value == via.TipoViaId.ToString()).Text,
                    Visible = true,
                    Label = "Tipo"
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarTitulo(DominioTemporal dominio, ICollection<SelectListItem> tiposTitulo, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "Tipo",
                    Value = dominio.TipoInscripcionID.ToString(),
                    Text = tiposTitulo.Single(x => x.Value == dominio.TipoInscripcionID.ToString()).Text,
                    Visible = true,
                    Label = "Tipo"
                };
                yield return new Propiedad
                {
                    Id = "Inscripcion",
                    Value = dominio.Inscripcion,
                    Text = "",
                    Visible = true,
                    Label = "Inscripción"
                };
                yield return new Propiedad
                {
                    Id = "fecha",
                    Value = dominio.Fecha.ToString(),
                    Text = "",
                    Visible = false,
                    Label = "Fecha"
                };
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarPersona(Persona persona, ICollection<METramiteEntradaRelacion> relaciones, ICollection<SelectListItem> tiposTitularidad, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                #region Propiedades
                yield return new Propiedad
                {
                    Id = "hdnIdPersona",
                    Value = persona.PersonaId.ToString(),
                    Text = null,
                    Visible = false,
                    Label = null
                };
                yield return new Propiedad
                {
                    Id = "Persona",
                    Value = $"{persona.NombreCompleto.Trim().ToUpper()}-{persona.NroDocumento}",
                    Text = null,
                    Visible = true,
                    Label = "Persona"
                };
                #region Propiedades de Titularidad en caso de ser hijo de entrada Título
                var entradaPadreTitulo = relaciones.SingleOrDefault(r => r.TramiteEntradaPadre?.ObjetoEntrada.IdEntrada == Convert.ToInt32(Entradas.Titulo));
                if (entradaPadreTitulo != null)
                {
                    using (var resp = cliente.GetAsync($"api/DominioTemporal/GetDominioTitular/?idPersona={persona.PersonaId}&tramite={entradaPadreTitulo.TramiteEntrada.IdTramite}").Result)
                    {
                        resp.EnsureSuccessStatusCode();
                        var dominioTitular = resp.Content.ReadAsAsync<DominioTitularTemporal>().Result;

                        yield return new Propiedad
                        {
                            Id = "TipoPersona",
                            Value = null,
                            Text = persona.TipoPersona.Descripcion,
                            Visible = false,
                            Label = "Tipo Persona"
                        };
                        yield return new Propiedad
                        {
                            Id = "TipoTitularidad",
                            Value = dominioTitular.TipoTitularidadID.ToString(),
                            Text = tiposTitularidad.Single(x => x.Value == dominioTitular.TipoTitularidadID.ToString()).Text,
                            Visible = false,
                            Label = "Tipo Titularidad"
                        };
                        yield return new Propiedad
                        {
                            Id = "Titularidad",
                            Value = dominioTitular.PorcientoCopropiedad.ToString(),
                            Text = null,
                            Visible = false,
                            Label = "% de Titularidad"
                        };
                    }
                }
                #endregion
                #endregion
                token.ThrowIfCancellationRequested();
            }
            return RunTaskPropiedades(collect, token);
        }

        private Task<IEnumerable<Propiedad>> ProcesarDDJJ(Tuple<long, DDJJTemporal> tupla, IEnumerable<Data.BusinessEntities.DeclaracionesJuradas.DDJJVersion> versionesDDJJ, IEnumerable<Persona> personas, IEnumerable<Data.BusinessEntities.DeclaracionesJuradas.VALAptitudes> aptitudes, IEnumerable<Aforo> aforos, CancellationToken token)
        {
            IEnumerable<Propiedad> collect()
            {
                var djTemp = tupla.Item2;
                long idClaseParcela = tupla.Item1;

                var versionSeleccionada = versionesDDJJ.SingleOrDefault(v => v.IdVersion == djTemp.IdVersion);
                #region Propiedades
                #region Serialización según versión de DDJJ
                string ddjjSerialized;
                if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.U)
                {
                    var aux = djTemp.ToFormularioU(versionSeleccionada, personas, aforos);
                    aux.IdClaseParcela = idClaseParcela;
                    ddjjSerialized = JsonConvert.SerializeObject(aux);
                }
                else if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.SoR)
                {
                    var aux = djTemp.ToFormularioSoR(versionSeleccionada, personas, aptitudes.Where(apt => apt.IdVersion == djTemp.IdVersion));
                    aux.IdClaseParcela = idClaseParcela;
                    ddjjSerialized = JsonConvert.SerializeObject(aux);
                }
                else if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.E1 || versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.E2)
                {
                    /*
                     * Como, estructuralmente, las clases E1 y E2 son iguales, decido inicializar un formulario E1. 
                     * Ésto es arbitrario y mientras ambas clases se mantengan iguales.
                     * Cualquier cosa, que arreglen todo para usar herencia y bindear la clase base. A mi ni me busquen....
                     */
                    ddjjSerialized = JsonConvert.SerializeObject(djTemp.ToFormularioE1(versionSeleccionada));
                }
                else
                {
                    throw new Exception("Versión de DDJJ incorrecta");
                }
                #endregion
                yield return new Propiedad { Id = "IdVersion", Value = versionSeleccionada.IdVersion.ToString(), Text = versionSeleccionada.TipoDeclaracionJurada, Label = "Versión", Visible = true };
                yield return new Propiedad { Id = "Poligono", Value = djTemp.IdPoligono.ToString(), Label = $"ID Polígono s/Mensura", Visible = false };
                yield return new Propiedad { Id = "IdDDJJ", Value = djTemp.IdDeclaracionJurada.ToString(), Visible = false };
                yield return new Propiedad { Id = "DeclaracionJurada", Value = ddjjSerialized, Visible = false };
                #endregion
                token.ThrowIfCancellationRequested();
            }

            return RunTaskPropiedades(collect, token);
        }

        private static string GetResults(string tipos, IEnumerable<string> fields, Dictionary<string, string> filters, IEnumerable<string> busqueda)
        {
            var server = new SolrServer() { UseDefaultBaseParams = true };

            //para los tipos formo solo un fq con los tipos concatenados por el operador OR. el operador usado entre fq distintos es AND y como quiero
            //dar la posibilidad de buscar en varios tipos al mismo tiempo ese AND no me sirve
            server.AddParam(new SolrParam("fq", $"{string.Join(" OR ", tipos.Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).Select(tipo => $"tipo:{tipo}"))}"));
            foreach (var filter in filters)
            {
                server.AddParam(new SolrParam("fq", $"{filter.Key}:{filter.Value}"));
            }
            if (fields.Any())
            {
                server.AddParam(new SolrParam("fl", $"{string.Join(",", fields)}"));
            }
            var results = server.Search($"{string.Join(" OR ", busqueda)}");
            return results;
        }

        public ActionResult GeneracionRemito()
        {
            string titulo = "Generación de Remito";
            ViewBag.Title = titulo;

            string sectorUsuarioNombre = string.Empty;
            string sectorUsuarioId = string.Empty;
            if (Usuario.SectorUsuario != null)
            {
                sectorUsuarioNombre = Usuario.SectorUsuario.Nombre;
                sectorUsuarioId = Usuario.SectorUsuario.IdSector.ToString();
            }
            var lstItemsSectores = GetSectores().Where(s => s.Value != sectorUsuarioId);

            ViewData["sectoresDestinos"] = new SelectList(lstItemsSectores, "Value", "Text");

            return PartialView();
        }

        public ActionResult ReimpresionRemito()
        {
            string titulo = "Reimprimir Remito";
            ViewBag.Title = titulo;

            return PartialView();
        }

        public ActionResult AperturaLibroDiario()
        {
            string titulo = "Apertura de Libro Diario";
            ViewBag.Title = titulo;

            ViewBag.FechaLibro = DateTime.Today.Date.ToShortDateString();
            ViewBag.libroDiarioAbierto = GetParamtrosGeneralesByClave("LIBRO_DIARIO_ABIERTO");

            return PartialView();
        }

        public ActionResult CierreLibroDiario()
        {
            string titulo = "Cierre de Libro Diario";
            ViewBag.Title = titulo;

            ViewBag.FechaLibro = DateTime.Today.Date.ToShortDateString();
            ViewBag.libroDiarioAbierto = GetParamtrosGeneralesByClave("LIBRO_DIARIO_ABIERTO");

            return PartialView();
        }

        public ActionResult GeneracionListadoGeneralTramites()
        {
            string titulo = "Listado General de Trámites";
            ViewBag.Title = titulo;

            ViewData["tiposTramites"] = new SelectList(GetTiposTramitesItems("Todos"), "Value", "Text");
            ViewData["objetosTramites"] = GetObjetosTramites();
            ViewData["estadosTramites"] = new SelectList(GetEstadosTramites("Todos"), "Value", "Text");
            ViewData["prioridadesTramites"] = new SelectList(GetPrioridadesTramites("Todos"), "Value", "Text");
            ViewData["jurisdicciones"] = new SelectList(GetJurisdicciones("Todas"), "Value", "Text");
            ViewData["localidades"] = GetObjetosByTipo(TipoObjetoAdministrativoEnum.Localidad);

            return PartialView();
        }

        public ActionResult TramiteSave(METramite model, int[] tramitesRequisitos, MEDesglose[] desgloses, METramiteDocumento[] tramitesDocumentos, MEDatosEspecificos[] datosEspecificos, bool esConfirmacion, bool esPresentacion = false, bool esReingresar = false)
        {
            model._Ip = Request.UserHostAddress;
            model._Id_Usuario = Usuario.Id_Usuario;
            try
            {
                model._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                model._Machine_Name = Request.UserHostName;
            }
            var tramiteParameters = new METramiteParameters
            {
                Tramite = model,
                TramitesRequisitos = tramitesRequisitos,
                Desgloses = desgloses,
                TramitesDocumentos = tramitesDocumentos,
                DatosEspecificos = datosEspecificos
            };

            using (var resp = cliente.PostAsync($"api/MesaEntradas/Tramites/Save?esConfirmacion={esConfirmacion}&esPresentacion={esPresentacion}&esReingresar={esReingresar}", tramiteParameters, new JsonMediaTypeFormatter()).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    ActionResult rta;
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            dynamic res = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                            rta = Json(new { mensajes = new[] { res.Message?.Value }, error = true });
                            break;
                        case HttpStatusCode.Conflict:
                        case HttpStatusCode.PreconditionFailed:
                        case HttpStatusCode.ExpectationFailed:
                            var ex = JsonConvert.DeserializeObject<ValidacionTramiteException>(resp.Content.ReadAsStringAsync().Result);
                            Session["Current_IdTramite"] = ex.IdTramite;
                            rta = Json(new { mensajes = ex.Errores.ToList(), error = resp.StatusCode != HttpStatusCode.Conflict });
                            break;
                        default:
                            rta = Json(new { mensajes = new[] { resp.ReasonPhrase }, error = true });
                            break;
                    }
                    return rta;
                }
                Session["Current_IdTramite"] = resp.Content.ReadAsAsync<int>().Result;
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult ReloadTramite()
        {
            string operacion = Session["UltimaOperacionTramite"]?.ToString() == "Alta" ? "Modificacion" : (Session["UltimaOperacionTramite"]?.ToString() ?? "Consulta");
            return RedirectToAction("EdicionTramite", new { idTramite = Convert.ToInt32(Session["Current_IdTramite"]), operacion = operacion });
        }

        [HttpPost]
        public JsonResult Tramites(DataTableParameters parametros)
        {
            return Json(GetAllTramites(parametros));
        }

        private DataTableResult<GrillaTramite> GetAllTramites(DataTableParameters parametros)
        {
            var url = $"api/MesaEntradas/Tramites/EnProceso?idUsuario={Usuario.Id_Usuario}";

            var sIdSectorProfesional = GetParamtrosGeneralesByClave("ID_SECTOR_EXTERNO");
            if (sIdSectorProfesional != string.Empty)
            {
                if (Usuario.IdSector.GetValueOrDefault() == Convert.ToInt32(sIdSectorProfesional))
                {
                    url = $"api/MesaEntradas/Tramites/Todos?idUsuario={Usuario.Id_Usuario}";
                }
            }
            using (HttpResponseMessage resp = cliente.PostAsync(url, parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaTramite>>().Result;
            }
        }

        [HttpPost]
        public JsonResult TramitesPendientes(DataTableParameters parametros)
        {
            return Json(GetAllTramitesPendientes(parametros));
        }

        private DataTableResult<GrillaTramite> GetAllTramitesPendientes(DataTableParameters parametros)
        {
            using (HttpResponseMessage resp = cliente.PostAsync($"api/MesaEntradas/Tramites/Pendientes?idUsuario={Usuario.Id_Usuario}", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaTramite>>().Result;
            }
        }

        [HttpPost]
        public JsonResult TramitesProcesados(DataTableParameters parametros)
        {
            return Json(GetAllTramitesProcesados(parametros));
        }

        private DataTableResult<GrillaTramite> GetAllTramitesProcesados(DataTableParameters parametros)
        {
            using (HttpResponseMessage resp = cliente.PostAsync($"api/MesaEntradas/Tramites/Procesados?idUsuario={Usuario.Id_Usuario}", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaTramite>>().Result;
            }
        }

        [HttpPost]
        public JsonResult TramitesReasignados(DataTableParameters parametros)
        {
            return Json(GetAllTramitesReasignados(parametros));
        }

        private DataTableResult<GrillaTramite> GetAllTramitesReasignados(DataTableParameters parametros)
        {
            using (HttpResponseMessage resp = cliente.PostAsync($"api/MesaEntradas/Tramites/Reasignados?idUsuario={Usuario.Id_Usuario}", parametros, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DataTableResult<GrillaTramite>>().Result;
            }
        }

        public List<SelectListItem> GetTiposTramitesItems(string opcionVacia = "")
        {
            List<METipoTramite> lstTipoTramite = GetTiposTramites();

            List<SelectListItem> tipoTramiteItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                tipoTramiteItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                tipoTramiteItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var tipoTramite in lstTipoTramite.OrderBy(tt => tt.Descripcion))
            {
                tipoTramiteItems.Add(new SelectListItem { Text = tipoTramite.Descripcion, Value = Convert.ToString(tipoTramite.IdTipoTramite) });
            }
            return tipoTramiteItems;
        }
        public List<METipoTramite> GetTiposTramites()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/Tipos").Result;
            resp.EnsureSuccessStatusCode();
            List<METipoTramite> lstTipoTramite = (List<METipoTramite>)resp.Content.ReadAsAsync<IEnumerable<METipoTramite>>().Result;
            return lstTipoTramite;
        }

        public METipoTramite GetTipoTramiteById(int idTipoTramite)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/TiposById?id=" + idTipoTramite.ToString()).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<METipoTramite>().Result;
        }

        public List<ObjetoTramite> GetObjetosTramites()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/Objetos").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<ObjetoTramite>>().Result;
        }

        public List<MEObjetoTramite> GetObjetosTramitesByTipo(int idTipoTramite)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/ObjetosByTipoTramite?idTipoTramite=" + idTipoTramite.ToString()).Result;
            resp.EnsureSuccessStatusCode();
            return (List<MEObjetoTramite>)resp.Content.ReadAsAsync<IEnumerable<MEObjetoTramite>>().Result;
        }
        public List<SelectListItem> GetEstadosTramites(string opcionVacia = "")
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/Estados").Result;
            resp.EnsureSuccessStatusCode();
            List<MEEstadoTramite> lstEstadoTramite = (List<MEEstadoTramite>)resp.Content.ReadAsAsync<IEnumerable<MEEstadoTramite>>().Result;

            List<SelectListItem> estadoTramiteItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                estadoTramiteItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                estadoTramiteItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var estadoTramite in lstEstadoTramite)
            {
                estadoTramiteItems.Add(new SelectListItem { Text = estadoTramite.Descripcion, Value = Convert.ToString(estadoTramite.IdEstadoTramite) });
            }
            return estadoTramiteItems;
        }

        public List<SelectListItem> GetPrioridadesTramites(string opcionVacia = "")
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/Prioridades").Result;
            resp.EnsureSuccessStatusCode();
            List<MEPrioridadTramite> lstPrioridadTramite = (List<MEPrioridadTramite>)resp.Content.ReadAsAsync<IEnumerable<MEPrioridadTramite>>().Result;

            List<SelectListItem> prioridadTramiteItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                prioridadTramiteItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                prioridadTramiteItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var prioridadTramite in lstPrioridadTramite)
            {
                prioridadTramiteItems.Add(new SelectListItem { Text = prioridadTramite.Descripcion, Value = Convert.ToString(prioridadTramite.IdPrioridadTramite) });
            }
            return prioridadTramiteItems;
        }

        public List<SelectListItem> GetSectores(string opcionVacia = "")
        {
            HttpResponseMessage resp = cliente.GetAsync("api/SeguridadService/GetSectores").Result;
            resp.EnsureSuccessStatusCode();
            List<Sector> lstSector = (List<Sector>)resp.Content.ReadAsAsync<IEnumerable<Sector>>().Result;
            lstSector = lstSector.OrderBy(s => s.Nombre).ToList();

            List<SelectListItem> sectorItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                sectorItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                sectorItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var sector in lstSector)
            {
                sectorItems.Add(new SelectListItem { Text = sector.Nombre, Value = Convert.ToString(sector.IdSector) });
            }
            return sectorItems;
        }

        public List<SelectListItem> GetUsuariosMismoSector(long idSector, string opcionVacia = "")
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/SeguridadService/GetUsuariosMismoSector?idSector={idSector}").Result;
            resp.EnsureSuccessStatusCode();
            List<Usuarios> lstUsuarios = resp.Content.ReadAsAsync<List<Usuarios>>().Result;
            lstUsuarios = lstUsuarios.OrderBy(s => s.Nombre).ToList();

            List<SelectListItem> usuarioItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                usuarioItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                usuarioItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var usuario in lstUsuarios)
            {
                usuarioItems.Add(new SelectListItem { Text = usuario.NombreApellidoCompleto, Value = Convert.ToString(usuario.Id_Usuario) });
            }
            return usuarioItems;
        }

        public List<SelectListItem> GetJurisdicciones(string opcionVacia = "")
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/Jurisdicciones").Result;
            resp.EnsureSuccessStatusCode();
            List<GeoSit.Data.BusinessEntities.ObjetosAdministrativos.Objeto> lstJurisdiccion = (List<GeoSit.Data.BusinessEntities.ObjetosAdministrativos.Objeto>)resp.Content.ReadAsAsync<IEnumerable<GeoSit.Data.BusinessEntities.ObjetosAdministrativos.Objeto>>().Result;
            lstJurisdiccion = lstJurisdiccion.OrderBy(s => s.Codigo).ToList();

            List<SelectListItem> jurisdiccionItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                jurisdiccionItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                jurisdiccionItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var jurisdiccion in lstJurisdiccion)
            {
                jurisdiccionItems.Add(new SelectListItem { Text = jurisdiccion.Codigo + "-" + jurisdiccion.Nombre, Value = Convert.ToString(jurisdiccion.FeatId) });
            }
            return jurisdiccionItems;
        }

        public List<Data.BusinessEntities.ObjetosAdministrativos.Objeto> GetObjetosByTipo(TipoObjetoAdministrativoEnum tipo)
        {
            using (var resp = cliente.GetAsync($"api/ObjetoAdministrativoService/GetObjetoByTipo?id={(long)tipo}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<Data.BusinessEntities.ObjetosAdministrativos.Objeto>>().Result.ToList();
            }
        }
        public List<Data.BusinessEntities.ObjetosAdministrativos.Objeto> GetObjetosByTipoIdPadreIdObjetoHijo(long hijo, TipoObjetoAdministrativoEnum tipo)
        {
            using (var resp = cliente.GetAsync($"api/objetoadministrativoservice/objetos/tipo/{(long)tipo}/hijo/{hijo}/padre").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<Data.BusinessEntities.ObjetosAdministrativos.Objeto>>().Result.OrderBy(s => s.Nombre).ToList();
            }
        }


        public List<SelectListItem> GetDesglosesDestinos(string opcionVacia = "")
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MesaEntradas/Tramites/DesglosesDestinos").Result;
            resp.EnsureSuccessStatusCode();
            List<MEDesgloseDestino> lstDesgloseDestino = (List<MEDesgloseDestino>)resp.Content.ReadAsAsync<IEnumerable<MEDesgloseDestino>>().Result;

            List<SelectListItem> desgloseDestinoItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                desgloseDestinoItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                desgloseDestinoItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var desloseDestino in lstDesgloseDestino)
            {
                desgloseDestinoItems.Add(new SelectListItem { Text = desloseDestino.Descripcion, Value = Convert.ToString(desloseDestino.IdDesgloseDestino) });
            }
            return desgloseDestinoItems;

        }

        public async Task<ActionResult> EdicionObjeto(EdicionObjetoEspecificoModels request)
        {
            switch (request.operacion)
            {
                case Operacion.Alta:
                    {
                        var resp = await cliente.GetAsync($"api/MesaEntradas/Entrada/{request.tipoObjetoSelected}/");
                        resp.EnsureSuccessStatusCode();
                        var entrada = await resp.Content.ReadAsAsync<MEEntrada>();

                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.Parcela))
                        {
                            var zonas = GetTiposParcelasListItems();
                            var tipos = GetClasesParcelaListItems();
                            ViewData["habilitarBuscadorParcela"] = true;

                            return PartialView("Partials/Parcela", new Parcela
                            {
                                Operacion = request.arbolObjetoSeleccionado == null ? OperacionParcela.Origen : OperacionParcela.Destino,
                                Zonas = zonas,
                                Tipos = tipos
                            });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.UnidadTributaria))
                        {
                            ViewData["habilitarBuscadorUt"] = true;

                            return PartialView("Partials/UnidadFuncional", new UnidadFuncional
                            {
                                Operacion = request.arbolObjetoSeleccionado == null ? OperacionParcela.Origen : OperacionParcela.Destino,
                                Tipos = GetTiposUF()
                            });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.MensuraRegistrada))
                        {
                            ViewData["habilitarBuscadorMensura"] = true;
                            return PartialView("Partials/MensuraRegistrada", new MensuraRegistrada());
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.DescripcionInmueble))
                        {
                            return PartialView("Partials/DescripcionDelInmueble", new DescripcionDelInmueble());
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.DDJJ))
                        {
                            if (request.arbolObjetoSeleccionado == null)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            }
                            return PartialView("Partials/DDJJ", new DDJJ() { Versiones = GetVersionesDDJJListItems() });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.Manzana))
                        {
                            return PartialView("Partials/Manzana", new ManzanaViewModel());
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.Persona))
                        {
                            if (request.arbolObjetoSeleccionado == null)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            }
                            ViewData["habilitarBuscadorPersona"] = true;

                            return PartialView("Partials/Persona", new PersonaViewModel
                            {
                                MostrarCamposTitularidad = request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.Titulo),
                                TiposPersona = GetTiposPersona(),
                                TiposTitularidad = GetTiposTitularidad()
                            });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.Via))
                        {
                            return PartialView("Partials/Via", new ViaViewModel { Tipos = GetTiposVia() });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.LibreDeuda))
                        {
                            if (request.arbolObjetoSeleccionado == null)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            }
                            ViewData["HabilitaConsultaLibreDeudaDGR"] = Convert.ToBoolean(ConfigurationManager.AppSettings["HabilitaConsultaLibreDeudaDGR"]);
                            return PartialView("Partials/LibreDeDeuda", new LibreDeDeudaViewModel
                            {
                                EntesEmisores = GetTiposEnteEmisor()
                            });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.Titulo))
                        {
                            if (request.arbolObjetoSeleccionado == null)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                            }
                            return PartialView("Partials/Titulo", new TituloViewModel { Tipos = GetTiposTitulo() });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.ComprobantePago))
                        {
                            return PartialView("Partials/ComprobantePago", new ComprobantePago { TiposTasa = GetTiposTasa() });
                        }
                        else
                        if (entrada?.IdEntrada == Convert.ToInt32(Entradas.EspacioPublico))
                        {
                            return PartialView("Partials/EspacioPublico", new EspacioPublico());
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                        }
                    }
                case Operacion.Modificacion:
                    {

                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.Parcela))
                        {
                            var zonas = GetTiposParcelasListItems();
                            var tipos = GetClasesParcelaListItems();
                            ViewData["habilitarBuscadorParcela"] = false;

                            return PartialView("Partials/Parcela", new Parcela
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Operacion = (OperacionParcela)Enum.Parse(typeof(OperacionParcela), request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "hdnOperacionObjetoEspecifico").Value),
                                Partida = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "Partida")?.Value,
                                IdPartidaPersona = Convert.ToInt32(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPartidaPersona")?.Value),
                                Zona = int.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(Parcela.Zona))?.Value, out int zonaId) ? zonaId : (int?)null,
                                Tipo = int.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(Parcela.Tipo))?.Value, out int tipoId) ? tipoId : (int?)null,
                                Plano = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(Parcela.Plano))?.Value,
                                Zonas = zonas,
                                Tipos = tipos
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.UnidadTributaria))
                        {
                            ViewData["habilitarBuscadorUt"] = false;

                            var strFechaVigencia = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "vigencia")?.Value;

                            var fechaVigencia = (strFechaVigencia != null ? Convert.ToDateTime(strFechaVigencia) : (DateTime?)null);

                            return PartialView("Partials/UnidadFuncional", new UnidadFuncional
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Vigencia = fechaVigencia,
                                Operacion = (OperacionParcela)Enum.Parse(typeof(OperacionParcela), request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "hdnOperacionObjetoEspecifico").Value),
                                UnidadFuncionalCodigo = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(UnidadFuncional.UnidadFuncionalCodigo))?.Value,
                                IdUnidadFuncional = Convert.ToInt32(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "hdnIdUnidadFuncional")?.Value),
                                Plano = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(UnidadFuncional.Plano))?.Value,
                                Tipo = int.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(UnidadFuncional.Tipo))?.Value, out int tipoId) ? tipoId : (int?)null,
                                Piso = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(UnidadFuncional.Piso))?.Value,
                                Unidad = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(UnidadFuncional.Unidad))?.Value,
                                Superficie = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(UnidadFuncional.Superficie))?.Value,
                                Tipos = GetTiposUF()
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.MensuraRegistrada))
                        {

                            var objetoSeleccionado = request.arbolObjetoSeleccionado;
                            var id = long.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "hdnIdMensura")?.Value, out long idTipoMensura) ? idTipoMensura : -1;
                            var tMensura = "";
                            if (objetoSeleccionado != null && id > 0)
                            {

                                tMensura = GetTipoMensura(id);
                            }
                            else
                            {
                                tMensura = "";
                            }

                            ViewData["habilitarBuscadorMensura"] = false;

                            return PartialView("Partials/MensuraRegistrada", new MensuraRegistrada
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Mensura = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(MensuraRegistrada.Mensura))?.Value,
                                IdMensura = int.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "hdnIdMensura")?.Value, out int idMensura) ? idMensura : -1,
                                TipoMensura = tMensura,
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.DescripcionInmueble))
                        {
                            return PartialView("Partials/DescripcionDelInmueble", new DescripcionDelInmueble
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Descripcion = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(DescripcionDelInmueble.Descripcion))?.Value,
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.DDJJ))
                        {
                            return PartialView("Partials/DDJJ", new DDJJ
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                IdDDJJ = long.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(DDJJ.IdDDJJ))?.Value, out long ddjjId) ? (long?)ddjjId : null,
                                IdVersion = long.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(DDJJ.IdVersion))?.Value, out long versionId) ? (long?)versionId : null,
                                DeclaracionJurada = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(DDJJ.DeclaracionJurada))?.Value,

                                Versiones = GetVersionesDDJJListItems()
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.Manzana))
                        {
                            return PartialView("Partials/Manzana", new ManzanaViewModel
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Plano = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ManzanaViewModel.Plano))?.Value,
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.Persona))
                        {
                            ViewData["habilitarBuscadorPersona"] = false;

                            return PartialView("Partials/Persona", new PersonaViewModel
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                MostrarCamposTitularidad = request.padre == Convert.ToInt32(Entradas.Titulo),
                                TiposPersona = GetTiposPersona(),
                                TiposTitularidad = GetTiposTitularidad(),
                                Persona = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(p => p.Id == "Persona")?.Value,
                                Titularidad = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "Titularidad")?.Value,
                                TipoTitularidad = Convert.ToInt32(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "TipoTitularidad")?.Value),
                                IdPersona = Convert.ToInt32(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPersona")?.Value)
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.Via))
                        {
                            return PartialView("Partials/Via", new ViaViewModel
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Plano = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ViaViewModel.Plano))?.Value,
                                Tipos = GetTiposVia(),
                                Tipo = int.TryParse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ViaViewModel.Tipo))?.Value, out int tipoId) ? tipoId : -1,
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.LibreDeuda))
                        {
                            var strFechaVigencia = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "fecha-vigencia").Value;
                            var strFechaEmision = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "fecha-emision")?.Value;

                            var fechaVigencia = (strFechaVigencia != null ? Convert.ToDateTime(strFechaVigencia) : (DateTime?)null);
                            var fechaEmision = (strFechaEmision != null ? Convert.ToDateTime(strFechaEmision) : (DateTime?)null);

                            ViewData["HabilitaConsultaLibreDeudaDGR"] = Convert.ToBoolean(ConfigurationManager.AppSettings["HabilitaConsultaLibreDeudaDGR"]);
                            return PartialView("Partials/LibreDeDeuda", new LibreDeDeudaViewModel
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                FechaVigencia = fechaVigencia,
                                FechaEmision = fechaEmision,
                                NroCertificado = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(LibreDeDeudaViewModel.NroCertificado))?.Value,
                                Superficie = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(LibreDeDeudaViewModel.Superficie))?.Value,
                                Valuacion = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(LibreDeDeudaViewModel.Valuacion))?.Value,
                                Deuda = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(LibreDeDeudaViewModel.Deuda))?.Value,
                                EnteEmisor = int.Parse(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(LibreDeDeudaViewModel.EnteEmisor))?.Value),
                                EntesEmisores = GetTiposEnteEmisor()
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.Titulo))
                        {
                            var fecha = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "fecha");

                            return PartialView("Partials/Titulo", new TituloViewModel
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Inscripcion = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(TituloViewModel.Inscripcion))?.Value,
                                Fecha = fecha != null ? Convert.ToDateTime(fecha.Value) : (DateTime?)null,
                                Tipos = GetTiposTitulo(),
                                Tipo = Convert.ToInt32(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "Tipo")?.Value),
                            });
                        }
                        else
                        if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.ComprobantePago))
                        {
                            var fechaVencimiento = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "fecha-vencimiento");
                            var fechaPago = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "fecha-pago");
                            var fechaLiquidacion = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == "fecha-liquidacion");

                            return PartialView("Partials/ComprobantePago", new ComprobantePago
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                Identificacion = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ComprobantePago.Identificacion))?.Value,
                                TipoTramite = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ComprobantePago.TipoTramite))?.Value,
                                FechaLiquidacion = fechaLiquidacion != null ? Convert.ToDateTime(fechaLiquidacion.Value) : (DateTime?)null,
                                FechaPago = fechaPago != null ? Convert.ToDateTime(fechaPago.Value) : (DateTime?)null,
                                FechaVencimiento = fechaVencimiento != null ? Convert.ToDateTime(fechaVencimiento.Value) : (DateTime?)null,
                                MedioPago = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ComprobantePago.MedioPago))?.Value,
                                Importe = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ComprobantePago.Importe))?.Value,
                                EstadoPago = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ComprobantePago.EstadoPago))?.Value,
                                TiposTasa = GetTiposTasa(),
                                TipoTasa = Convert.ToInt32(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(ComprobantePago.TipoTasa))?.Value)
                            });
                        }
                        else
                         if (request.arbolObjetoSeleccionado.TipoObjeto.Id == Convert.ToInt32(Entradas.EspacioPublico))
                        {
                            return PartialView("Partials/EspacioPublico", new EspacioPublico
                            {
                                Guid = request.arbolObjetoSeleccionado.Guid,
                                IdTramiteEntrada = request.arbolObjetoSeleccionado.IdTramiteEntrada,
                                IdEspacioPublico = Convert.ToInt64(request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(EspacioPublico.IdEspacioPublico))?.Value ?? "-1"),
                                Superficie = request.arbolObjetoSeleccionado.Propiedades.FirstOrDefault(t => t.Id == nameof(EspacioPublico.Superficie))?.Value,
                            });
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                            //throw new ArgumentException($"Tipo de objeto {request.arbolObjetoSeleccionado.TipoObjeto.Text} invalido");
                        }
                    }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotImplemented);
            //throw new ArgumentException($"Operacion {request.operacion} inválida");
        }
        private IEnumerable<Data.BusinessEntities.DeclaracionesJuradas.DDJJVersion> GetVersionesDDJJ()
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetVersiones").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<Data.BusinessEntities.DeclaracionesJuradas.DDJJVersion>>().Result;
            }
        }

        private List<SelectListItem> GetVersionesDDJJListItems()
        {
            return GetVersionesDDJJ()
                        .Select(v => new SelectListItem() { Value = v.IdVersion.ToString(), Text = v.TipoDeclaracionJurada })
                        .ToList();
        }

        private ICollection<SelectListItem> GetTiposTitularidad()
        {
            var response = cliente.GetAsync("api/mesaentradas/TiposTitularidad").Result;
            return response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
        }

        private ICollection<SelectListItem> GetTiposPersona()
        {
            var response = cliente.GetAsync("api/mesaentradas/TiposPersona").Result;
            return response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
        }

        private ICollection<SelectListItem> GetTiposUF()
        {
            var response = cliente.GetAsync("api/mesaentradas/TiposUF").Result;
            return response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
        }

        public string GetTipoMensura(long idMensura)
        {
            var response = cliente.GetAsync($"api/MensuraTemporal/GetTipoMensura/?idMensura={idMensura}").Result;
            var tiposMensuras = response.Content.ReadAsAsync<TipoMensura>().Result;
            return tiposMensuras?.Descripcion;
        }

        public ActionResult GetDocumentoMensura(long id)
        {
            var response = cliente.GetAsync($"api/MensuraTemporal/GetDocumentoMensura/?idMensura={id}").Result;
            var documentoMensura = response.Content.ReadAsAsync<Documento>().Result;
            return RedirectToAction("view", "pdfInternalViewer", new { id = documentoMensura.id_documento });
        }

        private IEnumerable<Data.BusinessEntities.Inmuebles.ClaseParcela> GetClasesParcela()
        {
            using (var response = cliente.GetAsync("api/ClaseParcela/get").Result.EnsureSuccessStatusCode())
            {
                return response.Content.ReadAsAsync<IEnumerable<Data.BusinessEntities.Inmuebles.ClaseParcela>>().Result;
            }
        }

        private ICollection<SelectListItem> GetClasesParcelaListItems()
        {
            return GetClasesParcela()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ClaseParcelaID.ToString(),
                        Text = t.Descripcion
                    }).ToList();
        }

        private IEnumerable<TipoParcela> GetTiposParcela()
        {
            using (var response = cliente.GetAsync("api/TipoParcela/get").Result.EnsureSuccessStatusCode())
            {
                return response.Content.ReadAsAsync<IEnumerable<TipoParcela>>().Result;
            }
        }
        private ICollection<SelectListItem> GetTiposParcelasListItems()
        {
            return GetTiposParcela()
                    .Select(t => new SelectListItem
                    {
                        Value = t.TipoParcelaID.ToString(),
                        Text = t.Descripcion
                    }).ToList();
        }

        private ICollection<SelectListItem> GetDDJJ()
        {
            var response = cliente.GetAsync("api/mesaentradas/DDJJACopiar").Result;
            return response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
        }


        private ICollection<SelectListItem> GetTiposTasa()
        {
            using (var response = cliente.GetAsync("api/mesaentradas/TiposTasa").Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
            }
        }

        private ICollection<SelectListItem> GetTiposTitulo()
        {
            var response = cliente.GetAsync("api/mesaentradas/TiposTitulos").Result;
            return response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
        }

        private ICollection<SelectListItem> GetTiposEnteEmisor()
        {
            var response = cliente.GetAsync("api/mesaentradas/TiposEnteEmisor").Result;
            var result = response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
            return result;
        }

        private ICollection<SelectListItem> GetTiposVia()
        {
            var response = cliente.GetAsync("api/mesaentradas/TiposVia").Result;
            return response.Content.ReadAsAsync<ICollection<SelectListItem>>().Result;
        }

        public List<SelectListItem> GetTiposDocumentos(string opcionVacia = "")
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoDocumentoService/GetTiposDocumento").Result;
            List<TiposDocumentosModel> lstTipoDocumento = (List<TiposDocumentosModel>)resp.Content.ReadAsAsync<IEnumerable<TiposDocumentosModel>>().Result;

            List<SelectListItem> tipoDocumentoItems = new List<SelectListItem>();
            if (opcionVacia.Trim() != string.Empty)
            {
                tipoDocumentoItems.Add(new SelectListItem { Text = opcionVacia, Value = "" });
            }
            else
            {
                tipoDocumentoItems.Add(new SelectListItem { Text = "", Value = "" });
            }
            foreach (var tipoDocumento in lstTipoDocumento)
            {
                tipoDocumentoItems.Add(new SelectListItem { Text = tipoDocumento.Descripcion, Value = Convert.ToString(tipoDocumento.TipoDocumentoId) });
            }
            return tipoDocumentoItems;

        }

        public async Task<IEnumerable<SelectListItem>> GetEntradas(string opcionVacia = "")
        {
            Dictionary<int, string> lstEntradas = new Dictionary<int, string>();
            HttpResponseMessage resp = await cliente.GetAsync("api/MesaEntradas/Entradas");
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadAsAsync<IEnumerable<MEEntrada>>();

            return result.Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.IdEntrada.ToString()
            });
        }

        public string GetParamtrosGeneralesByClave(string clave)
        {
            var param = GetParamtrosGenerales().Where(p => p.Clave == clave).FirstOrDefault();
            return param?.Valor ?? string.Empty;
        }

        public List<ParametrosGenerales> GetParamtrosGenerales()
        {
            using (var resp = cliente.GetAsync("api/MesaEntradas/Tramites/ParametrosGenerales").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<ParametrosGenerales>>().Result.ToList();
            }
        }

        public JsonResult CargarAccionesByTramite(int tramite)
        {
            /* llamada a la api para determinar qué acciones puede hacer cada tramite */
            HttpResponseMessage resp = cliente.GetAsync($"api/MesaEntradas/Tramites/{tramite}/Acciones?idUsuario={Usuario.Id_Usuario}").Result;
            resp.EnsureSuccessStatusCode();
            return Json(resp.Content.ReadAsAsync<IEnumerable<METipoMovimiento>>().Result.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CargarAcciones(int[] tramites, bool grillaReasignable)
        {
            HttpResponseMessage resp = cliente.PostAsync($"api/MesaEntradas/Tramites/AccionesByTramites?idUsuario={Usuario.Id_Usuario}&grillaReasignable={grillaReasignable}", tramites ?? new int[0], new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return Json(resp.Content.ReadAsAsync<IEnumerable<METipoMovimiento>>().Result.ToList());
        }

        [HttpPost]
        public JsonResult CargarAccionesGenerales(int cantTramites, bool grillaReasignable)
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/MesaEntradas/Tramites/AccionesGenerales?idUsuario={Usuario.Id_Usuario}&cantTramites={cantTramites}&grillaReasignable={grillaReasignable}").Result;
            resp.EnsureSuccessStatusCode();
            return Json(resp.Content.ReadAsAsync<IEnumerable<METipoMovimiento>>().Result.ToList());
        }


        [HttpPost]
        public ActionResult EjecutarAccion(AccionParameters accionParameters)
        {
            InitializeAuditData(accionParameters);
            using (var resp = cliente.PostAsync($"api/MesaEntradas/Tramites/EjecutarAccion?idUsuario={Usuario.Id_Usuario}", accionParameters, new JsonMediaTypeFormatter()).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    ActionResult rta;
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            dynamic res = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                            rta = Json(new { mensajes = new[] { res.Message?.Value }, error = true });
                            break;
                        case HttpStatusCode.Conflict:
                        case HttpStatusCode.PreconditionFailed:
                        case HttpStatusCode.ExpectationFailed:
                            rta = Json(new { mensajes = JsonConvert.DeserializeObject<List<string>>(resp.Content.ReadAsStringAsync().Result), error = resp.StatusCode != HttpStatusCode.Conflict });
                            break;
                        default:
                            rta = Json(new { mensajes = new[] { resp.ReasonPhrase }, error = true });
                            break;
                    }
                    return rta;
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        private void InitializeAuditData(AccionParameters accionParameters)
        {
            accionParameters._Ip = Request.UserHostAddress;
            try
            {
                accionParameters._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                accionParameters._Machine_Name = Request.UserHostName;
            }
        }


        public METramite GetTramiteById(int idTramite)
        {
            if (idTramite <= 0)
            {
                return null;
            }
            using (var resp = cliente.GetAsync($"api/MesaEntradas/Tramites/{idTramite}?includeEntradas=false").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<METramite>().Result;
            }
        }

        [HttpPost]
        public JsonResult TramiteConfirmar(int idTramite)
        {
            METramite tramite = new METramite()
            {
                IdTramite = idTramite,
                _Ip = Request.UserHostAddress
            };
            try
            {
                tramite._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                tramite._Machine_Name = Request.UserHostName;
            }
            using (HttpResponseMessage resp = cliente.PostAsync($"api/MesaEntradas/Tramites/Confirmar?idUsuario={Usuario.Id_Usuario}", tramite, new JsonMediaTypeFormatter()).Result)
            {
                resp.EnsureSuccessStatusCode();
                var success = resp.Content.ReadAsAsync<int>().Result;
                return Json(new { success = success });
            }
        }

        [HttpGet]
        public JsonResult CargarRequisitos(int idTipoTramite)
        {
            var tipoTramite = GetTipoTramiteById(idTipoTramite);
            var tramite = Session["ModelTramite"] as METramite;

            var lstObject = tramite?.TramiteRequisitos?.Select(tramiteRequisito => new
            {
                IdRequisitoTramite = tramiteRequisito.ObjetoRequisito.IdRequisito,
                Descripcion = tramiteRequisito.ObjetoRequisito.Requisito.Descripcion,
                Cumplido = (tramite?.TramiteRequisitos ?? new METramiteRequisito[0]).Any(tr => tr.IdObjetoRequisito == tramiteRequisito.ObjetoRequisito.IdObjetoRequisito) ? 1 : 0,
                Obligatorio = tramiteRequisito.ObjetoRequisito.Obligatorio
            });
            return Json(new { data = lstObject }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetEntradasByObjeto(int idObjetoTramite)
        {
            if (HabilitaDatosEspecificos(Session["ModelTramite"] as METramite))
            {
                var response = cliente.GetAsync("api/mesaentradas/GetEntradasByObjeto?idObjetoTramite=" + idObjetoTramite).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public JsonResult RecuperarObjetosTramiteByTipo(int idTipoTramite)
        {
            var lstObjetoTramite = GetObjetosTramitesByTipo(idTipoTramite);
            if (lstObjetoTramite != null && lstObjetoTramite.Count() > 0)
            {
                var lstObject = lstObjetoTramite.Select(ot => new ObjetoTramite
                {
                    IdObjetoTramite = ot.IdObjetoTramite,
                    Descripcion = ot.Descripcion
                }).OrderBy(o => o.Descripcion);
                return Json(new { data = lstObject }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = (ObjetoTramite)null }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RecuperarLocalidadesByJurisdiccion(int idJurisdiccion)
        {
            var lstLocalidades = GetObjetosByTipoIdPadreIdObjetoHijo(idJurisdiccion, TipoObjetoAdministrativoEnum.Localidad);
            if (lstLocalidades != null && lstLocalidades.Count() > 0)
            {
                var lstObject = lstLocalidades.Select(loc => new Localidad
                {
                    IdLocalidad = loc.FeatId,
                    Descripcion = loc.Nombre
                }).OrderBy(o => o.Descripcion);
                return Json(new { data = lstObject }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = (Localidad)null }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RecuperarPrioridadesByTipo(int idTipoTramite)
        {
            var lstObject = GetTipoTramiteById(idTipoTramite)
                                .PrioridadesTipos
                                .Where(a => a.FechaBaja is null)
                                .Select(pt => new
                                {
                                    pt.IdPrioridadTramite,
                                    pt.PrioridadTramite.Descripcion
                                }).OrderBy(o => o.Descripcion);
            return Json(new { data = lstObject }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInformeCaratulaExpediente(int id)
        {
            var resp = clienteInformes.GetAsync($"api/InformesMesaEntradas/GetInformeCaratulaExpediente?id={id}&idUsuario={Usuario.Id_Usuario}").Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            //Certificado
            int idTipoDocumento = 3;
            string fileName = "CaratulaExpediente.pdf";
            InformeImpreso = new OperationItem<Documento>
            {
                Item = new Documento
                {
                    contenido = bytes,
                    extension_archivo = "pdf",
                    descripcion = "Carátula de Expediente",
                    id_tipo_documento = idTipoDocumento,
                    fecha = DateTime.Today,
                    nombre_archivo = fileName,
                    id_usu_alta = Usuario.Id_Usuario,
                    id_usu_modif = Usuario.Id_Usuario
                }
            };
            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }

        public ActionResult GetInformeDetallado(int id)
        {
            using (clienteInformes)
            using (var resp = clienteInformes.GetAsync($"api/InformesMesaEntradas/GetInformeDetallado?id={id}&idUsuario={Usuario.Id_Usuario}").Result)
            {
                resp.EnsureSuccessStatusCode();
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                //Certificado
                int idTipoDocumento = 3;
                string fileName = "InformeDetallado.pdf";
                InformeImpreso = new OperationItem<Documento>
                {
                    Item = new Documento
                    {
                        contenido = Convert.FromBase64String(bytes64),
                        extension_archivo = "pdf",
                        descripcion = "Informe Detallado",
                        id_tipo_documento = idTipoDocumento,
                        fecha = DateTime.Today,
                        nombre_archivo = fileName,
                        id_usu_alta = Usuario.Id_Usuario,
                        id_usu_modif = Usuario.Id_Usuario
                    }
                };
                ArchivoDescarga = new Models.ArchivoDescarga(InformeImpreso.Item.contenido, fileName, "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetInformeObservaciones(int id)
        {
            var resp = clienteInformes.GetAsync($"api/InformesMesaEntradas/GetInformeObservaciones?id={id}&idUsuario={Usuario.Id_Usuario}").Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            //Certificado
            int idTipoDocumento = 3;
            string fileName = "InformeObservaciones.pdf";
            InformeImpreso = new OperationItem<Documento>
            {
                Item = new Documento
                {
                    contenido = bytes,
                    extension_archivo = "pdf",
                    descripcion = "Informe Observaciones",
                    id_tipo_documento = idTipoDocumento,
                    fecha = DateTime.Today,
                    nombre_archivo = fileName,
                    id_usu_alta = Usuario.Id_Usuario,
                    id_usu_modif = Usuario.Id_Usuario
                }
            };
            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }

        public ActionResult GetInformeAdjudicacion(int id)
        {
            string usuario = $"{Usuario.Nombre} {Usuario.Apellido}";

            var resp = clienteInformes.GetAsync($"api/InformeAdjudicacion/Get?idTramite={id}&usuario={usuario}").Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            //Certificado
            int idTipoDocumento = 3;
            string fileName = "InformeAdjudicacion.pdf";
            InformeImpreso = new OperationItem<Documento>
            {
                Item = new Documento
                {
                    contenido = bytes,
                    extension_archivo = "pdf",
                    descripcion = "Informe Adjudicacion",
                    id_tipo_documento = idTipoDocumento,
                    fecha = DateTime.Today,
                    nombre_archivo = fileName,
                    id_usu_alta = Usuario.Id_Usuario,
                    id_usu_modif = Usuario.Id_Usuario
                }
            };
            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }

        public JsonResult DesgloseSave(MEDesglose model)
        {
            //InitializeAuditData(ref model);
            model._Ip = Request.UserHostAddress;
            try
            {
                model._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                model._Machine_Name = Request.UserHostName;
            }
            HttpResponseMessage resp = cliente.PostAsync($"api/MesaEntradas/Tramites/Save?idUsuario={Usuario.Id_Usuario}", model, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return Json(new { success = resp.Content.ReadAsAsync<bool>().Result });
        }

        [HttpPost]
        public JsonResult UploadDocumento()
        {
            if (this.Request.Files?.Count != 1)
            {
                throw new Exception("Hubo un error con el archivo recibido");
            }
            int idTramite = Convert.ToInt32(this.Request.Form["idTramite"]);
            var archivo = this.Request.Files[0];
            // el path deberia estar definido y deberia ser visible para tanto la api como para la web
            // ademas de armar una estructura para poder dejar "temporalmente" los archivos
            // ejemplo: <RUTA>\Tramites\<ID TRAMITE>\Documentos\Temporales\<ARCHIVO>
            // validar y generar un nombre de archivo, unico para que no rompa al grabar

            //se recuperan las notas del tramite en cuestion para saber qué nombres de archivo tienen
            //se recuperan los nombres de archivos de la carpeta temporal del tramite
            //se procesan las 2 listas para saber si hay conflicto de nombre y resolver segun formato a definir
            var lstTramiteDocumento = GetAllTramiteDocumentoByTramite(idTramite);

            string nombreArchivoFinal = Path.GetFileName(archivo.FileName);
            var directorio = Directory.CreateDirectory(Path.Combine(uploadTempFolder, idTramite.ToString(), "temporales"));
            string nombreArchivoFinalFull = Path.Combine(directorio.FullName, nombreArchivoFinal);
            bool fileExist = System.IO.File.Exists(nombreArchivoFinalFull);

            if (fileExist || lstTramiteDocumento.Any(td => td.Documento.nombre_archivo == nombreArchivoFinal.Substring(0, nombreArchivoFinal.Length - 4)))
            {
                string fechaNombre = DateTime.Now.ToString("yyyyMMdd_HH-mm-ss");
                nombreArchivoFinal = nombreArchivoFinal.Substring(0, nombreArchivoFinal.Length - 4) + "_" + fechaNombre + nombreArchivoFinal.Substring(nombreArchivoFinal.Length - 4);
                nombreArchivoFinalFull = Path.Combine(directorio.FullName, nombreArchivoFinal);
            }
            archivo.SaveAs(nombreArchivoFinalFull);
            return Json(new { nombreArchivo = nombreArchivoFinal });
        }

        [HttpGet]
        public ActionResult ExisteArchivo(string tramite, string archivo)
        {
            ArchivoDescarga = null;
            string path;
            archivo = Encoding.Default.GetString(Convert.FromBase64String(archivo));
            if (System.IO.File.Exists((path = Path.Combine(uploadTempFolder, tramite, "temporales", archivo))) ||
                System.IO.File.Exists((path = Path.Combine(uploadTempFolder, tramite, "documentos", archivo))))
            {
                ArchivoDescarga = new ArchivoDescarga(System.IO.File.ReadAllBytes(path), archivo, "application/octet-stream");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public List<METramiteDocumento> GetAllTramiteDocumentoByTramite(int tramite)
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/MesaEntradas/Tramites/{tramite}/TramitesDocumentos").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<METramiteDocumento>>().Result;
        }

        [HttpPost]
        public JsonResult GetMovimientosRemito(DateTime fechaDesde, DateTime fechaHasta, int sectorDestino)
        {
            fechaHasta = fechaHasta.Date.AddDays(1).AddSeconds(-1);
            var remitoParameters = new MERemitoParameters
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                IdSectorDestino = sectorDestino,
                IdSectorOrigen = Usuario.IdSector.GetValueOrDefault()
            };
            HttpResponseMessage resp = cliente.PostAsync("api/MesaEntradas/Tramites/MovimientosRemito", remitoParameters, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            var movimientos = resp.Content.ReadAsAsync<List<MEMovimiento>>().Result;
            return Json(new
            {
                data = movimientos.Select(m => new
                {
                    m.IdMovimiento,
                    Fecha = m.FechaAlta.ToString("dd/MM/yyyy HH:mm"),
                    m.Tramite.Numero,
                    Iniciador = m.Tramite.Iniciador?.NombreCompleto,
                    Estado = m.Estado.Descripcion
                })
            });
        }

        [HttpPost]
        public JsonResult GetMovimientosByIds(int[] aMovimientos)
        {
            HttpResponseMessage resp = cliente.PostAsync($"api/MesaEntradas/Tramites/MovimientosByIds", aMovimientos, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            var movimientos = resp.Content.ReadAsAsync<List<MEMovimiento>>().Result;
            return Json(new
            {
                data = movimientos.Select(m => new
                {
                    m.IdMovimiento,
                    Fecha = m.FechaAlta.ToString("dd/MM/yyyy HH:mm"),
                    m.Tramite.Numero,
                    Iniciador = m.Tramite.Iniciador?.NombreCompleto,
                    Estado = m.Estado.Descripcion
                })
            });
            //movimientos.ForEach(m => m.Tramite.Movimientos.Clear());
            //return Json(new { movimientos });
        }

        public ActionResult GenerarRemito(int sectorDestino, int[] movimientos)
        {
            MERemitoParameters remitoParameters = new MERemitoParameters
            {

                IdSectorDestino = sectorDestino,
                IdSectorOrigen = Usuario.SectorUsuario.IdSector,
                MovimientosId = movimientos,
                _Ip = Request.UserHostAddress,
                _Id_Usuario = Usuario.Id_Usuario
            };
            try
            {
                remitoParameters._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                remitoParameters._Machine_Name = Request.UserHostName;
            }

            HttpResponseMessage resp = cliente.PostAsync("api/MesaEntradas/Remitos/Save", remitoParameters, new JsonMediaTypeFormatter()).Result;
            if (!resp.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            resp.EnsureSuccessStatusCode();
            return RedirectToAction("GetInformeRemito", new { idRemito = resp.Content.ReadAsAsync<int>().Result });
        }
        public ActionResult ReimprimirRemito(string numero)
        {
            MERemito remito = GetRemitoByNumero(numero);
            if (remito == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            return RedirectToAction("GetInformeRemito", new { idRemito = remito.IdRemito });
        }
        public ActionResult GetInformeRemito(int idRemito)
        {
            HttpResponseMessage resp = clienteInformes.GetAsync($"api/InformesMesaEntradas/GetInformeRemito?idRemito={idRemito}&idUsuario={Usuario.Id_Usuario}").Result;
            if (!resp.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"Remito {idRemito}.pdf", "application/pdf");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public FileResult DownloadArchivo() => getArchivoDescargable();
        public FileResult AbrirInformeRemito() => getArchivoDescargable();
        public FileResult AbrirListadoGeneralTramites() => getArchivoDescargable();
        public FileResult AbrirInformeDetallado() => getArchivoDescargable();

        private FileResult getArchivoDescargable()
        {
            Response.AppendHeader("Content-Disposition", new ContentDisposition { FileName = ArchivoDescarga.NombreArchivo, Inline = true }.ToString());
            return File(ArchivoDescarga.Contenido, ArchivoDescarga.MimeType);
        }

        public MERemito GetRemitoByNumero(string numero)
        {
            if (numero.Trim() == string.Empty)
            {
                return null;
            }
            HttpResponseMessage resp = cliente.GetAsync($"api/MesaEntradas/Remitos/RemitoByNumero?numero=" + numero).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<MERemito>().Result;
        }

        [HttpPost]
        public ActionResult GenerarListadoGeneralTramites()
        {
            var content = new Dictionary<string, string>()
                {
                    { "fechaIngresoDesde", this.Request.Form["fechaIngresoDesde"] },
                    { "fechaIngresoHasta", this.Request.Form["fechaIngresoHasta"] },
                    { "fechaLibroDesde", this.Request.Form["fechaLibroDesde"] },
                    { "fechaLibroHasta", this.Request.Form["fechaLibroHasta"] },
                    { "fechaVencDesde", this.Request.Form["fechaVencDesde"] },
                    { "fechaVencHasta", this.Request.Form["fechaVencHasta"] },
                    { "idJurisdiccion", this.Request.Form["idJurisdiccion"] },
                    { "idLocalidad", this.Request.Form["idLocalidad"] },
                    { "idPrioridad", this.Request.Form["idPrioridad"] },
                    { "idTipoTramite", this.Request.Form["idTipoTramite"] },
                    { "idObjetoTramite", this.Request.Form["idObjetoTramite"] },
                    { "idEstado", this.Request.Form["idEstado"] },
                    { "iniciadorId", this.Request.Form["iniciadorId"] },
                    { "jurisdiccionText", this.Request.Form["jurisdiccionText"] },
                    { "localidadText", this.Request.Form["localidadText"] },
                    { "prioridadText", this.Request.Form["prioridadText"] },
                    { "tipoTramiteText", this.Request.Form["tipoTramiteText"] },
                    { "objetoTramiteText", this.Request.Form["objetoTramiteText"] },
                    { "estadoText", this.Request.Form["estadoText"] },
                    { "iniciadorText", this.Request.Form["iniciadorText"] }
                };
            using (var resp = clienteInformes.PostAsync($"api/InformesMesaEntradas/GenerarInformeGeneralTramites?idUsuario={Usuario.Id_Usuario}", content, new JsonMediaTypeFormatter()).Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    MvcApplication.GetLogger().LogInfo(resp.ReasonPhrase);
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result), $"ListGralTramites_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public JsonResult AbrirLibroDiario(DateTime fechaLibro)
        {
            MELibroDiarioParameters libroDiarioParameters = new MELibroDiarioParameters();
            libroDiarioParameters._Ip = Request.UserHostAddress;
            libroDiarioParameters._Id_Usuario = Usuario.Id_Usuario;
            try
            {
                libroDiarioParameters._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                libroDiarioParameters._Machine_Name = Request.UserHostName;
            }
            libroDiarioParameters.FechaLibro = fechaLibro;

            HttpResponseMessage resp = cliente.PostAsync("api/MesaEntradas/Tramites/AbrirLibroDiario", libroDiarioParameters, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return Json(new { success = resp.Content.ReadAsAsync<bool>().Result });
        }

        public ActionResult GetInformePendientesConfirmar()
        {
            var resp = clienteInformes.GetAsync($"api/InformesMesaEntradas/GetInformePendientesConfirmar?idUsuario={Usuario.Id_Usuario}").Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            //Certificado
            int idTipoDocumento = 3;
            string fileName = "InformePendientesConfirmar.pdf";
            InformeImpreso = new OperationItem<Documento>
            {
                Item = new Documento
                {
                    contenido = bytes,
                    extension_archivo = "pdf",
                    descripcion = "Informe Pendientes Libro Diario",
                    id_tipo_documento = idTipoDocumento,
                    fecha = DateTime.Today,
                    nombre_archivo = fileName,
                    id_usu_alta = Usuario.Id_Usuario,
                    id_usu_modif = Usuario.Id_Usuario
                }
            };
            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }

        public JsonResult CerrarLibroDiario(DateTime fechaLibro)
        {
            MELibroDiarioParameters libroDiarioParameters = new MELibroDiarioParameters();
            libroDiarioParameters._Ip = Request.UserHostAddress;
            libroDiarioParameters._Id_Usuario = Usuario.Id_Usuario;
            try
            {
                libroDiarioParameters._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                libroDiarioParameters._Machine_Name = Request.UserHostName;
            }
            libroDiarioParameters.FechaLibro = fechaLibro;

            HttpResponseMessage resp = cliente.PostAsync("api/MesaEntradas/Tramites/CerrarLibroDiario", libroDiarioParameters, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return Json(new { success = resp.Content.ReadAsAsync<bool>().Result });
        }

        //public string GenerarTramite(string[] valores)
        //{
        //    if (valores != null)
        //    {
        //        HttpResponseMessage resp = cliente.PostAsync($"api/mesaentradas/GenerarTramite", valores, new JsonMediaTypeFormatter()).Result;

        //        if (resp.StatusCode == HttpStatusCode.OK)
        //            return resp.Content.ReadAsAsync<string>().Result;

        //        return "Error al obtener los datos";
        //    }
        //    else
        //    {
        //        return "Error en el ingreso de datos";
        //    }
        //}

        public ActionResult GetRequisitosTramitesByObjeto(int idObjetoTramite, int idTramite)
        {
            using (var resp = cliente.GetAsync($"api/MesaEntradas/Tramites/objetosbyrequisito?idObjetoTramite={idObjetoTramite}&idTramite={Math.Max(idTramite, 0)}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<List<MERequisitoCumplido>>().Result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerExpresionRegular(long tipo)
        {
            using (var resp = cliente.GetAsync($"api/TipoInscripcion/{tipo}/ejemploRegex").Result)
            {
                resp.EnsureSuccessStatusCode();
                var inscripcion = resp.Content.ReadAsAsync<TipoInscripcion>().Result;
                using (var resp2 = cliente.GetAsync($"api/GenericoService/RegexRandomGenerator?regex={Convert.ToBase64String(Encoding.UTF8.GetBytes(inscripcion.ExpresionRegular))}").Result)
                {
                    resp2.EnsureSuccessStatusCode();
                    return Json(new { regex = inscripcion.ExpresionRegular, ejemplo = JsonConvert.DeserializeObject(resp2.Content.ReadAsStringAsync().Result) }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult GetUnidadTributariaByParcela(long idParcela)
        {
            using (var resp = cliente.GetAsync($"api/UnidadTributaria/GetUnidadTributariaByParcela?idParcela={idParcela}").Result)
            {
                resp.EnsureSuccessStatusCode();
                var ut = resp.Content.ReadAsAsync<UnidadTributaria>().Result;
                return Json(new { id = ut.UnidadTributariaId, tipo = ut.TipoUnidadTributariaID }, JsonRequestBehavior.AllowGet);
            }
        }

        public void SendEmail(string subject, string body, string to)
        {
            try
            {
                SeguridadController seguridad = new SeguridadController();
                List<ParametrosGeneralesModel> pgm = seguridad.GetParametrosGenerales();

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["mail.smtp"]);

                mail.From = new MailAddress(ConfigurationManager.AppSettings["mail.sender.notificar"]);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["mail.port"]);
                SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["mail.user"], ConfigurationManager.AppSettings["mail.password"]);
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("EnviarCorreo", ex);
            }
        }

        public JsonResult GetTramiteByIdEmail(int idTramite)
        {
            var resp = cliente.GetAsync($"api/MesaEntradas/GetTramiteEmail/?idTramite={idTramite}").Result;
            resp.EnsureSuccessStatusCode();
            var tramite = resp.Content.ReadAsAsync<METramite>().Result;
            return Json(tramite);

        }

        public JsonResult GetPersonaByIdTramite(int idTramite)
        {
            var response = cliente.GetAsync($"api/MesaEntradas/GetTramitePersonaEmail/?idTramite={idTramite}").Result;
            response.EnsureSuccessStatusCode();
            var persona = response.Content.ReadAsAsync<Persona>().Result;
            return Json(persona);
        }

        public ActionResult GetEmailTramite(int idTramite)
        {
            var response = cliente.GetAsync($"api/MesaEntradas/GetTramitePersonaEmail/?idTramite={idTramite}").Result;
            var persona = response.Content.ReadAsAsync<Persona>().Result;
            var tramite = GetTramiteById(idTramite);
            SendEmail("Notificación del estado del Trámite",
                 "<b>Tramite N°: </b>" + tramite.Numero + "<br />" +
                 "<b>Tipo: </b>" + GetTipoTramiteById(tramite.IdTipoTramite).Descripcion + "<br />" +
                 "<b>Objeto: </b>" + GetObjetosTramites().Where(x => x.IdObjetoTramite == tramite.IdObjetoTramite).FirstOrDefault().Descripcion + "<br />" +
                 "<b>Motivo: </b>" + tramite.Motivo + "<br />" +
                 "<b>Estado: </b>" + tramite.Estado.Descripcion + "<br />" +
                 "<b>Fecha de Vencimiento: </b>" + tramite.FechaVenc,
                 persona.Email);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public JsonResult GetPlantilla(int idObjetoTramite)
        {
            var resp = cliente.GetAsync($"api/MesaEntradas/GetPlantilla/?idObjetoTramite={idObjetoTramite}").Result;
            resp.EnsureSuccessStatusCode();
            var objetoTrmaite = resp.Content.ReadAsAsync<MEObjetoTramite>().Result;
            return Json(objetoTrmaite);

        }

        [HttpGet]
        public ActionResult EsNotaEditable(string id)
        {
            if (long.TryParse(id, out long _id))
            {
                var response = cliente.GetAsync($"api/MesaEntradas/GetNotaEditable/?idNota={_id}&idUsuario={Usuario.Id_Usuario}").Result;
                if (!response.IsSuccessStatusCode || !response.Content.ReadAsAsync<bool>().Result)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult GetComprobantePago(long tramiteId, int idTipoTasa)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //Raro
            using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiRentasURL"]) })
            {
                try
                {
                    var resp = cliente.GetAsync($"tasacatastro?tramiteId={tramiteId}").Result.EnsureSuccessStatusCode();
                    var dato = resp.Content.ReadAsStringAsync().Result;
                    var fecha = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaMaximaComprobanteLeyConvenio"]);
                    if (dato != "[]" || idTipoTasa == 1 || DateTime.Today <= fecha)
                    {
                        return Json(new { comprobante = dato }, JsonRequestBehavior.AllowGet);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                catch (Exception exe)
                {
                    if (exe is SocketException || exe is HttpRequestException || exe is AggregateException)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }

        [HttpGet]
        public string GetLibreDeuda(string partida)
        {
            using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiRentasURL"]) })
            {
                var resp = cliente.GetAsync($"libredeuda?partida={partida}").Result.EnsureSuccessStatusCode();
                return resp.Content.ReadAsStringAsync().Result;
            }
        }

        public ActionResult GetPersonaByIdUt(long idUnidadTributaria)
        {
            var persona = GetPersonaByIdUnidadTributaria(idUnidadTributaria);
            return Json(persona);
        }

        private string GetPersonaByIdUnidadTributaria(long idUnidadTributaria)
        {
            var resp = cliente.GetAsync("api/MesaEntradas/GetPersonaByIdUt?idUnidadTributaria=" + idUnidadTributaria).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<string>().Result;
        }

    }
}