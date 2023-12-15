using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class ServicioExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ServicioExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var servicioExpedienteObras = _unitOfWork.ServicioExpedienteObraRepository
                .GetServicioExpedienteObras(id);
            return Ok(servicioExpedienteObras);
        }
        public IHttpActionResult GetServicioByExpedienteObra(long idExpedienteObra, long idServicioExpediente)
        {
            var servicioExpedienteObra = _unitOfWork.ServicioExpedienteObraRepository
                .GetServicioExpedienteObraById(idExpedienteObra, idServicioExpediente);
            return Ok(servicioExpedienteObra);
        }
        public IHttpActionResult Post(ServicioExpedienteObra servicioExpedienteObra)
        {
            return Ok();
        }

        public IHttpActionResult Delete(long idExpedienteObra, long idServicioExpediente)
        {
            //var servicioExpedienteObra = _unitOfWork.ServicioExpedienteObraRepository
            //    .GetServicioExpedienteObraById(idExpedienteObra, idServicioExpediente);
            //_saveObjects.Add(Operation.Remove, null, servicioExpedienteObra ?? new ServicioExpedienteObra
            //{
            //    ExpedienteObraId = idExpedienteObra,
            //    ServicioId = idServicioExpediente
            //});
            return Ok();
        }
    }
}
