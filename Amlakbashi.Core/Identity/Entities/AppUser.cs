using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using static Amlakbashi.Core.Entities.User;

namespace Amlakbashi.Core.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public string VerifyCode { get; set; }
        public string EmailVerifyCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastSentVerifyCodeDate { get; set; }
        public UserState Status { get; set; }
        public bool IsForeigner { get; set; }

        [StringLength(1000)]
        public string Temp { get; set; }

        public bool IsVerifyCodeValid(string code)
        {
            return (IsForeigner ? EmailVerifyCode : VerifyCode) == code && (DateTime.Now - LastSentVerifyCodeDate) <= new TimeSpan(0, 0, 2, 0, 0);
        }
    }
}
