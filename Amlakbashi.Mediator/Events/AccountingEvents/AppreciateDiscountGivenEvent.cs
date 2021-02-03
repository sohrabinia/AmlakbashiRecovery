using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.AccountingEvents
{
    public class AppreciateDiscountGivenEvent : INotification
    {
        public int userId { get; set; }
        public ActionLog.ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public AppreciateDiscountGivenEvent(int userId,
            ActionLog.ActionSourceEnum actionSource, int doerUserId)
        {
            this.userId = userId;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
        }
    }
}
