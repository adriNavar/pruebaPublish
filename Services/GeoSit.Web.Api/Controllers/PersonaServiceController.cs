using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Solr;

namespace GeoSit.Web.Api.Controllers
{
    public class PersonaServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Persona<
        [ResponseType(typeof(ICollection<Persona>))]
        public IHttpActionResult GetPersonas()
        {
            return Ok(db.Persona.Where(a => (a.UsuarioBajaId == null || a.UsuarioBajaId == 0)).ToList());
        }

        // GET api/GetPersonaById
        [ResponseType(typeof(Persona))]
        public IHttpActionResult GetPersonaById(long id)
        {
            Persona per = db.Persona.Where(a => a.PersonaId == id).SingleOrDefault();
            if (per == null)
            {
                return NotFound();
            }
            return Ok(per);
        }

        // GET api/GetDatosPersonaByAll
        [ResponseType(typeof(ICollection<Persona>))]
        public IHttpActionResult GetDatosPersonaByAll(string id)
        {
            string[] words = id.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var query = db.Persona.Where(a => a.UsuarioBajaId == 0 || a.UsuarioBajaId == null);

            foreach (var word in words)
            {
                query = query.Where(x => (x.NombreCompleto != null && x.NombreCompleto.ToUpper().Contains(word)) ||
                                         (x.Nombre != null && x.Nombre.ToUpper().Contains(word)) ||
                                         (x.Apellido != null && x.Apellido.ToUpper().Contains(word)) ||
                                         (x.NroDocumento != null && x.NroDocumento.ToUpper().Contains(word)));
            }

            //EL BUSCADOR LIMITA A MAXIMO 100 resultados -Acollado.
            var personas = query.Take(100).ToList();

            if (!personas.Any() && personas == null)
            {
                return NotFound();
            }
            return Ok(personas);
        }

        // GET api/GetDatosPersonaByNombre
        [ResponseType(typeof(ICollection<Persona>))]
        public IHttpActionResult GetDatosPersonaByNombre(string id)
        {
            //<List<Persona> per = db.Persona.Where(a => (a.NombreCompleto.ToUpper().Substring(0, id.Length) == id.ToUpper()) && a.UsuarioBajaId == 0).ToList();
            List<Persona> per = db.Persona.Where(a => (a.NombreCompleto.ToUpper().Contains(id.ToUpper())) && (a.UsuarioBajaId == 0 || a.UsuarioBajaId == null)).ToList();
            if (per == null)
            {
                return NotFound();
            }
            return Ok(per);
        }

        // GET api/GetDatosPersonaByDocumento
        [ResponseType(typeof(ICollection<Persona>))]
        public IHttpActionResult GetDatosPersonaByDocumento(string id)
        {
            List<Persona> per = db.Persona.Where(a => (a.NroDocumento == id) && (a.UsuarioBajaId == 0 || a.UsuarioBajaId == null)).ToList();
            if (per == null)
            {
                return NotFound();
            }
            return Ok(per);
        }

        // GET api/GetPersonaByClave
        [ResponseType(typeof(Persona))]
        public IHttpActionResult GetPersonaByClave(string nrodoc, long tipodoc)
        {
            Persona per = db.Persona.Where(a => (a.NroDocumento == nrodoc) && (a.UsuarioBajaId == 0 || a.UsuarioBajaId == null) && a.TipoDocId == tipodoc).FirstOrDefault();
            if (per == null)
            {
                return NotFound();
            }
            return Ok(per);
        }

