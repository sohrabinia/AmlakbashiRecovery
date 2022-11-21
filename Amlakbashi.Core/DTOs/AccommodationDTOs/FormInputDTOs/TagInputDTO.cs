using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    public class TagInputDTO
    {
        public long residenceId { get; set; }
        public string typePersianTitle { get; set; }
        public string cityPersianName { get; set; }
        public Dictionary<int, string> TagsDic { get; set; } = new Dictionary<int, string>();

        public static implicit operator TagInputDTO(TagPart part)
        {
            TagInputDTO dto = null;
            if (part != null)
            {
                dto = new TagInputDTO();
                dto.TagsDic = part.TagsDic;
                dto.residenceId = part.Id;
                dto.typePersianTitle = AdvertiseMainLocalization.GetAdvertiseTypePersianNameForUser(part.TypeID);
                dto.cityPersianName = part.RegionCity?.PersianName;
            }
            return dto;
        }
    }
}
