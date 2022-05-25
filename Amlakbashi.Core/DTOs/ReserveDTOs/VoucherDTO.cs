using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class VoucherDTO
    {
        public long accId { get; set; }
        public string accTitle { get; set; }
        public string accTypeString { get; set; }
        public string accAddress { get; set; }
        public int accCapacity { get; set; }
        public int accRoomCount { get; set; }
        public string accRoomCountString { get; set; }
        public long hostUserId { get; set; }
        public string hostFullName { get; set; }
        public string hostMobile { get; set; }
        public long reserveId { get; set; }
        public string requestDateString { get; set; }
        public string startDateString { get; set; }
        public string endDateString { get; set; }
        public string reserveDaysString { get; set; }
        public long guestUserId { get; set; }
        public string guestFullName { get; set; }
        public string guestMobile { get; set; }        
        public int guestCount { get; set; }
        public long reserveTotalPrice { get; set; }
        public long reservePaidAmount { get; set; }
        public long reserveRemainedAmount { get; set; }
        public bool isInvoice { get; set; }

        public static VoucherDTO Generate(Reserve reserve, long paidAmount, bool isInvoice)
        {
            var expired = (DateTime.Now.Date - reserve.EndDate).TotalDays > 0;
            var dto = new VoucherDTO();
            dto.isInvoice = isInvoice;
            var province = reserve.Advertise.RegionProvince;
            var city = reserve.Advertise.RegionCity;
            var area = reserve.Advertise.RegionArea;
            var locationString = RegionLocalization.GetLocationString(
                province == null ? null : province.PersianName,
                city == null ? null : city.PersianName,
                area == null ? null : area.PersianName,
                null);
            dto.accId = reserve.AdvertiseID;
            dto.reserveId = reserve.Id;
            dto.accTitle = reserve.Advertise.Title;
            dto.accTypeString = AdvertiseMainLocalization.GetAdvertiseTypeUserString(reserve.Advertise.TypeID);
            dto.accAddress = expired || isInvoice ? locationString : locationString + " - " + reserve.Advertise.Address;
            dto.accCapacity = reserve.Advertise.Capacity;
            dto.accRoomCount = reserve.Advertise.Room;
            dto.accRoomCountString = reserve.Advertise.Room < 1 ? "بدون اتاق" : reserve.Advertise.Room + " خوابه";
            dto.hostUserId = reserve.HostUserID;
            dto.hostFullName = reserve.HostUser.FullName;
            dto.hostMobile = expired || isInvoice ? "-" : reserve.HostUser.GetNormalizedNoticesPhoneNumber();
            dto.guestUserId = reserve.UserID;
            dto.guestMobile = isInvoice ? "-" : reserve.GuestUser.GetNormalizedNoticesPhoneNumber();
            dto.guestFullName = reserve.GuestUser.FullName;
            dto.guestCount = reserve.NumberOfGuests;
            dto.reserveTotalPrice = reserve.TotalPrice;
            dto.reservePaidAmount = paidAmount;
            dto.reserveRemainedAmount = dto.reserveTotalPrice - dto.reservePaidAmount;
            dto.reserveDaysString = DateTimeUtility.GetDatRangeDays(reserve.StartDate, reserve.EndDate) + " شب";
            dto.requestDateString = StringUtility.EnglishNumberToPersian(DateTimeUtility.GregorianToPersianDate(reserve.CreateDate).Replace(",", "/"));
            dto.startDateString = StringUtility.EnglishNumberToPersian(DateTimeUtility.GregorianToPersianDate(reserve.StartDate).Replace(",", "/"));
            dto.endDateString = StringUtility.EnglishNumberToPersian(DateTimeUtility.GregorianToPersianDate(reserve.EndDate).Replace(",", "/"));
            return dto;
        }
    }
}
