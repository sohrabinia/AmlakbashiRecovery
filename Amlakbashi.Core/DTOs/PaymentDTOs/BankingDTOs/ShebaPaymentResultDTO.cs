using System;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    [Serializable]
    public class ShebaPaymentResultDTO : BankingResultDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public ShebaPaymentBatchResultDTO[] Data { get; set; }
    }
}
