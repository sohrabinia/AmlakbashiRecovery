using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class AccommodationHotelItemDTO
    {
        public AccommodationHotelItemDTO(AdvertiseMode advertiseMode)
        {
            this.advertiseMode = advertiseMode;
            this.AdvertiseType = this.AdvertiseType == null ? new AdvertiseTypeDTO() : this.AdvertiseType;
            this.Address = this.Address == null ? new AddressDTO() : this.Address;
            this.Bed = this.Bed == null ? new BedDTO() : this.Bed;
            this.Capacity = this.Capacity == null ? new CapacityDTO() : this.Capacity;
            this.HotelUnitSpecific = this.HotelUnitSpecific == null ? new HotelUnitSpecificDTO() : this.HotelUnitSpecific;
            this.Norouz = this.Norouz == null ? new NorouzDTO() : this.Norouz;
            this.Position = this.Position == null ? new PositionDTO() : this.Position;
            this.Price = this.Price == null ? new PriceDTO() : this.Price;
            this.Reserve = this.Reserve == null ? new ReserveDTO() : this.Reserve;
            this.Rules = this.Rules == null ? new RulesDTO() : this.Rules;
            this.TitleDesc = this.TitleDesc == null ? new TitleDescDTO() : this.TitleDesc;
        }

        public static implicit operator AccommodationHotelItemDTO(AdvertiseDirector director)
        {
            var dto = new AccommodationHotelItemDTO(director.Mode);
            PropertyCopier<AdvertiseTypePart, AdvertiseTypeDTO>.Copy(director.GetAdvertisePart<AdvertiseTypePart>(), dto.AdvertiseType);
            PropertyCopier<AddressPart, AddressDTO>.Copy(director.GetAdvertisePart<AddressPart>(), dto.Address);
            PropertyCopier<BedPart, BedDTO>.Copy(director.GetAdvertisePart<BedPart>(), dto.Bed);
            PropertyCopier<CapacityPart, CapacityDTO>.Copy(director.GetAdvertisePart<CapacityPart>(), dto.Capacity);
            dto.Norouz = new NorouzDTO();//PropertyCopier<NorouzPart, NorouzDTO>.Copy(director.GetAdvertisePart<NorouzPart>(), dto.Norouz);
            PropertyCopier<PositionPart, PositionDTO>.Copy(director.GetAdvertisePart<PositionPart>(), dto.Position);
            PropertyCopier<PricePart, PriceDTO>.Copy(director.GetAdvertisePart<PricePart>(), dto.Price);
            PropertyCopier<ReservePart, ReserveDTO>.Copy(director.GetAdvertisePart<ReservePart>(), dto.Reserve);
            PropertyCopier<RulesPart, RulesDTO>.Copy(director.GetAdvertisePart<RulesPart>(), dto.Rules);
            PropertyCopier<TitleDescPart, TitleDescDTO>.Copy(director.GetAdvertisePart<TitleDescPart>(), dto.TitleDesc);
            PropertyCopier<HotelUnitSpecificPart, HotelUnitSpecificDTO>.Copy(director.GetAdvertisePart<HotelUnitSpecificPart>(), dto.HotelUnitSpecific);
            dto.Id = director.GetAdvertisePart<IdPart>().Id;
            return dto;
        }

        public long Id { get; set; }
        public AdvertiseMode advertiseMode { get; set; }
        public AdvertiseTypeDTO AdvertiseType { get; set; }
        public AddressDTO Address { get; set; }
        public BedDTO Bed { get; set; }
        public CapacityDTO Capacity { get; set; }
        public HotelUnitSpecificDTO HotelUnitSpecific { get; set; }
        public NorouzDTO Norouz { get; set; }
        public PositionDTO Position { get; set; }
        public PriceDTO Price { get; set; }
        public ReserveDTO Reserve { get; set; }
        public RulesDTO Rules { get; set; }
        public TitleDescDTO TitleDesc { get; set; }
    }
}
