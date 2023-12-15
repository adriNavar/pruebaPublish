using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.InterfaseRentas;
using GeoSit.Data.DAL.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using GeoSit.Data.DAL;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Dynamic;
using System.Net;
using System.IO;
using System.Text;
using System.Linq.Expressions;
using System.Xml.Linq;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers.InterfaseRentas
{
    public class InterfaseRentasHelper
    {
        const long TIPO_PARCELA_URBANA = 1;
        const long TIPO_PARCELA_RURAL = 2;

        const long TIPO_INSCRIPCION_MATRICULA = 1;
        const long TIPO_INSCRIPCION_TOMO_FOLIO_FINCA = 3;

        const int REGIMEN_BALDIO = 1;
        const int REGIMEN_EDIFICADO = 2;
        const int REGIMEN_PH = 3;

        const short ESTADO_OK = 1;
        const short ESTADO_ERROR = -1;

        private UnitOfWork _unitOfWork;

        private bool _interfaseEnabled;
        private string _wsAvaluo, _wsAvaluoMasivo, _wsBaja, _wsBajaPedido, _wsEdicion;
        private string _wsBusquedaPersonas, _wsZonasTributarias;

        public InterfaseRentasHelper(UnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
            using (var db = GeoSITMContext.CreateContext())
            {
                int buffer;
                var parametros = db.ParametrosGenerales.Where(x => x.Agrupador == "INTERFASE_SISTEMA_TRIBUTARIO");

                var param = parametros.FirstOrDefault(x => x.Clave == "IST_ENABLED");
                _interfaseEnabled = param != null ? int.TryParse(param.Valor, out buffer) && buffer != 0 : false;

                param = parametros.FirstOrDefault(x => x.Clave == "IST_WS_AVALUO");
                if (param != null) _wsAvaluo = param.Valor;

                param = parametros.FirstOrDefault(x => x.Clave == "IST_WS_AVALUO_MASIVO");
                if (param != null) _wsAvaluoMasivo = param.Valor;

                param = parametros.FirstOrDefault(x => x.Clave == "IST_WS_BAJA");
                if (param != null) _wsBaja = param.Valor;

                param = parametros.FirstOrDefault(x => x.Clave == "IST_WS_BAJA_PEDIDO");
                if (param != null) _wsBajaPedido = param.Valor;

                param = parametros.FirstOrDefault(x => x.Clave == "IST_WS_EDICION");
                if (param != null) _wsEdicion = param.Valor;

                param = parametros.FirstOrDefault(x => x.Clave == "IST_WS_BUSQUEDA_PERSONAS");
                if (param != null) _wsBusquedaPersonas = param.Valor;

                param = parametros.FirstOrDefault(x => x.Clave == "IST_WS_ZONAS_TRIBUTARIAS");
                if (param != null) _wsZonasTributarias = param.Valor;
            }
        }

        public string VerificarDeuda(long parcelaId, string partida)
        {
            int nroPartida = Convert.ToInt32(partida);
            return Execute<inmBajaPedido.BajadeInmueble>
            (
                client =>
                {
                    return client.bajaPedido(nroPartida);
                },
                "bajaPedido", _wsBajaPedido, new object[] { nroPartida }, parcelaId, partida
            );
        }

        public string Alta(Parcela parcela, string partida)
        {
            return Edicion("N", parcela, partida);
        }

        public string Modificacion(Parcela parcela, string partida)
        {
            return Edicion("M", parcela, partida);
        }

        private string Edicion(string tipoAccion, Parcela parcela, string partida)
        {
            string result = string.Empty;
            if (parcela.UnidadesTributarias != null && parcela.UnidadesTributarias.Any())
            {
                var partidaEntity = parcela.UnidadesTributarias.FirstOrDefault(x => x.CodigoMunicipal == partida);
                if (partidaEntity != null)
                {
                    // Nomenclatura
                    string s1 = string.Empty, s2 = string.Empty, s3 = string.Empty;
                    string manz = string.Empty, parc = string.Empty;
                    string urbsub = string.Empty;

                    if (parcela.Nomenclaturas != null && parcela.Nomenclaturas.Any())
                    {
                        var nomenclatura = parcela.Nomenclaturas.FirstOrDefault();
                        if (nomenclatura != null)
                        {
                            if (nomenclatura.Tipo == null)
                            {
                                nomenclatura.Tipo = _unitOfWork.TiposNomenclaturasRepository.GetTipoNomenclaturaById(nomenclatura.TipoNomenclaturaID);
                            }
                            if (nomenclatura.Tipo != null && !string.IsNullOrEmpty(nomenclatura.Tipo.ExpresionRegular))
                            {
                            var regex = new Regex(nomenclatura.Tipo.ExpresionRegular);
                            var match = regex.Match(nomenclatura.Nombre);
                            if (match.Success)
                            {
                                var groupNames = new HashSet<string>(regex.GetGroupNames());
                                var getGroupValue = new Func<string, string>(groupName =>
                                {
                                    if (groupNames.Contains(groupName) && match.Groups[groupName].Success)
                                    {
                                        return match.Groups[groupName].Value;
                                    }
                                    return string.Empty;
                                });
                                s1 = getGroupValue("Cir");
                                s2 = getGroupValue("Sect");
                                parc = getGroupValue("Par");
                                if (nomenclatura.TipoNomenclaturaID == TIPO_PARCELA_RURAL) s3 = getGroupValue("Div");
                                if (nomenclatura.TipoNomenclaturaID == TIPO_PARCELA_URBANA) manz = getGroupValue("Div");
                            }
                        }
                    }
                    }
                    switch (parcela.TipoParcelaID)
                    {
                        case 1:
                            urbsub = "U";
                            break;
                        case 2:
                            urbsub = "R";
                            break;
                    }

                    // Partida
                    int uf, codigoMunicipal, codigoProvincial;
                    int.TryParse(partidaEntity.UnidadFuncional, out uf);
                    int.TryParse(partidaEntity.CodigoMunicipal, out codigoMunicipal);
                    int.TryParse(partidaEntity.CodigoProvincial, out codigoProvincial);

                    // Domicilio
                    int calleId = 0, nroPuerta = 0;
                    string calle = string.Empty, piso = string.Empty, dpto = string.Empty;
                    if (partidaEntity.UTDomicilios != null)
                    {
                        var domicilioUT = partidaEntity.UTDomicilios.OrderByDescending(x => x.DomicilioID).FirstOrDefault();
                        var domicilio = domicilioUT != null ? domicilioUT.Domicilio : null;
                        if (domicilio != null)
                        {
                            int.TryParse(domicilio.numero_puerta, out nroPuerta);
                            calleId = domicilio.ViaId.HasValue ? Convert.ToInt32(domicilio.ViaId.Value) : 0;
                            calle = domicilio.ViaNombre;
                            dpto = domicilio.unidad ?? string.Empty;
                            piso = domicilio.piso ?? string.Empty;
                        }
                    }

                    // Valuación
                    int vigencia = 0;
                    float supTierra = 0, supMejora = 0, valorTierra = 0, valorMejora = 0;
                    if (partidaEntity.Valuaciones != null)
                    {
                        var valuacion = partidaEntity.Valuaciones.OrderByDescending(x => x.IdPadron).FirstOrDefault();
                        if (valuacion != null)
                        {
                            supTierra = valuacion.SuperficieTierra;
                            supMejora = valuacion.SuperificeCubierta + valuacion.SuperficieSemiCubierta;
                            valorTierra = Convert.ToSingle(valuacion.ValorTierra);
                            valorMejora = Convert.ToSingle(valuacion.ValorMejora);
                            DateTime? fechaVigencia = valuacion.Fecha_Vigencia_Desde.HasValue ? valuacion.Fecha_Vigencia_Desde.Value : (DateTime?)null;
                            vigencia = fechaVigencia.HasValue ? Convert.ToInt32(string.Format("{0:0000}{1:00}", fechaVigencia.Value.Year, fechaVigencia.Value.Month)) : 0;
                        }
                    }

                    // Regimen                    
                    var mejoras = _unitOfWork.ParcelaRepository.GetMejorasByIdParcela(parcela.ParcelaID);
                    int regimen = (mejoras.Count == 0 || supMejora <= 0) ? REGIMEN_BALDIO : (partidaEntity.PorcentajeCopropiedad == 100 ? REGIMEN_EDIFICADO : REGIMEN_PH);

                    // Dominio
                    DateTime fechaInscripcion = DateTime.MinValue;
                    string tipoInscripcion = string.Empty, matricula = string.Empty;
                    var dominios = _unitOfWork.DominioRepository.GetDominios(partidaEntity.UnidadTributariaId);

                    if (dominios != null && dominios.Any())
                    {
                        var dom = dominios.OrderByDescending(x => x.DominioID).FirstOrDefault();
                        if (dom != null)
                        {
                            if (dom.TipoInscripcionID == TIPO_INSCRIPCION_TOMO_FOLIO_FINCA)
                            {
                                tipoInscripcion = "T";
                            }
                            else if (dom.TipoInscripcionID == TIPO_INSCRIPCION_MATRICULA)
                            {
                                tipoInscripcion = "M";
                                matricula = dom.Inscripcion;
                            }
                            fechaInscripcion = dom.Fecha;
                        }
                    }
                    // Titulares     
                    var titulares = new List<inmEdita.titulares_inmueble>();
                    var responsablesFiscales = _unitOfWork.UnidadTributariaPersonaRepository.GetUnidadTributariaResponsablesFiscales(partidaEntity.UnidadTributariaId);
                    if (responsablesFiscales != null)
                    {
                        foreach (var responsableFiscal in responsablesFiscales)
                        {
                            Titular titular = null;
                            if (dominios != null)
                            {
                                titular = dominios.Where(x => x.Titulares != null)
                                                  .SelectMany(x => x.Titulares)
                                                  .OrderByDescending(x => x.DominioPersonaId)
                                                  .FirstOrDefault(x => x.PersonaId == responsableFiscal.PersonaId);
                            }
                            titulares.Add(new inmEdita.titulares_inmueble
                            {
                                tipo = "1", // Titular
                                num = responsableFiscal.CodSistemaTributario,
                                porc = Convert.ToString(titular != null ? titular.PorcientoCopropiedad : 100, CultureInfo.InvariantCulture)
                            });
                        }
                    }
                    // Zona Tributaria
                    CargarZonaTributaria(ref parcela);

                    var datosInmueble = new inmEdita.datos_inmueble
                    {
                        accion = tipoAccion,
                        obj_id = codigoMunicipal,
                        s1 = s1,
                        s2 = s2,
                        s3 = s3,
                        manz = manz,
                        parc = parc,
                        uf = uf,
                        porcuf = Convert.ToInt32(partidaEntity.PorcentajeCopropiedad),
                        parp = codigoProvincial,
                        anio_mensura = 1900,
                        regimen = regimen,
                        fchmatric = fechaInscripcion,
                        tmatric = tipoInscripcion,
                        matric = matricula,
                        supt = supTierra,
                        supm = supMejora,
                        avalt = valorTierra,
                        avalm = valorMejora,
                        calle_id = calleId,
                        calle_nom = calle,
                        puerta = nroPuerta,
                        piso = piso,
                        dpto = dpto,
                        vigencia = vigencia,
                        titularidad = "PA", // PA. Particular / MU. Municipal / PR. Provincial / NA. Nacional / AN. Adj.Nacional / AM. Adj.Municipal / AP. Adj.Provincial / PO. Posesión
                        zonat = parcela.ZonaTributaria,
                        urbsub = urbsub,
                        tipo = "N",
                        det = string.Empty
                    };
                    var datosTitulares = titulares.ToArray();
                    result = Execute<inmEdita.AltayModificacióndeInmueble>
                    (
                        client =>
                        {
                            return client.edita(datosInmueble, datosTitulares);
                        },
                        "edita", _wsEdicion, new object[] { datosInmueble, datosTitulares }, parcela.ParcelaID, partida
                    );
                }
                else
                {
                    result = string.Format("No se encontró la partida {0}.", partida);
                }
            }
            else
            {
                var nomenclatura = parcela.Nomenclaturas.FirstOrDefault();
                result = string.Format("La parcela {0} ({1}) no posee unidades tributarias.", parcela.ParcelaID, nomenclatura != null ? nomenclatura.Nombre : "Sin nomenclatura");
            }
            return result;
        }

        public string Baja(long parcelaId, string partida, DateTime fechaBaja)
        {
            var datosEntrada = new inmBaja.datos_entrada
            {
                objeto = Convert.ToInt32(partida),
                motivo = "Baja Manual GeoSIT",
                fchbaja = fechaBaja
            };
            return Execute<inmBaja.BajadeInmueble>
            (
                client =>
                {
                    return client.baja(datosEntrada);
                },
                "baja", _wsBaja, new object[] { datosEntrada }, parcelaId, partida
            );
        }

        public string Avaluo(long parcelaId, string partida, int vigencia, string supm, string supt, string avalm, string avalt)
        {
            int regimen = REGIMEN_EDIFICADO;
            var parcela = _unitOfWork.ParcelaRepository.GetParcelaById(parcelaId);
            if (parcela != null)
            {
                CargarZonaTributaria(ref parcela);
            }
            var mejoras = _unitOfWork.ParcelaRepository.GetMejorasByIdParcela(parcelaId);
            if (mejoras.Count == 0 && supm == "0" && avalm == "0")
            {
                regimen = REGIMEN_BALDIO;
            }
            else
            {
                var partidaEntity = _unitOfWork.UnidadTributariaRepository.GetUnidadesTributariasByParcela(parcelaId).FirstOrDefault(x => x.CodigoMunicipal == partida);
                if (partidaEntity != null)
                {
                    regimen = (partidaEntity.PorcentajeCopropiedad == 100 ? REGIMEN_EDIFICADO : REGIMEN_PH);
                }
            }
            var datosInmueble = new inmAvaluo.datos_inmueble
            {
                obj_id = Convert.ToInt32(partida),
                vigencia = vigencia,
                regimen = regimen,
                avalm = avalm,
                avalt = avalt,
                supm = supm,
                supt = supt,
                frente = "0",
                zonat = parcela != null ? parcela.ZonaTributaria : string.Empty
            };
            return Execute<inmAvaluo.AvaluodeInmueble>
            (
                client =>
                {
                    return client.avaluo(datosInmueble);
                },
                "avaluo", _wsAvaluo, new object[] { datosInmueble }, parcelaId, partida
            );
        }

        public string AvaluoMasivo(int[] vigencia, string[] obj_id, string[] supm, string[] supt, string[] avalm, string[] avalt, bool[] esPH)
        {
            var datosInmueble = new inmAvaluoMasivo.datos_inmueble { vigencia = vigencia[0] };
            var avaluos = new List<inmAvaluoMasivo.avaluos_inmuebles>();

            for (int i = 0; i < obj_id.Length; i++)
            {
                decimal supMejoras;
                int regimen = (decimal.TryParse(supm[i], NumberStyles.Number, CultureInfo.InvariantCulture, out supMejoras) && supMejoras > 0) ? (esPH[i] ? REGIMEN_PH : REGIMEN_EDIFICADO) : REGIMEN_BALDIO;

                avaluos.Add(new inmAvaluoMasivo.avaluos_inmuebles
                {
                    obj_id = obj_id[i],
                    supm = supm[i],
                    supt = supt[i],
                    avalm = avalm[i],
                    avalt = avalt[i],
                    regimen = regimen
                });
            }
            return Execute<inmAvaluoMasivo.AvaluoMasivodeInmueble>
            (
                client =>
                {
                    return client.avaluomasivo(datosInmueble, avaluos.ToArray());
                },
                "avaluomasivo", _wsAvaluoMasivo, null, null, null
            );
        }

        public InterfaseRentasResponse<InterfaseRentasPersona[]> BuscarPersonas(string nombre, int doc = 0, string cuit = "")
        {
            var result = new InterfaseRentasResponse<InterfaseRentasPersona[]>();
            try
            {
                if (!string.IsNullOrEmpty(_wsBusquedaPersonas))
                {
                    result.Data = BuscarPersonas(_wsBusquedaPersonas, nombre, doc, cuit);
                    if (result.Data.Length == 0)
                    {
                        string[] nameParts = Regex.Split(nombre, @"[\s,;]+");
                        if (nameParts.Length > 0)
                        {
                            result.Data = BuscarPersonas(_wsBusquedaPersonas, nameParts[0], doc, cuit);
                            if (result.Data.Length > 1 && nameParts.Length > 1)
                            {
                                var names = new HashSet<string>(nameParts.Skip(1));
                                result.Data = result.Data.Where(x1 => names.Any(x2 => x1.Nombre.IndexOf(x2, StringComparison.InvariantCultureIgnoreCase) > -1)).ToArray();
                            }
                        }
                    }
                }
                else throw new ApplicationException("El servicio de búsqueda de personas no está configurado.");
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        private InterfaseRentasPersona[] BuscarPersonas(string webService, string nombre, int doc, string cuit)
        {
            var personas = new List<InterfaseRentasPersona>();
            string urlTemplate = _wsBusquedaPersonas + "?nombre={0}&doc={1}&cuit={2}";
            string url = string.Format(urlTemplate, nombre, doc, cuit);

            var req = (HttpWebRequest)WebRequest.Create(url);
            var res = req.GetResponse();

            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {

                var searchResult = ParseSearchResult(reader.ReadToEnd());
                foreach (var entry in searchResult)
                {
                    var item = entry.Value as Dictionary<string, object>;
                    if (item != null)
                    {
                        personas.Add(new InterfaseRentasPersona
                        {
                            Codigo = Convert.ToString(item["num"]),
                            Nombre = Convert.ToString(item["nombre"]),
                            TipoDocumento = Convert.ToString(item["tdoc"]),
                            NroDocumento = Convert.ToString(item["ndoc"]),
                            CUIT = Convert.ToString(item["cuit"]),
                            Domicilio = Convert.ToString(item["domi"]),
                            Telefono = Convert.ToString(item["tel"]),
                            Celular = Convert.ToString(item["cel"]),
                            Mail = Convert.ToString(item["mail"])
                        });
                    }
                }
                if (personas.Any())
                {
                    personas = personas.OrderBy(x => x.Nombre).ToList();
                }
            }
            return personas.ToArray();
        }

        public InterfaseRentasZonaTributaria[] GetZonasTributarias()
        {
            var zonas = new List<InterfaseRentasZonaTributaria>();
            if (!string.IsNullOrEmpty(_wsZonasTributarias))
            {
                var req = (HttpWebRequest)WebRequest.Create(_wsZonasTributarias);
                var res = req.GetResponse();

                using (var stream = res.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {

                    var searchResult = ParseSearchResult(reader.ReadToEnd());
                    foreach (var entry in searchResult)
                    {
                        var item = entry.Value as Dictionary<string, object>;
                        if (item != null)
                        {
                            zonas.Add(new InterfaseRentasZonaTributaria
                            {
                                Codigo = Convert.ToString(item["cod"]),
                                Nombre = Convert.ToString(item["nombre"])
                            });
                        }
                    }
                    if (zonas.Any())
                    {
                        zonas = zonas.OrderBy(x => x.Nombre).ToList();
                    }
                }
            }
            return zonas.ToArray();
        }

        private void CargarZonaTributaria(ref Parcela parcela)
        {
            if (string.IsNullOrEmpty(parcela.ZonaTributaria) && !string.IsNullOrEmpty(parcela.Atributos))
            {
                try
                {
                    var xdoc = XDocument.Parse(parcela.Atributos);
                    var node = xdoc.Descendants("ZonaTributaria").FirstOrDefault();
                    parcela.ZonaTributaria = node != null ? node.Value : string.Empty;
                }
                catch
                {
                    parcela.ZonaTributaria = string.Empty;
                }
            }
        }

        public InterfaseRentasLog Reprocesar(InterfaseRentasLog log)
        {
            try
            {
                log.Estado = 0;
                var serviceType = Type.GetType(log.WebServiceClass, false);
                if (serviceType != null)
                {
                    var method = serviceType.GetMethod(log.Operacion);
                    if (method != null)
                    {
                        List<object> parameters = null;
                        if (!string.IsNullOrEmpty(log.Parametros))
                        {
                            var paramBuffer = JsonConvert.DeserializeObject(log.Parametros) as JArray;
                            if (paramBuffer != null)
                            {
                                parameters = new List<object>();
                                foreach (var token in paramBuffer)
                                {
                                    object param = null;
                                    var type = Type.GetType(token.Value<string>("_type"));
                                    var value = token.Value<object>("_value");

                                    if (value is JValue)
                                        param = (value as JValue).ToObject(type);
                                    else if (value is JObject)
                                        param = (value as JObject).ToObject(type);

                                    parameters.Add(param);
                                }
                            }
                        }
                        var methodParams = method.GetParameters();
                        int paramCount = methodParams != null ? methodParams.Length : 0;

                        if (parameters == null && paramCount > 0)
                        {
                            log.Resultado = "Este método no puede se puede reprocesar.";
                        }
                        else
                        {
                            var service = Activator.CreateInstance(serviceType);
                            if (!string.IsNullOrEmpty(log.WebServiceUrl))
                            {
                                var urlProperty = serviceType.GetProperty("Url");
                                if (urlProperty != null)
                                {
                                    urlProperty.SetValue(service, log.WebServiceUrl);
                                }
                            }
                            log.Resultado = Convert.ToString(method.Invoke(service, parameters != null ? parameters.ToArray() : null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Resultado = ex.Message;
                log.Estado = ESTADO_ERROR;
            }
            finally
            {
                CheckStatus(ref log);
                log.Fecha = DateTime.Now;
                if (log.Estado != 0) log.TransactionID++;
                _unitOfWork.InterfaseRentasLogRepository.UpdateLog(log);
                _unitOfWork.Save();
            }
            return log;
        }

        private string Execute<T>(Func<T, string> accion, string operacion,
                string customUrl, object[] parametros, long? idParcela, string partida)
                where T : System.Web.Services.Protocols.SoapHttpClientProtocol, new()
        {
            if (_interfaseEnabled)
            {
                var log = new InterfaseRentasLog
                {
                    TransactionID = 1,
                    Partida = partida,
                    ParcelaID = idParcela,
                    Fecha = DateTime.Now,
                    Operacion = operacion,
                    Resultado = string.Empty,
                    WebServiceUrl = customUrl,
                    WebService = typeof(T).Name,
                    WebServiceClass = typeof(T).FullName
                };
                if (parametros != null)
                {
                    log.Parametros = JsonConvert.SerializeObject(parametros.Select(x => x != null ? new { _type = x.GetType().FullName, _value = x } : null).ToArray());
                }
                try
                {
                    using (var client = new T())
                    {
                        if (!string.IsNullOrEmpty(customUrl))
                        {
                            client.Url = customUrl;
                        }
                        log.Resultado = accion(client);
                    }
                }
                catch (Exception ex)
                {
                    log.Resultado = ex.Message;
                    log.Estado = ESTADO_ERROR;
                }
                finally
                {
                    CheckStatus(ref log);
                    _unitOfWork.InterfaseRentasLogRepository.InsertLog(log);
                    _unitOfWork.Save();
                }
                return log.Resultado;
            }
            return "INFO: Interfase no habilitada.";
        }


        private void CheckStatus(ref InterfaseRentasLog log)
        {
            if (log.Resultado.StartsWith("ERROR", StringComparison.InvariantCultureIgnoreCase))
            {
                log.Estado = ESTADO_ERROR;
            }
            else if (log.Resultado.StartsWith("OK", StringComparison.InvariantCultureIgnoreCase))
            {
                log.Estado = ESTADO_OK;
            }
        }

        private Dictionary<string, Dictionary<string, object>> ParseSearchResult(string input)
        {
            var result = new Dictionary<string, Dictionary<string, object>>();
            string parentKey = string.Empty;
            string key = string.Empty;
            char prev = (char)0;

            string token = string.Empty;
            var sb = new StringBuilder();
            bool isArray = false;
            bool isValue = false;
            bool isKey = false;

            foreach (char c in input)
            {
                if (c == '[')
                {
                    isKey = true;
                    continue;
                }
                if (c == ']')
                {
                    isKey = false;
                    key = sb.ToString();
                    sb.Clear();
                    continue;
                }
                if (c == '>' && prev == '=')
                {
                    isValue = true;
                    continue;
                }
                if (char.IsWhiteSpace(c) && prev == '>')
                {
                    continue;
                }
                if (c == '\n')
                {
                    isValue = false;
                }
                if ((char.IsWhiteSpace(c) && !isValue))
                {
                    if (sb.Length > 0)
                    {
                        token = sb.ToString().Trim();
                        isArray = token.ToLower() == "array";
                        if (isArray) parentKey = key;

                        if (!string.IsNullOrEmpty(parentKey))
                        {
                            Dictionary<string, object> item;
                            if (result.ContainsKey(parentKey))
                            {
                                item = result[parentKey];
                            }
                            else
                            {
                                item = new Dictionary<string, object>();
                                result.Add(parentKey, item);
                            }
                            if (!isArray)
                            {
                                if (!item.ContainsKey(key)) item.Add(key, token);
                            }
                        }
                    }
                    sb.Clear();
                }
                if (isKey || isValue)
                {
                    sb.Append(c);
                }
                prev = c;
            }
            return result;
        }
    }
}