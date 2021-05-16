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

        public Task<ShebaPaymentResultDTO> PaySheba(string sheba, long amount, string fullName)
        {
            return podiumEngine.PaySheba(sheba, amount, fullName);
        }

        public Task<ShebaVerificationResultDTO> VerifySheba(string sheba)
        {
            return podiumEngine.VerifySheba(sheba);
        }
    }
}
