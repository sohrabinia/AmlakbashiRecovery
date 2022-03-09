using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Apartment
{
    public class ApartmentBuilder : AdvertiseBuilderBase
    {
        public ApartmentBuilder() : base(new Product<IPart>())
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
            BuildAdvertisePart<BedPart>();
            BuildAdvertisePart<BuildingSizePart>();
            BuildAdvertisePart<ElevatorPart>();
            BuildAdvertisePart<FloorPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<ParkingPart>();
            BuildAdvertisePart<PhotoPart>();
            BuildAdvertisePart<PricePart>();
            BuildAdvertisePart<RoomPart>();
            //BuildAdvertisePart<NorouzPart>();
            BuildAdvertisePart<MetaTitleDescPart>();
            BuildAdvertisePart<LicensePart>();
        }
    }
}
