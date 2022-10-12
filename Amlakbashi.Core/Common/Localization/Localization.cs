using System;
using System.Collections.Generic;
using System.Linq;
using Amlakbashi.Core.Common.Utilities;

namespace Amlakbashi.Core.Common.Localization
{
    public class Localization : ILocalization
    {
        public List<string> JalaliHolidays
        {
            get
            {
                return LocalizationCalendarData.JalaliHolidays;
            }
        }

        public List<string> JalaliNorouzDays
        {
            get
            {
                return LocalizationCalendarData.JalaliNorouzDays;
            }
        }

        public string GetString(string key)
        {
            return LocalizationStringData.Get(key);
        }

        public void GetJalaliDateHolidayStatus(string persian_date, out bool is_holiday_or_between, out bool is_holiday_pike, out bool is_norouz)
        {
            if (JalaliNorouzDays.Contains(persian_date))
            {
                is_norouz = true;
                is_holiday_pike = true;
                is_holiday_or_between = true;
                return;
            }
            is_norouz = false;
            if (DateTimeUtility.ManualHolidayPeakPersianDates.Contains(persian_date))
            {
                is_holiday_or_between = true;
                is_holiday_pike = true;
                return;
            }
            is_holiday_or_between = IsPersianDateHolidayOrBetween(persian_date);
            if (!is_holiday_or_between)
            {
                is_holiday_pike = false;
                return;
            }
            int previous_holidays = 0;
            int next_holidays = 0;
            var reach_a_normal_day = false;
            var gregorian_date = DateTimeUtility.PersianDateToGregorian(persian_date);
            while (!reach_a_normal_day)
            {
                var previous_persian_date = DateTimeUtility.GregorianToPersianDate(gregorian_date.AddDays(-(previous_holidays + 1)));
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
                var next_persian_date = DateTimeUtility.GregorianToPersianDate(
                    gregorian_date.AddDays(next_holidays + 1));
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

        public bool IsNorouz(List<string> persian_date_range)
        {
            return JalaliNorouzDays.Intersect(persian_date_range).Any();
        }

        #region private functions
        private bool IsPersianDateBetweenHolidays(string persian_date)
        {
            var gregorian_date = DateTimeUtility.PersianDateToGregorian(persian_date);
            var previous_gregorian_date = gregorian_date.AddDays(-1);
            var next_gregorian_date = gregorian_date.AddDays(1);
            return IsPersianDateHoliday(previous_gregorian_date) && IsPersianDateHoliday(next_gregorian_date);
        }
        private bool IsPersianDateHolidayOrBetween(string persian_date)
        {
            return IsPersianDateHoliday(DateTimeUtility.PersianDateToGregorian(persian_date)) || IsPersianDateBetweenHolidays(persian_date);
        }

        private bool IsPersianDateFriday(string persian_date)
        {
            var day_of_week = DateTimeUtility.PersianDateToGregorian(persian_date).DayOfWeek;
            return day_of_week == DayOfWeek.Thursday || day_of_week == DayOfWeek.Friday;
        }

        private bool IsPersianDateHoliday(DateTime gregorian_date)
        {
            var persian_date = DateTimeUtility.GregorianToPersianDate(gregorian_date.AddDays(1));
            return IsPersianDateFriday(persian_date) || JalaliHolidays.Contains(persian_date);
        }

        private bool IsNorouz(string persian_date)
        {
            return JalaliNorouzDays.Contains(persian_date);
        }
        #endregion
    }
}
