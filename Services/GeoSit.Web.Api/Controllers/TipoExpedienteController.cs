using System.Web.Http;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoExpedienteController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoExpedienteController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IHttpActionResult Get()
        {
            var tipoExpedientes = _unitOfWork.TipoExpedienteRepository.GetTipoExpedientes();
            return Ok(tipoExpedientes);
        }  
     

    }
}
