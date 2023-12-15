using System.Linq;
using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class UnidadTributariaTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public UnidadTributariaTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("api/UnidadTributariaTemporal/{id}/tramite/{tramite}")]
        public IHttpActionResult Get(long id, int tramite)
        {
            return Ok(_unitOfWork.UnidadTributariaTemporalRepository.GetById(id, tramite));
        }
        
        [HttpGet]
        [Route("api/UnidadTributariaTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetEntradasByTramite(int tramite)
        {
            return Ok(_unitOfWork.UnidadTributariaTemporalRepository.GetEntradasByIdTramite(tramite));
        }
    }
}
