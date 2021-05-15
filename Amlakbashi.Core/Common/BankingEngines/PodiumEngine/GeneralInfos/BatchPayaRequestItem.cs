using System;
using System.Collections.Generic;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos
{
    [Serializable]
    public class BatchPayaRequestItem
    {
        public string Amount { get; set; }
        public string BeneficiaryFullName { get; set; }
        public string Description { get; set; }
        public string DestShebaNumber { get; set; }
        public string BillNumber { get; set; }
        public static Dictionary<string,string> GenerateUrlEncodeDict(string arrayName, BatchPayaRequestItem item, int index)
        {
            var dict = new Dictionary<string, string>();
            dict.Add(arrayName + "[" + index + "].Amount", item.Amount);
            dict.Add(arrayName + "[" + index + "].BeneficiaryFullName", item.BeneficiaryFullName);
            dict.Add(arrayName + "[" + index + "].Description", item.Description);
            dict.Add(arrayName + "[" + index + "].DestShebaNumber", item.DestShebaNumber);
            dict.Add(arrayName + "[" + index + "].BillNumber", item.BillNumber);
            return dict;
        }
    }
}