using System;
using System.Linq;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;
using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Web.Api.Controllers
{
    public class DomicilioExpedienteObraController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DomicilioExpedienteObraController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var dieo = _unitOfWork.DomicilioExpedienteObraRepository.GetUbicacionExpedienteObras(id);
            return Ok(dieo);
        }

        [Route("api/domicilioexpedienteobra/getbyexpedienteobra")]
        public IHttpActionResult Get(long idExpedienteObra, long idDomicilio)
        {
            var domicilioEO = _unitOfWork
                                .DomicilioExpedienteObraRepository
                                .GetDomicilioExpedienteObraById(idDomicilio, idExpedienteObra);

            return Ok(domicilioEO);
        }

        public IHttpActionResult Post(DomicilioExpedienteObra domicilioExpedienteObra)
        {
            return Ok();
        }

        [Route("api/domicilioexpedienteobra/validate")]
        public IHttpActionResult Post(long idDomicilio, Operaciones<UnidadTributariaExpedienteObra> unidadesTributarias)
        {
            var idUnidadTributaria = _unitOfWork.DomicilioExpedienteObraRepository
                .GetUnidadTributariaExpedienteObraIdByDomicilioId(idDomicilio);

            var ut = _unitOfWork.UnidadTributariaExpedienteObraRepository
                .GetUnidadTributariaExpedienteObraById(idUnidadTributaria);

            if (ut != null)
            {
                var u = _unitOfWork.UnidadTributariaRepository.GetUnidadTributariaById(ut.UnidadTributariaId);
                return new TextResult(
                    string.Format("No se puede eliminar el domicilio, elimine primero la unidad tributaria {0}",
                        u.CodigoProvincial),
                        Request);
            }
            return Ok();
        }

        [Route("api/domicilioexpedienteobra/getbyexpedienteobraprimario")]
        public IHttpActionResult GetPrimary(long idExpedienteObra)
        {
            var domicilioInmuebleExpedienteObras = _unitOfWork.DomicilioExpedienteObraRepository
                .GetDomicilioExpedienteObras(idExpedienteObra);
            var primario = domicilioInmuebleExpedienteObras.FirstOrDefault(x => x.Primario);
            return Ok(primario);
        }

    }
}
