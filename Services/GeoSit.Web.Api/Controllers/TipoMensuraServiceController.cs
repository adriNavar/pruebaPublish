using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoMensuraServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/TipoMensura
        [ResponseType(typeof(ICollection<TipoMensura>))]
        public IHttpActionResult GetTiposMensura()
        {
            return Ok(db.TipoMensura.Where(x => x.FechaBaja == null).OrderBy(x => x.Descripcion).ToList());
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
