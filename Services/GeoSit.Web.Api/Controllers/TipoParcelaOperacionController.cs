using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoParcelaOperacionController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoParcelaOperacionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            return Ok(_unitOfWork.TipoParcelaOperacionRepository.GetTipoParcelaOperaciones());
        }
    }
}
