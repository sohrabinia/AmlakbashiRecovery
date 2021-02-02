using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Suit
{
    public class SuitChildBuilder : AdvertiseBuilderBase
    {
        public SuitChildBuilder() : base(new Product<IPart>())
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
            BuildAdvertisePart<NorouzPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<PhotoPart>();
            BuildAdvertisePart<BedPart>();
            BuildAdvertisePart<BuildingSizePart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<FloorPart>();
            BuildAdvertisePart<ParkingPart>();
            BuildAdvertisePart<PricePart>();
            BuildAdvertisePart<RoomPart>();
            BuildAdvertisePart<ElevatorPart>();
        }
    }
}
