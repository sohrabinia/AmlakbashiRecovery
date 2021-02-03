using MediatR;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertiseStatusEvent : INotification
    {
        public long advertiseId { get; set; }
        public AdvertiseStatus prevStatus { get; set; }
        public ChangeAdvertiseStatusEvent(long advertiseId, AdvertiseStatus prevStatus)
        {
            this.advertiseId = advertiseId;
            this.prevStatus = prevStatus;
        }
    }
}
