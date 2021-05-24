using MediatR;
using static Amlakbashi.Core.Entities.ActionLog;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Mediator.Commands.ReserveCommands
{
    public class SetReserveStatusCommand : IRequest<bool>
    {
        public long reserveId { get; set; }
        public bool sendSms { get; set; }
        public ReserveStatus status { get; set; }
        public ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public bool force { get; set; }
        public SetReserveStatusCommand(long reserveId, ReserveStatus status,
            bool sendSms, ActionSourceEnum actionSource, int doerUserId, bool force = false)
        {
            this.reserveId = reserveId;
            this.status = status;
            this.sendSms = sendSms;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
            this.force = force;
        }
    }
}
