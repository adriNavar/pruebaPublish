using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class RolController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public RolController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var roles = _unitOfWork.RolRepository.GetRoles();
            return Ok(roles);
        }
    }
}
