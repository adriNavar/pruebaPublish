using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DeclaracionJuradaTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DeclaracionJuradaTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id, int tramite)
        {
            return Ok(_unitOfWork.DeclaracionJuradaTemporalRepository.GetById(id, tramite));
        }

        [HttpGet]
        [Route("api/DeclaracionJuradaTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetDDJJByTramite(int tramite)
        {
            return Ok(_unitOfWork.DeclaracionJuradaTemporalRepository.GetDDJJByTramite(tramite));
        }
    }
}
