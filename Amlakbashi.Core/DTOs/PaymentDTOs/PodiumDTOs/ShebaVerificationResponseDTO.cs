using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs
{
    [Serializable]
    public class ShebaVerificationResponseDTO : PodiumResponseDTO
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

    [Serializable]
    public class BankAccountOwnerDTO
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
