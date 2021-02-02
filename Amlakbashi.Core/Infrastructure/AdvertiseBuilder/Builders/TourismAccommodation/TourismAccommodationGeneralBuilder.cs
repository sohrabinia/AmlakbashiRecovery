using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.TourismAccommodation
{
    public class TourismAccommodationGeneralBuilder : AdvertiseBuilderBase
    {
        public TourismAccommodationGeneralBuilder() : base(new Product<IPart>())
        {
        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<AddressPart>();
            BuildAdvertisePart<PhotoPart>();
            BuildAdvertisePart<TitleDescPart>();
            BuildAdvertisePart<MetaTitleDescPart>();
        }
    }
}
