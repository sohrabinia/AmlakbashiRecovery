using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using Amlakbashi.Core.Infrastructure.DTOHelpers;
using Amlakbashi.Core.Infrastructure.DTOHelpers.EntitiesDTOHelper;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class ElevatorInputDTO
    {
        public bool? Elevator { get; set; }
        public DTOCheckbox elevatorCheckbox { get; set; }

        public ElevatorInputDTO()
        {
            elevatorCheckbox = AccDTOHelper.GenerateAccCheckbox(
                Entities.Advertise.Property.Elevator, Elevator);
        }

        public static implicit operator ElevatorInputDTO(ElevatorPart part)
        {
            ElevatorInputDTO dto = null;
            if (part != null)
            {
                dto = new ElevatorInputDTO();
                PropertyCopier<ElevatorPart, ElevatorInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
