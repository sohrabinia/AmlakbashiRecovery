using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Reserves
{
    public class ReserveResponse
    {
        public long reserveId { get; set; }
        public ReserveAdvertiseResponse residencyInfo { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int guestCount { get; set; }
        public string hostName { get; set; }
        public string hostPhoneNumber { get; set; }
        public string hostImageUrl { get; set; }

        public static implicit operator ReserveResponse(Reserve reserve)
        {
            var response = new ReserveResponse()
            {
                reserveId = reserve.Id,
                fromDate = DateTimeUtility.GregorianToPersianDate(reserve.StartDate),
                toDate = DateTimeUtility.GregorianToPersianDate(reserve.EndDate),
                guestCount = reserve.NumberOfGuests,
                hostName = reserve.HostUser.FullName,
                hostImageUrl = reserve.HostUser.PhotoID == null ? "" : $"/عکس-پروفایل_کوچک-{reserve.HostUser.PhotoID}",
                hostPhoneNumber = reserve.Status == Reserve.ReserveStatus.Reserved ||
                        reserve.Status == Reserve.ReserveStatus.Started ? reserve.HostUser.MainMobile : null,
                residencyInfo = new ReserveAdvertiseResponse()
                {
                    id = reserve.AdvertiseID,
                    title = reserve.Advertise.Title,
                    type = AdvertiseMainLocalization.GetAdvertiseTypeUserString(reserve.Advertise.TypeID),
                    roomCount = reserve.Advertise.Room,
                    provinceName = reserve.Advertise.RegionProvince?.PersianName,
                    cityName = reserve.Advertise.RegionCity?.PersianName,
                    address = reserve.Status == Reserve.ReserveStatus.Reserved ||
                        reserve.Status == Reserve.ReserveStatus.Started ? reserve.Advertise.Address : null,
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
        public string address { get; set; }
        public string imageUrl { get; set; }
    }
}
