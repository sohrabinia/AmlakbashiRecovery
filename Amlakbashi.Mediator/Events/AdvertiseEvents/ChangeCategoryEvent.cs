using MediatR;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeCategoryEvent : INotification
    {
        public int categoryId { get; set; }
        public ChangeCategoryEvent(int categoryId)
        {
            this.categoryId = categoryId;
        }
    }
}
