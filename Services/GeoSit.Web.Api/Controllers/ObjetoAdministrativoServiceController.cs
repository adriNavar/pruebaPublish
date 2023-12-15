using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class ObjetoAdministrativoServiceController : ApiController
    {
        private readonly GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Objeto
        [ResponseType(typeof(ICollection<Objeto>))]
        public IHttpActionResult GetObjetos()
        {
            return Ok(db.Objetos.ToList());
        }

        // GET api/GetObjetoByTipo
        [ResponseType(typeof(ICollection<Objeto>))]
        public IHttpActionResult GetObjetoByTipo(long id)
        {
            var obj = db.Objetos.Where(a => a.TipoObjetoId == id && a.FechaBaja == null).ToList();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // GET api/GetObjetoByIdPadre
        [ResponseType(typeof(ICollection<Objeto>))]
        public IHttpActionResult GetObjetoByIdPadre(long id)
        {
            List<Objeto> obj = db.Objetos.Where(a => a.ObjetoPadreId == id).OrderBy(a=>a.Nombre).ToList();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // GET api/GetObjetoByIdPadreTipo
        [ResponseType(typeof(ICollection<Objeto>))]
        public IHttpActionResult GetObjetoByIdPadreTipo(long id, long tipo)
        {
            var objs = db.Objetos.Where(a => a.ObjetoPadreId == id && a.TipoObjetoId == tipo && a.FechaBaja == null).OrderBy(a => a.Nombre).ToList();
            if (objs == null)
            {
                return NotFound();
            }
            return Ok(objs);
        }
        // GET api/GetObjetoByIdPadreTipo
        [HttpGet]
        [Route("api/objetoadministrativoservice/objetos/tipo/{tipo}/hijo/{id}/padre")]
        public IHttpActionResult GetObjetosTipoByObjetoIdPadre(long id, long tipo)
        {
            var query = from objHijo in db.Objetos
                        join objByTipo in db.Objetos on objHijo.ObjetoPadreId equals objByTipo.ObjetoPadreId
                        where objHijo.FeatId == id && objByTipo.TipoObjetoId == tipo
                        orderby objByTipo.Nombre
                        select objByTipo;

            return Ok(query.ToList());
        }
        [ResponseType(typeof(Objeto))]
        public IHttpActionResult GetObjetoByNombreTipo(string nombre, long tipo)
        {            
            return Ok(db.Objetos.FirstOrDefault(a => a.Nombre == nombre && a.TipoObjetoId == tipo));
        }

        [ResponseType(typeof(Objeto))]
        public IHttpActionResult GetObjetoById(long id)
        {
            return Ok(db.Objetos.Find(id));
        }

        [ResponseType(typeof(Objeto))]
        public IHttpActionResult GetJurisdiccionByConfiguracion()
        {
            if(long.TryParse(db.ParametrosGenerales.SingleOrDefault(p => p.Clave == "ID_JURISDICCION")?.Valor, out long id))
            {
                return Ok(db.Objetos.Find(id));
            }
            return NotFound();
        }
    }
}