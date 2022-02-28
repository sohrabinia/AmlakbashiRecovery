using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    public class LicenseInputDTO
    {
        public bool License { get; set; }
        public long? LicenseFileId { get; set; }
        public string LicenseNumber { get; set; }

        public static implicit operator LicenseInputDTO(LicensePart part)
        {
            LicenseInputDTO dto = null;
            if (part != null)
            {
                dto = new LicenseInputDTO();
                dto.License = part.License;
                dto.LicenseFileId = part.LicenseFileId;
                dto.LicenseNumber = part.LicenseNumber;
            }
            return dto;
        }
    }
}
