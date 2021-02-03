using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Mediator.Commands.ReserveCommands
{
    public class ReserveAddHandleCommand : IRequest
    {
        public long ReserveId { get; set; }
        public ReserveAddHandleCommand(long reserveId)
        {
            ReserveId = reserveId;
        }
    }
}
