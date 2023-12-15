using GeoSit.Data.BusinessEntities.Via;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class TramoViaServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/GetTramosVias
        [ResponseType(typeof(ICollection<TramoVia>))]
        public IHttpActionResult GetTramosVias()
        {
            return Ok(db.TramoVia.ToList());
        }

        // GET api/GetByViaId
        [ResponseType(typeof(ICollection<TramoVia>))]
        public IHttpActionResult GetByViaId(long id)
        {
            List<TramoVia> obj = db.TramoVia.Where(a => a.ViaId == id && (a.UsuarioBajaId == 0 || a.UsuarioBajaId == null)).ToList();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // GET api/GetByObjetoPadreId
        [ResponseType(typeof(ICollection<TramoVia>))]
        public IHttpActionResult GetByObjetoPadreId(long id)
        {
            List<TramoVia> obj = db.TramoVia.Where(a => a.ObjetoPadreId == id && (a.UsuarioBajaId == 0 || a.UsuarioBajaId == null)).ToList();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // GET api/GetByObjetoPadreAndViaId
        [ResponseType(typeof(ICollection<TramoVia>))]
        public IHttpActionResult GetByObjetoPadreAndViaId(long id, long Padreid)
        {
            List<TramoVia> obj = db.TramoVia.Where(a => a.ViaId == id && a.ObjetoPadreId == Padreid && (a.UsuarioBajaId == 0 || a.UsuarioBajaId == null)).ToList();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // GET api/GetByViaIdAndAltura
        [ResponseType(typeof(TramoVia))]
        public IHttpActionResult GetByViaIdAndAltura(long id, long altura)
        {
            TramoVia obj = db.TramoVia.Where(a => a.ViaId == id && (a.UsuarioBajaId == 0 || a.UsuarioBajaId == null) && a.AlturaDesde <= altura && a.AlturaHasta >= altura).FirstOrDefault();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
    }
}