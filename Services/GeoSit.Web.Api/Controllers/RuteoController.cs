using GeoSit.Data.DAL.Common;
using GeoSit.Web.Api.Models;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class RuteoController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly HttpClient _cliente;

        public RuteoController(UnitOfWork unitOfWork)
        {
            var asd = new System.Net.NetworkCredential();
            _unitOfWork = unitOfWork;

            _cliente = new HttpClient(new HttpClientHandler { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials })
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["googleApiUrl"]),
                Timeout = TimeSpan.FromMinutes(10)
            };
        }



        //[HttpGet]
        //public string ObtenerDireccion(long latitud, long longitud)
        //{
        //    // 63534   2345          
           
        //    string googleApiKey = _unitOfWork.ParametroRepository.GetParametroByDescripcion("GOOGLE_KEY");
        //    StringBuilder stringParams = new StringBuilder();
        //    NumberFormatInfo formatNumber = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        //    stringParams.Append("?language=es");
        //    stringParams.Append("&latlng=").Append(latitud.ToString(formatNumber)).Append(",").Append(longitud.ToString(formatNumber));
        //    stringParams.Append("&key=").Append(googleApiKey);

        //    var result = _cliente.GetAsync("maps/api/geocode/json" + stringParams.ToString()).Result;
        //    DirectionsResponse response = result.Content.ReadAsAsync<DirectionsResponse>().Result;

        //    string badRequestError = string.Empty;
        //    switch (response.status)
        //    {
        //        case "OK":
        //            break;
        //        case "NOT_FOUND":
        //        case "ZERO_RESULTS":
        //            badRequestError = "No se pudo trazar la ruta para uno o mas de los destinos indicados.";
        //            break;
        //        case "MAX_WAYPOINTS_EXCEEDED":
        //            badRequestError = "Se excedió el número máximo de destinos permitidos.";
        //            break;
        //        case "OVER_QUERY_LIMIT":
        //            badRequestError = "Se excedió el número máximo de consultas.";
        //            break;
        //        case "REQUEST_DENIED":
        //            badRequestError = "No tiene permisos para dicha solicitud.";
        //            break;
        //        default:
        //            badRequestError = "No se pudo trazar la ruta. Por favor intentelo nuevamente.";
        //            break;
        //    }

        //    if (!string.IsNullOrEmpty(badRequestError))
        //    {
        //        Global.GetLogger().LogInfo(string.Format("RuteoController->Consulta Google Api\nError: {0}\nParametros:{1}", badRequestError, stringParams));
        //        var asdsa = BadRequest(badRequestError);
        //    }

        //    var route = response.routes[0].ToString();
        //    return route;
        //}


        [HttpGet]
        [ResponseType(typeof(string))]
        public IHttpActionResult ObtenerDireccion(double latitud, double longitud)
        {
            
            string googleApiKey = _unitOfWork.ParametroRepository.GetParametroByDescripcion("GOOGLE_KEY");
            StringBuilder stringParams = new StringBuilder();
            NumberFormatInfo formatNumber = new NumberFormatInfo() { NumberDecimalSeparator = "." };

            stringParams.Append("?language=es");
            stringParams.Append("&latlng=").Append(latitud.ToString(formatNumber)).Append(",").Append(longitud.ToString(formatNumber));
            stringParams.Append("&key=").Append(googleApiKey);

            var result = _cliente.GetAsync("maps/api/geocode/json" + stringParams.ToString()).Result;
            DirectionsResponse response = result.Content.ReadAsAsync<DirectionsResponse>().Result;

            string badRequestError = string.Empty;
            switch (response.status)
            {
                case "OK":
                    break;
                case "NOT_FOUND":
                case "ZERO_RESULTS":
                    badRequestError = "No se pudo trazar la ruta para uno o mas de los destinos indicados.";
                    break;
                case "MAX_WAYPOINTS_EXCEEDED":
                    badRequestError = "Se excedió el número máximo de destinos permitidos.";
                    break;
                case "OVER_QUERY_LIMIT":
                    badRequestError = "Se excedió el número máximo de consultas.";
                    break;
                case "REQUEST_DENIED":
                    badRequestError = "No tiene permisos para dicha solicitud.";
                    break;
                default:
                    badRequestError = "No se pudo trazar la ruta. Por favor intentelo nuevamente.";
                    break;
            }

            if (!string.IsNullOrEmpty(badRequestError))
            {
                Global.GetLogger().LogInfo(string.Format("RuteoController->Consulta Google Api\nError: {0}\nParametros:{1}", badRequestError, stringParams));
                var asdsa = BadRequest(badRequestError);
            }

            string route = response.routes[0].ToString();            

            return Ok(route);
        }                          




        
        [HttpPost]
        public IHttpActionResult GetIndicaciones(RuteoModel parametros)
        {
            try
            {
                string googleApiKey = _unitOfWork.ParametroRepository.GetParametroByDescripcion("GOOGLE_KEY");
                StringBuilder stringParams = new StringBuilder();
                NumberFormatInfo formatNumber = new NumberFormatInfo() { NumberDecimalSeparator = "." };

                stringParams.Append("?language=es&units=metric");
                stringParams.Append("&origin=").Append(parametros.Inicio.Latitud.ToString(formatNumber)).Append(",").Append(parametros.Inicio.Longitud.ToString(formatNumber));
                stringParams.Append("&destination=").Append(parametros.Fin.Latitud.ToString(formatNumber)).Append(",").Append(parametros.Fin.Longitud.ToString(formatNumber));
                stringParams.Append("&mode=").Append(parametros.Modo);

                if (parametros.Waypoints != null && parametros.Waypoints.Count > 0)
                {
                    stringParams.Append("&waypoints=optimize:true|");
                    foreach (var waypoint in parametros.Waypoints.OrderBy(x => x.Orden))
                    {
                        stringParams.Append(waypoint.Latitud.ToString(formatNumber)).Append(",").Append(waypoint.Longitud.ToString(formatNumber)).Append("|");
                    }

                    stringParams.Remove(stringParams.Length - 1, 1);
                }

                stringParams.Append("&key=").Append(googleApiKey);
                var result = _cliente.GetAsync("maps/api/directions/json" + stringParams.ToString()).Result;
                DirectionsResponse response = result.Content.ReadAsAsync<DirectionsResponse>().Result;

                string badRequestError = string.Empty;
                switch (response.status)
                {
                    case "OK":
                        break;
                    case "NOT_FOUND":
                    case "ZERO_RESULTS":
                        badRequestError = "No se pudo trazar la ruta para uno o mas de los destinos indicados.";
                        break;
                    case "MAX_WAYPOINTS_EXCEEDED":
                        badRequestError = "Se excedió el número máximo de destinos permitidos.";
                        break;
                    case "OVER_QUERY_LIMIT":
                        badRequestError = "Se excedió el número máximo de consultas.";
                        break;
                    case "REQUEST_DENIED":
                        badRequestError = "No tiene permisos para dicha solicitud.";
                        break;
                    default:
                        badRequestError = "No se pudo trazar la ruta. Por favor intentelo nuevamente.";
                        break;
                }

                if (!string.IsNullOrEmpty(badRequestError))
                {
                    Global.GetLogger().LogInfo(string.Format("RuteoController->Consulta Google Api\nError: {0}\nParametros:{1}", badRequestError, stringParams));
                    return BadRequest(badRequestError);
                }

                var route = response.routes[0];

                // direccion de partida - indicaciones de la primer leg
                parametros.Inicio.Orden = 0;
                parametros.Inicio.Direccion = route.legs[0].start_address;
                int stepOrder = 1;
                foreach (var step in route.legs[0].steps)
                {
                    Indicacion indicacion = new Indicacion()
                    {
                        Orden = stepOrder++,
                        Maniobra = step.maneuver,
                        Texto = step.html_instructions,
                        Distancia = step.distance != null ? step.distance.text : string.Empty
                    };

                    parametros.Inicio.Indicaciones.Add(indicacion);
                }

                // waypoints
                for (int i = 1; i < route.legs.Count; i++)
                {
                    int indice = route.waypoint_order[i - 1];
                    parametros.Waypoints[indice].Orden = i;
                    parametros.Waypoints[indice].Direccion = route.legs[i].start_address;
                    stepOrder = 1;
                    foreach (var step in route.legs[i].steps)
                    {
                        Indicacion indicacion = new Indicacion()
                        {
                            Orden = stepOrder++,
                            Maniobra = step.maneuver,
                            Texto = step.html_instructions,
                            Distancia = step.distance != null ? step.distance.text : string.Empty
                        };

                        parametros.Waypoints[indice].Indicaciones.Add(indicacion);
                    }
                }

                // direccion de fin - sin indicaciones
                parametros.Fin.Orden = route.legs.Count;
                parametros.Fin.Direccion = route.legs[route.legs.Count - 1].end_address;
                parametros.OverviewPolyline = route.overview_polyline.points;

                return Ok(parametros);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("RuteoController->GetIndicaciones", ex);
                return InternalServerError(ex);
            }
        }
    }
}
