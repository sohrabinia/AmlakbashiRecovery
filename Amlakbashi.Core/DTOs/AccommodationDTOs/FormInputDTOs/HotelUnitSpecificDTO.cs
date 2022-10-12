using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class HotelUnitSpecificDTO
    {
        public int UnitCount { get; set; }

        public static implicit operator HotelUnitSpecificDTO(HotelUnitSpecificPart part)
        {
            HotelUnitSpecificDTO dto = new HotelUnitSpecificDTO();
            if (part != null)
            {
                PropertyCopier<HotelUnitSpecificPart, HotelUnitSpecificDTO>
                    .Copy(part, dto);
            }
            return dto;
        }
    }
}
