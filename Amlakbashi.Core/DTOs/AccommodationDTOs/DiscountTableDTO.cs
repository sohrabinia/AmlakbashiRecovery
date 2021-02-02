using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class DiscountTableDTO
    {
        public long id { get; set; }
        public List<DiscountItemDTO> discounts { get; set; }
    }
}
