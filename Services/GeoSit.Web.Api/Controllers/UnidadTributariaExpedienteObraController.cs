using System.Linq;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ValidationRules;
using GeoSit.Data.BusinessEntities.ValidationRules.ExpedientesObra;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;
using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Web.Api.Controllers
{
    public class UnidadTributariaExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public UnidadTributariaExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var unidadTributariaExpedienteObra = _unitOfWork
                                                        .UnidadTributariaExpedienteObraRepository
                                                        .GetUnidadTributariaExpedienteObrasByIdExpedienteObra(id).ToList();
            return Ok(unidadTributariaExpedienteObra);
        }

        [Route("api/unidadtributariaexpedienteobra/getbyexpedienteobra")]
        public IHttpActionResult Get(long idExpedienteObra, long idUnidadTributaria)
        {
            var uteo = _unitOfWork
                            .UnidadTributariaExpedienteObraRepository
                            .GetUnidadTributariaExpedienteObraById(idExpedienteObra, idUnidadTributaria);
            return Ok(uteo);
        }
        [Route("api/unidadtributariaexpedienteobra/validate")]
        public IHttpActionResult Post(long idExpedienteObra, long idUnidadTributaria, Operaciones<UnidadTributariaExpedienteObra> operaciones)
        {
            UnidadTributariaExpedienteObra unidadTributariaExpedienteObra = new UnidadTributariaExpedienteObra
            {
                ExpedienteObraId = idExpedienteObra,
                UnidadTributariaId = idUnidadTributaria
            };
            var unidad = _unitOfWork
                .UnidadTributariaExpedienteObraRepository
                .GetUnidadTributariaExpedienteObraById(unidadTributariaExpedienteObra.ExpedienteObraId, unidadTributariaExpedienteObra.UnidadTributariaId);

            string errors = FluentValidator<UnidadTributariaExpedienteObra>.Validate(new UnidadTributariaValidator(unidad), unidadTributariaExpedienteObra);
            if (!string.IsNullOrEmpty(errors)) return new TextResult(errors, Request);
            var operacion = operaciones.FirstOrDefault(x => x.Item.ExpedienteObraId == unidadTributariaExpedienteObra.ExpedienteObraId &&
                x.Item.UnidadTributariaId == unidadTributariaExpedienteObra.UnidadTributariaId && x.Operation == Operation.Add);

            unidad = operacion != null ? operacion.Item : null;

            errors = FluentValidator<UnidadTributariaExpedienteObra>.Validate(new UnidadTributariaValidator(unidad), unidadTributariaExpedienteObra);
            if (!string.IsNullOrEmpty(errors)) return new TextResult(errors, Request);
            return Ok();
        }

        public IHttpActionResult Delete(long idExpedienteObra, long idUnidadTributaria)
        {
            return NotFound();
        }
    }
}
