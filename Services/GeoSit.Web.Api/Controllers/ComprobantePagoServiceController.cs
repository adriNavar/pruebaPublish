using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Common;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class ComprobantePagoServiceController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ComprobantePagoServiceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET api/GetComprobantePagoId
        [ResponseType(typeof(MEComprobantePago))]
        public IHttpActionResult GetComprobantePagoById(long id)
        {
            var cp = _unitOfWork.ComprobantePagoRepository.GetById(id);
            if (cp == null)
            {
                return NotFound();
            }
            return Ok(cp);
        }
        [HttpGet]
        [Route("api/ComprobantePagoService/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetComprobantesByTramite(int tramite)
        {
            return Ok(_unitOfWork.ComprobantePagoRepository.GetByTramite(tramite));
        }
    }
}