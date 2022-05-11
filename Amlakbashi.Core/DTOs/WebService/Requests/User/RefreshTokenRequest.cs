using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "token is required")]
        public string token { get; set; }
        public Entities.User.UserGeneralTypeEnum? panel { get; set; } = null;
    }
}
