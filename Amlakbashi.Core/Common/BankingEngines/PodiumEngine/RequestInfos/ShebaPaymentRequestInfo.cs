using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos
{
    [Serializable]
    public class ShebaPaymentRequestInfo : PodiumRequestInfo
    {
        public string UserName { get; set; }
        public string SourceDepositNumber { get; set; }
        //public string TransferMoneyBillNumber { get; set; }
        public string CentralBankTransferDetailType { get; set; }
        public string FileUniqueIdentifier { get; set; }
        public List<BatchPayaRequestItem> BatchPayaItemInfos { get; set; }
        //public string privateKey { get; set; }
        //public List<string> scVoucherHash { get; set; }
        public string scApiKey { get; set; }

    }
}