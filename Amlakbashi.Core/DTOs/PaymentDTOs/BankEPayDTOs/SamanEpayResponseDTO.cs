using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs
{
    public class SamanEpayResponseDTO
    {
        public string MID { get; set; } // terminal id
        public string TerminalId { get; set; }
        public string State { get; set; }
        public StatusEnum Status { get; set; }
        public string RRN { get; set; } // reference number
        public string RefNum { get; set; } // transaction id
        public int ResNum { get; set; } // payment id
        public string TraceNo { get; set; }
        public long Amount { get; set; }
        public int Wage { get; set; } // payments fee (is optional)
        public string SecurePan { get; set; } // cart number

        public string RedirectUrl { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; } = ActionLog.ActionSourceEnum.WebsiteDashboard;

        public enum StatusEnum
        {
            CanceledByUser = 1,
            OK = 2,
            Failed = 3,
            SessionIsNull = 4,
            InvalidParameters = 5,
            MerchantIpAddressIsInvalid = 8,
            TokenNotFound = 10,
            TokenRequired = 11,
            TerminalNotFound = 12
        }
    }
}
