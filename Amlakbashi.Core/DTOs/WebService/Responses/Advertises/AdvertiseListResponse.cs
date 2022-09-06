using System;
using System.Collections.Generic;
using System.Text;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises
{
    public class AdvertiseListResponse
    {
        public List<AdvertiseListItemResponse> advertiseList { get; set; } = new List<AdvertiseListItemResponse>();
        public PagingInfo pagingInfo { get; set; }
        public string categoryTitle { get; set; }
    }

    public class AdvertiseListItemResponse
    {
        public long id { get; set; }
        public string title { get; set; }
        public long price { get; set; }
        public int discountPercent { get; set; }
        public bool favourited { get; set; }
        public int roomCount { get; set; }
        public string typeTitle { get; set; }
        public float rate { get; set; }
        public int rateCount { get; set; }
        public bool instantReserve { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public List<string> imagesUrls { get; set; } = new List<string>();


        public static implicit operator AdvertiseListItemResponse(Entities.Advertise advertise)
        {
            var response = new AdvertiseListItemResponse();
            int discountPercent = 0;
            var discount = advertise.GetFirstDiscountData(true, true);
            if (discount.Percent > 0)
            {
                discountPercent = discount.Percent;
            }
            response.id = advertise.Id;
            response.title = advertise.Title;
            response.price = advertise.BasePrice;
            response.typeTitle = AdvertiseMainLocalization.GetAdvertiseTypePersianNameForUser(advertise.TypeID);
            response.roomCount = advertise.RoomCount;
            response.rate = advertise.AverageUsersScore;
            response.rateCount = advertise.UserRatingDict().Count;
            response.instantReserve = advertise.InstantReserveStatus == Entities.Advertise.InstantReserveStatusEnum.Permanent;
            response.discountPercent = discountPercent;
            response.provinceName = advertise.RegionProvince.PersianName;
            response.cityName = advertise.RegionCity.PersianName;
            response.areaName = advertise.AreaId != null ? advertise.RegionArea.PersianName : null;
            response.imagesUrls = advertise.GetImagesUrls();
            return response;
        }
    }
}
