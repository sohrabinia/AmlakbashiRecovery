using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Base;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Apartment;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Camp;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Complex;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Hotel;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.HotelApartment;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.House;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Hut;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Inn;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Pansion;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Suit;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.TourismAccommodation;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Builders.Villa;
using Amlakbashi.Core.Entities;
using System;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder
{
    public static class DirectorData
    {
        public static AdvertiseBuilderBase GenerateBuilder(Advertise data, DirectorType type)
        {
            switch (type)
            {
                case DirectorType.Basic:
                    return new BasicBuilder();
                case DirectorType.General:
                    return GenerateGeneralBuilder(data);
                case DirectorType.Extra:
                    return GenerateExtraBuilder(data);
                case DirectorType.AdvertisePage:
                    return GenerateAdvertisePageBuilder(data);
                case DirectorType.AdvertisePageChild:
                    return GenerateAdvertisePageChildBuilder(data);
                case DirectorType.HotelUnit:
                    return GenerateHotelChildBuilder(data);
                case DirectorType.ComplexUnit:
                    return GenerateComplexChildBuilder(data);
                default:
                    return null;
            }
        }

        private static AdvertiseBuilderBase GenerateAdvertisePageBuilder(Advertise data)
        {
            switch ((AdvertiseType)data.TypeID)
            {
                case AdvertiseType.Apartment:
                    return new ApartmentBuilder();
                case AdvertiseType.Villa:
                    return new VillaBuilder();
                case AdvertiseType.Hotel:
                    return new HotelBuilder();
                case AdvertiseType.SuitAndRoom:
                    return new SuitBuilder();
                case AdvertiseType.House:
                    return new HouseBuilder();
                case AdvertiseType.Camp:
                    return new CampBuilder();
                case AdvertiseType.TourismAccommodation:
                    return new TourismAccommodationBuilder();
                case AdvertiseType.HotelApartment:
                    return new HotelApartmentBuilder();
                case AdvertiseType.Inn:
                    return new InnBuilder();
                case AdvertiseType.Pansion:
                    return new PansionBuilder();
                case AdvertiseType.Complex:
                    return new ComplexBuilder();
                case AdvertiseType.Hut:
                    return new HutBuilder();
                default:
                    return null;
            }
        }

        private static AdvertiseBuilderBase GenerateAdvertisePageChildBuilder(Advertise data)
        {
            switch ((AdvertiseType)data.TypeID)
            {
                case AdvertiseType.Apartment:
                    return new ApartmentChildBuilder();
                case AdvertiseType.Villa:
                    return new VillaChildBuilder();
                case AdvertiseType.Hotel:
                    return new HotelChildBuilder();
                case AdvertiseType.SuitAndRoom:
                    return new SuitChildBuilder();
                case AdvertiseType.House:
                    return new HouseChildBuilder();
                case AdvertiseType.Camp:
                    return new CampChildBuilder();
                case AdvertiseType.TourismAccommodation:
                    return new TourismAccommodationChildBuilder();
                case AdvertiseType.Inn:
                    return new InnChildBuilder();
                case AdvertiseType.Pansion:
                    return new PansionChildBuilder();
                case AdvertiseType.Hut:
                    return new HutChildBuilder();
                default:
                    return null;
            }
        }

        private static AdvertiseBuilderBase GenerateGeneralBuilder(Advertise data)
        {
            switch ((AdvertiseType)data.TypeID)
            {
                case AdvertiseType.Apartment:
                    return new ApartmentGeneralBuilder();
                case AdvertiseType.Villa:
                    return new VillaGeneralBuilder();
                case AdvertiseType.Hotel:
                    return new HotelGeneralBuilder();
                case AdvertiseType.SuitAndRoom:
                    return new SuitGeneralBuilder();
                case AdvertiseType.House:
                    return new HouseGeneralBuilder();
                case AdvertiseType.Camp:
                    return new CampGeneralBuilder();
                case AdvertiseType.TourismAccommodation:
                    return new TourismAccommodationGeneralBuilder();
                case AdvertiseType.HotelApartment:
                    return new HotelApartmentGeneralBuilder();
                case AdvertiseType.Inn:
                    return new InnGeneralBuilder();
                case AdvertiseType.Pansion:
                    return new PansionGeneralBuilder();
                case AdvertiseType.Complex:
                    return new ComplexGeneralBuilder();
                case AdvertiseType.Hut:
                    return new HutGeneralBuilder();
                default:
                    return null;
            }
        }
        private static AdvertiseBuilderBase GenerateExtraBuilder(Advertise data)
        {
            switch ((AdvertiseType)data.TypeID)
            {
                case AdvertiseType.Apartment:
                    return new ApartmentExtraBuilder();
                case AdvertiseType.Villa:
                    return new VillaExtraBuilder();
                case AdvertiseType.Hotel:
                    return new HotelExtraBuilder();
                case AdvertiseType.SuitAndRoom:
                    return new SuitExtraBuilder();
                case AdvertiseType.House:
                    return new HouseExtraBuilder();
                case AdvertiseType.Camp:
                    return new CampExtraBuilder();
                case AdvertiseType.TourismAccommodation:
                    return new TourismAccommodationExtraBuilder();
                case AdvertiseType.HotelApartment:
                    return new HotelApartmentExtraBuilder();
                case AdvertiseType.Inn:
                    return new InnExtraBuilder();
                case AdvertiseType.Pansion:
                    return new PansionExtraBuilder();
                case AdvertiseType.Complex:
                    return new ComplexExtraBuilder();
                case AdvertiseType.Hut:
                    return new HutExtraBuilder();
                default:
                    return null;
            }
        }

        private static AdvertiseBuilderBase GenerateHotelChildBuilder(Advertise data)
        {
            switch ((AdvertiseType)data.TypeID)
            {
                case AdvertiseType.Hotel:
                    return new HotelChildFormBuilder();
                case AdvertiseType.Camp:
                    return new CampChildFormBuilder();
                case AdvertiseType.TourismAccommodation:
                    return new TourismAccommodationChildFormBuilder();
                case AdvertiseType.Inn:
                    return new InnChildFormBuilder();
                case AdvertiseType.Pansion:
                    return new PansionChildFormBuilder();
                default:
                    return null;
            }
        }

        private static AdvertiseBuilderBase GenerateComplexChildBuilder(Advertise data)
        {
            switch ((AdvertiseType)data.TypeID)
            {
                case AdvertiseType.None:
                case AdvertiseType.Apartment:
                    return new ApartmentChildFormBuilder();
                case AdvertiseType.SuitAndRoom:
                    return new SuitChildFormBuilder();
                case AdvertiseType.House:
                    return new HouseChildFormBuilder();
                case AdvertiseType.Villa:
                    return new VillaChildFormBuilder();
                case AdvertiseType.Hut:
                    return new HutChildFormBuilder();
                default:
                    return null;
            }
        }
    }
}
