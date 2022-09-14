using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs
{
    public class PayaPaymentRequestDTO
    {
        public string DestSheba { get; set; }
        public string DestFirstName { get; set; }
        public string DestLastName { get; set; }
        public CentralBankTransferEnum CentralBankTransferDetailType { get; set; }
        public long Amount { get; set; }
        public string SourceComment { get; set; }
        public string DestComment { get; set; }
        public long PaymentId { get; set; }
        public DateTime Timestamp { get; set; }
        public string TransactionId { get; set; }
    }
}
