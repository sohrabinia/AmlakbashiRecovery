using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class VerifyPhoneNumberRequest
    {
        [Required]
        public string verifyCode { get; set; }
    }
}
