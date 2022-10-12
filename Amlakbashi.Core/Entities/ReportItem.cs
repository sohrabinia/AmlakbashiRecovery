using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// امتیاز ها و نظرسنجی آگهی
    /// ستاره های نظرسنجی پایان رزرو
    /// </summary>
    public class ReportItem : Entity<long>, ISoftDelete
    {
        [Column("ReportItemID")]
        public override long Id { get; set; }
        public int UserID { get; set; }
        public long AdvertiseID { get; set; }
        public int ReportID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public long LastModifyDatetick { get; set; }
        public int Score { get; set; }
        public int OperatorID { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("AdvertiseID")]
        public virtual Advertise Advertise { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public enum ScoreType
        {
            Cleaning = 1,
            HostBehaviour = 2,
            Location = 3,
            InfoCorrectness = 4,
            Safety = 5,
            PriceWorth = 6
        }

        public static string GetUserRatingTypeString(Comment.UserRatingType ratingType)
        {
            switch (ratingType)
            {
                case Comment.UserRatingType.Cleaning:
                    return "پاکیزگی اقامتگاه";
                case Comment.UserRatingType.HostBehaviour:
                    return "برخورد میزبان";
                case Comment.UserRatingType.Location:
                    return "موقعیت اقامتگاه";
                case Comment.UserRatingType.InfoCorrectness:
                    return "صحت مطالب";
                case Comment.UserRatingType.Safety:
                    return "امنیت اقامتگاه";
                case Comment.UserRatingType.PriceWorth:
                    return "ارزش نسبت به قیمت";
                default:
                    return "";
            }
        }
    }
}
