using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeMaxInstantReserveStartEvent : INotification
    {
        public long advertiseId { get; set; }
        public ChangeMaxInstantReserveStartEvent(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
