using Amlakbashi.Core.Base.Builder;

namespace Amlakbashi.Core.Infrastructure.AdvertiseBuilder.Parts
{
    public class ReservePart : IPart
    {
        public int MaxInstantReserveStartTimeInterval { get; set; }
        public int MinReserveDuration { get; set; }
        public int MaxReserveDuration { get; set; }
    }
}
