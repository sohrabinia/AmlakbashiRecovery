using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankingDTOs;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class ShebaPaymentRequest : PodiumRequest<ShebaPaymentResult, ShebaPaymentResultInfo, ShebaPaymentRequestInfo>
    {
        private const string apiKey = "0a84cf56a4cc4951bfd64f2c700d61d5";
        private const string productId = "445929";
        private const string sourceDepositNumber = "3902.800.97374.1";
        private const string sourceSheba = "IR440570390280000097374001";

        private readonly ShebaPaymentRequestInfo request;
        public ShebaPaymentRequest(ShebaPaymentRequestDTO requestDTO)
        {
            request = new ShebaPaymentRequestInfo()
            {
                UserName = userName,
                SourceDepositNumber = sourceDepositNumber,
                DestDepositNumber = requestDTO.DestDepositNumber,
                SourceSheba = sourceSheba,
                DestSheba = requestDTO.DestSheba,
                DestFirstName = requestDTO.DestFirstName,
                DestLastName = requestDTO.DestLastName,
                Amount = requestDTO.Amount,
                PaymentId = requestDTO.PaymentId,
                SourceComment = requestDTO.SourceComment,
                DestComment = requestDTO.DestComment,
                CentralBankTransferDetailType = (int)requestDTO.CentralBankTransferDetailType,
                Timestamp = requestDTO.Timestamp.ToString("yyyy/MM/dd HH:mm:ss:FFF")
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