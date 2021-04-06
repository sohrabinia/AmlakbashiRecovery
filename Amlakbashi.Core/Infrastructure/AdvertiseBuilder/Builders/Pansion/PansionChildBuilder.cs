using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Pansion
{
    public class PansionChildBuilder : AdvertiseBuilderBase
    {
        public PansionChildBuilder() : base(new Product<IPart>())
        {

        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<IdPart>();
            BuildAdvertisePart<AddressPart>();
            BuildAdvertisePart<PositionPart>();
            BuildAdvertisePart<AdvertiseTypePart>();
            BuildAdvertisePart<RulesPart>();
            BuildAdvertisePart<ReservePart>();
            BuildAdvertisePart<TitleDescPart>();
            //BuildAdvertisePart<NorouzPart>();
            BuildAdvertisePart<HotelUnitSpecificPart>();
            BuildAdvertisePart<PricePart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<BedPart>();
        }
    }
}
