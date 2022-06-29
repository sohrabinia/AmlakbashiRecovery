using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveDashboardItemDTO
    {
        public long reserveId { get; set; }
        public bool instantReserve { get; set; }
        public long advertiseId { get; set; }
        public int index { get; set; }
        public bool isGuest { get; set; }
        public bool isHostler { get; set; }
        public long advertisePhotoId { get; set; }
        public string advertiseTitle { get; set; }
        public string advertiseSlug { get; set; }
        public int userId { get; set; }
        public int hostlerUserId { get; set; }
        public int guestUserId { get; set; }
        public long audiencePhotoId { get; set; }
        public string startDateString { get; set; }
        public int staydays { get; set; } 
        public int guestCount { get; set; }
        public long totalPrice { get; set; }
        public long depositePrice { get; set; }
        public long paidAmount { get; set; }
        public long remainedAmount { get; set; }
        public ReserveStatus status { get; set; }
        public string statusString { get; set; }
        public string statusColor { get; set; }
        public int unreadChatCount { get; set; }
        public bool cancelIsAvailable { get; set; }
        public string rulesString { get; set; }
        public bool isRecently { get; set; }
        public EarlyCheckoutEnum earlyCheckout { get; set; }

        public static ReserveDashboardItemDTO Generate(Reserve reserve,
            int index, bool isGuest, bool isHostler, int userId, long paidAmount,
            int unreadChatCount, Dictionary<string,string> rulesDict)
        {
            var advertise = reserve.Advertise;
            var dto = new ReserveDashboardItemDTO();
            dto.reserveId = reserve.Id;
            dto.instantReserve = reserve.InstantReserve;
            dto.advertiseId = reserve.AdvertiseID;
            dto.index = index;
            dto.isGuest = isGuest;
            dto.isHostler = isHostler;
            dto.advertisePhotoId = advertise.PhotoID == null ? 0 : (int)advertise.PhotoID;
            dto.advertiseTitle = advertise.Title;
            dto.advertiseSlug = advertise.Slug;
            dto.userId = userId;
            dto.hostlerUserId = reserve.HostUserID;
            dto.guestUserId = reserve.UserID;
            dto.audiencePhotoId = isGuest ?
                (reserve.HostUser.PhotoStatus != 2 ? 0 : (reserve.HostUser.PhotoID == null ? 0 : (long)reserve.HostUser.PhotoID)) :
                (reserve.GuestUser.PhotoStatus != 2 ? 0 : (reserve.GuestUser.PhotoID == null ? 0 : (long)reserve.GuestUser.PhotoID));
            dto.startDateString = DateTimeUtility.GregorianToPersianDate(reserve.StartDate);
            dto.staydays = DateTimeUtility.GetDatRangeDays(reserve.StartDate, reserve.EndDate);
            dto.guestCount = reserve.NumberOfGuests;
            dto.totalPrice = reserve.TotalPrice;
            dto.depositePrice = reserve.DepositPrice;
            dto.paidAmount = paidAmount;
            dto.remainedAmount = dto.totalPrice - paidAmount;
            dto.status = reserve.Status;
            dto.statusString = ReserveLocalization.GetStatusString((int)reserve.Status,
                isGuest ? StatusStringType.Guest : StatusStringType.Host, reserve.Id,
                reserve.HostResponse);
            dto.statusColor = ReserveStyleHelper.GetStatusColor((int)reserve.Status);
            dto.unreadChatCount = unreadChatCount;
            dto.rulesString = SerializeUtility.SerializeToJS(rulesDict);
            dto.cancelIsAvailable = (isGuest && Reserve.CancelIsAvailableForGuest((int)reserve.Status)) ||
                (isHostler && Reserve.CancelIsAvailableForHost((int)reserve.Status));
            dto.isRecently = (DateTime.Now - reserve.CreateDate).TotalHours <= 24 ? true : false;
            dto.earlyCheckout = reserve.EarlyCheckoutStatus;
            return dto;
        }
    }
}
