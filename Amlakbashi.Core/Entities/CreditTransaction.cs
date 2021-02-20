using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class CreditTransaction : Entity<long>
    {
        [Column("CreditTransactionID")]
        public override long Id { get; set; }
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public long Price { get; set; }
        public DateTime Date { get; set; }
        public long RemainedPrice { get; set; }
        public long BankTransactionID { get; set; }
        public long? ReserveID { get; set; }
        public int TransactionCause { get; set; }
        public string TransactionCauseString { get; set; }
        public long AdvertiseContactID { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }
    }
}
