using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class ExtraFormDTO
    {
        public long Id { get; set; }
        public PriceInputDTO price { get; set; }
        public HygieneProtocolInputDTO hygieneProtocol { get; set; }
        public BuildingSizeInputDTO buildingSize { get; set; }
        public LandAreaInputDTO landArea { get; set; }
        public CapacityInputDTO capacity { get; set; }
        public RoomInputDTO room { get; set; }
        public ParkingInputDTO parking { get; set; }
        public BedInputDTO bed { get; set; }
        public AmenitiesInputDTO amenities { get; set; }
        public ElevatorInputDTO elevator { get; set; }
        public RulesInputDTO rules { get; set; }
        public OwnershipInputDTO ownership { get; set; }
        public LicenseInputDTO license { get; set; }
        public TagInputDTO tags { get; set; }
        public AdvertiseMode advertiseMode { get; set; }

        public static ExtraFormDTO Generate(AdvertiseDirector director, long id)
        {
            var model = new ExtraFormDTO()
            {
                Id = id
            };
            model.advertiseMode = director.Mode;
            model.amenities = director.GetAdvertisePart<AmenitiesPart>();
            model.bed = director.GetAdvertisePart<BedPart>();
            model.buildingSize = director.GetAdvertisePart<BuildingSizePart>();
            model.capacity = director.GetAdvertisePart<CapacityPart>();
            model.landArea = director.GetAdvertisePart<LandAreaPart>();
            model.ownership = director.GetAdvertisePart<OwnershipPart>();
            model.parking = director.GetAdvertisePart<ParkingPart>();
            model.price = director.GetAdvertisePart<PricePart>();
            model.hygieneProtocol = director.GetAdvertisePart<HygieneProtocolPart>();
            model.room = director.GetAdvertisePart<RoomPart>();
            model.rules = director.GetAdvertisePart<RulesPart>();
            model.elevator = director.GetAdvertisePart<ElevatorPart>();
            model.license = director.GetAdvertisePart<LicensePart>();
            model.tags = director.GetAdvertisePart<TagPart>();
            model.tags.residenceId = id;
            return model;
        }
    }
}
