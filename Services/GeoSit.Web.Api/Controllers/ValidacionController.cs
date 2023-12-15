using GeoSit.Data.BusinessEntities.ValidacionesDB;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.DAL.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class ValidacionController : ApiController
    {
        private readonly UnitOfWork unitOfWork;
        public ValidacionController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("api/validacion/tramite")]
        public IHttpActionResult ValidarTramite(TramiteValidable tramiteValidable)
        {
            return validarObjeto(new ObjetoValidable() { Funcion = tramiteValidable.Funcion, IdTramite = tramiteValidable.IdTramite, TipoObjeto = TipoObjetoValidable.Tramite });
        }
        [HttpPost]
        public IHttpActionResult Validar(ObjetoValidable objetoValidable) => validarObjeto(objetoValidable);

        private IHttpActionResult validarObjeto(ObjetoValidable objetoValidable)
        {
            try
            {
                var errores = new List<string>();
                var statusCode = HttpStatusCode.ExpectationFailed;
                switch (unitOfWork.ValidacionDBRepository.Validar(objetoValidable, out errores))
                {
                    case ResultadoValidacion.Ok:
                        return Ok();
                    case ResultadoValidacion.Advertencia:
                        statusCode = HttpStatusCode.Conflict;
                        break;
                    case ResultadoValidacion.Bloqueo:
                        statusCode = HttpStatusCode.PreconditionFailed;
                        break;
                    default: //error
                        break;
                }
                return Content(statusCode, errores);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"Validacion/Validar({Environment.NewLine}, {JsonConvert.SerializeObject(objetoValidable)})", ex);
                return InternalServerError();
            }
        }
    }
}