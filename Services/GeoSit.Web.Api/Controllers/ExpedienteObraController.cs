using System.Linq;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ValidationRules;
using GeoSit.Data.BusinessEntities.ValidationRules.ExpedientesObra;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using System;
using System.Collections.Generic;
using System.Net.Http;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Certificados;

namespace GeoSit.Web.Api.Controllers
{
    public class ExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetSearch(long unidadTributariaId, string numeroLegajoIni, string numeroLegajoFin,
            string numeroExpedienteIni, string numeroExpedienteFin, string fechaLegajoIni, string fechaLegajoFin,
            string fechaExpedienteIni, string fechaExpedienteFin, long personaId, long estadoId)
        {
            var expedienteObras = _unitOfWork.ExpedienteObraRepository.GetExpedienteObras(unidadTributariaId, numeroLegajoIni,
                numeroLegajoFin, numeroExpedienteIni, numeroExpedienteFin, fechaLegajoIni, fechaLegajoFin,
                fechaExpedienteIni, fechaExpedienteFin, personaId, estadoId);
            return Ok(expedienteObras);
        }

        public IHttpActionResult GetExpediente(long id)
        {
            return Ok(_unitOfWork.ExpedienteObraRepository.GetExpedienteObraById(id));
        }

        public IHttpActionResult GetLoadExpediente(long id)
        {
            var expedientes = new List<ExpedienteObra>
            {
                _unitOfWork.ExpedienteObraRepository.GetExpedienteObraById(id)
            };
            return Ok(expedientes);
        }

        public IHttpActionResult GetInformePorExpediente(string numero)
        {
            var expedienteObras = _unitOfWork.ExpedienteObraRepository.GetExpedienteObraByNumeroExpediente(numero);
            if (expedienteObras == null)
                return ResponseMessage(new HttpResponseMessage() {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent("No hay datos para el numero de expediente [" + numero + "].")
                });

            return Ok(expedienteObras);
        }

        public IHttpActionResult GetInformePorLegajo(string numero)
        {
            var expedienteObras = _unitOfWork.ExpedienteObraRepository.GetExpedienteObraByNumeroLegajo(numero);
            if (expedienteObras == null)
                return ResponseMessage(new HttpResponseMessage() {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent("No hay datos para el numero de legajo [" + numero + "].")
                });

            return Ok(expedienteObras);
        }

        public IHttpActionResult GetNumeroLegajoSiguiente()
        {
            var legajo = _unitOfWork.ExpedienteObraRepository.GetNumeroLegajoSiguiente();
            return Ok(legajo);
        }

        public IHttpActionResult GetNumeroExpedienteSiguiente()
        {
            var expediente = _unitOfWork.ExpedienteObraRepository.GetNumeroExpedienteSiguiente();
            return Ok(expediente);
        }

        public IHttpActionResult Delete(long idExpedienteObra, long idUsuarioBaja)
        {
            var expedienteObra = _unitOfWork.ExpedienteObraRepository
                .GetExpedienteObraById(idExpedienteObra);

            if (expedienteObra == null) return Ok();

            _unitOfWork.ExpedienteObraRepository.DeleteExpedienteObra(idExpedienteObra, idUsuarioBaja, DateTime.Now);

            _unitOfWork.Save();

            return Ok();
        }

        [Route("api/expedienteobra/cancelall")]
        public IHttpActionResult Get()
        {
            return Ok();
        }

