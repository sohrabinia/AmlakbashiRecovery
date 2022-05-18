using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Amlakbashi.Core.DTOs.UserDTOs
{
    [Serializable]
    public class UserDTO
    {
        public int id { get; set; }
        public string mainMobile { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string mobile2 { get; set; }
        public string mobile3 { get; set; }
        public string tell { get; set; }
        public string thirdPersonTell { get; set; }
        public string email { get; set; }
        public string bankCardNumber { get; set; }
        public string shabaNumber { get; set; }
        public string bankFname { get; set; }
        public string bankLname { get; set; }
        public int userGeneralType { get; set; }
        public int CancelInstantReserveLimit { get; set; }
        public bool hasPassword { get; set; }

        public static UserDTO Generate(User user, AppUser identityUser)
        {
            UserDTO dto = new UserDTO();
            dto.id = user.Id;
            dto.fname = user.FName;
            dto.lname = user.LName;
            dto.email = identityUser.Email;

            dto.mainMobile = PhoneUtility.IsNumberForIran(
                user.GetPhoneNumber(User.PhoneType.MainMobile)) ?
                    user.GetLocalPhoneNumber(User.PhoneType.MainMobile) :
                user.GetCallablePhoneNumber(User.PhoneType.MainMobile);

            dto.mobile2 = PhoneUtility.IsNumberForIran(
                user.GetPhoneNumber(User.PhoneType.OtherMobile1)) ?
                    user.GetLocalPhoneNumber(User.PhoneType.OtherMobile1) :
                user.GetCallablePhoneNumber(User.PhoneType.OtherMobile1);

            dto.mobile3 = PhoneUtility.IsNumberForIran(
                user.GetPhoneNumber(User.PhoneType.OtherMobile2)) ?
                    user.GetLocalPhoneNumber(User.PhoneType.OtherMobile2) :
                user.GetCallablePhoneNumber(User.PhoneType.OtherMobile2);

            dto.tell = PhoneUtility.IsNumberForIran(
                user.GetPhoneNumber(User.PhoneType.LandLine)) ?
                    user.GetLocalPhoneNumber(User.PhoneType.LandLine) :
                user.GetCallablePhoneNumber(User.PhoneType.LandLine);

            dto.thirdPersonTell = PhoneUtility.IsNumberForIran(
                user.GetPhoneNumber(User.PhoneType.ThirdPerson)) ?
                    user.GetLocalPhoneNumber(User.PhoneType.ThirdPerson) :
                user.GetCallablePhoneNumber(User.PhoneType.ThirdPerson);

            dto.userGeneralType = user.UserGeneralType;
            dto.CancelInstantReserveLimit = user.CancelInstantReserveLimit;
            dto.hasPassword = identityUser.PasswordHash != null;
            return dto;
        }

        public bool Validate(out List<string> errors)
        {
            bool has_error = false;
            errors = new List<string>();
            if (userGeneralType == (int)User.UserGeneralTypeEnum.Host)
            {
                if (BankUtility.ValidateBankCardNumber(bankCardNumber) == false)
                {
                    errors.Add("شماره کارت وارد شده صحیح نمی باشد");
                    has_error = true;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(bankCardNumber) == false && BankUtility.ValidateBankCardNumber(bankCardNumber) == false)
                {
                    errors.Add("شماره کارت وارد شده صحیح نمی باشد");
                    has_error = true;
                }
            }
            if (string.IsNullOrEmpty(this.shabaNumber) == false)
            {
                if (Regex.IsMatch(@"[a-zA-Zآ-ی]", this.shabaNumber))
                {

                    errors.Add("شماره شبا نمی تواند شامل حروف باشد");
                    has_error = true;
                }
                if (this.shabaNumber.Length != 24)
                {
                    errors.Add("شماره شبا باید 24 رقم باشد ");
                    has_error = true;
                }
            }
            if (!string.IsNullOrEmpty(this.mobile2) &&
                !PhoneUtility.ValidatePhoneNumber(this.mobile2))
            {
                errors.Add("شماره موبایل 2 اشتباه است");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.mobile3) &&
                !PhoneUtility.ValidatePhoneNumber(this.mobile3))
            {
                errors.Add("شماره موبایل 3 اشتباه است");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.tell) &&
                !PhoneUtility.ValidatePhoneNumber(this.tell))
            {
                errors.Add("شماره ثابت اشتباه است");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.thirdPersonTell) &&
                !PhoneUtility.ValidatePhoneNumber(this.thirdPersonTell))
            {
                errors.Add("شماره شخص ثالث اشتباه است");
                has_error = true;
            }
            var phoneNumbersList = new List<string>()
            {
                mainMobile, mobile2, mobile3, tell, thirdPersonTell
            };
            if (PhoneUtility.HasSamePhoneNumber(phoneNumbersList))
            {
                errors.Add("شماره تلفن های وارد شده یکسان هستند");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.fname))
            {
                var isDigitPresent = StringUtility.ContainsNumber(this.fname);
                if (isDigitPresent)
                {
                    errors.Add("لطفا در قسمت نام فقط از حروف استفاده کنید");
                    has_error = true;
                }
            }
            if (!string.IsNullOrEmpty(this.lname))
            {
                var isDigitPresent = StringUtility.ContainsNumber(this.lname);
                if (isDigitPresent)
                {
                    errors.Add("لطفا در قسمت نام خانوادگی فقط از حروف استفاده کنید");
                    has_error = true;
                }
            }
            return !has_error;
        }
    }
}
