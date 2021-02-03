using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertisePriceEvent : INotification
    {
        public long advertiseId { get; set; }
        public ChangeAdvertisePriceEvent(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
