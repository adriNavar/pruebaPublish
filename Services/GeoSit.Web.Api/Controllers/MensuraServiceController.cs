using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Common;
using System.Net;
using Z.EntityFramework.Plus;

namespace GeoSit.Web.Api.Controllers
{
    public class MensuraServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Mensura<
        [ResponseType(typeof(ICollection<Mensura>))]
        public IHttpActionResult GetMensuras()
        {
            return Ok(db.Mensura.Where(a => (a.IdUsuarioBaja == null || a.IdUsuarioBaja == 0)).ToList());
        }

        // GET api/GetMensuraById
        [ResponseType(typeof(Mensura))]
        public IHttpActionResult GetMensuraById(long id)
        {
            var mensura = db.Mensura.Include(m => m.TipoMensura)
                                    .Include(m => m.EstadoMensura)
                                    .IncludeFilter(m => m.ParcelasMensuras.Where(pm => pm.FechaBaja == null))
                                    .IncludeFilter(m => m.MensurasRelacionadasOrigen.Where(mro => mro.FechaBaja == null))
                                    .IncludeFilter(m => m.MensurasRelacionadasDestino.Where(mrd => mrd.FechaBaja == null))
                                    .IncludeFilter(m => m.MensurasRelacionadasOrigen.Where(mro => mro.FechaBaja == null).Select(mro => mro.MensuraOrigen))
                                    .IncludeFilter(m => m.MensurasRelacionadasOrigen.Where(mro => mro.FechaBaja == null).Select(mro => mro.MensuraOrigen.TipoMensura))
                                    .IncludeFilter(m => m.MensurasRelacionadasOrigen.Where(mro => mro.FechaBaja == null).Select(mro => mro.MensuraOrigen.EstadoMensura))
                                    .IncludeFilter(m => m.MensurasRelacionadasDestino.Where(mrd => mrd.FechaBaja == null).Select(mrd => mrd.MensuraDestino))
                                    .IncludeFilter(m => m.MensurasRelacionadasDestino.Where(mrd => mrd.FechaBaja == null).Select(mrd => mrd.MensuraDestino.TipoMensura))
                                    .IncludeFilter(m => m.MensurasRelacionadasDestino.Where(mrd => mrd.FechaBaja == null).Select(mrd => mrd.MensuraDestino.EstadoMensura))
                                    .IncludeFilter(m => m.MensurasDocumentos.Where(md => md.FechaBaja == null))
                                    .IncludeFilter(m => m.MensurasDocumentos.Where(md => md.FechaBaja == null).Select(md => md.Documento))
                                    .IncludeFilter(m => m.MensurasDocumentos.Where(md => md.FechaBaja == null).Select(md => md.Documento.Tipo))
                                    .Where(a => a.IdMensura == id).SingleOrDefault();

            if (mensura == null)
            {
                return NotFound();
            }
            return Ok(mensura);
        }

        [HttpPost]
        public IHttpActionResult GetMensurasDetalleByIds(long[] ids)
        {
            var lista = db.Mensura
                          .Join(ids, mensura => mensura.IdMensura, id => id, (mensura, id) => mensura)
                          .Where(mensura => mensura.FechaBaja == null)
                          .Include(m => m.TipoMensura)
                          .Include(m => m.EstadoMensura)
                          .ToList();

            return Ok(lista);

        }

        // GET api/GetDatosMensuraByAll
        [ResponseType(typeof(ICollection<Mensura>))]
        public IHttpActionResult GetDatosMensuraByAll(string id)
        {
            string[] words = id.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var query = db.Mensura.Include(m => m.TipoMensura).Include(m => m.EstadoMensura).Include(m => m.ParcelasMensuras).Where(a => a.IdUsuarioBaja == 0 || a.IdUsuarioBaja == null);

            foreach (var word in words)
            {
                query = query.Where(x => (x.Descripcion != null && x.Descripcion.ToUpper().Contains(word)));
            }

            //EL BUSCADOR LIMITA A MAXIMO 100 resultados -Acollado.
            var mensuras = query.Take(100).ToList();

            if (!mensuras.Any() && mensuras == null)
            {
                return NotFound();
            }
            return Ok(mensuras);
        }

