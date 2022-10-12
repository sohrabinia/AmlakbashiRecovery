using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static Amlakbashi.Core.Entities.ReservePayment;

namespace Amlakbashi.Core.Entities
{
    public class Reserve : Entity<long>, IReserve
    {
        #region Properties

        [Column("Id")]
        public override long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public ReserveStatus Status { get; set; }

        [Column("GuestId")]
        public int UserID { get; set; }

        [Column("HostId")]
        public int HostUserID { get; set; }

        [Column("ResidenceId")]
        public long AdvertiseID { get; set; }
        public HostResponseEnum HostResponse { get; set; }
        public DateTime HostResponseDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfGuests { get; set; }
        public bool InstantReserve { get; set; }
        public bool InstantReserveCancelHost { get; set; }
        public long TotalPrice { get; set; }
        public long DepositPrice { get; set; }

        [Column("CouponId")]
        public long CouponID { get; set; }
        public long CouponPrice { get; set; }
        public long CouponCalculationPrice { get; set; }
        public long PrizePrice { get; set; }

        [Column("PrizeTransactionId")]
        public long PrizeTransactionID { get; set; }
        public EarlyCheckoutEnum EarlyCheckoutStatus { get; set; }
        public DateTime? CancelDate { get; set; }
        public ReserveStatus CancelState { get; set; }
        public string CancelReason { get; set; }
        public string CancelDiscussion { get; set; }
        public int HostCallState { get; set; }
        public int GuestCallState { get; set; }
        public string SupportInfo { get; set; }

        [Column("ShouldFollowUp")]
        public bool shouldFollow { get; set; }
        public bool DisableAutoCancel { get; set; }

        [Column("VisitResidenceByGuest")]
        public bool AccVisitedByGuest { get; set; }
        public bool Archive { get; set; }
        public bool RatingShownToGuest { get; set; }
        public bool PaymentGTAGRegistered { get; set; }

        [ForeignKey(nameof(AdvertiseID))]
        public virtual Advertise Advertise { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User GuestUser { get; set; }

        [ForeignKey(nameof(HostUserID))]
        public virtual User HostUser { get; set; }

        [JsonIgnore]
        public virtual ICollection<ReservePayment> ReservePayments { get; set; }

        [JsonIgnore]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }

        [JsonIgnore]
        public virtual ICollection<PrizeCreditTransaction> PrizeCreditTransactions { get; set; }

        [JsonIgnore]
        public virtual ICollection<Chat> Chats { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }
        #endregion

        public int InitialPriority;
        public int Priority;

        [NotMapped]
        public long TotalPayablePrice
        {
            get
            {
                return TotalPrice - CouponPrice - PrizePrice;
            }
        }

        public static string[] CancelReasons = { };

        #region Enums
        public enum StatusStringType 
        { 
            Guest = 0, 
            Host = 1, 
            Site = 2 
        }

        public enum ReserveStatus
        {
            Default = 0,
            WaitForResponse = 1,
            WaitForReserve = 2,
            Rejected = 4,
            Reserved = 5,
            CashPay = 6,
            Started = 7,
            Completed = 8,
            CancelRequestByGuest = 9,
            CanceledByGuest = 10,
            CanceledBySystem = 11,
            CanceledByHost = 12,
            CancelRequestByHost = 13,
            Deleted = 3
        }

        public enum HostResponseEnum
        {
            None = 0,
            Accepted = 1,
            Rejected = 2,
            RejectedPrice = 3,
            RejectedHomeFull = 4,
            NoInternet = 5,
        }

        public enum ReserveCategory
        {
            WaitForHostResponse = 0,
            WaitForGuestPayment = 1,
            Reserved = 2,
            Finished = 3,
            Unsuccessful = 4
        }

        public enum CallState
        {
            NotCalled = 0,
            Called = 1,
            Answered = 2
        }

        public enum ReserveManagerSelectType
        {
            All = 0,
            Guest = 1,
            Host = 2
        }

