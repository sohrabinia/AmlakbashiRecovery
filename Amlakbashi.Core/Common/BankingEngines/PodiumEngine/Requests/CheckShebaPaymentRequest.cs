using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class CheckShebaPaymentRequest : PodiumRequest<CheckShebaPaymentResult, CheckShebaPaymentResultInfo, CheckShebaPaymentRequestInfo>
    {
        private const string productId = "1476957";
        private const string apiKey = "9feb5d62c3f74b01b30231509e5f5e77";

        private readonly CheckShebaPaymentRequestInfo request;
        public CheckShebaPaymentRequest(string transactionId, string traceNumber)
        {
            request = new CheckShebaPaymentRequestInfo()
            {
                EndToEndId = traceNumber,
                TransactionId = transactionId
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
