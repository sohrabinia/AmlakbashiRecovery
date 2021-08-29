using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.AdvertiseCommands
{
    public class RemoveAdvertiseCacheCommand : IRequest
    {
        public long AdvertiseId { get; set; }
        public RemoveAdvertiseCacheCommand(long advertiseId)
        {
            this.AdvertiseId = advertiseId;
        }
    }
}
