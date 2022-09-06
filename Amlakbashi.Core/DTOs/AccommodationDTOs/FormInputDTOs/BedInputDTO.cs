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
    public class BedInputDTO
    {
        public int SingleBedCount { get; set; }
        public int DoubleBedCount { get; set; }
        public int BlanketAndMattressCount { get; set; }
        public ExtraBlanketCountItems ExtraBlanketCount { get; set; }
        public List<DTOSelectItem> extraBlanketSelectItems { get; set; }
        public BedInputDTO()
        {
            extraBlanketSelectItems = AccDTOHelper.GenerateAccSelectList<ExtraBlanketCountItems>();
        }

        public static implicit operator BedInputDTO(BedPart part)
        {
            BedInputDTO dto = null;
            if (part != null)
            {
                dto = new BedInputDTO();
                PropertyCopier<BedPart, BedInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
