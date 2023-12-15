using System;

namespace GeoSit.Core.Logging
{
    public interface ILogger
    {
        void Register(IManager manager);
        void Unregister(IManager manager);
        void LogError(string message, Exception ex);
        void LogInfo(string message);
    }
}
