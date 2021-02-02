using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class InstantReserveMaxStartDTO
    {
        public long id { get; set; }
        public int maxStart { get; set; }
    }
}
