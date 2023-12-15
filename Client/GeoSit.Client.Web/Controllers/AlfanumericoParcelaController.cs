using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.WebPages;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.Seguridad;
using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using Resources;
using GeoSit.Client.Web.Helpers;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Text;
using Newtonsoft.Json;

namespace GeoSit.Client.Web.Controllers
{
    public class AlfanumericoParcelaController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();

        private UnidadAlfanumericoParcela UnidadAlfanumericoParcela
        {
            get { return Session["UnidadAlfanumericoParcela"] as UnidadAlfanumericoParcela; }
            set { Session["UnidadAlfanumericoParcela"] = value; }
        }

        private UsuariosModel Usuario
        {
            get { return (UsuariosModel)Session["usuarioPortal"]; }
        }

        public AlfanumericoParcelaController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

       public ActionResult Index()
        {
            UnidadAlfanumericoParcela = new UnidadAlfanumericoParcela();

            var result = _cliente.GetAsync("api/tipoparcelaoperacion/get").Result;
            result.EnsureSuccessStatusCode();
            var OPERACIONES_EXCLUIDAS = new[]
            {
                Convert.ToInt64(TiposParcelaOperacion.CreacionParcelaMunicipal),
                Convert.ToInt64(TiposParcelaOperacion.Otros),
                Convert.ToInt64(TiposParcelaOperacion.ConjuntoInmobiliario),
                Convert.ToInt64(TiposParcelaOperacion.PropiedadHorizontal),
                Convert.ToInt64(TiposParcelaOperacion.Proyecto)
            };
            var tipoParcelaOperaciones = result.Content
                                               .ReadAsAsync<IEnumerable<TipoParcelaOperacion>>().Result
                                               .Where(x => !OPERACIONES_EXCLUIDAS.Contains(x.Id))
                                               .OrderBy(x => x.Descripcion);

            result = _cliente.GetAsync("api/tipoparcela/get").Result;
            result.EnsureSuccessStatusCode();
            var tipoParcelas = result.Content.ReadAsAsync<IEnumerable<TipoParcela>>().Result.OrderBy(x => x.Descripcion);
            long idTipoParcelaDefault = tipoParcelas.Min(x => x.TipoParcelaID);
            Session["TIPO_PARCELA"] = idTipoParcelaDefault;

            result = _cliente.GetAsync("api/claseparcela/get").Result;
            result.EnsureSuccessStatusCode();
            var claseParcelas = result.Content.ReadAsAsync<IEnumerable<ClaseParcela>>().Result.OrderBy(x => x.Descripcion);

            result = _cliente.GetAsync("api/estadoparcela/get").Result;
            result.EnsureSuccessStatusCode();
            var estadoParcelas = result.Content.ReadAsAsync<IEnumerable<EstadoParcela>>().Result.OrderBy(x => x.Descripcion);

            var jurisdicciones = GetAllJurisdicciones().Select(j => new
            {
                FeatId = j.FeatId,
                Descripcion = $"{j.Codigo}-{j.Nombre}"
            })
                .OrderBy(j => j.Descripcion).ToList();
            var jur = GetJurisdiccionByConfig();

            var alfanumerico = new AlfanumericoParcelaViewModel
            {
                TipoOperacionList = new SelectList(tipoParcelaOperaciones, "Id", "Descripcion"),
                TipoOperacionId = tipoParcelaOperaciones.Min(x => x.Id),
                TipoParcelaList = new SelectList(tipoParcelas, "TipoParcelaID", "Descripcion"),
                TipoParcelaId = idTipoParcelaDefault,
                ClaseParcelaList = new SelectList(claseParcelas, "ClaseParcelaID", "Descripcion"),
                ClaseParcelaId = claseParcelas.Min(x => x.ClaseParcelaID),
                EstadoParcelaList = new SelectList(estadoParcelas, "EstadoParcelaID", "Descripcion"),
                EstadoParcelaId = estadoParcelas.First().EstadoParcelaID,
                JurisdiccionList = new SelectList(jurisdicciones, "FeatId", "Descripcion"),
                JurisdiccionId = jur.FeatId
            };

            Session["ID_JURISDICCION"] = alfanumerico.JurisdiccionId;
            ViewBag.Jurisdiccion = Session["CODIGO_JURISDICCION"] = jur.Codigo;

            return PartialView("~/Views/AlfanumericoParcela/Index.cshtml", alfanumerico);
        }

