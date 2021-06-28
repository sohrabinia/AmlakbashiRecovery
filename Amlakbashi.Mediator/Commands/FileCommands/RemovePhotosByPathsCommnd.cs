using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class RemovePhotosByPathsCommnd : IRequest
    {
        public IEnumerable<string> PathList { get; set; }
        public RemovePhotosByPathsCommnd(IEnumerable<string> pathList)
        {
            PathList = pathList;
        }
    }
}
