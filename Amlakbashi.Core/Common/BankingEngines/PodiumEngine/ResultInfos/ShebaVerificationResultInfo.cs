using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class ShebaVerificationResultInfo : PodiumResultInfo
    {
        public string sheba { get; set; }
        public BankAccountOwner[] owners { get; set; }
    }
}