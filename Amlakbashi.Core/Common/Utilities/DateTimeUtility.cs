using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class DateTimeUtility
    {
        private static List<string> norouz_dates = new List<string>() {
                               "1398,12,28", "1398,12,29", "1399,1,1",
                               "1399,1,2", "1399,1,3","1399,1,4",
                               "1399,1,5", "1399,1,6", "1399,1,7",
                               "1399,1,8","1399,1,9", "1399,1,10",
                               "1399,1,11", "1399,1,12", "1399,1,13",
                               "1399,1,14", "1399,1,15"};

        private static List<string> persian_holidays = new List<string>() {
                               "1398,11,9", "1398,11,22",
                               "1398,12,18", "1398,12,29",
                               "1399,1,2", "1399,1,3", "1399,1,4",
                               "1399,1,12", "1399,1,13", "1399,1,21",
                               "1399,3,4", "1399,3,5",
                               "1399,3,14", "1399,3,15", "1399,3,28",
                               "1399,5,18", "1399,6,8", "1399,6,9",
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
                               "1400,12,9", "1400,12,29" };

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

        public static string ConvertDate(DateTime GDate, bool shorthand = false)
        {
            PersianCalendar objPersianCalendar = new PersianCalendar();
            var year = objPersianCalendar.GetYear(GDate).ToString();
            if (shorthand)
            {
                year = year.Remove(0, 2);
            }
            return year + "/" + objPersianCalendar.GetMonth(GDate) + "/" + objPersianCalendar.GetDayOfMonth(GDate);
        }

        public static int DiffDays(DateTime date)
        {
            return Convert.ToInt32((date - DateTime.Now).TotalDays);
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
            if (callTime.Hour < 8)
            {
                callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 8, 0, 0);
                delay = new TimeSpan(callTime.Ticks - now.Ticks);
            }
            else if (callTime.Hour >= 23)
            {
                callTime = new DateTime(callTime.Year, callTime.Month, callTime.Day, 8, 0, 0);
                callTime = callTime.AddDays(1);
                delay = new TimeSpan(callTime.Ticks - now.Ticks);
            }
            return delay;
        }

        public static List<string> GetHolidaysInGregorian()
        {
            var result = new List<string>();
            foreach (var item in persian_holidays)
            {
                var g_date = PersianDateToGregorian(item);
                if (g_date.Date >= DateTime.Now.Date)
                {
                    result.Add(g_date.ToString("yyyy-MM-dd"));
                }
            }
            return result;
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

        public static void GetPersianDateHolidayStatus(string persian_date, out bool is_holiday_or_between, out bool is_holiday_pike, out bool is_norouz)
        {
            if (IsNorouz(persian_date))
            {
                is_norouz = true;
                is_holiday_pike = true;
                is_holiday_or_between = true;
                return;
            }
            is_norouz = false;
            is_holiday_or_between = IsPersianDateHolidayOrBetween(persian_date);
            if (!is_holiday_or_between)
            {
                is_holiday_pike = false;
                return;
            }
            int previous_holidays = 0;
            int next_holidays = 0;
            var reach_a_normal_day = false;
            var gregorian_date = PersianDateToGregorian(persian_date);
            while (!reach_a_normal_day)
            {
                var previous_persian_date = GregorianToPersianDate(gregorian_date.AddDays(-(previous_holidays + 1)));
                if (IsPersianDateHolidayOrBetween(previous_persian_date))
                {
                    previous_holidays++;
                }
                else
                {
                    reach_a_normal_day = true;
                }
            }
            reach_a_normal_day = false;
            while (!reach_a_normal_day)
            {
                var next_persian_date = GregorianToPersianDate(gregorian_date.AddDays(next_holidays + 1));
                if (IsPersianDateHolidayOrBetween(next_persian_date))
                {
                    next_holidays++;
                }
                else
                {
                    reach_a_normal_day = true;
                }
            }
            is_holiday_pike = previous_holidays + next_holidays + 1 > 2;
        }
    }
}
