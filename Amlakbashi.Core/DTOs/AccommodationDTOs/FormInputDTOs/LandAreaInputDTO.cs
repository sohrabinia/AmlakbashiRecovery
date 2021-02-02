using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class LandAreaInputDTO
    {
        public int LandArea { get; set; }
        public bool mandatory { get; set; }

        public LandAreaInputDTO(bool mandatory)
        {
            this.mandatory = mandatory;
        }

        public static implicit operator LandAreaInputDTO(LandAreaPart part)
        {
            LandAreaInputDTO dto = null;
            if (part != null)
            {
                dto = new LandAreaInputDTO(false);
                PropertyCopier<LandAreaPart, LandAreaInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
