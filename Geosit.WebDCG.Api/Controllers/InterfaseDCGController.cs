using System.Configuration;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using RestSharp;


namespace Geosit.WebDCG.Api.Controllers
{
    public class InterfaseDCGController : ApiController
    {
        [Route("dominio_por_partida")]
        [HttpGet]
        public IHttpActionResult dominio_por_partida(int idUsuario, string partida)
        {
            if (!validar_usuario(idUsuario))
            {
                return Unauthorized();
            }

            string urlBase = ConfigurationManager.AppSettings["urlInterfaseDCG"].ToString();
            var client = new RestClient($"{urlBase}/DomPorPartida?idUsuario={idUsuario}&partida={partida}") 
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            GuardaLog(idUsuario, "201", response.StatusCode, partida);
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return Content(HttpStatusCode.NoContent, "No hay información disponible");
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("dominio_por_nomenclatura")]
        [HttpGet]
        public IHttpActionResult dominio_por_nomenclatura(int idUsuario, string nomenclatura)
        {
            if (!validar_usuario(idUsuario))
            {
                return Unauthorized();
            }
            string urlBase = ConfigurationManager.AppSettings["urlInterfaseDCG"].ToString();
            var client = new RestClient($"{urlBase}/DomPorNomenclatura?idUsuario={idUsuario}&nomenclatura={nomenclatura}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            GuardaLog(idUsuario, "202", response.StatusCode, nomenclatura);
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        [Route("dominio_por_inscripcion")]
        [HttpGet]
        public IHttpActionResult dominio_por_inscripcion(int idUsuario, int? matricula, int? libro, int? tomo, int? folio, int? finca, int? anio, int? uf)
        {
            if (!validar_usuario(idUsuario))
            {
                return Unauthorized();
            }
            string urlBase = ConfigurationManager.AppSettings["urlInterfaseDCG"].ToString();
            string parametros = $"matricula={(object)matricula ?? ""}&libro={(object)libro ?? ""}&tomo={(object)tomo ?? ""}&folio={(object)folio ?? ""}&finca={(object)finca ?? ""}&anio={(object)anio ?? ""}&uf={(object)uf ?? ""}";
            var client = new RestClient($"{urlBase}/DomPorInscripcion?idUsuario={idUsuario}&{parametros}")
            {
                Timeout = -1
            };
            IRestResponse response = client.Execute(new RestRequest(Method.GET));
            GuardaLog(idUsuario, "205", response.StatusCode, parametros);
            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            return Json(JObject.Parse(response.Content));
        }

        private bool validar_usuario(long idUsuario)
        {
            string urlBase = ConfigurationManager.AppSettings["urlInterfaseRPI"].ToString();
            var client = new RestClient($"{urlBase}/GetVerificarPermisoDeAcceso?idUsuario={idUsuario}")
            {
                Timeout = -1
            };
            return client.Execute(new RestRequest(Method.GET)).IsSuccessful;
        }

        private void GuardaLog(long idUsuario, string operacion, HttpStatusCode codigo, string valor)
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
