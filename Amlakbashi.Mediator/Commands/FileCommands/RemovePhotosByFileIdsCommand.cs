using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class RemovePhotosByFileIdsCommand : IRequest
    {
        public IEnumerable<long> PhotoIds { get; set; }
        public RemovePhotosByFileIdsCommand(IEnumerable<long> photoIds)
        {
            PhotoIds = photoIds;
        }
    }
}
