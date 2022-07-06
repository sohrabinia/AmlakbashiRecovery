using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Newtonsoft.Json;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class ShebaVerificationRequestInfo : PodiumRequestInfo
    {
        public string Iban { get; set; }
    }
}