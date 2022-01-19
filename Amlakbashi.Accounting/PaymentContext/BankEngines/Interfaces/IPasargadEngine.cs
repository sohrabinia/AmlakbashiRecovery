using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Accounting.PaymentContext.BankEngines.Interfaces
{
    internal interface IPasargadEngine
    {
        CheckPaymentDTO GetPaymentResult(string tref, out string result);
        CheckPaymentDTO GetPaymentResult(long paymentId, DateTime paymentDate);
        bool VerifyPayment(string paymentResult, int paymentId, long totalPayingPrice);
        EpayDTO GeneratePaymentData(int paymentId,
            long paymentTotalAmount, string redirectAddress, out DateTime invoiceDate);
    }
}
