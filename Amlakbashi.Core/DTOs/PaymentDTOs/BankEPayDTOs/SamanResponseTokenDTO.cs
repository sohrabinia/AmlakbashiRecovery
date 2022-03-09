using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs
{
    public class SamanResponseTokenDTO
    {
        public int status { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public string token { get; set; }
    }
}
