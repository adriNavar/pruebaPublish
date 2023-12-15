using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class PaisServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Pais
        [ResponseType(typeof(ICollection<Pais>))]
        public IHttpActionResult GetPaises()
        {
            return Ok(db.Pais.ToList());
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