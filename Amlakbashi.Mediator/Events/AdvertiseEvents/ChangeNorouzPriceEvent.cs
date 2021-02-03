using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeNorouzPriceEvent : INotification
    {
        public long advertiseId { get; set; }
        public ChangeNorouzPriceEvent(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
