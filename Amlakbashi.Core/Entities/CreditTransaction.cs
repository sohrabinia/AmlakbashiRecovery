using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amlakbashi.Core.Entities
{
    /// <summary>
    /// تراکنش های کیف پول
    /// </summary>
    [Table("WalletTransactions")]
    public class CreditTransaction : Entity<long>
    {
        [Column("Id")]
        public override long Id { get; set; }
        public DateTime Date { get; set; }
        public WalletTransactionType Type { get; set; }
        [Column("Amount")]
        public long Price { get; set; }
        [Column("WalletRemainingAmount")]
        public long RemainedPrice { get; set; }
        public long BankTransactionID { get; set; }
        [Column("Reason")]
        public WalletTransactionReason TransactionCause { get; set; }
        [Column("Description")]
        public string TransactionCauseString { get; set; }
        public int UserID { get; set; }
        public long? ReserveID { get; set; }
        public int? PaymentId { get; set; }
        public long? ModifiedWalletTransactionId { get; set; }

        //public long AdvertiseContactID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("ReserveID")]
        public virtual Reserve Reserve { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        [ForeignKey("ModifiedWalletTransactionId")]
        public virtual CreditTransaction ModifiedWalletTransaction { get; set; }

        public virtual CreditTransaction CorrectiveWalletTransaction { get; set; }

        public static string GetCreditTransactionCauseString(WalletTransactionReason reason, string transactionCauseString = "")
        {
            switch (reason)
            {
                case WalletTransactionReason.Reserve:
                    return "رزرو اقامتگاه";
                case WalletTransactionReason.SitePortion:
                    return "پرداخت درصد املاک باشی";
                case WalletTransactionReason.Charge:
                    return "شارژ کیف پول";
                case WalletTransactionReason.Clearing:
                    return "تسویه با میزبان";
                case WalletTransactionReason.Refund:
                    return "عودت به مهمان";
                case WalletTransactionReason.ContactAdvertise:
                    return "نمایش تماس";
                case WalletTransactionReason.Corrective:
                    return "اصلاح تراکنش";
                case WalletTransactionReason.Other:
                    return transactionCauseString;
                default:
                    return "";
            }
        }

        public enum WalletTransactionReason
        {
            Reserve = 1,
            SitePortion = 2,
            Charge = 3,
            Clearing = 4,
            Refund = 5,
            ContactAdvertise = 6,
            Corrective = 7,
            Other = 100
        }

        public enum WalletTransactionType
        {
            Decrease = 0,
            Increase = 1
        }

        public enum WalletTransactionTypeForPayment
        {
            Credit_Increase = 1,
            Credit_Decrease = 2,
            Credit_Inc_Then_Res
        }
    }
}
