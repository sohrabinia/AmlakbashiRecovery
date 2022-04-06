using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseCalendarResponse
    {
        public IList<long> occupiedDates { get; set; }
        public IList<AdvertiseCalendarPriceItemResponse> prices { get; set; }
    }

    public class AdvertiseCalendarPriceItemResponse
    {
        public string date { get; set; }
        public int price { get; set; }
        public int discount { get; set; }
    }
}
