using System.Configuration;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GeoSit.WebRentas.Api.Controllers
{
    public class InterfaseRentasController : ApiController
    {
        [Route("GetToken")]
        [HttpGet]
        public IHttpActionResult GetToken()
        {
            //return Json(new { token = QueryToken() });
            return StatusCode(HttpStatusCode.Unauthorized);
        }

        [Route("LibreDeuda")]
        [HttpGet]
        public IHttpActionResult LibreDeuda(string partida)
        {
            HttpStatusCode retval;
            if ((retval = QueryToken(out string token)) == HttpStatusCode.OK)
            {
                var client = new RestClient($"{ConfigurationManager.AppSettings["urlCatastro"]}{partida}/getLibreDeuda")
                {
                    Timeout = -1
                };
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", token);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return Json(JObject.Parse(response.Content));
                }
                retval = response.StatusCode;
            }
            return StatusCode(retval);
        }

        [Route("InmuebleTitulares")]
        [HttpGet]
        public IHttpActionResult InmuebleTitulares(string partida)
        {
            HttpStatusCode retval;
            if ((retval = QueryToken(out string token)) == HttpStatusCode.OK)
            {
                var client = new RestClient($"{ConfigurationManager.AppSettings["urlCatastro"]}{partida}/getInmuebleTitulares")
                {
                    Timeout = -1
                };
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", token);
                IRestResponse response = client.Execute(request);
                if(response.IsSuccessful)
                {
                    return Json(JArray.Parse(response.Content));
                }
                retval = response.StatusCode;
            }
            return StatusCode(retval);
        }

        [Route("DatosPersona")]
        [HttpGet]
        public IHttpActionResult DatosPersona(string dnicuit)
        {
            HttpStatusCode retval;
            if ((retval = QueryToken(out string token)) == HttpStatusCode.OK)
            {
                var client = new RestClient($"{ConfigurationManager.AppSettings["urlCatastro"]}{dnicuit}/getDatosPersona")
                {
                    Timeout = -1
                };
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", token);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return Json(JArray.Parse(response.Content));
                }
                retval = response.StatusCode;
            }
            return StatusCode(retval);
        }

        [Route("TasaCatastro")]
        [HttpGet]
        public IHttpActionResult TasaCatastro(string tramiteId)
        {
            HttpStatusCode retval;
            if ((retval = QueryToken(out string token)) == HttpStatusCode.OK)
            {
                var client = new RestClient($"{ConfigurationManager.AppSettings["urlTasa"]}{tramiteId}")
                {
                    Timeout = -1
                };
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", token);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return Json(JArray.Parse(response.Content));
                }
                retval = response.StatusCode;
            }
            return StatusCode(retval);
        }

        private HttpStatusCode QueryToken(out string token)
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            foreach (string param in new[] { "grant_type", "client_secret", "client_id", "username", "password" })
            {
                request.AddParameter(param, ConfigurationManager.AppSettings[param]);
            }

            var client = new RestClient($"{ConfigurationManager.AppSettings["urlToken"]}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(request);
            token = null;
            if (response.IsSuccessful)
            {
                var obj = JObject.Parse(response.Content);
                token = $"{obj["token_type"]} {obj["token"]}";
                return HttpStatusCode.OK;
            }
            else if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound && JObject.Parse(response.Content).ContainsKey("error_errorCode"))
            {
                return HttpStatusCode.Unauthorized;
            }
            return HttpStatusCode.ServiceUnavailable;
        }
    }
}