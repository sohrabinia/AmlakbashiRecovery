using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Villa
{
    public class VillaChildFormBuilder : AdvertiseBuilderBase
    {
        public VillaChildFormBuilder() : base(new Product<IPart>())
        {

        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<IdPart>();
            BuildAdvertisePart<AdvertiseTypePart>();
            BuildAdvertisePart<TitleDescPart>();
            BuildAdvertisePart<MetaTitleDescPart>();
            BuildAdvertisePart<NorouzPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<BedPart>();
            BuildAdvertisePart<BuildingSizePart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<PricePart>();
            BuildAdvertisePart<LandAreaPart>();
            BuildAdvertisePart<ParkingPart>();
            BuildAdvertisePart<PhotoPart>();
            BuildAdvertisePart<RoomPart>();
            BuildAdvertisePart<FloorPart>();
            BuildAdvertisePart<ElevatorPart>();
        }
    }
}
