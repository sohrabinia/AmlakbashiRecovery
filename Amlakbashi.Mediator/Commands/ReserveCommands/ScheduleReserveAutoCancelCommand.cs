using MediatR;
using System;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class ScheduleReserveAutoCancelCommand : IRequest
    {
        public long reserveId { get; set; }
        public TimeSpan delay { get; set; }

        //public bool sendSms { get; set; }
        //public bool force { get; set; }

        public ScheduleReserveAutoCancelCommand(long reserveId, TimeSpan delay)
        {
            this.reserveId = reserveId;
            this.delay = delay;
        }
    }
}
