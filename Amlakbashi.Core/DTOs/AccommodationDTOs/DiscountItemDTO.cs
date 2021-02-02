using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class DiscountItemDTO
    {
        public int id { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int percent { get; set; }
    }
}
