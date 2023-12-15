using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class LibreDeDeudaTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public LibreDeDeudaTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetLibreDeDeuda(int idLibreDeuda, int tramite)
        {
            return Ok(_unitOfWork.LibreDeDeudaTemporalRepository.GetLibreDeDeuda(idLibreDeuda, tramite));
        }

        [HttpGet]
        [Route("api/LibreDeDeudaTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetLibreDeDeudaByTramite(int tramite)
        {
            return Ok(_unitOfWork.LibreDeDeudaTemporalRepository.GetLibresDeDeudaByTramite(tramite));
        }

    }
}

