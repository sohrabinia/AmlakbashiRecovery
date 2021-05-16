using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    [Serializable]
    public class ShebaPaymentBatchResultDTO
    {
        public string ReferenceNumber { get; set; }
        public string DestinationBankName { get; set; }
        public string State { get; set; }
        public long BatchNumber { get; set; }
        public int BatchTransferId { get; set; }
        public int Amount { get; set; }
        public string BeneficiaryFullName { get; set; }
        public string Description { get; set; }
        public string DestShebaNumber { get; set; }
        public string BillNumber { get; set; }
        public string InquiryName { get; set; }
    }
}
