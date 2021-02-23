using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class HygieneProtocolInputDTO
    {
        public bool? HygieneProtocol { get; set; }

        public static implicit operator HygieneProtocolInputDTO(HygieneProtocolPart part)
        {
            HygieneProtocolInputDTO dto = null;
            if (part != null)
            {
                dto = new HygieneProtocolInputDTO();
                dto.HygieneProtocol = part.HygieneProtocol;
            }
            return dto;
        }
    }
}
