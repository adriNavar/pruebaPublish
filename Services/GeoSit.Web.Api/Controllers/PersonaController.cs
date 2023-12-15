using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class PersonaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PersonaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(string nombre)
        {
            var persona = _unitOfWork.PersonaRepository.SearchPersona(nombre);
            return Ok(persona);
        }

        public IHttpActionResult Get(long id)
        {
            var persona = _unitOfWork.PersonaRepository.GetPersona(id);
            return Ok(persona);
        }

        public IHttpActionResult GetDatos(long id)
        {
            var persona = _unitOfWork.PersonaRepository.GetPersonaDatos(id);
            return Ok(persona);
        }

        [HttpGet]
        [Route("api/Persona/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetPersonasByTramite(int tramite)
        {
            return Ok(_unitOfWork.PersonaRepository.GetPersonasByTramite(tramite));
        }

        [HttpPost]
        [Route("api/Persona/Buscar/Completa")]
        public IHttpActionResult BuscarPersonasCompleta(long[] personas)
        {
            return Ok(_unitOfWork.PersonaRepository.GetPersonasCompletas(personas));
        }
    }
}
