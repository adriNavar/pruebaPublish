using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class ActaServiceController : ApiController
    {

        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/GetInspector/1
        [ResponseType(typeof(ActaTipo))]
        public IHttpActionResult GetActasTipos()
        {
            List<ActaTipo> actas = db.ActaTipos.ToList();
            return Ok(actas.OrderBy(o => o.Descripcion));
        }

        // GET api/GetInspector/1
        [ResponseType(typeof(EstadoActa))]
        public IHttpActionResult GetActasEstados()
        {
            List<EstadoActa> actas = db.EstadoActas.ToList();
            return Ok(actas.OrderBy(o => o.Descripcion));
        }

        // GET api/GetInspector/1
        [ResponseType(typeof(ActaRolPersona))]
        public IHttpActionResult GetActaRolPersona()
        {
            List<ActaRolPersona> actas = db.ActaRolPersona.ToList();
            return Ok(actas.OrderBy(o => o.Descripcion));
        }

        [ResponseType(typeof(StringObject))]
        public IHttpActionResult GetActaBaja(int actaID, int usuarioConectado)
        {
            StringObject so = new StringObject();
            so.Response = "OK";

            try
            {
                if (actaID > 0)
                {
                    List<Acta> actas = db.Actas.Where(
                    i => i.ActaId == actaID
                    ).ToList();
                    if (actas.Count() == 1)
                    {
                        Acta acta = actas[0];
                        acta.FechaBaja = DateTime.Now;
                        acta.UsuarioBajaId = usuarioConectado;
                        //db.SaveChanges();
                        db.SaveChanges(new Auditoria(usuarioConectado, Eventos.Bajadeactas, Mensajes.BajadeactasOK, acta._Machine_Name,
                          acta._Machine_Name, Autorizado.Si, acta, null, Objetos.Acta, 1, TiposOperacion.Baja));

                    }
                    else
                    {
                        so.Response = "Error";
                        so.Message = "Acta inexistente";
                    }
                }
                else
                {
                    so.Response = "Error";
                    so.Message = "ID de Acta incorrecto";
                }
            }
            catch (Exception ex)
            {
                so.Response = "Error";
                so.Message = ex.Message;
            }

            return Json(so);
        }

        [ResponseType(typeof(StringObject))]
        public IHttpActionResult PostActaGuardar(Acta actaPost)
        {
            bool tieneModificaciones<T>() where T : class
            {
                return db.ChangeTracker.Entries<T>().Any(e => e.State == EntityState.Deleted || e.State == EntityState.Added);
            };
            var auditorias = new List<Auditoria>();
            Acta acta;
            if (actaPost.ActaId > 0)
            {
                acta = db.Actas.Find(actaPost.ActaId);
            }
            else
            {
                acta = db.Actas.Add(new Acta()
                {
                    UsuarioAltaId = actaPost.UsuarioModificacionId,
                    FechaAlta = DateTime.Now,
                });
            }
            if (actaPost.NroActa != acta.NroActa && db.Actas.Any(a => a.NroActa == actaPost.NroActa))
            {
                return Content(HttpStatusCode.Conflict, "El nro de acta ingresado ya existe en otro registro");
            }

            acta.NroActa = actaPost.NroActa;
            acta.InspectorId = actaPost.InspectorId;
            acta.Plazo = actaPost.Plazo;
            acta.Fecha = actaPost.Fecha;
            acta.ActaTipoId = actaPost.ActaTipoId;
            acta.FechaModificacion = DateTime.Now;
            acta.UsuarioModificacionId = actaPost.UsuarioModificacionId;
            acta.observaciones = actaPost.observaciones ?? string.Empty;

            auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                         acta._Machine_Name, Autorizado.Si, acta, acta, Objetos.Acta, 1, TiposOperacion.Alta));

            ////////////////////////////////////
            var utsBorradas = db.ActaUnidadesTributarias.RemoveRange(db.ActaUnidadesTributarias.Where(i => i.ActaId == acta.ActaId));
            if (!string.IsNullOrEmpty(actaPost.selectedUT))
            {
                db.ActaUnidadesTributarias.AddRange(actaPost.selectedUT
                                                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                            .Select(ut => new ActaUnidadTributaria() { Acta = acta, UnidadTributariaID = long.Parse(ut) }));

            }
            auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                         acta._Machine_Name, Autorizado.Si,
                                         string.Join(",", utsBorradas.Select(ut => ut.UnidadTributariaID)),
                                         actaPost.selectedUT,
                                         "ActaUnidadTributaria", 1, TiposOperacion.Alta));

            ////////////////////////////////////
            var personasBorradas = db.ActaPersonas.RemoveRange(db.ActaPersonas.Where(i => i.ActaId == acta.ActaId));
            IEnumerable<ActaPersonas> personasAgregadas = new ActaPersonas[0];
            if (!string.IsNullOrEmpty(actaPost.selectedPer))
            {
                personasAgregadas = db.ActaPersonas.AddRange(actaPost.selectedPer
                                                   .Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(persona => new ActaPersonas()
                                                   {
                                                       Acta = acta,
                                                       PersonaRolId = long.Parse(persona.Split('-')[1]),
                                                       PersonaId = long.Parse(persona.Split('-')[0]),
                                                   }));
            }
            auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                         acta._Machine_Name, Autorizado.Si,
                                         string.Join(",", personasBorradas.Select(per => $"{per.PersonaId}-{per.PersonaRolId}")),
                                         string.Join(",", personasAgregadas.Select(per => $"{per.PersonaId}-{per.PersonaRolId}")),
                                         "ActaPersonas", 1, TiposOperacion.Alta));

            ////////////////////////////////////
            var domiciliosBorrados = db.ActaDomicilios.RemoveRange(db.ActaDomicilios.Where(i => i.ActaID == acta.ActaId));
            if (!string.IsNullOrEmpty(actaPost.SelectedDomicilio))
            {
                db.ActaDomicilios.AddRange(actaPost.SelectedDomicilio
                                                   .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(dom => new ActaDomicilio()
                                                   {
                                                       Acta = acta,
                                                       id_domicilio = long.Parse(dom)
                                                   }));
            }
            auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                         acta._Machine_Name, Autorizado.Si,
                                         string.Join(",", domiciliosBorrados.Select(a => a.id_domicilio)),
                                         actaPost.SelectedDomicilio,
                                         "ActaDomicilio", 1, TiposOperacion.Alta));

            ////////////////////////////////////
            var actasRelBorradas = db.ActaActaRels.RemoveRange(db.ActaActaRels.Where(i => i.ActaId == acta.ActaId));
            if (!string.IsNullOrEmpty(actaPost.selectedActasOrigen))
            {
                db.ActaActaRels.AddRange(actaPost.selectedActasOrigen
                                                 .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(origen => new ActaActaRel() { Acta = acta, ActaRelId = long.Parse(origen) }));
            }
            if (tieneModificaciones<ActaActaRel>())
            {
                auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                             acta._Machine_Name, Autorizado.Si,
                                             string.Join(",", actasRelBorradas.Select(a => a.ActaRelId)),
                                             actaPost.selectedActasOrigen,
                                             "ActaActaRelacion", 1, TiposOperacion.Alta));
            }


            ////////////////////////////////////
            var objetosBorrados = db.ActaObjeto.RemoveRange(db.ActaObjeto.Where(i => i.ActaID == acta.ActaId));
            IEnumerable<ActaObjeto> objetosAgregados = new ActaObjeto[0];
            if (!string.IsNullOrEmpty(actaPost.selectedOtrosObjetos))
            {
                objetosAgregados = db.ActaObjeto.AddRange(actaPost.selectedPer
                                                                  .Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries)
                                                                  .Select(obj => new ActaObjeto()
                                                                  {
                                                                      Acta = acta,
                                                                      id_tipo_objeto = long.Parse(obj.Split('-')[0]),
                                                                      id_objeto = long.Parse(obj.Split('-')[1]),
                                                                  }));
            }
            if (tieneModificaciones<ActaObjeto>())
            {
                auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                             acta._Machine_Name, Autorizado.Si,
                                             string.Join(",", objetosBorrados.Select(obj => $"{obj.id_tipo_objeto}-{obj.id_tipo_objeto}")),
                                             string.Join(",", objetosAgregados.Select(obj => $"{obj.id_tipo_objeto}-{obj.id_tipo_objeto}")),
                                             "ActaObjeto", 1, TiposOperacion.Alta));
            }

            ////////////////////////////////////
            var docsBorrados = db.ActaDocumentos.RemoveRange(db.ActaDocumentos.Where(i => i.ActaID == acta.ActaId));
            if (!string.IsNullOrEmpty(actaPost.selectedDocs))
            {
                db.ActaDocumentos.AddRange(actaPost.selectedDocs
                                                   .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(doc => new ActaDocumento()
                                                   {
                                                       Acta = acta,
                                                       id_documento = long.Parse(doc)
                                                   }));
            }
            if (tieneModificaciones<ActaDocumento>())
            {
                auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                             acta._Machine_Name, Autorizado.Si,
                                             string.Join(",", docsBorrados.Select(a => a.id_documento)),
                                             actaPost.selectedOtrosObjetos,
                                             "ActaDocumento", 1, TiposOperacion.Alta));
            }

            ////////////////////////////////////
            var estadosBorrados = db.ActaEstados.RemoveRange(db.ActaEstados.Where(i => i.ActaId == acta.ActaId));
            db.ActaEstados.Add(new ActaEstado() { Acta = acta, EstadoActaId = actaPost.SelectedEstadoActa });
            auditorias.Add(new Auditoria(actaPost.UsuarioModificacionId, Eventos.AltadeActas, Mensajes.AltadeActasOK, acta._Machine_Name,
                                  acta._Machine_Name, Autorizado.Si,
                                  string.Join(",", estadosBorrados.Select(a => a.EstadoActaId)),
                                  actaPost.SelectedEstadoActa,
                                  "ActaEstado", 1, TiposOperacion.Alta));

            try
            {
                db.SaveChanges(auditorias);
            }
            catch (DbEntityValidationException dbEx)
            {
                string error = string.Join(Environment.NewLine, dbEx.EntityValidationErrors
                                                                    .SelectMany(entityerr => entityerr.ValidationErrors
                                                                                                      .Select(err => $"{entityerr.Entry.Entity}:{err.ErrorMessage}")));
                return Content(HttpStatusCode.BadRequest, error);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }

            return Content(HttpStatusCode.OK, acta.ActaId);
        }

        [ResponseType(typeof(List<Acta>))]
        public IHttpActionResult GetActaById(long id)
        {
            Acta acta = db.Actas.Find(id);
            db.Entry(acta).State = EntityState.Detached;
            //string estadoDesc;
            var actaEstadoRel = db.ActaEstados
                                  .Where(i => i.ActaId == acta.ActaId)
                                  .ToList();
            if (actaEstadoRel.Count == 1)
            {
                acta.SelectedEstadoActa = actaEstadoRel[0].EstadoActaId;
            }

            var roles = db.ActaRolPersona.ToList();

            acta.selectedPer = string.Join("@", db.ActaPersonas
                                                  .Include(p => p.Persona)
                                                  .Where(i => i.ActaId == acta.ActaId)
                                                  .ToList()
                                                  .Select(a => string.Join("-", new object[] { a.PersonaId, a.PersonaRolId, roles.Single(r => r.ActaRolId == a.PersonaRolId).Descripcion.Trim(), a.Persona.NombreCompleto.Trim() })));

            acta.selectedUT = string.Join(",", db.ActaUnidadesTributarias
                                                  .Include(p => p.UnidadTributaria)
                                                  .Where(i => i.ActaId == acta.ActaId)
                                                  .ToList()
                                                  .Select(a => string.Join("-", new object[] { a.UnidadTributaria.CodigoProvincial, a.UnidadTributariaID })));

            acta.selectedDocs = string.Join(",", db.ActaDocumentos
                                                   .Include(a => a.documento)
                                                   .Include(d => d.documento.Tipo)
                                                   .Where(i => i.ActaID == acta.ActaId)
                                                   .ToList()
                                                   .Select(a => string.Join("-", new object[] {
                                                       a.id_documento,
                                                       a.documento.Tipo.Descripcion,
                                                       a.documento.descripcion,
                                                       a.documento.fecha.ToString("dd/MM/yyyy"),
                                                       a.documento.nombre_archivo,
                                                   })));

            acta.SelectedDomicilio = string.Join(",", db.ActaDomicilios
                                            .Include(a => a.domicilio)
                                            .Include(a => a.domicilio.UnidadTributariaDomicilio)
                                            .Where(i => i.ActaID == acta.ActaId)
                                            .ToList()
                                            .SelectMany(d =>
                                            {
                                                if (!(d.domicilio.UnidadTributariaDomicilio?.Any() ?? false))
                                                {
                                                    return new[] { string.Join("-", new object[] { d.id_domicilio, $"{d.domicilio.ViaNombre.Trim()} {d.domicilio.numero_puerta ?? string.Empty}", string.Empty }) };
                                                }
                                                return d.domicilio
                                                        .UnidadTributariaDomicilio
                                                        .Select(ut => string.Join("-", new object[] {
                                                            d.id_domicilio,
                                                            $"{d.domicilio.ViaNombre.Trim()} {d.domicilio.numero_puerta}",
                                                            ut.UnidadTributariaID
                                                        }));
                                            }));

            //QUEDA PENDIENTE PARA MAS ADELANTE
            //acta.selectedActasOrigen = "";
            //List<ActaActaRel> actasRel = db.ActaActaRels.Where(i => i.ActaId == acta.ActaId).ToList<ActaActaRel>();
            //foreach (ActaActaRel ar in actasRel)
            //{
            //    acta.selectedActasOrigen += ar.ActaRelId + ",";
            //}
            //acta.selectedActasOrigen = acta.selectedActasOrigen != "" ? acta.selectedActasOrigen.Substring(0, acta.selectedActasOrigen.Length - 1) : "";

            //QUEDA PENDIENTE PARA MAS ADELANTE
            //acta.selectedOtrosObjetos = "";
            //List<ActaObjeto> actaObjetos = db.ActaObjeto.Where(i => i.ActaID == acta.ActaId).ToList<ActaObjeto>();
            //foreach (ActaObjeto obj in actaObjetos)
            //{
            //    acta.selectedOtrosObjetos += obj.id_tipo_objeto + "-" + obj.id_objeto + ",";
            //}
            //acta.selectedOtrosObjetos = acta.selectedOtrosObjetos != "" ? acta.selectedOtrosObjetos.Substring(0, acta.selectedOtrosObjetos.Length - 1) : "";

            return Ok(acta);
        }

        public IHttpActionResult PostActaBuscar(ActaBusqueda actaPost)
        {
            string[] tokens = (actaPost.selectedInspectoresBusqueda ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var convertedItems = Array.ConvertAll(tokens, long.Parse).ToList();

            var query = from acta in db.Actas
                        join actaEstado in db.ActaEstados on acta.ActaId equals actaEstado.ActaId
                        join actaUT in db.ActaUnidadesTributarias on acta.ActaId equals actaUT.ActaId into lj
                        from actaUT in lj.DefaultIfEmpty()
                        where acta.FechaBaja == null
                        select new { acta, actaUT, actaEstado };

            if (actaPost.buscaFecha == 1)
            {/*Fecha*/
                query = query.Where(reg => reg.acta.Fecha >= actaPost.fechaDesde && reg.acta.Fecha <= actaPost.fechaHasta);
            }

            if (actaPost.buscaNumero == 1)
            {/*Numero*/
                query = query.Where(reg => reg.acta.NroActa >= actaPost.numeroDesde && reg.acta.NroActa <= actaPost.numeroHasta);
            }

            if (actaPost.buscaInspectores == 1)
            {/*Inspectores*/
                query = query.Where(reg => convertedItems.Contains(reg.acta.InspectorId));
            }
            if (actaPost.buscaId == 1)
            {/*Id*/
                query = query.Where(reg => reg.acta.ActaId == actaPost.idActa);
            }
            if (actaPost.buscaUnidad == 1)
            {/*Unidad Tributaria*/
                query = query.Where(reg => reg.actaUT.UnidadTributariaID == actaPost.idUnidad);
            }
            if (actaPost.buscaEstado == 1)
            {/*Estado*/
                query = query.Where(reg => reg.actaEstado.EstadoActaId == actaPost.idEstado);
            }

            var resultados = from registro in query
                             join inspector in db.Inspectores on registro.acta.InspectorId equals inspector.InspectorID
                             join usuario in db.Usuarios on inspector.UsuarioID equals usuario.Id_Usuario
                             join estadoActa in db.EstadoActas on registro.actaEstado.EstadoActaId equals estadoActa.EstadoActaId
                             join actaTipo in db.ActaTipos on registro.acta.ActaTipoId equals actaTipo.ActaTipoId
                             group new
                             {
                                 registro.acta.ActaId,
                                 registro.acta.NroActa,
                                 Tipo = actaTipo.Descripcion,
                                 registro.acta.Fecha,
                                 registro.acta.Plazo,
                                 Inspector = usuario.Apellido + " " + usuario.Nombre,
                                 Estado = estadoActa.Descripcion
                             } by registro.acta.ActaId into g
                             select g.FirstOrDefault();
            return Ok(resultados.ToList().Select(a => new object[] { a.ActaId, a.NroActa, a.Tipo, a.Fecha, a.Plazo, a.Inspector, a.Estado }));
        }

        public class StringObject
        {
            public string Response { get; set; }
            public string Code { get; set; }
            public string Message { get; set; }
        }
        public class ActaBusqueda
        {
            public int buscaFecha { get; set; }
            public int buscaNumero { get; set; }
            public int buscaInspectores { get; set; }
            public int buscaId { get; set; }
            public int buscaUnidad { get; set; }
            public int buscaEstado { get; set; }
            public DateTime fechaDesde { get; set; }
            public DateTime fechaHasta { get; set; }
            public int numeroDesde { get; set; }
            public int numeroHasta { get; set; }
            public int idActa { get; set; }
            public int idUnidad { get; set; }
            public int idEstado { get; set; }
            public string selectedInspectoresBusqueda { get; set; }
        }
    }
}