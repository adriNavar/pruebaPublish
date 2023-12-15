using System;
using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoInscripcionController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoInscripcionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var inscripciones = _unitOfWork.TipoInscripcionRepository.GetTipoInscripciones();
            return Ok(inscripciones);
        }

        [HttpGet]
        [Route("api/tipoInscripcion/{id}/ejemploRegex")]
        public IHttpActionResult GetEjemploRegex(long id)
        {
            return Ok(_unitOfWork.TipoInscripcionRepository.GetTipoInscripcion(id));
            //return RedirectToRoute($"api/GenericoService/RegexRandomGenerator", new { regex = inscripcion.ExpresionRegular});
           
        }
    }
}
