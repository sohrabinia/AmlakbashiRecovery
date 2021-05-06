using log4net.Appender;
using log4net.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;

namespace Amlakbashi.Host.Appenders
{
    public class LogAppender : AppenderSkeleton
    {
        private const string logDir = "Logs/Logs";
        private const string filePrefix = "Log-";
        private readonly string fileName;
        public LogAppender()
        {
            var rootPath = "";
#if DEBUG
            rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\")));
#else
            rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif
            var now = DateTime.Now;
            fileName = Path.Combine(rootPath, logDir, filePrefix + now.Year + "-" +
                now.Month + "-" + now.Day + "-" + now.Hour + ".txt");

            if (Directory.Exists(Path.Combine(rootPath, logDir)) == false)
            {
                Directory.CreateDirectory(Path.Combine(rootPath, logDir));
            }
        }
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (!loggingEvent.LoggerName.StartsWith("Hangfire."))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string logDate = loggingEvent.TimeStamp.Year + "-" +
                        loggingEvent.TimeStamp.Month + "-" +
                        loggingEvent.TimeStamp.Day + " " +
                        loggingEvent.TimeStamp.TimeOfDay;
                    string logMessage = logDate +
                        " [" + loggingEvent.ThreadName + "] [" +
                        loggingEvent.Level.Name + "] [" +
                        loggingEvent.LoggerName + "]\n" +
                        loggingEvent.RenderedMessage + "\n";
                    var exceptionMessage = loggingEvent.ExceptionObject != null ?
                        loggingEvent.ExceptionObject.Message + "\n" : "";
                    var innerExceptionMessage = loggingEvent.ExceptionObject != null &&
                        loggingEvent.ExceptionObject.InnerException != null ?
                        loggingEvent.ExceptionObject.InnerException.Message + "\n" : "";
                    logMessage = logMessage + exceptionMessage + innerExceptionMessage +
                        "--------------------------------------------------------------------------------";
                    sw.WriteLine(logMessage);
                }
            }
        }
    }
}
