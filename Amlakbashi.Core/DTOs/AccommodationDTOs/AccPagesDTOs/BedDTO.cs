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
        public int SingleBed { get; set; }
        public int DoublesBed { get; set; }
        public int BlanketsAndMattresses { get; set; }
        public ExtraBlanketCountItems ExtraBlanketCount { get; set; }
    }
}
