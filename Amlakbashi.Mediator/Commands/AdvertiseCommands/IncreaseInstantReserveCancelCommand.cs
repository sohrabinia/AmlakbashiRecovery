using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class IncreaseInstantReserveCancelCommand : IRequest
    {
        public long advertiseId { get; set; }

        public IncreaseInstantReserveCancelCommand(long advertiseId)
        {
            this.advertiseId = advertiseId;
        }
    }
}
