using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class CreateAdvertiseGeneralEvent : INotification
    {
        public long advertiseId { get; set; }
        public bool IsAdmin { get; set; }
        public CreateAdvertiseGeneralEvent(long advertiseId, bool isAdmin = false)
        {
            this.advertiseId = advertiseId;
            this.IsAdmin = isAdmin;
        }
    }
}
