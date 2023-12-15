using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoSuperficieController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoSuperficieController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IHttpActionResult Get()
        {
            var tipos = _unitOfWork.TipoSuperficieRepository.GetTipoSuperficies();
            return Ok(tipos);
        }
    }
}
