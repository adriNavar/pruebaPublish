﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class ProvinciaServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Provincia
        [ResponseType(typeof(ICollection<Provincia>))]
        public IHttpActionResult GetProvincias()
        {
            List<Provincia> lista =  db.Provincia.OrderBy(a => a.Descripcion).ToList();
            return Ok(lista);
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