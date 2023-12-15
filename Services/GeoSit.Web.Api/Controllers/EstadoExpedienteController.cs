using System.Web.Http;
using GeoSit.Data.DAL.Common;
using System;

namespace GeoSit.Web.Api.Controllers
{
    public class EstadoExpedienteController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public EstadoExpedienteController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IHttpActionResult Get()
        {
            var estados = _unitOfWork.EstadoExpedienteRepository.GetEstadoExpedientes();
            return Ok(estados);
        }
        public IHttpActionResult GetEstado(long idExpedienteObra, long idEstado, string fechaEstado)
        {
            var fecha = DateTime.Parse(fechaEstado);
            return Ok(_unitOfWork.EstadoExpedienteObraRepository.GetEstadoExpedienteObraById(idExpedienteObra, idEstado, fecha));
        }
    }
}
