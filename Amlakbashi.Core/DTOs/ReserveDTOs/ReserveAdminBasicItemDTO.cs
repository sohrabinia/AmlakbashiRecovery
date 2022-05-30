using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Core.Infrastructure.StyleHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.ReserveSupport;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveAdminBasicItemDTO
    {
        public long id { get; set; }
        public long accommodationId { get; set; }
        public long parentAccomodationId { get; set; }
        public int guestUserId { get; set; }
        public string guestName { get; set; }
        public int hostUserId { get; set; }
        public int status { get; set; }
        public string statusString { get; set; }
        public string statusColor { get; set; }
        public int hostResponse { get; set; }
        public string hostResponseString { get; set; }
        public string hostResponseColor { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string startDateString { get; set; }
        public string endDateString { get; set; }
        public int guestCount { get; set; }
        public int stayDays { get; set; }
        public int guestCallState { get; set; }
        public string guestCallStateColor { get; set; }
        public int hostCallState { get; set; }
        public string hostCallStateColor { get; set; }
        public List<SupporterHelperDTO> supporters { get; set; }
        public int paymentTryCount { get; set; }
        public string lastPaymentTryDate { get; set; }
        public int totalChatCount { get; set; }
        public int newChatCount { get; set; }
        public int supportInfoCount { get; set; }
        public bool instantReserve { get; set; }

        public static implicit operator ReserveAdminBasicItemDTO(Reserve reserve)
        {
            var linkAdvertise = reserve.Advertise.ParentOrSelf;
            var reserveSupports = reserve.GetRelatedSupports();
            var generatedSupporters = new List<SupporterHelperDTO>();
            foreach (var rs in reserveSupports)
            {
                if (rs.SupporterID == null)
                {
                    continue;
                }
                var supportingReserves = rs.GetReserveIds(SupportReserveStatus.Supporting);
                var similarReserves = rs.GetReserveIds(SupportReserveStatus.Similar);
                if (supportingReserves.Contains(reserve.Id) ||
                    similarReserves.Contains(reserve.Id))
                {
                    var supporterUser = rs.Supporter;
                    var fullName = supporterUser.FullName;
                    generatedSupporters.Add(new SupporterHelperDTO()
                    {
                        name = string.IsNullOrEmpty(fullName) ?
                            supporterUser.Id.ToString() : fullName,
                        imageId = supporterUser.PhotoID == null ? 0 : (long)supporterUser.PhotoID,
                        color = similarReserves.Contains(reserve.Id) ? "#FF7F00;" : "#34A853",
                        transferReason = rs.TransferReason
                    });
                }
            }
            string lastPayTryDate;
            var payTryCount = reserve.GetPaymentTriesCount(out lastPayTryDate);
            var infoList = reserve.GetSupportInfoList();
            var guestUser = reserve.GuestUser;
            var guestName = guestUser.FullName;
            if (string.IsNullOrEmpty(guestName))
            {
                guestName = reserve.UserID.ToString();
            }
            var dto = new ReserveAdminBasicItemDTO()
            {
                id = reserve.Id,
                accommodationId = reserve.AdvertiseID,
                parentAccomodationId = linkAdvertise.Id ==
                    reserve.AdvertiseID ? 0 : linkAdvertise.Id,
                status = (int)reserve.Status,
                guestUserId = reserve.UserID,
                guestName = guestName,
                hostUserId = reserve.HostUserID,
                guestCount = reserve.NumberOfGuests,
                statusString = ReserveLocalization.GetStatusString((int)reserve.Status,
                    Reserve.StatusStringType.Site),
                statusColor = ReserveStyleHelper.GetStatusColor((int)reserve.Status),
                hostResponse = (int)reserve.HostResponse,
                hostResponseString = ReserveLocalization.GetHostResponseString((int)reserve.HostResponse),
                hostResponseColor = ReserveStyleHelper.GetHostResponseColor((int)reserve.HostResponse),
                startDate = reserve.StartDate,
                endDate = reserve.EndDate,
                startDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.StartDate).Remove(0, 2),
                endDateString = DateTimeUtility.GregorianToPersianDate(
                    reserve.EndDate).Remove(0, 2),
                stayDays = DateTimeUtility.GetDatRangeDays(reserve.StartDate, reserve.EndDate),
                supporters = generatedSupporters,
                hostCallState = reserve.HostCallState,
                guestCallState = reserve.GuestCallState,
                hostCallStateColor = ReserveStyleHelper.GetCallStateColor(reserve.HostCallState),
                guestCallStateColor = ReserveStyleHelper.GetCallStateColor(reserve.GuestCallState),
                totalChatCount = reserve.ChatCount,
                newChatCount = reserve.ChatCountUnreadBySupport,
                paymentTryCount = payTryCount,
                lastPaymentTryDate = lastPayTryDate,
                supportInfoCount = infoList.Length,
                instantReserve = reserve.InstantReserve
            };
            return dto;
        }
    }
}
