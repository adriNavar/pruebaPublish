using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DominioTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DominioTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetDominio(int idDominio, int tramite)
        {
            return Ok(_unitOfWork.DominioTemporalRepository.GetDominio(idDominio, tramite));
        }

        public IHttpActionResult GetDominioTitular(int idPersona, int tramite)
        {
            return Ok(_unitOfWork.DominioTemporalRepository.GetDominioTitular(idPersona, tramite));
        }

        [HttpGet]
        [Route("api/DominioTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetDominiosByTramite(int tramite)
        {
            return Ok(_unitOfWork.DominioTemporalRepository.GetDominiosByTramite(tramite));
        }
    }
}

