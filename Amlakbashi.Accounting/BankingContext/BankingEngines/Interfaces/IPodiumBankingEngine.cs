using Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs;

namespace Amlakbashi.Accounting.BankingContext.BankingEngines.Interfaces
{
    internal interface IPodiumBankingEngine
    {
        ShebaVerificationResponseDTO ShebaVerification(string sheba);
        PayaPaymentResponseDTO PayaPayment(PayaPaymentRequestDTO requestDTO);
        CheckPayaPaymentResponseDTO CheckPayaPayment(string transactionId, string traceNumber);
        CheckTransactionStatusResponseDTO CheckTransactionStatus(string transactionId);
    }
}
