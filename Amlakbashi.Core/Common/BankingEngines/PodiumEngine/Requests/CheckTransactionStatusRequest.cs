using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class CheckTransactionStatusRequest : PodiumRequest<CheckTransactionStatusResult, CheckTransactionStatusResultInfo, CheckTransactionStatusRequestInfo>
    {
        private const string productId = "1424918";
        private readonly CheckTransactionStatusRequestInfo request;

        public CheckTransactionStatusRequest(string transactionId)
        {
            request = new CheckTransactionStatusRequestInfo()
            {
                TransactionId = transactionId
            };
        }

        protected override RequestData GetRequestData()
        {
            string jsonRequest = request.GenerateJson();
            return new RequestData()
            {
                scProductId = productId,
                scApiKey = string.Empty,
                request = jsonRequest
            };
        }
    }
}
