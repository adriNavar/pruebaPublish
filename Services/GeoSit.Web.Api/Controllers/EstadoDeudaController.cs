using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class EstadoDeudaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public EstadoDeudaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetYears()
        {
            return Ok(_unitOfWork.EstadoDeudaRepository.GetYears());
        }

        public IHttpActionResult GetServiciosGenerales(string padron)
        {
            return Ok(_unitOfWork.EstadoDeudaRepository.GetEstadoDeudaServiciosGenerales(padron));
        }

        public IHttpActionResult GetRentas(int year)
        {
            return Ok(_unitOfWork.EstadoDeudaRepository.GetEstadoDeudaRentas(year));
        }
    }
}
