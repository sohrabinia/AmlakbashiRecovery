using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Results;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Requests
{
    public class ShebaPaymentRequest : PodiumRequest<ShebaPaymentResult, ShebaPaymentResultInfo, ShebaPaymentRequestInfo>
    {
        private const string userName = "97374service";
        private const string sourceDepositNumber = "3902.800.97374.1";
        private const string apiKey = "c162c3e83070480b87bd7d1485e45520";
        private const string productId = "35591";
        private CentralBankTransferEnum transferType;
        private readonly List<BatchPayaRequestItem> payItems;
        private readonly string fileUniqueIdentifier;
        //private const string TransferMoneyBillNumber = "";
        //private const string FileUniqueIdentifier = "";
        //private const string privateKey = "";
        //private readonly List<string> scVoucherHash;

        public ShebaPaymentRequest(CentralBankTransferEnum transferType, List<BatchPayaRequestItem> payItems,
            string fileUniqueIdentifier)
        {
            this.transferType = transferType;
            this.payItems = payItems;
            this.fileUniqueIdentifier = fileUniqueIdentifier;
        }
        
        protected override ShebaPaymentRequestInfo GetRequestData()
        {
            return new ShebaPaymentRequestInfo()
            {
                UserName = userName,
                SourceDepositNumber = sourceDepositNumber,
                scApiKey = apiKey,
                CentralBankTransferDetailType = ((int)transferType).ToString(),
                BatchPayaItemInfos = payItems,
                FileUniqueIdentifier = fileUniqueIdentifier,
                //privateKey = "",
                //scVoucherHash = new List<string>(),
                //TransferMoneyBillNumber = "",
            };
        }

        protected override string GetProductId()
        {
            return productId;
        }
    }
}