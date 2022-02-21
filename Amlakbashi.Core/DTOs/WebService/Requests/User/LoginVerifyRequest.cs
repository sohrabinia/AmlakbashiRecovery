using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class LoginVerifyRequest
    {
        [Required]
        public string guid { get; set; }
        public string verifyCode { get; set; }
    }
}
