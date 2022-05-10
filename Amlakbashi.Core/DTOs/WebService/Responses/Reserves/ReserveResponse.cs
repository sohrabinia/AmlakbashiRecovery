using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.WebService.Responses.Advertises;
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
        public Reserve.ReserveStatus status { get; set; }
        public string statusTitle { get; set; }
        public long price { get; set; }
        public DateTime? expireTime { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int guestCount { get; set; }
        public string hostName { get; set; }
        public string hostPhoneNumber { get; set; }
        public string hostImageUrl { get; set; }
        public string guestName { get; set; }
        public string guestPhoneNumber { get; set; }
        public string guestImageUrl { get; set; }
        public AdvertiseBasicInfoReponse residencyInfo { get; set; }


        public static implicit operator ReserveResponse(Reserve reserve)
        {
            var response = new ReserveResponse()
            {
                reserveId = reserve.Id,
                fromDate = DateTimeUtility.GregorianToPersianDate(reserve.StartDate),
                toDate = DateTimeUtility.GregorianToPersianDate(reserve.EndDate),
                guestCount = reserve.NumberOfGuests,
                status = reserve.Status,
                statusTitle = ReserveLocalization.GetStatusString((int)reserve.Status, Reserve.StatusStringType.Site),
                price = reserve.TotalPrice,
                hostName = reserve.HostUser.FullName,
                hostImageUrl = reserve.HostUser.GetUserImageApiUrl(),
                hostPhoneNumber = reserve.Status == Reserve.ReserveStatus.Reserved ||
                        reserve.Status == Reserve.ReserveStatus.Started ? reserve.HostUser.MainMobile : null,
                guestName = reserve.GuestUser.FullName,
                guestImageUrl = reserve.GuestUser.GetUserImageApiUrl(),
                guestPhoneNumber = reserve.Status == Reserve.ReserveStatus.Reserved ||
                        reserve.Status == Reserve.ReserveStatus.Started ? reserve.GuestUser.MainMobile : null,
                residencyInfo = reserve.Advertise
            };
            return response;
        }
    }
}
