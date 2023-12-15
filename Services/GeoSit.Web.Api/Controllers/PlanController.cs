using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class PlanController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PlanController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IHttpActionResult Get()
        {
            var planes = _unitOfWork.PlanRepository.GetPlanes();
            return Ok(planes);
        }
    }
}
