using Amlakbashi.Core.Entities;
using MediatR;

namespace Amlakbashi.Mediator.Events.UserEvents
{
    public class ChangeInstantReserveAccessCommand : IRequest
    {
        public int userId { get; set; }
        public User.InstantReserveAccessEnum instantReserveAccess { get; set; }
        public int doerUserId { get; set; }
        public ActionLog.ActionSourceEnum actionSource { get; set; }
        public ChangeInstantReserveAccessCommand(int userId,
            User.InstantReserveAccessEnum instantReserveAccess,
            int doerUserId, ActionLog.ActionSourceEnum actionSource)
        {
            this.userId = userId;
            this.instantReserveAccess = instantReserveAccess;
            this.doerUserId = doerUserId;
            this.actionSource = actionSource;
        }
    }
}
