using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Mediator.Commands.ReserveCommands
{
    public class SendReserveRequestCallCommand : IRequest
    {
        public long ReserveId { get; set; }
        public SendReserveRequestCallCommand(long reserveId)
        {
            ReserveId = reserveId;
        }
    }
}
