using System.Configuration;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GeoSit.WebPUIP.Api.Controllers
{
    public class InterfasePCIController : ApiController
    {
        [Route("RegistrarMutacionEstadoParcelario")]
        [HttpGet]
        public IHttpActionResult RegistrarMutacionEstadoParcelario(string tipoOperacion, string lstParcelasOrigen, string lstParcelasDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"tipoOperacion={tipoOperacion}&lstParcelasOrigen={lstParcelasOrigen}&lstParcelasDestino={lstParcelasDestino}&numPlano={numPlano}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarMutacionEstadoParcelario?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("RegistrarOperacionSinAntecedentes")]
        [HttpGet]
        public IHttpActionResult RegistrarOperacionSinAntecedentes(string tipoOperacion, string lstParcelasDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"tipoOperacion={tipoOperacion}&lstParcelasDestino={lstParcelasDestino}&numPlano={numPlano}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarOperacionSinAntecedentes?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("RegistrarAfectacion")]
        [HttpGet]
        public IHttpActionResult RegistrarAfectacion(string tipoOperacion, string ParcelaDestino, string lstUnidadesDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"tipoOperacion={tipoOperacion}&ParcelaDestino={ParcelaDestino}&lstUnidadesDestino={lstUnidadesDestino}&numPlano={numPlano}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarAfectacion?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("RegistrarDesafectacion")]
        [HttpGet]
        public IHttpActionResult RegistrarDesafectacion(string tipoOperacion, string ParcelaDestino, string lstUnidadesDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"tipoOperacion={tipoOperacion}&ParcelaDestino={ParcelaDestino}&lstUnidadesDestino={lstUnidadesDestino}&numPlano={numPlano}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarDesfectacion?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("RegistrarModificacionAfectacion")]
        [HttpGet]
        public IHttpActionResult RegistrarModificacionAfectacion(string tipoOperacion, string ParcelaDestino, string lstParcelasOrigen, string lstUnidadesDestino, string numPlano, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"tipoOperacion={tipoOperacion}&ParcelaDestino={ParcelaDestino}&lstParcelasOrigen={lstParcelasOrigen}&lstUnidadesDestino={lstUnidadesDestino}&numPlano={numPlano}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarModificacionAfectacion?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("RegistrarModificacionPartidaInmoviliaria")]
        [HttpGet]
        public IHttpActionResult RegistrarModificacionPartidaInmoviliaria(string partidaOriginal, string partidaNueva, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"partidaOriginal={partidaOriginal}&partidaNueva={partidaNueva}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarModificacionPartidaInmoviliaria?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("RegistrarModificacionNomenclaturaCatastral")]
        [HttpGet]
        public IHttpActionResult RegistrarModificacionNomenclaturaCatastral(string nomenclaturaRegistrada, string nomenclaturaNueva, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"nomenclaturaRegistrada={nomenclaturaRegistrada}&nomenclaturaNueva={nomenclaturaNueva}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarModificacionNomenclaturaCatastral?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("RegistrarBajaLogica")]
        [HttpGet]
        public IHttpActionResult RegistrarBajaLogica(string nomenclaturaRegistrada, string fechaRegistro, string numTramite, string motivo)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            string parametros = $"nomenclaturaRegistrada={nomenclaturaRegistrada}&fechaRegistro={fechaRegistro}&numTramite={numTramite}&motivo={motivo}";

            var client = new RestClient($"{urlBase}/RegistrarBajaLogica?{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("ConsultaPadronUnico")]
        [HttpGet]
        public IHttpActionResult ConsultaPadronUnico(string clave)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            var client = new RestClient($"{urlBase}/ConsultaPadronUnico?clave={clave}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("ConsultaDatosAdicionales")]
        [HttpGet]
        public IHttpActionResult ConsultaDatosAdicionales(string clave)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            var client = new RestClient($"{urlBase}/ConsultaDatosAdicionales?clave={clave}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("ConsultaNovedadesRegistradas")]
        [HttpGet]
        public IHttpActionResult ConsultaNovedadesRegistradas(string clave)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            var client = new RestClient($"{urlBase}/ConsultaNovedadesRegistradas?clave={clave}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("ConsultaAntecedentesSucedentes")]
        [HttpGet]
        public IHttpActionResult ConsultaAntecedentesSucedentes(string nroPlano)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfasePCI"].ToString();
            var client = new RestClient($"{urlBase}/ConsultaAntecedentesSucedentes?nroPlano={nroPlano}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }
    }
}
