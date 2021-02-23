
using Amlakbashi.Core.Common.Utilities;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.Localization
{
    public static class LocalizationCalendarData
    {
        //private static Calendar defaultCalendar = Calendar.Jalali;
        public readonly static List<string> JalaliHolidays = DateTimeUtility.persian_holidays;
        public readonly static List<string> JalaliNorouzDays = DateTimeUtility.norouz_dates;
        private enum Calendar
        {
            Jalali = 0,
            Gregorian = 1
        }
    }
}
