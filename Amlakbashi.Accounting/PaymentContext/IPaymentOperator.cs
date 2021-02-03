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
        bool VerifyPayment(BanksEnum bank, string paymentResult, int paymentId, long totalPayingPrice,
            out string referenceNumber, out long transactionReferenceID);
        Dictionary<string, object> GeneratePaymentData(BanksEnum bank,
            int paymentId, long paymentTotalAmount, string redirectAddress,
            out string sign, out DateTime invoiceDate);
    }
}
