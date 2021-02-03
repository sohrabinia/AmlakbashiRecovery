using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateAdvertiseOccupiedCommand : IRequest
    {
        public long advertiseId { get; set; }
        public UpdateAdvertiseOccupiedCommand(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
