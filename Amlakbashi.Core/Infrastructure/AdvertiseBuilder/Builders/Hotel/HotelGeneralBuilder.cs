using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Hotel
{
    public class HotelGeneralBuilder : AdvertiseBuilderBase
    {
        public HotelGeneralBuilder() : base(new Product<IPart>())
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
