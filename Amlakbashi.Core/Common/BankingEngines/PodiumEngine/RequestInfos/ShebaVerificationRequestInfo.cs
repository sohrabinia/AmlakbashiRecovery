using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class ShebaVerificationRequestInfo : PodiumRequestInfo
    {
        public string sheba { get; set; }
    }
}