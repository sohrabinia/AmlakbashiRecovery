using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    [Serializable]
    public class ShebaVerificationResultDTO : BankingResultDTO
    {
        public string Sheba { get; set; }
        public string Message { get; set; }
        public string AccountStatus { get; set; }
        public List<BankAccountOwnerDTO> Owners { get; set; }
        public string HostName { get; set; }
        public string BankCardName { get; set; }
        public int BankCardId { get; set; }
        public bool ShebaVerify { get; set; }
    }
}
