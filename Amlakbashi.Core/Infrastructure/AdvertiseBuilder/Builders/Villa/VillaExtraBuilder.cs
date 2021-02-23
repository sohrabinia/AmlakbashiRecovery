using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Villa
{
    public class VillaExtraBuilder : AdvertiseBuilderBase
    {
        public VillaExtraBuilder() : base(new Product<IPart>())
        {
        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<PricePart>();
            BuildAdvertisePart<HygieneProtocolPart>();
            BuildAdvertisePart<BuildingSizePart>();
            BuildAdvertisePart<LandAreaPart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<RoomPart>();
            BuildAdvertisePart<ParkingPart>();
            BuildAdvertisePart<BedPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<RulesPart>();
            BuildAdvertisePart<OwnershipPart>();
        }
    }
}
