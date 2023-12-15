using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class AtributoController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public AtributoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            return Ok(_unitOfWork.AtributoRepository.GetAtributos());
        }

        public IHttpActionResult Get(int id)
        {
            return Ok(_unitOfWork.AtributoRepository.GetAtributoById(id));
        }

        public IHttpActionResult Post(Atributo atributo)
        {
            _unitOfWork.AtributoRepository.InsertAtributo(atributo);
            _unitOfWork.Save();
            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            _unitOfWork.AtributoRepository.DeleteAtributo(id);
            _unitOfWork.Save();
            return Ok();
        }

        public IHttpActionResult GetVisiblesByComponente(long id)
        {
            return Ok(_unitOfWork.AtributoRepository.GetVisiblesByComponente(id));
        }
        public IHttpActionResult GetByComponente(long id)
        {
            return Ok(_unitOfWork.AtributoRepository.GetAtributosByIdComponente(id));
        }
    }
}
