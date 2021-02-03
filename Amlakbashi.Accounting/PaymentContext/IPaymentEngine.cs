using System;
using System.Collections.Generic;

namespace Amlakbashi.Accounting.PaymentContext
{
    internal interface IPaymentEngine
    {
        bool ReadPaymentResult(string tref, out string result);
        bool VerifyPayment(string paymentResult, int paymentId, long totalPayingPrice,
            out string referenceNumber, out long transactionReferenceID);
        Dictionary<string, object> GeneratePaymentData(int paymentId,
            long paymentTotalAmount, string redirectAddress,
            out string sign, out DateTime invoiceDate);
    }
}
