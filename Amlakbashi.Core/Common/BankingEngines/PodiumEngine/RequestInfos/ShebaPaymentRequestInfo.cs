using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class ShebaPaymentRequestInfo : PodiumRequestInfo
    {
        public string UserName { get; set; }
        public string SourceDepositNumber { get; set; }
        public string SourceSheba { get; set; }
        public string DestDepositNumber { get; set; }
        public string DestSheba { get; set; }
        public string DestFirstName { get; set; }
        public string DestLastName { get; set; }
        public int CentralBankTransferDetailType { get; set; }
        public long Amount { get; set; }
        public string SourceComment { get; set; }
        public string DestComment { get; set; }
        public long PaymentId { get; set; }
        public string Timestamp { get; set; }
    }
}