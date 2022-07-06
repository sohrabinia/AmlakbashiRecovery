using Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.BankingContext
{
    internal class BankingOperator : IBankingOperator
    {
        private readonly IPodiumBankingEngine podiumEngine;
        public BankingOperator(IPodiumBankingEngine podiumEngine)
        {
            this.podiumEngine = podiumEngine;
        }

        public ShebaPaymentResultDTO ShebaPayment(ShebaPaymentRequestDTO reqeustDTO)
        {
            return podiumEngine.ShebaPayment(reqeustDTO);
        }

        public ShebaVerificationResultDTO ShebaVerification(string sheba)
        {
            return podiumEngine.ShebaVerification(sheba);
        }

        public CheckShebaPaymentResultDTO CheckShebaPaymentStatus(string transactionId, string traceNumber)
        {
            return podiumEngine.CheckShebaPaymentStatus(transactionId, traceNumber);
        }
    }
}
