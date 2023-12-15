using System;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ValidationRules.MantenedorParcelario;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class DominioTitularController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public DominioTitularController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long idDominio, long idPersona)
        {
            return Ok(_unitOfWork.DominioTitularRepository.GetDominioTitularById(idDominio, idPersona));
        }

        public IHttpActionResult Get(long idDominio)
        {
            var cambioMunicipio = Int16.Parse(_unitOfWork.ParametroRepository.GetValor(35));
            return Ok(cambioMunicipio == (short)CambioMunicipio.TrelewMaipu ?
                _unitOfWork.DominioTitularRepository.GetDominioTitularesFromView(idDominio) :
                _unitOfWork.DominioTitularRepository.GetDominioTitulares(idDominio));
        }
        public IHttpActionResult GetDominioTitularByTitularId(long idPersona)
        {
            return Ok(_unitOfWork.DominioTitularRepository.GetDominioTitularByTitularId(idPersona));
        }

        public IHttpActionResult Validate(DomTitular domTitular)
        {
            try
            {
                domTitular.OperacionesDominioTitular.AnalyzeOperations("PersonaID");

                var dt = domTitular.OperacionesDominioTitular
                    .Find(x => x.Item.DominioID == domTitular.DominioId &&
                        x.Item.PersonaID == domTitular.PersonaId);

                if (dt != null)
                    return new TextResult("Titular: El titular ya existe para este dominio", Request);

                var dominioTitular = _unitOfWork.DominioTitularRepository.GetDominioTitularById(domTitular.DominioId, domTitular.PersonaId);

                if (dominioTitular != null)
                    return new TextResult("Titular: El titular ya existe para este dominio", Request);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
