using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class ResidenceManualPriceDTO
    {
        public long residenceId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int price { get; set; }
    }
}
