using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Common.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class PriceDTO
    {
        public int DailyPrice { get; set; }
        public int HolidayPrice { get; set; }
        public int PeakHolidayPrice { get; set; }
        public int ExtraCapacityPrice { get; set; }
        public long MonthlyPrice { get; set; }
        public int NowruzPrice { get; set; }
        public int NowruzExtraCapacityPrice { get; set; }
    }
}
