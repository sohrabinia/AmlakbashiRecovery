using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateAccUserRatingCommand : IRequest
    {
        public long advertiseId { get; set; }
        public UpdateAccUserRatingCommand(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
