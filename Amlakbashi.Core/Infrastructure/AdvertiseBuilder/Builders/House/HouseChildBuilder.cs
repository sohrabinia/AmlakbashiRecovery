using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.House
{
    public class HouseChildBuilder : AdvertiseBuilderBase
    {
        public HouseChildBuilder(): base(new Product<IPart>())
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
            BuildAdvertisePart<PhotoPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<BedPart>();
            BuildAdvertisePart<BuildingSizePart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<ParkingPart>();
            BuildAdvertisePart<PricePart>();
            BuildAdvertisePart<RoomPart>();
            BuildAdvertisePart<FloorPart>();
            BuildAdvertisePart<ElevatorPart>();
        }
    }
}
