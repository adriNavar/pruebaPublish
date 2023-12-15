using System.Web.Http;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class AuditoriaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public AuditoriaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetAuditoriaByObjeto(long idObjeto, string Objeto)
        {
            return Ok(_unitOfWork.AuditoriaRepository.GetAuditoriaByIdObjeto(idObjeto, Objeto));
        }

        public IHttpActionResult Post(Auditoria auditoria)
        {
            _unitOfWork.AuditoriaRepository.InsertAuditoria(auditoria);
            // la auditoria no guarda en auditoria
            _unitOfWork.Save(null);
            return Ok();
        }
    }
}
