using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class UnidadTributariaDomicilioController : ApiController
    {
        private readonly UnitOfWork unitOfWork;
        //private readonly SaveObjects saveObjects;

        public UnidadTributariaDomicilioController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            //this.saveObjects = saveObjects;
        }
        //public void Post(UnidadTributariaDomicilio unidadTributariaDomicilio)
        //{
        //    saveObjects.Add(Operation.Add, null, unidadTributariaDomicilio);
        //}
        //public void Delete(int id)
        //{
        //}
    }
}
