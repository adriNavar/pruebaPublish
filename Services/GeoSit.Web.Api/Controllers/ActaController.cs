using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class ActaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ActaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IHttpActionResult Get(long id)
        {
            var actas = _unitOfWork.ActaRepository.GetActas(id);
            return Ok(actas);
        }

        public IHttpActionResult Get(string date)
        {
            var actas = _unitOfWork.ActaRepository.GetActasFecha(date);
            return Ok(actas);
        }

    }
}
