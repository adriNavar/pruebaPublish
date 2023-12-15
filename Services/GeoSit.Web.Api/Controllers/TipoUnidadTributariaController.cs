using GeoSit.Data.DAL.Common;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoUnidadTributariaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoUnidadTributariaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IHttpActionResult Get()
        {
            return Ok(_unitOfWork.TipoUnidadTributariaRepository.GetTiposUnidadTributaria());
        }

        public IHttpActionResult GetById(int id)
        {
            return Ok(_unitOfWork.TipoUnidadTributariaRepository.GetTipoUnidadTributaria(id));
        }
    }
}
