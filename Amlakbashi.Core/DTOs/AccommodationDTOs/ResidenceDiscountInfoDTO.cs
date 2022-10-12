using Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class ResidenceDiscountInfoDTO
    {
        public long residenceId { get; set; }
        public string calendarPrices { get; set; }
        public List<DiscountDTO> discounts { get; set; }
    }
}