        private OA.Objeto GetJurisdiccionByConfig()
        {
            using (var result = _cliente.GetAsync("api/ObjetoAdministrativoService/GetJurisdiccionByConfiguracion").Result)
            {
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsAsync<OA.Objeto>().Result;
            }
        }

      private IEnumerable<OA.Objeto> GetAllJurisdicciones()
        {
            using (var result = _cliente.GetAsync($"api/ObjetoAdministrativoService/GetObjetoByTipo/{TiposObjetoAdministrativo.JURISDICCION}").Result)
            {
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsAsync<ICollection<OA.Objeto>>().Result.OrderBy(x => x.Nombre);
            }
        }
        public string ManzanaNomenclaturaFormat(string nomenclatura)
        {
            var regex = new Regex("(?<Dep>[0-9]{2})(?<Eji>[0-9]{3})(?<Cir>[0-9]{3})(?<Sec>[0-9]{3})(?<Div>[0-9]{3})");
            return regex.Replace(nomenclatura, "Dep: $1 Eji: $2 Cir: $3 Sec: $4 Div: $5");
        }

        public void OperacionesParcelasDestinosClear()
        {
            UnidadAlfanumericoParcela.OperacionesParcelasDestinos.Clear();
        }

        public JsonResult VerifyNumeroExpediente(string numeroExpediente)
        {
            var result = _cliente.GetAsync("api/expedienteobra/get?numeroExpediente=" + numeroExpediente).Result;
            result.EnsureSuccessStatusCode();
            var expediente = result.Content.ReadAsAsync<ExpedienteObra>().Result;

            return new JsonResult
            {
                Data = expediente != null ?
                (expediente.FechaExpediente != null ?
                expediente.FechaExpediente.Value.ToShortDateString() : string.Empty) : string.Empty
            };
        }

        public JsonResult FormatNomenclatura(long idParcela, string nomenclatura)
        {
            var result = _cliente.GetAsync("api/parcela/getparcelabyid/" + idParcela).Result;
            result.EnsureSuccessStatusCode();
            var parcela = result.Content.ReadAsAsync<Parcela>().Result;

            var nomenclaturas = parcela.Nomenclaturas.First(x => x.Nombre == nomenclatura).GetNomenclaturas();
            //var str = nomenclaturas.Select(x => x.Key + ": " + x.Value).Aggregate((s1, s2) => s1 + " " + s2);

            return new JsonResult
            {
                Data = new
                {
                    nomenclatura = nomenclaturas.Select(x => x.Key + ": " + x.Value).Aggregate((s1, s2) => s1 + " " + s2),
                    idParcela
                }
            };
        }

