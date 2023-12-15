using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class NomenclaturaServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Nomenclatura
        [ResponseType(typeof(ICollection<Nomenclatura>))]
        public IHttpActionResult GetNomenclaturas()
        {
            return Ok(db.Nomenclaturas.Where(a => (a.UsuarioBajaID == null || a.UsuarioBajaID == 0)).ToList());
        }

        // GET api/NomenclaturaById
        [ResponseType(typeof(Nomenclatura))]
        public IHttpActionResult GetNomenclaturaById(long id)
        {
            Nomenclatura Nomen = db.Nomenclaturas.Where(a => a.NomenclaturaID == id).FirstOrDefault();
            if (Nomen == null)
            {
                return NotFound();
            }

            return Ok(Nomen);
        }

        [ResponseType(typeof(ICollection<Nomenclatura>))]
        public IHttpActionResult GetNomenclaturaByNombre(string id)
        {
            List<Nomenclatura> Nomen = db.Nomenclaturas.Where(a => (a.Nombre.ToUpper().Contains(id.ToUpper()))).ToList();
            if (Nomen == null)
            {
                return NotFound();
            }
            return Ok(Nomen);
        }
    }
}