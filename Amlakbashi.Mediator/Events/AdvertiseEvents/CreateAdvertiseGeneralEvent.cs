using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class CreateAdvertiseGeneralEvent : INotification
    {
        public long advertiseId { get; set; }
        public CreateAdvertiseGeneralEvent(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
