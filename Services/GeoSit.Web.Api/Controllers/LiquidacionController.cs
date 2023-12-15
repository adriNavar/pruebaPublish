using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Common;
using System.Data;
using System.Linq;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class LiquidacionController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public LiquidacionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var liquidaciones = _unitOfWork.LiquidacionRepository.GetLiquidaciones(id);
            return Ok(liquidaciones);
        }
        public IHttpActionResult GetLiquidacionById(long id)
        {
            var liquidacion = _unitOfWork.LiquidacionRepository.GetLiquidacionById(id);
            return Ok(liquidacion);
        }

        public IHttpActionResult Post(Liquidacion liquidacion)
        {
            //var exist = _unitOfWork.LiquidacionRepository
            //    .GetLiquidacionById(liquidacion.LiquidacionId);

            //_saveObjects.Add(exist == null ? Operation.Add : Operation.Update, null, liquidacion);
            return Ok();
        }

        public IHttpActionResult Delete(long idLiquidacion)
        {
            //var liquidacion = _unitOfWork.LiquidacionRepository.GetLiquidacionById(idLiquidacion);
            //_saveObjects.Add(Operation.Remove, null, liquidacion ?? new Liquidacion
            //{
            //    LiquidacionId = idLiquidacion
            //});
            return Ok();
        }
        public IHttpActionResult GetParametrosGeneralesByNombre(string id)
        {
            ParametrosGenerales pg = GeoSITMContext.CreateContext().ParametrosGenerales.Where(x => x.Clave == id).FirstOrDefault();

            return Ok(pg);
        }
    }
}
