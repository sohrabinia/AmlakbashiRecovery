using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.AccountingEvents
{
    public class CreditTransactionUpdateEvent : INotification
    {
        public int UserId { get; set; }
        public long currentCredit { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; }
        public int CurrentUserId { get; set; }

        public CreditTransactionUpdateEvent(int userId, long currentCredit,
            ActionLog.ActionSourceEnum actionSource, int currentUserId)
        {
            this.UserId = userId;
            this.currentCredit = currentCredit;
            this.ActionSource = actionSource;
            this.CurrentUserId = currentUserId;
        }
    }
}
