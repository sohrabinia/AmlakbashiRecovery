using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.User
{
    public class UserGetProfileResponse
    {
        public string phoneNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber2 { get; set; }
        public string phoneNumber3 { get; set; }
        public string landLinePhoneNumber { get; set; }
        public string thirdPersonPhoneNumber { get; set; }
        public string email { get; set; }
        public string bankCardNumber { get; set; }
        public string bankCardOwnerFirstName { get; set; }
        public string bankCardOwnerLastName { get; set; }
        public string shebaNumber { get; set; }
        public Entities.User.NoticesPhoneNumberEnum noticesPhoneNumber { get; set; }
        public string imageUrl { get; set; }
    }
}
