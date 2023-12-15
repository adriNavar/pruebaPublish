using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DocumentoServiceController : ApiController
    {
        private readonly GeoSITMContext db = GeoSITMContext.CreateContext();
        private readonly UnitOfWork _unitOfWork;

        public DocumentoServiceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET api/1
        [ResponseType(typeof(Documento))]
        public IHttpActionResult GetDocumentoById(long id)
        {
            var doc = db.Documento.Include(d => d.Tipo).FirstOrDefault(a => a.id_documento == id);
            if (doc != null)
            {
                return Ok(doc);
            }
            return NotFound();
        }

        [HttpPost]
        public IHttpActionResult SetDocumento_Save(Documento documento)
        {
            try
            {
                return Ok(_unitOfWork.DocumentoRepository.Save(documento));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetUrl()
        {
            string url = db.ParametrosGenerales.FirstOrDefault(a => a.Clave.Contains("DOC_ADJUNTAR_URL"))?.Valor;//Actualizar esto - Tiene que ser identificado por Clave : DOC_ADJUNTA_URL -  where m.Name.Contains(key)
            return Ok(url);
        }

        [HttpGet]
        public IHttpActionResult GetDataBaseState()
        {
            string url = db.ParametrosGenerales.Where(a => a.Clave.Contains("DOC_ADJUNTAR_DB")).Select(a => a.Valor).FirstOrDefault();//Actualizar esto - Tiene que ser identificado por Clave : DOC_ADJUNTA_DB -  where m.Name.Contains(key)
            return Ok(url);
        }

        [HttpGet]
        [Route("api/DocumentoService/{id}/File")]
        public IHttpActionResult GetFileContent(long id)
        {
            try
            {
                return Ok(new UnitOfWork().DocumentoRepository.GetContent(id));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"DocumentoService-GetFileContent({id})", ex);
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/DocumentoService/{id}/FilePdf")]
        public IHttpActionResult GetAyudaLinea(long id)
        {
            try
            {
                return Ok(new UnitOfWork().DocumentoRepository.GetAyudaLineaPdf(id));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"DocumentoService-GetFileContent({id})", ex);
                return NotFound();
            }
        }
    }
}
