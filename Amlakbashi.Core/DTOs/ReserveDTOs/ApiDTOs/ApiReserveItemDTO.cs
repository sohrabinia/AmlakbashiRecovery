using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.ReserveDTOs.ApiDTOs
{
    [Serializable]
    public class ApiReserveItemDTO
    {
        public long id { get; set; }
        public string title { get; set; }
        public long advertiseId { get; set; }
        public long photoId { get; set; }
        public int status { get; set; }
        public string statusString { get; set; }
        public string statusColor { get; set; }
        public DateTime createDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public long totalPrice { get; set; }
        public long depositePrice { get; set; }
        public long paidPrice { get; set; }
        public long couponPrice { get; set; }
        public long prizePrice { get; set; }
        public int guestCount { get; set; }
        public bool chatAvailable { get; set; }
        public bool callAvailable { get; set; }
        public int chatCount { get; set; }
        public bool cancelAvailable { get; set; }
        public bool ratingAvailable { get; set; }
        public string dateString { get; set; }
        public int days { get; set; }
        public string partyMobile { get; set; }
        public string partyName { get; set; }
        public bool instantReserve { get; set; }

        public static implicit operator ApiReserveItemDTO(Reserve reserve)
        {
            var status = (Reserve.ReserveStatus)reserve.Status;
            var call_available = status == Reserve.ReserveStatus.Reserved
                    || status == Reserve.ReserveStatus.CashPay
                    || status == Reserve.ReserveStatus.Started
                    || status == Reserve.ReserveStatus.CancelRequestByGuest
                    || status == Reserve.ReserveStatus.CancelRequestByHost;
            var from_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.StartDate);
            var to_date_persian = DateTimeUtility.GregorianToPersianDate(reserve.EndDate);
            from_date_persian = DateTimeUtility.GetPersianDateDayOfWeek(from_date_persian) +
                " " + from_date_persian.Replace(",", "/").Remove(0, 2);
            to_date_persian = DateTimeUtility.GetPersianDateDayOfWeek(to_date_persian) +
                " " + to_date_persian.Replace(",", "/").Remove(0, 2);

            var dto = new ApiReserveItemDTO();
            dto.id = reserve.Id;
            dto.title = reserve.Advertise.Title;
            dto.advertiseId = reserve.AdvertiseID;
            dto.createDate = reserve.CreateDate;
            dto.startDate = reserve.StartDate;
            dto.endDate = reserve.EndDate;
            dto.status = (int)reserve.Status;
            dto.guestCount = reserve.NumberOfGuests;
            dto.totalPrice = reserve.TotalPrice;
            dto.depositePrice = reserve.DepositPrice;
            dto.couponPrice = reserve.CouponPrice;
            dto.prizePrice = reserve.PrizePrice;
            dto.ratingAvailable = status == Reserve.ReserveStatus.Started || status == Reserve.ReserveStatus.Completed;
            dto.chatAvailable = status == Reserve.ReserveStatus.WaitForResponse
                || status == Reserve.ReserveStatus.WaitForReserve
                || status == Reserve.ReserveStatus.Reserved
                || status == Reserve.ReserveStatus.CashPay
                || status == Reserve.ReserveStatus.Started
                || status == Reserve.ReserveStatus.CancelRequestByGuest
                || status == Reserve.ReserveStatus.CancelRequestByHost;
            dto.callAvailable = call_available;
            dto.days = DateTimeUtility.GetDatRangeDays(reserve.StartDate.Date, reserve.EndDate.Date);
            dto.dateString = from_date_persian + " تا " + to_date_persian;
            dto.photoId = reserve.Advertise.MainPhotoId == null ? 0 : (int)reserve.Advertise.MainPhotoId;
            dto.statusColor = ReserveStyleHelper.GetStatusColor((int)reserve.Status);
            return dto;
        }
    }
}
