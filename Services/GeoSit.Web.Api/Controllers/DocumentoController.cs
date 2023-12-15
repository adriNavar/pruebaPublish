using System.Configuration;
using System.IO;
using System.Web.Http;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DocumentoController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public DocumentoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var documento = _unitOfWork.DocumentoRepository.GetDocumentoById(id);
            return Ok(documento);
        }

        public IHttpActionResult GetByIdTipo(long id)
        {
            var documento = _unitOfWork.DocumentoRepository.GetDocumentosByTipo(id);
            return Ok(documento);
        }

        [Route("api/documento/file/{id}")]
        public IHttpActionResult GetArchivo(long id)
        {
            var documento = _unitOfWork.DocumentoRepository.GetDocumentoById(id);
            var path = Path.Combine(_uploadPath, documento.nombre_archivo);
            if (documento.contenido == null) return new TextResult(string.Empty, Request);
            File.WriteAllBytes(path, documento.contenido);
            return new TextResult(documento.nombre_archivo, Request);
        }
    }
}
