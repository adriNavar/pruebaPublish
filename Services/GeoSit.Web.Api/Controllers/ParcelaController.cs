using System;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.DAL.Common;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeoSit.Web.Api.Controllers.InterfaseRentas;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Web.Api.Ploteo;
using GeoSit.Web.Api.Solr;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Web.Api.Common;
using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using Newtonsoft.Json;

namespace GeoSit.Web.Api.Controllers
{
    public class ParcelaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly InterfaseRentasHelper _interfaseRentasHelper;

        public ParcelaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _interfaseRentasHelper = new InterfaseRentasHelper(unitOfWork);
        }

        public IHttpActionResult GetParcelaById(long id)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetParcelaById(id));
        }

        //Nueva fun param idParcela --> nombreCapa y el/los featId

        // GET api/parcela/Get/5
        public Parcela Get(int id)
        {
            return _unitOfWork.ParcelaRepository.GetParcelaById(id);
        }

        public IHttpActionResult GetZonificacion(long idParcela, bool esHistorico = false)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetZonificacion(idParcela, esHistorico));
        }

        public IHttpActionResult GetAtributosZonificacion(long idParcela)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetAtributosZonificacion(idParcela));
        }

        public IHttpActionResult GetParcelasOrigen(long idParcelaDestino)
        {
            return Ok(_unitOfWork.ParcelaOperacionRepository.GetParcelasOrigenOperacion(idParcelaDestino));
        }

        public IHttpActionResult GetParcelaOperacionesOrigen(long idParcelaOperacion)
        {
            return Ok(_unitOfWork.ParcelaOperacionRepository.GetParcelaOperacionesOrigen(idParcelaOperacion));
        }

        public IHttpActionResult GetParcelaOperacionesDestino(long idParcelaOperacion)
        {
            return Ok(_unitOfWork.ParcelaOperacionRepository.GetParcelaOperacionesDestino(idParcelaOperacion));
        }

        public IHttpActionResult GetParcelaDatos(long idParcelaOperacion)
        {
            return Ok(_unitOfWork.ParcelaOperacionRepository.GetParcelaDatos(idParcelaOperacion));
        }

        public IHttpActionResult GetParcelaValuacionZonas()
        {
            return Ok(_unitOfWork.ParcelaRepository.GetParcelaValuacionZonas());
        }

        public IHttpActionResult GetParcelaValuacionZona(long idAtributoZona)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetParcelaValuacionZona(idAtributoZona));
        }

        public IHttpActionResult GetParcelaByUt(long idUnidadTributaria)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetParcelaByUt(idUnidadTributaria));
        }

        [Route("api/parcela/{id}/esVigente")]
        [HttpGet]
        public IHttpActionResult ParcelaVigente(long id)
        {
            return Ok(_unitOfWork.ParcelaRepository.EsVigente(id));
        }

            
        public IHttpActionResult Post(UnidadMantenimientoParcelario unidadMantenimientoParcelario)
        {
            try
            {
                unidadMantenimientoParcelario.OperacionesUnidadTributaria.AnalyzeOperations("UnidadTributariaId");
                unidadMantenimientoParcelario.OperacionesParcelaOrigen.AnalyzeOperations("ParcelaOperacionID");
                unidadMantenimientoParcelario.OperacionesParcelaDocumento.AnalyzeOperations("ParcelaDocumentoID");
                unidadMantenimientoParcelario.OperacionesUnidadTributariaDocumento.AnalyzeOperations("UnidadTributariaDocID");
                unidadMantenimientoParcelario.OperacionesNomenclatura.AnalyzeOperations("NomenclaturaID");
                unidadMantenimientoParcelario.OperacionesUnidadTributariaPersona.AnalyzeOperations("PersonaID");
                unidadMantenimientoParcelario.OperacionesDominio.AnalyzeOperations("DominioID");
                unidadMantenimientoParcelario.OperacionesDominioTitular.AnalyzeOperations("PersonaID");
                unidadMantenimientoParcelario.OperacionesDesignaciones.AnalyzeOperations("IdDesignacion");
                unidadMantenimientoParcelario.OperacionesVIR.AnalyzeOperations("InmuebleId");

                Parcela parcela = null;

                var auditoriasParcela = new List<Auditoria>();
                var auditoriasUT = new Dictionary<UnidadTributaria, List<Auditoria>>();

                long getOperacion(Operation operacion)
                {
                    long idTipoOperacion = Convert.ToInt64(TiposOperacion.Alta);
                    switch (operacion)
                    {
                        case Operation.Update:
                            idTipoOperacion = Convert.ToInt64(TiposOperacion.Modificacion);
                            break;
                        case Operation.Remove:
                            idTipoOperacion = Convert.ToInt64(TiposOperacion.Baja);
                            break;
                    }
                    return idTipoOperacion;
                }
                Auditoria auditarParcela(string idEvento, Operation operacion, string datosAdicionales)
                {
                    var auditoria = new Auditoria()
                    {
                        Id_Objeto = parcela.ParcelaID,
                        Id_Evento = long.Parse(idEvento),
                        Id_Tipo_Operacion = getOperacion(operacion),
                        Datos_Adicionales = datosAdicionales,
                        Cantidad = 1,
                        Objeto = "Parcela",
                        Autorizado = Autorizado.Si,
                        Machine_Name = parcela._Machine_Name,
                        Ip = parcela._Ip,
                        Id_Usuario = parcela.UsuarioModificacionID,
                    };
                    auditoriasParcela.Add(auditoria);
                    return auditoria;
                }
                void auditarUnidadTributaria(UnidadTributaria ut, string idEvento, Operation operacion, string datosAdicionales)
                {
                    if (!auditoriasUT.Any(kvp => kvp.Key.UnidadTributariaId == ut.UnidadTributariaId))
                    {
                        auditoriasUT.Add(ut, new List<Auditoria>());
                    }

                    auditoriasUT[auditoriasUT.Single(kvp => kvp.Key.UnidadTributariaId == ut.UnidadTributariaId).Key].Add(new Auditoria()
                    {
                        Id_Evento = long.Parse(idEvento),
                        Id_Tipo_Operacion = getOperacion(operacion),
                        Datos_Adicionales = datosAdicionales,
                        Cantidad = 1,
                        Objeto = "UnidadTributaria",
                        Autorizado = Autorizado.Si,
                        Machine_Name = parcela._Machine_Name,
                        Ip = parcela._Ip,
                        Id_Usuario = parcela.UsuarioModificacionID,
                    });
                }

                if (unidadMantenimientoParcelario.OperacionesParcela.Any())
                {
                    parcela = unidadMantenimientoParcelario.OperacionesParcela.First().Item;
                    //unidadesTributariasModificadas = parcela.UnidadesTributarias ?? _unitOfWork.UnidadTributariaRepository.GetUnidadesTributarias(parcela.ParcelaID);
                    //if (unidadesTributariasModificadas != null)
                    //{
                    //    var idCollection = new HashSet<long>(unidadMantenimientoParcelario.OperacionesUnidadTributaria.Select(x => x.Item.UnidadTributariaId));
                    //    unidadesTributariasModificadas = unidadesTributariasModificadas.Where(x => !x.FechaBaja.HasValue && !idCollection.Contains(x.UnidadTributariaId)).ToList();
                    //}
                }
                foreach (var operacion in unidadMantenimientoParcelario.OperacionesParcela)
                {
                    if (operacion.Operation == Operation.Update)
                    {
                        _unitOfWork.ParcelaRepository.UpdateParcela(operacion.Item);
                        auditarParcela(Eventos.ModificarMantenedorParcelario, Operation.Update, "Se han guardado los datos de la parcela");
                    }
                }

                foreach (var operacion in unidadMantenimientoParcelario.OperacionesParcelaOrigen)
                {
                    string partida = operacion.Item.ParcelaOrigen.UnidadesTributarias.Single().CodigoProvincial;
                    string evento = Eventos.AltaParcelaOrigen;
                    operacion.Item.UsuarioModificacionID = parcela.UsuarioModificacionID;
                    operacion.Item.ParcelaOrigen = operacion.Item.ParcelaDestino = null;
                    switch (operacion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.ParcelaOperacionRepository.InsertParcelaOperacion(operacion.Item);
                            break;
                        case Operation.Update:
                            evento = Eventos.ModificacionParcelaOrigen;
                            _unitOfWork.ParcelaOperacionRepository.EditParcelaOperacion(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaParcelaOrigen;
                            _unitOfWork.ParcelaOperacionRepository.DeleteParcelaOperacion(operacion.Item);
                            break;
                    }
                    auditarParcela(evento, operacion.Operation, $"Partida: {partida}");
                }

                foreach (var operacion in unidadMantenimientoParcelario.OperacionesUnidadTributaria)
                {
                    operacion.Item.UsuarioModificacionID = parcela.UsuarioModificacionID;
                    operacion.Item.TipoUnidadTributaria = null;
                    operacion.Item.CodigoProvincial = operacion.Item.CodigoProvincial.ToUpper();

                    string evento = Eventos.AltaMantenedorUnidadTributaria;

                    Operation finalOp = parcela.FechaBaja.HasValue && operacion.Operation == Operation.Add
                                                ? Operation.None
                                                : operacion.Operation;

                    switch (finalOp)
                    {
                        case Operation.Add:
                            operacion.Item.ParcelaID = parcela.ParcelaID;
                            _unitOfWork.UnidadTributariaRepository.InsertUnidadTributaria(operacion.Item);
                            break;
                        case Operation.Update:
                            evento = Eventos.ModificarMantenedorUnidadTributaria;
                            _unitOfWork.UnidadTributariaRepository.EditUnidadTributaria(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaMantenedorUnidadTributaria;
                            _unitOfWork.UnidadTributariaRepository.DeleteUnidadTributaria(operacion.Item);
                            break;
                    }
                    auditarParcela(evento, operacion.Operation, operacion.Item.CodigoProvincial);
                    auditarUnidadTributaria(operacion.Item, evento, operacion.Operation, null);
                }

                if (parcela.FechaBaja.HasValue)
                {
                    foreach (var ut in _unitOfWork.UnidadTributariaRepository.GetUnidadesTributariasByParcela(parcela.ParcelaID))
                    {
                        ut.UsuarioBajaID = ut.UsuarioModificacionID = parcela.UsuarioBajaID;
                        ut.FechaBaja = ut.FechaModificacion = parcela.FechaBaja;
                        //ut.TipoUnidadTributaria = null;
                        string evento = Eventos.BajaMantenedorUnidadTributaria;
                        Operation operacion = Operation.Remove;
                        auditarParcela(evento, operacion, ut.CodigoProvincial);
                        auditarUnidadTributaria(ut, evento, operacion, null);
                    }
                }


                foreach (var operacion in unidadMantenimientoParcelario.OperacionesParcelaDocumento)
                {
                    operacion.Item.UsuarioModificacionID = parcela.UsuarioModificacionID;
                    string evento = Eventos.AltaDocumentosParcela;
                    switch (operacion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.ParcelaDocumentoRepository.InsertParcelaDocumento(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaDocumentosParcela;
                            _unitOfWork.ParcelaDocumentoRepository.DeleteParcelaDocumento(operacion.Item);
                            break;
                    }

                    var doc = _unitOfWork.DocumentoRepository.GetDocumentoById(operacion.Item.DocumentoID);
                    auditarParcela(evento, operacion.Operation, doc.nombre_archivo);
                }

                foreach (var operacion in unidadMantenimientoParcelario.OperacionesUnidadTributariaDocumento)
                {
                    operacion.Item.UsuarioModificacionID = parcela.UsuarioModificacionID;
                    string evento = Eventos.AltaDocumentosUnidadTributaria;
                    switch (operacion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.UnidadTributariaDocumentoRepository.InsertUnidadTributariaDocumento(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaDocumentosUnidadTributaria;
                            _unitOfWork.UnidadTributariaDocumentoRepository.RemoveUnidadTributariaDocumento(operacion.Item);
                            break;
                    }

                    var doc = _unitOfWork.DocumentoRepository.GetDocumentoById(operacion.Item.DocumentoID);
                    auditarUnidadTributaria(new UnidadTributaria { UnidadTributariaId = operacion.Item.UnidadTributariaID }, evento, operacion.Operation, doc.nombre_archivo);
                }

                foreach (var operacion in unidadMantenimientoParcelario.OperacionesNomenclatura)
                {
                    operacion.Item.UsuarioModificacionID = parcela.UsuarioModificacionID;
                    string evento = Eventos.AltaNomenclatura;
                    switch (operacion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.NomenclaturaRepository.InsertNomenclatura(operacion.Item);
                            break;
                        case Operation.Update:
                            evento = Eventos.ModificarNomenclatura;
                            _unitOfWork.NomenclaturaRepository.UpdateNomenclatura(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaNomenclatura;
                            _unitOfWork.NomenclaturaRepository.DeleteNomenclatura(operacion.Item);
                            break;
                    }

                    auditarParcela(evento, operacion.Operation, operacion.Item.Nombre);
                }

                double ms = 0d;
                foreach (var operacion in unidadMantenimientoParcelario.OperacionesDominio)
                {
                    operacion.Item.IdUsuarioModif = parcela.UsuarioModificacionID;

                    //me doy asco, deberían matarme y tirarme a la hoguera, pero a esta altura.... es lo que hay
                    //ante cualquier duda, ajo y agua. Ernesto.-
                    operacion.Item.FechaModif = DateTime.Now.AddMilliseconds(ms++); 
                    string evento = Eventos.AltaDominio;
                    switch (operacion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.DominioRepository.InsertDominio(operacion.Item);
                            break;
                        case Operation.Update:
                            evento = Eventos.ModificarDominio;
                            _unitOfWork.DominioRepository.UpdateDominio(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaDominio;
                            _unitOfWork.DominioRepository.DeleteDominio(operacion.Item);
                            break;
                    }
                    auditarUnidadTributaria(new UnidadTributaria { UnidadTributariaId = operacion.Item.UnidadTributariaID }, evento, operacion.Operation, operacion.Item.Inscripcion);
                }

                foreach (var operacion in unidadMantenimientoParcelario.OperacionesDominioTitular)
                {
                    operacion.Item.UsuarioModificacionID = parcela.UsuarioModificacionID;
                    string evento = Eventos.AltaTitular;
                    switch (operacion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.DominioTitularRepository.InsertDominioTitular(operacion.Item);
                            break;
                        case Operation.Update:
                            evento = Eventos.ModificarTitular;
                            _unitOfWork.DominioTitularRepository.UpdateDominioTitular(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaTitular;
                            _unitOfWork.DominioTitularRepository.DeleteDominioTitular(operacion.Item);
                            break;
                    }
                    var titular = _unitOfWork.PersonaRepository.GetPersonaDatos(operacion.Item.PersonaID);
                    var dom = unidadMantenimientoParcelario.OperacionesDominio.FirstOrDefault(d => d.Item.DominioID == operacion.Item.DominioID)?.Item
                              ?? _unitOfWork.DominioRepository.GetDominioById(operacion.Item.DominioID);

                    auditarUnidadTributaria(new UnidadTributaria { UnidadTributariaId = dom.UnidadTributariaID }, evento, operacion.Operation, $"{titular.NombreCompleto}");
                }

                foreach (var operacion in unidadMantenimientoParcelario.OperacionesDesignaciones)
                {
                    operacion.Item.IdUsuarioModif = parcela.UsuarioModificacionID;
                    string evento = Eventos.AltaDesignacion;
                    string tipo = operacion.Item.TipoDesignador?.Nombre;
                    operacion.Item.TipoDesignador = null;
                    switch (operacion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.DesignacionRepository.InsertDesignacion(operacion.Item);
                            break;
                        case Operation.Update:
                            evento = Eventos.ModificarDesignacion;
                            _unitOfWork.DesignacionRepository.UpdateDesignacion(operacion.Item);
                            break;
                        case Operation.Remove:
                            evento = Eventos.BajaDesignacion;
                            _unitOfWork.DesignacionRepository.DeleteDesignacion(operacion.Item);
                            break;
                    }
                    auditarParcela(evento, operacion.Operation, tipo);
                }

                foreach (var operacion in unidadMantenimientoParcelario.OperacionesVIR)
                {
                    string evento = Eventos.ModificacionVIR;
                    VIRInmueble inmueble;
                    switch (operacion.Operation)
                    {
                        case Operation.Update:
                            inmueble = _unitOfWork.VIRInmuebleRepository.SaveVIRInmueble(operacion.Item);
                            break;
                        default:
                            throw new InvalidOperationException("Operación no valida para datos VIR.");
                    }
                    var auditoria = auditarParcela(evento, operacion.Operation, $"Se modifica el inmueble VIR {operacion.Item.Partida}");

                    var entityEntry = _unitOfWork.GetDbEntityEntry(inmueble);
                    auditoria.Objeto_Origen = JsonConvert.SerializeObject(entityEntry.OriginalValues.ToObject());
                    auditoria.Objeto_Modif = JsonConvert.SerializeObject(entityEntry.CurrentValues.ToObject());
                }

                _unitOfWork.Save();

                DateTime ahora = DateTime.Now;
                foreach (var auditoria in auditoriasParcela)
                {
                    auditoria.Fecha = ahora;
                    _unitOfWork.AuditoriaRepository.InsertAuditoria(auditoria);
                }
                foreach (var kvp in auditoriasUT)
                {
                    foreach (var auditoria in kvp.Value)
                    {
                        auditoria.Id_Objeto = kvp.Key.UnidadTributariaId;
                        auditoria.Fecha = ahora;
                        _unitOfWork.AuditoriaRepository.InsertAuditoria(auditoria);
                    }
                }
                _unitOfWork.Save();

                //SolrUpdater.Instance.Enqueue(Entities.parcela);
                //SolrUpdater.Instance.Enqueue(Entities.prescripcion);
                //SolrUpdater.Instance.Enqueue(Entities.parcelamunicipal);
                //SolrUpdater.Instance.Enqueue(Entities.parcelaproyecto);
                //SolrUpdater.Instance.Enqueue(Entities.unidadtributaria);
                //SolrUpdater.Instance.Enqueue(Entities.unidadtributariahistorica);
                return Ok(parcela.FechaBaja == null);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Parcela/Post", ex);
                return InternalServerError(ex);
            }
        }

        public List<string> GetPartidasbyParcela(long idParcela)
        {
            return _unitOfWork.ParcelaRepository.GetPartidabyId(idParcela);
        }

        public IHttpActionResult GetVerificarDeuda(long parcelaId, string partidas)
        {
            var result = partidas.Split(',').Select
            (
                partida => new EstadoPartida
                {
                    partidaID = partida,
                    Estado = _interfaseRentasHelper.VerificarDeuda(parcelaId, partida)
                }
            );
            return Ok(result.ToArray());
        }

        public IHttpActionResult BajaComarcal(DateTime fechaBaja, long parcelaId, string partidas)
        {
            var result = partidas.Split(',').Select
            (
                partida => new EstadoPartida
                {
                    partidaID = partida,
                    Estado = _interfaseRentasHelper.Baja(parcelaId, partida, fechaBaja)
                }
            );
            return Ok(result.ToArray());
        }

        public IHttpActionResult GetJurisdiccion(long idParcela)
        {
            return Ok<Objeto>(null);

        }

        #region AltaModifComarcal
        //private void AltaModifComarcal(string tipoAccion, long idUnidadT, long? parcela, long? partida)
        //{            
        //    long parc = Convert.ToInt64(parcela);
        //    var parcelaData = _unitOfWork.ParcelaRepository.GetParcelaById(parc);
        //    idUnidadT = parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.UnidadTributariaId).FirstOrDefault();

        //    DateTime fechaVigencia = Convert.ToDateTime(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.Valuaciones.Select(y => y.Fecha_Vigencia_Hasta)).FirstOrDefault());
        //    using (var cliente = new inmEdita.AltayModificacióndeInmueble())
        //    {
        //        string resultado = cliente.edita(new inmEdita.datos_inmueble
        //        {
        //            accion = tipoAccion,
        //            obj_id = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.CodigoMunicipal).FirstOrDefault()),
        //            /*...*/
        //            uf = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.UnidadFuncional).FirstOrDefault()),
        //            porcuf = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.PorcentajeCopropiedad).FirstOrDefault()),
        //            parp = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.CodigoProvincial).FirstOrDefault()),
        //            /*...*/
        //            supt = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.Valuaciones.Select(y => y.SuperficieTierra)).FirstOrDefault()),
        //            supm = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.Valuaciones.Select(y => y.SuperficieSemiCubierta)).FirstOrDefault()),
        //            avalt = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.Valuaciones.Select(y => y.ValorTierra)).FirstOrDefault()),
        //            avalm = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.Valuaciones.Select(y => y.ValorMejora)).FirstOrDefault()),
        //            /*...*/
        //            calle_id = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.UTDomicilios.Select(y => y.Domicilio.ViaId))),
        //            calle_nom = parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.UTDomicilios.Select(y => y.Domicilio.ViaNombre)).FirstOrDefault().ToString(),
        //            puerta = Convert.ToInt32(parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.UTDomicilios.Select(y => y.Domicilio.numero_puerta))),
        //            piso = parcelaData.UnidadesTributarias.Where(x => x.CodigoMunicipal == partida.ToString()).Select(x => x.UTDomicilios.Select(y => y.Domicilio.piso)).FirstOrDefault().ToString(),
        //            /*...*/
        //            vigencia = Convert.ToInt32(fechaVigencia.Year + "" + fechaVigencia.Month)

        //        },
        //        new inmEdita.titulares_inmueble
        //        {

        //        });
        //        //LOG ESTADO OK
        //    }
        //    var a = _unitOfWork.UnidadTributariaRepository.GetUnidadTributariaByIdComplete(idUnidadT, parcela, partida);
        //}
        #endregion

        public Dictionary<string, string> GetNomenclaturasInternal(string ExpresionRegular, string Nombre)
        {
            //SI, esta mal copiar y pegar directo de la clase Nomenclatura, pero no podia traer la funcion. Acepto sugerencias.
            //Hay otro GetNomenclatura ver como funciona mas tarde.



            Dictionary<string, string> retVal = null;
            try
            {
                retVal = new Dictionary<string, string>();
                Regex regex = new Regex(ExpresionRegular);
                List<string> nombres = regex.GetGroupNames().ToList();
                GroupCollection partes = regex.Match(Nombre).Groups;
                nombres.RemoveAt(0);

                for (int i = 0; i < nombres.Count; i++)
                {
                    retVal.Add(nombres[i], partes[i + 1].Value);
                }

            }
            catch (Exception ex)
            {
                //ver catcheo
                throw ex;
            }
            return retVal;

        }

        public IHttpActionResult GetPlanosMensuraByIdParcela(long id)
        {
            return Ok(_unitOfWork.ParcelaDocumentoRepository.GetPlanoMensura(id));
        }

        [Route("api/parcela/{id}/superficies/")]
        [HttpGet]
        public IHttpActionResult GetSuperficiesByIdParcela(long id, bool esHistorico = false)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetSuperficiesByIdParcela(id, esHistorico));
        }

        [Route("api/parcela/{id}/estampilla")]
        [HttpGet]
        public IHttpActionResult GetParcelaEstampilla(long id)
        {
            ModPlot modPlot = new ModPlot(_unitOfWork.PlantillaRepository, _unitOfWork.LayerGrafRepository, _unitOfWork.PlantillaFondoRepository, _unitOfWork.HojaRepository, _unitOfWork.NorteRepository, _unitOfWork.ParcelaPlotRepository, _unitOfWork.CuadraPlotRepository, _unitOfWork.ManzanaPlotRepository, _unitOfWork.CallePlotRepository, _unitOfWork.ParametroRepository, _unitOfWork.ImagenSatelitalRepository, _unitOfWork.ExpansionPlotRepository, _unitOfWork.TipoPlanoRepository, _unitOfWork.PartidoRepository, _unitOfWork.CensoRepository,
                _unitOfWork.PloteoFrecuenteRepository, _unitOfWork.PloteoFrecuenteEspecialRepository, _unitOfWork.PlantillaViewportRepository, _unitOfWork.TipoViewportRepository, _unitOfWork.LayerViewportReposirory, _unitOfWork.AtributoRepository, _unitOfWork.ComponenteRepository);
            return Ok(modPlot.GetEstampilla(id));
        }

        [Route("api/parcela/{id}/simple")]
        [HttpGet]
        public IHttpActionResult GetParcelaSimple(long id)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetParcelaById(id, false));
        }

        [Route("api/parcela/{id}/utshistoricas")]
        [HttpGet]
        public IHttpActionResult GetParcelaUTSHistoricas(long id)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetParcelaById(id, utsHistoricas: true));
        }

        public IHttpActionResult GetJurisdiccionesByDepartamentoParcela(long id)
        {
            return Ok(_unitOfWork.ParcelaRepository.GetJurisdiccionesByDepartamentoParcela(id));
        }
    }
}
