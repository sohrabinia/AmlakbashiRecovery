using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    public class MetaTitleDescInputDTO
    {
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Slug { get; set; }
        public bool mandatory { get; set; }
        public MetaTitleDescInputDTO(bool mandatory)
        {
            this.mandatory = mandatory;
        }

        public static implicit operator MetaTitleDescInputDTO(MetaTitleDescPart part)
        {
            MetaTitleDescInputDTO dto = null;
            if (part != null)
            {
                dto = new MetaTitleDescInputDTO(false);
                PropertyCopier<MetaTitleDescPart, MetaTitleDescInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
