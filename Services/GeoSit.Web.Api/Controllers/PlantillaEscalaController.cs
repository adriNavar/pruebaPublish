using System.Web.Http;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    [RoutePrefix("api/plantillaescala")]
    public class PlantillaEscalaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PlantillaEscalaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("listbyplantillaid/{idplantilla}")]
        public IHttpActionResult GetListByPlantillaId(int idPlantilla)
        {
            var plantillaEscalas = _unitOfWork.PlantillaEscalaRepository.GetPlantillaTextosByPlantillaId(idPlantilla);
            return Ok(plantillaEscalas);
        }

        public IHttpActionResult Get()
        {
            var plantillaEscalas = _unitOfWork.PlantillaEscalaRepository.GetPlantillaEscalas();
            return Ok(plantillaEscalas);
        }

        public IHttpActionResult Get(int id)
        {
            var plantillaEscala = _unitOfWork.PlantillaEscalaRepository.GetPlantillaEscalaById(id);
            return Ok(plantillaEscala);
        }

        public IHttpActionResult Post(PlantillaEscala plantillaEscala)
        {
            //var plantillaEscala = viewPlantillaEscala.GetPlantillaEscala(viewPlantillaEscala);

            //var exist = _unitOfWork.PlantillaEscalaRepository.GetPlantillaEscalaById(plantillaEscala.IdPlantillaEscala);

            return Ok(true);
        }

        public IHttpActionResult Delete(int id)
        {
            var plantillaEscala = _unitOfWork.PlantillaEscalaRepository.GetPlantillaEscalaById(id);
            //_saveObjects.Add(Operation.Remove, null, plantillaEscala ?? new PlantillaEscala { IdPlantillaEscala = id });

            return Ok(plantillaEscala);
        }


    }
}
