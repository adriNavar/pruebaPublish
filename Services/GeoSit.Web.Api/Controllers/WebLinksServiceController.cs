using GeoSit.Data.BusinessEntities.SubSistemaWeb;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class WebLinksServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/WebLinks
        [ResponseType(typeof(ICollection<WebLinks>))]
        public IHttpActionResult GetWebLinks()
        {
            return Ok(db.WebLinks.ToList());
        }


    }
}