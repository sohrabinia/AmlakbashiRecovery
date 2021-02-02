using System;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.FormInputDTOs
{
    [Serializable]
    public class HotelApartmentInputDTO
    {
        public int ApartmentCount { get; set; }
        public int SuitCount { get; set; }
        public int VillaCount { get; set; }
        public int HouseCount { get; set; }
        public int HutCount { get; set; }

    }
}
