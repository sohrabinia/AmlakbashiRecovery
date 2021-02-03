using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.AccountingEvents
{
    public class PrizeCreditUpdateEvent : INotification
    {
        public int UserId { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; }
        public int CurrentUserId { get; set; }

        public PrizeCreditUpdateEvent(
            int userId, ActionLog.ActionSourceEnum actionSource, int currentUserId)
        {
            this.UserId = userId;
            this.ActionSource = actionSource;
            this.CurrentUserId = currentUserId;
        }
    }
}
