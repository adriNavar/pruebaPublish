using System;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class EstadoExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public EstadoExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var estados = _unitOfWork.EstadoExpedienteObraRepository.GetEstadoExpedienteObras(id);
            return Ok(estados);
        }

        public IHttpActionResult Post(/*SaveObjects saveObjects*/)
        {
            return Ok();
        }

        public IHttpActionResult Delete(long idExpedienteObra, long idEstadoExpediente)
        {
            return Ok();
        }
    }
}
