using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class ResidencePricesInfoDTO
    {
        public int dailyPrice { get; set; }
        public int holidayPrice { get; set; }
        public int peakHolidayPrice { get; set; }
        public int monthlyPrice { get; set; }
        public int extraCapacityPrice { get; set; }
        public int norouzPrice { get; set; }
        public int norouzExtraCapacityPrice { get; set; }
    }
}
