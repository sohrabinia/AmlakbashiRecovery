using MediatR;

namespace Amlakbashi.Mediator.Events.AccountingEvents
{
    public class SetReservePrizeCreditEvent : INotification
    {
        public long ReserveId { get; set; }
        public long PrizePrice { get; set; }
        public long PrizeCreditId { get; set; }
        public SetReservePrizeCreditEvent(long reserveId, long prizePrice, long prizeCreditId)
        {
            ReserveId = reserveId;
            PrizePrice = prizePrice;
            PrizeCreditId = prizeCreditId;
        }
    }
}
