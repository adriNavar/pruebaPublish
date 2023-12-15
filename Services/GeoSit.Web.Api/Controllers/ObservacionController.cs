using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class ObservacionController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ObservacionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var observaciones = _unitOfWork.ObservacionRepository.GetObservaciones(id);
            return Ok(observaciones);
        }
        public IHttpActionResult GetObservacionById(long id)
        {
            var observacion = _unitOfWork.ObservacionRepository.GetObservaciones(id);
            return Ok(observacion);
        }
        public IHttpActionResult Post(ObservacionExpediente observacionExpediente)
        {
            //var exist = _unitOfWork.ObservacionRepository
            //    .GetObservacionById(observacionExpediente.ObservacionExpedienteId);

            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, observacionExpediente);
            return Ok();
        }

        public IHttpActionResult Delete(long idObservacionExpediente)
        {
            //var observacionExpediente = _unitOfWork.ObservacionRepository.GetObservacionById(idObservacionExpediente);
            //_saveObjects.Add(Operation.Remove, null, observacionExpediente ?? new ObservacionExpediente
            //{
            //    ObservacionExpedienteId = idObservacionExpediente
            //});
            return Ok();
        }
    }
}
