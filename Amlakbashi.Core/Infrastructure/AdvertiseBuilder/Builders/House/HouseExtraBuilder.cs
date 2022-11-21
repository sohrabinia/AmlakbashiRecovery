using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.House
{
    public class HouseExtraBuilder : AdvertiseBuilderBase
    {
        public HouseExtraBuilder() : base(new Product<IPart>())
        {
        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<PricePart>();
            BuildAdvertisePart<HygieneProtocolPart>();
            BuildAdvertisePart<BuildingSizePart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<RoomPart>();
            BuildAdvertisePart<ParkingPart>();
            BuildAdvertisePart<BedPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<RulesPart>();
            BuildAdvertisePart<OwnershipPart>();
            BuildAdvertisePart<LicensePart>();
            BuildAdvertisePart<TagPart>();
        }
    }
}
