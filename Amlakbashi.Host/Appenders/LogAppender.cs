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
        private string logDir = "Logs/Logs";
        private string filePrefix = "Log-";

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.LoggerName.Contains("ResponseCaching") == false &&
                loggingEvent.LoggerName.StartsWith("Hangfire.") == false)
            {
                if (loggingEvent.RenderedMessage.Contains("Impersonation:"))
                {
                    logDir = "Logs/ImpersonationLogs";
                    filePrefix = "ImpersonationLog-";
                }

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

                        var logUrl = request != null ? "#Url: " + request.Path + request.QueryString + "\n" : "";

                        var logRefererUrl = request != null ? "#RefererUrl: " + request.Headers["referer"] + "\n" : "";

                        var logPostedData = "";
                        if (request != null && request.Method.ToLower() == "post")
                        {
                            logPostedData = "#PostedData:\n";
                            foreach (var item in request.Form)
                            {
                                logPostedData = logPostedData + item.Key + ": " + item.Value + "\n";
                            }
                        }

                        var logLocation = loggingEvent.LocationInformation != null ?
                            "#Location: " + loggingEvent.LocationInformation.FullInfo + "\n" : "";

                        var logMessage = "#Message: " + loggingEvent.RenderedMessage + "\n";

                        var exceptionMessage = loggingEvent.ExceptionObject != null ?
                            "#Exception: " + loggingEvent.ExceptionObject.Message + "\n" : "";

                        var innerExceptionMessage = loggingEvent.ExceptionObject != null &&
                            loggingEvent.ExceptionObject.InnerException != null ?
                            "#InnerException: " + loggingEvent.ExceptionObject.InnerException.Message + "\n" : "";

                        //var logStack = "StackTrace:\n";
                        //foreach (var item in loggingEvent.LocationInformation.StackFrames)
                        //{
                        //    logStack = logStack + item.FullInfo + "\n";
                        //}

                        var logSeperator = "--------------------------------------------------------------------------------";

                        log = logDetails + logUrl + logRefererUrl + logPostedData + logLocation + logMessage +
                            exceptionMessage + innerExceptionMessage + logSeperator;
                        sw.WriteLine(log);
                    }
                    catch (Exception exc)
                    {
                        string log = "LogAppender encountered with error:\n";
                        log = log + exc.Message + "\n"
                            + "--------------------------------------------------------------------------------";
                        sw.WriteLine(log);
                    }
                }
            }
        }
    }
}
