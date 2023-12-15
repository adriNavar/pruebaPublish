using GeoSit.Data.DAL.Common;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class EstadoParcelaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public EstadoParcelaController(UnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        // GET api/estadoparcela
        public IHttpActionResult Get()
        {
            return Ok(_unitOfWork.EstadosParcelaRepository.GetEstadosParcela());
        }
    }
}
