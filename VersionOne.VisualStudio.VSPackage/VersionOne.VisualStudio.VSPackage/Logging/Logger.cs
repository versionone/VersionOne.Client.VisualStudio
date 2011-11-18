using System;
using VersionOne.VisualStudio.DataLayer.Logging;
using NLogLogger = NLog.Logger;

namespace VersionOne.VisualStudio.VSPackage.Logging {
    internal class Logger : ILogger {
        private readonly NLogLogger wrappedLogger;

        internal Logger(NLogLogger logger) {
            this.wrappedLogger = logger;
        }

        public void Debug(string message) {
            wrappedLogger.Debug(message);
        }

        public void Debug(string message, Exception ex) {
            wrappedLogger.DebugException(message, ex);
        }

        public void Info(string message) {
            wrappedLogger.Info(message);
        }

        public void Info(string message, Exception ex) {
            wrappedLogger.Info(message, ex);
        }

        public void Warn(string message) {
            wrappedLogger.Warn(message);
        }

        public void Warn(string message, Exception ex) {
            wrappedLogger.Warn(message, ex);
        }

        public void Error(string message) {
            wrappedLogger.Error(message);
        }

        public void Error(string message, Exception ex) {
            wrappedLogger.Error(message, ex);
        }
    }
}