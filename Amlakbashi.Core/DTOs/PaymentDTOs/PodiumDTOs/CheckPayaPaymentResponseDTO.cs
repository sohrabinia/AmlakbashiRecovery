using System;
using System.Collections.Generic;
using System.Text;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs
{
    public class CheckPayaPaymentResponseDTO : PodiumResponseDTO
    {
        public string RefrenceNumber { get; set; }
        public string Status { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string BankName { get; set; }
        public int Amount { get; set; }
        public Payment.PaymentStatus PaymentStatus { get; set; }
        public int PaymentId { get; set; }
    }
}
