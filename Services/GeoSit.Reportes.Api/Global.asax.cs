using GeoSit.Core.Logging;
using GeoSit.Core.Logging.Loggers;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;

namespace GeoSit.Reportes.Api
{
    public class WebApiApplication : HttpApplication
    {
        private static LoggerManager _loggerManager = null;
        protected void Application_Start(object sender, EventArgs e)
        {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
            GlobalConfiguration.Configure(WebApiConfig.Register);

            if (_loggerManager == null)
            {
                _loggerManager = new LoggerManager();
                _loggerManager.Add(new Log4NET(Server.MapPath(ConfigurationManager.AppSettings["log4net.config"].ToString()), "DefaultLogger", "ErrorLogger"));
            }
        }
        internal static LoggerManager GetLogger()
        {
            return _loggerManager;
        }
    }
}
