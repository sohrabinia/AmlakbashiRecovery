using MediatR;

namespace Amlakbashi.Mediator.Events.PostEvents
{
    public class DeleteServiceEvent : INotification
    {
        public int ServiceId { get; set; }
        public DeleteServiceEvent(int serviceId)
        {
            ServiceId = serviceId;
        }
    }
}
