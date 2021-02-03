using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateAccTidinessRatingCommand : IRequest
    {
        public long advertiseId { get; set; }
        public UpdateAccTidinessRatingCommand(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
