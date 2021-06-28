using MediatR;

namespace Amlakbashi.Mediator.Commands.FileCommands
{
    public class SetWatermarkCommand : IRequest<string>
    {
        public long FileId { get; set; }
        public string ServerPath { get; set; }
        public SetWatermarkCommand(long fileId, string serverPath)
        {
            FileId = fileId;
            ServerPath = serverPath;
        }
    }
}
