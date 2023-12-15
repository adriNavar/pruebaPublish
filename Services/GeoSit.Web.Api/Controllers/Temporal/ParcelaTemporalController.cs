using System;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.DAL.Common;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using GeoSit.Web.Api.Controllers.InterfaseRentas;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Web.Api.Ploteo;
using GeoSit.Data.BusinessEntities.Temporal;

namespace GeoSit.Web.Api.Controllers
{
    public class ParcelaTemporalController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public ParcelaTemporalController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult GetParcelaById(long id, int tramite)
        {
            return Ok(_unitOfWork.ParcelaTemporalRepository.GetParcelaById(id,tramite));
        }

        public IHttpActionResult Get(long id, int tramite)
        {
            return Ok(_unitOfWork.ParcelaTemporalRepository.GetParcelaById(id, tramite));
        }

        [HttpGet]
        [Route("api/ParcelaTemporal/Tramite/{tramite}/Entradas")]
        public IHttpActionResult GetEntradasByIdTramite(int tramite)
        {
            return Ok(_unitOfWork.ParcelaTemporalRepository.GetEntradasByIdTramite(tramite));
        }
    }
}
