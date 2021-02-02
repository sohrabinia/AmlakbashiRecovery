using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class CapacityInputDTO
    {
        public int Capacity { get; set; }
        public int MoreThanCapacity { get; set; }

        public static implicit operator CapacityInputDTO(CapacityPart part)
        {
            CapacityInputDTO dto = null;
            if (part != null)
            {
                dto = new CapacityInputDTO();
                PropertyCopier<CapacityPart, CapacityInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
