using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseSearchIdResponse
    {
        public long id { get; set; }
        public string typeTitle { get; set; }
        public int roomCount { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string title { get; set; }
        public string imageUrl { get; set; }
    }
}
