using System;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    [Serializable]
    public class ShebaPaymentResultDTO : BankingResultDTO
    {
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public int UserId { get; set; }
        public long AdvertiseId { get; set; }
        public long PayablePrice { get; set; }
    }
}
