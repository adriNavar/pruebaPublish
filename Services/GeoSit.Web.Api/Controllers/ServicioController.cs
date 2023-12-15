using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class ServicioController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ServicioController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var servicios = _unitOfWork.ServicioRepository.GetServicios();
            return Ok(servicios);
        }
    }
}
