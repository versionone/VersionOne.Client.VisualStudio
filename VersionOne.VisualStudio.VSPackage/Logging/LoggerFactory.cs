using System;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;
using VersionOne.VisualStudio.DataLayer.Logging;

using LogLevel = VersionOne.VisualStudio.DataLayer.Logging.LogLevel;
using NLogLevel = NLog.LogLevel;

namespace VersionOne.VisualStudio.VSPackage.Logging {
    internal class LoggerFactory : ILoggerFactory {
        private readonly IDictionary<LogLevel, NLogLevel> levelMap = new Dictionary<LogLevel, NLogLevel> {
                                                                             {LogLevel.Debug, NLogLevel.Debug},
                                                                             {LogLevel.Info, NLogLevel.Info},
                                                                             {LogLevel.Warn, NLogLevel.Warn},
                                                                             {LogLevel.Error, NLogLevel.Error},
                                                                         };

        private LogLevel minLogLevel;

        public LogLevel MinLogLevel {
            get { return minLogLevel; } 
            set {
                minLogLevel = value;
                RecreateConfiguration();
            }
        }

        public LoggerFactory() {
            minLogLevel = LogLevel.Debug;
            RecreateConfiguration();
        }

        private void RecreateConfiguration() {
            var minimumLogLevel = TranslateLogLevel(MinLogLevel);
            var loggingConfiguration = new LoggingConfiguration();
            var fileTarget = new FileTarget {
                                 ArchiveAboveSize = 10485760,
                                 MaxArchiveFiles = 2,
                                 FileName = "${specialfolder:dir=.VersionOne.VisualStudioTracker:folder=MyDocuments}\\v1tracker-log.txt",
                                 Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message} | ${exception:format=ToString}",
                             };
            loggingConfiguration.AddTarget("file", fileTarget);
            loggingConfiguration.LoggingRules.Add(new LoggingRule("*", minimumLogLevel, fileTarget));
            LogManager.Configuration = loggingConfiguration;
        }

        public ILogger GetLogger(string name, Type type) {
            return new Logger(LogManager.GetLogger(name, type));
        }

        public ILogger GetLogger(string name) {
            return new Logger(LogManager.GetLogger(name));
        }

        private NLogLevel TranslateLogLevel(LogLevel level) {
            return levelMap.ContainsKey(level) ? levelMap[level] : NLogLevel.Debug;
        }
    }
}