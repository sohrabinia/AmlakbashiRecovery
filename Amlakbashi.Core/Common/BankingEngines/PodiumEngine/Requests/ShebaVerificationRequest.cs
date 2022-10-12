using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class ShebaVerificationRequest : PodiumRequest<ShebaVerificationResult, ShebaVerificationResultInfo, ShebaVerificationRequestInfo>
    {
        private const string productId = "1115396";
        private const string apiKey = "1b9f9ab699f141e08dc82964c2ad16e5";
        private readonly ShebaVerificationRequestInfo request;
        public ShebaVerificationRequest(string sheba)
        {
            request = new ShebaVerificationRequestInfo()
            {
                Iban = sheba
            };
        }

        protected override RequestData GetRequestData()
        {
            string jsonRequest = request.GenerateJson();
            return new RequestData()
            {
                scProductId = productId,
                scApiKey = apiKey,
                request = jsonRequest
            };
        }
    }
}