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
    public class UnidadTributariaDocumentoController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        
        public UnidadTributariaDocumentoController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public IHttpActionResult Post(UnidadTributariaDocumento UnidadTriutariaDocumento)
        //{
        //    try
        //    {
        //        _saveObjects.Add(Operation.Add, null, UnidadTriutariaDocumento);
        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return NotFound();
        //    }
        //}

        //public IHttpActionResult Delete(int id)
        //{
        //    try
        //    {
        //        UnidadTributariaDocumento unidadTributariaDocumento = _unitOfWork.UnidadTributariaDocumentoRepository.GetUnidadTributariaDocumentoByID(id);
        //        unidadTributariaDocumento.FechaBaja = DateTime.Now;
        //        unidadTributariaDocumento.UsuarioBajaID = 1;
        //        unidadTributariaDocumento.FechaModificacion = DateTime.Now;
        //        unidadTributariaDocumento.UsuarioModificacionID = 1;
        //        _saveObjects.Add(Operation.Remove, null, unidadTributariaDocumento);                
        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return NotFound();
        //    }
        //}
    }
}
