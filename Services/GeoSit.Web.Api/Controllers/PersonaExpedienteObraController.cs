using System.Linq;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ValidationRules;
using GeoSit.Data.BusinessEntities.ValidationRules.ExpedientesObra;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;
using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Web.Api.Controllers
{
    public class PersonaExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PersonaExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var documentoExpedienteObras = _unitOfWork.PersonaExpedienteObraRepository
                .GetPersonaExpedienteObras(id);
            return Ok(documentoExpedienteObras);
        }
        public IHttpActionResult GetPersonaByRolExpedienteObra(long idExpedienteObra, int idRol, long idPersona)
        {
            var pieo = _unitOfWork.PersonaExpedienteObraRepository.GetPersonaExpedienteObraById(idExpedienteObra, idPersona, idRol);
            return Ok(pieo);
        }
        [Route("api/personaexpedienteobra/validate")]
        public IHttpActionResult Post(long idExpedienteObra, long idPersona, int idRol, Operaciones<PersonaExpedienteObra> operaciones)
        {
            var personaExpObra = new PersonaExpedienteObra
            {
                ExpedienteObraId = idExpedienteObra,
                PersonaInmuebleId = idPersona,
                RolId = idRol
            };
            //chequeo en la bd
            var persona = _unitOfWork.PersonaExpedienteObraRepository
                                     .GetPersonaExpedienteObraById(personaExpObra.ExpedienteObraId, personaExpObra.PersonaInmuebleId, personaExpObra.RolId);

            string errors = FluentValidator<PersonaExpedienteObra>.Validate(new PersonaValidator(persona), personaExpObra);
            if (!string.IsNullOrEmpty(errors)) return new TextResult(errors, Request);

            //chequeo en memoria
            var item = operaciones.FirstOrDefault(p => p.Item.ExpedienteObraId == personaExpObra.ExpedienteObraId && 
                                                       p.Item.PersonaInmuebleId == personaExpObra.PersonaInmuebleId &&
                                                       p.Item.RolId == personaExpObra.RolId);
            if (item != null)
            {
                errors = FluentValidator<PersonaExpedienteObra>.Validate(new PersonaValidator(item.Item), personaExpObra);
            }
            if (!string.IsNullOrEmpty(errors)) return new TextResult(errors, Request);

            return Ok();
        }

        public IHttpActionResult Delete(long idExpedienteObra, long idPersona)
        {
            return Ok();
        }
    }
}
