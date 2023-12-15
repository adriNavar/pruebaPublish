using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using GeoSit.Data.DAL.Common;
using System.Web.Http.Description;
using System.Collections.Generic;
using System;

namespace GeoSit.Web.Api.Controllers
{
    public class UnidadTributariaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public UnidadTributariaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var unidades = _unitOfWork.UnidadTributariaRepository.GetUnidadesTributarias();

            if (unidades == null) return NotFound();

            return Ok(unidades);
        }
        public IHttpActionResult GetUF(long idParcela)
        {
            var unidades = _unitOfWork.UnidadTributariaRepository.GetUnidadesTributariasUF(idParcela);

            if (unidades == null) return NotFound();

            return Ok(unidades);
        }//

        public IHttpActionResult GetUnidadesTributariasByParcela(long idParcela)
        {
            var unidades = _unitOfWork.UnidadTributariaRepository.GetUnidadesTributariasByParcela(idParcela);

            if (unidades == null) return NotFound();

            return Ok(unidades);
        }

        public IHttpActionResult GetUnidadTributariaByParcela(long idParcela)
        {
            var unidades = _unitOfWork.UnidadTributariaRepository.GetUnidadTributariaByParcela(idParcela);

            if (unidades == null) return NotFound();

            return Ok(unidades);
        }

        public IHttpActionResult Get(long id, bool incluirDominios = false)
        {
            return Ok(_unitOfWork.UnidadTributariaRepository.GetUnidadTributariaById(id, incluirDominios));
        }

        public IHttpActionResult GetByPartida(string partida)
        {
            var ut = _unitOfWork.UnidadTributariaRepository.GetUnidadTributariaByPartida(partida);
            return Ok(ut);
        }

        public IHttpActionResult GetPorcentajeCopropiedadByParcela(long idParcela)
        {
            var unidades = _unitOfWork.UnidadTributariaRepository.GetUnidadesTributariasByParcela(idParcela);
            var totalPorcentajeCopropiedad = unidades.Sum(x => x.PorcentajeCopropiedad);
            return Ok(totalPorcentajeCopropiedad);
        }

        public IHttpActionResult GetRegularExpression()
        {
            return Ok(_unitOfWork.UnidadTributariaRepository.GetRegularExpression());
        }

        [HttpGet]
        //[ResponseType(typeof(List<unidad>))]
        public IHttpActionResult UnidadTributariaDom(long idUT)
        {
            var result = _unitOfWork.UnidadTributariaDomicilioRepository.GetUnidadTributuriaDombyId(idUT);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetActaDomiciliosUTbyId(long idDom)
        {
            var result = _unitOfWork.UnidadTributariaDomicilioRepository.GetUnidadTributuriaDombyIdDom(idDom);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetByIdDeclaracionJurada(long id)
        {
            var result = _unitOfWork.UnidadTributariaRepository.GetUnidadTributuriaByIdDeclaracionJurada(id);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetUnidadesTributariasByParcelas(long[] idsParcelas)
        {
            var unidades = _unitOfWork.UnidadTributariaRepository.GetUnidadesTributariasByParcelas(idsParcelas);

            if (unidades == null) return NotFound();

            return Ok(unidades);
        }

        [HttpGet]
        [Route("api/unidadtributaria/partida/jurisdiccion/{idJurisdiccion}/tipo/{idTipo}")]
        public IHttpActionResult GetNextPartida(long idTipo, long idJurisdiccion)
        {
            try
            {
                return Ok(_unitOfWork.MesaEntradasRepository.GenerarPartidaInmobiliaria(idJurisdiccion, idTipo));

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"Generar Partida Siguiente (tipo:{idTipo},jurisdiccion:{idJurisdiccion})", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetPartidaDisponible(long idUnidadTributaria, string partida)
        {
            if (_unitOfWork.UnidadTributariaRepository.ValidarPartidaDisponible(idUnidadTributaria, partida))
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }
    }
}
