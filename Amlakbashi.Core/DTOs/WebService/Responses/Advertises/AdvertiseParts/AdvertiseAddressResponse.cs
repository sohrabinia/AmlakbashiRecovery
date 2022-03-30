using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises.AdvertiseParts
{
    public class AdvertiseAddressResponse
    {
        public string countryDirection { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
