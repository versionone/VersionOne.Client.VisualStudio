using System;

namespace VersionOne.VisualStudio.DataLayer.Logging {
    public interface ILoggerFactory {
        LogLevel MinLogLevel { get; set; }
        ILogger GetLogger(string name, Type type);
        ILogger GetLogger(string name);
    }
}