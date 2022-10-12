using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class ShebaPaymentResultInfo : PodiumResultInfo
    {
        public int Amount { get; set; }
        public string RecieverFullNam { get; set; }
        public string DestinationIban { get; set; }
        public string Description { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionId { get; set; } // شناسه یکتای تراکنش
        public string EndToEndId { get; set; } // شماره پیگیری
        public string TransactionCode { get; set; } // شماره پیگیری تراکنش
    }
}
