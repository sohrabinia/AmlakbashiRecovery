using System;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    [Serializable]
    public class ShebaVerificationResultDTO : BankingResultDTO
    {
        public string sheba { get; set; }
        public BankAccountOwnerDTO[] owners { get; set; }
    }
}
