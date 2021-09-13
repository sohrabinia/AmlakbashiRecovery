using Amlakbashi.Core.DTOs.PaymentDTOs;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Accounting.PaymentContext
{
    internal interface IPaymentEngine
    {
        CheckPaymentDTO ReadPaymentResult(string tref, out string result);
        CheckPaymentDTO ReadPaymentResult(long paymentId, DateTime paymentDate);
        bool VerifyPayment(string paymentResult, int paymentId, long totalPayingPrice);
        Dictionary<string, object> GeneratePaymentData(int paymentId,
            long paymentTotalAmount, string redirectAddress,
            out string sign, out DateTime invoiceDate);
    }
}
