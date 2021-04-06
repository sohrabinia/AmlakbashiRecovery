using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using static Amlakbashi.Core.Entities.User;

namespace Amlakbashi.Data.Identity
{
    public class AppUser : IdentityUser
    {
        public string Code { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? SendVerification { get; set; }
        public UserState State { get; set; }
    }
}
