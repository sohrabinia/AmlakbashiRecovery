using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using Amlakbashi.Core.DTOs.PaymentDTOs.PodiumDTOs;
using Amlakbashi.Core.Common.Utilities;
using System;
using System.Text;
using System.Linq;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class ShebaPaymentRequest : PodiumRequest<ShebaPaymentResult, ShebaPaymentResultInfo, ShebaPaymentRequestInfo>
    {
        private const string apiKey = "9dd495943cea4dcd8380419d566b1b04";
        private const string productId = "1076566";
        private const string sourceDepositNumber = "3902.800.97374.1";
        private const string sourceSheba = "IR440570390280000097374001";

        private readonly ShebaPaymentRequestInfo request;
        public ShebaPaymentRequest(PayaPaymentRequestDTO requestDTO)
        {
            request = new ShebaPaymentRequestInfo()
            {
                SourceDepNum = sourceDepositNumber,
                DestinationIban = requestDTO.DestSheba,
                RecieverFullName = $"{requestDTO.DestFirstName} {requestDTO.DestLastName}",
                Amount = requestDTO.Amount,
                SrcComment = requestDTO.SourceComment,
                DestComment = requestDTO.DestComment,
                DetailType = (int)requestDTO.CentralBankTransferDetailType,
                senderReturnDepositNumber = sourceDepositNumber,
                TransactionDate = DateTimeUtility.GregorianToPersianDateWithSlash(DateTime.Now),
                TransactionId = requestDTO.TransactionId,
                TransactionBillNumber = requestDTO.PaymentId.ToString(),
                SourceTMBillNumber = $"*{requestDTO.PaymentId}",
                Description = "انتقال وجه خارجی",
                CustomerNumber = "",
                DestBankCode = "-1"
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