using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.UserDTOs
{
    [Serializable]
    public class UserDTO
    {
        public int id { get; set; }
        public string mainMobile { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string mobile1 { get; set; }
        public string mobile2 { get; set; }
        public string tell { get; set; }
        public string thirdPersonTell { get; set; }
        public string email { get; set; }
        public string bankCardNumber { get; set; }
        public string shabaNumber { get; set; }
        public string bankFname { get; set; }
        public string bankLname { get; set; }
        public int loginPriority { get; set; }
        public int userGeneralType { get; set; }
        public int responseFrom { get; set; }
        public int responseTo { get; set; }
        public int accessType { get; set; }
        public int OwnerShip { get; set; }
        public int CancelInstantReserveLimit { get; set; }
        public string ContactPhone { get; set; }
        public int AmlakbashiScore { get; set; }
        public string Address { get; set; }

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

            dto.mobile1 = PhoneUtility.IsNumberForIran(
                user.GetPhoneNumber(User.PhoneType.OtherMobile1)) ?
                    user.GetLocalPhoneNumber(User.PhoneType.OtherMobile1) :
                user.GetCallablePhoneNumber(User.PhoneType.OtherMobile1);

            dto.mobile2 = PhoneUtility.IsNumberForIran(
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

            dto.loginPriority = user.LoginPriority; // TODO: delete this
            dto.userGeneralType = user.UserGeneralType;
            dto.responseFrom = user.ResponseFrom;
            dto.responseTo = user.ResponseTo;
            dto.accessType = user.AccessType;
            dto.OwnerShip = user.OwnerShip;
            dto.CancelInstantReserveLimit = user.CancelInstantReserveLimit;
            dto.ContactPhone = user.ContactPhone;
            dto.AmlakbashiScore = user.AmlakbashiScore;
            dto.Address = user.Address;
            return dto;
        }

        public bool Validate(out List<string> errors)
        {
            bool has_error = false;
            errors = new List<string>();
            if (userGeneralType > (int)Entities.User.UserGeneralTypeEnum.Guest)
            {
                if (!string.IsNullOrEmpty(this.bankCardNumber) &&
                    Regex.IsMatch(@"[a-zA-Zآ-ی]", this.bankCardNumber))
                {

                    errors.Add("شماره کارت بانکی نمیتواند شامل حروف باشد. لطفا اصلاح کنید");
                    has_error = true;
                }
                if (!string.IsNullOrEmpty(this.bankCardNumber) &&
                    this.bankCardNumber.Length != 16)
                {
                    errors.Add("شماره کارت بانکی باید 16 رقم باشد. لطفا اصلاح کنید ");
                    has_error = true;
                }
                if (!string.IsNullOrEmpty(this.shabaNumber))
                {
                    if (Regex.IsMatch(@"[a-zA-Zآ-ی]", this.shabaNumber))
                    {

                        errors.Add("شماره شبا نمیتواند شامل حروف باشد. لطفا اصلاح کنید");
                        has_error = true;
                    }
                    if (this.shabaNumber.Length != 24)
                    {
                        errors.Add("شماره شبا باید 24 رفم باشد. لطفا اصلاح کنید ");
                        has_error = true;
                    }
                }
                //TODO: handle this in app and then uncomment it
                //if (string.IsNullOrEmpty(this.bankFname))
                //{
                //    errors.Add("لطفا نام صاحب حساب را وارد کنید");
                //    has_error = true;
                //}
                //if (string.IsNullOrEmpty(this.bankLname))
                //{
                //    errors.Add("لطفا نام خانوادگی صاحب حساب را وارد کنید");
                //    has_error = true;
                //}
            }
            if (!PhoneUtility.ValidateCallableNumber(this.mobile1))
            {
                errors.Add("شماره موبایل اشتباه است .");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.mobile2) &&
                !PhoneUtility.ValidateCallableNumber(this.mobile2))
            {
                errors.Add("شماره موبایل 2 اشتباه است .");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.tell) &&
                !PhoneUtility.ValidateCallableNumber(this.tell))
            {
                errors.Add("شماره ثابت اشتباه است .");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.thirdPersonTell) &&
                !PhoneUtility.ValidateCallableNumber(this.thirdPersonTell))
            {
                errors.Add("شماره شخص ثالث اشتباه است .");
                has_error = true;
            }
            if (!string.IsNullOrEmpty(this.fname))
            {
                var isDigitPresent = StringUtility.ContainsNumber(this.fname);
                if (isDigitPresent)
                {
                    errors.Add("لطفا در قسمت نام فقط از حروف استفاده کنید .");
                    has_error = true;
                }
            }
            if (!string.IsNullOrEmpty(this.lname))
            {
                var isDigitPresent = StringUtility.ContainsNumber(this.lname);
                if (isDigitPresent)
                {
                    errors.Add("لطفا در قسمت نام خانوادگی فقط از حروف استفاده کنید .");
                    has_error = true;
                }
            }
            return !has_error;
        }
    }
}
