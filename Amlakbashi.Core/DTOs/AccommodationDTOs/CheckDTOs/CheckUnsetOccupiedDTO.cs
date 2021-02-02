using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs
{
    public class CheckUnsetOccupiedDTO
    {
        public CheckUnsetOccupiedResult Result { get; set; }
        public IEnumerable<string> FailedDates { get; set; }

        public enum CheckUnsetOccupiedResult
        {
            OK = 0,
            ContainsReserved = 1
        }

        public override string ToString()
        {
            switch (Result)
            {
                case CheckUnsetOccupiedResult.ContainsReserved:
                    var dates_string = string.Join(" - ", FailedDates);
                    var msg = "محدوده انتخاب شده شامل روزهای رزرو شده میباشد: " + dates_string;
                    return msg;
                default:
                    return "آیا روز های انتخاب شده از روز های پر حذف شوند؟";
            }
        }
    }
}
