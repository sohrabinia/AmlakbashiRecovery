using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    public class CheckTransactionStatusRequestInfo : PodiumRequestInfo
    {
        public string TransactionId { get; set; }
    }
}
