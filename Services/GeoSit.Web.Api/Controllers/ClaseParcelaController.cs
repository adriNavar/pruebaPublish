using GeoSit.Data.DAL.Common;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class ClaseParcelaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ClaseParcelaController(UnitOfWork unitOfwork)
        {
            _unitOfWork = unitOfwork;
        }

        public IHttpActionResult Get()
        {
            return Ok(_unitOfWork.ClaseParcelaRepository.GetClasesParcelas());
        }       
    }
}
