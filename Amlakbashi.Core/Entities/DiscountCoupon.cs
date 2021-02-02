using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
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
        }
        public enum StatusEnum
        {
            NotUsed = 0,
            Used = 1,
        }
    }
}
