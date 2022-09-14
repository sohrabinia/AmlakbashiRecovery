using Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.BankingContext
{
    internal interface IBankingOperator
    {
        ShebaVerificationResponseDTO ShebaVerification(string sheba);
        PayaPaymentResponseDTO PayaPayment(PayaPaymentRequestDTO reqeustDTO);
        CheckPayaPaymentResponseDTO CheckPayaPayment(string transactionId, string traceNumber);
        CheckTransactionStatusResponseDTO CheckTransactionStatus(string transactionId);
    }
}
