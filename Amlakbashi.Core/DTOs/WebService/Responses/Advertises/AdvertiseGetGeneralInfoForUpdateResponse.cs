using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseGetGeneralInfoForUpdateResponse
    {
        public long residenceId { get; set; }
        public int? provinceId { get; set; }
        public int? cityId { get; set; }
        public int? areaId { get; set; }
        public string address { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public long? mainImageId { get; set; }
        public Dictionary<long, string> images { get; set; }

        public static implicit operator AdvertiseGetGeneralInfoForUpdateResponse(Advertise advertise)
        {
            return new AdvertiseGetGeneralInfoForUpdateResponse()
            {
                residenceId = advertise.Id,
                provinceId = advertise.ProvinceId,
                cityId = advertise.CityId,
                areaId = advertise.AreaId,
                address = advertise.Address,
                longitude = advertise.Longitude,
                latitude = advertise.Latitude,
                title = advertise.Title,
                description = advertise.Description,
                mainImageId = advertise.MainPhotoId,
                images = advertise.GetImagesIdAndUrls()
            };
        }
    }
}
