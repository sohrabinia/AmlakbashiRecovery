using Amlakbashi.Core.Common.Enums;
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
        CheckPaymentDTO ReadPaymentResult(BankEnum bank, string tref, out string result);
        CheckPaymentDTO ReadPaymentResult(BankEnum bank, long paymentId, DateTime paymentDate);
        bool VerifyPayment(BankEnum bank, string paymentResult, int paymentId, long totalPayingPrice);
        Dictionary<string, object> GeneratePaymentData(BankEnum bank,
            int paymentId, long paymentTotalAmount, string redirectAddress,
            out string sign, out DateTime invoiceDate);
    }
}
