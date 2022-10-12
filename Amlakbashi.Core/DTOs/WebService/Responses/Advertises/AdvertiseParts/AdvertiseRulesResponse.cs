using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Responses.Advertises.AdvertiseParts
{
    public class AdvertiseRulesResponse
    {
        public bool party { get; set; }
        public bool pets { get; set; }
        public bool smoking { get; set; }
        public string otherRules { get; set; }
        public string requiredEvidences { get; set; }
        public string reserveCancellationLevel { get; set; } = "متعادل";
        public IList<string> reserveCancellationRules { get; set; }
        public IList<string> nowruzReserveCancellationRules { get; set; }
    }
}