        public ActionResult ValidarPartida(string numero)
        {
            string partida = $"{Session["CODIGO_JURISDICCION"]}{numero}{Session["TIPO_PARCELA"]}".ToUpper();

            if (UnidadAlfanumericoParcela.OperacionesParcelasDestinos.Any(x => x.Item.UnidadesTributarias.Any(u => u.CodigoProvincial == partida)))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Conflict);
            }
            return Content(partida);
        }

        public ActionResult SaveParcelaOrigen(long operacion, long idParcela)
        {
            if (operacion != Convert.ToInt64(TiposParcelaOperacion.Creacion))
            {
                using (var result = _cliente.GetAsync($"api/parcela/{idParcela}/simple").Result)
                {
                    result.EnsureSuccessStatusCode();
                    var parcela = result.Content.ReadAsAsync<Parcela>().Result;
                    UnidadAlfanumericoParcela.OperacionesParcelasOrigenes.Add(new OperationItem<Parcela>
                    {
                        Operation = Operation.Add,
                        Item = parcela
                    });
                    return Json(new { id = idParcela, idTipo = parcela.TipoParcelaID, idClase = parcela.ClaseParcelaID });
                }
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        public ActionResult DeleteParcelaOrigen(long idParcela)
        {
            int idx = 0;
            if ((idx = UnidadAlfanumericoParcela.OperacionesParcelasOrigenes.FindIndex(x => x.Item.ParcelaID == idParcela)) != -1)
            {
                UnidadAlfanumericoParcela.OperacionesParcelasOrigenes.RemoveAt(idx);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        public ActionResult SaveParcelaDestino(string partida, long idTipoOperacion, long idParcela, long idTipoParcela,
            long idClaseParcela, long idEstadoParcela, double? superficie, string expedienteCreacion, string fechaCreacion)
        {
            using (_cliente)
            using (var resp = _cliente.GetAsync($"api/UnidadTributaria/GetPartidaDisponible?idUnidadTributaria={-1}&partida={partida}").Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Conflict);
                }

                using (var result = _cliente.GetAsync("api/parametro/getparametro/32").Result)
                {
                    var origen = result.Content.ReadAsAsync<ParametrosGenerales>().Result;
                    int tipoUtId = (idClaseParcela == 5 && idTipoOperacion == 4) ? 2 : 1;
                    var parcela = new Parcela
                    {
                        ParcelaID = idParcela,
                        Superficie = idTipoOperacion == 4 ? Convert.ToDecimal(superficie) : 0,
                        TipoParcelaID = idTipoParcela,
                        ClaseParcelaID = idClaseParcela,
                        EstadoParcelaID = idEstadoParcela,
                        OrigenParcelaID = origen.Valor.AsInt(),
                        ExpedienteAlta = expedienteCreacion,
                        FechaAltaExpediente = !fechaCreacion.IsEmpty() ? DateTime.Parse(fechaCreacion) : (DateTime?)null,
                        UnidadesTributarias = new List<UnidadTributaria>
                                                {
                                                    new UnidadTributaria { CodigoProvincial = partida,
                                                                           TipoUnidadTributariaID = tipoUtId,
                                                                           Superficie = idTipoOperacion == 4 ? superficie : 0 }
                                                },
                        UsuarioModificacionID = Usuario.Id_Usuario

                    };

                    bool AfectaPH = false;
                    AfectaPH = (idClaseParcela == 5 && idTipoOperacion == 4) ? true : false;
                    parcela.AtributosCrear(string.Empty, AfectaPH);

                    UnidadAlfanumericoParcela.OperacionesParcelasDestinos.Add(new OperationItem<Parcela>
                    {
                        Operation = Operation.Add,
                        Item = parcela
                    });
                }
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
        }

        public ActionResult DeleteParcelaDestino(long idParcela)
        {
            int idx = 0;
            if ((idx = UnidadAlfanumericoParcela.OperacionesParcelasDestinos.FindIndex(x => x.Item.ParcelaID == idParcela)) != -1)
            {
                UnidadAlfanumericoParcela.OperacionesParcelasDestinos.RemoveAt(idx);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }

        public ActionResult Save(int operacion, string expediente, string fecha, string vigencia)
        {
            UnidadAlfanumericoParcela.Operacion = operacion;
            UnidadAlfanumericoParcela.Expediente = expediente;
            UnidadAlfanumericoParcela.Fecha = !fecha.IsEmpty() ? DateTime.Parse(fecha) : (DateTime?)null;
            UnidadAlfanumericoParcela.IdJurisdiccion = Convert.ToInt64(Session["ID_JURISDICCION"]);
            UnidadAlfanumericoParcela.Vigencia = !vigencia.IsEmpty() ? DateTime.Parse(vigencia) : (DateTime?)null;
            UnidadAlfanumericoParcela.IdUsuario = Usuario.Id_Usuario;

            var modelo = new
            {
                unidadAlfanumericoParcela = UnidadAlfanumericoParcela,
                machineName = AuditoriaHelper.ReverseLookup(Request.UserHostAddress),
                ip = Request.UserHostAddress
            };
            var content = new StringContent(JsonConvert.SerializeObject(modelo), Encoding.UTF8, "application/json");
            var result = _cliente.PostAsync("api/alfanumericoparcela/post", content).Result;
            string data = "IDX_SOLR";
            try
            {
                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Conflict);
                }
                else if (result.StatusCode != System.Net.HttpStatusCode.ExpectationFailed)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
                }
                data = "MVW_PARCELA";

            }

            UnidadAlfanumericoParcela.Clear();
            return new JsonResult { Data = data };
        }

        public JsonResult UnidadAlfanumericoParcelaClear()
        {
            UnidadAlfanumericoParcela.Clear();
            return new JsonResult { Data = "Ok" };
        }

        public string GetValidarNumeroExpediente()
        {
            var result = _cliente.GetAsync("api/parametro/getvalor/" + @Recursos.ValidarNumeroExpediente).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsStringAsync().Result;
        }

        public List<ValuacionMejoraModel> GetMejorasByParcelaId(long ParcelaID)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/ValuacionService/GetMejorasByParcelaId/" + ParcelaID).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValuacionMejoraModel>)resp.Content.ReadAsAsync<List<ValuacionMejoraModel>>().Result;
        }

        public string GetZonaTributaria()
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/AlfanumericoParcela/GetZonaTributaria/").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<string>().Result;
        }

        public ActionResult SetJurisdiccion(long id)
        {
            using (var resp = _cliente.GetAsync($"api/ObjetoAdministrativoService/GetObjetoById/{id}").Result)
            {
                resp.EnsureSuccessStatusCode();
                Session["ID_JURISDICCION"] = id;
                Session["CODIGO_JURISDICCION"] = resp.Content.ReadAsAsync<OA.Objeto>().Result.Codigo;
                return Content(Session["CODIGO_JURISDICCION"].ToString());
            }
        }

        [HttpGet]
        public ActionResult GetJurisdiccionesByDepartamentoParcela(long id)
        {
            try
            {
                using (var resp = _cliente.GetAsync($"api/Parcela/GetJurisdiccionesByDepartamentoParcela/{id}").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    var data = resp.Content.ReadAsAsync<Dictionary<long, List<OA.Objeto>>>().Result;
                    var kvp = data.Single();
                    return Json(new { selectedValue = kvp.Key, values = kvp.Value.OrderBy(j => j.Nombre) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError($"AlfanumericoParcelaController/GetJurisdiccionesByDepartamentoParcela/{id}", ex);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult GetJurisdiccionesByDepartamentoParcelaUnificacion(long[] ids)
        {
            Dictionary<long, List<OA.Objeto>> jurisdicciones = new Dictionary<long, List<OA.Objeto>>();

            foreach (var idP in (ids ?? new long[0]))
            {
                using (var resp = _cliente.GetAsync($"api/Parcela/GetJurisdiccionesByDepartamentoParcela/{idP}").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    var data = resp.Content.ReadAsAsync<Dictionary<long, List<OA.Objeto>>>().Result;
                    var kvp = data.Single();
                    if (!jurisdicciones.ContainsKey(kvp.Key))
                    {
                        jurisdicciones.Add(kvp.Key, kvp.Value);
                    }
                    //jurisdicciones.Add(kvp);
                }
            }
            return Json(jurisdicciones.Values);
        }

        [HttpGet]
        public ActionResult GetJurisdicciones()
        {
            try
            {
                return Json(new { selectedValue = GetJurisdiccionByConfig().FeatId, values = GetAllJurisdicciones().OrderBy(j => j.Codigo) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError($"AlfanumericoParcelaController/GetJurisdicciones", ex);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }
        public ActionResult SetTipoParcelas(long id)
        {
            Session["TIPO_PARCELA"] = id.ToString();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        public string GetZonaValuatoria()
        {
            using (var resp = _cliente.GetAsync("api/AlfanumericoParcela/GetZonaValuatoria/").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<string>().Result;
            }
        }

        public List<OA.Domicilio> GetDatosDomiciliosByParcela(long parcelaId)
        {
            using (var resp = _cliente.GetAsync("api/AlfanumericoParcela/GetDatosDomiciliosByParcela/parcelaId=" + parcelaId).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<OA.Domicilio>>().Result;
            }
        }

        private TipoNomenclatura GetNomenclaturaById(long tipoNomenclaturaID)
        {
            using (var resp = _cliente.GetAsync("api/TipoNomenclatura/GetById?Id=" + tipoNomenclaturaID).Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<TipoNomenclatura>().Result;
            }
        }

        public ActionResult GetExpedienteRegularExpression()
        {
            var param = GetParametrosGenerales().FirstOrDefault(x => x.Clave == "EXP_CREACION");
            var obj = new object();

            if (param != null)
            {
                using (var resp = _cliente.GetAsync($"api/GenericoService/RegexRandomGenerator?regex={Convert.ToBase64String(Encoding.UTF8.GetBytes(param.Valor))}").Result)
                {
                    resp.EnsureSuccessStatusCode();
                    obj = new
                    {
                        regex = param.Valor,
                        ejemplo = resp.Content.ReadAsAsync<string>().Result
                    };
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        private List<ParametrosGeneralesModel> GetParametrosGenerales()
        {
            using (var resp = _cliente.GetAsync("api/SeguridadService/GetParametrosGenerales").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<ParametrosGeneralesModel>>().Result;
            }
        }

    }
}