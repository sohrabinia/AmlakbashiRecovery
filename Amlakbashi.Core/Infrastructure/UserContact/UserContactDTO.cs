using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.UserContact
{
    public class UserContactDTO
    {
        public string UserFcmAppNotificationToken { get; set; }
        public string UserAppNotificationToken { get; set; }
        public string UserEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string UserNotificationToken { get; set; }
        public string UserMainMobile { get; set; }
        public Reserve.ReserveStatus ReserveStatus { get; set; }
        public UserContactType Type { get; set; } = UserContactType.Unset;
        public string AdvertiseId { get; set; } = null;
        public string UserId { get; set; } = null;
        public string ReserveId { get; set; } = null;
        public string TransactionId { get; set; } = null;
        public string AudienceMobile { get; set; } = null;
        public string Price { get; set; } = null;
        public string RemainPrice { get; set; } = null;
        public string DoerTitle { get; set; } = null;
        public string CauseString { get; set; } = null;
        public string Code { get; set; } = null;
        public string Extra1 { get; set; } = null;
        public string Extra2 { get; set; } = null;
        public string Extra3 { get; set; } = null;
        public bool FcmNotification { get; set; } = false;
    }

    public enum UserContactType
    {
        Unset = -1,
        confirm = 0,
        payment = 1,
        RefuseCancelReserve = 2,
        GuestRefuseCancelReserveByHost = 3,
        GuestStayStarted = 4,
        HostCancelRequestSent = 5,
        GuestCancelRequestSent = 6,
        GuestReserveCanceled = 7,
        HostReserveCanceled = 8,
        GuestReserveCanceledByHost = 9,
        GuestReserveRejected = 10,
        FinishStay = 11,
        NewReserveChatHost = 12,
        NewReserveChatGuest = 13,
        GuestPayReserve = 14,
        HostReserveCashPay = 15,
        GuestReservedTotalPayed = 16,
        HostReservedTotalPayed = 17,
        GuestReservedDepositePayed = 18,
        HostReservedDepositePayed = 19,
        ReserveCanceledBySystem = 20,
        ReserveRequest = 21,
        HostReserveRejectedForReserved = 22,
        SiteClearingHost = 23,
        SiteClearingHostWithCredit = 24,
        SiteRefundGuest = 25,
        UserCreditIncrease = 26,
        UserCreditDecrease = 27,
        HostUpdatePrice = 28,
        PrizeCharge = 29,
        CouponPresent = 30,
        CouponAppreciate = 31
    }

    public enum ContactMethod
    {
        Mobile = 0,
        Email = 1,
        Notification = 2
    }
}
