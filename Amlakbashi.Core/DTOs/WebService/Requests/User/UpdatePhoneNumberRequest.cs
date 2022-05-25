using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.User
{
    public class UpdatePhoneNumberRequest
    {
        [Required]
        public string newPhoneNumber { get; set; }
    }
}
