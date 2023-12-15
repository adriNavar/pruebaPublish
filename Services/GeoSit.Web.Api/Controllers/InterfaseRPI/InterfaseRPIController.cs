using GeoSit.Data.BusinessEntities.LogRPI;
using GeoSit.Data.DAL.Common;
using System;
using System.Net;
using System.Web;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers.InterfaseRPI
{
    public class InterfaseRPIController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public InterfaseRPIController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetCertificadoCatastralByNumero(string numCertificadoCatastral, string usuario)
        {
            var codigoRespuesta = HttpStatusCode.OK;

            try
            {
                return Ok(_unitOfWork.RegistroPropiedadInmuebleRepository.GetCertificadoCatastral(numCertificadoCatastral));
            }
            catch (NullReferenceException)
            {
                codigoRespuesta = HttpStatusCode.NoContent;
                return StatusCode(codigoRespuesta);

            }
            catch (IndexOutOfRangeException)
            {
                codigoRespuesta = HttpStatusCode.Gone;
                return StatusCode(codigoRespuesta);

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"InterfaseRPI/GetCertificadoCatastralByNumero({numCertificadoCatastral},{usuario})", ex);
                return InternalServerError();
            }
            finally
            {
                _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogRespuesta(new RPILogRespuestas(usuario, TipoDeOperacion.ConsultaCertificadoCatastral, numCertificadoCatastral, codigoRespuesta));
            }
        }

        [HttpGet]
        public IHttpActionResult ObtenerCertificadoCatastralByNumero(string numCertificadoCatastral, string usuario)
        {
            var codigoRespuesta = HttpStatusCode.OK;

            try
            {
                return Ok(_unitOfWork.RegistroPropiedadInmuebleRepository.GetCertificadoCatastralByNumero(numCertificadoCatastral));
            }
            catch (NullReferenceException)
            {
                codigoRespuesta = HttpStatusCode.NoContent;
                return StatusCode(codigoRespuesta);

            }
            catch (IndexOutOfRangeException)
            {
                codigoRespuesta = HttpStatusCode.Gone;
                return StatusCode(codigoRespuesta);

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"InterfaseRPI/ObtenerCertificadoCatastralByNumero({numCertificadoCatastral},{usuario})", ex);
                return InternalServerError();
            }
            finally
            {
                _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogRespuesta(new RPILogRespuestas(usuario, TipoDeOperacion.ObtenerCertificadoCatastral, numCertificadoCatastral, codigoRespuesta));
            }
        }

        [HttpGet]
        public IHttpActionResult ObtenerPlanosMensuraByNumero(string numMensura, string letraMensura, string usuario)
        {
            var codigoRespuesta = HttpStatusCode.OK;

            try
            {
                return Ok(_unitOfWork.RegistroPropiedadInmuebleRepository.GetPlanosMensuraByNumero(numMensura, letraMensura));
            }
            catch (NullReferenceException)
            {
                codigoRespuesta = HttpStatusCode.NoContent;
                return StatusCode(codigoRespuesta);

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"InterfaseRPI/ObtenerPlanosMensuraByNumero({numMensura},{letraMensura},{usuario})", ex);
                return InternalServerError();
            }
            finally
            {
                var valor = numMensura + "-" + letraMensura;

                _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogRespuesta(new RPILogRespuestas(usuario, TipoDeOperacion.ObtenerListaDePlanosPorNumero, valor, codigoRespuesta));
            }
        }

        [HttpGet]
        public IHttpActionResult ObtenerPlanosMensuraByNomenclatura(string nomenclatura, string usuario)
        {
            var codigoRespuesta = HttpStatusCode.OK;

            try
            {
                return Ok(_unitOfWork.RegistroPropiedadInmuebleRepository.GetPlanosMensuraByNomenclatura(nomenclatura));
            }
            catch (NullReferenceException)
            {
                codigoRespuesta = HttpStatusCode.NoContent;
                return StatusCode(codigoRespuesta);

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"InterfaseRPI/ObtenerPlanosMensuraByNomenclatura({nomenclatura},{usuario})", ex);
                return InternalServerError();
            }
            finally
            {

                _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogRespuesta(new RPILogRespuestas(usuario, TipoDeOperacion.ObtenerListaDePlanosPorNomenclatura, nomenclatura, codigoRespuesta));
            }
        }

        [HttpGet]
        public IHttpActionResult ObtenerPlanosMensuraByNumeroPartida(string numeroPartida, string usuario)
        {
            var codigoRespuesta = HttpStatusCode.OK;

            try
            {
                return Ok(_unitOfWork.RegistroPropiedadInmuebleRepository.GetPlanosMensuraByNumeroPartida(numeroPartida));
            }
            catch (NullReferenceException)
            {
                codigoRespuesta = HttpStatusCode.NoContent;
                return StatusCode(codigoRespuesta);

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"InterfaseRPI/ObtenerPlanosMensuraByNumeroPartida({numeroPartida},{usuario})", ex);
                return InternalServerError();
            }
            finally
            {

                _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogRespuesta(new RPILogRespuestas(usuario, TipoDeOperacion.ObtenerListaDePlanosPorPartida, numeroPartida, codigoRespuesta));
            }
        }

        [HttpGet]
        public IHttpActionResult ObtenerPlanoMensuraByIdMensura(long idMensura, string usuario)
        {
            var codigoRespuesta = HttpStatusCode.OK;

            try
            {
                return Ok(_unitOfWork.RegistroPropiedadInmuebleRepository.GetPlanoMensuraByIdMensura(idMensura));
            }
            catch (NullReferenceException)
            {
                codigoRespuesta = HttpStatusCode.NoContent;
                return StatusCode(codigoRespuesta);

            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"InterfaseRPI/ObtenerPlanoMensuraByIdMensura({idMensura},{usuario})", ex);
                return InternalServerError();
            }
            finally
            {
                var mensura = idMensura.ToString();

                _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogRespuesta(new RPILogRespuestas(usuario, TipoDeOperacion.ObtenerPlanoDeMensuraPorID, mensura, codigoRespuesta));
            }
        }

        [HttpGet]
        public IHttpActionResult GetVerificarPermisoDeAcceso(long idUsuario)
        {
            var codigoRespuesta = HttpStatusCode.OK;

            try
            {
                return Ok(_unitOfWork.RegistroPropiedadInmuebleRepository.VerificarPermisoDeAcceso(idUsuario));
            }
            catch (NullReferenceException)
            {
                codigoRespuesta = HttpStatusCode.Unauthorized;
                return StatusCode(codigoRespuesta);
            }
            catch (IndexOutOfRangeException)
            {
                codigoRespuesta = HttpStatusCode.Forbidden;
                return StatusCode(codigoRespuesta);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"InterfaseRPI/GetVerificarPermisoDeAcceso({idUsuario})", ex);
                return InternalServerError();
            }
            finally
            {
                _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogRespuesta(new RPILogRespuestas(idUsuario.ToString(), TipoDeOperacion.VerificarPermisosDeAccesoR, idUsuario.ToString(), codigoRespuesta));
            }
        }

        [HttpPost]
        public IHttpActionResult RegistrarConsulta()
        {

            var form = HttpContext.Current.Request.Form;
            _unitOfWork.RegistroPropiedadInmuebleRepository.RegistrarLogConsulta(new RPILogConsultas(form["idUsuario"], Convert.ToInt32(form["operacion"]), form["valor"], Convert.ToInt32(form["codigo"])));
            return Ok();
        }
    }
}
