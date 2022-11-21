using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Hotel
{
    public class HotelExtraBuilder : AdvertiseBuilderBase
    {
        public HotelExtraBuilder() : base(new Product<IPart>())
        {
        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<HygieneProtocolPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<RulesPart>();
            BuildAdvertisePart<OwnershipPart>();
            BuildAdvertisePart<LicensePart>();
            BuildAdvertisePart<TagPart>();
        }
    }
}
