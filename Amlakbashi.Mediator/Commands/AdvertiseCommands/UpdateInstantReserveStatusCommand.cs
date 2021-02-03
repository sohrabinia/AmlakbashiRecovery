using MediatR;
using static Amlakbashi.Core.Entities.ActionLog;
using static Amlakbashi.Core.Entities.Advertise;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateInstantReserveStatusCommand : IRequest
    {
        public long advertiseId { get; set; }
        public InstantReserveStatusEnum status { get; set; }
        public int doerUserId { get; set; }
        public ActionSourceEnum actionSource { get; set; }

        public UpdateInstantReserveStatusCommand(long advertiseId,
            InstantReserveStatusEnum status, int doerUserId,
            ActionSourceEnum actionSource)
        {
            this.advertiseId = advertiseId;
            this.status = status;
            this.doerUserId = doerUserId;
            this.actionSource = actionSource;
        }
    }
}
