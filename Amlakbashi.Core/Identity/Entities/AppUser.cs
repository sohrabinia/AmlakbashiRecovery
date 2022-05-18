using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Amlakbashi.Core.Entities.User;

namespace Amlakbashi.Core.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        [Column("VerifyCode")]
        public string Code { get; set; }

        [Column("EmailVerifyCode")]
        public string EmailCode { get; set; }

        public DateTime? CreateDate { get; set; }

        [Column("LastSentVerifyCodeDate")]
        public DateTime? SendVerification { get; set; }

        [Column("Status")]
        public UserState State { get; set; }

        public bool IsForeigner { get; set; }

        [StringLength(1000)]
        public string Temp { get; set; }

        public bool IsVerifyCodeValid(string code)
        {
            return Code == code && (DateTime.Now - SendVerification) <= new TimeSpan(0, 0, 2, 0, 0);
        }
    }
}
