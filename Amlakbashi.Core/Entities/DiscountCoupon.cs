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
    /// بن های تخفیف
    /// تخفیف های درصدی از مبلغ رزرو حساب می شود
    /// </summary>
    public class DiscountCoupon : Entity<long>
    {
        [Column("ID")]
        public override long Id { get; set; }
        public int UserID { get; set; }
        public DateTime CreateTime { get; set; }
        public DiscountCouponType Type { get; set; }
        public StatusEnum Status { get; set; }
        public int PresentorUserID { get; set; }
        public int Percent { get; set; }
        public long UsingReserveID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public enum DiscountCouponType
        {
            Unset = 0,
            Present = 1,
            Appreciate = 2,
            Moupon = 3,
            Instagram = 4,
            Yalda1400 = 5,
            Pedar1400 = 6
        }
        public enum StatusEnum
        {
            NotUsed = 0,
            Used = 1,
        }

        public static DiscountCouponType GetDiscountCouponType(string discountCode)
        {
            switch (discountCode.ToLower())
            {
                case "trip5off":
                    return DiscountCouponType.Moupon;
                case "inst8":
                    return DiscountCouponType.Instagram;
                case "pedar1400":
                    return DiscountCouponType.Pedar1400;
                case "yalda1400":
                    return DiscountCouponType.Yalda1400;
                default:
                    return DiscountCouponType.Unset;
            }
        }
    }
}
