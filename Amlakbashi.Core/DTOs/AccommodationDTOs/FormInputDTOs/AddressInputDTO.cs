using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class AddressInputDTO
    {
        public int? Province { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static implicit operator AddressInputDTO(AddressPart part)
        {
            AddressInputDTO dto = null;
            if (part != null)
            {
                dto = new AddressInputDTO();
                PropertyCopier<AddressPart, AddressInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
