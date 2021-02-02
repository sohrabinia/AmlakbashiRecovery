using Amlakbashi.Core.Common.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    public class PrizeCreditTransaction : Entity<long>
    {
        [Column("ID")]
        public override long Id { get; set; }
        public int UserID { get; set; }
        public long Price { get; set; }
        public DateTime Date { get; set; }
        public long RemainedPrice { get; set; }
        public PrizeTransactionType Type { get; set; }
        public long? ReserveID { get; set; }
        public string CustomTitle { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }

        public enum PrizeTransactionType
        {
            Unset = 0,
            IncreasePresent = 1,
            DecreaseReserve = 2,
            IncreaseRefund = 3,
            Custom = 4
        }
    }
}
