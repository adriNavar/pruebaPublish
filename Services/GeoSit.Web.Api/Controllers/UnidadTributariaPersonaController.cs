using System;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ValidationRules.MantenedorParcelario;
using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Common;

namespace GeoSit.Web.Api.Controllers
{
    public class UnidadTributariaPersonaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public UnidadTributariaPersonaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long idUnidadTributaria, long idPersona)
        {
            return Ok(_unitOfWork.UnidadTributariaPersonaRepository.GetUnidadTributariaPersonaById(idUnidadTributaria, idPersona));
        }

        public IHttpActionResult Get(long idUnidadTributaria)
        {
            var cambioMunicipio = Int16.Parse(_unitOfWork.ParametroRepository.GetValor(35));
            return Ok(cambioMunicipio == (short)CambioMunicipio.TrelewMaipu ?
                _unitOfWork.UnidadTributariaPersonaRepository.GetUnidadTributariaResponsablesFiscalesFromView(idUnidadTributaria) :
                _unitOfWork.UnidadTributariaPersonaRepository.GetUnidadTributariaResponsablesFiscales(idUnidadTributaria));
        }

        [HttpPost]
        public IHttpActionResult AsignarResponsableFiscal(ResponsableFiscal responsableFiscal)
        {
            var utp = _unitOfWork.UnidadTributariaPersonaRepository.GetUnidadTributariaPersonaById(responsableFiscal.UnidadTributariaId, responsableFiscal.PersonaId);
            if (utp != null)
            {
                utp.CodSistemaTributario = responsableFiscal.CodSistemaTributario;
                utp.PersonaSavedId = responsableFiscal.PersonaId;
                
                _unitOfWork.UnidadTributariaPersonaRepository.UpdateUnidadTributariaPersona(utp);
                _unitOfWork.Save();
                return Ok();
            }
            return NotFound();
        }

        public IHttpActionResult Validate(UtPersona utPersona)
        {
            try
            {
                var utp = utPersona.OperacionesUnidadTributariaPersona
                    .Find(x => x.Item.UnidadTributariaID == utPersona.UnidadTributariaId &&
                        x.Item.PersonaID == utPersona.PersonaId);

                var unidadTributariaPersona = _unitOfWork.UnidadTributariaPersonaRepository
                    .GetUnidadTributariaPersonaById(utPersona.UnidadTributariaId,
                    utPersona.PersonaId);

                if (utPersona.Operacion == Operation.Add)
                {
                    if (utp != null)
                    {
                        if (utp.Operation == Operation.Remove) return Ok();
                        return new TextResult("El responsable fiscal ya existe para esta unidad tributaria", Request);
                    }
                    if (unidadTributariaPersona != null)
                        return new TextResult("El responsable fiscal ya existe para esta unidad tributaria", Request);
                }
                else
                {
                    if (utp != null)
                    {
                        if (utp.Item.PersonaID != utPersona.SavedPersonaId)
                        {
                            if (utp.Item.UnidadTributariaID == utPersona.UnidadTributariaId &&
                                utp.Item.PersonaID == utPersona.PersonaId)
                                return new TextResult("El responsable fiscal ya existe para esta unidad tributaria", Request);
                        }
                    }
                    if (unidadTributariaPersona == null) return Ok();
                    if (unidadTributariaPersona.PersonaID == utPersona.SavedPersonaId) return Ok();
                    if (unidadTributariaPersona.UnidadTributariaID == utPersona.UnidadTributariaId &&
                        unidadTributariaPersona.PersonaID == utPersona.PersonaId)
                        return new TextResult("El responsable fiscal ya existe para esta unidad tributaria", Request);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
