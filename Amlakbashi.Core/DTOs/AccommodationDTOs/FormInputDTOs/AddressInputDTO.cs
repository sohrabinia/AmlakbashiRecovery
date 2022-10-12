using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class AddressInputDTO
    {
        public int? ProvinceId { get; set; }
        public int? CityId { get; set; }
        public int? AreaId { get; set; }
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
