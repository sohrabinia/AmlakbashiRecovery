using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class ShebaVerificationRequest : PodiumRequest<ShebaVerificationResult, ShebaVerificationResultInfo, ShebaVerificationRequestInfo>
    {
        private const string productId = "437012";
        private const string apiKey = "2dace115dfaa47f39ef03fbffb88b88d";

        private readonly ShebaVerificationRequestInfo request;
        public ShebaVerificationRequest(string sheba)
        {
            request = new ShebaVerificationRequestInfo()
            {
                UserName = userName,
                Sheba = sheba,
                Timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:FFF")
            };
        }

        protected override RequestData GetRequestData()
        {
            string jsonRequest = request.GenerateJson();
            return new RequestData()
            {
                scProductId = productId,
                scApiKey = apiKey,
                request = jsonRequest,
                signature = GenerateSignature(jsonRequest)
            };
        }
    }
}