using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Mediator.Commands.ReserveCommands
{
    public class SendPayReserveCallCommand : IRequest
    {
        public long ReserveId { get; set; }
        public SendPayReserveCallCommand(long reserveId)
        {
            ReserveId = reserveId;
        }
    }
}
