using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs
{
    public class PasargadEpayResponseDTO
    {
        public string tref { get; set; } // TransactionReferenceID
        public int iN { get; set; } // InvoiceNumber - payment id
        public DateTime iD { get; set; } // InvoiceDate - payment date
        public string RedirectUrl { get; set; }
        public ActionLog.ActionSourceEnum ActionSource
        {
            get
            {
                return string.IsNullOrEmpty(RedirectUrl) ?
                    ActionLog.ActionSourceEnum.WebsiteDashboard :
                    ActionLog.ActionSourceEnum.Application;
            }
        }
    }
}
