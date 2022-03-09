using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class LoginVerifyRequest
    {
        [Required(ErrorMessage = "guid needed")]
        public string guid { get; set; }
        [Required(ErrorMessage = "verifyCode needed")]
        public string verifyCode { get; set; }
    }
}
