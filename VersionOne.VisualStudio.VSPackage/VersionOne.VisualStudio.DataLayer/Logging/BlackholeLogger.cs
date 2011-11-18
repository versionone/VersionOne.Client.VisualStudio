using System;

namespace VersionOne.VisualStudio.DataLayer.Logging {
    public class BlackholeLogger : ILogger {
        public void Debug(string message) {}

        public void Debug(string message, Exception ex) {}

        public void Info(string message) {}

        public void Info(string message, Exception ex) {}

        public void Warn(string message) {}

        public void Warn(string message, Exception ex) {}

        public void Error(string message) {}

        public void Error(string message, Exception ex) {}
    }
}