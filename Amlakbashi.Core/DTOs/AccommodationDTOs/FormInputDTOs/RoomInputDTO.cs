using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class RoomInputDTO
    {
        public int RoomCount { get; set; }
        public bool mandatory { get; set; }
        public int minValue { get; set; }
        public bool allowZero { get; set; }
        public RoomInputDTO(bool mandatory, int minValue, bool allowZero)
        {
            this.mandatory = mandatory;
            this.minValue = minValue;
            this.allowZero = allowZero;
        }

        public static implicit operator RoomInputDTO(RoomPart part)
        {
            RoomInputDTO dto = null;
            if (part != null)
            {
                dto = new RoomInputDTO(false, 0, true);
                PropertyCopier<RoomPart, RoomInputDTO>.Copy(part, dto);
            }
            return dto;
        }
    }
}
