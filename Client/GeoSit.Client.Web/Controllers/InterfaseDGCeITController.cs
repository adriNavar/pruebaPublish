using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;

namespace GeoSit.Client.Web.Controllers
{
    public class InterfaseDGCeITController : Controller
    {
        const string REPORT_TOKEN_COOKIE_NAME = "ReportToken";

        private readonly HttpClient _cliente = new HttpClient();

        public InterfaseDGCeITController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReporteGeneralInmueble(long id, string token)
        {
            ActionResult actionResult;
            try
            {
                using (var sc = new SeguridadController())
                {
                    var parametrosGenerales = sc.GetParametrosGenerales();

                    var param = parametrosGenerales.FirstOrDefault(x => x.Clave == "IDGC_MAIN_WEB_URL");
                    string baseUrl = param != null ? param.Valor : null;

                    param = parametrosGenerales.FirstOrDefault(x => x.Clave == "IDGC_REPORTS_SERVICE_GUID");
                    string servideGuid = param != null ? param.Valor : null;

                    if (!string.IsNullOrEmpty(baseUrl) && !string.IsNullOrEmpty(servideGuid))
                    {
                        using (var httpClient = new HttpClient())
                        {
                            string eToken = sc.GuidEncode(servideGuid, new
                            {
                                ReportId = 1,
                                FeatureId = id,
                                ServiceGuid = servideGuid,
                                TimeStamp = DateTime.Now.Ticks
                            });
                            var reqParams = new[] { new KeyValuePair<string, string>("EToken", eToken) };

                            string url = BuildUrl(baseUrl, "Forms/GetReport.ashx");
                            var result = httpClient.PostAsync(url, new FormUrlEncodedContent(reqParams)).Result;
                            result.EnsureSuccessStatusCode();

                            if (result.Content.Headers.ContentType.MediaType == MediaTypeNames.Application.Pdf)
                            {
                                var resultStream = result.Content.ReadAsStreamAsync().Result;
                                actionResult = File(resultStream, MediaTypeNames.Application.Pdf, string.Format("Reporte_General_Inmueble_{0}.pdf", id));
                            }
                            else
                            {
                                actionResult = Content(result.Content.ReadAsStringAsync().Result);
                            }
                        }
                    }
                    else
                    {
                        actionResult = Content(GetErrorMessage("Interfase no configurada."));
                    }
                }
            }
            catch (Exception ex)
            {
                actionResult = Content(GetErrorMessage(GetExceptionMessage(ex)));
            }
            finally
            {
                if (!string.IsNullOrEmpty(token))
                {
                    Response.AppendCookie(new HttpCookie(REPORT_TOKEN_COOKIE_NAME, token));
                }
            }
            return actionResult;
        }

        public ActionResult CopiarParcela(long id, char type)
        {
            ActionResult actionResult;
            try
            {
                var usuario = Session["usuarioPortal"] as UsuariosModel;
                if (usuario != null)
                {
                    string url = string.Format("api/InterfaseDGCeIT/CopiarParcela?featId={0}&ctype={1}&userId={2}", id, type, usuario.Id_Usuario);
                    var result = _cliente.PostAsync(url, new StringContent(string.Empty)).Result;

                    string content = result.Content.ReadAsStringAsync().Result;
                    actionResult = Content(result.StatusCode == HttpStatusCode.OK ? GetSuccessMessage(content) : GetErrorMessage(content));
                }
                else
                {
                    actionResult = Content(GetErrorMessage("No tiene permisos para realizar esta operación."));
                }
            }
            catch (Exception ex)
            {
                actionResult = Content(GetErrorMessage(GetExceptionMessage(ex)));
            }
            return actionResult;
        }

        private string GetExceptionMessage(Exception ex)
        {
            var ix = ex.InnerException;
            if (ix != null)
            {
                var messages = new List<string>();
                while (ix != null)
                {
                    messages.Add(ix.Message);
                    ix = ix.InnerException;
                }
                return string.Join("|", messages.ToArray());
            }
            return ex.Message;
        }

        private string GetErrorMessage(string message)
        {
            return string.Format("ERROR:{0}", message);
        }

        private string GetSuccessMessage(string message)
        {
            return string.Format("OK:{0}", message);
        }

        private string BuildUrl(string baseUrl, string path, string query = null)
        {
            const char separator = '/';
            var sb = new StringBuilder(baseUrl);
            if (sb[sb.Length - 1] != separator)
            {
                sb.Append(separator);
            }
            if (!string.IsNullOrEmpty(path))
            {
                sb.Append(path[0] != separator ? path : path.Substring(1));
            }
            if (!string.IsNullOrEmpty(query))
            {
                sb.AppendFormat("?{0}", query);
            }
            return sb.ToString();
        }
    }
}