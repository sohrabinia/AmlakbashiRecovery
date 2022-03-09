using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.User
{
    public class UserProfileResponse
    {
        public int id { get; set; }
        public string mainMobile { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string mobile1 { get; set; }
        public string mobile2 { get; set; }
        public string tell { get; set; }
        public string thirdPersonTell { get; set; }
        public string email { get; set; }
    }
}
