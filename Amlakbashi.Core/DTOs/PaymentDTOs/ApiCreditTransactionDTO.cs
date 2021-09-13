using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.PaymentDTOs
{
    [Serializable]
    public class ApiCreditTransactionDTO
    {
        public long id { get; set; }
        public long price { get; set; }
        public long remainedPrice { get; set; }
        public string dateString { get; set; }
        public string reasonString { get; set; }
        public string reasonColor { get; set; }
        public string comment1 { get; set; }
        public string comment2 { get; set; }

        public static implicit operator ApiCreditTransactionDTO(CreditTransaction transaction)
        {
            var dto = new ApiCreditTransactionDTO();
            dto.comment1 = "";
            dto.comment2 = "";
            dto.reasonColor = "";
            switch (transaction.TransactionCause)
            {
                case CreditTransaction.WalletTransactionReason.Reserve:
                case CreditTransaction.WalletTransactionReason.SitePortion:
                    dto.reasonColor = "#4285F4";
                    dto.comment1 = "کد رزرو: " + transaction.ReserveID;
                    break;
                case CreditTransaction.WalletTransactionReason.Charge:
                    dto.reasonColor = "#34A853";
                    dto.comment1 = "کد پیگیری: " + transaction.BankTransactionID;
                    break;
                case CreditTransaction.WalletTransactionReason.Clearing:
                case CreditTransaction.WalletTransactionReason.Refund:
                    dto.reasonColor = "#34A853";
                    dto.comment1 = "کد رزرو: " + transaction.ReserveID;
                    break;
                case CreditTransaction.WalletTransactionReason.ContactAdvertise:
                    dto.reasonColor = "#34A853";
                    dto.comment1 = "کد آگهی: نامشخص";
                    dto.comment2 = "کد کاربر: " + transaction.UserID;
                    break;
                case CreditTransaction.WalletTransactionReason.Other:
                    dto.reasonColor = "#4285F4";
                    dto.comment1 = transaction.BankTransactionID > 0 ? "کد پیگیری: " + transaction.BankTransactionID : "";
                    break;
            }
            dto.id = transaction.Id;
            dto.price = transaction.Price;
            dto.remainedPrice = transaction.RemainedPrice;
            dto.reasonString = CreditTransaction.GetCreditTransactionCauseString(transaction.TransactionCause, transaction.TransactionCauseString);
            dto.dateString = DateTimeUtility.ConvertDate(transaction.Date, true) + transaction.Date.ToString(" HH:mm");
            return dto;
        }
    }
}
