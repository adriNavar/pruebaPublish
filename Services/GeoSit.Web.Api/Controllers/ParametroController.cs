using System.Web.Http;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class ParametroController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ParametroController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetParametro(long id)
        {
            return Ok(_unitOfWork.ParametroRepository.GetParametro(id));
        }
        public IHttpActionResult GetParametroByClave(string clave)
        {
            return Ok(_unitOfWork.ParametroRepository.GetParametro(clave));
        }

        public IHttpActionResult GetValor(long id)
        {
            var valor = _unitOfWork.ParametroRepository.GetValor(id);
            return new TextResult(valor, Request);
        }

        public IHttpActionResult GetParametroByDescripcion(string descripcion)
        {
            var valor = _unitOfWork.ParametroRepository.GetParametroByDescripcion(descripcion);
            return new TextResult(valor, Request);
        }
    }
}
