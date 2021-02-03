using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.AccountingEvents
{
    public class PresentorPrizeGivenEvent : INotification
    {
        public int UserId { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; }
        public int DoerUserId { get; set; }

        public PresentorPrizeGivenEvent(
            int userId, ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            this.UserId = userId;
            this.ActionSource = actionSource;
            this.DoerUserId = doerUserId;
        }
    }
}
