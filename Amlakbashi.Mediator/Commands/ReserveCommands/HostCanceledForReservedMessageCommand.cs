using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class HostCanceledForReservedMessageCommand : IRequest
    {
        public long reserveId { get; set; }
        public HostCanceledForReservedMessageCommand(long reserveId)
        {
            this.reserveId = reserveId;
        }
    }
}
