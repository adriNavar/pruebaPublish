using System.Web.Http;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class PlantillaCategoriaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PlantillaCategoriaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var plantillaCategorias = _unitOfWork.PlantillaCategoriaRepository.GetPlantillaCategorias();
            return Ok(plantillaCategorias);
        }

        public IHttpActionResult Get(int id)
        {
            var plantillaCategoria = _unitOfWork.PlantillaCategoriaRepository.GetPlantillaCategoriaById(id);
            return Ok(plantillaCategoria);
        }
    }
}
