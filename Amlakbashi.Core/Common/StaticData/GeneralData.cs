using System;
using System.Diagnostics;

namespace Amlakbashi.Core.Common.StaticData
{
    public static class GeneralData
    {
        public static string WebsiteUrl
        {
            get
            {
#if DEBUG
                return "http://192.168.0.143:45455";
#endif
                return "https://www.amlakbashi.com";
            }
        }

        public static bool IsSupportOnline()
        {
            TimeSpan start = new TimeSpan(09, 00, 0);
            TimeSpan end = new TimeSpan(23, 30, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            var onlineSupport = (now > start) && (now < end);
            return onlineSupport;
        }
    }
}
