using System.Web.Http;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Temporal;

namespace GeoSit.Web.Api.Controllers
{
    public class EspacioPublicoTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public EspacioPublicoTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetEspacioPublicoById(long id, int tramite)
        {
            return Ok(_unitOfWork.EspacioPublicoTemporalRepository.GetEspacioPublicoById(id, tramite));
        }

        // GET api/parcela/Get/5
        public EspacioPublicoTemporal Get(long id, int tramite)
        {
            return _unitOfWork.EspacioPublicoTemporalRepository.GetEspacioPublicoById(id, tramite);
        }

        [HttpGet]
        [Route("api/EspacioPublicoTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetEspaciosPublicosByTramite(int tramite)
        {
            return Ok(_unitOfWork.EspacioPublicoTemporalRepository.GetEspaciosPublicosByTramite(tramite));
        }

    }
}
