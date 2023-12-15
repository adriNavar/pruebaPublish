using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.Intercambio.BusinessEntities;
using GeoSit.Data.Intercambio.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers.InterfaseProvincial
{
    public class InterfaseProvincialController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public InterfaseProvincialController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [ResponseType(typeof(ICollection<Municipio>))]
        [Route("api/InterfaseProvincial/Municipios")]
        public IHttpActionResult GetMunicipios()
        {
            try
            {
                using (var db = IntercambioContext.CreateContext())
                {
                    return Ok(db.Municipios
                        .OrderBy(x => x.Nombre)
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("InterfaseProvincial/Municipios", ex);
                return InternalServerError(ex);
            }
        }
    }
}
