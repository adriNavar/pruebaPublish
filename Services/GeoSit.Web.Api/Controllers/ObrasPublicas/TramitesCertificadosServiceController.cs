using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ValidationRules;
using GeoSit.Web.Api.Common;
using GeoSit.Data.BusinessEntities.ValidationRules.Tramites;
using System.Net.Http;
using Newtonsoft.Json;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers.ObrasPublicas
{
    [RoutePrefix("api/TramitesCertificadosService")]

    public class TramitesCertificadosServiceController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TramitesCertificadosServiceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: ZonaAtributoService
        [ResponseType(typeof(ICollection<TipoTramite>))]
        public IHttpActionResult GetTipos()
        {
            var tipos = new List<TipoTramite>(_unitOfWork.TipoTramiteRepository.GetTiposTramites());

            if (tipos.Count < 1)
            {
                return NotFound();
            }

            return Ok(tipos);
        }

        [ResponseType(typeof(Tramite))]
        public IHttpActionResult ValidateIdentificador(string identificador)
        {
            using (var context = GeoSITMContext.CreateContext())
            {
                var objeto = context.Tramites.FirstOrDefault(s => s.Fecha_Baja == null && s.Cod_Tramite == identificador);

                return Ok(objeto);
            }
        }

        [ResponseType(typeof(ICollection<Tramite>))]
        public IHttpActionResult GetObjetosTramitesCertificados()
        {
            return Ok(_unitOfWork.TramiteRepository.GetTramites().ToList());
        }
        [ResponseType(typeof(ICollection<Tramite>))]
        public IHttpActionResult GetObjetosTramitesCertificadosByCriteria(long? pTipoId, long? pNumDesde, long? pNumHasta, string pFechaDesde, string pFechaHasta, string pEstadoId, int? pUnidadT, int? pIdTramite, string pIdentificador)
        {
            return Ok(_unitOfWork.TramiteRepository.GetTramitesByCriteria(pTipoId, pNumDesde, pNumHasta, pFechaDesde, pFechaHasta, pEstadoId, pUnidadT, pIdTramite, pIdentificador));
        }
        [ResponseType(typeof(Tramite))]
        public IHttpActionResult GetObjetosTramitesCertificados(long idTramite)
        {

            //mTRT_Tramite = db.Tramite.Where(w => w.Id_Tramite == Id_Tramite).Include(b => b.Personas).FirstOrDefault();
            var objeto = _unitOfWork.TramiteRepository.GetTramiteById(idTramite);

            if (objeto == null)
            {
                return NotFound();
            }

            return Ok(objeto);
        }

        public IHttpActionResult GetObjetoMensuraTramite(long idTramite)
        {

            var objeto = _unitOfWork.TramiteRepository.GetTramiteMensura(idTramite);

            if (objeto == null)
            {
                return NotFound();
            }

            return Ok(objeto);
        }

        public IHttpActionResult GetObjetoTramiteUt(long idTramite)
        {

            var objeto = _unitOfWork.TramiteRepository.GetTramiteUt(idTramite);

            if (objeto == null)
            {
                return NotFound();
            }

            return Ok(objeto);
        }

        [ResponseType(typeof(ICollection<TramiteSeccion>))]
        public IHttpActionResult GetObjetosTramitesSecciones(long idTramite)
        {
            try
            {
                var objetos = _unitOfWork.TramiteSeccionRepository.GetTramiteSeccionByTramite(idTramite);

                if (objetos == null)
                {
                    throw new HttpRequestException("No Existen Inspectores seleccionados para la Busqueda.");
                }
                return Ok(objetos);
            }
            catch (HttpRequestException req)
            {
                return ResponseMessage(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(req.Message)
                });
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ResponseType(typeof(ICollection<TramiteRol>))]
        public IHttpActionResult GetRoles()
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                var objetos = db.Tramite_Rol.Where(s => !s.Id_Usu_Baja.HasValue).ToList();

                if (objetos == null)
                {
                    return NotFound();
                }

                return Ok(objetos);
            }

        }

        //[ResponseType(typeof(ICollection<TramitePermisos>))]
        public IHttpActionResult GetPermisosTramiteSaved(long idSeccion, long idTipoTramite)
        {
            using (var _context = GeoSITMContext.CreateContext())
            {
                List<TramitePermisos> objetos = new List<TramitePermisos>();
                if (idSeccion == 0)
                {
                    objetos = _context.Tramite_Permiso.Where(x => x.ID_TIPO_TRAMITE == idTipoTramite && x.FECHA_BAJA == null && x.ID_TIPO_SECCION == 0).ToList();
                }
                else
                {
                    objetos = _context.Tramite_Permiso.Where(x => x.ID_TIPO_SECCION == idSeccion && x.FECHA_BAJA == null).ToList();
                }

                if (objetos.Count() != 0)
                {
                    return Ok(JsonConvert.SerializeObject(objetos));
                }
                else
                {
                    return Ok("");
                }
            }

        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult DeleteTramiteCertificado(Tramite tramite)
        {
            _unitOfWork.TramiteRepository.DeleteTramite(tramite);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveTramite(Tramite trtTramite)
        {
            //var exist = _unitOfWork.TramiteRepository.GetTramiteById(trtTramite.Id_Tramite);
            //trtTramite.TRT_Tipo_Tramite = null;
            ////_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, trtTramite, null);

            return Ok();
        }
        [HttpGet]
        [HttpPost]
        public IHttpActionResult SaveSeccionPermiso(string jsonPost, long idSeccion, long usuarioId, long idTipoTramite)
        {
            List<tableData> list = JsonConvert.DeserializeObject<List<tableData>>(jsonPost);
            using (var _context = GeoSITMContext.CreateContext())
            {
                foreach (var row in list)
                {
                    TramitePermisos permiso;

                    if (idSeccion == 0)
                    {
                        permiso = _context.Tramite_Permiso.Where(x => x.ID_TIPO_TRAMITE == idTipoTramite && x.ID_FUNCION == row.id_funcion && x.FECHA_BAJA == null && x.ID_TIPO_SECCION == 0).FirstOrDefault();
                    }
                    else
                    {
                        permiso = _context.Tramite_Permiso.Where(x => x.ID_TIPO_SECCION == idSeccion && x.ID_FUNCION == row.id_funcion && x.FECHA_BAJA == null).FirstOrDefault();
                    }


                    if (permiso != null)
                    {
                        if (row.estado == false)
                        {
                            //REMOVE
                            //_context.Tramite_Permiso.Remove(permiso);
                            permiso.FECHA_BAJA = DateTime.Now;
                            permiso.ID_USU_BAJA = usuarioId;
                        }
                    }
                    else
                    {
                        if (row.estado == true)
                        {
                            //ADD
                            _context.Tramite_Permiso.Add(new TramitePermisos
                            {
                                ID_TIPO_SECCION = idSeccion,
                                ID_FUNCION = row.id_funcion,
                                FECHA_ALTA = DateTime.Now,
                                ID_USU_ALTA = usuarioId,
                                FECHA_MODIF = DateTime.Now,
                                ID_USU_MODIF = usuarioId,
                                FECHA_BAJA = null,
                                ID_USU_BAJA = null,
                                ID_TIPO_TRAMITE = idTipoTramite
                            });
                        }
                    }
                }
                _context.SaveChanges();
            }
            return Ok();
        }

        //PERSONAS
        [ResponseType(typeof(ICollection<TramitePersona>))]
        public IHttpActionResult GetObjetosTramitesPersonas(long idTramite)
        {
            var objetos = _unitOfWork.TramitePersonaRepository.GetTramitePersonaByTramite(idTramite);

            if (objetos == null)
            {
                return NotFound();
            }

            return Ok(objetos);
        }

        [ResponseType(typeof(ICollection<TramitePersona>))]
        public IHttpActionResult GetObjetoTramitePersona(long idTramitePersona)
        {
            var objetos = _unitOfWork.TramitePersonaRepository.GetTramitePersonaById(idTramitePersona);

            if (objetos == null)
            {
                return NotFound();
            }

            return Ok(objetos);
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult DeleteTramitePersona(long idTramitePersona)
        {
            var persona = _unitOfWork.TramitePersonaRepository.GetTramitePersonaById(idTramitePersona);
            //_saveObjects.Add(Operation.Remove, null, persona ?? new TRT_Tramite_Persona() { Id_Tramite_Persona = idTramitePersona });

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SavePersona(TramitePersona trtTramitePersona)
        {
            var exist = _unitOfWork.TramitePersonaRepository.GetTramitePersonaById(trtTramitePersona.Id_Tramite_Persona);
            trtTramitePersona.Persona = null;
            trtTramitePersona.Rol = null;
            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, trtTramitePersona);

            return Ok();
        }

        //UNIDADES TRIBUTARIAS
        [ResponseType(typeof(ICollection<TramiteUnidadTributaria>))]
        public IHttpActionResult GetObjetosTramitesUTS(long idTramite)
        {

            var objetos = _unitOfWork.TramiteUnidadTributariaRepository.GetTramiteUTSByTramite(idTramite);

            if (objetos == null)
            {
                return NotFound();
            }

            return Ok(objetos);

        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult DeleteTramiteUTS(long idTramiteUts)
        {
            var unidadtributaria = _unitOfWork.TramiteUnidadTributariaRepository.GetTramiteUTSById(idTramiteUts);
            //_saveObjects.Add(Operation.Remove, null, unidadtributaria ?? new TRT_Tramite_UTS() { Id_Tramite_Uts = idTramiteUts });

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveUTS(TramiteUnidadTributaria trtTramiteUts)
        {
            //var exist = _unitOfWork.TramitePersonaRepository.GetTramitePersonaById(trtTramiteUts.Id_Tramite_Uts);
            var exist = _unitOfWork.TramiteUnidadTributariaRepository.GetTramiteUTSById(trtTramiteUts.Id_Tramite_Uts);
            trtTramiteUts.UnidadTributaria = null;
            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, trtTramiteUts);

            return Ok();
        }

        //DOCUMENTOS
        [ResponseType(typeof(ICollection<TramiteDocumento>))]
        public IHttpActionResult GetObjetosTramitesDocumentos(long idTramite)
        {

            var objetos = _unitOfWork.TramiteDocumentoRepository.GetTramiteDocumentoByTramite(idTramite);

            if (objetos == null)
            {
                return NotFound();
            }

            return Ok(objetos);

        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult DeleteTramiteDoc(long idTramiteDoc)
        {
            var documento = _unitOfWork.TramiteDocumentoRepository.GetTramiteDocumentoById(idTramiteDoc);
            //_saveObjects.Add(Operation.Remove, null, documento ?? new TRT_Tramite_Documento() { Id_Tramite_Documento = idTramiteDoc });

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveDocumento(TramiteDocumento trtTramiteDoc)
        {
            var exist = _unitOfWork.TramiteDocumentoRepository.GetTramiteDocumentoById(trtTramiteDoc.Id_Tramite_Documento);
            trtTramiteDoc.Documento = null;
            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, trtTramiteDoc);

            return Ok();
        }

        [ResponseType(typeof(ICollection<TramiteTipoSeccion>))]
        public IHttpActionResult GetTiposSecciones(long tipoTraId)
        {
            using (var context = GeoSITMContext.CreateContext())
            {
                var objetos = context.Tipo_Seccion.Where(s => s.Fecha_Baja == null && s.Id_Tipo_Tramite == tipoTraId).ToList();

                return Ok(objetos);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveSeccion(TramiteSeccion trtTramiteSec)
        {
            var exist = _unitOfWork.TramiteSeccionRepository.GetTramiteSeccionById(trtTramiteSec.Id_Tramite_Seccion);
            trtTramiteSec.TipoSeccion = null;
            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, trtTramiteSec);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveAll(UnidadGeneracionCertificados unidadGeneracionCertificados)
        {
            try
            {
                var tramite = unidadGeneracionCertificados.OperacionTramite.Item;

                #region Tramite
                switch (unidadGeneracionCertificados.OperacionTramite.Operation)
                {
                    case Operation.Add:
                        _unitOfWork.TramiteRepository.InsertTramite(tramite);
                        var tipo = _unitOfWork.TipoTramiteRepository.GetTipoTramiteById(tramite.Id_Tipo_Tramite);
                        if (tipo.Autonumerico)
                        {
                            tipo.Numerador++;
                            _unitOfWork.TipoTramiteRepository.UpdateTipoTramite(tipo);
                        }
                        break;
                    case Operation.Update:
                        _unitOfWork.TramiteRepository.UpdateTramite(tramite);
                        break;
                    case Operation.Remove:
                        _unitOfWork.TramiteRepository.DeleteTramite(tramite);
                        break;
                }
                #endregion
                #region Unidades Tributarias
                unidadGeneracionCertificados.OperacionesUnidadesTributarias.AnalyzeOperations("Id_Unidad_Tributaria");
                foreach (var ut in unidadGeneracionCertificados.OperacionesUnidadesTributarias)
                {
                    switch (ut.Operation)
                    {
                        case Operation.Add:
                            ut.Item.Id_Usu_Alta = tramite.Id_Usu_Modif;
                            _unitOfWork.TramiteUnidadTributariaRepository.InsertTramiteUTS(ut.Item);
                            break;
                        case Operation.Remove:
                            ut.Item.Id_Usu_Baja = tramite.Id_Usu_Modif;
                            _unitOfWork.TramiteUnidadTributariaRepository.DeleteTramiteUTS(ut.Item);
                            break;
                    }
                }
                #endregion
                #region Documentos
                unidadGeneracionCertificados.OperacionesDocumentos.AnalyzeOperations("Id_Documento");
                foreach (var documento in unidadGeneracionCertificados.OperacionesDocumentos)
                {
                    switch (documento.Operation)
                    {
                        case Operation.Add:
                            documento.Item.Id_Usu_Alta = tramite.Id_Usu_Modif;
                            _unitOfWork.TramiteDocumentoRepository.InsertTramiteDocumento(documento.Item);
                            break;
                        case Operation.Remove:
                            documento.Item.Id_Usu_Baja = tramite.Id_Usu_Modif;
                            _unitOfWork.TramiteDocumentoRepository.DeleteTramiteDocumento(documento.Item);
                            var doc = new Documento { id_documento = documento.Item.Id_Documento, id_usu_baja = tramite.Id_Usu_Modif };
                            _unitOfWork.DocumentoRepository.DeleteDocumento(doc);
                            break;
                    }
                }
                #endregion
                #region Personas
                unidadGeneracionCertificados.OperacionesPersonas.AnalyzeOperations("Id_Persona", "Id_Rol");
                foreach (var persona in unidadGeneracionCertificados.OperacionesPersonas)
                {
                    switch (persona.Operation)
                    {
                        case Operation.Add:
                            persona.Item.Id_Usu_Alta = tramite.Id_Usu_Modif;
                            _unitOfWork.TramitePersonaRepository.InsertTramitePersona(persona.Item);
                            break;
                        case Operation.Remove:
                            persona.Item.Id_Usu_Baja = tramite.Id_Usu_Modif;
                            _unitOfWork.TramitePersonaRepository.DeleteTramitePersona(persona.Item);
                            break;
                    }
                }
                #endregion
                #region Secciones
                foreach (var seccion in unidadGeneracionCertificados.OperacionesSecciones)
                {
                    seccion.Item.Id_Usu_Modif = tramite.Id_Usu_Modif;
                    switch (seccion.Operation)
                    {
                        case Operation.Add:
                            _unitOfWork.TramiteSeccionRepository.InsertTramiteSeccion(seccion.Item);
                            break;
                        case Operation.Update:
                            _unitOfWork.TramiteSeccionRepository.UpdateTramiteSeccion(seccion.Item);
                            break;
                    }
                }
                #endregion
                #region InformeImpreso
                if (unidadGeneracionCertificados.InformeImpreso != null && tramite.Estado == "4")
                {
                    EliminateDocumentRows(tramite.Id_Tramite);
                    Documento documento = _unitOfWork.DocumentoRepository.InsertDocumento(unidadGeneracionCertificados.InformeImpreso.Item);
                    var unidadesTributarias = _unitOfWork.TramiteUnidadTributariaRepository.GetTramiteUTSByTramite(tramite.Id_Tramite);
                    foreach (var ut in unidadesTributarias)
                    {
                        var unidadDocumento = new UnidadTributariaDocumento();
                        unidadDocumento.Documento = documento;
                        unidadDocumento.FechaAlta = System.DateTime.Now;
                        unidadDocumento.FechaModificacion = System.DateTime.Now;
                        unidadDocumento.UsuarioAltaID = documento.id_usu_alta;
                        unidadDocumento.UsuarioModificacionID = documento.id_usu_modif;
                        unidadDocumento.UnidadTributariaID = ut.Id_Unidad_Tributaria;

                        _unitOfWork.UnidadTributariaDocumentoRepository.InsertUnidadTributariaDocumento(unidadDocumento);
                    }
                    var tramiteDocumento = new TramiteDocumento();
                    tramiteDocumento.Tramite = tramite;
                    tramiteDocumento.Documento = documento;
                    tramiteDocumento.Fecha_Alta = System.DateTime.Now;
                    tramiteDocumento.Fecha_Modif = System.DateTime.Now;
                    tramiteDocumento.Id_Usu_Alta = documento.id_usu_alta;
                    tramiteDocumento.Id_Usu_Modif = documento.id_usu_modif;
                    _unitOfWork.TramiteDocumentoRepository.InsertTramiteDocumento(tramiteDocumento);
                }
                #endregion
                _unitOfWork.Save();
                return Ok(tramite);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GetTramitePersonaByIdTramiteIdPersona(long idTramite, long idPersona, long idRol)
        {
            try
            {
                var persona = _unitOfWork.TramitePersonaRepository.GetTramitePersonaByTramite(idTramite)
                                         .SingleOrDefault(p => p.Id_Persona == idPersona && p.Id_Rol == idRol && !p.Id_Usu_Baja.HasValue);
                return Ok(persona);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GetTramiteUnidadTributariaByIdTramiteIdUnidadTributaria(long idTramite, long idUnidadTributaria)
        {
            try
            {
                var unidadT = _unitOfWork.TramiteUnidadTributariaRepository.GetTramiteUTSByTramite(idTramite).SingleOrDefault(p => p.Id_Unidad_Tributaria == idUnidadTributaria && !p.Id_Usu_Baja.HasValue);
                return Ok(unidadT);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GetTramiteDocumentoByIdTramiteIdDocumento(long idTramite, long idDocumento)
        {
            try
            {
                var documento = _unitOfWork.TramiteDocumentoRepository.GetTramiteDocumentoByTramite(idTramite).SingleOrDefault(p => p.Id_Documento == idDocumento && !p.Id_Usu_Baja.HasValue);
                return Ok(documento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult ValidateDocumento(long idTramite, long idDocumento, Operaciones<TramiteDocumento> operaciones)
        {
            operaciones.AnalyzeOperations("Id_Documento");
            var tramiteDocumento = new TramiteDocumento
            {
                Id_Tramite = idTramite,
                Id_Documento = idDocumento
            };
            /*chequeo contra db*/
            var documento = _unitOfWork.TramiteDocumentoRepository.GetTramiteDocumentoByTramite(tramiteDocumento.Id_Tramite)
                                       .SingleOrDefault(t => t.Id_Documento == tramiteDocumento.Id_Documento && !t.Fecha_Baja.HasValue);

            string errors = FluentValidator<TramiteDocumento>.Validate(new TramiteDocumentoValidator(documento), tramiteDocumento);
            if (!string.IsNullOrEmpty(errors)) return new TextResult(errors, Request);

            /*chequeo contra modificaciones actuales*/
            var item = operaciones.FirstOrDefault(p => p.Item.Id_Tramite == tramiteDocumento.Id_Tramite &&
                                                       p.Item.Id_Documento == tramiteDocumento.Id_Documento);
            if (item != null)
            {
                errors = FluentValidator<TramiteDocumento>.Validate(new TramiteDocumentoValidator(item.Item), tramiteDocumento);
            }
            if (!string.IsNullOrEmpty(errors)) return new TextResult(errors, Request);
            return Ok();
        }

        public IHttpActionResult ValidatePersona(long idTramite, long idPersona, int idRol, Operaciones<TramitePersona> operaciones)
        {
            operaciones.AnalyzeOperations("Id_Persona", "Id_Rol");
            var tramitePersona = new TramitePersona
            {
                Id_Tramite = idTramite,
                Id_Persona = idPersona,
                Id_Rol = idRol
            };
            /*chequeo contra db*/
            var persona = _unitOfWork.TramitePersonaRepository
                                     .GetTramitePersonaByTramite(tramitePersona.Id_Tramite)
                                     .SingleOrDefault(t => t.Id_Persona == tramitePersona.Id_Persona &&
                                                          t.Id_Rol == tramitePersona.Id_Rol &&
                                                          !t.Fecha_Baja.HasValue);

            var errors = FluentValidator<TramitePersona>.Validate(new TramitePersonaValidator(persona), tramitePersona);
            if (errors.Any()) return new TextResult(errors, Request);

            /*chequeo contra modificaciones actuales*/
            var item = operaciones.FirstOrDefault(p => p.Item.Id_Tramite == tramitePersona.Id_Tramite &&
                                                       p.Item.Id_Persona == tramitePersona.Id_Persona &&
                                                       p.Item.Id_Rol == tramitePersona.Id_Rol);
            if (item != null)
            {
                errors = FluentValidator<TramitePersona>.Validate(new TramitePersonaValidator(item.Item), tramitePersona);
            }
            if (errors.Any()) return new TextResult(errors, Request);

            return Ok();
        }

        public IHttpActionResult ValidateUnidadTributaria(long idTramite, long idUnidadTributaria, Operaciones<TramiteUnidadTributaria> operaciones)
        {
            operaciones.AnalyzeOperations("Id_Unidad_Tributaria");
            var tramiteUT = new TramiteUnidadTributaria
            {
                Id_Tramite = idTramite,
                Id_Unidad_Tributaria = idUnidadTributaria
            };
            /*chequeo contra db*/
            var ut = _unitOfWork.TramiteUnidadTributariaRepository.GetTramiteUTSByTramite(tramiteUT.Id_Tramite)
                                .SingleOrDefault(t => t.Id_Unidad_Tributaria == tramiteUT.Id_Unidad_Tributaria && !t.Fecha_Baja.HasValue);

            var errors = FluentValidator<TramiteUnidadTributaria>.Validate(new TramiteUnidadTributariaValidator(ut), tramiteUT);
            if (errors.Any()) return new TextResult(errors, Request);

            ///*chequeo contra modificaciones actuales*/
            var operacion = operaciones.FirstOrDefault(x => x.Item.Id_Tramite == tramiteUT.Id_Tramite &&
                                                            x.Item.Id_Unidad_Tributaria == tramiteUT.Id_Unidad_Tributaria);

            ut = operacion != null ? operacion.Item : null;

            errors = FluentValidator<TramiteUnidadTributaria>.Validate(new TramiteUnidadTributariaValidator(ut), tramiteUT);
            if (errors.Any()) return new TextResult(errors, Request);
            return Ok();
        }

        public IHttpActionResult Validate(UnidadGeneracionCertificados unidadGeneracionCertificado)
        {
            try
            {
                var tramite = unidadGeneracionCertificado.OperacionTramite.Item;
                #region Codigo de Tramite
                var existente = _unitOfWork.TramiteRepository.GetCertificadoByTipoCodigo(tramite.Id_Tipo_Tramite, tramite.Cod_Tramite);

                var errors = FluentValidator<Tramite>.Validate(new TramiteValidator(existente), tramite);
                if (errors.Any())
                    return new TextResult(errors, Request);
                #endregion
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public void EliminateDocumentRows(long idTramite)
        {
            var documentos = _unitOfWork.TramiteDocumentoRepository.GetTramiteDocumentoByTramite(idTramite);

            foreach (var doc in documentos)
            {
                var documentoData = _unitOfWork.DocumentoRepository.GetDocumentoById(doc.Id_Documento);

                if (documentoData.descripcion == "Informe de Trámite" && documentoData.nombre_archivo == "InformeTramite.pdf")
                {
                    _unitOfWork.TramiteDocumentoRepository.DeleteTramiteDocumento(doc);
                    _unitOfWork.DocumentoRepository.DeleteDocumento(documentoData);
                }
            }
        }
    }

    public class tableData
    {
        public long id_funcion { get; set; }
        public string descripcion { get; set; }
        public bool estado { get; set; }
    }
}