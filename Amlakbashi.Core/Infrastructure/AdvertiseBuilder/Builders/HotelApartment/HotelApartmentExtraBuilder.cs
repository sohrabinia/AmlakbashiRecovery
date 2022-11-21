using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.HotelApartment
{
    public class HotelApartmentExtraBuilder : AdvertiseBuilderBase
    {
        public HotelApartmentExtraBuilder() : base(new Product<IPart>())
        {
        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<HygieneProtocolPart>();
            BuildAdvertisePart<RulesPart>();
            BuildAdvertisePart<OwnershipPart>();
            BuildAdvertisePart<LicensePart>();
            BuildAdvertisePart<TagPart>();
        }
    }
}
