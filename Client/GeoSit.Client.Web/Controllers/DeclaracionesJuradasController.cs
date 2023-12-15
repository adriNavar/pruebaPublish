using GeoSit.Client.Web.Helpers;
using GeoSit.Client.Web.Helpers.ExtensionMethods;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Via;
using GeoSit.Web.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Client.Web.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class DeclaracionesJuradasController : Controller
    {
        private bool EsTemporal { get { return (bool)Session["DDJJTemporal"]; } set { Session["DDJJTemporal"] = value; } }

        private HttpClient cliente = new HttpClient();

        public DeclaracionesJuradasController()
        {
            cliente.Timeout = TimeSpan.FromMinutes(30);
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET: DeclaracionesJuradas
        public ActionResult DeclaracionesJuradas(long idUnidadTributaria, bool editable = true, bool cargadoDelBuscador = false)
        {
            var model = new DeclaracionesJuradasModel()
            {
                IdUnidadTributaria = idUnidadTributaria,
                PartidaInmobiliaria = GetUnidadTributaria(idUnidadTributaria)?.CodigoProvincial
            };
            ViewData["Editable"] = editable && SeguridadController.ExisteFuncion(Seguridad.AdministracionDDJJ);
            ViewData["CargadoDelBuscador"] = cargadoDelBuscador;
            return PartialView("_DeclaracionesJuradas", model);
        }

        public ActionResult DeclaracionesJuradasHeader(long idUnidadTributaria)
        {
            DeclaracionesJuradasModel model = new DeclaracionesJuradasModel();
            VALValuacion valuacion = GetValuacionVigenteAPI(idUnidadTributaria);

            if (valuacion != null)
            {
                model.ValuacionVigente = true;
                model.ValorTierra = valuacion.ValorTierra;
                model.VigenciaValorTierra = valuacion.FechaDesde;
                model.ValorMejoras = valuacion.ValorMejoras;
                model.VigenciaValorMejoras = valuacion.FechaDesde;
                model.ValorFiscalTotal = valuacion.ValorTotal;
                model.VigenciaValorFiscalTotal = valuacion.FechaDesde;
                model.UltimoDecretoAplicado = (valuacion.ValuacionDecretos?.Any() ?? false) ? valuacion.ValuacionDecretos.OrderByDescending(x => x.Decreto.FechaInicio).Select(x => x.Decreto.NroDecreto.ToString()).FirstOrDefault() ?? string.Empty : string.Empty;
                model.ListaDecretos = (valuacion.ValuacionDecretos?.Any() ?? false) ? string.Join(", ", valuacion.ValuacionDecretos.OrderBy(x => x.Decreto.FechaInicio).Select(x => x.Decreto.NroDecreto.ToString())) : string.Empty;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Valuaciones(long idUnidadTributaria, bool editable = true)
        {
            ValuacionesModel model = new ValuacionesModel()
            {
                IdUnidadTributaria = idUnidadTributaria,
                PartidaInmobiliaria = GetUnidadTributaria(idUnidadTributaria)?.CodigoProvincial
            };
            ViewData["Editable"] = editable;
            return PartialView("_Valuaciones", model);
        }

        public ActionResult GetValuacionesHeader(long idUnidadTributaria)
        {
            ValuacionesModel model = new ValuacionesModel();

            VALValuacion valuacion = GetValuacionVigenteAPI(idUnidadTributaria);

            if (valuacion != null)
            {
                model.ValuacionVigente = true;
                model.ValorTierra = valuacion.ValorTierra;
                model.VigenciaValorTierra = valuacion.FechaDesde;
                model.ValorMejoras = valuacion.ValorMejoras;
                model.VigenciaValorMejoras = valuacion.FechaDesde;
                model.ValorFiscalTotal = valuacion.ValorTotal;
                model.VigenciaValorFiscalTotal = valuacion.FechaDesde;
                model.UltimoDecretoAplicado = (valuacion.ValuacionDecretos?.Any() ?? false) ? valuacion.ValuacionDecretos.OrderByDescending(x => x.Decreto.FechaInicio).Select(x => x.Decreto.NroDecreto.ToString()).FirstOrDefault() ?? string.Empty : string.Empty;
                model.ListaDecretos = (valuacion.ValuacionDecretos?.Any() ?? false) ? string.Join(", ", valuacion.ValuacionDecretos.OrderBy(x => x.Decreto.NroDecreto).Select(x => x.Decreto.NroDecreto.ToString())) : string.Empty;

                if (valuacion.IdDeclaracionJurada.HasValue)
                {
                    METramite tramite = GetTramite(valuacion.IdDeclaracionJurada.Value);
                    if (tramite != null)
                    {
                        model.Tramite = tramite.Numero.ToString();
                    }
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MantenimientoParcelario(string partidaInmobiliaria)
        {

            partidaInmobiliaria = "A1-004879-1";

            MantenimientoParcelarioModel model = new MantenimientoParcelarioModel();
            model.PartidaInmobiliaria = "A1-004879-1";
            model.Departamento = "07";
            model.Distrito = "01";
            model.Seccion = "02";
            model.Manzana = "012";
            model.Parcela = "014";

            model.ValorTierra = 300000;
            model.VigenciaValorTierra = new DateTime(2020, 12, 31);
            model.ValorMejoras = 50000;
            model.VigenciaValorMejoras = new DateTime(2020, 12, 31);
            model.ValorFiscalTotal = 350000;
            model.VigenciaValorFiscalTotal = new DateTime(2020, 12, 31);

            return PartialView("_MantenimientoParcelario", model);
        }

        public ActionResult GetDeclaracionesJuradas(long IdUnidadTributaria)
        {
            DeclaracionesJuradasListaModel model = new DeclaracionesJuradasListaModel();
            List<DDJJ> ddjj = GetDeclaracionesJuradasAPI(IdUnidadTributaria);

            model.Lista = new List<DDJJModel>();

            foreach (DDJJ dj in ddjj)
            {
                METramite tramite = GetTramite(dj.IdDeclaracionJurada);
                VALValuacion valuacion = dj.Valuaciones?.Where(x => x.IdUnidadTributaria == IdUnidadTributaria && x.FechaHasta != null).OrderByDescending(x => x.FechaDesde).FirstOrDefault();

                model.Lista.Add(new DDJJModel()
                {
                    IdDeclaracionJurada = dj.IdDeclaracionJurada,
                    IdVersion = dj.IdVersion,
                    TipoDDJJ = dj.Version.TipoDeclaracionJurada.Substring(0, dj.Version.TipoDeclaracionJurada.IndexOf('(')).Trim(),
                    Tipo = dj.Version.TipoDeclaracionJurada,
                    VigenciaDesde = new { display = dj?.FechaVigencia.Value.ToShortDateString() ?? "-", timestamp = dj?.FechaVigencia.Value.Ticks },
                    VigenciaHasta = new { display = "-", timestamp = valuacion?.FechaHasta?.Ticks },
                    Version = dj.Version.VersionDeclaracionJurada.ToString(),
                    Valor = valuacion?.ValorTotal.ToString(),
                    Origen = dj.Origen.Descripcion,
                    Tramite = tramite?.Numero ?? "-",
                    IdTramite = tramite?.IdTramite
                });
            }
            return Json(new { data = model.Lista }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDeclaracionesJuradasE1E2NoVigentes(long IdUnidadTributaria)
        {
            DeclaracionesJuradasListaModel model = new DeclaracionesJuradasListaModel();
            List<DDJJ> ddjj = GetDeclaracionesJuradasE1E2NoVigentesAPI(IdUnidadTributaria);

            model.Lista = new List<DDJJModel>();

            foreach (DDJJ dj in ddjj)
            {
                METramite tramite = GetTramite(dj.IdDeclaracionJurada);
                var valuaciones = dj.Valuaciones?.Where(x => x.IdUnidadTributaria == IdUnidadTributaria && x.FechaHasta != null);
                DateTime? fechaDesde = valuaciones?.Min(v => v.FechaDesde);
                DateTime? fechaHasta = valuaciones?.Where(v => v.FechaHasta != null).Max(v => v.FechaHasta.Value);
                decimal? total = valuaciones?.OrderByDescending(v => v.FechaDesde).FirstOrDefault()?.ValorTotal;

                model.Lista.Add(new DDJJModel()
                {
                    IdDeclaracionJurada = dj.IdDeclaracionJurada,
                    IdVersion = dj.IdVersion,
                    TipoDDJJ = dj.Version.TipoDeclaracionJurada.Substring(0, dj.Version.TipoDeclaracionJurada.IndexOf('(')).Trim(),
                    Tipo = dj.Version.TipoDeclaracionJurada,
                    VigenciaDesde = new { display = fechaDesde?.ToShortDateString() ?? "-", timestamp = fechaDesde?.Ticks },
                    VigenciaHasta = new { display = fechaHasta?.ToShortDateString() ?? "-", timestamp = fechaHasta?.Ticks },
                    Version = dj.Version.VersionDeclaracionJurada.ToString(),
                    Valor = total?.ToString(),
                    Origen = dj.Origen.Descripcion,
                    Tramite = tramite?.Numero ?? "-",
                    IdTramite = tramite?.IdTramite
                });
            }
            return Json(new { data = model.Lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string EsFechaValida(DDJJ ddjj, long IdUnidadTributaria, long IdVersion)
        {
            using (var result = cliente.GetAsync($"api/DeclaracionJurada/GetValuaciones?idUnidadTributaria={IdUnidadTributaria}").Result)
            {
                result.EnsureSuccessStatusCode();
                var fechaDesde = result.Content.ReadAsAsync<List<VALValuacion>>().Result.Where(x => x.FechaHasta == null).OrderByDescending(x => x.FechaDesde).FirstOrDefault()?.FechaDesde.Date;

                return JsonConvert.SerializeObject(new
                {
                    valid = ddjj.FechaVigencia >= fechaDesde.GetValueOrDefault()
                                || (SeguridadController.ExisteFuncion(Seguridad.PermiteDDJJSoRNoVigentes)
                                        && IdVersion == Convert.ToInt64(VersionesDDJJ.SoR))
                });
            }

        }


        public ActionResult GetValuacionesHistoricas(long idUnidadTributaria)
        {
            var model = new ValuacionesListaModel()
            {
                Lista = new List<DDJJValuacion>()
            };

            var valuacionesHistoricas = GetValuacionesHistoricasAPI(idUnidadTributaria);
            foreach (var v in valuacionesHistoricas)
            {
                string nroTramite = null;
                if (v.IdDeclaracionJurada.HasValue)
                {
                    nroTramite = GetTramite(v.IdDeclaracionJurada.Value)?.Numero;
                }

                model.Lista.Add(new DDJJValuacion()
                {
                    IdValuacion = v.IdValuacion,
                    VigenciaDesde = v.FechaDesde,
                    VigenciaHasta = v.FechaHasta,
                    ValorTierra = v.ValorTierra,
                    ValorMejoras = v.ValorMejoras,
                    Decreto = v.ValuacionDecretos.Count > 0 ? string.Join(", ", v.ValuacionDecretos.OrderBy(x => x.Decreto.NroDecreto).Select(x => x.Decreto.NroDecreto)) : string.Empty,
                    Superficie = v.Superficie,
                    VFT = v.ValorTotal,
                    Tramite = nroTramite,
                    Vigente = false
                });
            }

            return Json(new { data = model.Lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteValuacion(int idValuacion)
        {
            try
            {
                var postModel = new
                {
                    idValuacion = idValuacion,
                    idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario
                };

                var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");

                HttpResponseMessage resp = cliente.PostAsync("api/DeclaracionJurada/DeleteValuacion", content).Result;
                resp.EnsureSuccessStatusCode();
                return Json(new { success = resp.Content.ReadAsAsync<bool>().Result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetValuacion(long idValuacion, long idUnidadTributaria, bool readOnly = false)
        {
            var valuacion = GetValuacionAPI(idValuacion);

            var model = new ValuacionesModel()
            {
                ReadOnly = readOnly,
                IdUnidadTributaria = idUnidadTributaria
            };

            if (valuacion != null)
            {
                if (valuacion.IdDeclaracionJurada.HasValue)
                {
                    model.Tramite = GetTramite(valuacion.IdDeclaracionJurada.Value)?.Numero;
                }

                model.IdValuacion = valuacion.IdValuacion;
                model.Superficie = valuacion.Superficie.HasValue ? valuacion.Superficie.ToString() : "";
                model.FechaDesde = valuacion.FechaDesde;
                model.FechaHasta = valuacion.FechaHasta;
                model.ValorTierra = valuacion.ValorTierra;
                model.ValorMejoras = valuacion.ValorMejoras.HasValue ? valuacion.ValorMejoras.Value : 0;
                model.ValorFiscalTotal = valuacion.ValorTotal;

                if (valuacion.ValuacionDecretos != null && valuacion.ValuacionDecretos.Count > 0)
                {
                    model.DecretoAplicado = string.Join(", ", valuacion.ValuacionDecretos.OrderByDescending(x => x.Decreto.FechaInicio).Select(x => x.Decreto.NroDecreto));
                }
            }

            return PartialView("Valuacion/_DatosValuacion", model);
        }

        //[HttpPost]
        //public ActionResult GuardarValuacion(ValuacionesModel model)
        //{
        //    try
        //    {
        //        VALValuacion valuacion = new VALValuacion();
        //        valuacion.IdValuacion = model.IdValuacion;
        //        valuacion.IdUnidadTributaria = model.IdUnidadTributaria;
        //        valuacion.IdDeclaracionJurada = model.IdDeclaracionJurada;
        //        valuacion.FechaDesde = model.Fecha.Value;
        //        valuacion.ValorTierra = model.ValorTierra;
        //        valuacion.ValorMejoras = model.ValorMejoras;
        //        valuacion.ValorTotal = model.ValorTierra + (model.ValorMejoras ?? 0);
        //        valuacion.Superficie = double.Parse(model.Superficie);

        //        var postModel = new
        //        {
        //            valuacion = valuacion,
        //            idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario
        //        };

        //        var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");


        //        HttpResponseMessage resp = cliente.PostAsync("api/DeclaracionJurada/SaveValuacion", content).Result;
        //        resp.EnsureSuccessStatusCode();
        //        return Json(new { success = resp.Content.ReadAsAsync<bool>().Result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult IniciarDDJJ(DDJJInicio data)
        {
            EsTemporal = string.IsNullOrEmpty(data.PartidaInmobiliaria);
            if (!data.IdTipoUnidadTributaria.HasValue)
            {//es ut de origen, o sea que no trae el tipo de UT desde la UI
                using (var result = cliente.GetAsync($"api/unidadtributaria/get?id={data.IdUnidadTributaria}").Result)
                {
                    result.EnsureSuccessStatusCode();
                    data.IdTipoUnidadTributaria = result.Content.ReadAsAsync<UnidadTributaria>().Result.TipoUnidadTributariaID;
                }
            }

            using (var result = cliente.GetAsync("api/DeclaracionJurada/GetVersiones").Result)
            {
                result.EnsureSuccessStatusCode();
                var versiones = result.Content.ReadAsAsync<IEnumerable<DDJJVersion>>().Result
                                    .Where(v => data.IdTipoUnidadTributaria != 3 || (v.TipoDeclaracionJurada != FormularioEnum.U && v.TipoDeclaracionJurada != FormularioEnum.SoR))
                                    .GroupBy(x => x.TipoDeclaracionJurada)
                                    .OrderBy(x => x.Key);

                UnidadTributaria getUnidadTributariaParcela(long idParcela)
                {
                    //busco la UT con tipo != 3. teoricamente no debería haber una UF con DDJJ tierra (U o SoR)
                    using (var resp = cliente.GetAsync($"api/UnidadTributaria/GetUnidadTributariaByParcela?idParcela={idParcela}").Result)
                    {
                        resp.EnsureSuccessStatusCode();
                        return resp.Content.ReadAsAsync<UnidadTributaria>().Result;
                    }
                }

                if (data.IdTipoParcela == null)
                {
                    data.IdTipoParcela = GetParcelaByUt(data.IdUnidadTributaria ?? 0)?.TipoParcelaID;
                }


                var formulario = versiones.Select(b => new SelectListItem() { Text = b.Key, Value = b.First().IdVersion.ToString() }).ToList();

                if (data.IdTipoParcela == 1)
                {
                    formulario = formulario.Where(x => !x.Value.Contains("3")).ToList();
                }
                else
                {
                    formulario = formulario.Where(x => !x.Value.Contains("4")).ToList();
                }


                string utsOrigen = JsonConvert.SerializeObject((data.DominiosOriginales ?? new DominioPrecarga[0]).Aggregate(new long[0], (accum, elem) => accum.Append(!elem.EsParcela ? elem.Id : getUnidadTributariaParcela(elem.Id).UnidadTributariaId).ToArray()));
                var model = new SeleccionFormularioModel()
                {
                    IdUnidadTributaria = data.IdUnidadTributaria ?? 0,
                    PartidaInmobiliaria = data.PartidaInmobiliaria,
                    Formularios = /*versiones.Select(g => new SelectListItem() { Text = g.Key, Value = g.First().IdVersion.ToString() }).ToList()*/formulario,
                    Versiones = JsonConvert.SerializeObject(versiones.SelectMany(v => v)),
                    Poligono = data.Poligono,
                    UnidadesTributariasOrigen = utsOrigen,
                    IdClaseParcela = data.IdClaseParcela
                };
                return PartialView("_SeleccionFormulario", model);
            }
        }

        public ActionResult SeleccionFormulario(long IdUnidadTributaria, long? IdClaseParcela)
        {
            var ut = GetUnidadTributaria(IdUnidadTributaria);
            var IdTipoParcela = GetParcelaByUt(IdUnidadTributaria).TipoParcelaID;

            return IniciarDDJJ(new DDJJInicio()
            {
                IdUnidadTributaria = ut.UnidadTributariaId,
                IdTipoUnidadTributaria = ut.TipoUnidadTributariaID,
                PartidaInmobiliaria = ut.CodigoProvincial,
                IdClaseParcela = IdClaseParcela,
                IdTipoParcela = IdTipoParcela
            });
        }


        [HttpPost]
        public ActionResult GetFormularioTemporal(SeleccionFormularioModel model, bool esEdicion = false)
        {
            using (var result = cliente.GetAsync($"api/DeclaracionJurada/GetVersion?idVersion={model.IdVersion}").Result)
            {
                result.EnsureSuccessStatusCode();
                var versionSeleccionada = result.Content.ReadAsAsync<DDJJVersion>().Result;
                ViewData["EsPrescripcion"] = EsParcelaPrescripcion(model.IdUnidadTributaria, model.IdClaseParcela);
                ViewData["EsTemporal"] = EsTemporal = true;
                ViewData["EsEdicion"] = esEdicion;

                if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.U)
                {
                    var form = JsonConvert.DeserializeObject<FormularioUModel>(model.FormularioSeleccionado, new JsonSerializerSettings() { DateFormatString = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern });
                    form.ClasesSeleccionada = JsonConvert.DeserializeObject<List<GeoSit.Web.Api.Models.ClaseParcela>>(form.ClasesJsonSerialized);
                    form.ClasesDisponibles = this.GetClasesDisponibles();
                    form.TiposMedidaLineal = this.GetTiposMedidaLineal();
                    form.Departamentos = GetSelectListObjeto(TipoObjetoAdministrativoEnum.Departamento);
                    form.UOtrasCar = GetUOtrasCar(model.IdVersion);
                    form.Localidades = new List<SelectListItem>();
                    form.Calles = new List<SelectListItem>();
                    form.Clases = new List<SelectListItem>();

                    if ((form.DDJJU.SuperficiePlano.HasValue || form.DDJJU.SuperficieTitulo.HasValue))
                    {
                        var superficie = form.DDJJU.SuperficiePlano.HasValue ? form.DDJJU.SuperficiePlano.Value : form.DDJJU.SuperficieTitulo.Value;
                        form.ClasesDisponiblesEdicion = GetClasesParcelasBySuperficieEdicion(superficie);
                    }
                    if (form.DDJJU.IdMensura.HasValue)
                    {
                        var mens = GetMensuraByIdDDJJ(model.IdDeclaracionJurada).Descripcion;
                        form.Mensura = mens;
                    }
                    if (form.DDJJDesignacion.IdDepartamento.HasValue)
                    {
                        form.Localidades = GetLocalidadesSelect(form.DDJJDesignacion.IdDepartamento.Value);
                    }
                    return PartialView("_FormularioU", form);
                }
                else if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.SoR)
                {
                    var form = JsonConvert.DeserializeObject<FormularioSoRModel>(model.FormularioSeleccionado, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    form.SorOtrasCar = GetSorOtrasCar(model.IdVersion);
                    form.Departamentos = GetSelectListObjeto(TipoObjetoAdministrativoEnum.Departamento);
                    form.Caminos = GetCaminosSelect();
                    form.Localidades = new List<SelectListItem>();
                    form.AllLocalidades = GetAllLocalidadesSelect();
                    //form.IdClaseParcela = model.IdClaseParcela;

                    if (form.DDJJSor.IdMensura.HasValue)
                    {
                        var mens = GetMensuraByIdDDJJ(model.IdDeclaracionJurada).Descripcion;
                        form.Mensura = mens;
                    }
                    if (form.DDJJDesignacion.IdDepartamento.HasValue)
                    {
                        form.Localidades = GetLocalidadesSelect(form.DDJJDesignacion.IdDepartamento.Value);
                    }

                    using (var respAptitudes = cliente.GetAsync($"api/DeclaracionJurada/GetAptitudes?idVersion={versionSeleccionada.IdVersion}").Result)
                    using (var respCaracteristicas = cliente.GetAsync("api/DeclaracionJurada/GetCaracteristicas").Result)
                    {
                        respAptitudes.EnsureSuccessStatusCode();
                        respCaracteristicas.EnsureSuccessStatusCode();

                        var aptitudes = respAptitudes.Content.ReadAsAsync<IEnumerable<VALAptitudes>>().Result;
                        foreach (var item in form.AptitudesDisponibles)
                        {
                            var apt = aptitudes.Single(a => a.IdAptitud == item.IdAptitud);
                            item.Numero = apt.Numero;
                            item.Descripcion = apt.Descripcion;
                            item.InputType = GetInputType(apt);
                        }
                        GetValuesToDropDowns(form, aptitudes, respCaracteristicas.Content.ReadAsAsync<IEnumerable<DDJJSorCaracteristicas>>().Result);
                    }

                    return PartialView("_FormularioSoR", form);
                }
                else if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.E1 || versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.E2)
                {
                    T getForm<T>() where T : FormularioE_BaseModel
                    {
                        var formBase = JsonConvert.DeserializeObject<T>(model.FormularioSeleccionado);
                        formBase.DestinoEdificio = GetDestinosMejoras(model.IdVersion);
                        formBase.EstadosConservacion = GetEstadosConservacion();
                        formBase.Caracteristicas = GetInmCaracteristicas(model.IdVersion);
                        formBase.Incisos = GetInmIncisos(model.IdVersion);
                        formBase.TipoCaracteristicas = GetInmTipoCaracteristicas(model.IdVersion);
                        formBase.MejoraCaracteristicas = (formBase.CaracteristicasToSave ?? new List<long>()).Select(cs => new INMMejoraCaracteristica { IdCaracteristica = cs, IdMejora = formBase.Mejora.IdMejora }).ToList();
                        formBase.OtrasCar = GetInmOtrasCaracteristicas(model.IdVersion).Select(otraCar =>
                                                                                                    {
                                                                                                        var m = formBase.OtrasCar.SingleOrDefault(x => x.IdOtraCar == otraCar.IdOtraCar);

                                                                                                        return new MejoraOtrasCar()
                                                                                                        {
                                                                                                            IdOtraCar = otraCar.IdOtraCar,
                                                                                                            Descripcion = otraCar.Descripcion,
                                                                                                            Valor = m?.Valor,
                                                                                                            Orden = otraCar.Orden,
                                                                                                            Requerido = otraCar.Requerido ?? false,
                                                                                                            EsReal = otraCar.EsReal
                                                                                                        };
                                                                                                    }).OrderByDescending(x => x.Orden.HasValue)
                                                                                                      .ThenBy(x => x.Orden)
                                                                                                      .ThenBy(x => x.Descripcion)
                                                                                                      .ToList();
                        return formBase;
                    }
                    if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.E1)
                    {
                        return PartialView("_FormularioE1", getForm<FormularioE1Model>());
                    }
                    return PartialView("_FormularioE2", getForm<FormularioE2Model>());
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult GetFormulario(SeleccionFormularioModel model, bool esEdicion = false)
        {
            if (model.IdVersion == 0)
            {
                model.IdVersion = int.Parse(model.FormularioSeleccionado);
            }
            var result = cliente.GetAsync($"api/DeclaracionJurada/GetVersion?idVersion={model.IdVersion}").Result;
            result.EnsureSuccessStatusCode();
            var versionSeleccionada = result.Content.ReadAsAsync<DDJJVersion>().Result;

            ViewData["EsPrescripcion"] = EsParcelaPrescripcion(model.IdUnidadTributaria, model.IdClaseParcela);
            ViewData["EsTemporal"] = EsTemporal;
            ViewData["EsEdicion"] = esEdicion;

            var nuevaDDJJ = new DDJJ { IdVersion = model.IdVersion, IdUnidadTributaria = model.IdUnidadTributaria, IdPoligono = model.Poligono };

            if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.U)
            {
                var formularioModel = new FormularioUModel()
                {
                    DDJJ = nuevaDDJJ,
                    PartidaInmobiliaria = model.PartidaInmobiliaria,
                    IdClaseParcela = model.IdClaseParcela,
                    ReadOnly = model.ReadOnly,
                    ClasesDisponibles = GetClasesDisponibles(),
                    TiposMedidaLineal = GetTiposMedidaLineal(),
                    Departamentos = GetSelectListObjeto(TipoObjetoAdministrativoEnum.Departamento),
                    UOtrasCar = GetUOtrasCar(model.IdVersion),
                    Localidades = new List<SelectListItem>(),
                    Calles = new List<SelectListItem>(),
                    Clases = new List<SelectListItem>(),
                };

                if (model.IdDeclaracionJurada > 0)
                {
                    var ddjj = GetDeclaracionJurada(model.IdDeclaracionJurada);
                    formularioModel.DDJJ = ddjj;
                    formularioModel.DDJJU = ddjj.U.SingleOrDefault() ?? new DDJJU();
                    formularioModel.DDJJDesignacion = GetDDJJDesignacion(model.IdDeclaracionJurada) ?? new DDJJDesignacion();
                    formularioModel.Tramite = GetTramite(model.IdDeclaracionJurada) ?? new METramite();
                    formularioModel.SuperficiePlano = ddjj.U.Select(sp => sp.SuperficiePlano).FirstOrDefault();
                    formularioModel.SuperficieTitulo = ddjj.U.Select(sp => sp.SuperficieTitulo).FirstOrDefault();
                    decimal superficie = formularioModel.SuperficiePlano ?? formularioModel.SuperficieTitulo ?? 0m;
                    formularioModel.ClasesDisponiblesEdicion = GetClasesParcelasBySuperficieEdicion(superficie);
                    if (formularioModel.DDJJU.IdMensura.HasValue)
                    {
                        var mens = GetMensuraByIdDDJJ(model.IdDeclaracionJurada).Descripcion;
                        formularioModel.Mensura = mens;
                    }

                    formularioModel.ClasesSeleccionada = this.GetMedidaLineasFromFraccionByIdUPrepare(formularioModel.DDJJU.IdU);
                }
                else
                {
                    int? idLoc = null;
                    var Designacion = GetDesignacionByUt(model.IdUnidadTributaria);
                    if (model.IdLocalidad.HasValue && model.PartidaInmobiliaria == null)
                    {
                        idLoc = formularioModel.DDJJDesignacion.IdLocalidad = model.IdLocalidad;
                    }
                    if (Designacion != null)
                    {
                        if (model.PartidaInmobiliaria != null && Designacion.IdLocalidad.HasValue)
                        {
                            idLoc = formularioModel.DDJJDesignacion.IdLocalidad = (int?)Designacion.IdLocalidad;
                        }
                    }
                    if (idLoc != null)
                    {
                        formularioModel.DDJJDesignacion.IdDepartamento = (int)GetObjetoPadre(idLoc ?? -1)?.ObjetoPadreId;
                    }
                    else if (idLoc == null && model.PartidaInmobiliaria != null)
                    {
                        var codigo = model.PartidaInmobiliaria.Substring(0, 2);
                        formularioModel.DDJJDesignacion.IdDepartamento = (int)GetIdDepartamentoByCodigo(codigo);
                    }

                    if (EsTemporal && !string.IsNullOrEmpty(model.UnidadesTributariasOrigen))
                    {
                        formularioModel.dominiosJSON = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<long[]>(model.UnidadesTributariasOrigen).SelectMany(ut => GetInscripcionDominioDefault(ut)));
                    }
                    else
                    {
                        formularioModel.dominiosJSON = JsonConvert.SerializeObject(GetInscripcionDominioDefault(model.IdUnidadTributaria));
                    }
                }

                if (formularioModel.DDJJDesignacion.IdDepartamento.HasValue)
                {
                    formularioModel.Localidades = GetLocalidadesSelect(formularioModel.DDJJDesignacion.IdDepartamento.Value);
                }

                return PartialView("_FormularioU", formularioModel);
            }
            else if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.SoR)
            {
                var formularioModel = new FormularioSoRModel()
                {
                    DDJJ = nuevaDDJJ,
                    PartidaInmobiliaria = model.PartidaInmobiliaria,
                    IdClaseParcela = model.IdClaseParcela,
                    ReadOnly = model.ReadOnly,
                    SorOtrasCar = GetSorOtrasCar(model.IdVersion),
                    Departamentos = GetSelectListObjeto(TipoObjetoAdministrativoEnum.Departamento),
                    Caminos = GetCaminosSelect(),
                    Localidades = new List<SelectListItem>(),
                    AllLocalidades = GetAllLocalidadesSelect()
                };

                if (model.IdDeclaracionJurada > 0)
                {
                    var ddjj = GetDeclaracionJurada(model.IdDeclaracionJurada);
                    formularioModel.DDJJ = ddjj;
                    formularioModel.DDJJSor = ddjj.Sor.SingleOrDefault() ?? new DDJJSor();
                    formularioModel.DDJJDesignacion = GetDDJJDesignacion(model.IdDeclaracionJurada) ?? new DDJJDesignacion();
                    formularioModel.Tramite = GetTramite(model.IdDeclaracionJurada) ?? new METramite();
                    if (formularioModel.DDJJSor.IdMensura.HasValue)
                    {
                        var mens = GetMensuraByIdDDJJ(model.IdDeclaracionJurada).Descripcion;
                        formularioModel.Mensura = mens;
                    }
                }
                else
                {
                    int? idLoc = null;
                    var Designacion = GetDesignacionByUt(model.IdUnidadTributaria);
                    if (model.IdLocalidad.HasValue && model.PartidaInmobiliaria == null)
                    {
                        idLoc = formularioModel.DDJJDesignacion.IdLocalidad = model.IdLocalidad;
                    }
                    if (Designacion != null)
                    {
                        if (model.PartidaInmobiliaria != null && Designacion.IdLocalidad.HasValue)
                        {
                            idLoc = formularioModel.DDJJDesignacion.IdLocalidad = (int?)Designacion.IdLocalidad;
                        }
                    }
                    if (idLoc != null)
                    {
                        formularioModel.DDJJDesignacion.IdDepartamento = (int)GetObjetoPadre(idLoc ?? -1)?.ObjetoPadreId;
                    }
                    else if (idLoc == null && model.PartidaInmobiliaria != null)
                    {
                        var codigo = model.PartidaInmobiliaria.Substring(0, 2);
                        formularioModel.DDJJDesignacion.IdDepartamento = (int)GetIdDepartamentoByCodigo(codigo);
                    }
                    //formularioModel.DDJJDesignacion.IdDepartamento = (int)GetObjetoPadre(idLoc ?? -1)?.ObjetoPadreId;
                    if (EsTemporal && !string.IsNullOrEmpty(model.UnidadesTributariasOrigen))
                    {
                        formularioModel.dominiosJSON = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<long[]>(model.UnidadesTributariasOrigen).SelectMany(ut => GetInscripcionDominioDefault(ut)));
                    }
                    else
                    {
                        formularioModel.dominiosJSON = JsonConvert.SerializeObject(GetInscripcionDominioDefault(model.IdUnidadTributaria));
                    }
                }
                // Obtenemos las aptitudes disponibles.
                IEnumerable<VALAptitudes> aptitudes;
                using (var resultAptitudes = cliente.GetAsync("api/DeclaracionJurada/GetAptitudes?idVersion=" + model.IdVersion).Result)
                {
                    resultAptitudes.EnsureSuccessStatusCode();
                    aptitudes = resultAptitudes.Content.ReadAsAsync<IEnumerable<VALAptitudes>>().Result;

                    // Se cargan los inputs disponibles con sus respectivos datos seleccionados si es un edit.
                    formularioModel.AptitudesDisponibles = GetAptitudesInputs(aptitudes, formularioModel.DDJJSor);
                }

                // Obtenemos las caracteristicas dispoibles para cada tipo.
                using (var resultCaracteristicas = cliente.GetAsync("api/DeclaracionJurada/GetCaracteristicas").Result)
                {
                    resultCaracteristicas.EnsureSuccessStatusCode();
                    var caracteristicas = resultCaracteristicas.Content.ReadAsAsync<IEnumerable<DDJJSorCaracteristicas>>().Result;
                    GetValuesToDropDowns(formularioModel, aptitudes, caracteristicas);
                }

                if (formularioModel.DDJJDesignacion.IdDepartamento.HasValue)
                {
                    formularioModel.Localidades = GetLocalidadesSelect(formularioModel.DDJJDesignacion.IdDepartamento.Value);
                }

                return PartialView("_FormularioSoR", formularioModel);
            }
            else if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.E1)
            {
                var formularioModel = new FormularioE1Model()
                {
                    DDJJ = nuevaDDJJ,
                    PartidaInmobiliaria = model.PartidaInmobiliaria,
                    ReadOnly = model.ReadOnly,
                    DestinoEdificio = GetDestinosMejoras(model.IdVersion),
                    CaracteristicasSeleccionadas = new List<CaracteristicaValue>(),
                    EstadosConservacion = GetEstadosConservacion(),
                    Caracteristicas = GetInmCaracteristicas(model.IdVersion),
                    Incisos = GetInmIncisos(model.IdVersion),
                    TipoCaracteristicas = GetInmTipoCaracteristicas(model.IdVersion)
                };

                if (model.IdDeclaracionJurada > 0)
                {
                    formularioModel.DDJJ = GetDeclaracionJurada(model.IdDeclaracionJurada);
                    formularioModel.Mejora = GetMejora(model.IdDeclaracionJurada) ?? new INMMejora();
                    formularioModel.Tramite = GetTramite(model.IdDeclaracionJurada) ?? new METramite();

                    if (formularioModel.Mejora != null)
                    {
                        formularioModel.MejoraCaracteristicas = GetInmMejorasCaracteristicas(formularioModel.Mejora.IdMejora);
                    }

                }

                var mejoraOtraCar = GetMejoraOtraCar(formularioModel.Mejora.IdMejora);

                formularioModel.OtrasCar = GetInmOtrasCaracteristicas(model.IdVersion).Select(otraCar =>
                {
                    var m = mejoraOtraCar.FirstOrDefault(x => x.IdOtraCar == otraCar.IdOtraCar);

                    return new MejoraOtrasCar()
                    {
                        IdOtraCar = otraCar.IdOtraCar,
                        Descripcion = otraCar.Descripcion,
                        Valor = m?.Valor,
                        Orden = otraCar.Orden,
                        Requerido = otraCar.Requerido ?? false,
                        EsReal = otraCar.EsReal
                    };
                }).OrderByDescending(x => x.Orden.HasValue)
                  .ThenBy(x => x.Orden)
                  .ThenBy(x => x.Descripcion)
                  .ToList();

                return PartialView("_FormularioE1", formularioModel);
            }
            else if (versionSeleccionada.TipoDeclaracionJurada == FormularioEnum.E2)
            {
                var formularioModel = new FormularioE2Model()
                {
                    DDJJ = nuevaDDJJ,
                    PartidaInmobiliaria = model.PartidaInmobiliaria,
                    ReadOnly = model.ReadOnly,
                    DestinoEdificio = GetDestinosMejoras(model.IdVersion),
                    CaracteristicasSeleccionadas = new List<CaracteristicaValue>(),
                    EstadosConservacion = GetEstadosConservacion(),
                    Caracteristicas = GetInmCaracteristicas(model.IdVersion),
                    Incisos = GetInmIncisos(model.IdVersion),
                    TipoCaracteristicas = GetInmTipoCaracteristicas(model.IdVersion)
                };

                if (model.IdDeclaracionJurada > 0)
                {
                    formularioModel.DDJJ = GetDeclaracionJurada(model.IdDeclaracionJurada);
                    formularioModel.Mejora = GetMejora(model.IdDeclaracionJurada) ?? new INMMejora();
                    formularioModel.Tramite = GetTramite(model.IdDeclaracionJurada) ?? new METramite();

                    if (formularioModel.Mejora != null)
                    {
                        formularioModel.MejoraCaracteristicas = GetInmMejorasCaracteristicas(formularioModel.Mejora.IdMejora);
                    }
                }

                var mejoraOtraCar = GetMejoraOtraCar(formularioModel.Mejora.IdMejora);

                formularioModel.OtrasCar = GetInmOtrasCaracteristicas(model.IdVersion).Select(otraCar =>
                {
                    var m = mejoraOtraCar.FirstOrDefault(x => x.IdOtraCar == otraCar.IdOtraCar);

                    return new MejoraOtrasCar()
                    {
                        IdOtraCar = otraCar.IdOtraCar,
                        Descripcion = otraCar.Descripcion,
                        Valor = m?.Valor,
                        Orden = otraCar.Orden,
                        Requerido = otraCar.Requerido ?? false,
                        EsReal = otraCar.EsReal
                    };
                }).OrderByDescending(x => x.Orden.HasValue)
                  .ThenBy(x => x.Orden)
                  .ThenBy(x => x.Descripcion)
                  .ToList();

                return PartialView("_FormularioE2", formularioModel);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult ValidarConsistencia(long id, long version)
        {
            using (var response = cliente.GetAsync($"api/declaracionjurada/{version}/unidadtributaria/{id}/inconsistencias").Result.EnsureSuccessStatusCode())
            {
                var data = response.Content.ReadAsAsync<ValueTuple<bool, string>>().Result;
                if (EsTemporal)
                {
                    return Json(new { error = data.Item1 || !string.IsNullOrEmpty(data.Item2), message = data.Item2 }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { error = data.Item1, message = data.Item2 }, JsonRequestBehavior.AllowGet);
            }
        }

        private void GetValuesToDropDowns(FormularioSoRModel formularioModel, IEnumerable<VALAptitudes> aptitudes, IEnumerable<DDJJSorCaracteristicas> caracteristicas)
        {
            formularioModel.Relieves = this.GetCaracteristicasPorTipo(TiposCaracteristicas.Relieve, aptitudes, caracteristicas);
            formularioModel.EspesoresCapaArable = this.GetCaracteristicasPorTipo(TiposCaracteristicas.EspesorCapaArable, aptitudes, caracteristicas);
            formularioModel.ColoresTierra = this.GetCaracteristicasPorTipo(TiposCaracteristicas.ColorDeLaTierra, aptitudes, caracteristicas);
            formularioModel.AguasDelSubsuelo = this.GetCaracteristicasPorTipo(TiposCaracteristicas.AguaEnSubsuelo, aptitudes, caracteristicas);
            formularioModel.CapacidadesGanaderas = this.GetCaracteristicasPorTipo(TiposCaracteristicas.CapacidadGanadera, aptitudes, caracteristicas);
            formularioModel.EstadosMonte = this.GetCaracteristicasPorTipo(TiposCaracteristicas.CalidadMonte, aptitudes, caracteristicas);
        }

        private List<SelectListItem> GetCaracteristicasPorTipo(TiposCaracteristicas tc, IEnumerable<VALAptitudes> aptitudes, IEnumerable<DDJJSorCaracteristicas> caracteristicas)
        {

            // Seleccionamos cualquiera de las aptitudes de la versión seleccionada que tenga los tipos de caracteristicas necesaria.
            VALAptitudes apt;
            if (tc == TiposCaracteristicas.CalidadMonte)
            {
                apt = aptitudes.FirstOrDefault(w => (AptitudType)w.IdAptitud == AptitudType.Monte);
            }
            else
            {
                var tipos = new[] { AptitudType.Alta, AptitudType.Baja, AptitudType.MedianamenteAlta, AptitudType.MuyBaja, AptitudType.Anegadiza };
                apt = aptitudes.FirstOrDefault(w => tipos.Contains((AptitudType)w.IdAptitud));
            }

            return caracteristicas
                    .Where(c => (TiposCaracteristicas)c.IdSorTipoCaracteristica == tc && apt.AptCar.Any(w => w.IdSorCar == c.IdSorCaracteristica))
                    .OrderBy(s => s.Numero)
                    .Select(s => new SelectListItem() { Text = $"{s.Numero} - {s.Descripcion}", Value = s.IdSorCaracteristica.ToString() })
                    .ToList();

        }

        private List<VALAptitudInput> GetAptitudesInputs(IEnumerable<VALAptitudes> aptitudes, DDJJSor sor)
        {
            string GetValorSeleccionado(List<DDJJSorCar> sorCar, long idAptitud, TiposCaracteristicas caracteristica)
            {
                DDJJSorCar sorcarSeleccionado = sorCar.FirstOrDefault(x => x.AptCar.IdAptitud == idAptitud && x.AptCar.SorCaracteristica.IdSorTipoCaracteristica == (int)caracteristica);
                if (sorcarSeleccionado != null)
                    return sorcarSeleccionado.AptCar.IdSorCar.ToString();
                else
                    return "";
            }

            if (aptitudes.Count() == 0)
            {
                throw new Exception("No existen Aptitudes para la versión solicitada, por favor contacte a un administrador");
            }

            List<VALAptitudInput> list = new List<VALAptitudInput>();

            List<DDJJSorCar> sorcar = GetSorCar(sor.IdSor);
            List<VALSuperficies> superficies = GetValSuperficies(sor.IdSor);

            foreach (VALAptitudes i in aptitudes)
            {
                var superficie = superficies.FirstOrDefault(x => x.IdAptitud == i.IdAptitud);

                list.Add(new VALAptitudInput()
                {
                    IdAptitud = i.IdAptitud,
                    Numero = i.Numero,
                    Descripcion = i.Descripcion,
                    InputType = this.GetInputType(i),
                    Superficie = (superficie?.Superficie ?? 0d).ToString("0.0000"),
                    RelieveSeleccionado = GetValorSeleccionado(sorcar, i.IdAptitud, TiposCaracteristicas.Relieve),
                    AguasDelSubsueloSeleccionado = GetValorSeleccionado(sorcar, i.IdAptitud, TiposCaracteristicas.AguaEnSubsuelo),
                    CapacidadesGanaderasSeleccionado = GetValorSeleccionado(sorcar, i.IdAptitud, TiposCaracteristicas.CapacidadGanadera),
                    ColoresTierraSeleccionado = GetValorSeleccionado(sorcar, i.IdAptitud, TiposCaracteristicas.ColorDeLaTierra),
                    EspesoresCapaArableSeleccionado = GetValorSeleccionado(sorcar, i.IdAptitud, TiposCaracteristicas.EspesorCapaArable),
                    EstadosMonteSeleccionado = GetValorSeleccionado(sorcar, i.IdAptitud, TiposCaracteristicas.CalidadMonte)
                });
            }

            return list.OrderBy(x => x.IdAptitud).ToList();
        }

        private AptitudTypeInput GetInputType(VALAptitudes i)
        {
            if (i.IdAptitud >= (int)AptitudType.Alta && i.IdAptitud <= (int)AptitudType.Anegadiza)
            {
                return AptitudTypeInput.AllDropDowns;
            }
            else if (i.IdAptitud == (int)AptitudType.Monte)
            {
                return AptitudTypeInput.OnlyDropEstado;

            }
            else // El resto serìa para mostrar únicamente superficie
            {
                return AptitudTypeInput.OnlySuperficie;
            }

        }

        public JsonResult GetInscripcionDominio(long idDeclaracionJurada)
        {
            using (var response = cliente.GetAsync($"api/declaracionjurada/GetDominios?idDeclaracionJurada={idDeclaracionJurada}").Result)
            {
                response.EnsureSuccessStatusCode();
                var dominios = response.Content.ReadAsAsync<IEnumerable<DDJJDominio>>().Result;

                string result = JsonConvert.SerializeObject(dominios, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        private DDJJDominio[] GetInscripcionDominioDefault(long idUnidadTributaria)
        {
            using (var response = cliente.GetAsync($"api/declaracionjurada/Dominios/{idUnidadTributaria}/Default").Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IEnumerable<DDJJDominio>>().Result.ToArray();
            }
        }

        public ActionResult EditarInscripcionDominio(InscripcionDominioModel model, long idUnidadTributaria, long? idClaseParcela)
        {
            using (var result = cliente.GetAsync("api/tipoinscripcion/get").Result.EnsureSuccessStatusCode())
            {
                var tipos = result.Content.ReadAsAsync<IEnumerable<TipoInscripcion>>().Result;
                model = model ?? new InscripcionDominioModel();
                var tiposFiltrados = tipos.FilterTiposIncripcion(EsParcelaPrescripcion(idUnidadTributaria, idClaseParcela));
                if (model.IdTipoInscripcion == 0)
                {
                    model.IdTipoInscripcion = tiposFiltrados.FirstOrDefault()?.TipoInscripcionID ?? 0;
                }
                else if (tiposFiltrados.All(t => t.TipoInscripcionID != model.IdTipoInscripcion))
                {
                    tiposFiltrados = tiposFiltrados.Concat(tipos.Where(t => t.TipoInscripcionID == model.IdTipoInscripcion));
                }
                model.TipoInscripciones = tiposFiltrados;

                model.FechaHora = (string.IsNullOrEmpty(model.Fecha) ? DateTime.Now : DateTime.Parse(model.Fecha)).ToShortDateString();
                return PartialView("Edits/_EditarInscripcionDominio", model);
            }
        }

        public bool EsParcelaPrescripcion(long idUnidadTributaria, long? idClaseParcela)
        {
            if (!idClaseParcela.HasValue)
            {
                using (var resp = cliente.GetAsync($"api/UnidadTributaria/Get?id={idUnidadTributaria}").Result.EnsureSuccessStatusCode())
                {
                    idClaseParcela = resp.Content.ReadAsAsync<UnidadTributaria>().Result.Parcela.ClaseParcelaID;
                }
            }
            return new Parcela { ClaseParcelaID = idClaseParcela.GetValueOrDefault() }.IsPrescripcion();
        }

        public ActionResult EditarPropietario(PropietariosModel model, long idUnidadTributaria, long? idClaseParcela)
        {
            using (var result = cliente.GetAsync("api/declaracionjurada/GetTiposTitularidad").Result.EnsureSuccessStatusCode())
            {
                var tipos = result.Content.ReadAsAsync<IEnumerable<TipoTitularidad>>().Result;
                model = model ?? new PropietariosModel();
                bool esPrescripcion = EsParcelaPrescripcion(idUnidadTributaria, idClaseParcela);
                var tiposFiltrados = tipos.FilterTiposTitularidad(esPrescripcion);

                if (model.IdTipoTitularidad == 0)
                {
                    model.IdTipoTitularidad = tiposFiltrados.FirstOrDefault()?.IdTipoTitularidad ?? 0;
                }
                else if (tiposFiltrados.All(t => t.IdTipoTitularidad != model.IdTipoTitularidad))
                {
                    tiposFiltrados = tiposFiltrados.Concat(tipos.Where(t => t.IdTipoTitularidad == model.IdTipoTitularidad));
                }
                ViewData["EsPrescripcion"] = esPrescripcion;
                model.TiposTitularidadList = new SelectList(tiposFiltrados, "IdTipoTitularidad", "Descripcion");
                return PartialView("Edits/_EditarPropietario", model);
            }
        }

        public ActionResult GetDomicilios(int personaId)
        {
            List<DomiciliosModel> model = new List<DomiciliosModel>();
            model.Add(new DomiciliosModel() { Id = 1, PersonaId = 1, Tipo = "Real", Provincia = "Corrientes", Localidad = "Capital", Barrio = "Centro", Calle = "Junin", Altura = "123", Piso = "-", Departamento = "-", CodigoPostal = "3400" });
            model.Add(new DomiciliosModel() { Id = 2, PersonaId = 2, Tipo = "Fiscal", Provincia = "Corrientes", Localidad = "Capital", Barrio = "La Olla", Calle = "Medrano", Altura = "5422", Piso = "1", Departamento = "3", CodigoPostal = "3400" });


            return Json(model.Where(x => x.PersonaId == personaId).ToList(), JsonRequestBehavior.AllowGet);
        }

        private List<GeoSit.Web.Api.Models.ClaseParcela> GetClasesDisponibles()
        {
            using (var result = cliente.GetAsync("api/declaracionjurada/GetClasesDisponiblesDepthRel").Result)
            {
                result.EnsureSuccessStatusCode();
                var list = result.Content.ReadAsAsync<IEnumerable<GeoSit.Web.Api.Models.ClaseParcela>>().Result.ToList();
                foreach (var l in list)
                {
                    l.Descripcion = $"{l.IdClaseParcela} - {l.Descripcion}";
                }
                return list;
            }

        }

        public List<GeoSit.Web.Api.Models.ClaseParcela> GetMedidaLineasFromFraccionByIdUPrepare(long idDDJJU)
        {
            using (var result = cliente.GetAsync($"api/declaracionjurada/GetMedidaLineasFromFraccionByIdUPrepare?idDeclaracionJuradaU={idDDJJU}").Result)
            {
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsAsync<IEnumerable<GeoSit.Web.Api.Models.ClaseParcela>>().Result.ToList();
            }
        }
        private List<TipoMedidaLineal> GetTiposMedidaLineal()
        {
            using (var result = cliente.GetAsync("api/declaracionjurada/GetTipoMedidaLineales").Result)
            {
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsAsync<IEnumerable<TipoMedidaLineal>>().Result.ToList();
            }
        }

        public ActionResult EditarDomicilio(int? domicilioId)
        {
            if (domicilioId.HasValue)
            {
                List<DomiciliosModel> model = new List<DomiciliosModel>();
                model.Add(new DomiciliosModel() { Id = 1, PersonaId = 1, Tipo = "Real", Provincia = "Corrientes", Localidad = "Capital", Barrio = "Centro", Calle = "Junin", Altura = "123", Piso = "-", Departamento = "-", CodigoPostal = "3400" });
                model.Add(new DomiciliosModel() { Id = 2, PersonaId = 2, Tipo = "Fiscal", Provincia = "Corrientes", Localidad = "Capital", Barrio = "La Olla", Calle = "Medrano", Altura = "5422", Piso = "1", Departamento = "3", CodigoPostal = "3400" });

                return PartialView("Edits/_EditarDomicilio", model.FirstOrDefault(x => x.Id == domicilioId.Value));
            }
            else
            {
                return PartialView("Edits/_EditarDomicilio", new DomiciliosModel());
            }
        }

        public ActionResult ActualizacionDecreto()
        {
            var model = new ActualizacionDecretoModel()
            {
                IsRunning = GetAplicarDecretoIsRunning(),
                DecretosList = GetDecretos().OrderBy(d => d.NroDecreto)
                                           .Select(d => new SelectListItem
                                           {
                                               Disabled = d.Aplicado == 1,
                                               Text = d.NroDecreto.ToString(),
                                               Value = d.IdDecreto.ToString()
                                           }).ToList()
            };
            return PartialView("Valuacion/_ActualizacionDecreto", model);
        }

        public ActionResult Revaluacion()
        {
            return PartialView("Valuacion/_DatosRevaluacion");
        }

        [HttpPost]
        public ActionResult GuardarFormularioSoR(FormularioSoRModel model)
        {
            try
            {
                long idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;

                List<DDJJDominio> dominios = JsonConvert.DeserializeObject<List<DDJJDominio>>(model.dominiosJSON, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                List<VALAptCar> aptCar = GetAptCar();
                List<DDJJSorCar> sorcar = new List<DDJJSorCar>();
                List<VALSuperficies> superficies = new List<VALSuperficies>();

                foreach (var aptitud in model.AptitudesDisponibles)
                {
                    AddSorCar(aptCar, sorcar, model.DDJJSor.IdSor, aptitud.AguasDelSubsueloSeleccionado, aptitud.IdAptitud);
                    AddSorCar(aptCar, sorcar, model.DDJJSor.IdSor, aptitud.CapacidadesGanaderasSeleccionado, aptitud.IdAptitud);
                    AddSorCar(aptCar, sorcar, model.DDJJSor.IdSor, aptitud.ColoresTierraSeleccionado, aptitud.IdAptitud);
                    AddSorCar(aptCar, sorcar, model.DDJJSor.IdSor, aptitud.EspesoresCapaArableSeleccionado, aptitud.IdAptitud);
                    AddSorCar(aptCar, sorcar, model.DDJJSor.IdSor, aptitud.EstadosMonteSeleccionado, aptitud.IdAptitud);
                    AddSorCar(aptCar, sorcar, model.DDJJSor.IdSor, aptitud.RelieveSeleccionado, aptitud.IdAptitud);

                    if (!string.IsNullOrEmpty(aptitud.Superficie))
                    {
                        if (double.TryParse(aptitud.Superficie, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double superficie))
                            superficies.Add(new VALSuperficies() { IdAptitud = aptitud.IdAptitud, Superficie = superficie, IdSor = model.DDJJSor.IdSor });
                        else
                            superficies.Add(new VALSuperficies() { IdAptitud = aptitud.IdAptitud, Superficie = 0, IdSor = model.DDJJSor.IdSor });
                    }
                }

                var postModel = new
                {
                    ddjj = model.DDJJ,
                    ddjjSor = model.DDJJSor,
                    ddjjDesignacion = model.DDJJDesignacion,
                    dominios,
                    sorCar = sorcar,
                    superficies,
                    idUsuario,
                    machineName = AuditoriaHelper.ReverseLookup(Request.UserHostAddress),
                    ip = Request.UserHostAddress
                };
                var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");


                using (var resp = cliente.PostAsync("api/DeclaracionJurada/SaveDDJJSor", content).Result)
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }
                }
            }
            catch (HttpRequestException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            void AddSorCar(List<VALAptCar> aptCar, List<DDJJSorCar> sorcar, long idSor, string selectedValue, long idAptitud)
            {
                if (!string.IsNullOrEmpty(selectedValue))
                {
                    long idAptCar = aptCar.FirstOrDefault(x => x.IdSorCar == long.Parse(selectedValue) && x.IdAptitud == idAptitud).IdAptCar;
                    sorcar.Add(new DDJJSorCar() { IdSor = idSor, IdAptCar = idAptCar });
                }
            }
        }

        [HttpPost]
        public ActionResult GuardarFormularioU(FormularioUModel model)
        {
            try
            {
                long idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;

                List<DDJJDominio> dominios = JsonConvert.DeserializeObject<List<DDJJDominio>>(model.dominiosJSON, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });

                if (!string.IsNullOrEmpty(model.CroquisBase64))
                {
                    model.DDJJU.Croquis = Convert.FromBase64String(model.CroquisBase64.Substring(model.CroquisBase64.IndexOf(",") + 1));
                }

                var postModel = new
                {
                    ddjj = model.DDJJ,
                    ddjjU = model.DDJJU,
                    ddjjDesignacion = model.DDJJDesignacion,
                    dominios,
                    clasesSeleccionada = model.ClasesJsonSerialized,
                    idUsuario,
                    machineName = AuditoriaHelper.ReverseLookup(Request.UserHostAddress),
                    ip = Request.UserHostAddress
                };
                var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");
                using (var resp = cliente.PostAsync("api/DeclaracionJurada/SaveDDJJU", content).Result)
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }
                }
            }
            catch (HttpRequestException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult GuardarFormularioE1(FormularioE1Model model)
        {
            try
            {
                long idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;

                List<INMMejoraOtraCar> otrasCar = new List<INMMejoraOtraCar>();
                foreach (MejoraOtrasCar e in model.OtrasCar.Where(x => !x.Hidden.HasValue || !x.Hidden.Value))
                {
                    otrasCar.Add(new INMMejoraOtraCar() { IdOtraCar = e.IdOtraCar, IdMejora = model.Mejora.IdMejora, Valor = e.Valor });
                }

                var postModel = new
                {
                    ddjj = model.DDJJ,
                    mejora = model.Mejora,
                    otrasCar,
                    caracteristicas = model.CaracteristicasToSave?.Where(x => x != 0).ToList(),
                    idUsuario,
                    machineName = AuditoriaHelper.ReverseLookup(Request.UserHostAddress),
                    ip = Request.UserHostAddress
                };
                var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");


                using (var resp = cliente.PostAsync("api/DeclaracionJurada/SaveFormularioE1", content).Result)
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }
                }
            }
            catch (HttpRequestException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult GuardarFormularioE2(FormularioE2Model model)
        {
            try
            {
                long idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;

                List<INMMejoraOtraCar> otrasCar = new List<INMMejoraOtraCar>();
                foreach (MejoraOtrasCar e in model.OtrasCar.Where(x => !x.Hidden.HasValue || !x.Hidden.Value))
                {
                    otrasCar.Add(new INMMejoraOtraCar() { IdOtraCar = e.IdOtraCar, IdMejora = model.Mejora.IdMejora, Valor = e.Valor });
                }

                var postModel = new
                {
                    ddjj = model.DDJJ,
                    mejora = model.Mejora,
                    otrasCar,
                    caracteristicas = model.CaracteristicasToSave.Where(x => x != 0).ToList(),
                    idUsuario,
                    machineName = AuditoriaHelper.ReverseLookup(Request.UserHostAddress),
                    ip = Request.UserHostAddress
                };
                var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");


                using (var resp = cliente.PostAsync("api/DeclaracionJurada/SaveFormularioE2", content).Result)
                {
                    if (resp.IsSuccessStatusCode)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }
                }
            }
            catch (HttpRequestException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult GetPersonaDomicilios(long idPersona)
        {
            using (var resp = cliente.GetAsync($"api/DeclaracionJurada/GetPersonaDomicilios?idPersona={idPersona}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<IEnumerable<DDJJPersonaDomicilio>>().Result.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        public Designacion GetDesignacionByUt(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync($"api/DeclaracionJurada/GetDesignacionByUt?idUnidadTributaria={idUnidadTributaria}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<Designacion>().Result;
            }
        }

        private List<SelectListItem> GetSelectListObjeto(TipoObjetoAdministrativoEnum tipoObjeto)
        {
            return GetObjetosByTipo(tipoObjeto)
                        .OrderBy(o => o.Nombre)
                        .Select(o => new SelectListItem() { Value = o.FeatId.ToString(), Text = o.Nombre })
                        .ToList();
        }

        public ActionResult ValoresAforoValido()
        {
            using (var result = cliente.GetAsync($"api/Declaracionjurada/ValoresAforoValido").Result)
            {
                if (result.IsSuccessStatusCode)
                {
                    string valores = result.Content.ReadAsStringAsync().Result;
                    return Json(valores, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return new HttpStatusCodeResult(result.StatusCode);
                }
            }
        }

        #region InformeDDJJ
        private ArchivoDescarga ArchivoDescarga
        {
            get { return Session["ArchivoDescarga"] as ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }
        public FileResult AbrirInformeDDJJ()
        {
            Response.AppendHeader("Content-Disposition", new ContentDisposition { FileName = ArchivoDescarga.NombreArchivo, Inline = true }.ToString());
            return File(ArchivoDescarga.Contenido, ArchivoDescarga.MimeType);
        }
        public ActionResult GenerarInformeDDJJ(long idDeclaracionJurada, string tipoDDJJ, long? idTramite)
        {
            if (idDeclaracionJurada > 0)
            {
                string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
                using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
                {
                    string tipo = string.Compare(tipoDDJJ, "sor", true) == 0 ? "DDJJSoR" : string.Compare(tipoDDJJ, "u", true) == 0 ? "DDJJU" : "E1E2";
                    HttpResponseMessage resp = apiReportes.GetAsync($"api/Informe{tipo}/Get?idDeclaracionJurada={idDeclaracionJurada}&idTramite={idTramite ?? 0}&usuario={usuario}").Result;
                    if (!resp.IsSuccessStatusCode)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result), $"Formulario{tipoDDJJ}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        #endregion

        #region Metodos API
        private List<ObjetoAdministrativoModel> GetObjetosByTipo(TipoObjetoAdministrativoEnum tipoObjeto)
        {
            using (var resp = cliente.GetAsync($"api/ObjetoAdministrativoService/GetObjetoByTipo/{(int)tipoObjeto}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<ObjetoAdministrativoModel>>().Result.ToList();
            }
        }

        private ObjetoAdministrativoModel GetObjetoPadre(long idHijo)
        {
            using (var resp = cliente.GetAsync($"api/objetoadministrativoservice/GetObjetoById/{idHijo}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<ObjetoAdministrativoModel>().Result;
            }
        }

        private List<SelectListItem> GetLocalidadesSelect(long idDepartamento)
        {
            using (var resp = cliente.GetAsync($"api/ObjetoAdministrativoService/GetObjetoByIdPadreTipo?id={idDepartamento}&tipo={(int)TipoObjetoAdministrativoEnum.Localidad}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<Via>>().Result
                           .OrderBy(x => x.Nombre)
                           .Select(v => new SelectListItem() { Value = v.FeatId.ToString(), Text = v.Nombre })
                           .ToList();
            }
        }

        [HttpPost]
        public JsonResult GetLocalidadesByDistancia(long distanciaLocalidad)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetLocalidadesByDistancia?distanciaLocalidad=" + distanciaLocalidad).Result)
            {
                resp.EnsureSuccessStatusCode();
                var localidades = resp.Content.ReadAsAsync<List<ObjetoAdministrativoModel>>().Result
                     .OrderBy(x => x.Nombre)
                     .Select(o => new SelectListItem() { Value = o.FeatId.ToString(), Text = o.Nombre })
                     .ToList();
                return Json(localidades);
            }
        }

        [HttpPost]
        public JsonResult GetClasesParcelasBySuperficie(decimal superficie)
        {
            /*using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetClasesParcelasBySuperficie?superficie=" + superficie).Result)
            {
                resp.EnsureSuccessStatusCode();
                var clases = resp.Content.ReadAsAsync<List<VALClasesParcelas>>().Result
                     .Select(o => new SelectListItem() { Value = o.IdClaseParcela.ToString(), Text = o.Descripcion })
                     .ToList();
                return Json(clases);
            }*/
            var clases = GetClasesParcelasBySuperficieEdicion(superficie).Select(o => new SelectListItem() { Value = o.IdClaseParcela.ToString(), Text = o.Descripcion });
            return Json(clases);


        }

        public List<VALClasesParcelas> GetClasesParcelasBySuperficieEdicion(decimal superficie)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetClasesParcelasBySuperficie?superficie=" + superficie).Result)
            {
                resp.EnsureSuccessStatusCode();
                var clases = resp.Content.ReadAsAsync<List<VALClasesParcelas>>().Result.ToList();

                return clases;
            }
        }


        private List<SelectListItem> GetAllLocalidadesSelect()
        {
            using (var resp = cliente.GetAsync($"api/ObjetoAdministrativoService/GetObjetoByTipo?id={(int)TipoObjetoAdministrativoEnum.Localidad}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<ObjetoAdministrativoModel>>().Result
                           .OrderBy(x => x.Nombre)
                           .Select(o => new SelectListItem() { Value = o.FeatId.ToString(), Text = o.Nombre })
                           .ToList();
            }
        }

        private List<SelectListItem> GetCaminosSelect()
        {
            using (var resp = cliente.GetAsync($"api/ViaService/GetViayTipo?id={(int)TipoVia.RUTA}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<Via>>().Result
                           .OrderBy(x => x.Nombre)
                           .Select(v => new SelectListItem() { Value = v.ViaId.ToString(), Text = v.Nombre })
                           .ToList();
            }
        }

        public ActionResult GetLocalidades(long idDepartamento)
        {
            return Json(GetLocalidadesSelect(idDepartamento), JsonRequestBehavior.AllowGet);
        }

        public Mensura GetMensuraByIdDDJJ(long idDDJJ)
        {
            using (var resp = cliente.GetAsync($"api/MensuraService/GetMensuraByIdDDJJ?idDDJJ={idDDJJ}").Result)
            {
                resp.EnsureSuccessStatusCode();

                return resp.Content.ReadAsAsync<Mensura>().Result;
            }
        }

        [HttpPost]
        public string GetCroquisClaseParcela(int id)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetCroquisClaseParcela?idClaseParcela=" + id).Result)
            {
                resp.EnsureSuccessStatusCode();
                var croquis = resp.Content.ReadAsAsync<string>().Result;
                return croquis;
            }
        }

        [HttpGet]
        public object GetClaseParcelaByIdDDJJ(long idDeclaracionJurada)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetClaseParcelaByIdDDJJ?idDeclaracionJurada=" + idDeclaracionJurada).Result)
            {
                resp.EnsureSuccessStatusCode();
                var idClaseParcela = resp.Content.ReadAsAsync<object>().Result;
                return idClaseParcela;
            }
        }

        private List<DDJJSorOtrasCar> GetSorOtrasCar(int idVersion)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetSorOtrasCar?idVersion=" + idVersion).Result;
            resp.EnsureSuccessStatusCode();

            return (List<DDJJSorOtrasCar>)resp.Content.ReadAsAsync<IEnumerable<DDJJSorOtrasCar>>().Result;
        }

        private List<DDJJUOtrasCar> GetUOtrasCar(int idVersion)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetUOtrasCar?idVersion=" + idVersion).Result;
            resp.EnsureSuccessStatusCode();

            return (List<DDJJUOtrasCar>)resp.Content.ReadAsAsync<IEnumerable<DDJJUOtrasCar>>().Result;
        }

        private List<DDJJ> GetDeclaracionesJuradasAPI(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync($"api/DeclaracionJurada/GetDeclaracionesJuradas?idUnidadTributaria={idUnidadTributaria}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<DDJJ>>().Result;
            }
        }

        private List<DDJJ> GetDeclaracionesJuradasE1E2NoVigentesAPI(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync($"api/DeclaracionJurada/GetDeclaracionesJuradasE1E2NoVigentes?idUnidadTributaria={idUnidadTributaria}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<DDJJ>>().Result;
            }
        }

        private List<EstadosConservacion> GetEstadosConservacion()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetEstadosConservacion").Result;
            resp.EnsureSuccessStatusCode();

            return (List<EstadosConservacion>)resp.Content.ReadAsAsync<List<EstadosConservacion>>().Result;
        }

        private List<VALValuacion> GetValuacionesHistoricasAPI(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync($"api/DeclaracionJurada/GetValuacionesHistoricas?idUnidadTributaria={idUnidadTributaria}").Result.EnsureSuccessStatusCode())
            {
                return resp.Content.ReadAsAsync<List<VALValuacion>>().Result;
            }
        }

        private VALValuacion GetValuacionAPI(long idValuacion)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetValuacion?idValuacion=" + idValuacion).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<VALValuacion>().Result;
            }
        }

        private VALValuacion GetValuacionVigenteAPI(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync($"api/DeclaracionJurada/GetValuacionVigente?idUnidadTributaria={idUnidadTributaria}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<VALValuacion>().Result;
            }
        }

        private DDJJ GetDeclaracionJurada(long idDeclaracionJurada)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetDeclaracionJuradaCompleta?idDeclaracionJurada=" + idDeclaracionJurada).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DDJJ>().Result;
            }
        }

        private INMMejora GetMejora(long idDeclaracionJurada)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetMejora?idDeclaracionJurada=" + idDeclaracionJurada).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<INMMejora>().Result;
            }
        }

        private List<INMCaracteristica> GetInmCaracteristicas(long idVersion)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetInmCaracteristicas?idVersion=" + idVersion).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<INMCaracteristica>>().Result;
            }
        }

        private List<INMInciso> GetInmIncisos(long idVersion)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetInmIncisos?idVersion=" + idVersion).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<INMInciso>>().Result;
            }
        }

        private List<INMMejoraCaracteristica> GetInmMejorasCaracteristicas(long idMejora)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetInmMejorasCaracteristicas?idMejora=" + idMejora).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<INMMejoraCaracteristica>>().Result;
            }
        }

        private List<INMTipoCaracteristica> GetInmTipoCaracteristicas(long idVersion)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetInmTipoCaracteristicas?idVersion=" + idVersion).Result;
            resp.EnsureSuccessStatusCode();

            return (List<INMTipoCaracteristica>)resp.Content.ReadAsAsync<List<INMTipoCaracteristica>>().Result;
        }


        private List<INMOtraCaracteristica> GetInmOtrasCaracteristicas(long idVersion)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetInmOtrasCaracteristicas?idVersion=" + idVersion).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<INMOtraCaracteristica>>().Result;
            }
        }

        private List<INMMejoraOtraCar> GetMejoraOtraCar(long idMejora)
        {
            using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetMejoraOtraCar?idMejora=" + idMejora).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<INMMejoraOtraCar>>().Result;
            }
        }

        private DDJJDesignacion GetDDJJDesignacion(long idDeclaracionJurada)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetDDJJDesignacion?idDeclaracionJurada=" + idDeclaracionJurada).Result;
            resp.EnsureSuccessStatusCode();

            return (DDJJDesignacion)resp.Content.ReadAsAsync<DDJJDesignacion>().Result;
        }

        private METramite GetTramite(long idDeclaracionJurada)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetTramite?idDeclaracionJurada=" + idDeclaracionJurada).Result;
            resp.EnsureSuccessStatusCode();

            return (METramite)resp.Content.ReadAsAsync<METramite>().Result;
        }

        private UnidadTributaria GetUnidadTributaria(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync("api/UnidadTributaria/Get/" + idUnidadTributaria).Result.EnsureSuccessStatusCode())
            {
                return resp.Content.ReadAsAsync<UnidadTributaria>().Result;
            }
        }

        private Parcela GetParcelaByUt(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync($"api/Parcela/GetParcelaByUt?idUnidadTributaria={idUnidadTributaria}").Result.EnsureSuccessStatusCode())
            {
                var parcela = resp.Content.ReadAsAsync<Parcela>().Result;
                return parcela;
            }
        }

        private List<VALAptCar> GetAptCar()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetAptCar").Result;
            resp.EnsureSuccessStatusCode();

            return (List<VALAptCar>)resp.Content.ReadAsAsync<List<VALAptCar>>().Result;
        }

        private List<DDJJSorCar> GetSorCar(long idSor)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetSorCar?idSor=" + idSor).Result;
            resp.EnsureSuccessStatusCode();

            return (List<DDJJSorCar>)resp.Content.ReadAsAsync<List<DDJJSorCar>>().Result;
        }

        private List<VALSuperficies> GetValSuperficies(long idSor)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetValSuperficies?idSor=" + idSor).Result;
            resp.EnsureSuccessStatusCode();

            return (List<VALSuperficies>)resp.Content.ReadAsAsync<List<VALSuperficies>>().Result;
        }

        private List<VALDecreto> GetDecretos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetDecretos").Result;
            resp.EnsureSuccessStatusCode();

            return (List<VALDecreto>)resp.Content.ReadAsAsync<List<VALDecreto>>().Result;
        }

        private bool GetAplicarDecretoIsRunning()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetAplicarDecretoIsRunning").Result;
            resp.EnsureSuccessStatusCode();

            return (bool)resp.Content.ReadAsAsync<bool>().Result;
        }

        private string GetAplicarDecretoStatusAPI()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetAplicarDecretoStatus").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<string>().Result;
        }

        public long GetIdDepartamentoByCodigo(string codigo)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetIdDepartamentoByCodigo?codigo=" + codigo).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<long>().Result;
        }

        public ActionResult GetAplicarDecretoStatus()
        {
            try
            {
                return Json(new { success = true, message = GetAplicarDecretoStatusAPI() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AplicarDecreto(long idDecreto)
        {
            try
            {
                var postModel = new
                {
                    idDecreto = idDecreto,
                    idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario
                };

                var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");
                DecretoAplicado da = await GetDecretoAplicado(content);
                return Json(new { success = true, data = da });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<DecretoAplicado> GetDecretoAplicado(StringContent content)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            client.Timeout = Timeout.InfiniteTimeSpan;
            var resp = await client.PostAsync("api/DeclaracionJurada/AplicarDecreto", content);
            return (await resp.Content.ReadAsAsync<DecretoAplicado>());
        }

        [HttpPost]
        public JsonResult Revaluar(long idUnidadTributaria)
        {
            try
            {
                var postModel = new
                {
                    idUnidadTributaria,
                    idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario,
                    machineName = AuditoriaHelper.ReverseLookup(Request.UserHostAddress),
                    ip = Request.UserHostAddress
                };

                var content = new StringContent(JsonConvert.SerializeObject(postModel), Encoding.UTF8, "application/json");

                HttpResponseMessage resp = cliente.PostAsync("api/DeclaracionJurada/Revaluar", content).Result;
                resp.EnsureSuccessStatusCode();
                return Json(new { success = resp.Content.ReadAsAsync<bool>().Result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<SelectListItem> GetDestinosMejoras(long idVersion)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetDestinosMejoras?idVersion=" + idVersion).Result;
            resp.EnsureSuccessStatusCode();

            List<SelectListItem> objetoSelect = new List<SelectListItem>();

            List<INMDestinoMejora> objeto = (List<INMDestinoMejora>)resp.Content.ReadAsAsync<List<INMDestinoMejora>>().Result;

            foreach (INMDestinoMejora o in objeto)
            {
                objetoSelect.Add(new SelectListItem() { Value = o.IdDestinoMejora.ToString(), Text = o.Descripcion });
            }

            return objetoSelect;
        }


        public ActionResult BuscarAforo(long idLocalidad, string calle, long? idVia, int? altura)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/BuscarAforo?idLocalidad=" + idLocalidad + "&calle=" + calle + "&idVia=" + idVia + "&altura=" + altura).Result;
            resp.EnsureSuccessStatusCode();

            Aforo a = resp.Content.ReadAsAsync<Aforo>().Result;

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVersion(long id)
        {
            using (var resp = cliente.GetAsync($"api/DeclaracionJurada/GetVersion?idVersion={id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<DDJJVersion>().Result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult BajaDDJJE1E2(long id)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            var ddjj = new
            {
                idDDJJ = id,
                idUsuario = usuario.Id_Usuario,
                machineName = AuditoriaHelper.ReverseLookup(Request.UserHostAddress),
                ip = Request.UserHostAddress,

            };

            var content = new StringContent(JsonConvert.SerializeObject(ddjj), Encoding.UTF8, "application/json");
            HttpResponseMessage resp = cliente.PostAsync("api/DeclaracionJurada/BajaMejoras", content).Result;
            if (!resp.IsSuccessStatusCode && resp.StatusCode != HttpStatusCode.PreconditionFailed)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return Json(new { ok = resp.IsSuccessStatusCode });
        }

        #endregion Metodos API

    }

    public enum TipoObjetoAdministrativoEnum
    {
        Pais = 1,
        Provincia = 2,
        Departamento = 3,
        Jurisdiccion = 4,
        Municipio = 5,
        Zona = 6,
        Seccion = 7,
        Localidad = 8,
        Faja = 9,
        Radio = 10,
        CentroUrbano = 11,
        Lote = 12,
        Paraje = 13
    }

    public enum TipoObjetoEnum
    {
        Afluentes = 1,
        Rutas = 2,
        Lagunas = 3,
        Reservas = 4,
        RedGeodesicaNivel1 = 5,
        Parajes = 6,
        CurvaDeNivel = 7,
        RedGeodesicaNivel2 = 8,
        RedGeodesicaNivel3 = 9,
        RedGeodesicaNivel4 = 10
    }

    public enum TipoVia
    {
        RUTA = 1,
        SIN_DETERMINAR = 2,
        AUTOPISTA = 3,
        PASAJE = 4,
        DIAGONAL = 5,
        CALLE = 6,
        PASAJE_PEATONAL = 7,
        AVENIDA = 8,
        BOLEVARD = 9,
        AVENIDA_DIAGONAL = 10,
        CALLEJON = 11,
    }

    public static class FormularioEnum
    {
        public static string E1 = "E1 (VIVIENDA/COMERCIO)";
        public static string E2 = "E2 (INDUSTRIA/SERVICIO)";
        public static string SoR = "SOR(SUBRURAL O RURAL)";
        public static string U = "U(URBANA)";
    }


}