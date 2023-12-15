using System;
using System.Collections.Generic;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ValidationRules.MantenedorParcelario;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DominioController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DominioController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long idUnidadTributaria, bool esHistorico = false)
        {
            var cambioMunicipio = Int16.Parse(_unitOfWork.ParametroRepository.GetValor(35));
            return Ok(cambioMunicipio == (short)CambioMunicipio.TrelewMaipu ?
                _unitOfWork.DominioRepository.GetDominiosFromView(idUnidadTributaria) :
                _unitOfWork.DominioRepository.GetDominios(idUnidadTributaria, esHistorico));
        }

        public IHttpActionResult GetHistorico(long idUnidadTributaria)
        {
            return Ok(_unitOfWork.DominioRepository.GetDominiosHistorico(idUnidadTributaria));
        }

        public IHttpActionResult GetDominio(long id)
        {
            return Ok(_unitOfWork.DominioRepository.GetDominioById(id));
        }

        public IHttpActionResult GetDominioByDominioTitular(List<DominioTitular> domTitular)
        {
            return Ok(_unitOfWork.DominioRepository.GetDominioByDominioTitular(domTitular));
        }

        public IHttpActionResult Validate(UtDominio utDominio)
        {
            try
            {
                utDominio.OperacionesDominio.AnalyzeOperations("DominioID");

                var utd = utDominio.OperacionesDominio
                    .Find(x => x.Item.Inscripcion == utDominio.Inscripcion);

                var dominio = _unitOfWork.DominioRepository
                    .GetDominio(utDominio.UnidadTributariaId, utDominio.Inscripcion, false);

                if (utd != null)
                {
                    if (utd.Item.DominioID == utDominio.DominioId) return Ok();
                    if (utd.Operation == Operation.Remove) return Ok();
                    return new TextResult("Dominios: La inscripción ya existe", Request);
                }

                if (dominio == null) return Ok();
                if (dominio.DominioID == utDominio.DominioId) return Ok();
                return new TextResult("Dominios: La inscripción ya existe", Request);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult GetDominiosUFByParcela(long idParcela, bool esHistorico = false)
        {
            return Ok(_unitOfWork.DominioRepository.GetDominiosUFByParcela(idParcela, esHistorico));
        }
    }
}
