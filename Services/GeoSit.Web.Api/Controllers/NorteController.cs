using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Web.Api.Controllers
{
    [RoutePrefix("api/norte")]
    public class NorteController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public NorteController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("gethttp")]
        public IHttpActionResult GetHttp()
        {
            var nortes = _unitOfWork.NorteRepository.GetNortes();
            nortes.ForEach(n =>
            {
                n.SBytes = Encoding.Default.GetString(n.IBytes);
                n.IBytes = null;
            });
            return Ok(nortes);
        }

        public IHttpActionResult Get()
        {
            var nortes = _unitOfWork.NorteRepository.GetNortes();
            return Ok(nortes);
        }

        public IHttpActionResult Get(int id)
        {
            var norte = _unitOfWork.NorteRepository.GetNorteById(id);
            return Ok(norte);
        }

        public IHttpActionResult Post(Norte norte)
        {
            _unitOfWork.NorteRepository.InsertNorte(norte);
            _unitOfWork.Save();
            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            _unitOfWork.NorteRepository.DeleteNorte(id);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
