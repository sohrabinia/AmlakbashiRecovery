using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class AddComplexChildEvent : INotification
    {
        public long childId { get; set; }
        public long parentId { get; set; }
        public AddComplexChildEvent(long childId, long parentId)
        {
            this.childId = childId;
            this.parentId = parentId;
        }
    }
}
