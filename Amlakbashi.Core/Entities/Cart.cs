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
    /// قبلا به عنوان سبد خرید استفاده می شد.
    /// در حال حاضر، به ازای هر پرداخت بانکی یک سبد خرید ساخته می شود. 
    /// </summary>
    public class Cart : Entity<long>
    {
        [Column("CartID")]
        public override long Id { get; set; }
        public string Type { get; set; }
        public int AmlakID { get; set; }
        public long? AdvertiseID { get; set; }
        public long? ReserveID { get; set; }
        public int BannerID { get; set; }
        public int UserID { get; set; }
        public int Count { get; set; }
        public int Status { get; set; }
        public long Price { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PayDate { get; set; }

        [Column("Payment_PaymentID")]
        public int? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("AdvertiseID")]
        public virtual Advertise Advertise { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }

        public enum CartStatus
        {
            NotPaid = 0,
            Paid = 1,
            Suspend = 2,
            Deleted = 3
        }
    }
}
