using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class RulesPart : IPart
    {
        public bool Party { get; set; }
        public bool Pets { get; set; }
        public bool Smoking { get; set; }

        [Important]
        public string RequiredEvidence { get; set; }

        [Important]
        public string OtherRules { get; set; }
    }
}
