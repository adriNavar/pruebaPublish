using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DesignacionController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DesignacionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetDesignacion(long idParcela)
        {
            return Ok(_unitOfWork.DesignacionRepository.GetDesignacion(idParcela));
        }

        [HttpGet]
        public IHttpActionResult GetDesignacionesParcela(long idParcela)
        {
            return Ok(_unitOfWork.DesignacionRepository.GetDesignacionesByParcela(idParcela));
        }

        [HttpGet]
        public IHttpActionResult GetTiposDesignador()
        {
            return Ok(_unitOfWork.DesignacionRepository.GetTiposDesignador());
        }
    }
}
