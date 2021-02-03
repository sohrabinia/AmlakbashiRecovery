using MediatR;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Mediator.Events.ReserveEvents
{
    public class ChangeReserveStateEvent : INotification
    {
        public long reserveId { get; set; }
        public ReserveStatus reserveStatus { get; set; }
        public HostResponseEnum hostResponse { get; set; }

        public ChangeReserveStateEvent(long reserveId, ReserveStatus reserveStatus,
            HostResponseEnum hostResponse)
        {
            this.reserveId = reserveId;
            this.reserveStatus = reserveStatus;
            this.hostResponse = hostResponse;
        }
    }
}
