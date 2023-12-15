using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoPersonaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoPersonaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            return Ok(_unitOfWork.TipoPersonaRepository.GetTipoPersonas());
        }
    }
}
