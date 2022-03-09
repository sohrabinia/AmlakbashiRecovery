using Amlakbashi.Core.Base.Builder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.HotelApartment
{
    public class HotelApartmentBuilder : AdvertiseBuilderBase
    {
        public HotelApartmentBuilder() : base(new Product<IPart>())
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
            //BuildAdvertisePart<NorouzPart>();
            BuildAdvertisePart<PhotoPart>();
            BuildAdvertisePart<MetaTitleDescPart>();
            BuildAdvertisePart<AmenitiesPart>();
            BuildAdvertisePart<ElevatorPart>();
            BuildAdvertisePart<LicensePart>();
        }
    }
}
