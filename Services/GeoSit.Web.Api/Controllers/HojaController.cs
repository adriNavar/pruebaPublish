using System.Web.Http;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class HojaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public HojaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var hojas = _unitOfWork.HojaRepository.GetHojas();
            return Ok(hojas);
        }

        public IHttpActionResult Get(int id)
        {
            var hoja = _unitOfWork.HojaRepository.GetHojaById(id);
            return Ok(hoja);
        }

        public IHttpActionResult Post(Hoja hoja)
        {
            _unitOfWork.HojaRepository.InsertHoja(hoja);
            _unitOfWork.Save();
            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            _unitOfWork.HojaRepository.DeleteHoja(id);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
