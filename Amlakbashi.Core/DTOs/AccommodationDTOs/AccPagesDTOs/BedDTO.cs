using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class BedDTO
    {
        public int SingleBedCount { get; set; }
        public int DoubleBedCount { get; set; }
        public int BlanketAndMattressCount { get; set; }
        public ExtraBlanketCountItems ExtraBlanketCount { get; set; }
    }
}
