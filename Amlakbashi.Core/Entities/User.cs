using Amlakbashi.Core.Common.Entity;
using Amlakbashi.Core.Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Amlakbashi.Core.Entities
{
    public class User : Entity<int>, ISoftDelete
    {
        [Column("UserID")]
        public override int Id { get; set; }
        public string MainMobile { get; set; }
        public int LoginPriority { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Tell { get; set; }
        public string ThirdPersonTell { get; set; }
        public string Mobile { get; set; }
        public string Mobile2 { get; set; }
        public string Address { get; set; }//Description
        public string ForgetCode { get; set; }
        public string AdminLoginCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public int State { get; set; }
        public string Code { get; set; }
        public DateTime? SendVerification { get; set; }
        public int ResponseFrom { get; set; }
        public int ResponseTo { get; set; }
        public long? PhotoID { get; set; }
        public string ContactPhone { get; set; }
        public int OwnerShip { get; set; }
        public int AmlakbashiScore { get; set; }
        public long UserScore { get; set; }
        public int PhotoStatus { get; set; }
        public long Credit { get; set; }
        public long PrizeCredit { get; set; }
        public int UserGeneralType { get; set; }
        public int AccessType { get; set; }
        public string NotificationToken { get; set; }
        public string AppNotificationToken { get; set; }
        public string FcmAppNotificationToken { get; set; }
        public long LastNotifPermitionTicks { get; set; }
        public int PresentorUserID { get; set; }
        public bool PresentorPrizeGiven { get; set; }
        public bool RecieveAppreciateDiscount { get; set; }
        public int CancelInstantReserveLimit { get; set; } = 3;
        public InstantReserveAccessEnum InstantReserveAccess { get; set; }
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        [ForeignKey("PhotoID")]
        public virtual File Photo { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserFavorite> Favorite { get; set; }

        [JsonIgnore]
        [InverseProperty("GuestUser")]
        public virtual ICollection<Reserve> Reserves { get; set; }

        [JsonIgnore]
        [InverseProperty("HostUser")]
        public virtual ICollection<Reserve> HostReserves { get; set; }

        [JsonIgnore]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }

        [JsonIgnore]
        public virtual ICollection<PrizeCreditTransaction> PrizeCreditTransactions { get; set; }

        [JsonIgnore]
        public virtual ICollection<Advertise> Advertises { get; set; }

        [JsonIgnore]
        public virtual ICollection<DiscountCoupon> DiscountCoupons { get; set; }

        [JsonIgnore]
        public virtual ICollection<BankCard> BankCards { get; set; }

        [JsonIgnore]
        public virtual ICollection<BlogPost> BlogPosts { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; }

        [JsonIgnore]
        public virtual ICollection<Chat> Chats { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }
        [JsonIgnore]
        [InverseProperty("Guest")]
        public virtual ICollection<ReserveSupport> ReserveSupportsAsGuest { get; set; }
        [JsonIgnore]
        [InverseProperty("Supporter")]
        public virtual ICollection<ReserveSupport> ReserveSupportsAsSupporter { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                return (!string.IsNullOrEmpty(FName) ? FName + " " : "") +
                    (!string.IsNullOrEmpty(LName) ? LName : "");
            }
        }

        public enum InstantReserveAccessEnum
        {
            None = 0,
            Verified = 1,
            Banned = 2,
            Requested = 3
        }
        public void SetLoginPriority(LoginPriorites priority)
        {
            LoginPriority = (int)priority;
        }
        public LoginPriorites GetLoginProperty()
        {
            return (LoginPriorites)LoginPriority;
        }
        public string GetUserName()
        {
            switch ((LoginPriorites)LoginPriority)
            {
                case LoginPriorites.Mobile:
                    return PhoneUtility.InternationalNumberToLocal(MainMobile);
                case LoginPriorites.Email:
                    return Email;
                default:
                    return null;
            }
        }
        public string GetPhoneNumber(PhoneType type)
        {
            switch (type)
            {
                case PhoneType.MainMobile:
                    return MainMobile;
                case PhoneType.LandLine:
                    return Tell;
                case PhoneType.OtherMobile1:
                    return Mobile;
                case PhoneType.OtherMobile2:
                    return Mobile2;
                case PhoneType.ThirdPerson:
                    return ThirdPersonTell;
                default:
                    return "";
            }
        }
        public string GetLocalPhoneNumber(PhoneType type)
        {
            return PhoneUtility.InternationalNumberToLocal(GetPhoneNumber(type));
        }
        public string GetCallablePhoneNumber(PhoneType type)
        {
            return PhoneUtility.InternationalNumberToCallable(GetPhoneNumber(type));
        }
        public void SetPhoneNumber(PhoneType type, string international_number)
        {
            switch (type)
            {
                case PhoneType.MainMobile:
                    MainMobile = international_number;
                    break;
                case PhoneType.LandLine:
                    Tell = international_number;
                    break;
                case PhoneType.OtherMobile1:
                    Mobile = international_number;
                    break;
                case PhoneType.OtherMobile2:
                    Mobile2 = international_number;
                    break;
                case PhoneType.ThirdPerson:
                    ThirdPersonTell = international_number;
                    break;
            }
        }
        public void SetLocalPhoneNumber(PhoneType type, string local_number,
            int country_code)
        {
            var international_number = PhoneUtility.LocalNumberToInternational(local_number, country_code);
            SetPhoneNumber(type, international_number);
        }
        public enum PhoneType { MainMobile, LandLine, OtherMobile1, OtherMobile2, ThirdPerson }
        public enum LoginPriorites { Mobile = 0, Email = 1 }

        public User ShallowCopy()
        {
            return (User)this.MemberwiseClone();
        }

        public enum UserState
        {
            Suspend = 0,
            Acticved = 1,
            InActived = 2,
            Deleted = 3,
        }

        public enum AccessTypeEnum
        {
            Full = 0,
            ReserveBanned = 1,
            LoginBanned = 2,
        }

        public enum UserPhotoState
        {
            not_set = 0,
            ready_publish = 1,
            publish = 2,
            not_verified = 3
        }

        public enum OwnerType
        {
            real_reservation = 0,
            reservation = 1,
            owner = 3,
            real_owner = 10
        }

        public enum UserGeneralTypeEnum
        {
            Guest = 0,
            Host = 1
        }

        public enum CreditTransactionType
        {
            Credit_Increase = 1, Credit_Decrease = 2,
            Credit_Inc_Then_Res
        }

        public enum CreditTransactionCause
        {
            Reserve = 1,
            SitePortion = 2,
            Charge = 3,
            Clearing = 4,
            Refund = 5,
            ContactAdvertise = 6,
            Other = 100
        }

        public enum UserFilterType
        {
            All = -1,
            Guest = 0,
            ActiveHost = 1,
            Host = 2,
            Staff = 3,
            InstantReserveRequest = 4,
            InstantReserveAllow = 5,
            PhotoChangeRequest = 6
        }

        public static string GetAccessTypeString(AccessTypeEnum accessType)
        {
            switch (accessType)
            {
                case AccessTypeEnum.Full:
                    return "دسترسی کامل";
                case AccessTypeEnum.ReserveBanned:
                    return "ممنوعیت درخواست رزرو";
                case AccessTypeEnum.LoginBanned:
                    return "ممنوعیت ورود به سایت";
                default:
                    return "";
            }
        }

        public static string GetUserGeneralTypeString(int type)
        {
            switch ((UserGeneralTypeEnum)type)
            {
                case UserGeneralTypeEnum.Guest:
                    return "مهمان";
                case UserGeneralTypeEnum.Host:
                    return "میزبان";
                default:
                    return "";
            }
        }

        public static string GetUserGeneralTypeColor(int type)
        {
            switch ((UserGeneralTypeEnum)type)
            {
                case UserGeneralTypeEnum.Guest:
                    return "#d314ff";
                case UserGeneralTypeEnum.Host:
                    return "#015f96";
                default:
                    return "#242424";
            }
        }

        public static string GetStateTitle(int state)
        {
            switch ((UserState)state)
            {
                case UserState.Suspend:
                    return "معلق";
                case UserState.Acticved:
                    return "فعال";
                case UserState.InActived:
                    return "غیرفعال";
                case UserState.Deleted:
                    return "پاک شده";
                default:
                    return "";
            }
        }

        public static string GetCreditTransactionCauseString(int transaction, string transactionCauseString = "")
        {
            switch ((CreditTransactionCause)transaction)
            {
                case CreditTransactionCause.Reserve:
                    return "رزرو اقامتگاه";
                case CreditTransactionCause.SitePortion:
                    return "پرداخت درصد املاک باشی";
                case CreditTransactionCause.Charge:
                    return "شارژ کیف پول";
                case CreditTransactionCause.Clearing:
                    return "تسویه با میزبان";
                case CreditTransactionCause.Refund:
                    return "عودت به مهمان";
                case CreditTransactionCause.ContactAdvertise:
                    return "نمایش تماس";
                case CreditTransactionCause.Other:
                    return transactionCauseString;
                default:
                    return "";
            }
        }

        public static string GetFullName(string fname, string lname)
        {
            var fullName = "";
            if (!string.IsNullOrEmpty(fname))
                fullName += fname + " ";
            if (!string.IsNullOrEmpty(lname))
                fullName += lname;
            return fullName;
        }

        public bool UserHasSimilarReserve(long advertiseId, DateTime startDate, DateTime endDate)
        {
            var reserves = Reserves.Where(x =>
                x.AdvertiseID == advertiseId &&
                x.Status != Reserve.ReserveStatus.Deleted &&
                x.Status != Reserve.ReserveStatus.Rejected &&
                x.Status != Reserve.ReserveStatus.CanceledBySystem &&
                x.Status != Reserve.ReserveStatus.CanceledByGuest &&
                x.Status != Reserve.ReserveStatus.CanceledByHost);
            foreach (var item in reserves)
            {
                if (DateTimeUtility.DateRangesHaveOverlap(item.StartDate, item.EndDate,
                    startDate, endDate))
                {
                    return true;
                }
            }
            return false;
        }

        public enum SignInFirstStepStatus
        {
            Error = 0,
            MobileLogin = 1,
            EmailLogin = 2,
        }

        public enum SignInEmailStatus
        {
            Error = 0,
            Done = 1,
        }
    }
}
