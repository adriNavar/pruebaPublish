using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class ControlTecnicoExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ControlTecnicoExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var controles = _unitOfWork.ControlTecnicoRepository.GetControlTecnicos(id);
            return Ok(controles);
        }
        public IHttpActionResult GetControlTecnicoById(long id)
        {
            var controlTecnico = _unitOfWork.ControlTecnicoRepository.GetControlTecnicoById(id);
            return Ok(controlTecnico);
        }
        public IHttpActionResult Post()
        {
            return Ok();
        }

        public IHttpActionResult Delete(long idControlTecnico)
        {
            return Ok();
        }
    }
}
