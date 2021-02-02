using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Complex
{
    public class ComplexExtraBuilder : AdvertiseBuilderBase
    {
        public ComplexExtraBuilder() : base(new Product<IPart>())
        {
        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<RulesPart>();
            BuildAdvertisePart<OwnershipPart>();
        }
    }
}
