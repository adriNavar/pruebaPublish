using System;
using System.Web.Http;
using GeoSit.Core.Logging;
using GeoSit.Core.Logging.Loggers;
using System.Configuration;
using System.Collections.Generic;
using GeoSit.Web.Api.Services.Interfaces;
using GeoSit.Web.Api.Services;

namespace GeoSit.Web.Api
{
    public class Global : System.Web.HttpApplication
    {
        private static LoggerManager _loggerManager = null;
        private List<IStandaloneService> _services = null;
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            if (_loggerManager == null)
            {
                _loggerManager = new LoggerManager();
                _loggerManager.Add(new Log4NET(Server.MapPath(ConfigurationManager.AppSettings["log4net.config"].ToString()), "DefaultLogger", "ErrorLogger"));
            }
            _services = new List<IStandaloneService>()
            {
                { new SessionsCleanup() },
                { new FullDeltaIndexer() }
            };
            foreach (var service in _services)
            {
                service.Start();
            }
        }
        protected void Application_End(object sender, EventArgs e)
        {
            foreach (var service in _services)
            {
                service.Stop();
            }
        }
        internal static LoggerManager GetLogger()
        {
            return _loggerManager;
        }
    }
}