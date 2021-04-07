using Microsoft.AspNetCore.Identity;
using System;
using static Amlakbashi.Core.Entities.User;

namespace Amlakbashi.Core.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public string Code { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? SendVerification { get; set; }
        public UserState State { get; set; }
    }
}
