using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Localization
{
    public interface ILocalization
    {
        string GetString(string key);
        List<string> JalaliHolidays { get; }
        List<string> JalaliNorouzDays { get; }
        bool IsNorouz(List<string> persian_date_range);
        void GetJalaliDateHolidayStatus(string persian_date,
            out bool is_holiday_or_between,
            out bool is_holiday_pike, out bool is_norouz);
    }
}
