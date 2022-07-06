using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class CheckShebaPaymentRequestInfo : PodiumRequestInfo
    {
        public string EndToEndId { get; set; }
        public string TransactionId { get; set; }
    }
}
