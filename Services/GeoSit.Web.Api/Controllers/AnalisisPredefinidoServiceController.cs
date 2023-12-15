using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class AnalisisPredefinidoServiceController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public AnalisisPredefinidoServiceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET api/GetDistritos
        [HttpGet]
        [ResponseType(typeof(ICollection<Distritos>))]
        public IHttpActionResult GetDistritos()
        {
            return Ok(_unitOfWork.AnalisisPredefinidoRepository.GetDistritos());
        }

        [HttpPost]
        [ResponseType(typeof(ICollection<CargasTecnicas>))]
        public IHttpActionResult CargarReclamos(CargasTecnicas reclamoTecnico)
        {
            _unitOfWork.AnalisisPredefinidoRepository.InsertReclamos(reclamoTecnico);
            return Ok();
        }

    }
}