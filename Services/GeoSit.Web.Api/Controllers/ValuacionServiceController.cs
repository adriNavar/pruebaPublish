using GeoSit.Data.DAL.Common;
using System;
using System.Net;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class ValuacionServiceController : ApiController
    {
        private readonly UnitOfWork unitOfWork;
        public ValuacionServiceController()
        {
            unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public IHttpActionResult GetValuacionParcela(long id, bool esHistorico = false)
        {
            try
            {
                return Ok(unitOfWork.ParcelaRepository.GetValuacionParcela(id, esHistorico));
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.InternalServerError, "Error al recuperar la valuación");
            }
        }

        [HttpGet]
        public IHttpActionResult GetValuacionUnidadTributaria(long id)
        {
            try
            {
                return Ok(unitOfWork.DeclaracionJuradaRepository.GetValuacionVigente(id));
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.InternalServerError, "Error al recuperar la valuación");
            }
        }
    }
}