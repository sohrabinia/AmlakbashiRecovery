using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class CheckShebaPaymentRequestInfo : PodiumRequestInfo
    {
        public string UserName { get; set; }
        public string Date { get; set; }
        public string PaymentId { get; set; }
        public string Timestamp { get; set; }
    }
}
