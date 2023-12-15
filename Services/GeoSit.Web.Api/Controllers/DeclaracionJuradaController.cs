using System;
using System.Collections.Generic;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Models;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace GeoSit.Web.Api.Controllers
{
    public class DeclaracionJuradaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DeclaracionJuradaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetVersiones()
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetVersiones());
        }

        public IHttpActionResult GetVersion(int idVersion)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetVersion(idVersion));
        }

        public IHttpActionResult GetDesignacionByUt(long idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDesignacionByUt(idUnidadTributaria));
        }

        public IHttpActionResult GetTiposTitularidad()
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetTiposTitularidad());
        }

        public IHttpActionResult GetSorOtrasCar(int idVersion)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetSorOtrasCar(idVersion));
        }

        public IHttpActionResult GetUOtrasCar(int idVersion)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetUOtrasCar(idVersion));
        }

        public IHttpActionResult GetSorTipoCaracteristicas()
        {
            var versiones = _unitOfWork.DeclaracionJuradaRepository.GetSorTipoCaracteristicas();
            return Ok(versiones);
        }

        public IHttpActionResult GetDominios(long idDeclaracionJurada)
        {
            var dominios = _unitOfWork.DeclaracionJuradaRepository.GetDominios(idDeclaracionJurada);
            return Ok(dominios);
        }

        [HttpGet]
        [Route("api/declaracionjurada/dominios/{idUT}/default")]
        public IHttpActionResult GetDominiosByIdUnidadTributaria(long idUT)
        {
            var dominios = _unitOfWork.DeclaracionJuradaRepository.GetDominiosByIdUnidadTributaria(idUT);
            return Ok(dominios);
        }

        public IHttpActionResult GetDeclaracionJuradaSor(int idDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionJurada(idDeclaracionJurada));
        }

        public IHttpActionResult GetDeclaracionJuradaU(int idDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionJurada(idDeclaracionJurada));
        }

        public IHttpActionResult GetMejora(int idDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetMejora(idDeclaracionJurada));
        }

        public IHttpActionResult GetDeclaracionJurada(int idDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionJurada(idDeclaracionJurada));
        }

        public IHttpActionResult GetDeclaracionJuradaCompleta(int idDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionJuradaCompleta(idDeclaracionJurada));
        }

        public IHttpActionResult GetValuaciones(int idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetValuaciones(idUnidadTributaria));
        }

        public IHttpActionResult GetValuacionesHistoricas(int idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetValuacionesHistoricas(idUnidadTributaria));
        }

        [HttpPost]
        [Route("api/declaracionjurada/DeleteValuacion")]
        public IHttpActionResult DeleteValuacion([FromBody] JObject postdata)
        {
            int idValuacion = int.Parse(postdata["idValuacion"].ToString());
            int idUsuario = int.Parse(postdata["idUsuario"].ToString());

            return Ok(_unitOfWork.DeclaracionJuradaRepository.DeleteValuacion(idValuacion, idUsuario));
        }

        [HttpPost]
        [Route("api/declaracionjurada/SaveValuacion")]
        public IHttpActionResult SaveValuacion([FromBody] JObject postdata)
        {
            VALValuacion valuacion = postdata["valuacion"].ToObject<VALValuacion>();
            int idUsuario = int.Parse(postdata["idUsuario"].ToString());

            return Ok(_unitOfWork.DeclaracionJuradaRepository.SaveValuacion(valuacion, idUsuario));
        }

        public IHttpActionResult GetDecretoByNumero(long nroDecreto)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDecretoByNumero(nroDecreto));
        }

        public IHttpActionResult GetTramiteByNumero(long nroTramite)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetTramiteByNumero(nroTramite));
        }

        public IHttpActionResult GetValuacion(int idValuacion)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetValuacion(idValuacion));
        }

        public IHttpActionResult GetValuacionVigente(long idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetValuacionVigente(idUnidadTributaria));
        }

        public IHttpActionResult GetDDJJDesignacion(int IdDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDDJJDesignacion(IdDeclaracionJurada));
        }

        public IHttpActionResult GetDeclaracionesJuradas(int idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionesJuradas(idUnidadTributaria));
        }

        public IHttpActionResult GetDeclaracionesJuradasE1E2NoVigentes(int idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionesJuradasNoVigentes(idUnidadTributaria));
        }

        public IHttpActionResult GetDeclaracionJuradaVigenteU(long idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionJuradaVigenteU(idUnidadTributaria));
        }

        public IHttpActionResult GetDeclaracionJuradaVigenteSoR(long idUnidadTributaria)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetDeclaracionJuradaVigenteSoR(idUnidadTributaria));
        }


        public IHttpActionResult GetTramite(int IdDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetTramite(IdDeclaracionJurada));
        }

        public IHttpActionResult GetMensura(int idMensura)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetMensura(idMensura));
        }

        public IHttpActionResult GetOCObjetos(int idSubtipoObjeto)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetOCObjetos(idSubtipoObjeto));
        }

        [HttpPost]
        [Route("api/declaracionjurada/SaveDDJJSor")]
        public IHttpActionResult SaveDDJJSor([FromBody] JObject postdata)
        {
            var ddjj = postdata["ddjj"].ToObject<DDJJ>();
            var ddjjSor = postdata["ddjjSor"].ToObject<DDJJSor>();
            var ddjjDesignacion = postdata["ddjjDesignacion"].ToObject<DDJJDesignacion>();
            var dominios = postdata["dominios"].ToObject<List<DDJJDominio>>();
            var sorCar = postdata["sorCar"].ToObject<List<DDJJSorCar>>();
            var superficies = postdata["superficies"].ToObject<List<VALSuperficies>>();

            long idUsuario = long.Parse(postdata["idUsuario"].ToString());
            string ip = postdata["ip"].ToString();
            string machineName = postdata["machineName"].ToString();

            bool resultado = _unitOfWork.DeclaracionJuradaRepository.SaveDDJJSor(ddjj, ddjjSor, ddjjDesignacion, dominios, sorCar, superficies, idUsuario, machineName, ip);
            return Ok(resultado);
        }

        [HttpPost]
        [Route("api/declaracionjurada/SaveDDJJU")]
        public IHttpActionResult SaveDDJJU([FromBody] JObject postdata)
        {
            try
            {
                var ddjj = postdata["ddjj"].ToObject<DDJJ>();
                var ddjjU = postdata["ddjjU"].ToObject<DDJJU>();
                var ddjjDesignacion = postdata["ddjjDesignacion"].ToObject<DDJJDesignacion>();
                var dominios = postdata["dominios"].ToObject<List<DDJJDominio>>();

                long idUsuario = long.Parse(postdata["idUsuario"].ToString());
                string ip = postdata["ip"].ToString();
                string machineName = postdata["machineName"].ToString();

                string clasesSeleccionada = postdata["clasesSeleccionada"].ToString();

                List<ClaseParcela> clases = new List<ClaseParcela>();

                if (!string.IsNullOrEmpty(clasesSeleccionada))
                {
                    clasesSeleccionada = clasesSeleccionada.Remove(clasesSeleccionada.Length - 1, 1).Remove(0, 1);
                    clasesSeleccionada = "[" + clasesSeleccionada + "]";
                    clases = JsonConvert.DeserializeObject<List<ClaseParcela>>(clasesSeleccionada);
                }
                bool resultado = _unitOfWork.DeclaracionJuradaRepository.SaveDDJJU(ddjj, ddjjU, ddjjDesignacion, dominios, idUsuario, clases, machineName, ip);
                //SolrUpdater.Instance.Enqueue(Entities.parcela);
                //SolrUpdater.Instance.Enqueue(Entities.prescripcion);
                //SolrUpdater.Instance.Enqueue(Entities.unidadtributaria);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/declaracionjurada/SaveFormularioE1")]
        public IHttpActionResult SaveFormularioE1([FromBody] JObject postdata)
        {
            var ddjj = postdata["ddjj"].ToObject<DDJJ>();
            var mejora = postdata["mejora"].ToObject<INMMejora>();
            var otrasCar = postdata["otrasCar"].ToObject<List<INMMejoraOtraCar>>();
            var caracteristicas = postdata["caracteristicas"].ToObject<List<int>>();

            long idUsuario = long.Parse(postdata["idUsuario"].ToString());
            string ip = postdata["ip"].ToString();
            string machineName = postdata["machineName"].ToString();

            return Ok(_unitOfWork.DeclaracionJuradaRepository.SaveFormularioE1(ddjj, mejora, otrasCar, caracteristicas, idUsuario, machineName, ip));
        }

        [HttpPost]
        [Route("api/declaracionjurada/SaveFormularioE2")]
        public IHttpActionResult SaveFormularioE2([FromBody] JObject postdata)
        {
            var ddjj = postdata["ddjj"].ToObject<DDJJ>();
            var mejora = postdata["mejora"].ToObject<INMMejora>();
            var otrasCar = postdata["otrasCar"].ToObject<List<INMMejoraOtraCar>>();
            var caracteristicas = postdata["caracteristicas"].ToObject<List<int>>();

            long idUsuario = long.Parse(postdata["idUsuario"].ToString());
            string ip = postdata["ip"].ToString();
            string machineName = postdata["machineName"].ToString();

            return Ok(_unitOfWork.DeclaracionJuradaRepository.SaveFormularioE2(ddjj, mejora, otrasCar, caracteristicas, idUsuario, machineName, ip));
        }

        public IHttpActionResult GetAptitudes(int idVersion)
        {
            var aptitudes = _unitOfWork.DeclaracionJuradaRepository.GetAptitudes(idVersion);
            return Ok(aptitudes);
        }

        [HttpGet]
        [Route("api/DeclaracionJurada/Aptitudes")]
        public IHttpActionResult GetAptitudes()
        {
            var aptitudes = _unitOfWork.DeclaracionJuradaRepository.GetAptitudes();
            return Ok(aptitudes);
        }

        public IHttpActionResult GetCaracteristicas()
        {
            var caracteristicas = _unitOfWork.DeclaracionJuradaRepository.GetCaracteristicas();
            return Ok(caracteristicas);
        }

        public IHttpActionResult GetAptCar()
        {
            var aptcar = _unitOfWork.DeclaracionJuradaRepository.GetAptCar();
            return Ok(aptcar);
        }

        public IHttpActionResult GetSorCar(long idSor)
        {
            var sorCar = _unitOfWork.DeclaracionJuradaRepository.GetSorCar(idSor);
            return Ok(sorCar);
        }

        public IHttpActionResult GetValSuperficies(long idSor)
        {
            var valSuperficies = _unitOfWork.DeclaracionJuradaRepository.GetValSuperficies(idSor);
            return Ok(valSuperficies);
        }

        public IHttpActionResult GetInmOtrasCaracteristicas(long idVersion)
        {
            var inmOtrasCaracteristicas = _unitOfWork.DeclaracionJuradaRepository.GetInmOtrasCaracteristicas(idVersion);
            return Ok(inmOtrasCaracteristicas);
        }

        public IHttpActionResult GetDestinosMejoras(long idVersion)
        {
            var destinosMejoras = _unitOfWork.DeclaracionJuradaRepository.GetDestinosMejoras(idVersion);
            return Ok(destinosMejoras);
        }

        public IHttpActionResult GetMejoraOtraCar(long idMejora)
        {
            var mejoraOtraCar = _unitOfWork.DeclaracionJuradaRepository.GetMejoraOtraCar(idMejora);
            return Ok(mejoraOtraCar);
        }

        public IHttpActionResult GetClasesDisponibles()
        {
            var clasesAvailables = _unitOfWork.DeclaracionJuradaRepository.GetClasesParcelas();
            return Ok(clasesAvailables);
        }



        //
        // Nos devuelve todas las clases disponibles con sus respectivos tipos de medida lineal.

        public IHttpActionResult GetClasesDisponiblesDepthRel()
        {

            return Ok(this.GetClasesDisponiblesDepth());
        }


        private List<ClaseParcela> GetClasesDisponiblesDepth()
        {

            List<ClaseParcela> listResult = new List<ClaseParcela>();
            List<VALClasesParcelas> clases = _unitOfWork.DeclaracionJuradaRepository.GetClasesParcelasFull();

            foreach (VALClasesParcelas c in clases)
            {
                ClaseParcela parcela = new ClaseParcela()
                {
                    IdClaseParcela = c.IdClaseParcela,
                    Descripcion = c.Descripcion
                };

                if (c.ClasesParcelasMedidasLineales != null && c.ClasesParcelasMedidasLineales.Count > 0)
                {
                    foreach (VALClasesParcelasMedidaLineal cml in c.ClasesParcelasMedidasLineales.OrderBy(x => x.Orden))
                    {
                        TipoMedidaLinealConParcela tipo = new TipoMedidaLinealConParcela()
                        {
                            Orden = cml.Orden,
                            IdTipoMedidaLineal = cml.TipoMedidaLineal.IdTipoMedidaLineal,
                            IdClasesParcelasMedidaLineal = cml.IdClasesParcelasMedidaLineal,
                            Descripcion = cml.TipoMedidaLineal.Descripcion,
                            RequiereAforo = cml.RequiereAforo.HasValue ? (cml.RequiereAforo.Value == 1 ? true : false) : false,
                            RequiereLongitud = cml.RequiereLongitud.HasValue ? (cml.RequiereLongitud.Value == 1 ? true : false) : false,
                        };

                        parcela.TiposMedidasLineales.Add(tipo);
                    }
                }

                listResult.Add(parcela);
            }

            return listResult;
        }

        [HttpPost]
        [Route("api/declaracionjurada/Revaluar")]
        public IHttpActionResult Revaluar([FromBody] JObject postdata)
        {
            return Ok(_unitOfWork
                            .DeclaracionJuradaRepository
                            .Revaluar(postdata["idUnidadTributaria"].ToObject<long>(),
                                      postdata["idUsuario"].ToObject<long>(),
                                      postdata["machineName"].ToString(),
                                      postdata["ip"].ToString()));
        }

        public IHttpActionResult GetPersonaDomicilios(long idPersona)
        {
            var personaDomicilios = _unitOfWork.DeclaracionJuradaRepository.GetPersonaDomicilios(idPersona);
            return Ok(personaDomicilios);
        }

        public IHttpActionResult GetMedidaLineasFromFraccionByIdU(int idDeclaracionJuradaU)
        {
            List<ClaseParcela> clasesToEdit = this.GetMedidaLineasFromFraccionByIdUPrepare(idDeclaracionJuradaU);
            return Ok(clasesToEdit);
        }

        public IHttpActionResult GetLocalidadesByDistancia(long distanciaLocalidad)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetLocalidadesByDistancia(distanciaLocalidad));
        }

        public IHttpActionResult GetClasesParcelasBySuperficie(decimal superficie)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetClasesParcelasBySuperficie(superficie));
        }

        public IHttpActionResult GetCroquisClaseParcela(int idClaseParcela)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetCroquisClaseParcela(idClaseParcela));
        }

        public IHttpActionResult GetIdDepartamentoByCodigo(string codigo)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetIdDepartamentoByCodigo(codigo));
        }

        public IHttpActionResult GetClaseParcelaByIdDDJJ(long idDeclaracionJurada)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetClaseParcelaByIdDDJJ(idDeclaracionJurada));
        }

        //
        // Nos devuelve las clases (con sus respectivos tipos relacionados) de una declaración jurada.
        // Este método se utiliza para la carga, cuando es una modificación.

        public List<ClaseParcela> GetMedidaLineasFromFraccionByIdUPrepare(int idDeclaracionJuradaU)
        {

            List<DDJJUFracciones> fracciones = _unitOfWork.DeclaracionJuradaRepository.GetMedidaLineasFromFraccionByIdU(idDeclaracionJuradaU);

            if (fracciones == null || fracciones.Count == 0)
            {
                return new List<ClaseParcela>();
            }

            List<ClaseParcela> clasesDisponibles = this.GetClasesDisponiblesDepth();

            List<ClaseParcela> result = new List<ClaseParcela>();

            foreach (DDJJUFracciones i in fracciones)
            {
                DDJJUMedidaLineal ml = i.MedidasLineales.FirstOrDefault();
                ClaseParcela cp = new ClaseParcela()
                {
                    IdClaseParcela = ml.ClaseParcelaMedidaLineal.IdClaseParcela,
                    Descripcion = getDescripcion(ml.ClaseParcelaMedidaLineal.IdClaseParcela, clasesDisponibles)
                };


                foreach (DDJJUMedidaLineal mlItem in i.MedidasLineales)
                {
                    TipoMedidaLinealConParcela cpConf = this.getTipoMedidaLinealByIds(
                        mlItem.ClaseParcelaMedidaLineal.IdClaseParcela,
                        mlItem.ClaseParcelaMedidaLineal.IdTipoMedidaLineal,
                        clasesDisponibles);

                    Aforo aforoFound = null;

                    if (cpConf.RequiereAforo)
                    {
                        aforoFound = _unitOfWork.DeclaracionJuradaRepository.BuscarAforoPorId(mlItem.IdTramoVia, mlItem.IdVia);

                    }

                    cp.TiposMedidasLineales.Add(
                        new TipoMedidaLinealConParcela()
                        {
                            IdClasesParcelasMedidaLineal = mlItem.IdClaseParcelaMedidaLineal,
                            IdTipoMedidaLineal = mlItem.ClaseParcelaMedidaLineal.IdTipoMedidaLineal,
                            Calle = aforoFound != null ? aforoFound.Calle : mlItem.Calle,
                            Desde = aforoFound != null && !string.IsNullOrEmpty(aforoFound.Desde) ? aforoFound.Desde : string.Empty,
                            Hasta = aforoFound != null && !string.IsNullOrEmpty(aforoFound.Hasta) ? aforoFound.Hasta : string.Empty,
                            Paridad = aforoFound != null && !string.IsNullOrEmpty(aforoFound.Paridad) ? aforoFound.Paridad : string.Empty,
                            Altura = mlItem.AlturaCalle?.ToString(),
                            Descripcion = mlItem.ClaseParcelaMedidaLineal.TipoMedidaLineal.Descripcion,
                            Orden = mlItem.ClaseParcelaMedidaLineal.Orden,
                            RequiereAforo = cpConf != null ? cpConf.RequiereAforo : false,
                            RequiereLongitud = cpConf != null ? cpConf.RequiereLongitud : false,
                            ValorAforo = mlItem.ValorAforo,
                            ValorMetros = mlItem.ValorMetros,
                            IdTramoVia = mlItem.IdTramoVia,
                            IdVia = mlItem.IdVia
                        }
                    );
                };

                result.Add(cp);
            }

            return result;


        }



        private TipoMedidaLinealConParcela getTipoMedidaLinealByIds(long idClaseParcela, long idTipoMedidaLineal, List<ClaseParcela> clasesDisponibles)
        {

            ClaseParcela cp = clasesDisponibles.Where(w => w.IdClaseParcela.Equals(idClaseParcela)).FirstOrDefault();

            TipoMedidaLinealConParcela tml = cp?.TiposMedidasLineales.Where(w => w.IdTipoMedidaLineal.Equals(idTipoMedidaLineal)).FirstOrDefault();

            return tml;

        }

        [HttpGet()]
        [Route("api/DeclaracionJurada/BuscarAforo")]
        public IHttpActionResult BuscarAforo(long idLocalidad, string calle, long? idVia, int? altura)
        {
            Aforo aforoResult = _unitOfWork.DeclaracionJuradaRepository.BuscarAforoAlgoritmo(idLocalidad, calle, idVia, altura);

            if (aforoResult != null)
            {
                aforoResult.Altura = altura.HasValue ? altura.Value.ToString() : string.Empty;
            }

            return Ok(aforoResult);
        }

        public IHttpActionResult GetDecretos()
        {
            var decretos = _unitOfWork.DeclaracionJuradaRepository.GetDecretos();
            return Ok(decretos);
        }

        [HttpPost]
        [Route("api/declaracionjurada/AplicarDecreto")]
        public IHttpActionResult AplicarDecreto([FromBody] JObject postdata)
        {
            long idDecreto = postdata["idDecreto"].ToObject<long>();
            int idUsuario = postdata["idUsuario"].ToObject<int>();
            var decretoAplicado = _unitOfWork.DeclaracionJuradaRepository.AplicarDecreto(idDecreto, idUsuario);
            return Ok(decretoAplicado);
        }

        public IHttpActionResult GetAplicarDecretoIsRunning()
        {
            var isRunning = _unitOfWork.DeclaracionJuradaRepository.GetAplicarDecretoIsRunning();
            return Ok(isRunning);
        }

        public IHttpActionResult GetAplicarDecretoStatus()
        {
            var status = _unitOfWork.DeclaracionJuradaRepository.GetAplicarDecretoStatus();
            return Ok(status);
        }

        public IHttpActionResult GetEstadosConservacion()
        {
            var estadosConservacion = _unitOfWork.DeclaracionJuradaRepository.GetEstadosConservacion();
            return Ok(estadosConservacion);
        }

        private string getDescripcion(long idClaseParcela, List<ClaseParcela> clasesParcela)
        {
            ClaseParcela cp = clasesParcela.Where(w => w.IdClaseParcela.Equals(idClaseParcela)).FirstOrDefault();

            if (cp == null)
                return string.Empty;
            else
                return cp.Descripcion;

        }

        public IHttpActionResult GetTipoMedidaLineales()
        {
            List<TipoMedidaLineal> result = new List<TipoMedidaLineal>();
            List<VALTiposMedidasLineales> tmlList = _unitOfWork.DeclaracionJuradaRepository.GetTipoMedidaLineales();

            if (tmlList != null)
            {
                result = tmlList.Select(s => new TipoMedidaLineal() { IdTipoMedidaLineal = s.IdTipoMedidaLineal, Descripcion = s.Descripcion }).ToList();
            }

            return Ok(result);
        }
        public IHttpActionResult GetInmCaracteristicas(int idVersion)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetInmCaracteristicas(idVersion));
        }

        public IHttpActionResult GetInmIncisos(int idVersion)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetInmIncisos(idVersion));
        }

        public IHttpActionResult GetInmTipoCaracteristicas(int idVersion)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetInmTipoCaracteristicas(idVersion));
        }

        public IHttpActionResult GetInmMejorasCaracteristicas(int idMejora)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.GetInmMejorasCaracteristicas(idMejora));
        }

        [HttpGet]
        public IHttpActionResult BuscarAforoById(long? idTramoVia, long? idVia)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.BuscarAforoPorId(idTramoVia, idVia));
        }

        [HttpPost]
        [Route("api/DeclaracionJurada/Aforos/Vias")]
        public IHttpActionResult BuscarAforoMultiplesVias(List<Tuple<long?,long?>> tramos_y_vias)
        {
            return Ok(_unitOfWork.DeclaracionJuradaRepository.BuscarAforosVia(tramos_y_vias));
        }

        [HttpGet]
        public IHttpActionResult ValoresAforoValido()
        {
            try
            {
                return Ok(_unitOfWork.DeclaracionJuradaRepository.ValoresAforoValido());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return Content(System.Net.HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"DeclaracionJurada->ValoresAforoValido", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult BajaMejoras([FromBody] JObject data)
        {
            try
            {
                _unitOfWork.DeclaracionJuradaRepository.BajaMejoras(long.Parse(data["idDDJJ"].ToString()), long.Parse(data["idUsuario"].ToString()), data["machineName"].ToString(), data["ip"].ToString());

                return Ok();
            }
            catch (ApplicationException)
            {
                return StatusCode(System.Net.HttpStatusCode.PreconditionFailed);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"DeclaracionJurada->BajaMejoras", ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("api/declaracionjurada/{version}/unidadtributaria/{idUnidadTributaria}/inconsistencias")]
        public IHttpActionResult ValidarConsistencia(long idUnidadTributaria, long version)
        {
            try
            {
                return Ok(_unitOfWork.DeclaracionJuradaRepository.ValidarConsistencia(idUnidadTributaria, version));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"ValidarConsistencia/{version}/ut/{idUnidadTributaria}", ex);
                return InternalServerError();
            }
        }
    }
}
