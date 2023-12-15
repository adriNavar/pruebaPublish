using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public TipoExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var tipoExpedienteObras = _unitOfWork.TipoExpedienteObraRepository.GetTipoExpedienteObras(id);
            return Ok(tipoExpedienteObras);
        }
        [Route("api/tipoexpedienteobra/getbyexpedienteobra")]
        public IHttpActionResult Get(long idExpedienteObra, long idTipoExpediente)
        {
            var tipoExpedienteObra = _unitOfWork.TipoExpedienteObraRepository.GetTipoExpedienteObraById(idExpedienteObra, idTipoExpediente);
            return Ok(tipoExpedienteObra);
        }

        public IHttpActionResult Post(TipoExpedienteObra tipoExpedienteObra)
        {
            //_saveObjects.Add(Operation.Add, null, tipoExpedienteObra);
            return Ok();
        }

        public IHttpActionResult Delete(long idExpedienteObra, long idTipoExpediente)
        {
            //var tipoExpedienteObra = _unitOfWork.TipoExpedienteObraRepository
            //    .GetTipoExpedienteObraById(idExpedienteObra, idTipoExpediente);
            //_saveObjects.Add(Operation.Remove, null, tipoExpedienteObra ?? new TipoExpedienteObra
            //{
            //    ExpedienteObraId = idExpedienteObra,
            //    TipoExpedienteId = idTipoExpediente
            //});
            return Ok();
        }
    }
}
