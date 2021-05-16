using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class ShebaVerificationRequest : PodiumRequest<ShebaVerificationResult, ShebaVerificationResultInfo, ShebaVerificationRequestInfo>
    {
        private readonly string sheba;
        private const string productId = "34254";
        public ShebaVerificationRequest(string sheba)
        {
            this.sheba = sheba;
        }

        protected override string GetProductId()
        {
            return productId;
        }

        protected override ShebaVerificationRequestInfo GetRequestData()
        {
            return new ShebaVerificationRequestInfo()
            {
                sheba = sheba
            };
        }
    }
}