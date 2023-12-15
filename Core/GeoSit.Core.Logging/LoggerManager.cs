using GeoSit.Core.Logging.CustomEventArgs;
using System;
using System.Collections.Generic;

namespace GeoSit.Core.Logging
{
    public sealed class LoggerManager : IManager
    {
        public event EventHandler<LoggerEventArgs> NotifyError;
        public event EventHandler<LoggerEventArgs> NotifyInfo;

        private ICollection<ILogger> _loggers = null;
        public LoggerManager()
        {
            _loggers = new List<ILogger>();
        }

        public void Add(ILogger logger)
        {
            logger.Register(this);
            _loggers.Add(logger);
        }

        public void LogInfo(string message)
        {
            NotifyInfo?.Invoke(this, new LoggerEventArgs(message, null));
        }

        public void LogError(string message, Exception ex)
        {
            NotifyError?.Invoke(this, new LoggerEventArgs(message, ex));
        }
    }
}
