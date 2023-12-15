using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.ValidacionesDB;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using static GeoSit.Data.DAL.Repositories.MesaEntradasRepository;

namespace GeoSit.Web.Api.Controllers.MesaEntradas
{
    public class MesaEntradasController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        //private readonly InterfaseRentasHelper _interfaseRentasHelper;

        public MesaEntradasController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //_interfaseRentasHelper = new InterfaseRentasHelper(unitOfWork);
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/enproceso")]
        public IHttpActionResult TramitesEnProceso(long idUsuario, DataTableParameters parametros)
        {

            return recuperarTramites(Grilla.EnProceso, parametros, idUsuario);
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/pendientes")]
        public IHttpActionResult TramitesPendientes(long idUsuario, DataTableParameters parametros)
        {
            return recuperarTramites(Grilla.Pendientes, parametros, idUsuario);
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/procesados")]
        public IHttpActionResult TramitesProcesados(long idUsuario, DataTableParameters parametros)
        {
            return recuperarTramites(Grilla.Procesados, parametros, idUsuario);
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/reasignados")]
        public IHttpActionResult TramitesReasignados(long idUsuario, DataTableParameters parametros)
        {
            return recuperarTramites(Grilla.Reasignados, parametros, idUsuario);
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/todos")]
        public IHttpActionResult TramitesTodos(long idUsuario, DataTableParameters parametros)
        {

            return recuperarTramites(Grilla.Todos, parametros, idUsuario);
        }

        private IHttpActionResult recuperarTramites(Grilla grilla, DataTableParameters parametros, long idUsuario = 0)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).RecuperarTramites(grilla, parametros, idUsuario));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<METipoTramite>))]
        [Route("api/mesaentradas/tramites/tipos")]
        public IHttpActionResult GetTiposTramites()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.TiposTramites.Where(x => x.FechaBaja == null).OrderBy(x => x.Descripcion).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<METipoTramite>))]
        [Route("api/mesaentradas/tramites/tiposbyid")]
        public IHttpActionResult GetTiposTramitesById(int id)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.TiposTramites
                        .Include("PrioridadesTipos")
                        .Include("PrioridadesTipos.PrioridadTramite")
                        .Single(t => t.IdTipoTramite == id));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Trámites por Id", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<MEObjetoTramite>))]
        [Route("api/mesaentradas/tramites/objetos")]
        public IHttpActionResult GetObjetosTramites()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var query = from objetoTramite in db.ObjetosTramites
                                where objetoTramite.FechaBaja == null
                                orderby objetoTramite.Descripcion.Substring(0, 1) == "-" ? 1 : 999, objetoTramite.Descripcion
                                select objetoTramite;
                    return Ok(query.ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Objetos Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<MEObjetoTramite>))]
        [Route("api/mesaentradas/tramites/objetosbytipotramite")]
        public IHttpActionResult GetObjetosTramitesByTipoTramite(int idTipoTramite)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.ObjetosTramites.Where(ot => ot.IdTipoTramite == idTipoTramite).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Objetos Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<MEEstadoTramite>))]
        [Route("api/mesaentradas/tramites/estados")]
        public IHttpActionResult GetEstadosTramites()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.EstadosTramites.OrderBy(x => x.Descripcion).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Estados Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<MEPrioridadTramite>))]
        [Route("api/mesaentradas/tramites/prioridades")]
        public IHttpActionResult GetPrioridadesTramites()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.PrioridadesTramites.Where(x => x.FechaBaja == null).OrderBy(x => x.Descripcion).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Prioridades Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<Objeto>))]
        [Route("api/mesaentradas/tramites/desglosesdestinos")]
        public IHttpActionResult GetDesglosesDestinos()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.DesglosesDestinos.OrderBy(d => d.Descripcion).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Jurisdicciones", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<Objeto>))]
        [Route("api/mesaentradas/tramites/jurisdicciones")]
        public IHttpActionResult GetJurisdicciones()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var param = db.ParametrosGenerales.ToList().Where(x => x.Clave == "ID_TIPO_OBJETO_JURISDICCION").FirstOrDefault();
                    long idTipoObjeto = param != null ? long.Parse(param.Valor) : 13;
                    return Ok(db.Objetos.Where(o => o.TipoObjetoId == idTipoObjeto && o.FechaBaja == null).OrderBy(x => x.Codigo).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Jurisdicciones", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<Objeto>))]
        [Route("api/mesaentradas/tramites/localidades")]
        public IHttpActionResult GetLocalidades()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var param = db.ParametrosGenerales.ToList().Where(x => x.Clave == "ID_TIPO_OBJETO_LOCALIDAD").FirstOrDefault();
                    long idTipoObjeto = param != null ? long.Parse(param.Valor) : 14;
                    return Ok(db.Objetos.Where(o => o.TipoObjetoId == idTipoObjeto && o.FechaBaja == null).OrderBy(x => x.Nombre).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Localidades", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<Objeto>))]
        [Route("api/mesaentradas/tramites/localidadesByJurisdiccion")]
        public IHttpActionResult GetLocalidadesByJurisdiccion(int idJurisdiccion)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var param = db.ParametrosGenerales.ToList().Where(x => x.Clave == "ID_TIPO_OBJETO_LOCALIDAD").FirstOrDefault();
                    long idTipoObjeto = param != null ? long.Parse(param.Valor) : 14;
                    var objetoPadreId = db.Objetos.FirstOrDefault(x => x.FeatId == idJurisdiccion)?.ObjetoPadreId ?? 0;
                    return Ok(db.Objetos.Where(o => o.FechaBaja == null && o.ObjetoPadreId == objetoPadreId && o.TipoObjetoId == idTipoObjeto).OrderBy(x => x.Nombre).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Localidades", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(List<string>))]
        [Route("api/mesaentradas/tramites/{id}/acciones")]
        public IHttpActionResult GetAccionesByTramite(long id, long idUsuario, bool grillaReasignable)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetAccionesByTramite(id, idUsuario, grillaReasignable));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Acciones del Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [Route("api/mesaentradas/tramites/{idTramite}/usuario/{idUsuario}/acciones")]
        public IHttpActionResult GetAccionesTramiteByUsuarioEdicion(long idTramite, long idUsuario)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetAccionesNoMovimientosUsuarioEdicion(idTramite, idUsuario));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Acciones del Trámite", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/AccionesByTramites")]
        public IHttpActionResult AccionesByTramites(long idUsuario, long[] tramites, bool grillaReasignable)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetAccionesByTramites(tramites, idUsuario, grillaReasignable));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Acciones del Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/mesaentradas/tramites/AccionesGenerales")]
        public IHttpActionResult AccionesGenerales(long idUsuario, int cantTramites, bool grillaReasignable)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetAccionesGenerales(cantTramites, idUsuario, grillaReasignable));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Acciones del Trámites", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/EjecutarAccion")]
        public IHttpActionResult EjecutarAccion(long idUsuario, AccionParameters accionParameters)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    //bool ok = new MesaEntradasRepository(db).EjecutarAccion(idUsuario, accionParameters);
                    //if (ok && (EnumTipoMovimiento)int.Parse(accionParameters.accion) == EnumTipoMovimiento.Finalizar)
                    //{
                    //    SolrUpdater.Instance.Enqueue(Entities.mensura);
                    //    SolrUpdater.Instance.Enqueue(Entities.parcela);
                    //    SolrUpdater.Instance.Enqueue(Entities.parcelahistorica);
                    //    SolrUpdater.Instance.Enqueue(Entities.prescripcion);
                    //    SolrUpdater.Instance.Enqueue(Entities.unidadtributaria);
                    //    SolrUpdater.Instance.Enqueue(Entities.unidadtributariahistorica);
                    //    SolrUpdater.Instance.Enqueue(Entities.manzana);
                    //    SolrUpdater.Instance.Enqueue(Entities.calle);
                    //}
                    return Ok(new MesaEntradasRepository(db).EjecutarAccion(idUsuario, accionParameters));
                }
            }
            catch (ValidacionException ex)
            {
                var statusCode = HttpStatusCode.ExpectationFailed;
                switch (ex.ErrorValidacion)
                {
                    case ResultadoValidacion.Advertencia:
                        statusCode = HttpStatusCode.Conflict;
                        break;
                    case ResultadoValidacion.Bloqueo:
                        statusCode = HttpStatusCode.PreconditionFailed;
                        break;
                    default: //error
                        break;
                }
                return Content(statusCode, ex.Errores.ToList());
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Ejecutar Acción", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(METramite))]
        [Route("api/mesaentradas/tramites/{id}")]
        public IHttpActionResult GetTramiteById(int id, bool includeEntradas = true)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetTramiteById(id, includeEntradas));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Trámite", ex);
                return InternalServerError(ex);
            }
        }

        [Route("api/mesaentradas/tramites/{tramite}/entradas")]
        public IHttpActionResult GetEntradasByIdTramite(int tramite)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetEntradasByTramite(tramite));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"Recuperar Entradas del Trámite ({tramite})", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/save")]
        public IHttpActionResult TramiteSave(bool esConfirmacion, bool esPresentacion, bool esReingresar, METramiteParameters tramiteParameters)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).TramiteSave(esConfirmacion, esPresentacion, esReingresar, tramiteParameters));
                }
            }
            catch (ValidacionTramiteException ex)
            {
                var statusCode = HttpStatusCode.ExpectationFailed;
                switch (ex.ErrorValidacion)
                {
                    case ResultadoValidacion.Advertencia:
                        statusCode = HttpStatusCode.Conflict;
                        break;
                    case ResultadoValidacion.Bloqueo:
                        statusCode = HttpStatusCode.PreconditionFailed;
                        break;
                    default: //error
                        break;
                }
                return Content(statusCode, ex);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Tramite Save", ex);
                return InternalServerError(ex);
            }
        }

        //[HttpPost]
        //[Route("api/mesaentradas/tramites/confirmar")]
        //public IHttpActionResult TramiteConfirmar(long idUsuario, METramite tramite)
        //{
        //    try
        //    {
        //        using (var db = GeoSITMContext.CreateContext())
        //        {
        //            METramite tramiteActual = db.TramitesMesaEntrada.Include("TramiteRequisitos").Single(t => t.IdTramite == tramite.IdTramite);
        //            List<MERequisito> requisitos = db.Requisitos.Where(r => r.Obligatorio == 1).ToList();
        //            var requisitosObligFaltantes = requisitos.Where(r => !tramiteActual.TramiteRequisitos.Any(tr => tr.IdTramiteRequisito == r.IdRequisitoTramite));
        //            if (requisitosObligFaltantes != null && requisitosObligFaltantes.Count() > 0)
        //            {
        //                return Ok(2);
        //            }
        //            else
        //            {
        //                MesaEntradasRepository mesaEntradasRepository = new MesaEntradasRepository(db);
        //                return Ok(new MesaEntradasRepository(db).TramiteConfirmar(idUsuario, tramite));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Global.GetLogger().LogError("Tramite Confirmar", ex);
        //        return InternalServerError(ex);
        //    }
        //}

        [ResponseType(typeof(ICollection<ParametrosGenerales>))]
        [Route("api/mesaentradas/tramites/parametrosgenerales")]
        public IHttpActionResult GetParametrosGenerales()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.ParametrosGenerales.ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Parametros Generales", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/desgloses/save")]
        public IHttpActionResult DesgloseSave(long idUsuario, MEDesglose desglose)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    MesaEntradasRepository mesaEntradasRepository = new MesaEntradasRepository(db);
                    return Ok(new MesaEntradasRepository(db).DesgloseSave(idUsuario, desglose));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Desglose Save", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<METramiteDocumento>))]
        [Route("api/mesaentradas/tramites/{id}/TramitesDocumentos")]
        public IHttpActionResult GetAllTramiteDocumentoByTramite(int id)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetAllTramiteDocumentoByTramite(id));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Trámite Documentos por Tramite", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/MovimientosRemito")]
        public IHttpActionResult GetMovimientosRemito(MERemitoParameters remitoParameters)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    MesaEntradasRepository mesaEntradasRepository = new MesaEntradasRepository(db);
                    return Ok(new MesaEntradasRepository(db).GetMovimientosRemito(remitoParameters));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Movimientos para Remito", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/MovimientosByIds")]
        public IHttpActionResult GetMovimientosByIds(int[] movimientos)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetMovimientosByIds(movimientos));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Movimientos por ids", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/remitos/save")]
        public IHttpActionResult RemitoSave(MERemitoParameters remitoParameters)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    MesaEntradasRepository mesaEntradasRepository = new MesaEntradasRepository(db);
                    return Ok(new MesaEntradasRepository(db).RemitoSave(remitoParameters));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Remito Save", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(MERemito))]
        [Route("api/mesaentradas/remitos/{id}")]
        public IHttpActionResult GetRemitoById(int id)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetRemitoById(id));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Remito", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(MERemito))]
        [Route("api/mesaentradas/remitos/RemitoByNumero")]
        public IHttpActionResult GetRemitoByNumero(string numero)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).GetRemitoByNumero(numero));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Remito por numero", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [ResponseType(typeof(ICollection<METramite>))]
        [Route("api/mesaentradas/tramites/tramitesbyfiltro")]
        public IHttpActionResult TramitesByFiltro(Dictionary<string, string> valores)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).RecuperarTramitesByFiltro(valores));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tramites por Filtro", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<METramite>))]
        [Route("api/mesaentradas/tramites/tramitespendientesconfirmar")]
        public IHttpActionResult TramitesPendientesConfirmar()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).RecuperarTramitesPendientesConfirmar());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tramites pendientes de Confirmar", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/abrirlibrodiario")]
        public IHttpActionResult AbrirLibroDiario(MELibroDiarioParameters libroDiarioParameters)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).AbrirLibroDiario(libroDiarioParameters));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Abrir Libro Diario", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/mesaentradas/tramites/cerrarlibrodiario")]
        public IHttpActionResult CerrarLibroDiario(MELibroDiarioParameters libroDiarioParameters)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).CerrarLibroDiario(libroDiarioParameters));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Cerrar Libro Diario", ex);
                return InternalServerError(ex);
            }
        }


        [ResponseType(typeof(MEEntrada))]
        [Route("api/mesaentradas/entrada/{id}/")]
        public IHttpActionResult GetEntrada(int id)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.Entradas.Find(id));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Entrada", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(MEEntrada))]
        [Route("api/mesaentradas/entradaByIdComponente/{idComponente}/")]
        public IHttpActionResult GetEntradaByIdComponente(int idComponente)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.Entradas.FirstOrDefault(t => t.IdComponente == idComponente));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Entrada", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<MEEntrada>))]
        [Route("api/mesaentradas/GetEntradasByObjeto")]
        public IHttpActionResult GetEntradas(int idObjetoTramite)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var ids = db.ObjetosEntrada.Where(x => x.IdObjetoTramite == idObjetoTramite).Select(x => x.IdEntrada).Distinct().ToList();
                    return Ok(db.Entradas.Where(x => ids.Contains(x.IdEntrada)).ToList());
                    //return Ok(db.Entradas.ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Entradas", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/DDJJACopiar")]
        public IHttpActionResult GetDDJJACopiar()
        {
            try
            {
                return Ok(GetFromView("VW_DDJJ", "id_ddjj", "tipo_ddjj"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Entradas", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/TiposUF")]
        public IHttpActionResult GetTiposUF()
        {
            try
            {
                return Ok(GetFromView("INM_TIPO_UT", "id_tipo_ut", "descripcion"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos UT", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/TiposPersona")]
        public IHttpActionResult GetTiposPersona()
        {
            try
            {
                return Ok(GetFromView("INM_TIPO_PERSONA", "id_tipo_persona", "descripcion"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Persona", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/TiposEnteEmisor")]
        public IHttpActionResult GetTiposEnteEmisor()
        {
            try
            {
                return Ok(GetFromView("INM_ENTE_EMISOR", "id_ente_emisor", "descripcion", "fecha_baja is null"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Persona", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/TiposTitulos")]
        public IHttpActionResult GetTiposTitulo()
        {
            try
            {
                return Ok(GetFromView("INM_TIPO_INSCRIPCION", "id_tipo_inscripcion", "descripcion"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Titulo", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/TiposTasa")]
        public IHttpActionResult GetTiposTasa()
        {
            try
            {
                return Ok(GetFromView("ME_TIPO_TASA", "id_tipo_tasa", "descripcion"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Titulo", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/TiposTitularidad")]
        public IHttpActionResult GetTiposTitularidad()
        {
            try
            {
                return Ok(GetFromView("inm_tipo_titularidad", "id_tipo_titularidad", "descripcion"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Titularidad", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<SelectListItem>))]
        [Route("api/mesaentradas/TiposVia")]
        public IHttpActionResult GetTiposVia()
        {
            try
            {
                return Ok(GetFromView("grf_tipo_via", "id_tipo_via", "descripcion"));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Tipos Titularidad", ex);
                return InternalServerError(ex);
            }
        }

        private List<SelectListItem> GetFromView(string viewName, string idField, string descField, string whereClause = null, int limit = 0)
        {
            using (var qbuilder = GeoSITMContext.CreateContext().CreateSQLQueryBuilder())
            {
                qbuilder.AddTable(viewName, null)
                        .AddFields(idField, descField);


                if (!string.IsNullOrEmpty(whereClause))
                {
                    qbuilder.AddRawFilter(whereClause);
                }
                if (limit > 0)
                {
                    qbuilder.MaxResults(limit);
                }

                return qbuilder.ExecuteQuery(reader =>
                {
                    return new SelectListItem()
                    {
                        Value = reader.GetStringOrEmpty(0),
                        Text = reader.GetStringOrEmpty(1)
                    };
                }).ToList();
            }
        }

        public class SelectListItem
        {
            public string Text { get; set; }

            public string Value { get; set; }
        }


        [HttpGet]
        public IHttpActionResult GetExpedientesDDJJByIdUnidadTributaria(long idUnidadTributaria)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(new MesaEntradasRepository(db).ObtenerExpedientesDDJJByIdUnidadTributaria(idUnidadTributaria));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("ObtenerExpedientesDDJJByIdUnidadTributarias", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetNextMensura(long idDepartamento)
        {
            try
            {
                string[] newMesura;

                using (var db = GeoSITMContext.CreateContext())
                {
                    newMesura = new MesaEntradasRepository(db).GenerarMensura(idDepartamento);
                }

                return Ok(newMesura.ToList());

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Generar Mensura Siguiente", ex);
                return InternalServerError(ex);
            }
        }


        //[Route("api/mesaentradas/GenerarTramite")]
        //public IHttpActionResult GenerarTramite(string[] valores)
        //{

        //    if (valores != null)
        //    {

        //        using (var db = GeoSITMContext.CreateContext())
        //        {
        //            if (valores.First().ToString() == "Partida")
        //            {
        //                return Ok(new MesaEntradasRepository(db).GenerarPartidaInmobiliaria(valores));
        //            }
        //            else if (valores.First().ToString() == "Nomenclatura")
        //            {
        //                return Ok(new MesaEntradasRepository(db).GenerarNomenclaturaParcela(valores));
        //            }
        //            else
        //            {
        //                string[] lNextMesura = new MesaEntradasRepository(db).GenerarMensura(Convert.ToInt64(valores.GetValue(valores.Length - 1)));
        //                string newMesura = "";

        //                foreach (var item in lNextMesura)
        //                {
        //                    newMesura = newMesura + item.ToUpper() + "-";
        //                }

        //                return Ok(newMesura.Substring(0, newMesura.Length - 1));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("Error en el ingreso de datos");
        //    }
        //}

        [Route("api/mesaentradas/tramites/objetosbyrequisito")]
        public IHttpActionResult GetObjetosByRequisito(int idObjetoTramite, int idTramite)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    var requisitos = new MesaEntradasRepository(db).GetObjetosByRequisito(idObjetoTramite, idTramite);

                    if (requisitos.Count() == 0)
                        Global.GetLogger().LogInfo($"No existen requisitos asociados para el Objeto: {idObjetoTramite}");

                    return Ok(requisitos);
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Objetos por Requisito", ex);
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GetTramitePersonaEmail(int idTramite)
        {
            return Ok(_unitOfWork.MesaEntradasRepository.GetTramitePersonaEmail(idTramite));
        }

        public IHttpActionResult GetTramiteEmail(int idTramite)
        {
            return Ok(_unitOfWork.MesaEntradasRepository.GetTramiteEmail(idTramite));
        }

        public IHttpActionResult GetPlantilla(int idObjetoTramite)
        {
            return Ok(_unitOfWork.MesaEntradasRepository.GetPlantilla(idObjetoTramite));
        }

        public IHttpActionResult GetNotaEditable(long idNota, long idUsuario)
        {
            return Ok(_unitOfWork.MesaEntradasRepository.GetNotaEditable(idNota, idUsuario));
        }

        public IHttpActionResult GetPersonaByIdUt(long idUnidadTributaria)
        {
            return Ok(_unitOfWork.MesaEntradasRepository.GetPersonaByIdUt(idUnidadTributaria));
        }

    }
}