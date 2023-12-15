using Newtonsoft.Json.Linq;
using System.Web.Http;

namespace GeoSit.WebPCIDummy.Api.Controllers
{
    public class InterfasePCIDummyController : ApiController
    {
        [Route("RegistrarMutacionEstadoParcelario")]
        [HttpGet]
        public IHttpActionResult RegistrarMutacionEstadoParcelario(string tipoOperacion, string lstParcelasOrigen, string lstParcelasDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            string result = string.Empty;
            if (motivo != "999")
            {
                result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                return Json(JObject.Parse(result));
            }
            else
            {
                result = "ERROR - Su operación no pudo ser registrada.";
                return BadRequest(result);
            }
        }

        [Route("RegistrarOperacionSinAntecedentes")]
        [HttpGet]
        public IHttpActionResult RegistrarOperacionSinAntecedentes(string tipoOperacion, string lstParcelasDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            {
                string result = string.Empty;
                if (motivo != "999")
                {
                    result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                    return Json(JObject.Parse(result));
                }
                else
                {
                    result = "ERROR - Su operación no pudo ser registrada.";
                    return BadRequest(result);
                }
            }
        }

        [Route("RegistrarAfectacion")]
        [HttpGet]
        public IHttpActionResult RegistrarAfectacion(string tipoOperacion, string ParcelaDestino, string lstUnidadesDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            {
                string result = string.Empty;
                if (motivo != "999")
                {
                    result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                    return Json(JObject.Parse(result));
                }
                else
                {
                    result = "ERROR - Su operación no pudo ser registrada.";
                    return BadRequest(result);
                }
            }
        }

        [Route("RegistrarDesafectacion")]
        [HttpGet]
        public IHttpActionResult RegistrarDesafectacion(string tipoOperacion, string ParcelaDestino, string lstUnidadesDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            {
                string result = string.Empty;
                if (motivo != "999")
                {
                    result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                    return Json(JObject.Parse(result));
                }
                else
                {
                    result = "ERROR - Su operación no pudo ser registrada.";
                    return BadRequest(result);
                }
            }
        }

        [Route("RegistrarModificacionAfectacion")]
        [HttpGet]
        public IHttpActionResult RegistrarModificacionAfectacion(string tipoOperacion, string ParcelaDestino, string lstParcelasOrigen, string lstUnidadesDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            {
                string result = string.Empty;
                if (motivo != "999")
                {
                    result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                    return Json(JObject.Parse(result));
                }
                else
                {
                    result = "ERROR - Su operación no pudo ser registrada.";
                    return BadRequest(result);
                }
            }
        }

        [Route("RegistrarModificacionPartidaInmoviliaria")]
        [HttpGet]
        public IHttpActionResult RegistrarModificacionPartidaInmoviliaria(string partidaOriginal, string partidaNueva, string fechaRegistro, string numTramite, string motivo)
        {
            {
                string result = string.Empty;
                if (motivo != "999")
                {
                    result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                    return Json(JObject.Parse(result));
                }
                else
                {
                    result = "ERROR - Su operación no pudo ser registrada.";
                    return BadRequest(result);
                }
            }
        }

        [Route("RegistrarModificacionNomenclaturaCatastral")]
        [HttpGet]
        public IHttpActionResult RegistrarModificacionNomenclaturaCatastral(string nomenclaturaRegistrada, string nomenclaturaNueva, string fechaRegistro, string numTramite, string motivo)
        {
            {
                string result = string.Empty;
                if (motivo != "999")
                {
                    result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                    return Json(JObject.Parse(result));
                }
                else
                {
                    result = "ERROR - Su operación no pudo ser registrada.";
                    return BadRequest(result);
                }
            }
        }

        [Route("RegistrarBajaLogica")]
        [HttpGet]
        public IHttpActionResult RegistrarBajaLogica(string nomenclaturaRegistrada, string fechaRegistro, string numTramite, string motivo)
        {
            {
                string result = string.Empty;
                if (motivo != "999")
                {
                    result = "{\"registracion\": {\"mensaje\": \"Su operación ha sido registrada correctamente.\"}}";
                    return Json(JObject.Parse(result));
                }
                else
                {
                    result = "ERROR - Su operación no pudo ser registrada.";
                    return BadRequest(result);
                }
            }
        }

        [Route("ConsultaPadronUnico")]
        [HttpGet]
        public IHttpActionResult ConsultaPadronUnico(string clave)
        {
            string result = string.Empty;
            if (clave != "999")
            {
                result = "{\"Datos Registrados\": {\"Nomenclatura catastral del inmueble\": \"1234567abcdef\",\"Partida inmobiliaria\": \"A10000013\",\"Plano Mensura\": \"0000012901-0000\",\"Fecha DGC\": \"26/01/2009\", \"Número Trámite DCG\": \"1234567890\",\"Motivo Operación DCG\":\"MOTIVO OPERACION DCG\",\"Inscripción de Dominio\": \"0000010901-0000\",\"Fecha RPI\": \"20/12/2021\",\"Número Trámite RPI\": \"1234567890\",\"Motivo Operación RPI\":\"MOTIVO OPERACION RPI\"}}";
            }
            else
            {
                result = "{\"Datos Registrados\": {\"mensaje\": \"ERROR - No existen datos para esa consulta.\"}}";
            }
            
                
            return Json(JObject.Parse(result));
            
        }

