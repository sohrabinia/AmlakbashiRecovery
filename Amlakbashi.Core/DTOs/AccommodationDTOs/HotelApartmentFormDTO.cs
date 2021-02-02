using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs
{
    [Serializable]
    public class HotelApartmentFormDTO
    {
        public long Id { get; set; }
        public string ParentTitle { get; set; }
        public int ApartmentCount { get; set; }
        public int SuitCount { get; set; }
    }
}
