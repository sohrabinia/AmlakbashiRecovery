using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class InstantReserveDetailDTO
    {
        public bool banned { get; set; }
        public InstantReserveStatusEnum status { get; set; }
        public string statusString { get; set; }
        public string statusColor { get; set; }
        public string buttonTitle { get; set; }
    }
}
