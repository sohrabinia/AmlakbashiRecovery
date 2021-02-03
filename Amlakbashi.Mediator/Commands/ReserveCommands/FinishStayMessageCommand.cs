using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class FinishStayMessageCommand : IRequest
    {
        public long reserveId { get; set; }
        public FinishStayMessageCommand(long reserveId)
        {
            this.reserveId = reserveId;
        }
    }
}
