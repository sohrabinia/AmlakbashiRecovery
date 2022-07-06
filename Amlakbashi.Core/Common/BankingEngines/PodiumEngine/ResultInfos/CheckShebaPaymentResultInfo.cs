using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class CheckShebaPaymentResultInfo : PodiumResultInfo
    {
        public CheckShebaPaymentResultData ResultData { get; set; }
    }

    [Serializable]
    public class CheckShebaPaymentResultData
    {
        public string TransactionNumber { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string BankName { get; set; }
        public string State { get; set; }
        public int Amount { get; set; }
        public int BranchId { get; set; }
    }
}
