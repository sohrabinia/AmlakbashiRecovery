using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.DTOs.AccommodationDTOs.AccPagesDTOs
{
    public class ReserveDTO
    {
        public int MaxInstantReserveStartTimeInterval { get; set; }
        public int MinReserveDuration { get; set; }
        public int MaxReserveDuration { get; set; }
    }
}
