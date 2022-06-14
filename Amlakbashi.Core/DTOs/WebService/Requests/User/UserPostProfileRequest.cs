using Amlakbashi.Core.Common.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class UserPostProfileRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber2 { get; set; }
        public string phoneNumber3 { get; set; }
        public string landLinePhoneNumber { get; set; }
        public string thirdPersonPhoneNumber { get; set; }
        public string bankCardNumber { get; set; }
        public string bankCardOwnerFirstName { get; set; }
        public string bankCardOwnerLastName { get; set; }
        public string shebaNumber { get; set; }
        public Entities.User.NoticesPhoneNumberEnum noticesPhoneNumber { get; set; }

        [BindNever]
        public int id { get; set; }

        public bool IsValid(ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(bankCardNumber) == false &&
                BankUtility.ValidateBankCardNumber(bankCardNumber) == false)
            {
                modelState.AddModelError(nameof(bankCardNumber), "bankCardNumber is incorrect");
            }
            if (string.IsNullOrEmpty(shebaNumber) == false &&
                (Regex.IsMatch(@"[a-zA-Zآ-ی]", shebaNumber) || shebaNumber.Length != 24))
            {
                modelState.AddModelError(nameof(shebaNumber), "shebaNumber is incorrect");
            }
            if (string.IsNullOrEmpty(phoneNumber3) == false &&
                PhoneUtility.ValidatePhoneNumber(phoneNumber2) == false)
            {
                modelState.AddModelError(nameof(phoneNumber2), "phoneNumber2 is incorrect");
            }
            if (string.IsNullOrEmpty(phoneNumber3) == false &&
                PhoneUtility.ValidatePhoneNumber(phoneNumber3) == false)
            {
                modelState.AddModelError(nameof(phoneNumber3), "phoneNumber3 is incorrect");
            }
            if (string.IsNullOrEmpty(landLinePhoneNumber) == false &&
                PhoneUtility.ValidatePhoneNumber(landLinePhoneNumber) == false)
            {
                modelState.AddModelError(nameof(landLinePhoneNumber), "landLinePhoneNumber is incorrect");
            }
            if (string.IsNullOrEmpty(this.thirdPersonPhoneNumber) == false &&
                PhoneUtility.ValidatePhoneNumber(thirdPersonPhoneNumber) == false)
            {
                modelState.AddModelError(nameof(thirdPersonPhoneNumber), "thirdPersonPhoneNumber is incorrect");
            }
            if (string.IsNullOrEmpty(firstName) || StringUtility.ContainsNumber(firstName))
            {
                modelState.AddModelError(nameof(firstName), "firstName is incorrect");
            }
            if (string.IsNullOrEmpty(lastName) || StringUtility.ContainsNumber(lastName))
            {
                modelState.AddModelError(nameof(lastName), "lastName is incorrect");
            }
            if (string.IsNullOrEmpty(bankCardOwnerFirstName) || StringUtility.ContainsNumber(bankCardOwnerFirstName))
            {
                modelState.AddModelError(nameof(bankCardOwnerFirstName), "bankCardOwnerFirstName is incorrect");
            }
            if (string.IsNullOrEmpty(bankCardOwnerLastName) || StringUtility.ContainsNumber(bankCardOwnerLastName))
            {
                modelState.AddModelError(nameof(bankCardOwnerLastName), "bankCardOwnerLastName is incorrect");
            }
            if (Enum.IsDefined(typeof(Entities.User.NoticesPhoneNumberEnum), noticesPhoneNumber) == false ||
                (noticesPhoneNumber == Entities.User.NoticesPhoneNumberEnum.PhoneNumber2 && string.IsNullOrEmpty(phoneNumber2)) ||
                (noticesPhoneNumber == Entities.User.NoticesPhoneNumberEnum.PhoneNumber3 && string.IsNullOrEmpty(phoneNumber3)))
            {
                modelState.AddModelError(nameof(noticesPhoneNumber), "noticesphonenumber is incorrect");
            }
            return modelState.IsValid;
        }
    }
}
