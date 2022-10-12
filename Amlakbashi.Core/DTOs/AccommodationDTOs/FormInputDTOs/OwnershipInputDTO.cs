using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class OwnershipInputDTO
    {
        public Advertise.OwnershipTypeEnum OwnershipType { get; set; }
        public string OwnerPhoneNumber { get; set; }
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
