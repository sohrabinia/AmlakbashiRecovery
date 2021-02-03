using MediatR;
using static Amlakbashi.Core.Entities.ActionLog;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Mediator.Events.AdvertiseEvents
{
    public class ChangeInstantReserveStatusEvent : INotification
    {
        public long advertiseId { get; set; }
        public int userId { get; set; }
        public InstantReserveStatusEnum newStatus { get; set; }
        public ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public ChangeInstantReserveStatusEvent(long advertiseId, int userId,
            InstantReserveStatusEnum newStatus, ActionSourceEnum actionSource,
            int doerUserId)
        {
            this.advertiseId = advertiseId;
            this.userId = userId;
            this.newStatus = newStatus;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
        }
    }
}
