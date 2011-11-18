using System;

namespace VersionOne.VisualStudio.DataLayer.Logging {
    public interface ILoggerFactory {
        ILogger GetLogger(string name, Type type);
        ILogger GetLogger(string name);
    }
}