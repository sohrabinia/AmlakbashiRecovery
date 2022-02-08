using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.PaymentContext.BankEngines.Interfaces
{
    internal interface IPasargadEngine
    {
        CheckPaymentDTO GetPaymentResult(string tref, out string result);
        Task<CheckPaymentDTO> GetPaymentResult(long paymentId, DateTime paymentDate);
        bool VerifyPayment(string paymentResult, int paymentId, long totalPayingPrice);
        EpayDTO GetPaymentData(int paymentId, long paymentTotalAmount, string redirectAddress);
    }
}
