using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class LocalidadServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Localidad
        [ResponseType(typeof(ICollection<Localidad>))]
        public IHttpActionResult GetLocalidades()
        {
            return Ok(db.Localidad.ToList());
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