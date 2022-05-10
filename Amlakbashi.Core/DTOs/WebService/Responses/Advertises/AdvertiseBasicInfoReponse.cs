using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseBasicInfoReponse
    {
        public long id { get; set; }
        public Advertise.AdvertiseStatus status { get; set; }
        public string statusTitle { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int roomCount { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string address { get; set; }
        public string imageUrl { get; set; }

        public static implicit operator AdvertiseBasicInfoReponse(Advertise advertise)
        {
            return new AdvertiseBasicInfoReponse()
            {
                id = advertise.Id,
                status = advertise.Status,
                statusTitle = AdvertiseMainLocalization.GetAdvertiseStatusString((int)advertise.Status, true),
                title = advertise.Title,
                type = AdvertiseMainLocalization.GetAdvertiseTypeUserString(advertise.TypeID),
                roomCount = advertise.Room,
                cityName = advertise.RegionCity?.PersianName,
                provinceName = advertise.RegionProvince?.PersianName,
                address = advertise.Address,
                imageUrl = advertise.GetMainImageApiUrl()
            };
        }
    }
}
