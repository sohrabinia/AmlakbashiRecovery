using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs
{
    public class SamanRequestTokenDTO
    {
        public string Action { get; } = "token";
        public string TerminalId { get; set; }
        public string RedirectUrl { get; set; }
        public string ResNum { get; set; }
        public long Amount { get; set; }
        public string CellNumber { get; set; }
    }
}
