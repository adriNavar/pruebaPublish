using GeoSit.Core.Logging.CustomEventArgs;
using log4net;
using log4net.Config;
using System;
using System.IO;

namespace GeoSit.Core.Logging.Loggers
{
    public class Log4NET : ILogger
    {
        private ILog _defaultLogger = null;
        private ILog _errorLogger = null;
        public Log4NET(string configFile, string infoLoggerName, string errorLoggerName)
        {
            XmlConfigurator.Configure(new FileInfo(configFile));
            this._defaultLogger = LogManager.GetLogger(infoLoggerName);
            this._errorLogger = LogManager.GetLogger(errorLoggerName);
        }
        

        private void Log4NET_Info(object sender, LoggerEventArgs e)
        {
            this.LogInfo(e.Message);
        }

        private void Log4NET_Error(object sender, LoggerEventArgs e)
        {
            this.LogError(e.Message, e.Exception);
        }

        public void Register(IManager manager)
        {
            manager.NotifyError += this.Log4NET_Error;
            manager.NotifyInfo += this.Log4NET_Info;
        }

        public void Unregister(IManager manager)
        {
            manager.NotifyError -= this.Log4NET_Error;
            manager.NotifyInfo -= this.Log4NET_Info;
        }

        public void LogError(string message, Exception ex)
        {
            this._errorLogger.Error(message, ex);
        }

        public void LogInfo(string message)
        {
            this._defaultLogger.Info(message);
        }
    }
}
