using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Inn
{
    public class InnChildFormBuilder : AdvertiseBuilderBase
    {
        public InnChildFormBuilder() : base(new Product<IPart>())
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
