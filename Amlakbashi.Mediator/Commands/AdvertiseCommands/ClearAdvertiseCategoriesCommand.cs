using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class ClearAdvertiseCategoriesCommand : IRequest
    {
        public long advertiseId { get; private set; }
        public ClearAdvertiseCategoriesCommand(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
