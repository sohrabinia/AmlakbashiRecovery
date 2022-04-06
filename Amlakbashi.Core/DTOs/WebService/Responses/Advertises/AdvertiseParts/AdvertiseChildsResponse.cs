using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises.AdvertiseParts
{
    public class AdvertiseChildsResponse
    {
        public long id { get; set; }
        public int roomCount { get; set; }
        public int capacity { get; set; }
        public int extraCapacity { get; set; }
        public int singleBedCount { get; set; }
        public int doubleBedCount { get; set; }
        public int price { get; set; }
        public int holidyPrice { get; set; }
        public int peakHolidayPrice { get; set; }
        public int extraCapacityPrice { get; set; }
        //public List<string> imagesUrls { get; set; } = new List<string>();
    }
}
