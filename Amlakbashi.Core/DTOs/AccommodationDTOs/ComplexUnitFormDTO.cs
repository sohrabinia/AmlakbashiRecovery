using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class ComplexUnitFormDTO
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public Advertise.AdvertiseType ParentType { get; set; }
        public bool Available { get; set; }
        public ComplexTypeInputDTO type { get; set; }
        public TitleDescInputDTO titleAndDesc { get; set; }
        public MetaTitleDescInputDTO metaTitleAndDesc { get; set; }
        public AmenitiesInputDTO amenities { get; set; }
        public BedInputDTO bed { get; set; }
        public BuildingSizeInputDTO buildingSize { get; set; }
        public CapacityInputDTO capacity { get; set; }
        public PriceInputDTO price { get; set; }
        public LandAreaInputDTO landArea { get; set; }
        public ParkingInputDTO parking { get; set; }
        public PhotoInputDTO photo { get; set; }
        public RoomInputDTO room { get; set; }
        public FloorInputDTO floor { get; set; }
        public ElevatorInputDTO elevator { get; set; }

        public ComplexUnitFormDTO(Advertise.AdvertiseType parentType)
        {
            this.ParentType = parentType;
            titleAndDesc = new TitleDescInputDTO(false);
            capacity = new CapacityInputDTO();
            bed = new BedInputDTO();
            price = new PriceInputDTO();
            type = new ComplexTypeInputDTO(parentType);
            amenities = new AmenitiesInputDTO();
            buildingSize = new BuildingSizeInputDTO(false);
            landArea = new LandAreaInputDTO(false);
            parking = new ParkingInputDTO();
            photo = new PhotoInputDTO(false);
            room = new RoomInputDTO(false, 0, true);
            floor = new FloorInputDTO(false);
            elevator = new ElevatorInputDTO();
        }

        public static ComplexUnitFormDTO Generate(AdvertiseDirector director, long id,
            long parentId, Advertise.AdvertiseType parentType)
        {
            var model = new ComplexUnitFormDTO(parentType)
            {
                Id = id,
                ParentId = parentId,
                Available = director.GetAdvertisePart<IdPart>().Available
            };
            PropertyCopier<AdvertiseTypePart, ComplexTypeInputDTO>.Copy(director.GetAdvertisePart<AdvertiseTypePart>(), model.type);
            model.capacity = director.GetAdvertisePart<CapacityPart>();
            model.bed = director.GetAdvertisePart<BedPart>();
            model.titleAndDesc = director.GetAdvertisePart<TitleDescPart>();
            model.metaTitleAndDesc = director.GetAdvertisePart<MetaTitleDescPart>();
            model.price = director.GetAdvertisePart<PricePart>();
            model.amenities = director.GetAdvertisePart<AmenitiesPart>();
            model.landArea = director.GetAdvertisePart<LandAreaPart>();
            model.parking = director.GetAdvertisePart<ParkingPart>();
            model.photo = director.GetAdvertisePart<PhotoPart>();
            model.room = director.GetAdvertisePart<RoomPart>();
            model.floor = director.GetAdvertisePart<FloorPart>();
            model.elevator = director.GetAdvertisePart<ElevatorPart>();
            model.buildingSize = director.GetAdvertisePart<BuildingSizePart>();

            model.photo.accId = id;
            model.photo.accTitle = AdvertiseMainLocalization.PhotoTitle(director.AdvertiseType, director.Mode);
            return model;
        }
    }
}
