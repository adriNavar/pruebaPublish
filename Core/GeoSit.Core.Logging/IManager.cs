using GeoSit.Core.Logging.CustomEventArgs;
using System;

namespace GeoSit.Core.Logging
{
    public interface IManager
    {
        event EventHandler<LoggerEventArgs> NotifyError;
        event EventHandler<LoggerEventArgs> NotifyInfo;
    }
}
