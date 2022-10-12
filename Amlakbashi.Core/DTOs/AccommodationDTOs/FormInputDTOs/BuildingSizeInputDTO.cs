using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class BuildingSizeInputDTO
    {
        public int BuildingArea { get; set; }
        public bool mandatory { get; set; }

        public BuildingSizeInputDTO(bool mandatory)
        {
            this.mandatory = mandatory;
        }

        public static implicit operator BuildingSizeInputDTO(BuildingSizePart part)
        {
            BuildingSizeInputDTO dto = null;
            if (part != null)
            {
                dto = new BuildingSizeInputDTO(false);
                PropertyCopier<BuildingSizePart, BuildingSizeInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
