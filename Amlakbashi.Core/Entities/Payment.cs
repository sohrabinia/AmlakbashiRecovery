using Amlakbashi.Core.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// پرداخت بانکی
    /// </summary>
    public class Payment : Entity<int>
    {
        public override int Id { get; set; }
        public int UserID { get; set; }
        public string TransactionId { get; set; }
        public long ReferenceNumber { get; set; }
        public string TraceNumber { get; set; }
        public long Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PayDate { get; set; }
        public BankEnum Bank { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentType Type { get; set; }
        public PaymentMethod Method { get; set; }
        public long? ReserveID { get; set; }
        public long? ReservePaymentId { get; set; }
        public long? WalletTransactionId { get; set; }
        public string ProductType { get; set; }
        public long ReservePrice { get; set; }
        public long CouponID { get; set; }
        public long PrizePrice { get; set; }

        [Column(TypeName = "varchar(16)")]
        public string CreditCardNumber { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }

        [ForeignKey("ReservePaymentId")]
        public virtual ReservePayment ReservePayment { get; set; }

        [ForeignKey("WalletTransactionId")]
        public virtual CreditTransaction CreditTransaction { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; }

        public enum PaymentStatus
        {
            NotPaid = 0,
            Paid = 1
        }

        public enum PaymentType
        {
            Income = 0,
            Expenditure = 1
        }

        public enum PaymentMethod
        {
            EPay = 0,
            Podium = 1
        }
    }
}
