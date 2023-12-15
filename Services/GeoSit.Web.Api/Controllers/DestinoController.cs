using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DestinoController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DestinoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var destinos = _unitOfWork.DestinoRepository.GetDestinos();
            return Ok(destinos);
        }
    }
}
