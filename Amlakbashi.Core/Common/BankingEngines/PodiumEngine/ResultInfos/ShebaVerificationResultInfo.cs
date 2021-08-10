using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class ShebaVerificationResultInfo : PodiumResultInfo
    {
        public ShebaVerificationResultData Data { get; set; }
    }

    [Serializable]
    public class ShebaVerificationResultData
    {
        public string Sheba { get; set; }
        public string AccountStatus { get; set; }
        public string AccountStatusName { get; set; }
        public BankAccountOwner[] AccountOwners { get; set; }
    }
}