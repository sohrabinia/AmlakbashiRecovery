using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.CategoryCommands
{
    public class RemoveCategoryItemCacheCommand : IRequest
    {
        public long AdvertiseId { get; set; }
        public RemoveCategoryItemCacheCommand(long advertiseId)
        {
            this.AdvertiseId = advertiseId;
        }
    }
}
