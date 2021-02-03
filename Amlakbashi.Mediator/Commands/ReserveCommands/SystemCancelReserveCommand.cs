using MediatR;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class SystemCancelReserveCommand : IRequest<bool>
    {
        public long reserveId { get; set; }
        public bool force { get; set; }
        public bool sendSms { get; set; }
        public SystemCancelReserveCommand(long reserveId, bool sendSms, bool force)
        {
            this.reserveId = reserveId;
            this.sendSms = sendSms;
            this.force = force;
        }
    }
}
