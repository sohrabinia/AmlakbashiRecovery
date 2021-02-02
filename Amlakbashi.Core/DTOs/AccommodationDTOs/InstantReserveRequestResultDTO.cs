using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    public class InstantReserveRequestResultDTO
    {
        public int status { get; set; }
        public bool needMsg { get; set; }
        public string msg { get; set; }
        public InstantReserveDetailDTO newData { get; set; }
    }
}
