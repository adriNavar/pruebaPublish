using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Web.Api.Common;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class ControlTecnicoController : ApiController
    {
        // GET api/ControlTecnico
        [ResponseType(typeof(ICollection<ControlTecnico>))]
        public IHttpActionResult GetControlesTecnicos()
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                return Ok(db.ControlesTecnicos.Where(c => c.FechaBaja != null).ToList());
            }
        }

        // GET api/ControlTecnico/5
        [ResponseType(typeof(ControlTecnico))]
        public IHttpActionResult GetControlTecnico(long id)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                ControlTecnico controltecnico = db.ControlesTecnicos.Find(id);
                if (controltecnico == null)
                {
                    return NotFound();
                }

                return Ok(controltecnico);
            }
        }

        // PUT api/ControlTecnico/5
        public IHttpActionResult PutControlTecnico(long id, ControlTecnico controltecnico)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != controltecnico.ControlTecnicoId)
                {
                    return BadRequest();
                }

                db.Entry(controltecnico).State = EntityState.Modified;

                try
                {
                    //db.SaveChanges();
                    db.SaveChanges(new Auditoria(controltecnico.UsuarioModificacionId, Eventos.ModificarControlTecnico, Mensajes.ModificarControlTecnicoOK, controltecnico._Machine_Name,
controltecnico._Machine_Name, Autorizado.Si, controltecnico, controltecnico, Objetos.ControlTecnico, 1, TiposOperacion.Modificacion));

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ControlTecnicoExists(id, db))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
        }

        // POST api/ControlTecnico
        [ResponseType(typeof(ControlTecnico))]
        public IHttpActionResult PostControlTecnico(ControlTecnico controltecnico)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.ControlesTecnicos.Add(controltecnico);

                try
                {
                    db.SaveChanges(new Auditoria(controltecnico.UsuarioModificacionId, Eventos.AltaAlfanumericodeParcelasyPosesiones, Mensajes.AltaAlfanumericodeParcelasyPosesionesOK, controltecnico._Machine_Name,
                            controltecnico._Machine_Name, Autorizado.Si, null ,controltecnico, "ControlTecnico", 1, TiposOperacion.Alta));
                   // db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (ControlTecnicoExists(controltecnico.ControlTecnicoId, db))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtRoute("DefaultApi", new { id = controltecnico.ControlTecnicoId }, controltecnico);
            }
        }

        // DELETE api/ControlTecnico/5
        [ResponseType(typeof(ControlTecnico))]
        public IHttpActionResult DeleteControlTecnico(long id)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                ControlTecnico controltecnico = db.ControlesTecnicos.Find(id);
                if (controltecnico == null)
                {
                    return NotFound();
                }

                db.ControlesTecnicos.Remove(controltecnico);
                db.SaveChanges(new Auditoria(controltecnico.UsuarioModificacionId, Eventos.AltaAlfanumericodeParcelasyPosesiones, Mensajes.AltaAlfanumericodeParcelasyPosesionesOK, controltecnico._Machine_Name,
                    controltecnico._Machine_Name, Autorizado.Si, null, controltecnico, "ControlTecnico", 1, TiposOperacion.Baja));

                return Ok(controltecnico);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ControlTecnicoExists(long id, GeoSITMContext db)
        {
            return db.ControlesTecnicos.Count(e => e.ControlTecnicoId == id) > 0;
        }
    }
}