        [Route("ConsultaDatosAdicionales")]
        [HttpGet]
        public IHttpActionResult ConsultaDatosAdicionales(string clave)
        {
            string result = string.Empty;
            if (clave != "999")
            {
                result = "{\"Datos Registrados\": {\"Nomenclatura catastral del inmueble\": \"1234567abcdef\",\"Partida inmobiliaria\": \"A10000013\",\"Inscripción de Dominio\": \"0000012901-0000\",\"Antecedentes\": [{\"Nomenclatura catastral del inmueble\": \"123123abcdef\",\"Partida inmobiliaria\": \"A10000010\",\"Inscripción de Dominio\": \"0000010901-0000\"},{\"Nomenclatura catastral del inmueble\": \"456456abcdef\",\"Partida inmobiliaria\": \"A10000020\",\"Inscripción de Dominio\": \"0000010902-0000\"}],\"Sucedentes\": null,\"Operaciones registradas\": [{\"Tipo Operación DGC\": \"Unificación de parcelas\",\"Fecha registración DGC\": \"4/4/2019\"},{\"Tipo Operación DGC\": \"Inscripción de Matricula\",\"Fecha registración DGC\": \"12/11/2019\"}],\"Datos de Auditoría\": [{\"Mensura y Proyecto de Unificación\": \"9690-O 4/4/2019 REGISTRADA\",\"Inscripción de Matricula RPI\": \"15027-I 12/11/2019 REGISTRADA\"}]}}";
            }
            else
            {
                result = "{\"Datos Registrados\": {\"mensaje\": \"ERROR - No existen datos para esa consulta.\"}}";
            }
                
            return Json(JObject.Parse(result));
            
        }

        [Route("ConsultaNovedadesRegistradas")]
        [HttpGet]
        public IHttpActionResult ConsultaNovedadesRegistradas(string clave)
        {
            string result = string.Empty;
            if (clave != "999")
            {
                result = "{\"Datos Registrados\": {\"Nomenclatura catastral del inmueble\": \"1234567abcdef\",\"Partida inmobiliaria\": \"A10000013\",\"Inscripción de Dominio\": \"0000012901-0000\",\"Operaciones Registradas\": [{\"Operaciones Registradas\":{\"Tipo Operación DGR\": null,\"Fecha Registración DGR\": \"\",\"Tipo Operación RPI\": null,\"Fecha Registración RPI\": \"\",\"Datos Auditoría\": null}},{\"Operaciones Registradas\":{\"Tipo Operación DGR\": null,\"Fecha Registración DGR\": \"\",\"Tipo Operación RPI\": null,\"Fecha Registración RPI\": \"\",\"Datos Auditoría\": null}}]}}";
            }
            else
            {
                result = "{\"Datos Registrados\": {\"mensaje\": \"ERROR - No existen datos para esa consulta.\"}}";
            }
                
            return Json(JObject.Parse(result));
        }

        [Route("ConsultaAntecedentesSucedentes")]
        [HttpGet]
        public IHttpActionResult ConsultaAntecedentesSucedentes(string nroPlano)
        {
            string result = string.Empty;
            if (nroPlano != "999")
            {
                result = "{\"Datos Registrados\": {\"Plano Mensura\": \"" + nroPlano + "\",\"Antecedentes\": [{\"Antecedente\":{\"Nomenclatura catastral del inmueble\": \"12345\",\"Partida inmobiliaria\": \"A12345\",\"Inscripción de Dominio\": \"5678\"}},{\"Antecedente\":{\"Nomenclatura catastral del inmueble\": \"87905\",\"Partida inmobiliaria\": \"A34345\",\"Inscripción de Dominio\": \"9880\"}}],\"Sucedentes\": [{\"Sucedente\":{\"Nomenclatura catastral del inmueble\": \"12345\",\"Partida inmobiliaria\": \"A12345\",\"Inscripción de Dominio\": \"5678\"}},{\"Sucedente\":{\"Nomenclatura catastral del inmueble\": \"34567\",\"Partida inmobiliaria\": \"A78945\",\"Inscripción de Dominio\": \"9880\"}}]}}";
            }
            else
            {
                result = "{\"Datos Registrados\": {\"mensaje\": \"ERROR - No existen datos para esa consulta.\"}}";
            }

            return Json(JObject.Parse(result));
        }
    }
}