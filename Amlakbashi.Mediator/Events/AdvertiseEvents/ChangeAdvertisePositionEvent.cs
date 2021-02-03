using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertisePositionEvent : INotification
    {
        public long advertiseId { get; set; }
        public ChangeAdvertisePositionEvent(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
