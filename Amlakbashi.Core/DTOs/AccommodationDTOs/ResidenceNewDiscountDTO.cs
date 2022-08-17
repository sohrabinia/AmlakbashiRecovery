using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class ResidenceNewDiscountDTO
    {
        public long residenceId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int discount { get; set; }
    }
}
