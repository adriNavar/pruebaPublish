using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class NacionalidadServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Nacionalidad
        [ResponseType(typeof(ICollection<Nacionalidad>))]
        public IHttpActionResult GetNacionalidades()
        {
            return Ok(db.Nacionalidad.Where(x=>x.FechaBaja == null).ToList());
        }

        [ResponseType(typeof(Nacionalidad))]
        public IHttpActionResult GetNacionalidadById(long id)
        {
            Nacionalidad nacionalidad = db.Nacionalidad.Where(a => a.NacionalidadId == id).FirstOrDefault();
            if (nacionalidad == null)
            {
                return NotFound();
            }
            db.Entry(nacionalidad).Reference(d => d.Descripcion).Load();
            return Ok(nacionalidad);
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