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
        public int HolidayPikePrice { get; set; }
        public int MoreThanCapacityPrice { get; set; }
        public long RentPrice { get; set; }
    }
}
