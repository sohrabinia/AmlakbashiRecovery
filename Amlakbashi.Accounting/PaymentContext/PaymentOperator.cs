using Amlakbashi.Accounting.PaymentContext.BankEngines.Interfaces;
using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.PaymentContext
{
    internal class PaymentOperator : IPaymentOperator
    {
        private readonly IPasargadEngine pasargadEngine;
        private readonly ISamanEngine samanEngine;
        public PaymentOperator(IPasargadEngine pasargadEngine, ISamanEngine samanEngine)
        {
            this.pasargadEngine = pasargadEngine;
            this.samanEngine = samanEngine;
        }

        public EpayDTO GeneratePaymentData(BankEnum bank, int paymentId, long paymentTotalAmount,
            string redirectAddress, out DateTime invoiceDate)
        {
            return pasargadEngine.GeneratePaymentData(paymentId,
                paymentTotalAmount, redirectAddress, out invoiceDate);
        }

        public CheckPaymentDTO GetPasargadPaymentResult(BankEnum bank, string tref, out string result)
        {
            return pasargadEngine.GetPaymentResult(tref, out result);
        }

        public CheckPaymentDTO GetPasargadPaymentResult(BankEnum bank, long paymentId, DateTime paymentDate)
        {
            return pasargadEngine.GetPaymentResult(paymentId, paymentDate);
        }

        public bool VerifyPasargadPayment(BankEnum bank, string paymentResult,
            int paymentId, long totalPayingPrice)
        {
            return pasargadEngine.VerifyPayment(paymentResult, paymentId, totalPayingPrice);
        }

        public async Task<EpayDTO> GetSamanPaymentToken(SamanRequestTokenDTO requestToken)
        {
            return await samanEngine.GetPaymentToken(requestToken);
        }

        public Task<string> VerifySamanEpay(string RefNum)
        {
            return samanEngine.VerifyEpay(RefNum);
        }
    }
}
