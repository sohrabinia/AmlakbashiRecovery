using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class AccommodationHutItemDTO
    {
        public AccommodationHutItemDTO(AdvertiseMode advertiseMode)
        {
            this.advertiseMode = advertiseMode;
            this.AdvertiseType = this.AdvertiseType == null ? new AdvertiseTypeDTO() : this.AdvertiseType;
            this.Address = this.Address == null ? new AddressDTO() : this.Address;
            this.Amenities = this.Amenities == null ? new AmenitiesDTO() : this.Amenities;
            this.Bed = this.Bed == null ? new BedDTO() : this.Bed;
            this.BuildingSize = this.BuildingSize == null ? new BuildingSizeDTO() : this.BuildingSize;
            this.Capacity = this.Capacity == null ? new CapacityDTO() : this.Capacity;
            this.Elevator = this.Elevator == null ? new ElevatorDTO() : this.Elevator;
            this.Floor = this.Floor == null ? new FloorDTO() : this.Floor;
            this.Norouz = this.Norouz == null ? new NorouzDTO() : this.Norouz;
            this.Parking = this.Parking == null ? new ParkingDTO() : this.Parking;
            this.Photo = this.Photo == null ? new PhotoDTO() : this.Photo;
            this.Position = this.Position == null ? new PositionDTO() : this.Position;
            this.Price = this.Price == null ? new PriceDTO() : this.Price;
            this.Reserve = this.Reserve == null ? new ReserveDTO() : this.Reserve;
            this.Rules = this.Rules == null ? new RulesDTO() : this.Rules;
            this.Room = this.Room == null ? new RoomDTO() : this.Room;
            this.TitleDesc = this.TitleDesc == null ? new TitleDescDTO() : this.TitleDesc;
        }

        public static implicit operator AccommodationHutItemDTO(AdvertiseDirector director)
        {
            var dto = new AccommodationHutItemDTO(director.Mode);
            PropertyCopier<AdvertiseTypePart, AdvertiseTypeDTO>.Copy(director.GetAdvertisePart<AdvertiseTypePart>(), dto.AdvertiseType);
            PropertyCopier<AddressPart, AddressDTO>.Copy(director.GetAdvertisePart<AddressPart>(), dto.Address);
            PropertyCopier<AmenitiesPart, AmenitiesDTO>.CopyWithoutCheckType(director.GetAdvertisePart<AmenitiesPart>(), dto.Amenities);
            PropertyCopier<BedPart, BedDTO>.Copy(director.GetAdvertisePart<BedPart>(), dto.Bed);
            PropertyCopier<BuildingSizePart, BuildingSizeDTO>.Copy(director.GetAdvertisePart<BuildingSizePart>(), dto.BuildingSize);
            PropertyCopier<CapacityPart, CapacityDTO>.Copy(director.GetAdvertisePart<CapacityPart>(), dto.Capacity);
            PropertyCopier<ElevatorPart, ElevatorDTO>.CopyWithoutCheckType(director.GetAdvertisePart<ElevatorPart>(), dto.Elevator);
            PropertyCopier<FloorPart, FloorDTO>.Copy(director.GetAdvertisePart<FloorPart>(), dto.Floor);
            dto.Norouz = new NorouzDTO();//PropertyCopier<NorouzPart, NorouzDTO>.Copy(director.GetAdvertisePart<NorouzPart>(), dto.Norouz);
            PropertyCopier<ParkingPart, ParkingDTO>.Copy(director.GetAdvertisePart<ParkingPart>(), dto.Parking);
            PropertyCopier<PhotoPart, PhotoDTO>.Copy(director.GetAdvertisePart<PhotoPart>(), dto.Photo);
            PropertyCopier<PositionPart, PositionDTO>.Copy(director.GetAdvertisePart<PositionPart>(), dto.Position);
            PropertyCopier<PricePart, PriceDTO>.Copy(director.GetAdvertisePart<PricePart>(), dto.Price);
            PropertyCopier<ReservePart, ReserveDTO>.Copy(director.GetAdvertisePart<ReservePart>(), dto.Reserve);
            PropertyCopier<RoomPart, RoomDTO>.Copy(director.GetAdvertisePart<RoomPart>(), dto.Room);
            PropertyCopier<RulesPart, RulesDTO>.Copy(director.GetAdvertisePart<RulesPart>(), dto.Rules);
            PropertyCopier<TitleDescPart, TitleDescDTO>.Copy(director.GetAdvertisePart<TitleDescPart>(), dto.TitleDesc);
            dto.Id = director.GetAdvertisePart<IdPart>().Id;
            return dto;
        }

        public long Id { get; set; }
        public AdvertiseMode advertiseMode { get; set; }
        public AdvertiseTypeDTO AdvertiseType { get; set; }
        public AddressDTO Address { get; set; }
        public AmenitiesDTO Amenities { get; set; }
        public BedDTO Bed { get; set; }
        public BuildingSizeDTO BuildingSize { get; set; }
        public CapacityDTO Capacity { get; set; }
        public ElevatorDTO Elevator { get; set; }
        public FloorDTO Floor { get; set; }
        public NorouzDTO Norouz { get; set; }
        public ParkingDTO Parking { get; set; }
        public PhotoDTO Photo { get; set; }
        public PositionDTO Position { get; set; }
        public PriceDTO Price { get; set; }
        public ReserveDTO Reserve { get; set; }
        public RoomDTO Room { get; set; }
        public RulesDTO Rules { get; set; }
        public TitleDescDTO TitleDesc { get; set; }
    }
}
