using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class RulesPart : IPart
    {
        public bool AllowParty { get; set; }
        public bool AllowPets { get; set; }
        public bool AllowSmoking { get; set; }

        [Important]
        public string EvidenceRequired { get; set; }

        [Important]
        public string OtherRules { get; set; }
    }
}