        public IHttpActionResult Validate(UnidadExpedienteObra unidadExpdteObra)
        {
            try
            {
                var expedienteObra = unidadExpdteObra.OperacionExpedienteObra.Item;
                #region Ubicación Primaria
                var ubicaciones = _unitOfWork.DomicilioExpedienteObraRepository.GetDomicilioExpedienteObras(expedienteObra.ExpedienteObraId);

                var ubicacion = unidadExpdteObra.OperacionesDomicilios.FirstOrDefault(x => ((DomicilioExpedienteObra)x.Item).Primario);

                if (ubicaciones != null && (ubicaciones.FirstOrDefault(x => x.Primario) == null) && ubicacion == null)
                    return new TextResult("Ubicación de la Obra: Debe establecer un domicilio primario", Request);
                #endregion
                #region Nro (Legajo & Expediente)
                var expedienteNoLegajo = _unitOfWork.ExpedienteObraRepository.GetExpedienteObraByNumeroLegajo(expedienteObra.NumeroLegajo);
                var expedienteNoExpediente = _unitOfWork.ExpedienteObraRepository.GetExpedienteObraByNumeroExpediente(expedienteObra.NumeroExpediente);

                string errors = FluentValidator<ExpedienteObra>.Validate(new ExpedienteObraValidator(expedienteNoLegajo, expedienteNoExpediente), expedienteObra);
                if (!string.IsNullOrEmpty(errors))
                    return new TextResult(errors, Request);
                #endregion
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/expedienteobra/saveall")]
        public IHttpActionResult Post(UnidadExpedienteObra unidadExpdteObra)
        {
            string inspeccionCollection = null;
            try
            {
                var expedienteObra = unidadExpdteObra.OperacionExpedienteObra.Item;

                #region Expediente Obra
                switch (unidadExpdteObra.OperacionExpedienteObra.Operation)
                {
                    case Operation.Add:
                        _unitOfWork.ExpedienteObraRepository.InsertExpedienteObra(expedienteObra);
                        break;
                    case Operation.Update:
                        _unitOfWork.ExpedienteObraRepository.UpdateExpedienteObra(expedienteObra);
                        break;
                    case Operation.Remove:
                        _unitOfWork.ExpedienteObraRepository.DeleteExpedienteObra(expedienteObra);
                        break;
                }
                #endregion
                #region Unidades Tributarias
                unidadExpdteObra.OperacionesUnidadesTributarias.AnalyzeOperations("UnidadTributariaId");
                foreach (var ut in unidadExpdteObra.OperacionesUnidadesTributarias)
                {
                    switch (ut.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.UnidadTributariaExpedienteObraRepository.InsertUnidadTributariaExpedienteObra(ut.Item, expedienteObra);
                            else
                                _unitOfWork.UnidadTributariaExpedienteObraRepository.InsertUnidadTributariaExpedienteObra(ut.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.UnidadTributariaExpedienteObraRepository.UpdateUnidadTributariaExpedienteObra(ut.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.UnidadTributariaExpedienteObraRepository.DeleteUnidadTributariaExpedienteObra(ut.Item);
                            break;
                    }
                }
                #endregion
                #region Tipos
                unidadExpdteObra.OperacionesTipos.AnalyzeOperations("TipoExpedienteId");
                foreach (var tipo in unidadExpdteObra.OperacionesTipos)
                {
                    switch (tipo.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.TipoExpedienteObraRepository.InsertTipoExpedienteObra(tipo.Item, expedienteObra);
                            else
                                _unitOfWork.TipoExpedienteObraRepository.InsertTipoExpedienteObra(tipo.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.TipoExpedienteObraRepository.UpdateTipoExpedienteObra(tipo.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.TipoExpedienteObraRepository.DeleteTipoExpedienteObra(tipo.Item);
                            break;
                    }
                }
                #endregion
                #region Domicilios
                unidadExpdteObra.OperacionesDomicilios.AnalyzeOperations("DomicilioInmuebleId");
                foreach (var domicilio in unidadExpdteObra.OperacionesDomicilios)
                {
                    switch (domicilio.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.DomicilioExpedienteObraRepository.InsertDomicilioExpedienteObra(domicilio.Item, expedienteObra);
                            else
                                _unitOfWork.DomicilioExpedienteObraRepository.InsertDomicilioExpedienteObra(domicilio.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.DomicilioExpedienteObraRepository.UpdateDomicilioExpedienteObra(domicilio.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.DomicilioExpedienteObraRepository.DeleteDomicilioExpedienteObra(domicilio.Item);
                            break;
                    }
                }
                #endregion
                #region Estados
                unidadExpdteObra.OperacionesEstados.AnalyzeOperations("EstadoExpedienteId");
                foreach (var estado in unidadExpdteObra.OperacionesEstados)
                {
                    switch (estado.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.EstadoExpedienteObraRepository.InsertEstadoExpedienteObra(estado.Item, expedienteObra);
                            else
                                _unitOfWork.EstadoExpedienteObraRepository.InsertEstadoExpedienteObra(estado.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.EstadoExpedienteObraRepository.UpdateEstadoExpedienteObra(estado.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.EstadoExpedienteObraRepository.DeleteEstadoExpedienteObra(estado.Item);
                            break;
                    }
                }
                #endregion
                #region Superficies
                unidadExpdteObra.OperacionesSuperficies.AnalyzeOperations("TipoSuperficieId");
                foreach (var superficie in unidadExpdteObra.OperacionesSuperficies)
                {
                    switch (superficie.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.TipoSuperficieExpedienteObraRepository.InsertTipoSuperficieExpedienteObra(superficie.Item, expedienteObra);
                            else
                                _unitOfWork.TipoSuperficieExpedienteObraRepository.InsertTipoSuperficieExpedienteObra(superficie.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.TipoSuperficieExpedienteObraRepository.UpdateTipoSuperficieExpedienteObra(superficie.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.TipoSuperficieExpedienteObraRepository.DeleteTipoSuperficieExpedienteObra(superficie.Item);
                            break;
                    }
                }
                #endregion
                #region Servicios
                unidadExpdteObra.OperacionesServicios.AnalyzeOperations("ServicioId");
                foreach (var servicio in unidadExpdteObra.OperacionesServicios)
                {
                    switch (servicio.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.ServicioExpedienteObraRepository.InsertServicioExpedienteObra(servicio.Item, expedienteObra);
                            else
                                _unitOfWork.ServicioExpedienteObraRepository.InsertServicioExpedienteObra(servicio.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.ServicioExpedienteObraRepository.UpdateServicioExpedienteObra(servicio.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.ServicioExpedienteObraRepository.DeleteServicioExpedienteObra(servicio.Item);
                            break;
                    }
                }
                #endregion
                #region Documentos
                unidadExpdteObra.OperacionesDocumentos.AnalyzeOperations("DocumentoId");
                foreach (var documento in unidadExpdteObra.OperacionesDocumentos)
                {
                    switch (documento.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.ExpedienteObraDocumentoRepository.InsertExpedienteObraDocumento(documento.Item, expedienteObra);
                            else
                                _unitOfWork.ExpedienteObraDocumentoRepository.InsertExpedienteObraDocumento(documento.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.ExpedienteObraDocumentoRepository.UpdateExpedienteObraDocumento(documento.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.ExpedienteObraDocumentoRepository.DeleteExpedienteObraDocumento(documento.Item);
                            var doc = new Documento { id_documento = documento.Item.DocumentoId, id_usu_baja = expedienteObra.UsuarioModificacionId };
                            _unitOfWork.DocumentoRepository.DeleteDocumento(doc);
                            break;
                    }
                }
                #endregion
                #region Personas
                unidadExpdteObra.OperacionesPersonas.AnalyzeOperations("PersonaInmuebleId", "RolId");
                foreach (var persona in unidadExpdteObra.OperacionesPersonas)
                {
                    switch (persona.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.PersonaExpedienteObraRepository.InsertPersonaExpedienteObra(persona.Item, expedienteObra);
                            else
                                _unitOfWork.PersonaExpedienteObraRepository.InsertPersonaExpedienteObra(persona.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.PersonaExpedienteObraRepository.UpdatePersonaExpedienteObra(persona.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.PersonaExpedienteObraRepository.DeletePersonaExpedienteObra(persona.Item);
                            break;
                    }
                }
                #endregion
                #region Liquidaciones
                unidadExpdteObra.OperacionesLiquidaciones.AnalyzeOperations("LiquidacionId");
                foreach (var liquidacion in unidadExpdteObra.OperacionesLiquidaciones)
                {
                    switch (liquidacion.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.LiquidacionRepository.InsertLiquidacion(liquidacion.Item, expedienteObra, unidadExpdteObra.OperacionesUnidadesTributarias.Where(x => x.Operation == Operation.Add).Select(x => x.Item).FirstOrDefault());
                            else
                                _unitOfWork.LiquidacionRepository.InsertLiquidacion(liquidacion.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.LiquidacionRepository.UpdateLiquidacion(liquidacion.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.LiquidacionRepository.DeleteLiquidacion(liquidacion.Item);
                            break;
                    }
                }
                #endregion
                #region Inspecciones
                unidadExpdteObra.OperacionesInspecciones.AnalyzeOperations("InspeccionId");
                foreach (var inspeccion in unidadExpdteObra.OperacionesInspecciones)
                {
                    switch (inspeccion.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.InspeccionRepository.InsertInspeccion(inspeccion.Item, expedienteObra);
                            else
                                _unitOfWork.InspeccionRepository.InsertInspeccion(inspeccion.Item);

                            inspeccionCollection += inspeccion.Item.InspeccionId + "andAand";
                            break;
                        case Operation.Remove:
                            _unitOfWork.InspeccionRepository.DeleteInspeccion(inspeccion.Item);
                            inspeccionCollection += inspeccion.Item.InspeccionId + "andBand";
                            break;
                    }
                }
                #endregion
                #region Controles Tecnicos
                unidadExpdteObra.OperacionesControlesTecnicos.AnalyzeOperations("ControlTecnicoId");
                foreach (var ctrl in unidadExpdteObra.OperacionesControlesTecnicos)
                {
                    switch (ctrl.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.ControlTecnicoRepository.InsertControlTecnico(ctrl.Item, expedienteObra);
                            else
                                _unitOfWork.ControlTecnicoRepository.InsertControlTecnico(ctrl.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.ControlTecnicoRepository.UpdateControlTecnico(ctrl.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.ControlTecnicoRepository.DeleteControlTecnico(ctrl.Item);
                            break;
                    }
                }
                #endregion
                #region Observaciones
                unidadExpdteObra.OperacionesObservaciones.AnalyzeOperations("ObservacionExpedienteId");
                foreach (var obs in unidadExpdteObra.OperacionesObservaciones)
                {
                    switch (obs.Operation)
                    {
                        case Operation.Add:
                            if (expedienteObra != null)
                                _unitOfWork.ObservacionRepository.InsertObservacion(obs.Item, expedienteObra);
                            else
                                _unitOfWork.ObservacionRepository.InsertObservacion(obs.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.ObservacionRepository.UpdateObservacion(obs.Item);
                            break;
                        case Operation.Remove:
                            _unitOfWork.ObservacionRepository.DeleteObservacion(obs.Item);
                            break;
                    }
                }
                #endregion

                _unitOfWork.Save();
                ExpedienteObraInspecciones response = new ExpedienteObraInspecciones
                {
                    idExpediente = unidadExpdteObra.OperacionExpedienteObra.Item.ExpedienteObraId,
                    Inspecciones = inspeccionCollection != null ? inspeccionCollection.Remove(inspeccionCollection.Length - 3) : ""
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GetCertificadoCatastral(long id)
        {
            return Ok(_unitOfWork.ExpedienteObraRepository.GetCertificadoCatastral(id));
        }
        public IHttpActionResult GetCertificadosCatastral()
        {
            return Ok(_unitOfWork.ExpedienteObraRepository.GetCertificadosCatastral());
        }

        public IHttpActionResult InsertCertificadoCatastral(INMCertificadoCatastral certificadoCatastral)
        {
            try
            {

                _unitOfWork.ExpedienteObraRepository.InsertCertificadoCatastral(certificadoCatastral);
                _unitOfWork.Save();

                return Ok(certificadoCatastral.CertificadoCatastralId);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
