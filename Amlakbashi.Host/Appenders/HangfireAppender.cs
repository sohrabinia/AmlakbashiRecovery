using log4net.Appender;
using log4net.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace Amlakbashi.Host.Appenders
{
    public class HangfireAppender : AppenderSkeleton
    {
        private const string logDir = "Logs/HangfireLogs";
        private const string filePrefix = "HangfireLog-";

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.LoggerName.Contains("Hangfire."))
            {
                var rootPath = "";
#if DEBUG
                rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\")));
#else
                rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif
                var now = DateTime.Now;
                var fileName = Path.Combine(rootPath, logDir, filePrefix + now.Year + "-" +
                    now.Month + "-" + now.Day + "-" + now.Hour + ".txt");

                if (Directory.Exists(Path.Combine(rootPath, logDir)) == false)
                {
                    Directory.CreateDirectory(Path.Combine(rootPath, logDir));
                }

                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string log = "";
                    string logDate = loggingEvent.TimeStamp.Year + "-" +
                        loggingEvent.TimeStamp.Month + "-" +
                        loggingEvent.TimeStamp.Day + " " +
                        loggingEvent.TimeStamp.TimeOfDay;
                    string logDetails = logDate +
                        " [" + loggingEvent.ThreadName + "] [" +
                        loggingEvent.Level.Name + "] [" +
                        loggingEvent.LoggerName + "]\n";
                    var logLocation = loggingEvent.LocationInformation != null ?
                        "Location: " + loggingEvent.LocationInformation.FullInfo + "\n" : "";
                    var logMessage = "Message: " + loggingEvent.RenderedMessage + "\n";
                    var exceptionMessage = loggingEvent.ExceptionObject != null ?
                        "Exception: " + loggingEvent.ExceptionObject.Message + "\n" : "";
                    var innerExceptionMessage = loggingEvent.ExceptionObject != null &&
                        loggingEvent.ExceptionObject.InnerException != null ?
                        "InnerException: " + loggingEvent.ExceptionObject.InnerException.Message + "\n" : "";
                    var logSeperator = "--------------------------------------------------------------------------------";
                    log = logDetails + logLocation + logMessage + exceptionMessage + innerExceptionMessage + logSeperator;
                    sw.WriteLine(log);
                }
            }
        }
    }
}
