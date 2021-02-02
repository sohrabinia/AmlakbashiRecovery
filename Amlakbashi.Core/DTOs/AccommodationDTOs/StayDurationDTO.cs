using System;
using System.Collections.Generic;
namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class StayDurationDTO
    {
        public long id { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }
}
