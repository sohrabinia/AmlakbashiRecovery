using MediatR;

namespace Amlakbashi.Mediator.Events.ReserveEvents
{
    public class ReserveRequestEvent : INotification
    {
        public long reserveId { get; set; }

        public ReserveRequestEvent(long reserveId)
        {
            this.reserveId = reserveId;
        }
    }
}
