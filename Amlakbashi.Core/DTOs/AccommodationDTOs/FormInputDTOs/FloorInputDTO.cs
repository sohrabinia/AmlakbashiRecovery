using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.DTOHelpers;
using Amlakbashi.Core.Infrastructure.DTOHelpers.EntitiesDTOHelper;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class FloorInputDTO
    {
        public FloorItems Floor { get; set; }
        public bool mandatory { get; set; }
        public List<DTOSelectItem> floorSelectItems { get; set; }

        public FloorInputDTO(bool mandatory)
        {
            this.mandatory = mandatory;
            floorSelectItems = AccDTOHelper.GenerateAccSelectList<FloorItems>();
        }

        public static implicit operator FloorInputDTO(FloorPart part)
        {
            FloorInputDTO dto = null;
            if (part != null)
            {
                dto = new FloorInputDTO(false);
                PropertyCopier<FloorPart, FloorInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
