using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class OwnershipInputDTO
    {
        public int OwnershipType { get; set; }
        public int OwnerID { get; set; }
        public string OwnerMobile { get; set; }
        public string OwnerFullName { get; set; }

        public static implicit operator OwnershipInputDTO(OwnershipPart part)
        {
            OwnershipInputDTO dto = null;
            if (part != null)
            {
                dto = new OwnershipInputDTO();
                PropertyCopier<OwnershipPart, OwnershipInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
