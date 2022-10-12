using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos
{
    public class PodiumBankInfo
    {
        public PodiumBankInfoName name { get; set; }
    }

    public class PodiumBankInfoName
    {
        public string en { get; set; }
        public string fa { get; set; }
    }
}
