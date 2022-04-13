using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Reserves
{
    public class ReserveListResponse
    {
        public List<ReserveListItemResponse> reserveList { get; set; } = new List<ReserveListItemResponse>();
        public PagingInfo pagingInfo { get; set; }
    }

    public class ReserveListItemResponse
    {
        public long id { get; set; }
        public ReserveAdvertiseResponse residencyInfo { get; set; }
        public Reserve.ReserveStatus status { get; set; }
        public string statusTitle { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int guestCount { get; set; }
        public long price { get; set; }
        public string hostName { get; set; }
        public string hostImageUrl { get; set; }
        public string remainedTime { get; set; }

        public static implicit operator ReserveListItemResponse(Reserve reserve)
        {
            var response = new ReserveListItemResponse()
            {
                id = reserve.Id,
                fromDate = DateTimeUtility.GregorianToPersianDate(reserve.StartDate),
                toDate = DateTimeUtility.GregorianToPersianDate(reserve.EndDate),
                guestCount = reserve.NumberOfGuests,
                status = reserve.Status,
                statusTitle = ReserveLocalization.GetStatusString((int)reserve.Status,
                    Reserve.StatusStringType.Guest, reserve.Id, reserve.HostResponse),
                price = reserve.TotalPayablePrice,
                hostName = reserve.HostUser.FullName,
                hostImageUrl = reserve.HostUser.PhotoID == null ? "" : $"/عکس-پروفایل_کوچک-{reserve.HostUser.PhotoID}",
                remainedTime = "",
                residencyInfo = new ReserveAdvertiseResponse()
                {
                    id = reserve.AdvertiseID,
                    title = reserve.Advertise.Title,
                    type = AdvertiseMainLocalization.GetAdvertiseTypeUserString(reserve.Advertise.TypeID),
                    roomCount = reserve.Advertise.Room,
                    provinceName = reserve.Advertise.RegionProvince?.PersianName,
                    cityName = reserve.Advertise.RegionCity?.PersianName,
                    imageUrl = $"/file/accthumbxxxlarge?accid={reserve.AdvertiseID}&fileid={reserve.Advertise.PhotoID}"
                }
            };
            return response;
        }
    }

    public class ReserveAdvertiseResponse
    {
        public long id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int roomCount { get; set; }
        public string provinceName { get; set; }
        public string cityName { get; set; }
        public string imageUrl { get; set; }
    }
}
