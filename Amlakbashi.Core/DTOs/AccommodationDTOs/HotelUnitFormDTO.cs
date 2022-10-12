using Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class HotelUnitFormDTO
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public Advertise.AdvertiseType Type { get; set; }
        public bool Active { get; set; }
        public HotelUnitSpecificDTO hotelUnitSpecific { get; set; }
        public TitleDescInputDTO titleAndDesc { get; set; }
        public CapacityInputDTO capacity { get; set; }
        public BedInputDTO bed { get; set; }
        public PriceInputDTO price { get; set; }

        public HotelUnitFormDTO()
        {
            hotelUnitSpecific = new HotelUnitSpecificDTO();
            titleAndDesc = new TitleDescInputDTO(false);
            capacity = new CapacityInputDTO();
            bed = new BedInputDTO();
            price = new PriceInputDTO();
        }

        public static HotelUnitFormDTO Generate(AdvertiseDirector director, long id, long parentId)
        {
            var model = new HotelUnitFormDTO()
            {
                Id = id,
                ParentId = parentId,
                Type = director.AdvertiseType,
                Active = director.GetAdvertisePart<IdPart>().Active
            };
            model.hotelUnitSpecific = director.GetAdvertisePart<HotelUnitSpecificPart>();
            model.capacity = director.GetAdvertisePart<CapacityPart>();
            model.bed = director.GetAdvertisePart<BedPart>();
            model.titleAndDesc = director.GetAdvertisePart<TitleDescPart>();
            model.price = director.GetAdvertisePart<PricePart>();
            return model;
        }
    }
}
