using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class ShebaPaymentResultInfo : PodiumResultInfo
    {
        //public string result { get; set; }
        public ShabaPayResult result { get; set; }
        public string MessageCode { get; set; }
        [Serializable]
        public class ShabaPayResult
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public List<BatchPayaResultItem> Data { get; set; }
            public int MessageCode { get; set; }
        }
    }
}
