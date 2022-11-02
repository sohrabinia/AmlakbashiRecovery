using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    public class TagInputDTO
    {
        public Dictionary<int, string> TagsDic { get; set; }

        public static implicit operator TagInputDTO(TagPart part)
        {
            TagInputDTO dto = null;
            if (part != null)
            {
                dto = new TagInputDTO();
                dto.TagsDic = part.TagsDic;
            }
            return dto;
        }
    }
}
