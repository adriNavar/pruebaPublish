using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class TiposDivisionesAdministrativasController : ApiController
    {
        // GET api/TiposDivisionesAdministrativas
        [ResponseType(typeof(ICollection<TipoDivision>))]
        public IHttpActionResult GetTiposDivision()
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                return Ok(db.TiposDivision.ToList());
            }
        }
    }
}