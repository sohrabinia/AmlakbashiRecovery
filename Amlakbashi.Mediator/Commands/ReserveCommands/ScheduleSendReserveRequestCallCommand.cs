using MediatR;
using System;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class ScheduleSendReserveRequestCallCommand : IRequest<TimeSpan>
    {
        public long ReserveId { get; set; }
        public ScheduleSendReserveRequestCallCommand(long reserveId)
        {
            ReserveId = reserveId;
        }
    }
}
