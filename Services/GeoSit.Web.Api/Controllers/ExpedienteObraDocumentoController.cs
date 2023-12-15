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
    public class ExpedienteObraDocumentoController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ExpedienteObraDocumentoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long idExpedienteObra)
        {
            var documentoExpedienteObras = _unitOfWork.ExpedienteObraDocumentoRepository
                .GetExpedienteObraDocumentos(idExpedienteObra);
            return Ok(documentoExpedienteObras);
        }
        public IHttpActionResult GetDocumentoExpedienteObra(long idExpedienteObra, long idDocumento)
        {
            var documento = _unitOfWork.ExpedienteObraDocumentoRepository.GetExpedienteObraDocumentoById(idExpedienteObra, idDocumento);
            return Ok(documento);
        }
        [Route("api/expedienteobradocumento/validate")]
        public IHttpActionResult Post(long idExpedienteObra, long idDocumento, Operaciones<ExpedienteObraDocumento> operaciones)
        {
            var documentoEO = new ExpedienteObraDocumento
            {
                ExpedienteObraId = idExpedienteObra,
                DocumentoId = idDocumento
            };
            var documento = _unitOfWork.ExpedienteObraDocumentoRepository.GetExpedienteObraDocumentoById(documentoEO.ExpedienteObraId, documentoEO.DocumentoId);
            string errors = FluentValidator<ExpedienteObraDocumento>.Validate(new DocumentoValidator(documento), documentoEO);
            if (!string.IsNullOrEmpty(errors)) return new TextResult(errors, Request);

            var item = operaciones.FirstOrDefault(p => p.Item.ExpedienteObraId == documentoEO.ExpedienteObraId && 
                                                       p.Item.DocumentoId == documentoEO.DocumentoId);
            if (item != null)
            {
                errors = FluentValidator<ExpedienteObraDocumento>.Validate(new DocumentoValidator(documento), documentoEO);
            }
            if (errors.Any()) return new TextResult(errors, Request);
            return Ok();
        }

        public IHttpActionResult Delete(long idExpedienteObra, long idDocumento)
        {
            return Ok();
        }
    }
}
