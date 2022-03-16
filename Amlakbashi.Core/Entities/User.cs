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
        [Column("Id")]
        public override int Id { get; set; }

        [Column("FirstName")]
        public string FName { get; set; }

        [Column("LastName")]
        public string LName { get; set; }

        [Column("PhoneNumber")]
        public string MainMobile { get; set; }

        [Column("PhoneNumber2")]
        public string Mobile { get; set; }

        [Column("PhoneNumber3")]
        public string Mobile2 { get; set; }

        [Column("LandlinePhoneNumber")]
        public string Tell { get; set; }

        [Column("ThirdPersonPhoneNumber")]
        public string ThirdPersonTell { get; set; }

        [Column("Type")]
        public int UserGeneralType { get; set; }
        public string NotificationToken { get; set; }
        public string AppNotificationToken { get; set; }
        public string FcmAppNotificationToken { get; set; }
        public long LastNotifPermitionTicks { get; set; }
        public int OwnerShip { get; set; }
        public int AmlakbashiScore { get; set; }
        public long UserScore { get; set; }
        public int CancelInstantReserveLimit { get; set; } = 3;
        public InstantReserveAccessEnum InstantReserveAccess { get; set; }
        public long? PhotoID { get; set; }
        public int PhotoStatus { get; set; }

        [Column("Description")]
        public string Address { get; set; } //Description

        [Column("WalletAmount")]
        public long Credit { get; set; }

        [Column("GiftWalletAmount")]
        public long PrizeCredit { get; set; }
        public int PresentorUserID { get; set; }
        public bool PresentorPrizeGiven { get; set; }
        public bool RecieveAppreciateDiscount { get; set; }
        public string ContactPhone { get; set; }
        public bool ForbiddenRegionsAccess { get; set; }
        public bool IsDeleted { get; set; }

        //public string Email { get; set; }
        //public int State { get; set; }
        //public int LoginPriority { get; set; }
        //public string ForgetCode { get; set; }
        //public string Code { get; set; }
        //public DateTime? CreateDate { get; set; }
        //public DateTime? SendVerification { get; set; }
        //public int AccessType { get; set; }
        //public int ResponseFrom { get; set; }
        //public int ResponseTo { get; set; }
        //public string AdminLoginCode { get; set; }

        [JsonIgnore]
        [InverseProperty("HostUser")]
        public virtual ICollection<Reserve> HostReserves { get; set; }

        [JsonIgnore]
        public virtual ICollection<Advertise> Advertises { get; set; }

        [JsonIgnore]
        [ForeignKey("PhotoID")]
        public virtual File Photo { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserFavorite> Favorite { get; set; }

        [JsonIgnore]
        public virtual ICollection<CreditTransaction> CreditTransactions { get; set; }

        [JsonIgnore]
        public virtual ICollection<PrizeCreditTransaction> PrizeCreditTransactions { get; set; }

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
        
        [JsonIgnore]
        [InverseProperty("GuestUser")]
        public virtual ICollection<Reserve> Reserves { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return (!string.IsNullOrEmpty(FName) ? FName + " " : "") +
                    (!string.IsNullOrEmpty(LName) ? LName : "");
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

        public User ShallowCopy()
        {
            return (User)this.MemberwiseClone();
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
                    return "حذف شده";
                case UserState.ReserveBanned:
                    return "ممنوعیت رزرو";
                default:
                    return "";
            }
        }

        public static List<UserState> GetAdminStateEnum()
        {
            var array = (UserState[])Enum.GetValues(typeof(UserState));
            var list = array.ToList();
            list.Remove(UserState.Deleted);
            list.Remove(UserState.InActived);
            return list;
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

        public enum SignInFirstStepResult
        {
            Error = 0,
            MobileConfirm = 1,
            EnterPassword = 2
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

        public enum UserState
        {
            Suspend = 0,
            Acticved = 1,
            InActived = 2,
            ReserveBanned = 3,
            Deleted = 4
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

        public enum PhoneType 
        { 
            MainMobile, 
            LandLine, 
            OtherMobile1, 
            OtherMobile2, 
            ThirdPerson 
        }

        public enum InstantReserveAccessEnum
        {
            None = 0,
            Verified = 1,
            Banned = 2,
            Requested = 3
        }
    }
}
