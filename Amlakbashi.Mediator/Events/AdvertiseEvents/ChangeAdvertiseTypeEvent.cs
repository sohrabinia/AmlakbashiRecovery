using MediatR;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertiseTypeEvent : INotification
    {
        public long advertiseId { get; set; }
        public AdvertiseType prevType { get; set; }
        public ChangeAdvertiseTypeEvent(long advertiseId, AdvertiseType prevType)
        {
            this.advertiseId = advertiseId;
            this.prevType = prevType;
        }
    }
}
