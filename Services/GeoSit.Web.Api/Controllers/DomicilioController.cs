using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DomicilioController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DomicilioController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(string nombreVia)
        {
            var domicilioInmuebles = _unitOfWork.DomicilioRepository.GetDomicilioInmuebles(nombreVia);
            return Ok(domicilioInmuebles);
        }

        public IHttpActionResult Get(long id)
        {
            var domicilio = _unitOfWork.DomicilioRepository.GetDomicilioInmuebleById(id);
            return Ok(domicilio);
        }

        public IHttpActionResult GetByUnidadTributaria(long id)
        {
            var domicilio = _unitOfWork.DomicilioRepository.GetDomicilioByUnidadTributariaId(id);

            return Ok(domicilio);
        }

        public IHttpActionResult GetDomiciliosByUnidadTributariaId(long idUnidadTributaria)
        {
            var domicilios = _unitOfWork.DomicilioRepository.GetDomiciliosByUnidadTributariaId(idUnidadTributaria);
            return Ok(domicilios);
        }

    }
}
