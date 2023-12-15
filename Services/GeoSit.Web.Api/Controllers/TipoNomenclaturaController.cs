using GeoSit.Data.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class TipoNomenclaturaController : ApiController
    {
        private readonly UnitOfWork unitOfWork;


        public TipoNomenclaturaController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        // GET api/tiponomenclatura
        public IHttpActionResult Get()
        {
            return Ok(unitOfWork.TiposNomenclaturasRepository.GetTiposNomenclatura());
        }

        public IHttpActionResult GetById(long id)
        {
            return Ok(unitOfWork.TiposNomenclaturasRepository.GetTipoNomenclaturaById(id));
        }
    }
}
