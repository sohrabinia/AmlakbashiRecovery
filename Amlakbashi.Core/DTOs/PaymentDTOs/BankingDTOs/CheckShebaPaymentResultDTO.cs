using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs
{
    public class CheckShebaPaymentResultDTO : BankingResultDTO
    {
        public string RefrenceNumber { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
