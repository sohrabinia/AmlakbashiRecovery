using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.ReserveDTOs
{
    public class ReserveGuestReceiptDTO
    {
        public Reserve Reserve { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public long PayedPrice { get; set; }
        public long PayablePrice { get; set; }
    }
}
