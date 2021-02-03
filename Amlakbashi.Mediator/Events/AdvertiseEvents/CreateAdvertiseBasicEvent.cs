using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class CreateAdvertiseBasicEvent : INotification
    {
        public long advertiseId { get; set; }
        public int userId { get; set; }
        public CreateAdvertiseBasicEvent(long advertiseId, int userId)
        {
            this.advertiseId = advertiseId;
            this.userId = userId;
        }
    }
}
