using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class ScheduleReservePaymentCommand : IRequest
    {
        public long reserveId { get; set; }
        public ScheduleReservePaymentCommand(long reserveId)
        {
            this.reserveId = reserveId;
        }
    }
}
