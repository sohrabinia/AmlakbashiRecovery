using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeStayDurationEvent : INotification
    {
        public long advertiseId { get; set; }
        public ChangeStayDurationEvent(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
