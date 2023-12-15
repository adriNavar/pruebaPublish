using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class MensuraTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public MensuraTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetMensura(int idMensura, int tramite)
        {
            return Ok(_unitOfWork.MensuraTemporalRepository.GetMensura(idMensura, tramite));
        }

        public IHttpActionResult GetTipoMensura(long idMensura)
        {
            return Ok(_unitOfWork.MensuraTemporalRepository.GetTipoMensura(idMensura));
        }

        public IHttpActionResult GetDocumentoMensura(long idMensura)
        {
            return Ok(_unitOfWork.MensuraTemporalRepository.GetDocumentoMensura(idMensura));
        }

        [HttpGet]
        [Route("api/MensuraTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetEntradasByIdTramite(int tramite)
        {
            return Ok(_unitOfWork.MensuraTemporalRepository.GetEntradasByIdTramite(tramite));
        }

    }
}
