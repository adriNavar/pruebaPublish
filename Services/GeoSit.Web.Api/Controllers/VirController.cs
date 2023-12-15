using System;
using System.Linq;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Common;
using Newtonsoft.Json;

namespace GeoSit.Web.Api.Controllers
{
    public class VirController : ApiController
    {
        private readonly UnitOfWork unitOfWork;

        public VirController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long idinmueble)
        {
            var virValuacion = unitOfWork.VIRValuacionRepository.GetVIRValuacionesByIdInmueble(idinmueble);
            return Ok(virValuacion);
        }

        [HttpGet]
        public IHttpActionResult GetValuacionVIRInmueble(long id)
        {
            try
            {
                return Ok(unitOfWork.VIRValuacionRepository.GetValuacionVigente(id));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"GetValuacionVIRVigente{id}", ex);
                return InternalServerError();
            }
        }
        public IHttpActionResult GetVirDetalle(long idInmueble)
        {
            var virInmueble = unitOfWork.VIRInmuebleRepository.GetVIRInmuebleByIdInmueble(idInmueble);
            return Ok(virInmueble);
        }

        public IHttpActionResult GetVirEstados()
        {
            var virEstados = unitOfWork.VIRVbEuCoefEstadoRepository.GetVIRVbEuCoefEstados();
            return Ok(virEstados);
        }

        public IHttpActionResult GetVirUsos()
        {
            var virUsos = unitOfWork.VIRVbEuUsoRepository.GetVIRVbEuUsos();
            return Ok(virUsos);
        }

        public IHttpActionResult GetVirUsoByUso(string uso)
        {
            var virUso = unitOfWork.VIRVbEuUsoRepository.GetVIRVbEuUsoByUso(uso);
            return Ok(virUso);
        }

        public IHttpActionResult GetTipoEdif(int tipo)
        {
            var virTipoEdif = unitOfWork.VIRVbEuTipoEdifRepository.GetTipoEdif(tipo);
            return Ok(virTipoEdif);
        }

        public IHttpActionResult GetVIREquivInmDestinosMejoras()
        {
            var virEquivInm = unitOfWork.VIREquivInmDestinosMejorasRepository.GetVIREquivInmDestinosMejoras();
            return Ok(virEquivInm);
        }
    }
}
