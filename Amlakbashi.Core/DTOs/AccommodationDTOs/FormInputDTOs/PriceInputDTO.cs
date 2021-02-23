using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts;
using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class PriceInputDTO
    {
        public int id { get; set; }
        public int dailyPrice { get; set; }
        public int holidayPrice { get; set; }
        public int holidayPikePrice { get; set; }
        public int pikeHolidayPrice { get; set; }//just to handle app
        public int moreThanCapacityPrice { get; set; }
        public long rentPrice { get; set; }
        public int norouzPrice { get; set; }
        public int norouzOverCapacityPrice { get; set; }
        public int minValue { get; set; }
        public long minValueMonthly { get; set; }
        public PriceInputDTO()
        {
            minValue = 30000;
            minValueMonthly = 700000;
        }

        public static implicit operator PriceInputDTO(PricePart part)
        {
            PriceInputDTO dto = null;
            if (part != null)
            {
                dto = new PriceInputDTO();
                PropertyCopier<PricePart, PriceInputDTO>.CopyInsensetive(part, dto);
            }
            if (dto != null && part != null)
                dto.pikeHolidayPrice = part.HolidayPikePrice;
            return dto;
        }
    }
}
