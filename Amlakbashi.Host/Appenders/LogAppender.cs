using log4net.Appender;
using log4net.Core;
using System;
using System.IO;
using System.Reflection;

namespace Amlakbashi.Host.Appenders
{
    public class LogAppender : AppenderSkeleton
    {
        private const string filePrefix = "Logs/Logs/Log-";
        private readonly string fileName;
        public LogAppender()
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\")));
            var now = DateTime.Now;
            fileName = Path.Combine(rootPath, filePrefix + now.Year + "-" +
                now.Month + "-" + now.Day + "-" + now.Hour + ".txt");
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
