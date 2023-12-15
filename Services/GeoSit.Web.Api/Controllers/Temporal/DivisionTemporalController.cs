using System.Web.Http;
using GeoSit.Data.DAL.Common;


namespace GeoSit.Web.Api.Controllers
{
    public class DivisionTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DivisionTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetManzana(int idManzana, int tramite)
        {
            return Ok(_unitOfWork.DivisionTemporalRepository.GetManzana(idManzana, tramite));
        }

        [HttpGet]
        [Route("api/DivisionTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetManzanasByTramite(int tramite)
        {
            return Ok(_unitOfWork.DivisionTemporalRepository.GetByTramite(tramite));
        }

    }
}

