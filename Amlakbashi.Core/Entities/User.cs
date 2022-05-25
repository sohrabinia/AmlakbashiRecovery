using Amlakbashi.Core.Common.Entity;
using Amlakbashi.Core.Common.StaticData;
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
        public override int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
        public string PhoneNumber3 { get; set; }
        public string LandlinePhoneNumber { get; set; }
        public string ThirdPersonPhoneNumber { get; set; }
        public int Type { get; set; }
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
        public string Description { get; set; }
        public long WalletAmount { get; set; }
        public long GiftWalletAmount { get; set; }
        public int PresentorUserID { get; set; }
        public bool PresentorPrizeGiven { get; set; }
        public bool RecieveAppreciateDiscount { get; set; }
        public string ContactPhone { get; set; }
        public bool ForbiddenRegionsAccess { get; set; }
        public NoticesPhoneNumberEnum NoticesPhoneNumber { get; set; } = NoticesPhoneNumberEnum.PhoneNumber;
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        [ForeignKey("PhotoID")]
        public virtual File Photo { get; set; }

        [JsonIgnore]
        [InverseProperty("HostUser")]
        public virtual ICollection<Reserve> HostReserves { get; set; }

        [JsonIgnore]
        public virtual ICollection<Advertise> Advertises { get; set; }

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

        #region Functions

        [NotMapped]
        public string FullName
        {
            get
            {
                return (!string.IsNullOrEmpty(FirstName) ? FirstName + " " : "") +
                    (!string.IsNullOrEmpty(LastName) ? LastName : "");
            }
        }

        public IList<string> GetPhoneNumbersList(bool withMainPhoneNumber = false)
        {
            var list = new List<string>()
            {
                PhoneNumber2, PhoneNumber3, LandlinePhoneNumber, ThirdPersonPhoneNumber
            };
            if (withMainPhoneNumber)
            {
                list.Add(PhoneNumber);
            }
            return list;
        }

        public string GetPhoneNumber(PhoneType type)
        {
            switch (type)
            {
                case PhoneType.MainMobile:
                    return PhoneNumber;
                case PhoneType.LandLine:
                    return LandlinePhoneNumber;
                case PhoneType.OtherMobile1:
                    return PhoneNumber2;
                case PhoneType.OtherMobile2:
                    return PhoneNumber3;
                case PhoneType.ThirdPerson:
                    return ThirdPersonPhoneNumber;
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

        public string GetNormalizedNoticesPhoneNumber()
        {
            switch (NoticesPhoneNumber)
            {
                case NoticesPhoneNumberEnum.PhoneNumber:
                    return PhoneUtility.NormalizePhoneNumber(PhoneNumber);
                case NoticesPhoneNumberEnum.PhoneNumber2:
                    return PhoneUtility.NormalizePhoneNumber(PhoneNumber2);
                case NoticesPhoneNumberEnum.PhoneNumber3:
                    return PhoneUtility.NormalizePhoneNumber(PhoneNumber3);
                default:
                    return null;
            }
        }

        public string GetNoticesPhoneNumber()
        {
            switch (NoticesPhoneNumber)
            {
                case NoticesPhoneNumberEnum.PhoneNumber:
                    return PhoneNumber;
                case NoticesPhoneNumberEnum.PhoneNumber2:
                    return PhoneNumber2;
                case NoticesPhoneNumberEnum.PhoneNumber3:
                    return PhoneNumber3;
                default:
                    return null;
            }
        }

        public User ShallowCopy()
        {
            return (User)this.MemberwiseClone();
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

        public string GetCurrentUserImageApiUrl()
        {
            return PhotoID == null ? null : $"{GeneralData.WebsiteUrl}/api/file/user";
        }

        public string GetUserImageApiUrl()
        {
            return PhotoID == null ? null : $"{GeneralData.WebsiteUrl}/api/file/user/{Id}";
        }

        #endregion

        #region Static Functions

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

        #endregion

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

        public enum NoticesPhoneNumberEnum
        {
            PhoneNumber = 1,
            PhoneNumber2 = 2,
            PhoneNumber3 = 3
        }
    }
}
