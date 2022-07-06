using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amlakbashi.Core.DTOs.UserDTOs
{
    public class UserEditDTO
    {
        public int Id { get; set; }
        public string MainMobile { get; set; }
        public long? PhotoID { get; set; }
        public long UserScore { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Mobile { get; set; }
        public string Mobile2 { get; set; }
        public string Tell { get; set; }
        public string ThirdPersonTell { get; set; }
        public string Address { get; set; }
        public int AmlakbashiScore { get; set; }
        public int OwnerShip { get; set; }
        public bool ContactPhone { get; set; }
        public User.UserState UserState { get; set; }
        public long Credit { get; set; }
        public int CancelInstantReserveLimit { get; set; }
        public bool ForbiddenRegionsAccess { get; set; }

        public int InstantReserveCancelCount { get; set; }
        public string EmailAddress { get; set; }
        public string LastSendedSmsCode { get; set; }
        public string LastSendedEmailCode { get; set; }

        public bool HasError { get; set; } = false;
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public static UserEditDTO Generate(User user, AppUser identityUser)
        {
            return new UserEditDTO()
            {
                Id = user.Id,
                MainMobile = user.PhoneNumber,
                PhotoID = user.PhotoID,
                UserScore = user.UserScore,
                FName = user.FirstName,
                LName = user.LastName,
                Address = user.Description,
                Mobile = user.PhoneNumber2,
                Mobile2 = user.PhoneNumber3,
                Tell = user.LandlinePhoneNumber,
                ThirdPersonTell = user.ThirdPersonPhoneNumber,
                AmlakbashiScore = user.AmlakbashiScore,
                Credit = user.WalletAmount,
                CancelInstantReserveLimit = user.CancelInstantReserveLimit,
                InstantReserveCancelCount = user.Advertises.Sum(x => x.InstantReserveCancels),
                EmailAddress = identityUser.Email,
                LastSendedEmailCode = identityUser.EmailVerifyCode,
                LastSendedSmsCode = identityUser.VerifyCode,
                UserState = identityUser.Status,
                OwnerShip = user.OwnerShip,
                ContactPhone = string.IsNullOrEmpty(user.ContactPhone) == false && user.ContactPhone == "1",
                ForbiddenRegionsAccess = user.ForbiddenRegionsAccess
            };
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Mobile) == false && PhoneUtility.ValidateInternationalNumber(Mobile) == false)
            {
                ErrorMessages.Add("شماره موبایل اشتباه است");
                HasError = true;
            }
            if (string.IsNullOrEmpty(Mobile2) == false && PhoneUtility.ValidateInternationalNumber(Mobile2) == false)
            {
                ErrorMessages.Add("شماره موبایل 2 اشتباه است");
                HasError = true;
            }
            if (string.IsNullOrEmpty(Tell) == false && PhoneUtility.ValidateInternationalNumber(Tell) == false)
            {
                ErrorMessages.Add("شماره ثابت اشتباه است");
                HasError = true;
            }
            if (string.IsNullOrEmpty(ThirdPersonTell) == false && PhoneUtility.ValidateInternationalNumber(ThirdPersonTell) == false)
            {
                ErrorMessages.Add("شماره شخص ثالث اشتباه است");
                HasError = true;
            }
            if (string.IsNullOrEmpty(FName))
            {
                ErrorMessages.Add("لطفا نام کاربر را وارد کنید");
                HasError = true;
            }
            if (string.IsNullOrEmpty(LName))
            {
                ErrorMessages.Add("لطفا نام خانوادگی کاربر را وارد کنید");
                HasError = true;
            }
            return !HasError;
        }
    }
}
