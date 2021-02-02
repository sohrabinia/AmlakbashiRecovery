using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.CheckDTOs
{
    public class CheckSetOccupiedDTO
    {
        public CheckSetOccupiedResult Result { get; set; }
        public IEnumerable<string> FailedDates { get; set; }

        public enum CheckSetOccupiedResult
        {
            OK = 0,
            ContainsReserved = 1,
            ContainsAcceptedRerserve = 2,
            ContainsReserveRequest = 3
        }

        public override string ToString()
        {
            switch (Result)
            {
                case CheckSetOccupiedResult.ContainsReserved:
                    var dates_string = string.Join(" - ", FailedDates);
                    var msg = "محدوده انتخاب شده شامل روزهای رزرو شده میباشد: " + dates_string;
                    return msg;
                case CheckSetOccupiedResult.ContainsAcceptedRerserve:
                    return "در محدوده انتخاب شده یک درخواست رزرو تایید شده وجود دارد";
                case CheckSetOccupiedResult.ContainsReserveRequest:
                    return "شما در این روز یک درخواست رزرو دارید. در صورت اعلام پر بودن درخواست لغو میشود.";
                default:
                    return "آیا روز های انتخاب شده به عنوان روز پر ثبت شود؟";
            }
        }
    }
}