        // GET api/GetDatosMensuraByDescripcion
        [ResponseType(typeof(ICollection<Mensura>))]
        public IHttpActionResult GetDatosMensuraByDescripcion(string id)
        {
            List<Mensura> per = db.Mensura.Where(a => (a.Descripcion.ToUpper().Contains(id.ToUpper())) && (a.IdUsuarioBaja == 0 || a.IdUsuarioBaja == null)).ToList();
            if (per == null)
            {
                return NotFound();
            }
            return Ok(per);
        }

        [HttpPost]
        public IHttpActionResult SetMensura_Save(Mensura mensura)
        {
            mensura.Numero = mensura.Numero.ToUpper();
            mensura.Letra = mensura.Letra.ToUpper();
            mensura.FechaModif = DateTime.Now;
            if (mensura.IdMensura == 0)
            {
                mensura.FechaAlta = mensura.FechaModif;
                db.Entry(mensura).State = EntityState.Added;
            }
            else
            {
                db.Entry(mensura).State = EntityState.Modified;
                db.Entry(mensura).Property(x => x.FechaAlta).IsModified = false;
                db.Entry(mensura).Property(x => x.IdUsuarioAlta).IsModified = false;

                //Elimino relaciones
                /*var parcelasActuales = db.ParcelaMensura.Where(x => x.IdMensura == mensura.IdMensura).ToList();
                foreach (var eliminada in parcelasActuales.Where(x => !mensura.ParcelasMensuras.Any(y => y.IdParcela == x.IdParcela)))
                {
                    db.Entry(eliminada).State = EntityState.Deleted;
                }
                /*foreach (var agregada in parcelasActuales.Where(x => !mensura.ParcelasMensuras.Any(y => y.IdParcela == x.IdParcela)))
                {
                    db.Entry(agregada).State = EntityState.Deleted;
                }
                var listaMensuraDocumento = db.MensuraDocumento.Where(x => x.IdMensura == mensura.IdMensura).ToList();
                foreach (var item in listaMensuraDocumento)
                {
                    db.Entry(item).State = EntityState.Deleted;
                }
                var listaMensuraRelacionadas = db.MensurasRelacionadas.Where(x => x.IdMensuraDestino == mensura.IdMensura || x.IdMensuraOrigen == mensura.IdMensura).ToList();
                foreach (var item in listaMensuraRelacionadas)
                {
                    db.Entry(item).State = EntityState.Deleted;
                }

                var lstParcelaIds = parcelasActuales.Select(p => p.IdParcela).ToList();
                var lstDocumentoIds = listaMensuraDocumento.Select(p => p.IdDocumento).ToList();
                var documentoIds = string.Join(",", lstDocumentoIds);
                var parcelaIds = string.Join(",", lstParcelaIds);
                var listaParcelaDocumento = db.ParcelasDocumentos.Where(x => parcelaIds.Contains(x.ParcelaID.ToString())).ToList();
                foreach (var item in listaParcelaDocumento)
                {
                    if (documentoIds.Contains(item.DocumentoID.ToString()))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }*/
            }

            try
            {
                db.SaveChanges(new Auditoria(mensura.IdUsuarioAlta, Eventos.AltaDeMensura, Mensajes.AgregarMensuraOK, mensura._Machine_Name,
                               mensura._Machine_Name, Autorizado.Si, null, mensura, "Mensura", 1, TiposOperacion.Alta));
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return Ok(mensura);
        }

        [HttpPost]
        public IHttpActionResult DeleteMensura_Save(Mensura mensura)
        {
            var activityInDb = db.Mensura.Include("ParcelasMensuras")
                                         .Include("MensurasRelacionadasOrigen")
                                         .Include("MensurasRelacionadasDestino")
                                         .Include("MensurasDocumentos")
                                         .Include("MensurasDocumentos.Documento").Where(x => x.IdMensura == mensura.IdMensura).FirstOrDefault();

            if (activityInDb != null)
            {
                activityInDb.IdUsuarioBaja = activityInDb.IdUsuarioModif = mensura.IdUsuarioBaja.Value;
                activityInDb.FechaBaja = activityInDb.FechaModif = DateTime.Now;


                foreach (var item in activityInDb.ParcelasMensuras ?? new List<ParcelaMensura>())
                {
                    item.IdUsuarioBaja = item.IdUsuarioModif = mensura.IdUsuarioBaja.Value;
                    item.FechaBaja = item.FechaModif = DateTime.Now;
                }

                foreach (var item in activityInDb.MensurasRelacionadasOrigen ?? new List<MensuraRelacionada>())
                {
                    item.IdUsuarioBaja = item.IdUsuarioModif = mensura.IdUsuarioBaja.Value;
                    item.FechaBaja = item.FechaModif = DateTime.Now;
                }

                foreach (var item in activityInDb.MensurasRelacionadasDestino ?? new List<MensuraRelacionada>())
                {
                    item.IdUsuarioBaja = item.IdUsuarioModif = mensura.IdUsuarioBaja.Value;
                    item.FechaBaja = item.FechaModif = DateTime.Now;
                }

                foreach (var item in activityInDb.MensurasDocumentos ?? new List<MensuraDocumento>())
                {
                    item.IdUsuarioBaja = item.IdUsuarioModif = mensura.IdUsuarioBaja.Value;
                    item.FechaBaja = item.FechaModif = DateTime.Now;
                    item.Documento.id_usu_baja = item.Documento.id_usu_modif = mensura.IdUsuarioBaja.Value;
                    item.Documento.fecha_baja_1 = item.Documento.fecha_modif = DateTime.Now;

                    var parcelasDocumentos = db.ParcelasDocumentos.Where(x => x.DocumentoID == item.Documento.id_documento).ToList();

                    foreach (var pDoc in parcelasDocumentos ?? new List<ParcelaDocumento>())
                    {
                        pDoc.FechaBaja = pDoc.FechaModificacion = DateTime.Now;
                        pDoc.UsuarioBajaID = pDoc.UsuarioModificacionID = mensura.IdUsuarioBaja.Value;
                    }
                }

                try
                {
                    db.SaveChanges(new Auditoria((long)mensura.IdUsuarioBaja, Eventos.EliminarMensura, Mensajes.EliminarMensuraOK, mensura._Machine_Name,
                                   mensura._Machine_Name, Autorizado.Si, db.Entry(activityInDb).OriginalValues.ToObject(), activityInDb, "Mensura", 1, TiposOperacion.Baja));

                }
                catch (DbUpdateConcurrencyException)
                {
                    return Conflict();
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult SetParcelaMensura_Save(long id, List<ParcelaMensura> lstParcelaMensura)
        {
            var mensura = db.Mensura.Find(id);
            var parcelasActuales = db.ParcelaMensura.Where(x => x.IdMensura == id && x.FechaBaja == null).ToList();
            DateTime fecha = DateTime.Now;
            foreach (var eliminada in parcelasActuales.Where(x => !lstParcelaMensura.Any(y => y.IdParcela == x.IdParcela)))
            {
                var query = from par in db.ParcelaMensura
                            join docmen in db.MensuraDocumento on par.IdMensura equals docmen.IdMensura
                            join doc in db.ParcelasDocumentos on new { docmen.IdDocumento, par.IdParcela } equals new { IdDocumento = doc.DocumentoID, IdParcela = doc.ParcelaID }
                            where par.IdMensura == id && par.IdParcela == eliminada.IdParcela && doc.FechaBaja == null && docmen.FechaBaja == null
                            select doc;
                foreach (var doc in query.ToList())
                {
                    doc.FechaBaja = fecha;
                    doc.FechaModificacion = fecha;
                    doc.UsuarioModificacionID = mensura.IdUsuarioModif;
                    doc.UsuarioBajaID = mensura.IdUsuarioModif;
                }
                eliminada.FechaBaja = fecha;
                eliminada.FechaModif = fecha;
                eliminada.IdUsuarioModif = mensura.IdUsuarioModif;
                eliminada.IdUsuarioBaja = mensura.IdUsuarioModif;
            }
            foreach (var agregada in lstParcelaMensura.Where(x => !parcelasActuales.Any(y => y.IdParcela == x.IdParcela)))
            {
                agregada.FechaAlta = fecha;
                agregada.FechaModif = fecha;
                db.ParcelaMensura.Add(agregada);
            }
            db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SetMensuraDocumento_Save(long id, List<MensuraDocumento> lstMensuraDocumento)
        {
            var mensura = db.Mensura.Find(id);
            var MensuraDocumentoActuales = db.MensuraDocumento.Where(x => x.IdMensura == id && x.FechaBaja == null).ToList();
            DateTime fecha = DateTime.Now;
            foreach (var eliminada in MensuraDocumentoActuales.Where(x => !lstMensuraDocumento.Any(y => y.IdDocumento == x.IdDocumento)))
            {
                var query = from doc in db.ParcelasDocumentos
                            join par in db.ParcelaMensura on doc.ParcelaID equals par.IdParcela
                            where par.IdMensura == id && par.FechaBaja == null && doc.DocumentoID == eliminada.IdDocumento && doc.FechaBaja == null
                            select doc;
                foreach (var doc in query.ToList())
                {
                    doc.FechaBaja = fecha;
                    doc.FechaModificacion = fecha;
                    doc.UsuarioModificacionID = mensura.IdUsuarioModif;
                    doc.UsuarioBajaID = mensura.IdUsuarioModif;
                }
                eliminada.FechaBaja = fecha;
                eliminada.FechaModif = fecha;
                eliminada.IdUsuarioModif = mensura.IdUsuarioModif;
                eliminada.IdUsuarioBaja = mensura.IdUsuarioModif;
            }
            foreach (var agregada in lstMensuraDocumento.Where(x => !MensuraDocumentoActuales.Any(y => y.IdDocumento == x.IdDocumento)))
            {
                agregada.FechaAlta = fecha;
                agregada.FechaModif = fecha;
                db.MensuraDocumento.Add(agregada);

            }
            db.SaveChanges();

            return Ok();
        }

        public IHttpActionResult SetParcelaDocumento_Save(List<ParcelaDocumento> lstParcelaDocumento)
        {
            if (lstParcelaDocumento.Any())
            {
                DateTime fecha = DateTime.Now;
                foreach (var parcelaDocumento in lstParcelaDocumento)
                {
                    var parDocsExistentes = from parDoc in db.ParcelasDocumentos
                                            where parDoc.ParcelaID == parcelaDocumento.ParcelaID &&
                                                  parDoc.DocumentoID == parcelaDocumento.DocumentoID
                                            select parDoc;

                    if (!parDocsExistentes.Any())
                    {//si no existe, doy de alta
                        parcelaDocumento.FechaAlta = fecha;
                        parcelaDocumento.FechaModificacion = fecha;
                        db.ParcelasDocumentos.Add(parcelaDocumento);
                    }

                    var parDocExistente = parDocsExistentes.FirstOrDefault(parDoc => parDoc.FechaBaja != null && parDoc.UsuarioBajaID != null);
                    if (parDocExistente != null)
                    {//si existe y está dado de baja, reactivo
                        parDocExistente.FechaBaja = null;
                        parDocExistente.UsuarioBajaID = null;
                    }
                }
                db.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SetMensuraRelacionada_Save(long id, List<MensuraRelacionada> lstMensuraRelacionada)
        {
            var mensura = db.Mensura.Find(id);
            var MensuraRelacionadasActuales = db.MensurasRelacionadas.Where(x => x.IdMensuraDestino == id && x.FechaBaja == null).ToList();
            DateTime fecha = DateTime.Now;
            foreach (var eliminada in MensuraRelacionadasActuales.Where(x => !lstMensuraRelacionada.Any(y => y.IdMensuraOrigen == x.IdMensuraOrigen)))
            {
                eliminada.FechaBaja = fecha;
                eliminada.FechaModif = fecha;
                eliminada.IdUsuarioModif = mensura.IdUsuarioModif;
                eliminada.IdUsuarioBaja = mensura.IdUsuarioModif;
            }
            foreach (var agregada in lstMensuraRelacionada.Where(x => !MensuraRelacionadasActuales.Any(y => y.IdMensuraOrigen == x.IdMensuraOrigen)))
            {
                agregada.FechaAlta = fecha;
                agregada.FechaModif = fecha;
                db.MensurasRelacionadas.Add(agregada);

            }
            db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SetParcelaMensuraMantenedorDelete_Save(long idParcelaMensura, long idUsuario)
        {

            var parcelaMensura = db.ParcelaMensura.Find(idParcelaMensura);
            if (parcelaMensura == null)
            {
                return NotFound();
            }

            parcelaMensura.FechaBaja = parcelaMensura.FechaModif = DateTime.Now;
            parcelaMensura.IdUsuarioBaja = parcelaMensura.IdUsuarioModif = idUsuario;
            db.Entry(parcelaMensura).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(parcelaMensura);
        }

        [HttpPost]
        public IHttpActionResult Reindexar()
        {
            //SolrUpdater.Instance.Enqueue(Entities.mensura);
            //SolrUpdater.Instance.Enqueue(Entities.documento);
            return Ok();
        }

        [HttpGet]
        [Route("api/mensura/departamento/{idDepartamento}/numero")]
        public IHttpActionResult GetNextNumero(long idDepartamento)
        {
            try
            {
                return Ok(new UnitOfWork().MesaEntradasRepository.GenerarMensura(idDepartamento));

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"Generar Mensura Siguiente (departamento:{idDepartamento})", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/mensuraservice/{id}/{numero}/{letra}/disponible")]
        public IHttpActionResult ValidarDisponible(long id, string numero, string letra)
        {
            letra = letra.ToUpper();
            var queryC = from mensura in db.Mensura
                         where mensura.FechaBaja == null && mensura.Numero == numero && mensura.Letra == letra && mensura.IdMensura != id
                         select mensura;

            var queryT = from mensura in db.MensurasTemporal
                         join tramite in db.TramitesMesaEntrada on mensura.IdTramite equals tramite.IdTramite
                         where mensura.FechaBaja == null && mensura.Numero == numero && mensura.Letra == letra && mensura.IdMensura != id
                         select new { tramite = tramite.Numero };

            string msg = string.Empty;
            if (queryC.Any())
            {
                msg = $"Ya existe otra mensura {numero}-{letra}.";
            }
            else if (queryT.Any())
            {
                msg = $"La mensura {numero}-{letra} se generó en el trámite {queryT.Single().tramite}.";
            }

            return Content(string.IsNullOrEmpty(msg) ? HttpStatusCode.OK : HttpStatusCode.Conflict, msg);
        }

        public IHttpActionResult GetMensuraByIdDDJJ(long idDDJJ)
        {
            try
            {
                return Ok(new UnitOfWork().MesaEntradasRepository.GetMensuraByIdDDJJ(idDDJJ));

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"Obtener la siguiente mensura (idUnidadTributaria:{idDDJJ})", ex);
                return InternalServerError(ex);
            }
        }
    }
}
