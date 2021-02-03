using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.UserEvents
{
    public class BankCardUpdateEvent : INotification
    {
        public BankCard PreviusData { get; set; }
        public BankCard CurrentData { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; }
        public int CurrentUserId { get; set; }

        public BankCardUpdateEvent(BankCard previeusData, BankCard currentData, ActionLog.ActionSourceEnum actionSource
            , int currentUserId)
        {
            this.PreviusData = previeusData;
            this.CurrentData = currentData;
            this.ActionSource = actionSource;
            this.CurrentUserId = currentUserId;
        }
    }
}
