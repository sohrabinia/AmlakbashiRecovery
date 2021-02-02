using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class Payment : Entity<int>
    {
        [Column("PaymentID")]
        public override int Id { get; set; }
        public string Authority { get; set; }
        public int UserID { get; set; }
        public long RefID { get; set; }
        public long TotalPrice { get; set; }
        public DateTime Date { get; set; }
        public int BankId { get; set; }
        public long? ReserveID { get; set; }
        public long CouponID { get; set; }
        public long PrizePrice { get; set; }
        public long ReservePrice { get; set; }
        public string ProductType { get; set; }
        public int Status { get; set; }
        public DateTime? PayDate { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; }

        public enum PaymentStatus
        {
            NotPaid = 0,
            Paid = 1
        }
    }
}
