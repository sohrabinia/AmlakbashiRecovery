using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class AdvertiseReport : Entity<int>
    {
        /// <summary>
        /// برای ثبت گزارش تخلف در محتوای آگهی
        /// فقط در اپلیکیشن استفاده شده است
        /// </summary>
        public long AdvertiseID { get; set; }
        public ReportReason Reason { get; set; }
        public string ReasonString { get; set; }

        [JsonIgnore]
        [ForeignKey("AdvertiseID")]
        public virtual Advertise Advertise { get; set; }

        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();
            if (Reason == ReportReason.Unset)
            {
                errors.Add("لطفا دلیل شکایت را وارد کنید");
            }
            if (Reason == ReportReason.Other &&
                string.IsNullOrEmpty(ReasonString))
            {
                errors.Add("لطفا متن دلیل شکایت را بنویسید");
            }
            return !errors.Any();
        }

        public enum ReportReason
        {
            Unset = 0,
            Other = 1,
            ReligiousPolitical = 2,
            NastyPhoto = 3,
            Offensive = 4
        }

        public static string GetReasonString(ReportReason reason)
        {
            switch (reason)
            {
                case ReportReason.Other:
                    return "موارد دیگر";
                case ReportReason.ReligiousPolitical:
                    return "محتوای نامناسب سیاسی یا مذهبی";
                case ReportReason.NastyPhoto:
                    return "تصاویر ناپسند";
                case ReportReason.Offensive:
                    return "محتوای توهین آمیز یا نفرت انگیز";
                default:
                    return "";
            }
        }
    }
}