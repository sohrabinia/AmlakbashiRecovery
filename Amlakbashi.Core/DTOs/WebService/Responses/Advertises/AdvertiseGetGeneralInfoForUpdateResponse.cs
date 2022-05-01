using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetGeneralInfoForUpdateResponse
    {
        public long id { get; set; }
        public int? province { get; set; }
        public int? city { get; set; }
        public int? area { get; set; }
        public string address { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public List<string> imagesUrls { get; set; }

        public static implicit operator AdvertiseGetGeneralInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetGeneralInfoForUpdateResponse()
            {
                id = advertise.Id,
                province = advertise.Province,
                city = advertise.City,
                area = advertise.Area,
                address = advertise.Address,
                longitude = advertise.Longitude,
                latitude = advertise.Latitude,
                title = advertise.Title,
                description = advertise.Description
            };
        }
    }
}
