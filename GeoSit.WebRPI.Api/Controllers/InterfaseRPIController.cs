using System.Configuration;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GeoSit.WebRPI.Api.Controllers
{
    public class InterfaseRPIController : ApiController
    {
        [Route("ConsultarCertificadoCatastral")]
        [HttpGet]
        public IHttpActionResult ConsultarCertificadoCatastral(string idUsuario, string numCertificado)
        {
            if (!VerificaUsuario(idUsuario))
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
            var client = new RestClient($"{urlBase}/GetCertificadoCatastralByNumero?numCertificadoCatastral={numCertificado}&usuario={idUsuario}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Json(JObject.Parse(response.Content));
                case HttpStatusCode.NoContent:
                    return StatusCode(HttpStatusCode.NoContent);
                case HttpStatusCode.Gone:
                    return Content(HttpStatusCode.Gone, "Certificado vencido");
                default:
                    return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [Route("ObtenerCertificadoCatastral")]
        [HttpGet]
        public IHttpActionResult ObtenerCertificadoCatastral(string idUsuario, string numCertificado)
        {
            if (!VerificaUsuario(idUsuario))
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
            var client = new RestClient($"{urlBase}/GetCertificadoCatastralByNumero?numCertificadoCatastral={numCertificado}&usuario={idUsuario}")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    client = new RestClient($"{urlBase}/ObtenerCertificadoCatastralByNumero?numCertificadoCatastral={numCertificado}&usuario={idUsuario}")
                    {
                        Timeout = -1
                    };
                    return Json(client.Execute(request).Content);
                case HttpStatusCode.NoContent:
                    return StatusCode(HttpStatusCode.NoContent);
                case HttpStatusCode.Gone:
                    return Content(HttpStatusCode.Gone, "Certificado vencido");
                default:
                    return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [Route("ObtenerPlanoMensuraPorNumeroPlano")]
        [HttpGet]
        public IHttpActionResult ObtenerPlanoMensuraPorNumeroPlano(string idUsuario, string numPlano, string letraIdPlano)
        {
            if (!VerificaUsuario(idUsuario))
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
            var client = new RestClient($"{urlBase}/ObtenerPlanosMensuraByNumero?numMensura={numPlano}&letraMensura={letraIdPlano}&usuario={idUsuario}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));

            if (string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return Json(JArray.Parse(response.Content));
        }

        [Route("ObtenerListaPlanoMensuraPorNumeroPartida")]
        [HttpGet]
        public IHttpActionResult ObtenerListaPlanoMensuraPorNumeroPartida(string idUsuario, string numPartidaInmobiliaria)
        {
            if (!VerificaUsuario(idUsuario))
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string pattern = @"[A-Za-z][0-9]{7}[1-3]";
            Match m = Regex.Match(numPartidaInmobiliaria, pattern);
            if (m.Success)
            {
                string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
                var client = new RestClient($"{urlBase}/ObtenerPlanosMensuraByNumeroPartida?numeroPartida={numPartidaInmobiliaria}&usuario={idUsuario}")
                {
                    Timeout = -1
                };
                IRestResponse response = client.Execute(new RestRequest(Method.GET));

                if (string.IsNullOrEmpty(response.Content))
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }

                return Json(JArray.Parse(response.Content));
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "Número de partida inválido");
            }
        }

        [Route("ObtenerListaPlanoMensuraPorNomenclaturaCatastral")]
        [HttpGet]
        public IHttpActionResult ObtenerListaPlanoMensuraPorNomenclaturaCatastral(string idUsuario, string numnomenclatura)
        {
            if (!VerificaUsuario(idUsuario))
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
            var client = new RestClient($"{urlBase}/ObtenerPlanosMensuraByNomenclatura?nomenclatura={numnomenclatura}&usuario={idUsuario}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));

            if (string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return Json(JArray.Parse(response.Content));
        }

        [Route("ObtenerPlanoMensuraPorId")]
        [HttpGet]
        public IHttpActionResult ObtenerPlanoMensuraPorId(string idUsuario, string idMensura)
        {
            if (!VerificaUsuario(idUsuario))
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
            var client = new RestClient($"{urlBase}/ObtenerPlanoMensuraByIdMensura?idMensura={idMensura}&usuario={idUsuario}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));

            if (string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return Json(response.Content);
        }

        private bool VerificaUsuario(string idUsuario)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfaseDCG"].ToString();
            var client = new RestClient($"{urlBase}/VerificarPermisosAcceso?idUsuario={idUsuario}")
            {
                Timeout = -1
            };
            var resp = client.Execute(new RestRequest(Method.GET));
            GuardaLog(idUsuario, "206", resp.StatusCode, idUsuario);
            return resp.IsSuccessful;
        }

        private void GuardaLog(string idUsuario, string operacion, HttpStatusCode codigo, string valor)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
            var client = new RestClient($"{urlBase}/RegistrarConsulta")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("idUsuario", idUsuario);
            request.AddParameter("operacion", operacion);
            request.AddParameter("codigo", (int)codigo);
            request.AddParameter("valor", valor);
            client.Execute(request);
        }
    }
}