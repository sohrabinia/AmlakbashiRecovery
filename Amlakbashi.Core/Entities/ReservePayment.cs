using Amlakbashi.Core.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// هر گونه پرداخت برای یک رزرو از طریق پرداخت بانکی یا کیف پول
    /// </summary>
    public class ReservePayment : Entity<long>, ISoftDelete
    {
        [Column("ReservePaymentID")]
        public override long Id { get; set; }
        public int Status { get; set; }
        public long ReserveID { get; set; }
        public int UserID { get; set; }
        public int OperatorID { get; set; }
        public long TransactionID { get; set; }
        public long RefID { get; set; }
        public int PaymentType { get; set; }
        public DateTime CreateDate { get; set; }
        public long Price { get; set; }
        public int PaymentMethod { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public enum ReservePaymentType
        {
            GuestDeposite = 0,
            GuestClearing = 1,
            SiteDepositeToHost = 2,
            SiteClearingToHost = 3,
            SiteRefundToGuest = 4,
        }

        public enum ReservePaymentMethod
        {
            EPay = 0,
            AmlakbashiCredit = 1,
            BankCard = 2
        }

        public static string GetPaymentDatabaseString(ReservePaymentType payment_type)
        {
            return "Reserve_" + payment_type.ToString();
        }
    }
}
