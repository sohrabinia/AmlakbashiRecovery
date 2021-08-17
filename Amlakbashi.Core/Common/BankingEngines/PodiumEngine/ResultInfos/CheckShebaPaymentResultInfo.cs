using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class CheckShebaPaymentResultInfo : PodiumResultInfo
    {
        public CheckShebaPaymentResultData Data { get; set; }
    }

    [Serializable]
    public class CheckShebaPaymentResultData
    {
        public string RefrenceNumber { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
