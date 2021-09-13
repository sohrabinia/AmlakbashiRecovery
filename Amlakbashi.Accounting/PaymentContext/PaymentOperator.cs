using Amlakbashi.Accounting.PaymentContext.PaymentEngines.Interfaces;
using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.DTOs.PaymentDTOs;
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

        public Dictionary<string, object> GeneratePaymentData(BankEnum bank, int paymentId, long paymentTotalAmount, string redirectAddress, out string sign, out DateTime invoiceDate)
        {
            return GetEngine(bank).GeneratePaymentData(paymentId,
                paymentTotalAmount, redirectAddress, out sign, out invoiceDate);
        }

        public CheckPaymentDTO ReadPaymentResult(BankEnum bank, string tref, out string result)
        {
            return GetEngine(bank).ReadPaymentResult(tref, out result);
        }

        public CheckPaymentDTO ReadPaymentResult(BankEnum bank, long paymentId, DateTime paymentDate)
        {
            return GetEngine(bank).ReadPaymentResult(paymentId, paymentDate);
        }

        public bool VerifyPayment(BankEnum bank, string paymentResult,
            int paymentId, long totalPayingPrice)
        {
            return GetEngine(bank).VerifyPayment(paymentResult, paymentId, totalPayingPrice);
        }

        private IPaymentEngine GetEngine(BankEnum bank)
        {
            switch (bank)
            {   
                case BankEnum.Pasargad:
                    return pasargadEngine;
                default:
                    return null;
            }
        }
    }
}
