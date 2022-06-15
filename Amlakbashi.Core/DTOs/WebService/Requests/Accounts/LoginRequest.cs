using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Accounts
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "phoneNumber is required")]
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string referralCode { get; set; }
    }
}
