using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class ReservePart : IPart
    {
        public int MaxInstantReserveStart { get; set; }
        public int MinReserveDays { get; set; }
        public int MaxReserveDays { get; set; }
    }
}
