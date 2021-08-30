using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class RemovePhotosByFileIdsCommand : IRequest
    {
        public long AdvertiseId { get; set; }
        public IEnumerable<long> PhotoIds { get; set; }
        public RemovePhotosByFileIdsCommand(long advertiseId, IEnumerable<long> photoIds)
        {
            AdvertiseId = advertiseId;
            PhotoIds = photoIds;
        }
    }
}
