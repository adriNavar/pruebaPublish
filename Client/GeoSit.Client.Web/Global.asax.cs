using System.Collections.Concurrent;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;
using Newtonsoft.Json;
using System;
using GeoSit.Client.Web.Models;
using System.Net.Http;
using System.Configuration;
using GeoSit.Core.Logging.Loggers;
using GeoSit.Core.Logging;
using System.Net;
using System.Web.SessionState;
using System.Web.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoSit.Client.Web
{
    public class MvcApplication : HttpApplication
    {
        public const string MultipleParameterKey = "_multiple_";

        private static LoggerManager _loggerManager = null;
        private static MvcApplication _appInstance = null;

        private static readonly ConcurrentDictionary<int, ICaptchaManager> CaptchaManagers =
            new ConcurrentDictionary<int, ICaptchaManager>();

        protected void Application_Start()
        {
            //Inicializacion de logs
            if (_loggerManager == null)
            {
                _loggerManager = new LoggerManager();
                _loggerManager.Add(new Log4NET(Server.MapPath(ConfigurationManager.AppSettings["log4net.config"].ToString()), "DefaultLogger", "ErrorLogger"));
            }
            if (_appInstance == null)
            {
                _appInstance = this;
            }

            //Inicialización de Captcha
            CaptchaUtils.CaptchaManager.StorageProvider = new CookieStorageProvider();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

            Initialize();
            CaptchaUtils.CaptchaManagerFactory = GetCaptchaManager;
            using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
            {
                cliente.PostAsync("api/GenericoService/CleanUsuariosActivosAll/", null);
            }
        }

        public void Initialize()
        {

        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                if (Session.IsNewSession)
                {
                    var excludedUrls = WebConfigurationManager
                                        .OpenWebConfiguration("~")
                                        .Locations.Cast<ConfigurationLocation>()
                                        .Select(loc => loc.Path.ToLower())
                                        .Concat(new[]
                                                {
                                                    (WebConfigurationManager
                                                            .GetSection("system.web/authentication") as AuthenticationSection)
                                                            .Forms
                                                            .LoginUrl.ToLower()
                                                });
                    if (!excludedUrls.Any(url => Context.Request.AppRelativeCurrentExecutionFilePath.ToLower().Contains(url)))
                    {
                        Context.Items["ExpiredSession"] = true;
                        Context.Response.Write("EXPIRED");
                        Context.Response.End();
                        Session.Clear();
                    }
                }
            }
        }
        protected void Application_EndRequest()
        {
            if (Context.Items["ExpiredSession"] is bool)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }

        private static ICaptchaManager GetCaptchaManager(IParameterContainer parameterContainer)
        {
            if (parameterContainer.TryGet(MultipleParameterKey, out int numberOfCaptcha))
            {
                return CaptchaManagers.GetOrAdd(numberOfCaptcha, CreateCaptchaManagerByNumber);
            }
            return CaptchaUtils.CaptchaManager;
        }

        private static ICaptchaManager CreateCaptchaManagerByNumber(int i)
        {
            var captchaManager = new DefaultCaptchaManager(new SessionStorageProvider());
            captchaManager.ImageElementName += i;
            captchaManager.InputElementName += i;
            captchaManager.TokenElementName += i;
            captchaManager.ImageUrlFactory = (helper, pair) =>
            {
                var dictionary = new RouteValueDictionary();
                dictionary.Add(captchaManager.TokenParameterName, pair.Key);
                dictionary.Add(MultipleParameterKey, i);
                return helper.Action("Generate", "DefaultCaptcha", dictionary);
            };
            captchaManager.RefreshUrlFactory = (helper, pair) =>
            {
                var dictionary = new RouteValueDictionary();
                dictionary.Add(MultipleParameterKey, i);
                return helper.Action("Refresh", "DefaultCaptcha", dictionary);
            };
            return captchaManager;
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            Session["ID_CAPA_TEMP_SEQ"] = (short)10000;
            if (ConfigurationManager.AppSettings["mostrarMensajeInicio"].ToString() != "S")
            {
                Session["showafterlogin"] = false;
            }
        }
        protected void Session_End(object sender, EventArgs e)
        {
            var usuarioMem = Session["usuarioPortal"] as UsuariosModel;
            if (usuarioMem != null && !string.IsNullOrEmpty(usuarioMem.Token))
            {
                var limpiarMTs = BorrarMapasTematicos(usuarioMem);
                var limpiarUsuario = BorrarUsuarioActivo(usuarioMem);
                limpiarMTs.Wait();
                limpiarUsuario.Wait();
                GetLogger().LogInfo($"Sesión expirada con éxito {{{usuarioMem.Login}, {usuarioMem.Token}}}");
            }
        }
        internal static LoggerManager GetLogger() => _loggerManager;
        internal static MvcApplication GetInstance() => _appInstance;

        private Task BorrarUsuarioActivo(UsuariosModel usuario)
        {
            return Task.Run(() =>
            {
                using (var cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials }))
                {
                    try
                    {
                        cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                        using (var resp = cliente.PostAsJsonAsync("api/GenericoService/CleanUsuariosActivosByToken", usuario.Token).Result)
                        {
                            resp.EnsureSuccessStatusCode();
                        }
                    }
                    catch (Exception ex)
                    {
                        GetLogger().LogError("CleanUsuariosActivosByToken", ex);
                    }
                }
            });
        }
        internal Task BorrarMapasTematicos(UsuariosModel usuario)
        {
            return Task.Run(() =>
            {
                using (var cliente = new HttpClient(new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials }))
                {
                    try
                    {
                        cliente.Timeout = TimeSpan.FromMinutes(15);
                        cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("idUsuario", usuario.Id_Usuario.ToString()),
                            new KeyValuePair<string, string>("token", usuario.Token)
                        });
                        using (var resp = cliente.PostAsync($"api/MapasTematicosService/DeleteObjetoResultado", content).Result)
                        {
                            resp.EnsureSuccessStatusCode();
                        }
                    }
                    catch (Exception ex)
                    {
                        GetLogger().LogError("DeleteObjetoResultado", ex);
                    }
                }
            });
        }
    }
}