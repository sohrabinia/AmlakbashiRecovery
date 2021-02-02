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
    public class ParkingInputDTO
    {
        public ParkingItems Parking { get; set; }
        public List<DTOSelectItem> parkingSelectItems { get; set; }

        public ParkingInputDTO()
        {
            parkingSelectItems = AccDTOHelper.GenerateAccSelectList<ParkingItems>();
        }

        public static implicit operator ParkingInputDTO(ParkingPart part)
        {
            ParkingInputDTO dto = null;
            if (part != null)
            {
                dto = new ParkingInputDTO();
                PropertyCopier<ParkingPart, ParkingInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
