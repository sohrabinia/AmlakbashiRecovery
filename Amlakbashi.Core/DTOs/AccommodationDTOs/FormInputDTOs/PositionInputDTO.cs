using Amlakbashi.Core.Infrastructure.DTOHelpers;
using Amlakbashi.Core.Infrastructure.DTOHelpers.EntitiesDTOHelper;
using System;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class PositionInputDTO
    {
        public PositionType Position { get; set; }
        public List<DTOSelectItem> positionSelectItems { get; set; }
        public PositionInputDTO()
        {
            positionSelectItems = AccDTOHelper.GenerateAccSelectList<PositionType>();
        }
    }
}
