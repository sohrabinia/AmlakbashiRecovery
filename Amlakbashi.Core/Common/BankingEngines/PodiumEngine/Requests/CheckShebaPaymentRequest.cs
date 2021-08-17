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
        private const string productId = "487396";
        private const string apiKey = "9283cddaf3064a38a515cf796c9f38df";

        private readonly CheckShebaPaymentRequestInfo request;
        public CheckShebaPaymentRequest(string date, string paymentId)
        {
            request = new CheckShebaPaymentRequestInfo()
            {
                UserName = userName,
                Date = date,
                PaymentId = paymentId,
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
