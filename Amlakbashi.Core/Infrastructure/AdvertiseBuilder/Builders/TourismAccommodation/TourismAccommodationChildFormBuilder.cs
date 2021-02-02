using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.TourismAccommodation
{
    public class TourismAccommodationChildFormBuilder : AdvertiseBuilderBase
    {
        public TourismAccommodationChildFormBuilder() : base(new Product<IPart>())
        {

        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<IdPart>();
            BuildAdvertisePart<AdvertiseTypePart>();
            BuildAdvertisePart<TitleDescPart>();
            BuildAdvertisePart<BedPart>();
            BuildAdvertisePart<CapacityPart>();
            BuildAdvertisePart<HotelUnitSpecificPart>();
            BuildAdvertisePart<PricePart>();
        }
    }
}
