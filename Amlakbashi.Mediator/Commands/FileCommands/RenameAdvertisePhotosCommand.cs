using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class RenameAdvertisePhotosCommand : IRequest
    {
        public long AdvertiseId { get; set; }
        public RenameAdvertisePhotosCommand(long advertiseId)
        {
            AdvertiseId = advertiseId;
        }
    }
}
