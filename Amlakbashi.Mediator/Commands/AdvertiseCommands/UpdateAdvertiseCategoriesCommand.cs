using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class UpdateAdvertiseCategoriesCommand : IRequest
    {
        public long advertiseId { get; private set; }
        public UpdateAdvertiseCategoriesCommand(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
