using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class TiposObjetosAdministrativosController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/TiposObjetosAdministrativos
        [ResponseType(typeof(ICollection<TipoObjeto>))]
        public IHttpActionResult GetTiposObjeto()
        {
            return Ok(db.TiposObjeto.ToList());
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