using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.InterfaseRentas;
using GeoSit.Data.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers.InterfaseRentas
{
    public class InterfaseRentasController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly InterfaseRentasHelper _interfaseRentasHelper;

        public InterfaseRentasController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _interfaseRentasHelper = new InterfaseRentasHelper(unitOfWork);
        }

        public IHttpActionResult GetLogs()
        {
            var logs = _unitOfWork.InterfaseRentasLogRepository.GetAll().OrderByDescending(x => x.Fecha).ToArray();
            return Ok(logs);
        }

        public IHttpActionResult GetZonasTributarias()
        {
            var zonas = _interfaseRentasHelper.GetZonasTributarias();
            return Ok(zonas);
        }

        public IHttpActionResult Reprocesar(long logId)
        {
            var log = _unitOfWork.InterfaseRentasLogRepository.GetById(logId);
            if (log != null)
            {
                return Ok(_interfaseRentasHelper.Reprocesar(log));
            }
            return NotFound();
        }

        public IHttpActionResult Editar([FromBody] Parcela parcela, string partida)
        {
            var result = _interfaseRentasHelper.Modificacion(parcela, partida);
            return Ok(result);
        }

        public IHttpActionResult Avaluo(long parcelaId, string partida, int vigencia, string supm, string supt, string avalm, string avalt)
        {
            var result = _interfaseRentasHelper.Avaluo(parcelaId, partida, vigencia, supm, supt, avalm, avalt);
            return Ok(result);
        }

        public IHttpActionResult BuscarPersonas(string nombre, int doc, string cuit)
        {
            var result = _interfaseRentasHelper.BuscarPersonas(nombre, doc, cuit);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }        
    }
}