        public enum GuestPayResult
        {
            AlreadyPaid = 0,
            ReadyToPay = 1,
            UnhandledPaymentType = 2,
            IncorrectPaymentType = 3,
            NotEnoughCredit = 4,
            Paid = 5,
        }

        public enum EarlyCheckoutEnum
        {
            Unset = 0,
            RequestedByHost = 1,
            ConfirmedByGuest = 2
        }

        public enum ReserveCancelType
        {
            Unset = 0,
            CancelByGuestForGuestProblem = 1,
            CancelByGuestForHostProblem = 2,
            CancelByHostForHostProblem = 3,
            CancelByHostForGuestProblem = 4
        }

        public enum ReserveCancelReasons
        {
            Unset = 0,

            Guest_Guest_TripCancellation = 1,
            Guest_Guest_NotHavingEvidence = 2,
            Guest_Guest_IncorrectNumberOfGuests = 3,
            Guest_Guest_ChangeTripDate = 4,

            Guest_Host_IncorrectResidenceInfo = 51,
            Guest_Host_IncorrectHost = 52,
            Guest_Host_DirtyResidence = 53,

            Host_Host_LowPrice = 101,
            Host_Host_ResidenceFull = 102,
            Host_Host_ResidenceRebuilding = 103,

            Host_Guest_NotHavingEvidence = 151,
            Host_Guest_IncorrectNumberOfGuest = 152,
            Host_Guest_IncorrectGuest = 153,
        }

        #endregion

        #region Functions
        public static int GetReserveInitialPriorityGuest(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.Started:
                case ReserveStatus.Reserved:
                case ReserveStatus.WaitForReserve:
                case ReserveStatus.CashPay:
                case ReserveStatus.WaitForResponse:
                case ReserveStatus.CancelRequestByGuest:
                case ReserveStatus.CancelRequestByHost:
                case ReserveStatus.Completed:
                    return 1;
                case ReserveStatus.Rejected:
                case ReserveStatus.CanceledByHost:
                case ReserveStatus.CanceledByGuest:
                case ReserveStatus.CanceledBySystem:
                case ReserveStatus.Deleted:
                    return 2;
                default:
                    return 3;
            }
        }

