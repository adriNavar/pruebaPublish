using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using Oracle.ManagedDataAccess.Client;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.Enums;

namespace GeoSit.Web.Api.Controllers
{
    public class DivisionServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Division
        [ResponseType(typeof(ICollection<Division>))]
        public IHttpActionResult GetNomenclaturas()
        {
            return Ok(db.Divisiones.Where(a => (a.UsuarioBajaId == null || a.UsuarioBajaId == 0)).ToList());
        }

        // GET api/DivisionById
        [ResponseType(typeof(Division))]
        public IHttpActionResult GetDivisionById(long id)
        {
            Division Divi = db.Divisiones.Where(a => a.FeatId == id).FirstOrDefault();
            if (Divi == null)
            {
                return NotFound();
            }

            return Ok(Divi);
        }

        [ResponseType(typeof(ICollection<Division>))]
        public IHttpActionResult GetNomenclaturaByNomenclatura(string id)
        {
            List<Division> Divi = db.Divisiones.Where(a => a.Nomenclatura == id).ToList();
            if (Divi == null)
            {
                return NotFound();
            }
            return Ok(Divi);
        }

        [ResponseType(typeof(ICollection<Division>))]
        public IHttpActionResult GetDivisionByNomenAndTipo(string nomenclatura, long tipo)
        {
            List<Division> Divi = db.Divisiones.Where(a => a.Nomenclatura == nomenclatura && a.TipoDivisionId == tipo).ToList();
            if (Divi == null)
            {
                return NotFound();
            }
            return Ok(Divi);
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult GetGeometryWKTByNomenclatura(string nomenclatura)
        {
            using (var db = GeoSITMContext.CreateContext())
            using (var builder = db.CreateSQLQueryBuilder())
            {
                return Ok(db.Database
                            .SqlQuery<string>(builder.AddTable("oa_division", "t1")
                                                     .AddFilter("nomenclatura", string.Format("'{0}'", nomenclatura), SQLOperators.EqualsTo)
                                                     .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo() { Campo = "geometry" }, "t1").ToWKT(), "geom").ToString())
                            .FirstOrDefault());
            }

        }

    }
}