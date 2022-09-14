using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class CheckTransactionStatusResultInfo : PodiumResultInfo
    {
        public CheckTransactionStatusResultData ResultData { get; set; }
    }

    [Serializable]
    public class CheckTransactionStatusResultData
    {
        public string TransactionCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionStatus { get; set; }
    }
}
