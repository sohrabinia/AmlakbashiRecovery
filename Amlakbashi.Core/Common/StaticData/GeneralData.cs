using Microsoft.AspNetCore.Hosting;
using System;

namespace Amlakbashi.Core.Common.StaticData
{
    public static class GeneralData
    {
        public static IWebHostEnvironment WebHostEnvironment { get; set; }

        public static string WebsiteUrl
        {
            get
            {
                return WebHostEnvironment.EnvironmentName == "Production" ? "https://www.amlakbashi.com" :
                    "http://192.168.0.172:45455";
            }
        }

        public static string VideosDirectoryDrive
        {
            get
            {
                return WebHostEnvironment.EnvironmentName == "Production" ? "F:/videos" :
                    $"{WebHostEnvironment.WebRootPath}/content/videos";
            }
        }

        public static bool IsSupportersOnline()
        {
            TimeSpan start = new TimeSpan(09, 00, 0);
            TimeSpan end = new TimeSpan(21, 00, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            var onlineSupport = (now > start) && (now < end);
            return onlineSupport;
        }
    }
}
