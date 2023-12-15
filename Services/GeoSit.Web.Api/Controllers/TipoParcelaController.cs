using GeoSit.Data.DAL.Common;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoParcelaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoParcelaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IHttpActionResult Get()
        {
            return Ok(_unitOfWork.TipoParcelaRepository.GetTipoParcelas());
        }

        public IHttpActionResult GetById(long TipoParcelaId)
        {
            return Ok(_unitOfWork.TipoParcelaRepository.GetTipoParcela(TipoParcelaId));
        }
    }
}
