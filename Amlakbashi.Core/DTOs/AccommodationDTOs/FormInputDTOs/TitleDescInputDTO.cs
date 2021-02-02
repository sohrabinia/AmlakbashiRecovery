using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class TitleDescInputDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool mandatory { get; set; }
        public TitleDescInputDTO(bool mandatory)
        {
            this.mandatory = mandatory;
        }

        public static implicit operator TitleDescInputDTO(TitleDescPart part)
        {
            TitleDescInputDTO dto = null;
            if (part != null)
            {
                dto = new TitleDescInputDTO(false);
                PropertyCopier<TitleDescPart, TitleDescInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
