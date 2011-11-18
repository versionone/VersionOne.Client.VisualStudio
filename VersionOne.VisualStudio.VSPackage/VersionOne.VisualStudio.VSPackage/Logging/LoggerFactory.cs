using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using VersionOne.VisualStudio.DataLayer.Logging;

namespace VersionOne.VisualStudio.VSPackage.Logging {
    internal class LoggerFactory : ILoggerFactory {
        private static ILoggerFactory instance;

        internal static ILoggerFactory Instance {
            get { return instance ?? (instance = new LoggerFactory()); }
        }

        static LoggerFactory() {
            var loggingConfiguration = new LoggingConfiguration();
            var fileTarget = new FileTarget {
                                 ArchiveAboveSize = 10485760,
                                 MaxArchiveFiles = 2,
                                 FileName = "${basedir}\\v1tracker-log.txt",
                             };
            loggingConfiguration.AddTarget("file", fileTarget);
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
            LogManager.Configuration = loggingConfiguration;
        }

        public ILogger GetLogger(string name, Type type) {
            return new Logger(LogManager.GetLogger(name, type));
        }

        public ILogger GetLogger(string name) {
            return new Logger(LogManager.GetLogger(name));
        }
    }
}