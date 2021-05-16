using log4net.Appender;
using log4net.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;

namespace Amlakbashi.Host.Appenders
{
    public class ImpersonationAppender : AppenderSkeleton
    {
        private const string logDir = "Logs/ImpersonationLogs";
        private const string filePrefix = "ImpersonationLog-";

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.RenderedMessage.Contains("Impersonation"))
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

                HttpContext context = new HttpContextAccessor().HttpContext;
                HttpRequest request = context != null ? context.Request : null;
                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    try
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

                        var logUrl = request != null ? "Url: " + request.Path + request.QueryString + "\n" : "";

                        var logMessage = loggingEvent.RenderedMessage + "\n";

                        var logSeperator = "--------------------------------------------------------------------------------";

                        log = logDetails + logUrl + logMessage + logSeperator;
                        sw.WriteLine(log);
                    }
                    catch (Exception exc)
                    {
                        string log = "ImpersonationAppender encountered with error:\n";
                        log = log + exc.Message + "\n"
                            + "--------------------------------------------------------------------------------";
                        sw.WriteLine(log);
                    }
                }
            }
        }
    }
}
