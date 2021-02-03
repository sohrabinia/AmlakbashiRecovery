using Amlakbashi.Accounting.PaymentContext.PaymentEngines.Interfaces;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Accounting.PaymentContext
{
    internal class PaymentOperator : IPaymentOperator
    {
        private readonly IPasargadPaymentEngine pasargadEngine;
        public PaymentOperator(IPasargadPaymentEngine pasargadEngine)
        {
            this.pasargadEngine = pasargadEngine;
        }

        public Dictionary<string, object> GeneratePaymentData(BanksEnum bank, int paymentId, long paymentTotalAmount, string redirectAddress, out string sign, out DateTime invoiceDate)
        {
            return GetEngine(bank).GeneratePaymentData(paymentId,
                paymentTotalAmount, redirectAddress, out sign, out invoiceDate);
        }

        public bool ReadPaymentResult(BanksEnum bank, string tref, out string result)
        {
            return GetEngine(bank).ReadPaymentResult(tref, out result);
        }

        public bool VerifyPayment(BanksEnum bank, string paymentResult,
            int paymentId, long totalPayingPrice,
            out string referenceNumber, out long transactionReferenceID)
        {
            return GetEngine(bank).VerifyPayment(paymentResult, paymentId,
                totalPayingPrice, out referenceNumber, out transactionReferenceID);
        }

        private IPaymentEngine GetEngine(BanksEnum bank)
        {
            switch (bank)
            {   
                case BanksEnum.Pasargad:
                    return pasargadEngine;
                default:
                    return null;
            }
        }
    }
}
