using System.Web.Http;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Web.Api.Controllers
{
    public class NomenclaturaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        public NomenclaturaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //_saveObjects = saveObjects;
        }

        public IHttpActionResult Get(string nomenclatura)
        {
            return Ok(_unitOfWork.NomenclaturaRepository.GetNomenclatura(nomenclatura));
        }

        public IHttpActionResult GetByIdParcelaIdTipoNomenclatura(long idParcela, long idTipoNomenclatura)
        {
            return Ok(_unitOfWork.NomenclaturaRepository.GetNomenclatura(idParcela, idTipoNomenclatura));
        }

        [HttpGet]
        public IHttpActionResult Generar(long idParcela, long tipo)
        {
            return Ok(_unitOfWork.NomenclaturaRepository.Generar(idParcela, tipo));
        }
        //public IHttpActionResult Post(Nomenclatura nomenclatura)
        //{
        //    _saveObjects.Add(Operation.Add, null, nomenclatura);
        //    return Ok();
        //}

        //public IHttpActionResult Put(Nomenclatura nomenclatura)
        //{
        //    _saveObjects.Add(Operation.Update, null, nomenclatura);
        //    return Ok();
        //}

        //[HttpPost]
        //public IHttpActionResult DeleteNomenclatura(Nomenclatura nomenclatura)
        //{

        //    //Nomenclatura nomenclatura = _unitOfWork.NomenclaturaRepository.GetNomenclaturaById(id);
        //    _saveObjects.Add(Operation.Remove, null, nomenclatura);
        //    return Ok();
        //}
    }
}
