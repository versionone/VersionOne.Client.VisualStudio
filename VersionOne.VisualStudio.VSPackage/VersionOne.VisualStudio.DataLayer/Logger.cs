using System;
using System.Diagnostics;
using System.IO;

// TODO really it should be NLog or log4net wrapper
namespace VersionOne.VisualStudio.DataLayer {
    [Obsolete]
    public class Logger {
        private const string LogFileName = "log.txt";

        public static void Info(string message) {
            LogMessageToFile("Info", message);
        }

        public static void Warning(string message) {
            LogMessageToFile("Warn", message);
        }
        
        public static void Warning(string message, Exception ex) {
            Warning(FormatMessageAndException(message, ex));
        }

        // TODO we should not stick to a single exception type
        public static void Error(string message) {
            LogMessageToFile("Error", message);
            throw new DataLayerException(message);
        }

        public static void Error(string message, Exception ex) {
            LogMessageToFile("Error", FormatMessageAndException(message, ex));
            throw new DataLayerException(message);
        }

        private static void LogMessageToFile(string severity, string message) {
            Debug.WriteLine(message);
            
            string formattedMessage = string.Format("[{0}] {1} : {2}{3}{3}", severity, DateTime.Now.ToString(), message, Environment.NewLine);

            try {
                File.AppendAllText(LogFileName, formattedMessage);
            } catch {
                // Do nothing. Logging failure should not cause application failure.
            }
        }

        private static string FormatMessageAndException(string message, Exception ex) {
            return string.Format("{0}.\n\tException:{1}\n\t Stacktrace:{2}", message, ex.Message, ex.StackTrace);
        }
    }
}