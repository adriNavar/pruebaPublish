using System;

namespace GeoSit.Core.Logging.CustomEventArgs
{
    public class LoggerEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public LoggerEventArgs(string message, Exception exception)
        {
            this.Message = message;
            this.Exception = exception;
        }
    }
}
