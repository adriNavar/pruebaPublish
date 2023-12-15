using System.Web.Http;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    [RoutePrefix("api/plantillatexto")]
    public class PlantillaTextoController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PlantillaTextoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("listbyplantillaid/{idplantilla}")]
        public IHttpActionResult GetListByPlantillaId(int idPlantilla)
        {
            var plantillaTextos = _unitOfWork.PlantillaTextoRepository.GetPlantillaTextosByPlantillaId(idPlantilla);
            return Ok(plantillaTextos);
        }

        public IHttpActionResult Get()
        {
            var plantillaTextos = _unitOfWork.PlantillaTextoRepository.GetPlantillaTextos();
            return Ok(plantillaTextos);
        }

        public IHttpActionResult Get(int id)
        {
            var plantillaTexto = _unitOfWork.PlantillaTextoRepository.GetPlantillaTextoById(id);
            return Ok(plantillaTexto);
        }

        public IHttpActionResult Post(/*ViewPlantillaTexto viewPlantillaTexto*/)
        {
            //var plantillaTexto = viewPlantillaTexto.GetPlantillaTexto();

            //var exist = _unitOfWork.PlantillaTextoRepository.GetPlantillaTextoById(plantillaTexto.IdPlantillaTexto);
            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, plantillaTexto);

            return Ok();
        }
        

        public IHttpActionResult Delete(int id)
        {
            //var plantillaTexto = _unitOfWork.PlantillaTextoRepository.GetPlantillaTextoById(id);
            //_saveObjects.Add(Operation.Remove, null, plantillaTexto ?? new PlantillaTexto { IdPlantillaTexto = id });

            return Ok();
        }
    }
}
