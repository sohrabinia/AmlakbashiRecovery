using Amlakbashi.Core.DTOs.PaymentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.PaymentContext
{
    internal interface IPaymentOperator
    {
        bool ReadPaymentResult(BanksEnum bank, string tref, out string result);
        CheckPaymentDTO ReadPaymentResult(BanksEnum bank, long paymentId, DateTime paymentDate);
        bool VerifyPayment(BanksEnum bank, string paymentResult, int paymentId, long totalPayingPrice,
            out string referenceNumber, out long transactionReferenceID, out DateTime transactionDate);
        Dictionary<string, object> GeneratePaymentData(BanksEnum bank,
            int paymentId, long paymentTotalAmount, string redirectAddress,
            out string sign, out DateTime invoiceDate);
    }
}
