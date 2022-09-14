using Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces;
using Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs;
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

        public PayaPaymentResponseDTO PayaPayment(PayaPaymentRequestDTO reqeustDTO)
        {
            return podiumEngine.PayaPayment(reqeustDTO);
        }

        public ShebaVerificationResponseDTO ShebaVerification(string sheba)
        {
            return podiumEngine.ShebaVerification(sheba);
        }

        public CheckPayaPaymentResponseDTO CheckPayaPayment(string transactionId, string traceNumber)
        {
            return podiumEngine.CheckPayaPayment(transactionId, traceNumber);
        }

        public CheckTransactionStatusResponseDTO CheckTransactionStatus(string transactionId)
        {
            return podiumEngine.CheckTransactionStatus(transactionId);
        }
    }
}
