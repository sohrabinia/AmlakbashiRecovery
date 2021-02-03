using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class AddHotelChildEvent : INotification
    {
        public long childId { get; set; }
        public long parentId { get; set; }
        public AddHotelChildEvent(long childId, long parentId)
        {
            this.childId = childId;
            this.parentId = parentId;
        }
    }
}
