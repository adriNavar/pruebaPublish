using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Via;
using GeoSit.Data.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class ViaServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/GetVias
        [ResponseType(typeof(ICollection<Via>))]
        public IHttpActionResult GetVias()
        {
            return Ok(db.Via.ToList());
        }

        // GET api/GetViaById
        [ResponseType(typeof(Via))]
        public IHttpActionResult GetViaById(long id)
        {
            Via obj = db.Via.FirstOrDefault(a => a.ViaId == id);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }


        // GET api/GetViayTipo
        [ResponseType(typeof(ICollection<Via>))]
        public IHttpActionResult GetViayTipo(long id)
        {
            List<Via> obj = db.Via.Where(a => a.TipoViaId == id && a.FechaBaja == null).ToList();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // GET api/GetViaByNombre
        [ResponseType(typeof(ICollection<Via>))]
        public IHttpActionResult GetViaByNombre(string id)
        {
            List<Via> obj = db.Via.Where(a => a.FechaBaja == null && a.Nombre.ToUpper().Contains(id.ToUpper())).ToList();
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        [HttpGet]
        public IHttpActionResult GetViaByNombreYLocalidad(string nombre, long idLocalidad)
        {
            nombre = nombre ?? string.Empty;

            var query = from via in db.Via
                        where via.FechaBaja == null &&
                              via.Nombre.ToLower().Contains(nombre.ToLower()) &&
                              (from tramo in db.TramoVia
                               where tramo.FechaBaja == null &&
                                     tramo.ObjetoPadreId == idLocalidad &&
                                     tramo.ViaId == via.ViaId
                               select 1).Any()
                        orderby via.Nombre
                        select via;
            return Ok(query.ToList());
        }

        [HttpGet]
        [Route("api/ViaService/TiposVias")]
        public IHttpActionResult GetTiposVias()
        {
            return Ok(db.TiposVias.Where(x => x.FechaBaja == null));
        }

        [HttpGet]
        [Route("api/ViaService/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetViasByTramite(int tramite)
        {
            int tipoEntradaVia = Convert.ToInt32(Entradas.Via);

            var query = from entradaTramite in db.TramitesEntradas
                        join objetoEntrada in db.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join via in db.Via on entradaTramite.IdObjeto.Value equals via.ViaId
                        where objetoEntrada.IdEntrada == tipoEntradaVia && tramite == entradaTramite.IdTramite
                        select via;

            return Ok(query.ToList());
        }
    }


}