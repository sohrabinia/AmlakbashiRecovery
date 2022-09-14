using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs
{
    public class CheckTransactionStatusResponseDTO : PodiumResponseDTO
    {
        public string TransactionCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
    }

    public enum TransactionStatusEnum
    {
        Unset,
        InProgress,
        Success,
        UnSuccess,
        Reversed,
        Reversing
    }
}
