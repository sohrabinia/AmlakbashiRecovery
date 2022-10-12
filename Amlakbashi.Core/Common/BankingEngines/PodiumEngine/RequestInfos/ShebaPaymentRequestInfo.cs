using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class ShebaPaymentRequestInfo : PodiumRequestInfo
    {
        public string SourceDepNum { get; set; }
        public string DestinationIban { get; set; }
        public string RecieverFullName { get; set; }
        public int DetailType { get; set; }
        public long Amount { get; set; }
        public string SrcComment { get; set; }
        public string DestComment { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionId { get; set; }
        public string Description { get; set; }
        public bool IsAutoVerify { get; set; } = true;
        public string senderReturnDepositNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string DestBankCode { get; set; }
        public string TransactionBillNumber { get; set; }
        public string SourceTMBillNumber { get; set; }
    }
}