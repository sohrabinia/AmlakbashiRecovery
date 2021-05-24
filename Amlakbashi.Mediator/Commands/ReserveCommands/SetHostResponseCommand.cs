using MediatR;
using static Amlakbashi.Core.Entities.ActionLog;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class SetHostResponseCommand : IRequest<bool>
    {
        public long reserveId { get; set; }
        public HostResponseEnum hostResponse { get; set; }
        public bool sendSms { get; set; }
        public ActionSourceEnum actionSource { get; set; }
        public int doerUserId { get; set; }
        public bool force { get; set; }

        public SetHostResponseCommand(long reserveId, HostResponseEnum hostResponse,
            bool sendSms, ActionSourceEnum actionSource, int doerUserId, bool force)
        {
            this.reserveId = reserveId;
            this.hostResponse = hostResponse;
            this.sendSms = sendSms;
            this.actionSource = actionSource;
            this.doerUserId = doerUserId;
            this.force = force;
        }
    }
}
