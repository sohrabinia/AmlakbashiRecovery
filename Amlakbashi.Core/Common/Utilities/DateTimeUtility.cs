using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class DateTimeUtility
    {
        public static List<string> norouz_dates = new List<string>() {
                               "1400,12,28", "1400,12,29", "1401,1,1",
                               "1401,1,2", "1401,1,3","1401,1,4",
                               "1401,1,5", "1401,1,6", "1401,1,7",
                               "1401,1,8","1401,1,9", "1401,1,10",
                               "1401,1,11", "1401,1,12", "1401,1,13"};

        public static List<string> persian_holidays = new List<string>() {
            "1401,1,1", "1401,1,2", "1401,1,3", "1401,1,4", "1401,1,13",
            "1401,2,3", "1401,2,13", "1401,2,14",
            "1401,3,14", "1401,3,15",
            "1401,4,19", "1401,4,27",
            "1401,5,16", "1401,5,17",
            "1401,6,26",
            "1401,7,3", "1401,7,5", "1401,7,13",
            "1401,10,6",
            "1401,11,15", "1401,11,22", "1401,11,29",
            "1401,12,17", "1401,12,29",

            "1402,1,1", "1402,1,2", "1402,1,3", "1402,1,4", "1402,1,12", "1402,1,13", "1402,1,24",
            "1402,2,2", "1402,2,3", "1402,2,26",
            "1402,3,14", "1402,3,15",
            "1402,4,8", "1402,4,16",
            "1402,5,6", "1402,5,7",
            "1402,6,15", "1402,6,23", "1402,6,25",
            "1402,7,2", "1402,7,11",
            "1402,9,26",
            "1402,11,6", "1402,11,20", "1402,11,22",
            "1402,12,7", "1402,12,29",
        };

        public static List<string> ManualHolidayPeakPersianDates
        {
            get
            {
                return new List<string>()
                {
                    "1401,4,26"
                };
            }
        }

        public static DateTime PersianDateToGregorian(string persian_date)
        {
            var persian_date_split = Array.ConvertAll(persian_date.Split(','), x => int.Parse(x));
            PersianCalendar persian_calendar = new PersianCalendar();
            DateTime gregorian_date = new DateTime(persian_date_split[0], persian_date_split[1], persian_date_split[2], persian_calendar);
            return gregorian_date;
        }

        public static string GregorianToPersianDate(DateTime gregorian_date, bool include_zero = false)
        {
            PersianCalendar persian_calendar = new PersianCalendar();
            if (include_zero)
            {
                return string.Format("{0},{1},{2}", persian_calendar.GetYear(gregorian_date),
                      persian_calendar.GetMonth(gregorian_date).ToString("D2"),
                      persian_calendar.GetDayOfMonth(gregorian_date).ToString("D2"));
            }
            else
            {
                return string.Format("{0},{1},{2}", persian_calendar.GetYear(gregorian_date),
                  persian_calendar.GetMonth(gregorian_date),
                  persian_calendar.GetDayOfMonth(gregorian_date));
            }
        }

        public static string GregorianToPersianDateWithSlash(DateTime gregorian_date)
        {
            PersianCalendar persian_calendar = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", persian_calendar.GetYear(gregorian_date),
                  persian_calendar.GetMonth(gregorian_date).ToString("D2"),
                  persian_calendar.GetDayOfMonth(gregorian_date).ToString("D2"));
        }

        public static bool IsValidPersianDate(string date)
        {
            var stringDateParts = date.Split(',');
            if (stringDateParts.Length != 3)
            {
                return false;
            }
            int integerDateParts;
            foreach (var item in stringDateParts)
            {
                if (int.TryParse(item, out integerDateParts) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static string PersianTodayString
        {
            get
            {
                var today = GregorianToPersianDate(DateTime.Now.Date, true);
                today = today.Replace(',', '/');
                today = StringUtility.EnglishNumberToPersian(today);
                return today;
            }
        }

        public static string GetPersianMonthName(int persian_month)
        {
            switch (persian_month)
            {
                case 1:
                    return "Farvardin";
                case 2:
                    return "Ordibehesht";
                case 3:
                    return "Khordad";
                case 4:
                    return "Tir";
                case 5:
                    return "Mordad";
                case 6:
                    return "Shahrivar";
                case 7:
                    return "Mehr";
                case 8:
                    return "Aban";
                case 9:
                    return "Azar";
                case 10:
                    return "Dey";
                case 11:
                    return "Bahman";
                case 12:
                    return "Esfand";
                default:
                    throw new IndexOutOfRangeException("out of range persian month.");
            }
        }

        public static List<string> PersianDateRangeToList(string from_date, string to_date,
            bool includeStartingDay, bool includeEndingDay)
        {
            var days = GetPersianDateRangeDays(from_date, to_date);
            var from_date_gregorian = PersianDateToGregorian(from_date);
            var persian_date_list = new List<string>();
            var startingDay = includeStartingDay ? 0 : 1;
            var endingDay = days + (includeEndingDay ? 1 : 0);
            for (int i = startingDay; i < endingDay; i++)
            {
                persian_date_list.Add(GregorianToPersianDate(from_date_gregorian.AddDays(i)));
            }
            return persian_date_list;
        }

        public static List<DateTime> DateRangeToList(DateTime fromDate, DateTime toDate, bool includeEndDay = false)
        {
            toDate = includeEndDay ? toDate : toDate.AddDays(-1);
            var days = GetDateRangeFromGregorianDate(fromDate, toDate);
            return days;
        }

        public static int GetPersianDateRangeDays(string from_date, string to_date)
        {
            var from_date_gregorian = PersianDateToGregorian(from_date);
            var to_date_gregorian = PersianDateToGregorian(to_date);
            return GetDatRangeDays(from_date_gregorian, to_date_gregorian);
        }

        public static int GetDatRangeDays(DateTime from_date, DateTime to_date)
        {
            return (int)(to_date - from_date).TotalDays;
        }

        public static long DateValueOfJS(DateTime date)
        {
            date = date.ToUniversalTime();
            DateTime startDt = new DateTime(1970, 1, 1);
            TimeSpan timeSpan = date - startDt;
            return (long)timeSpan.TotalMilliseconds;
        }

        public static DateTime JSValueToDate(long jsValue)
        {
            DateTime startDt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            var result = startDt.AddMilliseconds(jsValue).ToLocalTime();
            return result;
        }

        public static string ConvertDate(DateTime GDate, bool shorthand = false, bool withClock = false)
        {
            PersianCalendar objPersianCalendar = new PersianCalendar();
            var year = objPersianCalendar.GetYear(GDate).ToString();
            if (shorthand)
            {
                year = year.Remove(0, 2);
            }
            var datetime = year + "/" + objPersianCalendar.GetMonth(GDate) + "/" + objPersianCalendar.GetDayOfMonth(GDate);
            if (withClock)
            {
                datetime = datetime + " " + objPersianCalendar.GetHour(GDate) + ":" + objPersianCalendar.GetMinute(GDate);
            }
            return datetime;
        }

        public static DateTime ConvertDate(string JDateText)
        {
            PersianCalendar objPersianCalendar = new PersianCalendar();
            string[] strJDate = JDateText.Split('/');
            DateTime JDATE = objPersianCalendar.ToDateTime(int.Parse(strJDate[0]), int.Parse(strJDate[1]), int.Parse(strJDate[2]), 0, 0, 0, 0);
            return JDATE;
        }

        public static DateTime GetSiteClearingDate(DateTime start_date, DateTime end_date)
        {
            var days = (end_date - start_date).TotalDays;
            if (days > 1)
            {
                return start_date.AddDays(2);
            }
            else
            {
                return end_date;
            }
        }

        private static bool IsNorouz(string persian_date)
        {
            return norouz_dates.Contains(persian_date);
        }

        public static bool IsNorouz(List<string> persian_date_range)
        {
            return norouz_dates.Intersect(persian_date_range).Any();
        }

        public static bool IsNorouz(DateTime startDate, DateTime endDate)
        {
            var dateRange = GetDateRangeFromGregorianDate(startDate, endDate);
            var persianDateRange = dateRange.Select(s => GregorianToPersianDate(s)).ToList();
            return norouz_dates.Intersect(persianDateRange).Any();
        }

        public static string GetPersianDateDayOfWeek(string persian_date)
        {
            return GetDayOfWeekName(PersianDateToGregorian(persian_date).DayOfWeek);

        }

        public static string GetDayOfWeekName(DayOfWeek day_of_week)
        {
            switch (day_of_week)
            {
                case DayOfWeek.Sunday:
                    return "یکشنبه";
                case DayOfWeek.Monday:
                    return "دوشنبه";
                case DayOfWeek.Tuesday:
                    return "سه شنبه";
                case DayOfWeek.Wednesday:
                    return "چهارشنبه";
                case DayOfWeek.Thursday:
                    return "پنجشنبه";
                case DayOfWeek.Friday:
                    return "جمعه";
                case DayOfWeek.Saturday:
                    return "شنبه";
                default:
                    return "";
            }
        }

        private static List<DateTime> GetDateRangeFromGregorianDate(DateTime from, DateTime to)
        {
            var range_list = new List<DateTime>();
            while (from <= to)
            {
                range_list.Add(from);
                from = from.AddDays(1);
            }
            return range_list;
        }

        private static bool IsPersianDateBetweenHolidays(string persian_date)
        {
            var gregorian_date = PersianDateToGregorian(persian_date);
            var previous_gregorian_date = gregorian_date.AddDays(-1);
            var next_gregorian_date = gregorian_date.AddDays(1);
            return IsPersianDateHoliday(previous_gregorian_date) && IsPersianDateHoliday(next_gregorian_date);
        }

        private static bool IsPersianDateHolidayOrBetween(string persian_date)
        {
            return IsPersianDateHoliday(PersianDateToGregorian(persian_date)) || IsPersianDateBetweenHolidays(persian_date);
        }

        private static bool IsPersianDateHoliday(DateTime gregorian_date)
        {
            var persian_date = GregorianToPersianDate(gregorian_date.AddDays(1));
            return IsPersianDateFriday(persian_date) || persian_holidays.Contains(persian_date);
        }

        private static bool IsPersianDateFriday(string persian_date)
        {
            var day_of_week = PersianDateToGregorian(persian_date).DayOfWeek;
            return day_of_week == DayOfWeek.Thursday || day_of_week == DayOfWeek.Friday;
        }

        public static bool DateRangesHaveOverlap(DateTime date_1_from, DateTime date_1_to, DateTime date_2_from, DateTime date_2_to)
        {
            var range_1_from = GetDateRangeFromGregorianDate(date_1_from, date_1_to.AddDays(-1));
            var range_1_to = GetDateRangeFromGregorianDate(date_1_from.AddDays(1), date_1_to);
            var range_2_from = GetDateRangeFromGregorianDate(date_2_from, date_2_to.AddDays(-1));
            var range_2_to = GetDateRangeFromGregorianDate(date_2_from.AddDays(1), date_2_to);
            for (int i = 0; i < range_1_from.Count; i++)
            {
                if (range_2_from.Contains(range_1_from[i]))
                    return true;
            }
            for (int i = 0; i < range_1_to.Count; i++)
            {
                if (range_2_to.Contains(range_1_to[i]))
                    return true;
            }
            return false;
        }

        public static TimeSpan DelayAvoidingNightTime(TimeSpan desiredDelay)
        {
            var now = DateTime.Now;
            var delay = desiredDelay;
            var callTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            callTime = callTime.AddTicks(delay.Ticks);
            if (callTime.Hour < 10)
            {
                callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 10, 0, 0);
                delay = new TimeSpan(callTime.Ticks - now.Ticks);
            }
            else if (callTime.Hour >= 23)
            {
                callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 10, 0, 0);
                callTime = callTime.AddDays(1);
                delay = new TimeSpan(callTime.Ticks - now.Ticks);
            }
            return delay;
        }

        public static void GetCurrentPersianMonth(out int year, out int month)
        {
            var objPersianCalendar = new PersianCalendar();
            var now = DateTime.Now;
            month = objPersianCalendar.GetMonth(now);
            year = objPersianCalendar.GetYear(now);
        }

        public static void GetPreviousPersianMonth(out int year, out int month)
        {
            var objPersianCalendar = new PersianCalendar();
            var now = DateTime.Now;
            month = objPersianCalendar.GetMonth(now);
            year = objPersianCalendar.GetYear(now);
            if (month > 1)
            {
                month -= 1;
            }
            else
            {
                month = 12;
                year -= 1;
            }
        }

        public static bool IsStartDateLowerThanEndDate(string persianStartDate, string persianEndDate)
        {
            return PersianDateToGregorian(persianStartDate) < PersianDateToGregorian(persianEndDate);
        }
    }
}
