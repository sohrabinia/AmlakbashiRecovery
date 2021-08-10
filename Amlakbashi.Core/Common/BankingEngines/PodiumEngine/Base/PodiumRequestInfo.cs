using Newtonsoft.Json;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base
{
    public abstract class PodiumRequestInfo
    {
        public string GenerateJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}