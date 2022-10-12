using System;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs
{
    [Serializable]
    public class PayaPaymentResponseDTO : PodiumResponseDTO
    {
        public string Message { get; set; }
        public string TraceNumber { get; set; }
        public string TransactionId { get; set; }
        public string RecieverFullName { get; set; }
        public int UserId { get; set; }
        public long AdvertiseId { get; set; }
        public long PayablePrice { get; set; }
    }
}
