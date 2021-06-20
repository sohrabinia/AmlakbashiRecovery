using Amlakbashi.Core.DTOs.PaymentDTOs;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Accounting.PaymentContext
{
    internal interface IPaymentEngine
    {
        bool ReadPaymentResult(string tref, out string result);
        CheckPaymentDTO ReadPaymentResult(long paymentId, DateTime paymentDate);
        bool VerifyPayment(string paymentResult, int paymentId, long totalPayingPrice,
            out string referenceNumber, out long transactionReferenceID, out DateTime transactionDate);
        Dictionary<string, object> GeneratePaymentData(int paymentId,
            long paymentTotalAmount, string redirectAddress,
            out string sign, out DateTime invoiceDate);
    }
}