        [HttpPost]
        public IHttpActionResult SetPersona_Save(Persona parametros)
        {
            try
            {
                parametros.FechaModif = DateTime.Now;
                var persona = db.Persona.Find(parametros.PersonaId);
                string evento = Eventos.ModificarPersonas, mensaje = Mensajes.ModificarPersonasOK, operacion = TiposOperacion.Modificacion;
                if (persona == null)
                {
                    evento = Eventos.AltadePersonas;
                    mensaje = Mensajes.AltadePersonasOK;
                    operacion = TiposOperacion.Alta;
                    persona = db.Persona.Add(new Persona()
                    {
                        FechaAlta = parametros.FechaModif,
                        UsuarioAltaId = parametros.UsuarioModifId,
                    });
                }
                else
                {
                    db.Profesion.RemoveRange(db.Profesion.Where(x => x.PersonaId == persona.PersonaId));
                }

                persona.Apellido = parametros.Apellido;
                persona.Email = parametros.Email;
                persona.EstadoCivil = parametros.EstadoCivil;
                persona.Nacionalidad = parametros.Nacionalidad;
                persona.Nombre = parametros.Nombre;
                string nombreCompleto = $"{persona.Apellido}, {persona.Nombre}".Trim();
                persona.NombreCompleto = nombreCompleto.Substring(0, Math.Min(nombreCompleto.Length, 50));
                persona.NroDocumento = parametros.NroDocumento;
                persona.Sexo = parametros.Sexo;
                persona.Telefono = parametros.Telefono;
                persona.TipoDocId = parametros.TipoDocId;
                persona.TipoPersonaId = parametros.TipoPersonaId;

                persona.FechaModif = parametros.FechaModif;
                persona.UsuarioModifId = parametros.UsuarioModifId;

                db.SaveChanges(new Auditoria(parametros.UsuarioModifId, evento, mensaje, parametros._Machine_Name,
                               parametros._Machine_Name, Autorizado.Si, null, parametros, "Persona", 1, TiposOperacion.Alta));

                //SolrUpdater.Instance.Enqueue(Entities.persona);

                return Ok(persona);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("PersonaServiceController-SetPersona_Save", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult DeletePersona_Save(Persona parametros)
        {
            var activityInDb = db.Persona.Find(parametros.PersonaId);
            Persona Valores = new Persona();
            if (activityInDb != null)
            {
                Valores.PersonaId = activityInDb.PersonaId;
                Valores.TipoDocId = activityInDb.TipoDocId;
                Valores.NroDocumento = activityInDb.NroDocumento;
                Valores.TipoPersonaId = activityInDb.TipoPersonaId;
                Valores.NombreCompleto = activityInDb.NombreCompleto;
                Valores.Nombre = activityInDb.Nombre;
                Valores.Apellido = activityInDb.Apellido;
                Valores.UsuarioAltaId = activityInDb.UsuarioAltaId;
                Valores.FechaAlta = activityInDb.FechaAlta;
                Valores.UsuarioModifId = activityInDb.UsuarioModifId;
                Valores.FechaModif = activityInDb.FechaModif;
                Valores.FechaBaja = parametros.FechaBaja;
                Valores.UsuarioBajaId = parametros.UsuarioBajaId;
                Valores.Sexo = activityInDb.Sexo;
                Valores.EstadoCivil = activityInDb.EstadoCivil;
                Valores.Nacionalidad = activityInDb.Nacionalidad;
                Valores.Telefono = activityInDb.Telefono;
                Valores.Email = activityInDb.Email;

                db.Entry(activityInDb).CurrentValues.SetValues(Valores);
                db.Entry(activityInDb).State = EntityState.Modified;

                try
                {
                    db.SaveChanges(new Auditoria(parametros.UsuarioModifId, Eventos.ModificarPersonas, Mensajes.ModificarPersonasOK, parametros._Machine_Name,
                                   parametros._Machine_Name, Autorizado.Si, activityInDb, Valores, "Persona", 1, TiposOperacion.Baja));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetPersonaCompleta(long id)
        {
            using (db)
            {
                var persona = db.Persona
                                .Include(p => p.TipoDocumentoIdentidad)
                                .Include(p => p.PersonaDomicilios.Select(pd => pd.Domicilio))
                                .Include(p => p.PersonaDomicilios.Select(pd => pd.TipoDomicilio))
                                .Where(a => a.PersonaId == id)
                                .SingleOrDefault();

                return persona == null ? (IHttpActionResult)NotFound() : Ok(persona);
            }
        }
    }
}
