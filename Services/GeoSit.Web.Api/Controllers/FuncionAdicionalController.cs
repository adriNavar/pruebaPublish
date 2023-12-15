using System.Web.Http;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class FuncionAdicionalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public FuncionAdicionalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var funcionAdicionals = _unitOfWork.FuncionAdicionalRepository.GetFuncionAdicionales();
            return Ok(funcionAdicionals);
        }

        public IHttpActionResult Get(int id)
        {
            var funcionAdicional = _unitOfWork.FuncionAdicionalRepository.GetFuncionAdicionalById(id);
            return Ok(funcionAdicional);
        }

        public IHttpActionResult Post(FuncionAdicional funcionAdicional)
        {
            _unitOfWork.FuncionAdicionalRepository.InsertFuncionAdicional(funcionAdicional);
            _unitOfWork.Save();
            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            _unitOfWork.FuncionAdicionalRepository.DeleteFuncionAdicional(id);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
