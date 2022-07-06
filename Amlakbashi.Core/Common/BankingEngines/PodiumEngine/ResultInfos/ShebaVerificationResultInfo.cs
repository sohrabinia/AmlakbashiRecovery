using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base;
using System;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.ResultInfos
{
    [Serializable]
    public class ShebaVerificationResultInfo : PodiumResultInfo
    {
        public string referenceNumber { get; set; }
        public string accountNumber { get; set; }
        public string accountStatus { get; set; }
        public string accountComment { get; set; }
        public int paymentCode { get; set; }
        public int paymentCodeValid { get; set; }
        public string iban { get; set; }
        public BankAccountOwner[] ibanAccountOwnerList { get; set; }
        public PodiumBankInfo bankInfo { get; set; }
    }
}