using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Newtonsoft.Json;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class ShebaVerificationRequestInfo : PodiumRequestInfo
    {
        public string UserName { get; set; }
        public string Sheba { get; set; }
        public string Timestamp { get; set; }
    }
}