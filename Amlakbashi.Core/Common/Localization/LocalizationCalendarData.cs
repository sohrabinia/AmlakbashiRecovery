
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.Localization
{
    public static class LocalizationCalendarData
    {
        //private static Calendar defaultCalendar = Calendar.Jalali;
        public readonly static List<string> JalaliHolidays = new List<string>(){
            "1399,7,17", "1399,7,26", "1399,8,4",
            "1399,8,13", "1399,10,28", "1399,11,22",
            "1399,12,7", "1399,12,21", "1399,12,30",
            "1400,1,1", "1400,1,2", "1400,1,3",
            "1400,1,4", "1400,1,8", "1400,1,12",
            "1400,2,13", "1400,2,23", "1400,3,15",
            "1400,3,16", "1400,4,29", "1400,5,6",
            "1400,5,26", "1400,5,27", "1400,7,5",
            "1400,7,13", "1400,7,14", "1400,7,22",
            "1400,8,1", "1400,10,16", "1400,11,25",
            "1400,12,9", "1400,12,29"
        };
        public readonly static List<string> JalaliNorouzDays = new List<string>() {
            "1398,12,28", "1398,12,29", "1399,1,1",
            "1399,1,2", "1399,1,3","1399,1,4",
            "1399,1,5", "1399,1,6", "1399,1,7",
            "1399,1,8","1399,1,9", "1399,1,10",
            "1399,1,11", "1399,1,12", "1399,1,13",
            "1399,1,14", "1399,1,15"
        };
        private enum Calendar
        {
            Jalali = 0,
            Gregorian = 1
        }
    }
}
