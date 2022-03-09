using Amlakbashi.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs
{
    public class EpayDTO
    {
        public Dictionary<string, object> BankData { get; set; }
        public string Url { get; set; }
        public BankEnum Bank { get; set; }
        public DateTime Date { get; set; }
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