        public static int GetReservePriorityHost(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.CashPay:
                    return 1;
                case ReserveStatus.WaitForResponse:
                    return 2;
                case ReserveStatus.Reserved:
                    return 3;
                case ReserveStatus.WaitForReserve:
                    return 4;
                case ReserveStatus.CancelRequestByGuest:
                    return 5;
                case ReserveStatus.CancelRequestByHost:
                    return 6;
                case ReserveStatus.Started:
                    return 7;
                case ReserveStatus.CanceledByGuest:
                    return 8;
                case ReserveStatus.Completed:
                    return 9;
                case ReserveStatus.Rejected:
                    return 10;
                case ReserveStatus.CanceledByHost:
                    return 11;
                case ReserveStatus.CanceledBySystem:
                    return 12;
                case ReserveStatus.Deleted:
                    return 13;
                default:
                    return 14;
            }
        }

        public static int GetReserveInitialPriorityHost(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.CashPay:
                case ReserveStatus.WaitForResponse:
                case ReserveStatus.Reserved:
                case ReserveStatus.WaitForReserve:
                case ReserveStatus.CancelRequestByGuest:
                case ReserveStatus.CancelRequestByHost:
                case ReserveStatus.Started:
                case ReserveStatus.CanceledByGuest:
                    return 1;
                case ReserveStatus.Completed:
                case ReserveStatus.Rejected:
                case ReserveStatus.Deleted:
                case ReserveStatus.CanceledByHost:
                case ReserveStatus.CanceledBySystem:
                    return 2;
                default:
                    return 3;
            }
        }

        public static int GetReservePriorityGuest(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.Started:
                    return 1;
                case ReserveStatus.Reserved:
                    return 2;
                case ReserveStatus.WaitForReserve:
                    return 3;
                case ReserveStatus.CashPay:
                    return 4;
                case ReserveStatus.WaitForResponse:
                    return 5;
                case ReserveStatus.CancelRequestByHost:
                    return 6;
                case ReserveStatus.CancelRequestByGuest:
                    return 7;
                case ReserveStatus.Completed:
                    return 8;
                case ReserveStatus.Rejected:
                    return 9;
                case ReserveStatus.CanceledByGuest:
                    return 10;
                case ReserveStatus.CanceledByHost:
                    return 11;
                case ReserveStatus.CanceledBySystem:
                    return 12;
                case ReserveStatus.Deleted:
                    return 13;
                default:
                    return 14;
            }
        }

        public static int GetUnseccessfulReservePriority(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.CanceledBySystem:
                    return 1;
                case ReserveStatus.CanceledByHost:
                case ReserveStatus.CancelRequestByGuest:
                    return 2;
                case ReserveStatus.Rejected:
                    return 3;
                default:
                    return 4;
            }
        }

        public static bool StatusIsReserving(ReserveStatus status)
        {
            switch (status)
            {
                case ReserveStatus.Reserved:
                case ReserveStatus.CashPay:
                case ReserveStatus.Started:
                case ReserveStatus.Completed:
                case ReserveStatus.CancelRequestByGuest:
                case ReserveStatus.CancelRequestByHost:
                    return true;
                default:
                    return false;
            }
        }

        public static bool StatusIsCanceled(ReserveStatus status)
        {
            switch (status)
            {
                case ReserveStatus.CanceledByGuest:
                case ReserveStatus.CanceledByHost:
                case ReserveStatus.CanceledBySystem:
                    return true;
                default:
                    return false;
            }
        }

        public static bool StatusIsCanceling(ReserveStatus status)
        {
            switch (status)
            {
                case ReserveStatus.CancelRequestByHost:
                case ReserveStatus.CancelRequestByGuest:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CancelIsAvailableForGuest(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.WaitForResponse:
                case ReserveStatus.WaitForReserve:
                case ReserveStatus.CashPay:
                case ReserveStatus.Reserved:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CancelIsAvailableForHost(int status)
        {
            switch ((ReserveStatus)status)
            {
                case ReserveStatus.WaitForReserve:
                case ReserveStatus.Reserved:
                case ReserveStatus.CashPay:
                case ReserveStatus.Started:
                    return true;
                default:
                    return false;
            }
        }

        public bool CanReserveStarted(out DateTime canStartTime)
        {
            canStartTime = new DateTime(StartDate.Year, StartDate.Month,
                StartDate.Day, 8, 0, 0);
            return DateTime.Now > canStartTime;
        }

        public void AddSupportInfo(string text, User supporter)
        {
            var now = DateTime.Now;
            var date = DateTimeUtility.GregorianToPersianDate(now) + " " + now.ToString("HH:mm");
            text = text.Replace(",", "");
            date = date.Replace(",", "/");
            var supporter_name = !string.IsNullOrEmpty(supporter.FullName) ?
                supporter.FullName : supporter.Id.ToString();
            var info = string.Format("{0} - {1} : {2}", date, supporter_name, text);
            if (string.IsNullOrEmpty(SupportInfo))
            {
                SupportInfo = info;
            }
            else
            {
                SupportInfo += "," + info;
            }
        }

        public string[] GetSupportInfoList()
        {
            var output = new List<string>();
            if (!string.IsNullOrEmpty(SupportInfo))
            {
                return SupportInfo.Split(',');
            }
            else
            {
                return new string[0];
            }
        }

        public void AddCancelDiscussion(string text, User user)
        {
            var now = DateTime.Now;
            var date = DateTimeUtility.GregorianToPersianDate(now) + " " + now.ToString("HH:mm");
            text = text.Replace(",", "");
            date = date.Replace(",", "/");
            var user_name = !string.IsNullOrEmpty(user.FullName) ?
                user.FullName : user.Id.ToString();
            var discussion = string.Format("{0} - {1} : {2}", date, user_name, text);
            if (string.IsNullOrEmpty(CancelDiscussion))
            {
                CancelDiscussion = discussion;
            }
            else
            {
                CancelDiscussion += "," + discussion;
            }
        }

        public string[] GetCancelDiscussionList()
        {
            var output = new List<string>();
            if (!string.IsNullOrEmpty(CancelDiscussion))
            {
                return CancelDiscussion.Split(',');
            }
            else
            {
                return new string[0];
            }
        }

        public ReserveCategory? GetStateCategory()
        {
            switch (Status)
            {
                case ReserveStatus.WaitForResponse:
                    return ReserveCategory.WaitForHostResponse;
                case ReserveStatus.WaitForReserve:
                    return ReserveCategory.WaitForGuestPayment;
                case ReserveStatus.Reserved:
                case ReserveStatus.CashPay:
                case ReserveStatus.Started:
                case ReserveStatus.CancelRequestByGuest:
                case ReserveStatus.CancelRequestByHost:
                    return ReserveCategory.Reserved;
                case ReserveStatus.Completed:
                    return ReserveCategory.Finished;
                case ReserveStatus.Rejected:
                case ReserveStatus.CanceledByGuest:
                case ReserveStatus.CanceledBySystem:
                case ReserveStatus.CanceledByHost:
                    return ReserveCategory.Unsuccessful;
                default:
                    return null;
            }
        }

        public static IList<ReserveStatus> GetHostCategoryStates(Reserve.ReserveCategory category)
        {
            switch (category)
            {
                case Reserve.ReserveCategory.WaitForHostResponse:
                    return new List<ReserveStatus>() { 
                        ReserveStatus.WaitForResponse
                    };
                case Reserve.ReserveCategory.WaitForGuestPayment:
                    return new List<ReserveStatus>() {
                        ReserveStatus.WaitForReserve
                    };
                case Reserve.ReserveCategory.Reserved:
                    return new List<ReserveStatus>() {
                        ReserveStatus.Reserved,
                        ReserveStatus.Started,
                        ReserveStatus.CashPay,
                        ReserveStatus.CancelRequestByGuest,
                        ReserveStatus.CancelRequestByHost
                    };
                case Reserve.ReserveCategory.Finished:
                    return new List<ReserveStatus>() {
                        ReserveStatus.Completed
                    };
                case Reserve.ReserveCategory.Unsuccessful:
                    return new List<ReserveStatus>() {
                        ReserveStatus.Rejected,
                        ReserveStatus.CanceledBySystem,
                        ReserveStatus.CanceledByHost,
                        ReserveStatus.CanceledByGuest
                    };
                default:
                    return null;
            }
        }

        public static IList<ReserveStatus> GetGuestCategoryStates(Reserve.ReserveCategory category)
        {
            var statusList = new List<ReserveStatus>();
            switch (category)
            {
                case Reserve.ReserveCategory.WaitForHostResponse:
                case Reserve.ReserveCategory.WaitForGuestPayment:
                case Reserve.ReserveCategory.Reserved:
                    return new List<ReserveStatus>() {
                        ReserveStatus.WaitForResponse,
                        ReserveStatus.WaitForReserve,
                        ReserveStatus.Reserved,
                        ReserveStatus.Started,
                        ReserveStatus.CashPay,
                        ReserveStatus.CancelRequestByGuest,
                        ReserveStatus.CancelRequestByHost};
                case Reserve.ReserveCategory.Finished:
                    return new List<ReserveStatus>() { 
                        ReserveStatus.Completed 
                    };
                case Reserve.ReserveCategory.Unsuccessful:
                    return new List<ReserveStatus>() {
                        ReserveStatus.Rejected,
                        ReserveStatus.CanceledByHost,
                        ReserveStatus.CanceledByGuest,
                        ReserveStatus.CanceledBySystem
                    };
                default:
                    return null;
            }
        }

        public static IList<ReserveCancelReasons> GetReserveCancelReasonsByUserType(User.UserGeneralTypeEnum userType)
        {
            var reasonsArray = (Reserve.ReserveCancelReasons[])Enum.GetValues(typeof(Reserve.ReserveCancelReasons));
            return reasonsArray.Where(x=> userType == User.UserGeneralTypeEnum.Guest ? (int)x > 0 &&(int)x <= 100 : (int)x > 100).ToList();
        }

        public int GetPaymentTriesCount(out string lastTryDateStr)
        {
            var payments = Payments.Where(w => w.Status == Payment.PaymentStatus.NotPaid && w.Type == Payment.PaymentType.Income);
            if (payments.Any())
            {
                var paymentsList = payments.OrderByDescending(x => x.CreateDate).ToList();
                var lastDate = paymentsList.Last().CreateDate;
                lastTryDateStr = DateTimeUtility.GregorianToPersianDate(lastDate).Remove(0, 2);
                lastTryDateStr += (" " + lastDate.ToString("HH:mm"));
                return paymentsList.Count;
            }
            else
            {
                lastTryDateStr = "";
                return 0;
            }
        }

        public long GetReservePaymentPrice(ReservePaymentType type, out DateTime date,
             out long transactionId, int targetUserID = 0)
        {
            var payments = ReservePayments.Where(x => x.PaymentType == (int)type);
            if (targetUserID > 0)
            {
                payments = payments.Where(x => x.UserID == targetUserID);
            }
            var any_payments = payments.Any();
            if (any_payments)
            {
                var payment = payments.AsEnumerable().Last();
                date = payment.CreateDate;
                transactionId = payment.TransactionID;
            }
            else
            {
                date = DateTime.MinValue;
                transactionId = 0;
            }
            return any_payments ? payments.Sum(x => x.Price) : 0;
        }

        public long GetGuestPaidAmount()
        {
            var guestPaidAmount = ReservePayments.Where(
                        x => x.PaymentType == (int)ReservePaymentType.GuestDeposite ||
                        x.PaymentType == (int)ReservePaymentType.GuestClearing).Sum(x=>x.Price);
            var siteRefundToGuestAmount = ReservePayments.Where(
                x => x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest).Sum(x => x.Price);
            return guestPaidAmount - siteRefundToGuestAmount;
        }

        public IList<ReserveSupport> GetRelatedSupports()
        {
            var supports = GuestUser.ReserveSupportsAsGuest.AsQueryable();
            var ids = new List<int>();
            foreach (var support in supports)
            {
                if (support.JourneyStartDate == StartDate)
                {
                    ids.Add(support.Id);
                    continue;
                }
                if (support.LastSupporterActionDate == null)
                    continue;
                if (Math.Abs(((DateTime)support.LastSupporterActionDate -
                   CreateDate).TotalMinutes) < 180)
                {
                    ids.Add(support.Id);
                }
            }
            supports = supports.Where(x => ids.Contains(x.Id));
            return supports.ToList();
        }

        public bool HasFailureExpenditurePayment()
        {
            return Payments.Any(x => x.Type == Payment.PaymentType.Expenditure && x.Status == Payment.PaymentStatus.NotPaid);
        }

        public Payment GetFailureExpenditurePayment()
        {
            return Payments.FirstOrDefault(x => x.Type == Payment.PaymentType.Expenditure && x.Status == Payment.PaymentStatus.NotPaid);
        }

        public int ChatCount
        {
            get
            {
                return Chats == null ? 0 : Chats.Count();
            }
        }

        public int ChatCountUnreadBySupport
        {
            get
            {
                return Chats == null ? 0 : Chats.Count(c =>
                   c.SupportReadStatus == (int)Chat.ReadStatusEnum.NotRead);
            }
        }
        #endregion
    }
}
