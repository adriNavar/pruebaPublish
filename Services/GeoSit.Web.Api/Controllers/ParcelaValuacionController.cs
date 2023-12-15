//using GeoSit.Data.BusinessEntities.Inmuebles;
//using GeoSit.Data.DAL.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace GeoSit.Web.Api.Controllers
//{
//    public class ParcelaValuacionController : ApiController
//    {
//        private readonly UnitOfWork unitOfWork;

//        public ParcelaValuacionController(UnitOfWork unitOfWork)
//        {
//            this.unitOfWork = unitOfWork;
//        }

//        // GET api/parcelavaluacion/5
//        public IHttpActionResult GetByIdParcela(int idParcela)
//        {
//            return Ok(unitOfWork.ParcelaRepository.GetParcelaValuacionByParcelaId(idParcela));
//        }                
//    }
//}
