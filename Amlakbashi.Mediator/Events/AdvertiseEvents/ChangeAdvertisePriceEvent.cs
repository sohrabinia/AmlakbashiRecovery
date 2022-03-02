using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeAdvertisePriceEvent : INotification
    {
        public long advertiseId { get; set; }
        public bool changeNorouzPrice { get; set; } = false;
        public ChangeAdvertisePriceEvent(long advertiseId, bool changeNorouzPrice = false)
        {
            this.advertiseId = advertiseId;
            this.changeNorouzPrice = changeNorouzPrice;
        }
    }
}
