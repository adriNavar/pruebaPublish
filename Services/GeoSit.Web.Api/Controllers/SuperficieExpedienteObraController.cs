using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class SuperficieExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public SuperficieExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var superficies = _unitOfWork.TipoSuperficieExpedienteObraRepository.GetTipoSuperficieExpedienteObras(id);
            return Ok(superficies);
        }

        public IHttpActionResult GetById(long id)
        {
            var superficie = _unitOfWork.TipoSuperficieExpedienteObraRepository.GetTipoSuperficieExpedienteObraById(id);
            return Ok(superficie);
        }

        public IHttpActionResult Post(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra)
        {
            //var exist = _unitOfWork.TipoSuperficieExpedienteObraRepository
            //    .GetTipoSuperficieExpedienteObraById(tipoSuperficieExpedienteObra.ExpedienteObraSuperficieId);

            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, tipoSuperficieExpedienteObra);

            return Ok();
        }

        public IHttpActionResult Delete(long idExpedienteObraSuperficie)
        {
            //var tipoSuperficieExpedienteObra = _unitOfWork.TipoSuperficieExpedienteObraRepository
            //    .GetTipoSuperficieExpedienteObraById(idExpedienteObraSuperficie);
            //_saveObjects.Add(Operation.Remove, null, tipoSuperficieExpedienteObra ?? new TipoSuperficieExpedienteObra
            //{
            //    ExpedienteObraSuperficieId = idExpedienteObraSuperficie
            //});
            return Ok();
        }
    }
}
