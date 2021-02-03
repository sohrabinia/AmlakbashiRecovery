using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateAdvertiseScoreCommand : IRequest
    {
        public long AdvertiseId { get; set; }
        public UpdateAdvertiseScoreCommand(long advertiseId)
        {
            this.AdvertiseId = advertiseId;
        }
    }
}
