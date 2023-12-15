using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    [RoutePrefix("api/layer")]
    public class LayerController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public LayerController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("listbyplantillaid/{idplantilla}")]
        public IHttpActionResult GetListByPlantillaId(int idPlantilla)
        {
            var layers = _unitOfWork.LayerRepository.GetLayersByPlantillaId(idPlantilla);
            return Ok(layers);
        }

        public IHttpActionResult Get()
        {
            var layers = _unitOfWork.LayerRepository.GetLayers().ToList();

            foreach (var layer in layers)
            {
                layer.IBytes = null;
            }

            return Ok(layers);
        }

        public IHttpActionResult Get(int id)
        {
            var layer = _unitOfWork.LayerRepository.GetLayerById(id);

            if (layer == null) return Ok();

            if (layer.PuntoImagenNombre == null) return Ok(layer);

            return Ok(layer);
        }

        public IHttpActionResult Post()
        {
            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
