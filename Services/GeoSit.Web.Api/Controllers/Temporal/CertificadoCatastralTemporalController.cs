using System.Web.Http;
using GeoSit.Data.DAL.Common;


namespace GeoSit.Web.Api.Controllers
{
    public class CertificadoCatastralTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public CertificadoCatastralTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetCertificado(int idCertificado, int tramite)
        {
            return Ok(_unitOfWork.CertificadoCatastralTemporalRepository.GetCertificado(idCertificado, tramite));
        }

        [HttpGet]
        [Route("api/CertificadoCatastralTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetCertificadosByTramite(int tramite)
        {
            return Ok(_unitOfWork.CertificadoCatastralTemporalRepository.GetCertificadosByTramite(tramite));
        }
    }
}
