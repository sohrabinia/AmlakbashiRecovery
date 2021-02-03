using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.UserEvents
{
    public class UserUpdateEvent : INotification
    {
        public User PreviusData { get; set; }
        public User CurrentData { get; set; }
        public ActionLog.ActionSourceEnum ActionSource { get; set; }
        public int CurrentUserId { get; set; }

        public UserUpdateEvent(User previeusData, User currentData, ActionLog.ActionSourceEnum actionSource
            , int currentUserId)
        {
            this.PreviusData = previeusData;
            this.CurrentData = currentData;
            this.ActionSource = actionSource;
            this.CurrentUserId = currentUserId;
        }
    }
}
