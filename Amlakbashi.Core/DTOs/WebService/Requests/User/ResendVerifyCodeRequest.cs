using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class ResendVerifyCodeRequest
    {
        [Required(ErrorMessage = "guid is required")]
        public string guid { get; set; }
    }
}
