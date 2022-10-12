using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Accounts
{
    public class LoginVerifyRequest
    {
        [Required(ErrorMessage = "guid is required")]
        public string guid { get; set; }

        [Required(ErrorMessage = "verifyCode is required")]
        public string verifyCode { get; set; }
    }
}
