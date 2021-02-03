using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertiseRulesEvent : INotification
    {
        public long advertiseId { get; set; }
        public ChangeAdvertiseRulesEvent(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
