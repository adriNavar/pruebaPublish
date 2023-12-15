using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoDocumentoServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/TipoProfesion
        [ResponseType(typeof(ICollection<TipoDocumento>))]
        public IHttpActionResult GetTiposDocumento()
        {
            return Ok(db.TipoDocumento.Where(x => x.FechaBaja == null).ToList());
        }

        // GET api/GetTipoDocumentoById
        [ResponseType(typeof(TipoDocumento))]
        public IHttpActionResult GetTipoDocumentoById(long id)
        {
            return Ok(db.TipoDocumento.Where(a => a.TipoDocumentoId == id).FirstOrDefault());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}