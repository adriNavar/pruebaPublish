using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class PersonaDomicilioServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/PersonaDomicilio<
        [ResponseType(typeof(ICollection<PersonaDomicilio>))]
        public IHttpActionResult GetPersonaDomicilios()
        {
            return Ok(db.PersonaDomicilio.ToList());
        }

        [ResponseType(typeof(ICollection<PersonaDomicilio>))]
        public IHttpActionResult GetPersonaDomiciliosByPersona(long id)
        {
            List<PersonaDomicilio> perdom = db.PersonaDomicilio.Where(a => a.PersonaId == id && (a.UsuarioBajaId == null || a.UsuarioBajaId == 0)).ToList();
            if (perdom == null)
            {
                return NotFound();
            }
            return Ok(perdom);
        }

        [ResponseType(typeof(PersonaDomicilio))]
        public IHttpActionResult GetPersonaDomiciliosByAtributos(long idpersona, long iddomicilio, long idtipo)
        {
            PersonaDomicilio perdom = db.PersonaDomicilio.Where(a => a.PersonaId == idpersona && a.DomicilioId == iddomicilio && a.TipoDomicilioId == idtipo && (a.UsuarioBajaId == null || a.UsuarioBajaId == 0)).FirstOrDefault();
            if (perdom == null)
            {
                return NotFound();/**/
            }
            return Ok(perdom);
        }

        [HttpPost]
        public IHttpActionResult SetPersonaDomicilio_Save(PersonaDomicilio parametros)
        {
            try
            {
                parametros.FechaModif = DateTime.Now;
                var current = db.PersonaDomicilio.SingleOrDefault(a => a.PersonaId == parametros.PersonaId && a.DomicilioId == parametros.DomicilioId && a.TipoDomicilioId == parametros.TipoDomicilioId && a.FechaBaja == null);

                if (current == null)
                {
                    current = db.PersonaDomicilio.Add(new PersonaDomicilio()
                    {
                        PersonaId = parametros.PersonaId,
                        DomicilioId = parametros.DomicilioId,
                        TipoDomicilioId = parametros.TipoDomicilioId,

                        FechaAlta = parametros.FechaModif,
                        UsuarioAltaId = parametros.UsuarioModifId
                    });
                }
                current.UsuarioModifId = parametros.UsuarioModifId;
                current.FechaModif = parametros.FechaModif;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("PersonaDomicilioServiceController-SetPersonaDomicilio_Save", ex);
                return InternalServerError();
            }
            return Ok();
        }
        [HttpDelete]
        public IHttpActionResult DeletePersonaDomicilio_Save(PersonaDomicilio parametros)
        {
            try
            {
                var current = db.PersonaDomicilio.SingleOrDefault(a => a.PersonaId == parametros.PersonaId && a.DomicilioId == parametros.DomicilioId && a.TipoDomicilioId == parametros.TipoDomicilioId && a.FechaBaja == null);
                if (current != null)
                {
                    current.UsuarioBajaId = current.UsuarioModifId = parametros.UsuarioBajaId.Value;
                    current.FechaBaja = current.FechaModif = DateTime.Now;
                }

                var personasConDomicilio = from perDom in db.PersonaDomicilio
                                           where perDom.PersonaId != parametros.PersonaId && perDom.DomicilioId == parametros.DomicilioId &&
                                                 perDom.TipoDomicilioId == parametros.TipoDomicilioId && perDom.FechaBaja == null
                                           select perDom;
                if (!personasConDomicilio.Any())
                {
                    var domicilio = db.Domicilios.SingleOrDefault(x => x.DomicilioId == parametros.DomicilioId && x.TipoDomicilioId == parametros.TipoDomicilioId && x.FechaBaja == null);
                    if(domicilio != null)
                    {
                        domicilio.UsuarioBajaId = domicilio.UsuarioModifId = parametros.UsuarioBajaId.Value;
                        domicilio.FechaBaja = domicilio.FechaModif = DateTime.Now;
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("PersonaDomicilioServiceController-DeletePersonaDomicilio_Save", ex);
                return InternalServerError();
            }
            return Ok();
        }
    }
}
