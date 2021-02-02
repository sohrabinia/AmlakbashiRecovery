using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Apartment
{
    public class ApartmentGeneralBuilder : AdvertiseBuilderBase
    {
        public ApartmentGeneralBuilder() : base(new Product<IPart>())
        {
        }

        protected override void BuildParts()
        {
            BuildAdvertisePart<AddressPart>();
            BuildAdvertisePart<FloorPart>();
            BuildAdvertisePart<PhotoPart>();
            BuildAdvertisePart<TitleDescPart>();
            BuildAdvertisePart<MetaTitleDescPart>();
        }
    }
}
